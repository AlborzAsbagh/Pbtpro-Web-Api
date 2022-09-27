using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiNew.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using WebApiNew.Filters;

namespace WebApiNew.Controllers
{
    
    [MyBasicAuthenticationFilter]
    public class MknLokasyonLogController : ApiController
    {
        private static readonly int TAB_ONAYLANAN = 1, TAB_ONAY_BEKLEYEN = 0;
        // GET: api/MknLokasyonLog
        public IEnumerable<MknLokasyonLog> Get([FromUri] int page, [FromUri]int pageSize,[FromUri]int mknId)
        {
            Util mtds = new Util();
            using (var cnn=mtds.baglan())
            {
                var start = page * pageSize;
                var end = start + pageSize;
                var sql = @";WITH mTable AS(SELECT A.*,(SELECT TOP 1 MKN_SAYAC_BIRIM FROM orjin.VW_MAKINE WHERE TB_MAKINE_ID = MKL_MAKINE_ID) MKL_SAYAC_BIRIM,ISNULL(D.LOK_TANIM+' / ','')+B.LOK_TANIM MKL_KAYNAK_LOKASYON,ISNULL(E.LOK_TANIM+' / ','')+C.LOK_TANIM MKL_HEDEF_LOKASYON, 
                                                    ROW_NUMBER() OVER (ORDER BY MKL_TARIH DESC, MKL_SAAT DESC) AS RowNum 
                                                    FROM orjin.TB_MAKINE_LOKASYON A
                                                    inner join orjin.TB_LOKASYON B on (B.TB_LOKASYON_ID = MKL_KAYNAK_LOKASYON_ID)
                                                    inner join orjin.TB_LOKASYON C on (C.TB_LOKASYON_ID = MKL_HEDEF_LOKASYON_ID)
                                                    left join orjin.TB_LOKASYON D on (B.LOK_ANA_LOKASYON_ID = D.TB_LOKASYON_ID)
                                                    left join orjin.TB_LOKASYON E on (C.LOK_ANA_LOKASYON_ID = E.TB_LOKASYON_ID)
                                                    WHERE MKL_MAKINE_ID = @MKNID) 
                                                    SELECT * FROM mTable WHERE RowNum>@START AND RowNum <=@END";
                var logs=cnn.Query<MknLokasyonLog>(sql,new {@MKNID=mknId,@START=start,@END=end });
                return logs;
            }
        }

