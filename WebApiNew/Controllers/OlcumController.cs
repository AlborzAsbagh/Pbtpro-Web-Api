using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dapper;
using Dapper.Contrib.Extensions;
using WebApiNew.App_GlobalResources;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    [MyBasicAuthenticationFilter]
    public class OlcumController : ApiController
    {


        [Route("api/Olcum/ListFiltered")]
        [HttpPost]
        public List<Olcum> OlcumList([FromUri] int kllId, [FromUri] int page, [FromUri] int pageSize, [FromBody] Filtre filtre)
        {
            var util = new Util();
            var prms = new DynamicParameters();
            #region filtre

            var filtreQuery = "";
            if (filtre != null)
            {
                if (filtre.MakineID > 0)
                {
                    prms.Add("MKN_ID", filtre.MakineID);
                    filtreQuery += " AND O.IDO_MAKINE_ID = @MKN_ID";
                }
                if (filtre.LokasyonID > 0)
                {
                    prms.Add("LOK_ID", filtre.LokasyonID);
                    filtreQuery += " AND O.IDO_LOKASYON_ID = @LOK_ID";
                }

                if (filtre.Kelime != "")
                {
                    prms.Add("KELIME", filtre.Kelime);
                    filtreQuery += @" AND  (IDO_TANIM     like '%'+@KELIME+'%' 
                                         OR L.LOK_TANIM   like '%'+@KELIME+'%' 
                                         OR M.MKN_KOD     like '%'+@KELIME+'%' 
                                         OR M.MKN_TANIM   like '%'+@KELIME+'%' 
                                            ) ";
                }

                if (filtre.BasTarih != "" && filtre.BitTarih != "")
                {
                    DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                    DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                    prms.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
                    prms.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
                    filtreQuery += " AND IDO_TARIH BETWEEN  @BAS_TARIH and @BIT_TARIH";
                }
                else if (filtre.BasTarih != "")
                {
                    DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                    prms.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
                    filtreQuery += " AND IDO_TARIH >=  @BAS_TARIH ";
                }
                else if (filtre.BitTarih != "")
                {
                    DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                    prms.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
                    filtreQuery += " AND IDO_TARIH <= @BIT_TARIH ";
                }
            }

            #endregion
            prms.Add("KLL_ID", kllId);
            prms.Add("PAGE", page);
            prms.Add("PAGE_SIZE", pageSize);
            string sql = @"SELECT * FROM (SELECT 
                                                O.*
                                                ,L.*
                                                ,M.*
												,P.*
                                                ,K.KOD_TANIM PBC_BIRIM
                                                ,ROW_NUMBER() OVER (ORDER BY IDO_TARIH DESC, IDO_SAAT DESC) ROW_NUM
                                                FROM orjin.TB_ISEMRI_OLCUM O
                                                LEFT JOIN orjin.TB_LOKASYON L ON L.TB_LOKASYON_ID=O.IDO_LOKASYON_ID
                                                LEFT JOIN orjin.TB_MAKINE M ON M.TB_MAKINE_ID=O.IDO_MAKINE_ID                                                
												LEFT JOIN orjin.TB_PERIYODIK_BAKIM_OLCUM_PARAMETRE P ON O.IDO_OLCUM_PARAMETRE_ID=P.TB_PERIYODIK_BAKIM_OLCUM_PARAMETRE_ID
                                                LEFT JOIN orjin.TB_KOD K ON K.TB_KOD_ID=P.PBC_BIRIM_KOD_ID
                                                 WHERE  orjin.UDF_LOKASYON_YETKI_KONTROL(O.IDO_LOKASYON_ID , @KLL_ID) = 1 " + filtreQuery;

            sql += @"
                                            ) MTABLE WHERE ROW_NUM BETWEEN @PAGE*@PAGE_SIZE+1 AND @PAGE*@PAGE_SIZE+@PAGE_SIZE";
            using (var cnn = util.baglan())
            {
                List<Olcum> listem = cnn.Query<Olcum, Lokasyon, Makine, OlcumParametre, Olcum>(sql, map: (i, l, m, p) =>
                {
                    i.IDO_ACIKLAMA = Util.RemoveRtfFormatting(i.IDO_ACIKLAMA);
                    i.IDO_MAKINE = m;
                    i.IDO_LOKASYON = l;
                    i.IDO_PARAMETRE = p;
                    return i;
                }, splitOn: "TB_LOKASYON_ID,TB_MAKINE_ID,TB_PERIYODIK_BAKIM_OLCUM_PARAMETRE_ID", param: prms).ToList();
                return listem;
            }
        }

        [Route("api/Olcum/ParamList")]
        [HttpGet]
        public List<OlcumParametre> ParamList()
        {
            var util = new Util();

            string sql = @"SELECT *,K.KOD_TANIM PBC_BIRIM FROM orjin.TB_PERIYODIK_BAKIM_OLCUM_PARAMETRE P
                                LEFT JOIN orjin.TB_KOD K ON K.TB_KOD_ID=P.PBC_BIRIM_KOD_ID";
            using (var cnn = util.baglan())
            {
                List<OlcumParametre> listem = cnn.Query<OlcumParametre>(sql).ToList();
                return listem;
            }
        }

        [Route("api/Olcum/Data")]
        [HttpGet]
        public OlcumGirisData OlcumEkleData(int kllId)
        {
            var util = new Util();
            var prms = new DynamicParameters();
            prms.Add("KUL_ID", kllId);
            string sql = @"SELECT *,K.KOD_TANIM PBC_BIRIM FROM orjin.TB_PERIYODIK_BAKIM_OLCUM_PARAMETRE P
                                LEFT JOIN orjin.TB_KOD K ON K.TB_KOD_ID=P.PBC_BIRIM_KOD_ID;
                           SELECT * FROM orjin.TB_LOKASYON WHERE orjin.UDF_LOKASYON_YETKI_KONTROL(TB_LOKASYON_ID,@KUL_ID) = 1;";
            using (var cnn = util.baglan())
            {
                var result = cnn.QueryMultiple(sql, prms);
                return new OlcumGirisData
                {
                    OLCUM_TANIMLARI = result.Read<OlcumParametre>().ToList(),
                    LOKASYONLAR = result.Read<Lokasyon>().ToList()
                };
            }
        }

        [Route("api/Olcum/Save")]
        [HttpPost]
        public Bildirim Post(Olcum entity)
        {
            var result = new Bildirim();
            try
            {
                var util = new Util();
                using (var cnn = util.baglan())
                {
                    if (entity.TB_ISEMRI_OLCUM_ID > 0)
                    {
                        var b = cnn.Update(entity);
                        result.MsgId = b ? Bildirim.MSG_ISLEM_BASARILI : Bildirim.MSG_ISLEM_HATA;
                        result.Durum = b;
                    }
                    else
                    {
                        result.Id = cnn.Insert(entity);
                        result.MsgId = Bildirim.MSG_ISLEM_BASARILI;
                        result.Durum = true;
                    }

                    result.MsgId = Bildirim.MSG_ISLEM_BASARILI;
                    result.Durum = true;
                }
            }
            catch (Exception e)
            {
                result.Durum = false;
                result.Aciklama = String.Format(Localization.errorFormatted, e.Message);
                result.MsgId = Bildirim.MSG_ISLEM_HATA;
                result.HasExtra = true;
                result.Error = true;
            }

            return result;
        }


        [Route("api/Olcum/Delete")]
        [HttpPost]
        public Bildirim Delete(int id)
        {
            var result = new Bildirim();
            var olcum = new Olcum { TB_ISEMRI_OLCUM_ID = id };
            try
            {
                var util = new Util();
                using (var cnn = util.baglan())
                {
                    var exist = cnn.QueryFirst<int>("select count(*) from orjin.TB_ISEMRI_OLCUM WHERE TB_ISEMRI_OLCUM_ID = @ID", new { ID = id }) > 0;

                    if (cnn.Delete(olcum))
                    {
                        result.MsgId = Bildirim.MSG_ISLEM_BASARILI;
                        result.Durum = true;
                        return result;
                    }
                    result.MsgId = exist ? Bildirim.MSG_ISLEM_HATA : Bildirim.MSG_KAYIT_YOK;
                    result.Durum = !exist;

                }
            }
            catch (Exception e)
            {
                result.Durum = false;
                result.Aciklama = String.Format(Localization.errorFormatted, e.Message);
                result.MsgId = Bildirim.MSG_ISLEM_HATA;
                result.HasExtra = true;
                result.Error = true;
            }

            return result;
        }



    }
}
