using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using WebApiNew.Filters;
using WebApiNew.Models;
using Dapper;

namespace WebApiNew.Controllers
{
    [MyBasicAuthenticationFilter]
    public class indexController : ApiController
    {
        private readonly string key = "exp";

        public object Get([FromUri] int ID)
        {
            try
            {

                var onayCounts=new OnayController().GetOnayCounts(ID);
                MainModel entity = new MainModel();
                var prms = new DynamicParameters();
                        prms.Add("USER_ID", ID);
                var util = new Util();
                string mainQuery =
					@" SELECT 
                    (select count(*) from orjin.TB_ISEMRI WHERE ISM_KAPATILDI=1 AND orjin.UDF_ATOLYE_YETKI_KONTROL(ISM_ATOLYE_ID, @USER_ID) = 1 AND orjin.UDF_LOKASYON_YETKI_KONTROL(ISM_LOKASYON_ID, @USER_ID) = 1) AS KIS_EMRI,
                    (select count(*) from orjin.TB_ISEMRI WHERE ISM_KAPATILDI=0 AND orjin.UDF_ATOLYE_YETKI_KONTROL(ISM_ATOLYE_ID, @USER_ID) = 1 AND orjin.UDF_LOKASYON_YETKI_KONTROL(ISM_LOKASYON_ID, @USER_ID) = 1) AS AIS_EMRI,
                    (select count(*) from orjin.TB_IS_TALEBI WHERE (IST_DURUM_ID=0 OR IST_DURUM_ID=1) AND orjin.UDF_LOKASYON_YETKI_KONTROL(IST_BILDIREN_LOKASYON_ID, @USER_ID) = 1 AND orjin.UDF_ATOLYE_YETKI_KONTROL(IST_ATOLYE_GRUP_ID,@USER_ID)=1) AS AIS_TALEP,
                    (select count(*) from orjin.TB_IS_TALEBI WHERE IST_DURUM_ID NOT IN (0,1) AND orjin.UDF_LOKASYON_YETKI_KONTROL(IST_BILDIREN_LOKASYON_ID, @USER_ID) = 1) AS KIS_TALEP,
                    /*(select count(*) from orjin.TB_MAKINE) AS TOPLAM_MAKINE,*/
                    /*(select count(*) from orjin.TB_MAKINE WHERE MKN_AKTIF = 1)AS AKTIF_MAKINE,*/
                    (select count(*) from orjin.VW_STOK_FIS where orjin.UDF_LOKASYON_YETKI_KONTROL(SFS_LOKASYON_ID,@USER_ID) = 1 and SFS_ISLEM_TIP = '09' and SFS_TALEP_DURUM_ID IN (1)) AS BMALZEME_TALEBI,
                    /*(stuff((SELECT ';' + CONVERT(varchar(50), MME_KOD)    FROM {0}.orjin.TB_KULLANICI_MOBIL_MENU tkm left join {0}.orjin.TB_MOBIL_MENU tm on tm.TB_MOBIL_MENU_ID = tkm.KMM_MOBIL_MENU_ID where KMM_GOR= 1 AND KMM_KULLANICI_ID = @USER_ID  FOR XML PATH('')), 1, 1, '')) AS YETKILI_MENULER,*/
                    (select COALESCE((select COALESCE(TB_RESIM_ID,-1) from orjin.TB_RESIM where RSM_VARSAYILAN = 1 AND RSM_REF_GRUP = 'PERSONEL' and RSM_REF_ID = (SELECT KLL_PERSONEL_ID from {0}.orjin.TB_KULLANICI WHERE TB_KULLANICI_ID = @USER_ID)),(select TOP 1 COALESCE(TB_RESIM_ID,-1) from orjin.TB_RESIM where RSM_REF_GRUP = 'PERSONEL' and RSM_REF_ID = (SELECT KLL_PERSONEL_ID from {0}.orjin.TB_KULLANICI WHERE TB_KULLANICI_ID = @USER_ID)))) AS RESIM_ID ";
               
                string yetkiQuery = @"SELECT TB_KULLANICI_YETKI_ID
                                          ,KYT_KULLANICI_ID
                                          ,KYT_YETKI_KOD
	                                      ,YTK.YTK_TANIM KYT_YETKI_TANIM
                                          ,KYT_EKLE
                                          ,KYT_SIL
                                          ,KYT_DEGISTIR
                                          ,KYT_GOR
                                          FROM {0}.orjin.TB_KULLANICI_YETKI 
                                          LEFT JOIN {0}.orjin.TB_YETKI YTK ON KYT_YETKI_KOD = YTK.YTK_KOD 
                                          WHERE KYT_KULLANICI_ID = (SELECT (CASE WHEN K.KLL_ROL_ID > 0 THEN K.KLL_ROL_ID ELSE K.TB_KULLANICI_ID END) 
                                          FROM {0}.orjin.TB_KULLANICI K WHERE K.TB_KULLANICI_ID=@USER_ID) AND KYT_YETKI_KOD IN (1001,4001,10001,2003,5001,6005,1004,4006,5008,2011,3002,9001,1016,1017,2001,1021);";

                string prsquery = @"SELECT * FROM orjin.VW_PERSONEL WHERE TB_PERSONEL_ID = (SELECT KLL_PERSONEL_ID FROM {0}.orjin.TB_KULLANICI WHERE TB_KULLANICI_ID=@USER_ID);";
                string ytkLokQuery = @"SELECT KLT_LOKASYON_ID from {0}.orjin.TB_KULLANICI_LOKASYON where KLT_KULLANICI_ID=@USER_ID and KLT_GOR=1;"; 
                var query= string.Format(mainQuery + yetkiQuery + prsquery + ytkLokQuery, util.GetMasterDbName());
                using (var con = util.baglan())
                {
                    var result=con.QueryMultiple(query,prms);
                    entity = result.ReadFirst<MainModel>();
                    entity.YETKILER = result.Read<Yetki>().ToList();
                    entity.PERSONEL = result.ReadFirstOrDefault<Personel>();
                    entity.YETKILI_LOKASYON_IDLER = result.Read<int>().ToList();
                    entity.TOPLAM_ONAY = onayCounts.TOTAL;
                    entity.KUL_ID = ID;
                    entity.DB_NAME = util.GetDbName();
                    entity.MASTER_DB_NAME = util.GetMasterDbName();

                }
                return entity;
            }