        private string GetFilteredSql(Filtre filtre,ref DynamicParameters dynamicParams,int tab=-1)
        {
            #region sql

            var sql = @";WITH mTable AS(SELECT * ,
                                                    LOWER(ISNULL((SELECT TOP 1 MES_SAYAC_BIRIM FROM orjin.VW_SAYAC WHERE MES_REF_GRUP='MAKINE' AND (MES_VARSAYILAN = 1) AND MES_REF_ID= MKL_MAKINE_ID),
                                                          ISNULL((SELECT TOP 1 MES_SAYAC_BIRIM FROM orjin.VW_SAYAC WHERE MES_REF_GRUP='MAKINE' AND MES_REF_ID= MKL_MAKINE_ID),''))) MKL_SAYAC_BIRIM, 
                                                    ROW_NUMBER() OVER (ORDER BY MKL_TARIH DESC, MKL_SAAT DESC) AS RowNum 
                                                    FROM orjin.VW_MAKINE_LOKASYON  WHERE 1 = 1
                                                    AND (coalesce(MKL_HEDEF_LOKASYON_ID,0) in (-1,0) OR MKL_OLUSTURAN_ID = @KLL_ID OR orjin.UDF_LOKASYON_YETKI_KONTROL(MKL_HEDEF_LOKASYON_ID,@KLL_ID) = 1 ) ";
            if (filtre.LokasyonID > 0)
            {
                dynamicParams.Add("LOKASYON_ID", filtre.LokasyonID);
                sql += " AND MKL_HEDEF_LOKASYON_ID = @LOKASYON_ID";
            }

            if (filtre.MakineID > 0)
            {
                dynamicParams.Add("MKNID", filtre.MakineID);
                sql += " AND MKL_MAKINE_ID = @MKNID";
            }

            if (filtre.durumID > 0)
            {
                dynamicParams.Add("MKL_DURUM_ID", filtre.durumID);
                sql += " AND MKL_DURUM_ID = @MKL_DURUM_ID";
            }

            if (tab > -1)
            {   if(tab==TAB_ONAY_BEKLEYEN)
                {
                    sql += " AND MKL_DURUM_ID = 1";
                }
                else
                {
                    sql += " AND MKL_DURUM_ID > 1";
                }
            }

            if (filtre.BasTarih != "" && filtre.BitTarih != "")
            {
                DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                dynamicParams.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
                dynamicParams.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
                sql += " AND MKL_TARIH BETWEEN  @BAS_TARIH and @BIT_TARIH";
            }
            else if (filtre.BasTarih != "")
            {
                DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                dynamicParams.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
                sql += " AND MKL_TARIH >=  @BAS_TARIH ";
            }
            else if (filtre.BitTarih != "")
            {
                DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                dynamicParams.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
                sql += " AND MKL_TARIH <= @BIT_TARIH ";
            }

            if (!String.IsNullOrWhiteSpace(filtre.Kelime))
            {
                dynamicParams.Add("QUERY", filtre.Kelime);
                sql += @"AND (
                                        MKL_MAKINE_TANIM LIKE '%'+@QUERY+'%' OR
                                        MKL_ACIKLAMA LIKE '%'+@QUERY+'%' OR
                                        MKL_ONAY_ACIKLAMA LIKE '%'+@QUERY+'%' OR
                                        MKL_MAKINE_KODU LIKE '%'+@QUERY+'%' OR
                                        MKL_MAKINE_TANIM LIKE '%'+@QUERY+'%' OR
                                        MKL_KAYNAK_LOKASYON LIKE '%'+@QUERY+'%' OR
                                        MKL_HEDEF_LOKASYON LIKE '%'+@QUERY+'%' OR
                                        MKL_HEDEF_LOKASYON LIKE '%'+@QUERY+'%' OR
                                        MKL_PROJE LIKE '%'+@QUERY+'%' OR
                                        MKL_PERSONEL_ISIM LIKE '%'+@QUERY+'%' OR
                                        MKL_ZIMMET_PERSONEL LIKE '%'+@QUERY+'%' OR
                                        MKL_ONAY_KULLANICI LIKE '%'+@QUERY+'%' OR
                                        MKL_ONAY_DURUMU LIKE '%'+@QUERY+'%'
                                       )";
            }

            sql += @") 
                                                    SELECT * FROM mTable WHERE RowNum > @START AND RowNum <= @END";

            #endregion

