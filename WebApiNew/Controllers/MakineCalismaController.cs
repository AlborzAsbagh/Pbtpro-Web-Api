using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.UI;
using System.Windows.Forms;
using WebApiNew.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Ajax.Utilities;
using WebApiNew.App_GlobalResources;
using WebApiNew.Filters;

namespace WebApiNew.Controllers
{
    
    [MyBasicAuthenticationFilter]
    public class MakineCalismaController : ApiController
    {
        [Route("api/getSantiyeCalismaAyarlari")]
        [HttpGet]
        public ResponseModel GetSantiyeCalismaAyar()
        {
            ResponseModel mResponse = new ResponseModel();
            try
            {
                var util = new Util();
                using (var conn = util.baglan())
                {
                    mResponse.Data = conn.Query<SantiyeCalismaAyar>(@"SELECT A.*, K.KOD_TANIM AS SCA_VARSAYILAN_IS_BIRIM FROM orjin.TB_SANTIYE_CALISMA_AYAR A
                                                                        LEFT JOIN orjin.TB_KOD K ON K.TB_KOD_ID = A.SCA_VARSAYILAN_IS_BIRIM_KOD_ID WHERE A.SCA_AKTIF = 1");
                    mResponse.Status = true;
                    mResponse.Error = false;
                }
            }
            catch (Exception e)
            {
                mResponse.Error = true;
                mResponse.Status = false;
                mResponse.Message = string.Format(Localization.GetCalismaAyarListError,e.Message);
                mResponse.Data = null;
            }
            return mResponse;
        }

        [Route("api/MakineCalismaList/{uid}")]
        [HttpPost]
        public ResponseModel Get([FromBody] Filtre filtre, [FromUri] int uid, [FromUri] int page, [FromUri] int pageSize,
            [FromUri] bool sortAsc)
        {
            var from = page * pageSize;
            var to = from + pageSize;

            ResponseModel mResponse = new ResponseModel();
            try
            {
                string sortstr = sortAsc ? "ASC" : "DESC";
                var prms = new DynamicParameters();
                prms.Add("KUL_ID", uid);
                var sql = @";WITH MTABLE AS (SELECT MPJ.*
                    ,(SELECT VAR_TANIM FROM orjin.TB_VARDIYA WHERE TB_VARDIYA_ID=MPJ.MPJ_VARDIYA_KOD_ID) AS MPJ_VARDIYA_TANIM
                    ,(SELECT SCA_ACIKLAMA FROM orjin.TB_SANTIYE_CALISMA_AYAR WHERE TB_SANTIYE_CALISMA_AYAR_ID = MPJ.MPJ_CALISMA_TIP_ID) AS MPJ_CALISMA_TIP_TANIM
                    ,(select COALESCE(TB_RESIM_ID,0) from orjin.TB_RESIM where RSM_VARSAYILAN= 1 AND RSM_REF_GRUP = 'MAKINE_CALISMA_KARTI' and RSM_REF_ID = TB_MAKINE_PUANTAJ_ID) as RSM_VAR_ID
                    ,stuff((SELECT ';' + CONVERT(varchar(11), R.TB_RESIM_ID) FROM orjin.TB_RESIM R WHERE R.RSM_REF_GRUP = 'MAKINE_CALISMA_KARTI' and R.RSM_REF_ID = TB_MAKINE_PUANTAJ_ID FOR XML PATH('')),1,1,'') [RSM_IDS]
                    ,stuff((SELECT '~;~' + CONVERT(varchar(11), D.TB_DOSYA_ID)+'*:*'+D.DSY_DOSYA_UZANTI+'*:*'+D.DSY_TANIM FROM dbo.TB_DOSYA D WHERE D.DSY_REF_GRUP = 'MAKINE_CALISMA_KARTI' and D.DSY_REF_ID = TB_MAKINE_PUANTAJ_ID FOR XML PATH('')),1,3,'') [DOCUMENTS]
                    ,stuff((SELECT '~;~' + CONVERT(varchar(11), O.TB_MAKINE_PUANTAJ_PERSONEL_ID)+'*:*'+P.PRS_ISIM+'*:*'+CONVERT(VARCHAR(50), O.MPP_SURE_SAAT,128)+'*:*'+
					CONVERT(VARCHAR(11),COALESCE((select R.TB_RESIM_ID from orjin.TB_RESIM R where R.RSM_VARSAYILAN = 1 AND R.RSM_REF_GRUP = 'PERSONEL' AND R.RSM_REF_ID = P.TB_PERSONEL_ID),-1))
                            FROM orjin.TB_MAKINE_PUANTAJ_PERSONEL O LEFT JOIN orjin.TB_PERSONEL P ON O.MPP_PERSONEL_ID = P.TB_PERSONEL_ID WHERE O.MPP_MAKINE_PUANTAJ_ID = MPJ.TB_MAKINE_PUANTAJ_ID  FOR XML PATH('')),1,3,'') [OPERATORS]
                            ,ROW_NUMBER() OVER(ORDER BY MPJ_TARIH " + sortstr + @") AS SATIR
                             FROM orjin.VW_MAKINE_PUANTAJ MPJ                         
                             LEFT OUTER JOIN orjin.TB_MAKINE M ON M.TB_MAKINE_ID = MPJ_MAKINE_ID  
                             WHERE  
                             orjin.UDF_LOKASYON_YETKI_KONTROL(M.MKN_LOKASYON_ID,@KUL_ID)= 1 AND 
                             orjin.UDF_ATOLYE_YETKI_KONTROL(M.MKN_ATOLYE_ID,@KUL_ID) = 1 ";

                if (filtre != null)
                {
                    if (!filtre.BasTarih.IsNullOrWhiteSpace())
                    {
                        sql += " AND  MPJ.MPJ_TARIH >= @BAS_TARIH";
                        prms.Add("BAS_TARIH", Convert.ToDateTime(filtre.BasTarih).ToString("yyyy-MM-dd"));
                    }
                    if (!filtre.BitTarih.IsNullOrWhiteSpace())
                    {
                        sql += " AND MPJ.MPJ_TARIH <= @BIT_TARIH";
                        prms.Add("BIT_TARIH", Convert.ToDateTime(filtre.BitTarih).ToString("yyyy-MM-dd"));
                    }
                    if (filtre.LokasyonID > 0)
                    {
                        sql += " AND MPJ.MPJ_SANTIYE_ID=@LOK_ID";
                        prms.Add("LOK_ID", filtre.LokasyonID);
                    }
                    if (filtre.MakineID > 0)
                    {
                        sql += " AND MPJ.MPJ_MAKINE_ID =@MKN_ID";
                        prms.Add("MKN_ID", filtre.MakineID);
                    }
                    if (filtre.isEmriTipId > 0) //Çalışma Tipi
                    {
                        sql += " AND MPJ.MPJ_CALISMA_TIP_ID = @CALIS_ID";
                        prms.Add("CALIS_ID", filtre.isEmriTipId);
                    }
                    if (filtre.ProjeID > 0)
                    {
                        sql += " AND MPJ.MPJ_PROJE_ID = @PROJE_ID";
                        prms.Add("PROJE_ID", filtre.ProjeID);
                    }
                    if (!filtre.Kelime.IsNullOrWhiteSpace())
                    {
                        sql += @" AND 
                             (
                             MPJ.MPJ_KOD LIKE '%'+@ARA+'%' OR
                             MPJ.MPJ_ACIKLAMA LIKE '%'+@ARA+'%' OR
                             MPJ.MPJ_ISTANIM LIKE '%'+@ARA+'%' OR
                             MPJ.MPJ_MAKINE_TANIM LIKE '%'+@ARA+'%' OR
                             MPJ.MPJ_MAKINE_TIPI LIKE '%'+@ARA+'%' OR
                             MPJ.MPJ_MAKINE_MODEL LIKE '%'+@ARA+'%' OR
                             MPJ.MPJ_MAKINE_MARKA LIKE '%'+@ARA+'%' OR
                             M.MKN_PLAKA LIKE '%'+@ARA+'%' OR
                             MPJ.MPJ_PROJE_KOD LIKE '%'+@ARA+'%' OR
                             MPJ.MPJ_PROJE_TANIM LIKE '%'+@ARA+'%' OR
                             MPJ.MPJ_LOKASYON LIKE '%'+@ARA+'%' OR
                             MPJ.MPJ_CALISMA_YERI LIKE '%'+@ARA+'%'
                             )";
                        prms.Add("ARA", filtre.Kelime);
                    }
                }
                sql += ")  SELECT * FROM MTABLE WHERE SATIR > @FROM AND SATIR <= @TO";
                prms.Add("FROM", from);
                prms.Add("TO", to);
                var util = new Util();
                using (var conn = util.baglan())
                {
                    var response = conn.QueryMultiple(sql, prms);
                    var list=response.Read<MakineCalisma>();
                    mResponse.Data = list;
                    mResponse.Status = true;
                    mResponse.Error = false;
                    mResponse.Page = page;
                    mResponse.PageSize = pageSize;
                    mResponse.Count = list.Count();
                }
            }
            catch (Exception e)
            {
                mResponse.Error = true;
                mResponse.Status = false;
                mResponse.Message = string.Format(Localization.GetCalismaListError,e.Message);
                mResponse.Data = null;
            }
            return mResponse;
        }


        [Route("api/MakineCalismaOperatorList/{mpid}")]
        [HttpPost]
        public ResponseModel GetMakineCalismaOperatorList([FromUri] int mpid)
        {

            ResponseModel mResponse = new ResponseModel();
            try
            {
                var prms = new DynamicParameters();
                prms.Add("MPID",mpid);
                var sql = @"SELECT MPP.*,COALESCE((select R.TB_RESIM_ID from orjin.TB_RESIM R where R.RSM_VARSAYILAN = 1 AND R.RSM_REF_GRUP = 'PERSONEL' AND R.RSM_REF_ID = P.TB_PERSONEL_ID),-1) AS MPP_RESIM_ID,P.PRS_ISIM AS MPP_PERSONEL_ISIM,P.* FROM orjin.TB_MAKINE_PUANTAJ_PERSONEL MPP LEFT JOIN orjin.TB_PERSONEL P ON P.TB_PERSONEL_ID=MPP.MPP_PERSONEL_ID WHERE MPP.MPP_MAKINE_PUANTAJ_ID=@MPID";
                var util = new Util();
                using (var conn = util.baglan())
                {
                    var response = conn.Query<MakineCalismaOperator,Personel,MakineCalismaOperator>( sql:sql,map:(mpp, p) =>
                    {
                        mpp.MPP_PERSONEL = p;
                        return mpp;
                    },
                    splitOn:"TB_PERSONEL_ID",param:new {@MPID=mpid});
                    mResponse.Data = response;
                    mResponse.Status = true;
                    mResponse.Error = false;
                    mResponse.Count = response.Count();
                }
            }
            catch (Exception e)
            {
                mResponse.Error = true;
                mResponse.Status = false;
                mResponse.Message = string.Format(Localization.GetCalismaOperatorListError, e.Message);
                mResponse.Data = null;
            }
            return mResponse;
        }

        [Route("api/MakineCalismaKodGetir")]
        public ResponseModel GetMakilneCalismaKod()
        {
            var mResponse = new ResponseModel();
            var util = new Util();
            try
            {
                using (var conn = util.baglan())
                {
                    var sql = "";
                    conn.Execute("UPDATE orjin.TB_NUMARATOR SET NMR_NUMARA = NMR_NUMARA+1 WHERE NMR_KOD = 'MPJ_KOD'");
                    string ss = "";
                    sql = @"SELECT 
                        NMR_ON_EK+RIGHT(replicate('0',NMR_HANE_SAYISI)+CAST(NMR_NUMARA AS VARCHAR(MAX)),NMR_HANE_SAYISI) as deger FROM orjin.TB_NUMARATOR
                        WHERE NMR_KOD = 'MPJ_KOD'";
                    mResponse.Status = true;
                    mResponse.Error = false;
                    mResponse.Data = conn.QueryFirst<String>(sql);
                    return mResponse;
                }
            }
            catch (Exception e)
            {
                mResponse.Error = true;
                mResponse.Status = false;
                mResponse.Message = string.Format(Localization.KodOlusturError, e.Message);
                mResponse.Data = null;
            }
            return mResponse; 
        }

        [Route("api/MakineCalismaKayit/{uid}")]
        [HttpPost]
        public ResponseModel Post([FromUri]int uid,[FromBody]MakineCalisma entity)
        {
            bool isUpdate = entity.TB_MAKINE_PUANTAJ_ID > 0;
            var mResponse=new ResponseModel();
            var util=new Util();
            try
            {
                using (var conn = util.baglan())
                {
                    #region exec version

                    var sql = @"INSERT INTO orjin.TB_MAKINE_PUANTAJ
                                  (
                                MPJ_KOD,
                                MPJ_SANTIYE_ID,
                                MPJ_MAKINE_ID,
                                MPJ_CALISMA_TIPI,
                                MPJ_VARDIYA_KOD_ID,
                                MPJ_BASLANGIC_TARIH,
                                MPJ_TARIH,
                                MPJ_SAAT,
                                MPJ_BITIS_TARIH,
                                MPJ_SURE_BITIS_SAAT,
                                MPJ_SURE_SAAT,
                                MPJ_SURE_DAKIKA,
                                MPJ_FIYAT,
                                MPJ_TUTAR,
                                MPJ_IS_MIKTAR,
                                MPJ_IS_BIRIM_KOD_ID,
                                MPJ_SAYAC_ID,
                                MPJ_SAYAC_BASLANGIC_DEGER,
                                MPJ_SAYAC_BITIS_DEGER,
                                MPJ_SAYAC_FARK,
                                MPJ_OLUSTURAN_ID,
                                MPJ_OLUSTURMA_TARIH,
                                MPJ_DEGISTIREN_ID,
                                MPJ_DEGISTIRME_TARIH
                                )
                                VALUES(
                                @MPJ_KOD,
                                @MPJ_SANTIYE_ID,
                                @MPJ_MAKINE_ID,
                                @MPJ_CALISMA_TIPI,
                                @MPJ_VARDIYA_KOD_ID,
                                @MPJ_BASLANGIC_TARIH,
                                @MPJ_TARIH,
                                @MPJ_SAAT,
                                @MPJ_BITIS_TARIH,
                                @MPJ_SURE_BITIS_SAAT,
                                @MPJ_SURE_SAAT,
                                @MPJ_SURE_DAKIKA,
                                @MPJ_FIYAT,
                                @MPJ_TUTAR,
                                @MPJ_IS_MIKTAR,
                                @MPJ_IS_BIRIM_KOD_ID,
                                @MPJ_SAYAC_ID,
                                @MPJ_SAYAC_BASLANGIC_DEGER,
                                @MPJ_SAYAC_BITIS_DEGER,
                                @MPJ_SAYAC_FARK,
                                @MPJ_OLUSTURAN_ID,
                                @MPJ_OLUSTURMA_TARIH,
                                @MPJ_DEGISTIREN_ID,
                                @MPJ_DEGISTIRME_TARIH)";
                    var prms = new DynamicParameters();
                    prms.Add("MPJ_KOD", entity.MPJ_KOD);
                    prms.Add("MPJ_SANTIYE_ID", entity.MPJ_SANTIYE_ID);
                    prms.Add("MPJ_MAKINE_ID", entity.MPJ_MAKINE_ID);
                    prms.Add("MPJ_CALISMA_TIPI", entity.MPJ_CALISMA_TIPI);
                    prms.Add("MPJ_VARDIYA_KOD_ID", entity.MPJ_VARDIYA_KOD_ID);
                    prms.Add("MPJ_BASLANGIC_TARIH", entity.MPJ_BASLANGIC_TARIH);
                    prms.Add("MPJ_TARIH", entity.MPJ_TARIH);
                    prms.Add("MPJ_SAAT", entity.MPJ_SAAT);
                    prms.Add("MPJ_BITIS_TARIH", entity.MPJ_BITIS_TARIH);
                    prms.Add("MPJ_SURE_BITIS_SAAT", entity.MPJ_SURE_BITIS_SAAT);
                    prms.Add("MPJ_SURE_SAAT", entity.MPJ_SURE_SAAT);
                    prms.Add("MPJ_SURE_DAKIKA", entity.MPJ_SURE_DAKIKA);
                    prms.Add("MPJ_FIYAT", entity.MPJ_FIYAT);
                    prms.Add("MPJ_TUTAR", entity.MPJ_TUTAR);
                    prms.Add("MPJ_IS_MIKTAR", entity.MPJ_IS_MIKTAR);
                    prms.Add("MPJ_IS_BIRIM_KOD_ID", entity.MPJ_IS_BIRIM_KOD_ID);
                    prms.Add("MPJ_SAYAC_ID", entity.MPJ_SAYAC_ID);
                    prms.Add("MPJ_SAYAC_BASLANGIC_DEGER", entity.MPJ_SAYAC_BASLANGIC_DEGER);
                    prms.Add("MPJ_SAYAC_BITIS_DEGER", entity.MPJ_SAYAC_BITIS_DEGER);
                    prms.Add("MPJ_SAYAC_FARK", entity.MPJ_SAYAC_FARK);
                    prms.Add("MPJ_OLUSTURAN_ID", entity.MPJ_OLUSTURAN_ID);
                    prms.Add("MPJ_OLUSTURMA_TARIH", entity.MPJ_OLUSTURMA_TARIH);
                    prms.Add("MPJ_DEGISTIREN_ID", entity.MPJ_DEGISTIREN_ID);
                    prms.Add("MPJ_DEGISTIRME_TARIH", entity.MPJ_DEGISTIRME_TARIH);
                    #endregion

                        var list=new List<MakineCalisma>();
                    if (entity.TB_MAKINE_PUANTAJ_ID < 1)
                    {
                        entity.TB_MAKINE_PUANTAJ_ID=conn.Insert(entity);
                        list.Add(entity);
                        mResponse.Data = list;
                        mResponse.Error = false;
                        mResponse.Status = true;
                        mResponse.Message = Localization.MakineCalismaKartKayitOk;
                        return mResponse;
                    }
                    else
                    {
                        var b=conn.Update(entity);
                        mResponse.Status = b;
                        mResponse.Error = !b;
                        mResponse.Message = b ? Localization.MakineCalismaKartGuncelleOk : 
                            conn.QueryFirst<long>("SELECT COUNT(*) FROM orjin.TB_MAKINE_PUANTAJ WHERE TB_MAKINE_PUANTAJ_ID = @MPID",new {@MPID=entity.TB_MAKINE_PUANTAJ_ID}) >0? Localization.MknCalismaKartKayitDegisiklikYok : 
                                    Localization.MknCalismaKartKayitKayitYok;
                        list.Add(entity);
                        mResponse.Data = list;
                        return mResponse;
                    }
                }
            }
            catch (Exception e)
            {
                mResponse.Data = null;
                mResponse.Error = true;
                mResponse.Status = false;
                mResponse.Message = string.Format(isUpdate?Localization.MakineCalismaKartGuncelleErr:Localization.MakineCalismaKartKayitErr,e.Message);
            }

            return mResponse;
        }


        [Route("api/MakineCalismaPersonelKayit")]
        [HttpPost]
        public ResponseModel MakineCalismaOperatorEkle([FromBody]MakineCalismaOperator entity)
        {
            var isUpdate = entity.TB_MAKINE_PUANTAJ_PERSONEL_ID > 0;
            var mResponse=new ResponseModel();
            var util=new Util();
            try
            {
                using (var conn = util.baglan())
                {
                        var list=new List<MakineCalismaOperator>();
                    if (entity.TB_MAKINE_PUANTAJ_PERSONEL_ID < 1)
                    {
                        entity.MPP_OLUSTURMA_TARIH=DateTime.Now;
                        entity.TB_MAKINE_PUANTAJ_PERSONEL_ID=conn.Insert(entity);
                        list.Add(entity);
                        mResponse.Data = list;
                        mResponse.Error = false;
                        mResponse.Status = true;
                        mResponse.Message = Localization.MknCalismaOperatorKayitOk;
                        return mResponse;
                    }
                    else
                    {
                        var b=conn.Update(entity);
                        mResponse.Status = b;
                        mResponse.Error = !b;
                        mResponse.Message = b ? Localization.MknCalismaOperatorGuncelleOk : 
                            conn.QueryFirst<long>("SELECT COUNT(*) FROM orjin.TB_MAKINE_PUANTAJ_PERSONEL WHERE TB_MAKINE_PUANTAJ_PERSONEL_ID = @MPID", new {@MPID=entity.TB_MAKINE_PUANTAJ_PERSONEL_ID}) >0? Localization.MknOprGuncelleDegYokHata : Localization.MknOprGuncelleKayitYokHata;
                        list.Add(entity);
                        mResponse.Data = list;
                        return mResponse;
                    }
                }
            }
            catch (Exception e)
            {
                mResponse.Data = entity;
                mResponse.Error = true;
                mResponse.Status = false;
                mResponse.Message = string.Format(isUpdate?Localization.MknCalismaOperatorGuncelleHata:Localization.MknCalismaOperatorKayitHata,e.Message);
            }  

            return mResponse;
        }

        [Route("api/MakineCalismaSil/{itemId}")]
        [HttpGet]
        public ResponseModel Delete([FromUri]int itemId)
        {
            var mResponse=new ResponseModel();
            var util=new Util();
            try
            {
                using (var conn = util.baglan())
                {
                    var prm = new {@ID = itemId};
                    var cnt=-1;
                    var result = -1;
                    cnt = conn.QueryFirst<int>(
                        "SELECT COUNT(*) FROM orjin.TB_MAKINE_PUANTAJ WHERE TB_MAKINE_PUANTAJ_ID = @ID", prm);
                    if(cnt>0)
                        result=conn.Execute("DELETE FROM orjin.TB_MAKINE_PUANTAJ WHERE TB_MAKINE_PUANTAJ_ID = @ID", prm);
                        mResponse.Status = result>0;
                        mResponse.Error = cnt<=0;
                        mResponse.Message = result>0?Localization.MknCalismaSilOk:cnt>0 ?  Localization.MknCalismaSilHata:Localization.MknCalismaSilKayitYok;
                        return mResponse;
                    
                }
            }
            catch (Exception e)
            {
                mResponse.Data = null;
                mResponse.Error = true;
                mResponse.Status = false;
                mResponse.Message = string.Format(Localization.MknCalismaSilSunucuHata,e.Message);
            }

            return mResponse;
        }
        [Route("api/MakineCalismaOperatorSil/{itemId}")]
        [HttpGet]
        public ResponseModel DeleteOperator([FromUri]int itemId)
        {
            var mResponse=new ResponseModel();
            var util=new Util();
            var list = new List<MakineCalismaOperator>();
            list.Add(new MakineCalismaOperator() { TB_MAKINE_PUANTAJ_PERSONEL_ID = itemId });
            mResponse.Data = list;
            try
            {
                using (var conn = util.baglan())
                {
                    var prm = new {@ID = itemId};
                    var cnt=-1;
                    var result = -1;
                    cnt = conn.QueryFirst<int>(
                        "SELECT COUNT(*) FROM orjin.TB_MAKINE_PUANTAJ_PERSONEL WHERE TB_MAKINE_PUANTAJ_PERSONEL_ID = @ID", prm);
                    if(cnt>0)
                        result=conn.Execute("DELETE FROM orjin.TB_MAKINE_PUANTAJ_PERSONEL WHERE TB_MAKINE_PUANTAJ_PERSONEL_ID = @ID", prm);
                        mResponse.Status = result>0;
                        mResponse.Error = cnt<=0;
                        mResponse.Message = result>0?Localization.MknOperatorSilOk:cnt>0 ? Localization.MknOperatorSilHata:Localization.MknOperatorSilKayitYok;
                        return mResponse;
                    
                }
            }
            catch (Exception e)
            {
                mResponse.Error = true;
                mResponse.Status = false;
                mResponse.Message = string.Format(Localization.MknOperatorSilSunucuHata,e.Message);
            }

            return mResponse;
        }

    }
}