            catch (Exception e)
            {
                return e.Message;
            }
        }

        [Route("api/getAccessCheck")]
        [HttpGet]
        public object GetAccessCheck(int kullId)
        {
            CipherController cc = new CipherController();
            var parametreler = new DynamicParameters();
            var util = new Util();
            util.MasterBaglantisi = true;
            AccessCheck accessCheck = new AccessCheck();
            parametreler.Add("TB_KULLANICI_ID", kullId);
            String fdt;
            using (var cnn=util.baglan())
            {
                int firmaId = cnn.QueryFirstOrDefault<int>(@"select KLL_FIRMA_ID FROM orjin.TB_KULLANICI WHERE TB_KULLANICI_ID=@TB_KULLANICI_ID", parametreler);
                parametreler.Add("TB_FIRMA_ID",firmaId);
                fdt = cnn.QueryFirstOrDefault<string>(@"select ISL_FDT from orjin.TB_FIRMA WHERE TB_FIRMA_ID=@TB_FIRMA_ID", parametreler);
                if (string.IsNullOrEmpty(fdt))
                {
                    if (firmaId < 1) 
                        firmaId = cnn.QueryFirstOrDefault<int>(@"select top 1 TB_FIRMA_ID FROM orjin.TB_FIRMA WHERE ISL_AKTIF = 1");
                    parametreler.Add("TB_FIRMA_ID", firmaId);
                    fdt = cnn.QueryFirstOrDefault<string>(@"select top 1 ISL_FDT from orjin.TB_FIRMA WHERE TB_FIRMA_ID= @TB_FIRMA_ID", parametreler);
                }
                if (string.IsNullOrEmpty(fdt))
                {
                    fdt = cc.encryptText(DateTime.Now.AddDays(60).ToString("dd.MM.yyyy"), key);
                    parametreler.Add("TB_FIRMA_ID", firmaId);
                    parametreler.Add("FDT", fdt);
                    cnn.Execute(@"UPDATE orjin.TB_FIRMA SET ISL_FDT = @FDT WHERE TB_FIRMA_ID=@TB_FIRMA_ID", parametreler);
                }
            }
            util.MasterBaglantisi = false;
            accessCheck.expiration = cc.decryptText(fdt, key);
            return accessCheck;
        }


       
        public class AccessCheck
        {
            public AccessCheck()
            {
                version = "1.5.1";
                isKeyValid = true;
            }
            public string expiration { get; set; }

            public string version { get; set; }

            public bool isKeyValid { get; set; }
        }
    }


}