            return sql;

        }

        [Route("api/MknLokasyonLog/Filtered")]
        [HttpPost]
        public ResponseModel GetFiltered([FromUri] int userId,[FromUri] int page, [FromUri]int pageSize,[FromBody]Filtre filtre)
        {
            Util mtds = new Util();
            using (var cnn = mtds.baglan())
            {
                var response=new ResponseModel();
                response.Status = true;
                response.Error = false;
                response.Page = page;
                response.PageSize = pageSize;
                var start = page * pageSize;
                var end = start + pageSize;
                var dynamicParams = new DynamicParameters();
                dynamicParams.Add("KLL_ID", userId);
                dynamicParams.Add("START", start);
                dynamicParams.Add("END", end);
                try
                {
                    var sql = GetFilteredSql(filtre, ref dynamicParams);
                    response.Data = cnn.Query<MknLokasyonLog>(sql, dynamicParams);
                }
                catch (Exception e)
                {
                    response.Status = false;
                    response.Error = true;
                    response.Page = page;
                    response.PageSize = pageSize;
                    response.Message = e.Message;
                }
                return response;
            }
        }


        [Route("api/MknLokasyonLog/{tab}/Filtered")]
        [HttpPost]
        public ResponseModel GetTabFiltered([FromUri] int userId,[FromUri] int page, [FromUri]int pageSize,[FromUri]int tab,[FromBody]Filtre filtre)
        {
            Util mtds = new Util();
            using (var cnn = mtds.baglan())
            {
                var response=new ResponseModel();
                response.Status = true;
                response.Error = false;
                response.Page = page;
                response.PageSize = pageSize;
                var start = page * pageSize;
                var end = start + pageSize;
                var dynamicParams = new DynamicParameters();
                dynamicParams.Add("KLL_ID", userId);
                dynamicParams.Add("START", start);
                dynamicParams.Add("END", end);
                try
                {
                    var sql = GetFilteredSql(filtre, ref dynamicParams,tab);
                    response.Data = cnn.Query<MknLokasyonLog>(sql, dynamicParams);
                }
                catch (Exception e)
                {
                    response.Status = false;
                    response.Error = true;
                    response.Page = page;
                    response.PageSize = pageSize;
                    response.Message = e.Message;
                }
                return response;
            }
        }

        // GET: api/MknLokasyonLog/5
        public MknLokasyonLog Get(int id)
        {
            Util mtds = new Util();
            using (var cnn = mtds.baglan())
            {
                var sql = @"SELECT A.*,ISNULL(D.LOK_TANIM+' / ','')+B.LOK_TANIM MKL_KAYNAK_LOKASYON,ISNULL(E.LOK_TANIM+' / ','')+C.LOK_TANIM MKL_HEDEF_LOKASYON
                                                    FROM orjin.TB_MAKINE_LOKASYON A
                                                    inner join orjin.TB_LOKASYON B on (B.TB_LOKASYON_ID = MKL_KAYNAK_LOKASYON_ID)
                                                    inner join orjin.TB_LOKASYON C on (C.TB_LOKASYON_ID = MKL_HEDEF_LOKASYON_ID)
                                                    left join orjin.TB_LOKASYON D on (B.LOK_ANA_LOKASYON_ID = D.TB_LOKASYON_ID)
                                                    left join orjin.TB_LOKASYON E on (C.LOK_ANA_LOKASYON_ID = E.TB_LOKASYON_ID) WHERE A.TB_MAKINE_LOKASYON_ID = @ID";
                var logs = cnn.Query<MknLokasyonLog>(sql, new { @ID = id});
                return logs.FirstOrDefault();
            }
        }

        // POST: api/MknLokasyonLog
        public Bildirim Post([FromBody]MknLokasyonLog value)
        {
            value.MKL_DURUM_ID = MknLokasyonLog.DURUM_ONAY_BEKLEYEN;
            var metodlar = new Util();
            Bildirim bildirim = new Bildirim();
            using (var conn=metodlar.baglan())
            {
                bildirim.Id =  conn.Insert<MknLokasyonLog>(value);
                bildirim.Durum = bildirim.Id > 0;
                bildirim.Aciklama = bildirim.Durum ? "Lokasyon değişikliği başarılı bir şekilde kaydedildi" : "Lokasyon değişikliği kaydedilemedi!";
                bildirim.MsgId = bildirim.Durum ? Bildirim.MSG_LOK_DEG_KAYIT_OK : Bildirim.MSG_LOK_DEG_KAYIT_ERR;
                if (bildirim.Durum)
                {
                   //conn.Execute("UPDATE orjin.TB_MAKINE SET  MKN_LOKASYON_ID = @NEWLOC WHERE  TB_MAKINE_ID = @MKNID ",new {@NEWLOC=value.MKL_HEDEF_LOKASYON_ID,@MKNID=value.MKL_MAKINE_ID });
                }
            }
            return bildirim;
        }

        // PUT: api/MknLokasyonLog/5
        public Bildirim Put(int id, [FromBody]MknLokasyonLog value)
        {
            var metodlar = new Util();
            Bildirim bildirim = new Bildirim();
            using (var conn = metodlar.baglan())
            {
                var mevcutDurum = conn.QueryFirst<int>("SELECT MKL_DURUM_ID FROM orjin.TB_MAKINE_LOKASYON WHERE TB_MAKINE_LOKASYON_ID =  @ID",new {ID=value.TB_MAKINE_LOKASYON_ID});
                bildirim.Durum = conn.Update<MknLokasyonLog>(value);
                bildirim.Aciklama = bildirim.Durum ? "Lokasyon değişikliği başarılı bir şekilde güncellendi" : "Lokasyon değişikliği güncellenemedi!";
                bildirim.MsgId = bildirim.Durum ? Bildirim.MSG_ISLEM_BASARILI : Bildirim.MSG_ISLEM_HATA;
                bildirim.Id = id;
                if (bildirim.Durum)
                {
                    if (mevcutDurum == 1 && value.MKL_DURUM_ID == 2)
                    {
                        conn.Execute(
                            @"UPDATE orjin.TB_MAKINE SET  MKN_LOKASYON_ID = @NEWLOC WHERE  TB_MAKINE_ID = @MKNID; 
                                          UPDATE orjin.TB_ARAC SET ARC_LOKASYON_ID = @NEWLOC WHERE ARC_MAKINE_ID = @MKNID; ",
                            new { @NEWLOC = value.MKL_HEDEF_LOKASYON_ID, @MKNID = value.MKL_MAKINE_ID });
                    }
                }
            }
            return bildirim;
        }
        [Route("api/MknTransferOnay")]
        [HttpPost]
        public Bildirim ConfirmItems(int id,[FromBody]List<MknLokasyonLog> values)
        {
            var metodlar = new Util();
            Bildirim bildirim = new Bildirim();
            using (var conn = metodlar.baglan())
            {
                var idlist = new int[values.Count];
                for (var i = 0; i < values.Count; i++)
                {
                    idlist[i] = values[i].TB_MAKINE_LOKASYON_ID;
                }
                var durumIdleri=conn.Query<int>("SELECT MKL_DURUM_ID FROM orjin.TB_MAKINE_LOKASYON WHERE TB_MAKINE_LOKASYON_ID IN @IDS" ,new {IDS=idlist}).ToArray();
                bildirim.Durum = conn.Update(values);
                bildirim.Aciklama = bildirim.Durum ? "Lokasyon değişikliği başarılı bir şekilde güncellendi" : "Lokasyon değişikliği güncellenemedi!";
                bildirim.MsgId = bildirim.Durum ? Bildirim.MSG_ISLEM_BASARILI : Bildirim.MSG_ISLEM_HATA;
                bildirim.Id = id;

                if (bildirim.Durum)
                {
                    var i = 0;
                    foreach (var value in values)
                    {
                        if (durumIdleri[i] == 1 && value.MKL_DURUM_ID == 2)
                        {
                            conn.Execute(
                                @"UPDATE orjin.TB_MAKINE SET  MKN_LOKASYON_ID = @NEWLOC WHERE  TB_MAKINE_ID = @MKNID; 
                                              UPDATE orjin.TB_ARAC SET ARC_LOKASYON_ID = @NEWLOC WHERE ARC_MAKINE_ID = @MKNID; ",
                                new { @NEWLOC = value.MKL_HEDEF_LOKASYON_ID, @MKNID = value.MKL_MAKINE_ID });
                        }
                    }
                }
            }
            return bildirim;
        }

        // DELETE: api/MknLokasyonLog/5
        public Bildirim Delete(int id)
        {
            var metodlar = new Util();
            Bildirim bildirim = new Bildirim();
            using (var conn=metodlar.baglan())
            {
                /*MknLokasyonLog log = conn.QueryFirstOrDefault<MknLokasyonLog>("SELECT * from orjin.TB_MAKINE_LOKASYON WHERE TB_MAKINE_LOKASYON_ID=@ID",new { @ID = id });
                Makine makine = conn.QueryFirstOrDefault<Makine>("SELECT * from orjin.TB_MAKINE WHERE TB_MAKINE_ID=@ID",new { @ID = log.MKL_MAKINE_ID });
                int lastId = conn.QueryFirstOrDefault<int>("SELECT MAX(TB_MAKINE_LOKASYON_ID) from orjin.TB_MAKINE_LOKASYON WHERE MKL_MAKINE_ID=@MKNID", new {@MKNID=log.MKL_MAKINE_ID });
                */
                var result = conn.Execute("DELETE FROM orjin.TB_MAKINE_LOKASYON WHERE TB_MAKINE_LOKASYON_ID=@ID", new {@ID=id });
                bildirim.Id = id;
                bildirim.Durum = result > 0;
                bildirim.Aciklama=bildirim.Durum? "Makine lokasyon değişim bilgisi başarılı bir şekilde silindi" : "Makine lokasyon değişim bilgisi silinemedi!";
                bildirim.MsgId = bildirim.Durum ? Bildirim.MSG_LOK_DEG_SIL_OK : Bildirim.MSG_LOK_DEG_SIL_ERR;
                /* if (bildirim.Durum && 
                     lastId == id && 
                     log.MKL_HEDEF_LOKASYON_ID == makine.MKN_LOKASYON_ID)
                 {
                     conn.Execute("UPDATE orjin.TB_MAKINE SET  MKN_LOKASYON_ID = @NEWLOC WHERE  TB_MAKINE_ID = @MKNID ", new { @NEWLOC = log.MKL_KAYNAK_LOKASYON_ID, @MKNID = log.MKL_MAKINE_ID });                    
                 }*/
                return bildirim;
            }
        }
    }
}
