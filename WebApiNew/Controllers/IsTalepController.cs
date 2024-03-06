using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web.Http;
using Dapper;
using Dapper.Contrib.Extensions;
using WebApiNew.App_GlobalResources;
using WebApiNew.Filters;
using WebApiNew.Models;
using WebApiNew.Utility.Abstract;

namespace WebApiNew.Controllers
{

    [MyBasicAuthenticationFilter]
    public class IsTalepController : ApiController
    {
        private readonly ILogger _logger;
        Util klas = new Util();
        List<Prm> parametreler = new List<Prm>();
        Parametreler prms = new Parametreler();

        public IsTalepController(ILogger logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("api/istalep/TalepListeOld")]
        public List<IsTalep> TalepListeold([FromBody] Filtre filtre, [FromUri] int ilkDeger, [FromUri] int sonDeger, [FromUri] int ID, [FromUri] int durumID, [FromUri] bool sortAsc)
        {
            List<IsTalep> listem = new List<IsTalep>();
            parametreler.Clear();
            parametreler.Add(new Prm("KUL_ID", ID));
            string sort = sortAsc ? "ASC" : "DESC";
            string query = @"SELECT *
            ,(CASE WHEN GUN    > 0 THEN CAST(GUN    AS VARCHAR(5))+ ' g '    ELSE '' END +                                                      
                    CASE WHEN SAAT   > 0 THEN CAST(SAAT   AS VARCHAR(5))+ ' s '   ELSE '' END +                                                      
                    CASE WHEN DAKIKA > 0 THEN CAST(DAKIKA AS VARCHAR(5))+ ' dk ' ELSE '' END ) AS ISLEM_SURE           
            from (select *, 	                                    
                CASE WHEN (IST.IST_DURUM_ID = 4) THEN                                                                                                       
 		            CASE                                                                                                                                       
 	   		            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)/(60 * 24) > 0               
 	    	            THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)/(60 * 24)                   
     	                WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)/(60 * 24) > =0           
 	    	            THEN 0                                                                                                                                 
 	    	            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)/(60 * 24) < 0               
     	                THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)/(60 * 24)                
        	            END    	                                                                                                                             
                    WHEN (IST.IST_DURUM_ID = 5 ) THEN                                                                                                        
     	            0                                                                                                                                       
                    ELSE                                                                                                                                     
 		            CASE                                                                                                                                       
 	   		            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())/(60 * 24) > 0                                             
 	    	            THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())/(60 * 24)                                                 
     	                WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())/(60 * 24) > =0                                         
 	    	            THEN 0                                                                                                                                 
 	    	            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())/(60 * 24) < 0                                             
     	                THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())/(60 * 24)                                              
        	            END    	                                                                                                                             
                    END AS GUN,                                                                                                                              
 	            CASE WHEN (IST.IST_DURUM_ID = 4) THEN                                                                                                       
 		            CASE                                                                                                                                       
 	   		            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% (60 * 24) / 60 > 0         
 	    	            THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% (60 * 24) / 60             
     	                WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% (60 * 24) / 60 > =0     
 	    	            THEN 0                                                                                                                                 
 	    	            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% (60 * 24) / 60 < 0         
     	                THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% (60 * 24) / 60          
        	            END    	                                                                                                                             
                    WHEN (IST.IST_DURUM_ID = 5 ) THEN                                                                                                        
     	            0                                                                                                                                       
                    ELSE                                                                                                                                     
 		            CASE                                                                                                                                       
 	   		            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% (60 * 24) / 60 > 0                                       
 	    	            THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% (60 * 24) / 60                                           
     	                WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% (60 * 24) / 60 > =0                                   
 	    	            THEN 0                                                                                                                                 
 	    	            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% (60 * 24) / 60 < 0                                       
     	                THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% (60 * 24) / 60                                        
        	            END    	                                                                                                                             
                    END AS SAAT,                                                                                                                             
 	            CASE WHEN (IST.IST_DURUM_ID = 4) THEN                                                                                                       
 		            CASE                                                                                                                                       
 	   		            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% 60  > 0                    
 	    	            THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% 60                         
     	                WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% 60  > =0                
 	    	            THEN 0                                                                                                                                 
 	    	            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% 60  < 0                    
     	                THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% 60                      
        	            END    	                                                                                                                             
                    WHEN (IST.IST_DURUM_ID = 5 ) THEN                                                                                                        
     	            0                                                                                                                                       
                    ELSE                                                                                                                                     
 		            CASE                                                                                                                                       
 	   		            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% 60  > 0                                                  
 	    	            THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% 60                                                       
     	                WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% 60  > =0                                              
 	    	            THEN 0                                                                                                                                 
 	    	            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% 60  < 0                                                  
     	                THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% 60                                                    
        	            END    	                                                                                                                             
                    END AS DAKIKA,                                                                                                                           
                    CASE WHEN (IST.IST_DURUM_ID = 4) THEN                                                                                                    
    	                CASE WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI) >                   
 		   	            SOC_COZUM_SURE_GUN * 1440 + SOC_COZUM_SURE_SAAT * 60 + SOC_COZUM_SURE_DK THEN                                                          
 		                (DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)-                               
     		                (SOC_COZUM_SURE_GUN * 1440 + SOC_COZUM_SURE_SAAT * 60 + SOC_COZUM_SURE_DK))                                                           
     	            ELSE                                                                                                                                    
         	            DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)-                             
                            (SOC_COZUM_SURE_GUN * 1440 + SOC_COZUM_SURE_SAAT * 60 + SOC_COZUM_SURE_DK)                                                       
                        END                                                                                                                                  
                    WHEN (IST.IST_DURUM_ID = 5 ) THEN                                                                                                        
 		            0                                                                                                                                          
                    ELSE                                                                                                                                     
 	                CASE WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE()) >                                                     
 		   	            SOC_COZUM_SURE_GUN * 1440 + SOC_COZUM_SURE_SAAT * 60 + SOC_COZUM_SURE_DK THEN                                                          
 		                (DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())-                                                              
     		                (SOC_COZUM_SURE_GUN * 1440 + SOC_COZUM_SURE_SAAT * 60 + SOC_COZUM_SURE_DK))                                                           
     	            ELSE                                                                                                                                    
         	            DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())-                                                            
         	            (SOC_COZUM_SURE_GUN * 1440 + SOC_COZUM_SURE_SAAT * 60 + SOC_COZUM_SURE_DK)                                                          
                        END		                                                                                                                                
                    END AS FARK_DK 
                    ,(select COALESCE(TB_RESIM_ID,0) from orjin.TB_RESIM where RSM_VARSAYILAN= 1 AND RSM_REF_GRUP = 'CAGRI MERKEZI' and RSM_REF_ID = TB_IS_TALEP_ID) as RSM_VAR_ID
                    ,stuff((SELECT ';' + CONVERT(varchar(11), R.TB_RESIM_ID) FROM orjin.TB_RESIM R WHERE R.RSM_REF_GRUP = 'CAGRI MERKEZI' and R.RSM_REF_ID = TB_IS_TALEP_ID FOR XML PATH('')), 1, 1, '') [RSM_IDS] 
                    ,(SELECT LOK_TUM_YOL FROM orjin.TB_LOKASYON L WHERE L.TB_LOKASYON_ID = IST_BILDIREN_LOKASYON_ID) as IST_LOKASYON_TUM,ROW_NUMBER() OVER(ORDER BY IST_ACILIS_TARIHI " + sort + @") AS satir 
                    from orjin.VW_IS_TALEP IST LEFT JOIN orjin.TB_SERVIS_ONCELIK SO ON (IST.IST_ONCELIK_ID = SO.TB_SERVIS_ONCELIK_ID) where 1=1 ";
            if (durumID != -1)
            {
                if (durumID == 2)
                    query = query + " and IST_DURUM_ID IN (2,3)";
                else if (durumID == 3)
                    query = query + " and IST_DURUM_ID IN (4,5)";
                else
                    query = query + " and IST_DURUM_ID IN (0,1)";
            }
            if (filtre != null)
            {
                if (filtre.MakineID > 0)
                {
                    parametreler.Add(new Prm("IST_MAKINE_ID", filtre.DepoID));
                    query = query + " AND IST_MAKINE_ID = @IST_MAKINE_ID";
                }

                if (filtre.LokasyonID > 0)
                {
                    parametreler.Add(new Prm("IST_BILDIREN_LOKASYON_ID", filtre.LokasyonID));
                    query = query + " AND IST_BILDIREN_LOKASYON_ID = @IST_BILDIREN_LOKASYON_ID";
                }
                if (filtre.PersonelID > 0)
                {
                    parametreler.Add(new Prm("IST_TALEP_EDEN_ID", filtre.PersonelID));
                    query = query + " AND IST_TALEP_EDEN_ID = @IST_TALEP_EDEN_ID";

                }
                if (filtre.BasTarih != "" && filtre.BitTarih != "")
                {
                    DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                    DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                    parametreler.Add(new Prm("BAS_TARIH", bas.ToString("yyyy-MM-dd")));
                    parametreler.Add(new Prm("BIT_TARIH", bit.ToString("yyyy-MM-dd")));
                    query = query + " AND IST_ACILIS_TARIHI BETWEEN  @BAS_TARIH and @BIT_TARIH";
                }
                else
                if (filtre.BasTarih != "")
                {
                    DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                    parametreler.Add(new Prm("BAS_TARIH", bas.ToString("yyyy-MM-dd")));
                    query = query + " AND IST_ACILIS_TARIHI >=  @BAS_TARIH ";
                }
                else
                if (filtre.BitTarih != "")
                {
                    DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                    parametreler.Add(new Prm("BIT_TARIH", bit.ToString("yyyy-MM-dd")));
                    query = query + " AND IST_ACILIS_TARIHI <= @BIT_TARIH";
                }
                if (filtre.Kelime != "")
                {
                    parametreler.Add(new Prm("KELIME", filtre.Kelime));
                    query = query + " AND  (IST_KOD like '%'+@KELIME+'%' OR IST_TANIMI like '%'+@KELIME+'%' OR IST_TALEP_EDEN_ADI like '%'+@KELIME+'%' OR IST_ACIKLAMA like '%'+@KELIME+'%')";
                }
            }
            parametreler.Add(new Prm("ILK_DEGER", ilkDeger));
            parametreler.Add(new Prm("SON_DEGER", sonDeger));
            query = query + " ) as tablom where satir > @ILK_DEGER and satir <= @SON_DEGER AND orjin.UDF_LOKASYON_YETKI_KONTROL(IST_BILDIREN_LOKASYON_ID,@KUL_ID) = 1 AND orjin.UDF_ATOLYE_YETKI_KONTROL(IST_ATOLYE_GRUP_ID,@KUL_ID) = 1";

            DataTable dt = klas.GetDataTable(query, parametreler);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IsTalep entity = new IsTalep();
                entity.IST_ACILIS_TARIHI = Util.getFieldDateTime(dt.Rows[i], "IST_ACILIS_TARIHI");
                entity.TB_IS_TALEP_ID = Convert.ToInt32(dt.Rows[i]["TB_IS_TALEP_ID"]);
                entity.IST_ACIKLAMA = Util.getFieldString(dt.Rows[i], "IST_ACIKLAMA");
                entity.IST_ACILIS_SAATI = Util.getFieldString(dt.Rows[i], "IST_ACILIS_SAATI");
                entity.IST_BILDIREN_LOKASYON = Util.getFieldString(dt.Rows[i], "IST_BILDIREN_LOKASYON");
                entity.IST_BINA = Util.getFieldString(dt.Rows[i], "IST_BINA");
                entity.IST_DURUM_ADI = Util.getFieldString(dt.Rows[i], "IST_DURUM_ADI");
                entity.IST_EKIPMAN_KOD = Util.getFieldString(dt.Rows[i], "IST_EKIPMAN_KOD");
                entity.IST_EKIPMAN_TANIM = Util.getFieldString(dt.Rows[i], "IST_EKIPMAN_TANIM");
                entity.IST_KAT = Util.getFieldString(dt.Rows[i], "IST_KAT");
                entity.IST_KOD = Util.getFieldString(dt.Rows[i], "IST_KOD");
                entity.IST_KONU = Util.getFieldString(dt.Rows[i], "IST_KONU");
                entity.IST_MAKINE_KOD = Util.getFieldString(dt.Rows[i], "IST_MAKINE_KOD");
                entity.IST_MAKINE_TANIM = Util.getFieldString(dt.Rows[i], "IST_MAKINE_TANIM");
                entity.IST_NOT = Util.getFieldString(dt.Rows[i], "IST_NOT");
                entity.IST_TAKIP_EDEN_ADI = Util.getFieldString(dt.Rows[i], "IST_TAKIP_EDEN_ADI");
                entity.IST_TALEP_EDEN_ADI = Util.getFieldString(dt.Rows[i], "IST_TALEP_EDEN_ADI");
                entity.IST_ISEMRI_NO = Util.getFieldString(dt.Rows[i], "IST_ISEMRI_NO");
                entity.IST_KAPAMA_TARIHI = Util.getFieldDateTime(dt.Rows[i], "IST_KAPAMA_TARIHI");
                entity.IST_KAPAMA_SAATI = Util.getFieldString(dt.Rows[i], "IST_KAPAMA_SAATI");
                entity.IST_TANIMI = Util.getFieldString(dt.Rows[i], "IST_TANIMI");
                entity.IST_TEKNISYEN_TANIM = Util.getFieldString(dt.Rows[i], "IST_TEKNISYEN_TANIM");
                entity.IST_BILDIREN_LOKASYON_TUM = Util.getFieldString(dt.Rows[i], "IST_LOKASYON_TUM");
                entity.ISLEM_SURE = Util.getFieldString(dt.Rows[i], "ISLEM_SURE");
                entity.IST_DURUM_ID = Util.getFieldInt(dt.Rows[i], "IST_DURUM_ID");
                entity.IST_EKIPMAN_ID = Util.getFieldInt(dt.Rows[i], "IST_EKIPMAN_ID");
                entity.IST_MAKINE_ID = Util.getFieldInt(dt.Rows[i], "IST_MAKINE_ID");
                entity.IST_TALEP_EDEN_ID = Util.getFieldInt(dt.Rows[i], "IST_TALEP_EDEN_ID");
                entity.IST_BILDIREN_LOKASYON_ID = Util.getFieldInt(dt.Rows[i], "IST_BILDIREN_LOKASYON_ID");
                /*List<int> resimIdler = new List<int>();
                if (dt.Rows[i]["RSM_IDS"] != DBNull.Value)
                {
                    string[] ids = dt.Rows[i]["RSM_IDS"].ToString().Split(';');
                    for (int j = 0; j < ids.Length; j++)
                    {
                        resimIdler.Add(Convert.ToInt32(ids[j]));
                    }
                }
                entity.ResimIDleri = resimIdler;
                entity.ResimVarsayilanID = Metodlar.getFieldInt(dt.Rows[i], "RSM_VAR_ID");
                entity.ResimIDleri = resimIdler;*/
                listem.Add(entity);
            }

            return listem;
        }

        [HttpPost]
        [Route("api/istalep/TalepListe")]
        public object TalepListe([FromBody] Filtre filtre, [FromUri] int ilkDeger, [FromUri] int sonDeger, [FromUri] int ID, [FromUri] int durumID, [FromUri] bool sortAsc)
        {
            try
            {
                List<IsTalep> listem = new List<IsTalep>();
                parametreler.Clear();
                parametreler.Add(new Prm("KUL_ID", ID));
                string sort = sortAsc ? "ASC" : "DESC";
                #region sql
                string query = @"SELECT 	   TB_IS_TALEP_ID
                                                ,COALESCE(IST_KOD, '') AS IST_KOD
                                                ,COALESCE(IST_ACILIS_SAATI, '') AS IST_ACILIS_SAATI
                                                ,COALESCE(IST_GUNCELLEME_SAATI, '') AS IST_GUNCELLEME_SAATI
                                                ,COALESCE(IST_KAPAMA_SAATI, '') AS IST_KAPAMA_SAATI
                                                ,COALESCE(IST_TANIMI, '') AS IST_TANIMI
                                                ,COALESCE(IST_KONU, '') AS IST_KONU
                                                ,COALESCE(IST_NOT, '') AS IST_NOT
                                                ,COALESCE(IST_PLANLANAN_BASLAMA_SAATI, '') AS IST_PLANLANAN_BASLAMA_SAATI
                                                ,COALESCE(IST_PLANLANAN_BITIS_SAATI, '') AS IST_PLANLANAN_BITIS_SAATI
                                                ,COALESCE(IST_IRTIBAT_TELEFON, '') AS IST_IRTIBAT_TELEFON
                                                ,COALESCE(IST_MAIL_ADRES, '') AS IST_MAIL_ADRES
                                                ,COALESCE(IST_BASLAMA_SAATI, '') AS IST_BASLAMA_SAATI
                                                ,COALESCE(IST_BITIS_SAATI, '') AS IST_BITIS_SAATI
                                                ,COALESCE(IST_IPTAL_NEDEN, '') AS IST_IPTAL_NEDEN
                                                ,COALESCE(IST_IPTAL_SAAT, '') AS IST_IPTAL_SAAT
                                                ,COALESCE(IST_ACIKLAMA, '') AS IST_ACIKLAMA
                                                ,COALESCE(IST_SONUC, '') AS IST_SONUC
                                                ,COALESCE(IST_ACILIS_NEDEN, '') AS IST_ACILIS_NEDEN
                                                ,COALESCE(IST_OZEL_ALAN_1, '') AS IST_OZEL_ALAN_1
                                                ,COALESCE(IST_OZEL_ALAN_2, '') AS IST_OZEL_ALAN_2
                                                ,COALESCE(IST_OZEL_ALAN_3, '') AS IST_OZEL_ALAN_3
                                                ,COALESCE(IST_OZEL_ALAN_4, '') AS IST_OZEL_ALAN_4
                                                ,COALESCE(IST_OZEL_ALAN_5, '') AS IST_OZEL_ALAN_5
                                                ,COALESCE(IST_OZEL_ALAN_6, '') AS IST_OZEL_ALAN_6
                                                ,COALESCE(IST_OZEL_ALAN_7, '') AS IST_OZEL_ALAN_7
                                                ,COALESCE(IST_OZEL_ALAN_8, '') AS IST_OZEL_ALAN_8
                                                ,COALESCE(IST_OZEL_ALAN_9, '') AS IST_OZEL_ALAN_9
                                                ,COALESCE(IST_OZEL_ALAN_10, '') AS IST_OZEL_ALAN_10
                                                ,COALESCE(IST_ON_DEGERLENDIRME, '') AS IST_ON_DEGERLENDIRME
                                                ,COALESCE(IST_DEGERLENDIRME_ACIKLAMA, '') AS IST_DEGERLENDIRME_ACIKLAMA
                                                ,COALESCE(IST_TALEP_EDEN_ADI, '') AS IST_TALEP_EDEN_ADI
                                                ,COALESCE(IST_TAKIP_EDEN_ADI, '') AS IST_TAKIP_EDEN_ADI
                                                ,COALESCE(IST_ATOLYE_GRUBU_TANIMI, '') AS IST_ATOLYE_GRUBU_TANIMI
                                                ,COALESCE(IST_TIP_TANIM, '') AS IST_TIP_TANIM
                                                ,COALESCE(IST_KATEGORI_TANIMI, '') AS IST_KATEGORI_TANIMI
                                                ,COALESCE(IST_SERVIS_NEDENI, '') AS IST_SERVIS_NEDENI
                                                ,COALESCE(IST_IRTIBAT, '') AS IST_IRTIBAT
                                                ,COALESCE(IST_ONCELIK, '') AS IST_ONCELIK
                                                ,COALESCE(IST_BILDIREN_LOKASYON, '') AS IST_BILDIREN_LOKASYON
                                                ,COALESCE(IST_TEKNISYEN_TANIM, '') AS IST_TEKNISYEN_TANIM
                                                ,COALESCE(IST_ISEMRI_NO, '') AS IST_ISEMRI_NO
                                                ,COALESCE(IST_MAKINE_KOD, '') AS IST_MAKINE_KOD
                                                ,COALESCE(IST_MAKINE_TANIM, '') AS IST_MAKINE_TANIM
                                                ,COALESCE(IST_EKIPMAN_KOD, '') AS IST_EKIPMAN_KOD
                                                ,COALESCE(IST_EKIPMAN_TANIM, '') AS IST_EKIPMAN_TANIM
                                                ,COALESCE(IST_KAT, '') AS IST_KAT
                                                ,COALESCE(IST_BINA, '') AS IST_BINA
                                                ,COALESCE(IST_DURUM_ADI, '') AS IST_DURUM_ADI
                                                ,COALESCE(IST_DURUM_ADI2, '') AS IST_DURUM_ADI2
                                                ,COALESCE(IST_MAKINE_DURUM, '') AS IST_MAKINE_DURUM
                                                ,COALESCE(IST_ARIZA_TANIM_KOD, '') AS IST_ARIZA_TANIM_KOD
                                                ,COALESCE(ISEMRI_TIPI, '') AS ISEMRI_TIPI
                                                ,IST_ACILIS_TARIHI
                                                ,IST_GUNCELLEME_TARIHI
                                                ,IST_GUNCELEYEN_ID
                                                ,IST_KAPAMA_TARIHI
                                                ,IST_TALEP_EDEN_ID
                                                ,IST_IS_TAKIPCISI_ID
                                                ,IST_ATOLYE_GRUP_ID
                                                ,IST_TIP_KOD_ID
                                                ,IST_KOTEGORI_KODI_ID
                                                ,IST_SERVIS_NEDENI_KOD_ID
                                                ,IST_IRTIBAT_KOD_KOD_ID
                                                ,IST_BILDIRILEN_BINA
                                                ,IST_BILDIRILEN_KAT
                                                ,IST_DURUM_ID
                                                ,IST_ONCELIK_ID
                                                ,IST_PLANLANAN_BASLAMA_TARIHI
                                                ,IST_PLANLANAN_BITIS_TARIHI
                                                ,IST_BILDIREN_LOKASYON_ID
                                                ,IST_BASLAMA_TARIHI
                                                ,IST_BITIS_TARIHI
                                                ,IST_IPTAL_TARIH
                                                ,IST_MAKINE_ID
                                                ,IST_EKIPMAN_ID
                                                ,IST_ISEMRI_ID
                                                ,IST_AKTIF
                                                ,IST_BIRLESIM_ID
                                                ,IST_SABLON_ID
                                                ,IST_ARIZA_ID
                                                ,IST_MAKINE_DURUM_KOD_ID
                                                ,IST_ARIZA_TANIM_KOD_ID
                                                ,IST_OKUNDU
                                                ,IST_ISEMRI_TIP_ID
                                                ,IST_OZEL_ALAN_11
                                                ,IST_OZEL_ALAN_12
                                                ,IST_OZEL_ALAN_13
                                                ,IST_OZEL_ALAN_14
                                                ,IST_OZEL_ALAN_15
                                                ,IST_OZEL_ALAN_16
                                                ,IST_OZEL_ALAN_17
                                                ,IST_OZEL_ALAN_18
                                                ,IST_OZEL_ALAN_19
                                                ,IST_OZEL_ALAN_20
                                                ,IST_OLUSTURAN_ID
                                                ,IST_OLUSTURMA_TARIH
                                                ,IST_DEGISTIREN_ID
                                                ,IST_DEGISTIRME_TARIH
                                                ,IST_IS_DEVAM_DURUM_ID
                                                ,IST_DEGERLENDIRME_PUAN
                                                ,IST_DEPARTMAN_ID
                                                ,IST_ONCELIK_IKON_INDEX
                                                ,IST_NOT_ICON
                                                ,IST_TEKNISYEN_ID
                                                ,IST_BELGE
                                                ,IST_RESIM
                                                ,IST_BIRLESIM
                                                ,IST_KULLANICI_DEPARTMAN_ID
			,COALESCE((select TB_RESIM_ID from orjin.TB_RESIM where RSM_VARSAYILAN= 1 AND RSM_REF_GRUP = 'CAGRI MERKEZI' and RSM_REF_ID = TB_IS_TALEP_ID),0) as ResimVarsayilanID
            ,stuff((SELECT ';' + CONVERT(varchar(11), R.TB_RESIM_ID) FROM orjin.TB_RESIM R WHERE R.RSM_REF_GRUP = 'CAGRI MERKEZI' and R.RSM_REF_ID = TB_IS_TALEP_ID FOR XML PATH('')), 1, 1, '') AS ResimIDleri 
                    ,(SELECT LOK_TUM_YOL FROM orjin.TB_LOKASYON L WHERE L.TB_LOKASYON_ID = IST_BILDIREN_LOKASYON_ID) as IST_BILDIREN_LOKASYON_TUM
            ,(CASE WHEN GUN    > 0 THEN CAST(GUN    AS VARCHAR(5))+ ' g '    ELSE '' END +                                                      
                    CASE WHEN SAAT   > 0 THEN CAST(SAAT   AS VARCHAR(5))+ ' s '   ELSE '' END +                                                      
                    CASE WHEN DAKIKA > 0 THEN CAST(DAKIKA AS VARCHAR(5))+ ' dk ' ELSE '' END ) AS ISLEM_SURE           
            from (select *, 	                                    
                CASE WHEN (IST.IST_DURUM_ID = 4) THEN                                                                                                       
 		            CASE                                                                                                                                       
 	   		            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)/(60 * 24) > 0               
 	    	            THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)/(60 * 24)                   
     	                WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)/(60 * 24) > =0           
 	    	            THEN 0                                                                                                                                 
 	    	            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)/(60 * 24) < 0               
     	                THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)/(60 * 24)                
        	            END    	                                                                                                                             
                    WHEN (IST.IST_DURUM_ID = 5 ) THEN                                                                                                        
     	            0                                                                                                                                       
                    ELSE                                                                                                                                     
 		            CASE                                                                                                                                       
 	   		            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())/(60 * 24) > 0                                             
 	    	            THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())/(60 * 24)                                                 
     	                WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())/(60 * 24) > =0                                         
 	    	            THEN 0                                                                                                                                 
 	    	            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())/(60 * 24) < 0                                             
     	                THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())/(60 * 24)                                              
        	            END    	                                                                                                                             
                    END AS GUN,                                                                                                                              
 	            CASE WHEN (IST.IST_DURUM_ID = 4) THEN                                                                                                       
 		            CASE                                                                                                                                       
 	   		            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% (60 * 24) / 60 > 0         
 	    	            THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% (60 * 24) / 60             
     	                WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% (60 * 24) / 60 > =0     
 	    	            THEN 0                                                                                                                                 
 	    	            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% (60 * 24) / 60 < 0         
     	                THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% (60 * 24) / 60          
        	            END    	                                                                                                                             
                    WHEN (IST.IST_DURUM_ID = 5 ) THEN                                                                                                        
     	            0                                                                                                                                       
                    ELSE                                                                                                                                     
 		            CASE                                                                                                                                       
 	   		            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% (60 * 24) / 60 > 0                                       
 	    	            THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% (60 * 24) / 60                                           
     	                WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% (60 * 24) / 60 > =0                                   
 	    	            THEN 0                                                                                                                                 
 	    	            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% (60 * 24) / 60 < 0                                       
     	                THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% (60 * 24) / 60                                        
        	            END    	                                                                                                                             
                    END AS SAAT,                                                                                                                             
 	            CASE WHEN (IST.IST_DURUM_ID = 4) THEN                                                                                                       
 		            CASE                                                                                                                                       
 	   		            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% 60  > 0                    
 	    	            THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% 60                         
     	                WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% 60  > =0                
 	    	            THEN 0                                                                                                                                 
 	    	            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% 60  < 0                    
     	                THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI,IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)% 60                      
        	            END    	                                                                                                                             
                    WHEN (IST.IST_DURUM_ID = 5 ) THEN                                                                                                        
     	            0                                                                                                                                       
                    ELSE                                                                                                                                     
 		            CASE                                                                                                                                       
 	   		            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% 60  > 0                                                  
 	    	            THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% 60                                                       
     	                WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% 60  > =0                                              
 	    	            THEN 0                                                                                                                                 
 	    	            WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% 60  < 0                                                  
     	                THEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())% 60                                                    
        	            END    	                                                                                                                             
                    END AS DAKIKA,                                                                                                                           
                    CASE WHEN (IST.IST_DURUM_ID = 4) THEN                                                                                                    
    	                CASE WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI) >                   
 		   	            SOC_COZUM_SURE_GUN * 1440 + SOC_COZUM_SURE_SAAT * 60 + SOC_COZUM_SURE_DK THEN                                                          
 		                (DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)-                               
     		                (SOC_COZUM_SURE_GUN * 1440 + SOC_COZUM_SURE_SAAT * 60 + SOC_COZUM_SURE_DK))                                                           
     	            ELSE                                                                                                                                    
         	            DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, IST.IST_BITIS_TARIHI+IST.IST_BITIS_SAATI)-                             
                            (SOC_COZUM_SURE_GUN * 1440 + SOC_COZUM_SURE_SAAT * 60 + SOC_COZUM_SURE_DK)                                                       
                        END                                                                                                                                  
                    WHEN (IST.IST_DURUM_ID = 5 ) THEN                                                                                                        
 		            0                                                                                                                                          
                    ELSE                                                                                                                                     
 	                CASE WHEN DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE()) >                                                     
 		   	            SOC_COZUM_SURE_GUN * 1440 + SOC_COZUM_SURE_SAAT * 60 + SOC_COZUM_SURE_DK THEN                                                          
 		                (DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())-                                                              
     		                (SOC_COZUM_SURE_GUN * 1440 + SOC_COZUM_SURE_SAAT * 60 + SOC_COZUM_SURE_DK))                                                           
     	            ELSE                                                                                                                                    
         	            DATEDIFF(minute,IST.IST_ACILIS_TARIHI+ IST.IST_ACILIS_SAATI, GETDATE())-                                                            
         	            (SOC_COZUM_SURE_GUN * 1440 + SOC_COZUM_SURE_SAAT * 60 + SOC_COZUM_SURE_DK)                                                          
                        END		                                                                                                                                
                    END AS FARK_DK ,ROW_NUMBER() OVER(ORDER BY IST_ACILIS_TARIHI " + sort + " , IST_ACILIS_SAATI " + sort + @" ) AS satir 
                    from orjin.VW_IS_TALEP IST LEFT JOIN orjin.TB_SERVIS_ONCELIK SO ON (IST.IST_ONCELIK_ID = SO.TB_SERVIS_ONCELIK_ID) ";
                String where = " WHERE orjin.UDF_LOKASYON_YETKI_KONTROL(IST_BILDIREN_LOKASYON_ID,@KUL_ID) = 1 AND orjin.UDF_ATOLYE_YETKI_KONTROL(IST_ATOLYE_GRUP_ID,@KUL_ID) = 1 ";

                if (filtre != null)
                {
                    if (filtre.MakineID > 0)
                    {
                        parametreler.Add(new Prm("IST_MAKINE_ID", filtre.DepoID));
                        where += " AND IST_MAKINE_ID = @IST_MAKINE_ID";
                    }

                    if (filtre.LokasyonID > 0)
                    {
                        parametreler.Add(new Prm("IST_BILDIREN_LOKASYON_ID", filtre.LokasyonID));
                        where += " AND IST_BILDIREN_LOKASYON_ID = @IST_BILDIREN_LOKASYON_ID ";
                    }
                    if (filtre.PersonelID > 0)
                    {
                        if(filtre.IlgiliKisiId > 0)
                        {
                            parametreler.Add(new Prm("IST_TALEP_EDEN_ID", filtre.PersonelID));
                            where += " AND ( IST_TALEP_EDEN_ID = @IST_TALEP_EDEN_ID ";
                        }
                        else
                        {
                            parametreler.Add(new Prm("IST_TALEP_EDEN_ID", filtre.PersonelID));
                            where += " AND IST_TALEP_EDEN_ID = @IST_TALEP_EDEN_ID ";
                        }

                    }
                    if (filtre.IlgiliKisiId > 0)
                    {
                        if (filtre.PersonelID > 0)
                        {
                            parametreler.Add(new Prm("IST_TALEP_EDEN_ID", filtre.IlgiliKisiId));
                            where += " OR IST_TALEP_EDEN_ID = @IST_TALEP_EDEN_ID ) ";
                        }
                        else
                        {
                            parametreler.Add(new Prm("IST_TALEP_EDEN_ID", filtre.IlgiliKisiId));
                            where += " AND IST_TALEP_EDEN_ID = @IST_TALEP_EDEN_ID ";
                        }

                    }
                    if (filtre.BasTarih != "" && filtre.BitTarih != "")
                    {
                        DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                        DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                        parametreler.Add(new Prm("BAS_TARIH", bas.ToString("yyyy-MM-dd")));
                        parametreler.Add(new Prm("BIT_TARIH", bit.ToString("yyyy-MM-dd")));
                        where += " AND IST_ACILIS_TARIHI BETWEEN  @BAS_TARIH and @BIT_TARIH";
                    }
                    else
                    if (filtre.BasTarih != "")
                    {
                        DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                        parametreler.Add(new Prm("BAS_TARIH", bas.ToString("yyyy-MM-dd")));
                        where += " AND IST_ACILIS_TARIHI >=  @BAS_TARIH ";
                    }
                    else
                    if (filtre.BitTarih != "")
                    {
                        DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                        parametreler.Add(new Prm("BIT_TARIH", bit.ToString("yyyy-MM-dd")));
                        where += " AND IST_ACILIS_TARIHI <= @BIT_TARIH";
                    }
                    if (filtre.Kelime != "")
                    {
                        parametreler.Add(new Prm("KELIME", filtre.Kelime));
                        where += " AND  (IST_KOD like '%'+@KELIME+'%' OR IST_TANIMI like '%'+@KELIME+'%' OR IST_TALEP_EDEN_ADI like '%'+@KELIME+'%' OR IST_ACIKLAMA like '%'+@KELIME+'%')";
                    }
                }

                var q1 = "SELECT COUNT(*) FROM orjin.VW_IS_TALEP " + where + " AND IST_DURUM_ID IN (0,1);";//açık
                var q2 = "SELECT COUNT(*) FROM orjin.VW_IS_TALEP " + where + " AND IST_DURUM_ID IN (2,3);";//devam
                var q3 = "SELECT COUNT(*) FROM orjin.VW_IS_TALEP " + where + " AND IST_DURUM_ID IN (4,5);";//kapalı
                var q4 = "SELECT COUNT(*) FROM orjin.VW_IS_TALEP " + where + " AND IST_DURUM_ID IN (0,1,2,3,4,5);";

                if (durumID != -1)
                {
                    if (durumID == 2)
                        where += " AND IST_DURUM_ID IN (2,3)";
                    else if (durumID == 3)
                        where += " AND IST_DURUM_ID IN (4,5)";
                    else
                        where += " AND IST_DURUM_ID IN (0,1)";
                }
                query += where;
                parametreler.Add(new Prm("ILK_DEGER", ilkDeger));
                parametreler.Add(new Prm("SON_DEGER", sonDeger));
                query = query + " ) as tablom where satir > @ILK_DEGER and satir <= @SON_DEGER;";
                query += q1 + q2 + q3 + q4;
                #endregion
                using (var conn = klas.baglanCmd())
                {
                    var prms = new DynamicParameters();
                    parametreler.ForEach(p => prms.Add(p.ParametreAdi, p.ParametreDeger));
                    var result = conn.QueryMultiple(query, prms);
                    listem = result.Read<IsTalep>().ToList();
                    var acikSayi = result.ReadFirst<int>();
                    var devamSayi = result.ReadFirst<int>();
                    var kapaliSayi = result.ReadFirst<int>();
                    var toplamSayi = result.ReadFirst<int>();
                    return new { AcikSayi = acikSayi, DevamSayi = devamSayi, KapaliSayi = kapaliSayi, Toplam = toplamSayi, Istalepleri = listem };
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Bildirim Post([FromBody] IsTalep entity, [FromUri] int ID)
        {
            Bildirim bildirimEntity = new Bildirim();

            if (entity.TB_IS_TALEP_ID < 1)
            {
                var util = new Util();
                using (TransactionScope ts = new TransactionScope())
                using (var cnn = util.baglan())
                {
                    try
                    {
                        bool reqNotExists = cnn.QueryFirstOrDefault<int>("SELECT COUNT(*) FROM orjin.TB_IS_TALEBI WHERE IST_KOD = @IST_KOD", new { entity.IST_KOD }) < 1;
                        if (reqNotExists)
                        {
                            #region Kaydet
                            var isemritipId = cnn.QueryFirstOrDefault<int>("SELECT TOP 1 ISP_ISEMRI_TIPI_ID FROM orjin.TB_IS_TALEBI_PARAMETRE");

                            var userId = cnn.QueryFirstOrDefault<int>(@" select isk.TB_IS_TALEBI_KULLANICI_ID
	                                FROM [PBTPRO_MASTER].[orjin].[TB_KULLANICI] kll
	                                left join [PBTPRO_1].[orjin].[TB_PERSONEL] prs on prs.TB_PERSONEL_ID = kll.KLL_PERSONEL_ID " + 
	                                $" left join [PBTPRO_1].[orjin].[TB_IS_TALEBI_KULLANICI] isk on prs.TB_PERSONEL_ID = isk.ISK_PERSONEL_ID where kll.TB_KULLANICI_ID = {entity.USER_ID}");

                            if (entity.IST_TIP_KOD_ID < 1)
                                entity.IST_TIP_KOD_ID = cnn.QueryFirstOrDefault<int>("SELECT TOP 1 ISP_VARSAYILAN_IS_TIPI FROM orjin.TB_IS_TALEBI_PARAMETRE");
                            string qu1 = @"INSERT INTO orjin.TB_IS_TALEBI
                                                    (IST_KOD ,
                                                    IST_ACILIS_TARIHI,
                                                    IST_ACILIS_SAATI,
                                                    IST_GUNCELEYEN_ID,
                                                    IST_TALEP_EDEN_ID,
                                                    IST_IS_TAKIPCISI_ID,
                                                    IST_TIP_KOD_ID,
                                                    IST_KOTEGORI_KODI_ID,
                                                    IST_SERVIS_NEDENI_KOD_ID,
                                                    IST_IRTIBAT_KOD_KOD_ID,
                                                    IST_BILDIRILEN_BINA,
                                                    IST_BILDIRILEN_KAT,
                                                    IST_TANIMI,
                                                    IST_KONU,
                                                    IST_NOT,
                                                    IST_DURUM_ID,                
                                                    IST_BILDIREN_LOKASYON_ID,  
                                                    IST_ATOLYE_GRUP_ID,
                                                    IST_IRTIBAT_TELEFON,
                                                    IST_MAIL_ADRES,
                                                    IST_BASLAMA_TARIHI,
                                                    IST_BASLAMA_SAATI,
                                                    IST_MAKINE_ID,
                                                    IST_EKIPMAN_ID,
                                                    IST_ISEMRI_ID,
                                                    IST_ACIKLAMA,
                                                    IST_SONUC,
                                                    IST_AKTIF,
                                                    IST_BIRLESIM_ID,
                                                    IST_ACILIS_NEDEN,                
                                                    IST_ISEMRI_TIP_ID,
                                                    IST_OLUSTURAN_ID, 
                                                    IST_OLUSTURMA_TARIH,                
                                                    IST_IS_DEVAM_DURUM_ID,
                                                    IST_DEPARTMAN_ID)
                                                    VALUES(@IST_KOD,
                                                    @IST_ACILIS_TARIHI,
                                                    @IST_ACILIS_SAATI,               
                                                    @IST_GUNCELEYEN_ID,
                                                    @IST_TALEP_EDEN_ID,
                                                    @IST_IS_TAKIPCISI_ID,
                                                    @IST_TIP_KOD_ID,
                                                    @IST_KOTEGORI_KODI_ID,
                                                    @IST_SERVIS_NEDENI_KOD_ID,
                                                    @IST_IRTIBAT_KOD_KOD_ID,
                                                    @IST_BILDIRILEN_BINA,
                                                    @IST_BILDIRILEN_KAT,
                                                    @IST_TANIMI,
                                                    @IST_KONU,
                                                    @IST_NOT,
                                                    @IST_DURUM_ID,               
                                                    @IST_BILDIREN_LOKASYON_ID,
                                                    @IST_ATOLYE_GRUP_ID,
                                                    @IST_IRTIBAT_TELEFON,
                                                    @IST_MAIL_ADRES,
                                                    @IST_BASLAMA_TARIHI,
                                                    @IST_BASLAMA_SAATI,               
                                                    @IST_MAKINE_ID,
                                                    @IST_EKIPMAN_ID,
                                                    @IST_ISEMRI_ID,
                                                    @IST_ACIKLAMA,
                                                    @IST_SONUC,
                                                    @IST_AKTIF,
                                                    @IST_BIRLESIM_ID,
                                                    @IST_ACILIS_NEDEN,
                                                    @IST_ISEMRI_TIP_ID,
                                                    @IST_OLUSTURAN_ID,
                                                    @IST_OLUSTURMA_TARIH,               
                                                    @IST_IS_DEVAM_DURUM_ID,
                                                    @IST_DEPARTMAN_ID)";
                            prms.Clear();
                            var prms1 = new DynamicParameters();
                            prms1.Add("IST_KOD", entity.IST_KOD);
                            prms1.Add("IST_ACILIS_TARIHI", entity.IST_ACILIS_TARIHI);
                            prms1.Add("IST_ACILIS_SAATI", entity.IST_ACILIS_SAATI);
                            prms1.Add("IST_GUNCELEYEN_ID", -1);
                            prms1.Add("IST_TALEP_EDEN_ID", userId);
                            prms1.Add("IST_IS_TAKIPCISI_ID", entity.IST_TALEP_EDEN_ID);
                            prms1.Add("IST_TIP_KOD_ID", entity.IST_TIP_KOD_ID);
                            prms1.Add("IST_KOTEGORI_KODI_ID", -1);
                            prms1.Add("IST_SERVIS_NEDENI_KOD_ID", -1);
                            prms1.Add("IST_IRTIBAT_KOD_KOD_ID", -1);
                            prms1.Add("IST_BILDIRILEN_BINA", -1);
                            prms1.Add("IST_BILDIRILEN_KAT", -1);
                            prms1.Add("IST_TANIMI", entity.IST_TANIMI);
                            prms1.Add("IST_KONU", "");
                            prms1.Add("IST_NOT", "");
                            prms1.Add("IST_DURUM_ID", 0);
                            prms1.Add("IST_BILDIREN_LOKASYON_ID", entity.IST_BILDIREN_LOKASYON_ID == 0 ? -1 : entity.IST_BILDIREN_LOKASYON_ID);
                            prms1.Add("IST_ATOLYE_GRUP_ID", -1);
                            prms1.Add("IST_IRTIBAT_TELEFON", "");
                            prms1.Add("IST_MAIL_ADRES", "");
                            prms1.Add("IST_BASLAMA_TARIHI", entity.IST_ACILIS_TARIHI);
                            prms1.Add("IST_BASLAMA_SAATI", entity.IST_ACILIS_SAATI);
                            prms1.Add("IST_MAKINE_ID", entity.IST_MAKINE_ID == 0 ? -1 : entity.IST_MAKINE_ID);
                            prms1.Add("IST_EKIPMAN_ID", -1);
                            prms1.Add("IST_ISEMRI_ID", -1);
                            prms1.Add("IST_ACIKLAMA", entity.IST_ACIKLAMA);
                            prms1.Add("IST_SONUC", "");
                            prms1.Add("IST_AKTIF", "1");
                            prms1.Add("IST_BIRLESIM_ID", -1);
                            prms1.Add("IST_ACILIS_NEDEN", -1);
                            prms1.Add("IST_ISEMRI_TIP_ID", isemritipId);
                            prms1.Add("IST_OLUSTURAN_ID", ID);
                            prms1.Add("IST_OLUSTURMA_TARIH", DateTime.Now);
                            prms1.Add("IST_IS_DEVAM_DURUM_ID", -1);
                            prms1.Add("IST_DEPARTMAN_ID", -1);
                            cnn.Execute(qu1, prms1);
                            int SonTalepId = cnn.QueryFirstOrDefault<int>("SELECT TOP 1 TB_IS_TALEP_ID FROM orjin.TB_IS_TALEBI ORDER BY  TB_IS_TALEP_ID DESC");
                            if (SonTalepId > 0)
                            {
                                var itl = new IsTalebiLog
                                {
                                    ITL_IS_TANIM_ID = SonTalepId,
                                    ITL_KULLANICI_ID = entity.IST_TALEP_EDEN_ID,
                                    ITL_TARIH = entity.IST_ACILIS_TARIHI,
                                    ITL_SAAT = entity.IST_ACILIS_SAATI,
                                    ITL_ISLEM = "Yeni iş talebi",
                                    ITL_ACIKLAMA = String.Format("Talep no: '{0}' - Konu :'{1}'", entity.IST_KOD,
                                        entity.IST_TANIMI),
                                    ITL_ISLEM_DURUM = "AÇIK",
                                    ITL_TALEP_ISLEM = "Yeni İş Talebi",
                                    ITL_OLUSTURAN_ID = ID,
                                    ITL_OLUSTURMA_TARIH = DateTime.Now
                                };
                                cnn.Insert(itl);
                            }
                            #endregion
                            bildirimEntity.Id = cnn.QueryFirstOrDefault<int>("select max(TB_IS_TALEP_ID) from orjin.TB_IS_TALEBI");
                            bildirimEntity.Aciklama = "İş talebi kaydı başarılı bir şekilde gerçekleşti.";
                            bildirimEntity.MsgId = Bildirim.MSG_IST_KAYIT_OK;
                            bildirimEntity.Durum = true;

                            ts.Complete();
                        }
                    }
                    catch (Exception e)
                    {
                        bildirimEntity.Aciklama = string.Format(Localization.errorFormatted, e.Message);
                        bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                        bildirimEntity.HasExtra = true;
                        bildirimEntity.Durum = false;
                    }
                }
            }

            return bildirimEntity;
        }

        [Route("api/IsTalepKodGetir")]
        public TanimDeger GetIsTalepKod()
        {
            var util = new Util();
            using (var cnn = util.baglan())
            {
                TanimDeger entity = new TanimDeger();
                entity.Tanim = cnn.QueryFirstOrDefault<string>(Queries.GENERATE_KOD, new { KOD = "IST_KOD" });
                return entity;
            }


        }

        [HttpPost]
        [Route("api/IsTalepSil")]
        public void IsTalepSil([FromUri] int talepID)
        {
            try
            {
                var util = new Util();
                using (var cnn = util.baglan())
                {
                    var parametreler = new DynamicParameters();
                    parametreler.Add("TALEP_ID", talepID);
                    // BİRLİEŞİMİN LOGLARI SİLİNİYOR..
                    cnn.Execute("DELETE FROM orjin.TB_IS_TALEBI_LOG WHERE ITL_IS_TANIM_ID IN (SELECT ISB_IS_TANIM_ID_DIGER FROM orjin.TB_IS_TALEBI_BIRLESIM WHERE ISB_IS_TANIM_ID = @TALEP_ID)", parametreler);
                    // Bağlı kayıtlar siliniyor
                    cnn.Execute("DELETE FROM orjin.TB_IS_TALEBI WHERE IST_BIRLESIM_ID = @TALEP_ID", parametreler);
                    // Birleşim tablosu siliniyor
                    cnn.Execute("DELETE FROM orjin.TB_IS_TALEBI_BIRLESIM WHERE ISB_IS_TANIM_ID =@TALEP_ID", parametreler);
                    // DOKUMAN SİLİNİYOR
                    cnn.Execute("DELETE FROM TB_DOSYA WHERE DSY_REF_ID = @TALEP_ID AND DSY_REF_GRUP = 'CAGRI MERKEZI'", parametreler);
                    //talebin kendisi siliniyor..
                    cnn.Execute("DELETE FROM orjin.TB_IS_TALEBI WHERE TB_IS_TALEP_ID = @TALEP_ID", parametreler);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/IsTalepGuncelle")]
        public void IsTalepGuncelle([FromBody] IsTalep entity)
        {
            try
            {
                parametreler.Clear();
                parametreler.Add(new Prm("TB_IS_TALEP_ID", entity.TB_IS_TALEP_ID));
                int eskiDurumId = Convert.ToInt32(klas.GetDataCell("select IST_DURUM_ID from orjin.TB_IS_TALEBI where TB_IS_TALEP_ID=@TB_IS_TALEP_ID", parametreler));
                string saatSifirli = DateTime.Now.ToString("HH:mm:ss");
                if (entity.TB_IS_TALEP_ID > 0)
                {
                    #region Günccelle

                    string query = @"UPDATE orjin.TB_IS_TALEBI SET
                                    IST_KOD =@IST_KOD ,              
                                    IST_GUNCELLEME_TARIHI =@IST_GUNCELLEME_TARIHI,
                                    IST_GUNCELLEME_SAATI =@IST_GUNCELLEME_SAATI,
                                    IST_GUNCELEYEN_ID =@IST_GUNCELEYEN_ID,
                                    IST_TALEP_EDEN_ID =@IST_TALEP_EDEN_ID,
                                    IST_IS_TAKIPCISI_ID =@IST_IS_TAKIPCISI_ID,
                                    IST_TIP_KOD_ID =@IST_TIP_KOD_ID,
                                    IST_KOTEGORI_KODI_ID =@IST_KOTEGORI_KODI_ID,
                                    IST_SERVIS_NEDENI_KOD_ID =@IST_SERVIS_NEDENI_KOD_ID,
                                    IST_IRTIBAT_KOD_KOD_ID =@IST_IRTIBAT_KOD_KOD_ID,
                                    IST_BILDIRILEN_BINA =@IST_BILDIRILEN_BINA,
                                    IST_BILDIRILEN_KAT =@IST_BILDIRILEN_KAT,
                                    IST_TANIMI =@IST_TANIMI,
                                    IST_KONU =@IST_KONU,
                                    IST_NOT =@IST_NOT,
                                    IST_DURUM_ID =@IST_DURUM_ID,
                                    IST_BILDIREN_LOKASYON_ID =@IST_BILDIREN_LOKASYON_ID,
                                    IST_IRTIBAT_TELEFON =@IST_IRTIBAT_TELEFON,
                                    IST_MAIL_ADRES =@IST_MAIL_ADRES,  
                                    IST_MAKINE_ID =@IST_MAKINE_ID,
                                    IST_EKIPMAN_ID =@IST_EKIPMAN_ID,
                                    IST_ISEMRI_ID =@IST_ISEMRI_ID,
                                    IST_ACIKLAMA =@IST_ACIKLAMA,
                                    IST_SONUC =@IST_SONUC,
                                    IST_AKTIF =@IST_AKTIF,
                                    IST_BIRLESIM_ID =@IST_BIRLESIM_ID,
                                    IST_ACILIS_NEDEN =@IST_ACILIS_NEDEN,                    
                                    IST_DEGISTIRME_TARIH =@IST_DEGISTIRME_TARIH,
                                    IST_IS_DEVAM_DURUM_ID =@IST_IS_DEVAM_DURUM_ID,
                                    IST_DEPARTMAN_ID =@IST_DEPARTMAN_ID
                                    where TB_IS_TALEP_ID = @TB_IS_TALEP_ID";
                    prms.Clear();

                    prms.Add("@TB_IS_TALEP_ID", entity.TB_IS_TALEP_ID);
                    prms.Add("@IST_KOD", entity.IST_KOD);
                    prms.Add("@IST_GUNCELLEME_TARIHI", DateTime.Now);
                    prms.Add("@IST_GUNCELLEME_SAATI", saatSifirli);
                    prms.Add("@IST_GUNCELEYEN_ID", entity.IST_GUNCELEYEN_ID);
                    prms.Add("@IST_TALEP_EDEN_ID", entity.IST_TALEP_EDEN_ID);
                    prms.Add("@IST_IS_TAKIPCISI_ID", entity.IST_TALEP_EDEN_ID);
                    prms.Add("@IST_TIP_KOD_ID", entity.IST_TIP_KOD_ID);
                    prms.Add("@IST_KOTEGORI_KODI_ID", entity.IST_KOTEGORI_KODI_ID);
                    prms.Add("@IST_SERVIS_NEDENI_KOD_ID", entity.IST_SERVIS_NEDENI_KOD_ID);
                    prms.Add("@IST_IRTIBAT_KOD_KOD_ID", entity.IST_IRTIBAT_KOD_KOD_ID);
                    prms.Add("@IST_BILDIRILEN_BINA", entity.IST_BILDIRILEN_BINA);
                    prms.Add("@IST_BILDIRILEN_KAT", entity.IST_BILDIRILEN_KAT);
                    prms.Add("@IST_TANIMI", entity.IST_TANIMI);
                    prms.Add("@IST_KONU", entity.IST_KONU);
                    prms.Add("@IST_NOT", entity.IST_NOT);
                    prms.Add("@IST_DURUM_ID", entity.IST_DURUM_ID);
                    prms.Add("@IST_BILDIREN_LOKASYON_ID", entity.IST_BILDIREN_LOKASYON_ID);
                    prms.Add("@IST_IRTIBAT_TELEFON", entity.IST_IRTIBAT_TELEFON);
                    prms.Add("@IST_MAIL_ADRES", entity.IST_MAIL_ADRES);
                    prms.Add("@IST_MAKINE_ID", entity.IST_MAKINE_ID);
                    prms.Add("@IST_EKIPMAN_ID", entity.IST_EKIPMAN_ID);
                    prms.Add("@IST_ISEMRI_ID", entity.IST_ISEMRI_ID);
                    prms.Add("@IST_ACIKLAMA", entity.IST_ACIKLAMA);
                    prms.Add("@IST_SONUC", entity.IST_SONUC);
                    prms.Add("@IST_AKTIF", entity.IST_AKTIF);
                    prms.Add("@IST_BIRLESIM_ID", entity.IST_BIRLESIM_ID);
                    prms.Add("@IST_ACILIS_NEDEN", entity.IST_ACILIS_NEDEN);
                    prms.Add("@IST_DEGISTIREN_ID", entity.IST_DEGISTIREN_ID);
                    prms.Add("@IST_DEGISTIRME_TARIH", DateTime.Now);
                    prms.Add("@IST_IS_DEVAM_DURUM_ID", entity.IST_IS_DEVAM_DURUM_ID);
                    prms.Add("@IST_DEPARTMAN_ID", entity.IST_DEPARTMAN_ID);
                    klas.cmd(query, prms.PARAMS);

                    #endregion

                    if (eskiDurumId == 0 && entity.IST_DURUM_ID == 1)
                    {
                        //okundu log
                        string queryOkundu = @"INSERT INTO [orjin].[TB_IS_TALEBI_LOG]
                                           ([ITL_IS_TANIM_ID]
                                           ,[ITL_KULLANICI_ID]
                                           ,[ITL_TARIH]
                                           ,[ITL_SAAT]
                                           ,[ITL_ISLEM]
                                           ,[ITL_ACIKLAMA]
                                           ,[ITL_ISLEM_DURUM]
                                           ,[ITL_TALEP_ISLEM]
                                           ,[ITL_DEGISTIREN_ID]
                                           ,[ITL_DEGISTIRME_TARIH])
                                     VALUES
                                           (@ITL_IS_TANIM_ID,
                                            @ITL_KULLANICI_ID,
                                            @ITL_TARIH, 
                                            @ITL_SAAT,
                                            @ITL_ISLEM, 
                                            @ITL_ACIKLAMA, 
                                            @ITL_ISLEM_DURUM, 
                                            @ITL_TALEP_ISLEM,
                                            @ITL_DEGISTIREN_ID,
                                            @ITL_DEGISTIRME_TARIH)";
                        parametreler.Clear();
                        parametreler.Add(new Prm("KUL_ID", entity.IST_DEGISTIREN_ID));
                        string KLL_TANIM = klas.GetDataCell("select KLL_TANIM from orjin.TB_KULLANICI where TB_KULLANICI_ID=@KUL_ID", parametreler);

                        parametreler.Clear();
                        DataRow drSonTalep = klas.GetDataRow("SELECT TOP 1 * FROM orjin.TB_IS_TALEBI ORDER BY  TB_IS_TALEP_ID DESC", parametreler);
                        if (drSonTalep != null)
                        {
                            prms.Clear();
                            prms.Add("ITL_IS_TANIM_ID", Convert.ToInt32(drSonTalep["TB_IS_TALEP_ID"]));
                            prms.Add("ITL_KULLANICI_ID", entity.IST_DEGISTIREN_ID);
                            prms.Add("ITL_TARIH", DateTime.Now);
                            prms.Add("ITL_SAAT", DateTime.Now.ToString(C.DB_TIME_FORMAT));
                            prms.Add("ITL_ISLEM", "Okundu");
                            klas.MasterBaglantisi = true;
                            prms.Add("ITL_ACIKLAMA", String.Format("'{0}' tarafından okundu", KLL_TANIM));
                            prms.Add("ITL_ISLEM_DURUM", "BEKLIYOR");
                            prms.Add("ITL_TALEP_ISLEM", "Okundu");
                            prms.Add("ITL_DEGISTIREN_ID", entity.IST_DEGISTIREN_ID);
                            prms.Add("ITL_DEGISTIRME_TARIH", DateTime.Now);
                            klas.cmd(queryOkundu, prms.PARAMS);

                        }
                    }
                }
            }
            catch (Exception)
            {
                klas.kapat();
            }
        }

        [Route("api/IsTalepTarihce")]
        public List<IsTalebiLog> GetIsTalepTarihceByID([FromUri] int talepID)
        {
            parametreler.Clear();
            parametreler.Add(new Prm("ITL_IS_TANIM_ID", talepID));
            string sql = "select * from orjin.TB_IS_TALEBI_LOG where ITL_IS_TANIM_ID=@ITL_IS_TANIM_ID";
            List<IsTalebiLog> listem = new List<IsTalebiLog>();
            DataTable dt = klas.GetDataTable(sql, parametreler);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IsTalebiLog entity = new IsTalebiLog();
                entity.ITL_ACIKLAMA = dt.Rows[i]["ITL_ACIKLAMA"] != DBNull.Value ? dt.Rows[i]["ITL_ACIKLAMA"].ToString() : "";
                entity.TB_IS_TALEP_LOG_ID = Util.getFieldInt(dt.Rows[i], "TB_IS_TALEP_LOG_ID");
                entity.ITL_KULLANICI_ID = Util.getFieldInt(dt.Rows[i], "ITL_KULLANICI_ID");
                entity.ITL_IS_TANIM_ID = Util.getFieldInt(dt.Rows[i], "ITL_IS_TANIM_ID");
                entity.ITL_TARIH = Util.getFieldDateTime(dt.Rows[i], "ITL_TARIH");
                entity.ITL_TALEP_ISLEM = Util.getFieldString(dt.Rows[i], "ITL_TALEP_ISLEM");
                entity.ITL_SAAT = Util.getFieldString(dt.Rows[i], "ITL_SAAT");
                entity.ITL_ISLEM_DURUM = Util.getFieldString(dt.Rows[i], "ITL_ISLEM_DURUM");
                entity.ITL_ISLEM = Util.getFieldString(dt.Rows[i], "ITL_ISLEM");
                entity.ITL_ACIKLAMA = Util.getFieldString(dt.Rows[i], "ITL_ACIKLAMA");
                listem.Add(entity);
            }

            return listem;
        }

		// Mobilden gelen istekte sadece bir teknesiyen seçilebilir
		[Route("api/IsEmriOlustur")]
        [HttpGet]
        public async Task<string> IsEmriOlustur([FromUri] int talepID, [FromUri] int userId, [FromUri] int teknisyenId, [FromUri] int atolyeId)
        {
			string isemriNo = "";
            try
            {
                parametreler.Clear();
                parametreler.Add(new Prm("TB_IS_TALEP_ID", talepID));
                DataRow drTalep = klas.GetDataRow("select * from orjin.VW_IS_TALEP where TB_IS_TALEP_ID = @TB_IS_TALEP_ID", parametreler);
                parametreler.Clear();
                parametreler.Add(new Prm("TB_MAKINE_ID", drTalep["IST_MAKINE_ID"]));
                DataRow drMakine = klas.GetDataRow("select * from orjin.VW_MAKINE where TB_MAKINE_ID = @TB_MAKINE_ID ", parametreler);
                IsEmri entity = new IsEmri();
                IsEmriController ismCont = new IsEmriController(_logger);
                entity.ISM_DUZENLEME_TARIH = DateTime.Now;
                entity.ISM_DUZENLEME_SAAT = DateTime.Now.ToString(C.DB_TIME_FORMAT);
                entity.ISM_ATOLYE_ID = atolyeId;
                entity.ISM_ONCELIK_ID = Util.getFieldInt(drTalep, "IST_ONCELIK_ID");
                entity.ISM_BILDIREN = Util.getFieldString(drTalep, "IST_TALEP_EDEN_ADI");
                entity.ISM_IS_TARIH = Util.getFieldDateTime(drTalep, "IST_ACILIS_TARIHI");
                entity.ISM_IS_SAAT = Util.getFieldString(drTalep, "IST_ACILIS_SAATI");
                entity.ISM_IS_SONUC = Util.getFieldString(drTalep, "IST_ACIKLAMA");
                entity.ISM_ISEMRI_NO = (await ismCont.GetIsEmriKodGetir()).Tanim;

                entity.ISM_DUZENLEME_TARIH = DateTime.Now;
                entity.ISM_DUZENLEME_SAAT = DateTime.Now.ToString(C.DB_TIME_FORMAT);
                entity.ISM_TIP_ID = Util.getFieldInt(drTalep, "IST_ISEMRI_TIP_ID");
                entity.ISM_BASLAMA_TARIH = DateTime.Now;
                entity.ISM_BASLAMA_SAAT = DateTime.Now.ToString(C.DB_TIME_FORMAT);
                entity.ISM_BITIS_TARIH = null;
                entity.ISM_BITIS_SAAT = "";
                entity.ISM_REF_ID = -1;
                entity.ISM_REF_GRUP = "";
                parametreler.Clear();
                entity.ISM_DURUM_KOD_ID = Convert.ToInt32(klas.GetDataCell("SELECT TB_KOD_ID FROM orjin.TB_KOD WHERE KOD_GRUP='32801' AND KOD_TANIM='AÇIK'", parametreler));
                if (drMakine != null)
                {
                    entity.ISM_LOKASYON_ID = Util.getFieldInt(drMakine, "MKN_LOKASYON_ID");
                    entity.ISM_MAKINE_ID = Util.getFieldInt(drMakine, "TB_MAKINE_ID");
                    entity.ISM_PROJE_ID = Util.getFieldInt(drMakine, "MKN_PROJE_ID");
                    entity.ISM_MASRAF_MERKEZ_ID = Util.getFieldInt(drMakine, "MKN_MASRAF_MERKEZ_KOD_ID");
                    entity.ISM_MAKINE_DURUM_KOD_ID = Util.getFieldInt(drTalep, "IST_MAKINE_DURUM_KOD_ID");
                    entity.ISM_MAKINE_GUVENLIK_NOTU = Util.getFieldString(drMakine, "MKN_GUVENLIK_NOTU");
                }
                else
                {
                    entity.ISM_LOKASYON_ID = Util.getFieldInt(drTalep, "IST_BILDIREN_LOKASYON_ID");
                    entity.ISM_MAKINE_ID = -1;
                    entity.ISM_PROJE_ID = -1;
                    entity.ISM_MAKINE_GUVENLIK_NOTU = "";
                    entity.ISM_MASRAF_MERKEZ_ID = -1;
                    entity.ISM_MAKINE_DURUM_KOD_ID = -1;
                }
                entity.ISM_EKIPMAN_ID = Util.getFieldInt(drTalep, "IST_EKIPMAN_ID");


                entity.ISM_KONU = Util.getFieldString(drTalep, "IST_TANIMI");
                entity.ISM_ATOLYE_ID = atolyeId;

                entity.ISM_OLUSTURAN_ID = userId;
                entity.ISM_ACIKLAMA = String.Format("'{0}' koldu iş talebi", drTalep["IST_KOD"].ToString());
				await ismCont.Post(entity, userId);
                parametreler.Clear();
                int _isemriID = Convert.ToInt32(klas.GetDataCell("SELECT MAX(TB_ISEMRI_ID) FROM orjin.TB_ISEMRI", parametreler));
                // iş talep durumu değiştiriliyor.
                parametreler.Clear();
                parametreler.Add(new Prm("TB_IS_TALEP_ID", drTalep["TB_IS_TALEP_ID"]));
                parametreler.Add(new Prm("IST_ISEMRI_ID", _isemriID));
                klas.cmd("UPDATE orjin.TB_IS_TALEBI SET IST_ISEMRI_ID=@IST_ISEMRI_ID , IST_DURUM_ID=3 WHERE TB_IS_TALEP_ID= @TB_IS_TALEP_ID", parametreler);

                // iş talep personel ekleniyor
                if (teknisyenId != -1)
                {
                    parametreler.Clear();
                    parametreler.Add(new Prm("TB_IS_TALEP_ID", drTalep["TB_IS_TALEP_ID"]));
                    parametreler.Add(new Prm("ITK_TEKNISYEN_ID", teknisyenId));
                    klas.cmd("INSERT INTO orjin.TB_IS_TALEBI_TEKNISYEN (ITK_IS_TALEBI_ID,ITK_TEKNISYEN_ID,ITK_FAZLA_MESAI_VAR,ITK_MAIL_GONDERILDI) VALUES(@TB_IS_TALEP_ID,@ITK_TEKNISYEN_ID,0,0)", parametreler);
                    // iş emri personel i ekleniyor
                    IsEmriPersonel ismPersonel = new IsEmriPersonel();
                    ismPersonel.IDK_ISEMRI_ID = _isemriID;
                    ismPersonel.IDK_REF_ID = teknisyenId;
                    ismPersonel.IDK_OLUSTURAN_ID = userId;
                    ismCont.PersonelListKaydet(ismPersonel);
                }
                // iş talep log yazılıyor
                //atama logu
                string query = @"INSERT INTO [orjin].[TB_IS_TALEBI_LOG]
                                           ([ITL_IS_TANIM_ID]
                                           ,[ITL_KULLANICI_ID]
                                           ,[ITL_TARIH]
                                           ,[ITL_SAAT]
                                           ,[ITL_ISLEM]
                                           ,[ITL_ACIKLAMA]
                                           ,[ITL_ISLEM_DURUM]
                                           ,[ITL_TALEP_ISLEM]
                                           ,[ITL_OLUSTURAN_ID]
                                           ,[ITL_OLUSTURMA_TARIH])
                                     VALUES
                                           (@ITL_IS_TANIM_ID,
                                            @ITL_KULLANICI_ID,
                                            @ITL_TARIH, 
                                            @ITL_SAAT,
                                            @ITL_ISLEM, 
                                            @ITL_ACIKLAMA, 
                                            @ITL_ISLEM_DURUM, 
                                            @ITL_TALEP_ISLEM,
                                            @ITL_OLUSTURAN_ID,
                                            @ITL_OLUSTURMA_TARIH)";
                parametreler.Clear();
                parametreler.Add(new Prm("TB_PERSONEL_ID", teknisyenId));
                string PRS_ISIM = klas.GetDataCell("select PRS_ISIM from orjin.TB_PERSONEL where TB_PERSONEL_ID=@TB_PERSONEL_ID", parametreler);
                prms.Clear();

                prms.Add("ITL_IS_TANIM_ID", talepID);
                prms.Add("ITL_KULLANICI_ID", userId);
                prms.Add("ITL_TARIH", DateTime.Now);
                prms.Add("ITL_SAAT", DateTime.Now.ToString(C.DB_TIME_FORMAT));
                if (atolyeId != -1)
                {
                    AtolyeController atlCont = new AtolyeController();
                    string atolyeTanim = atlCont.AtolyeListesi(userId).FirstOrDefault(a => a.TB_ATOLYE_ID == atolyeId).ATL_TANIM;
                    prms.Add("ITL_ISLEM", "Atölye Ataması");
                    prms.Add("ITL_TALEP_ISLEM", "Atölye Ataması");
                    prms.Add("ITL_ACIKLAMA", String.Format("Atölye: {0}  İş Emri Numarası: {1}", atolyeTanim, entity.ISM_ISEMRI_NO));
                }
                else
                {
                    prms.Add("ITL_ISLEM", "Teknisyen Ataması");
                    prms.Add("ITL_TALEP_ISLEM", "Teknisyen Ataması");
                    prms.Add("ITL_ACIKLAMA", String.Format("Teknisyen : {0} İş Emri Numarası: {1}", PRS_ISIM, entity.ISM_ISEMRI_NO));
                }
                prms.Add("ITL_ISLEM_DURUM", "DEVAM EDIYOR");
                prms.Add("ITL_OLUSTURAN_ID", userId);
                prms.Add("ITL_OLUSTURMA_TARIH", DateTime.Now);
                klas.cmd(query, prms.PARAMS);
                isemriNo = entity.ISM_ISEMRI_NO;
                return isemriNo;
            }
            catch (Exception)
            {
                klas.kapat();
                return isemriNo;
            }

        }

        [Route("api/IsTalepNotEkle")]
        [HttpPost]
        public void IsTalepNotEkle([FromUri] int talepID, [FromUri] string Not)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                parametreler.Clear();
                parametreler.Add(new Prm("TB_IS_TALEP_ID", talepID));
                parametreler.Add(new Prm("IST_NOT", Not));
                klas.cmd("UPDATE orjin.TB_IS_TALEBI SET IST_NOT = @IST_NOT WHERE TB_IS_TALEP_ID =@TB_IS_TALEP_ID ", parametreler);
                bildirimEntity.Aciklama = "İşlem başarılı bir şekilde gerçekleşti.";
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_BASARILI;
                bildirimEntity.Durum = true;
            }
            catch (Exception e)
            {
                bildirimEntity.Aciklama = string.Format(Localization.errorFormatted, e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }
        }


        public static void IsTalepTarihceYaz(int talepID, int kulId, string islem, string aciklama, string durum)
        {
            Util klas = new Util();
            try
            {

                DataRow drSonTalep = klas.GetDataRow("SELECT TOP 1 * FROM orjin.TB_IS_TALEBI ORDER BY  TB_IS_TALEP_ID DESC", new List<Prm>());
                //okundu log
                string query = @"INSERT INTO [orjin].[TB_IS_TALEBI_LOG]
                                           ([ITL_IS_TANIM_ID]
                                           ,[ITL_KULLANICI_ID]
                                           ,[ITL_TARIH]
                                           ,[ITL_SAAT]
                                           ,[ITL_ISLEM]
                                           ,[ITL_ACIKLAMA]
                                           ,[ITL_ISLEM_DURUM]
                                           ,[ITL_TALEP_ISLEM]
                                           ,[ITL_OLUSTURAN_ID]
                                           ,[ITL_OLUSTURMA_TARIH])
                                     VALUES
                                           (@ITL_IS_TANIM_ID,
                                            @ITL_KULLANICI_ID,
                                            @ITL_TARIH, 
                                            @ITL_SAAT,
                                            @ITL_ISLEM, 
                                            @ITL_ACIKLAMA, 
                                            @ITL_ISLEM_DURUM, 
                                            @ITL_TALEP_ISLEM,
                                            @ITL_OLUSTURAN_ID,
                                            @ITL_OLUSTURMA_TARIH)";


                if (drSonTalep != null)
                {
                    List<Prm> prmList = new List<Prm>();
                    prmList.Add(new Prm("ITL_IS_TANIM_ID", talepID));
                    prmList.Add(new Prm("ITL_KULLANICI_ID", kulId));
                    prmList.Add(new Prm("ITL_TARIH", DateTime.Now));
                    prmList.Add(new Prm("ITL_SAAT", DateTime.Now.ToString(C.DB_TIME_FORMAT)));
                    prmList.Add(new Prm("ITL_ISLEM", islem));
                    prmList.Add(new Prm("ITL_ACIKLAMA", aciklama));
                    prmList.Add(new Prm("ITL_ISLEM_DURUM", durum));
                    prmList.Add(new Prm("ITL_TALEP_ISLEM", islem));
                    prmList.Add(new Prm("ITL_OLUSTURAN_ID", kulId));
                    prmList.Add(new Prm("ITL_OLUSTURMA_TARIH", DateTime.Now));
                    klas.cmd(query, prmList);
                }

            }
            catch (Exception)
            {
                klas.kapat();

                throw;
            }
        }

        [HttpGet]
        [Route("Api/IsTalep/TalepEkleData")]
        public IsTalepEkleData GetTalepEkleData([FromUri] int ID, [FromUri] bool isUpdate)
        {
            var dynamicParams = new DynamicParameters();
            dynamicParams.Add("KUL_ID", ID);
            dynamicParams.Add("KOD", "IST_KOD");
            var istKodQuery = Queries.GENERATE_KOD;
            var wReqUsersQuery = @"SELECT * FROM orjin.TB_IS_TALEBI_KULLANICI;";
            var locationsQuery = @"SELECT * FROM orjin.TB_LOKASYON WHERE orjin.UDF_LOKASYON_YETKI_KONTROL(TB_LOKASYON_ID,@KUL_ID) = 1;";
            var wReqTypesQuery = @"SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP= '32951' AND KOD_AKTIF = 1;";
            var wReqParamQuery = @"SELECT TOP 1 * FROM orjin.TB_IS_TALEBI_PARAMETRE;";
            var sb = new StringBuilder();
            if (!isUpdate)
                sb.Append(istKodQuery);
            sb.Append(wReqUsersQuery);
            sb.Append(locationsQuery);
            sb.Append(wReqTypesQuery);
            sb.Append(wReqParamQuery);
            var util = new Util();
            var isTalepEkleData = new IsTalepEkleData();
            using (var cnn = util.baglan())
            {
                var result = cnn.QueryMultiple(sb.ToString(), dynamicParams);
                if (!isUpdate)
                    isTalepEkleData.IsTalepKod = result.ReadSingleOrDefault<String>();
                isTalepEkleData.TalepKullanicilari = result.Read<TalepKullanici>().ToList();
                isTalepEkleData.Lokasyonlar = result.Read<Lokasyon>().ToList();
                isTalepEkleData.IsTalepTipleri = result.Read<Kod>().ToList();
                isTalepEkleData.Parametre = result.ReadFirstOrDefault<IsTalepParametre>();
            }

            return isTalepEkleData;
        }

        //Is Talebi Post Method For Web Form
		[HttpGet]
		[Route("api/postIsTalebi")]
		public string postIsTalebi(
            [FromUri] int istKodNumber,
            [FromUri] string istKod ,  
            [FromUri] string tanimi , 
            [FromUri] string konu ,  
            [FromUri] string telNo , 
            [FromUri] string mail ,
			[FromUri] int talepEdenId,
            [FromUri] int lokasyonId ,
            [FromUri] string birimBolum ,
            [FromUri] string birimBolumYetkilisi
			)
		{
			try
			{
				string query1 = @" insert into orjin.TB_IS_TALEBI 
                            (IST_KOD,IST_BILDIREN_LOKASYON_ID,IST_DURUM_ID,IST_ACILIS_TARIHI,IST_ACILIS_SAATI,IST_TANIMI,IST_ACIKLAMA,IST_IRTIBAT_TELEFON,
                            IST_MAIL_ADRES,IST_TALEP_EDEN_ID,IST_IS_TAKIPCISI_ID,IST_TALEPEDEN_LOKASYON_ID,IST_BIRIM_BOLUM,IST_BIRIM_BOLUM_YETKILISI)

                              values(@IST_KOD,@IST_BILDIREN_LOKASYON_ID,1,(select convert (varchar(10) , GETDATE() , 101)),
                                    (select convert (varchar(8) , GETDATE() , 108)),@IST_TANIMI,
                                @IST_ACIKLAMA,@IST_IRTIBAT_TELEFON,@IST_MAIL_ADRES,@IST_TALEP_EDEN_ID,@IST_IS_TAKIPCISI_ID,@IST_TALEPEDEN_LOKASYON_ID,@IST_BIRIM_BOLUM,@IST_BIRIM_BOLUM_YETKILISI) ";
				prms.Clear();
				prms.Add("IST_KOD", istKod);
				prms.Add("IST_BILDIREN_LOKASYON_ID", lokasyonId);
				prms.Add("IST_TANIMI", konu);
				prms.Add("IST_ACIKLAMA", tanimi);
				prms.Add("IST_IRTIBAT_TELEFON", telNo);
				prms.Add("IST_MAIL_ADRES", mail);
				prms.Add("IST_TALEP_EDEN_ID", talepEdenId);
				prms.Add("IST_IS_TAKIPCISI_ID", talepEdenId);
				prms.Add("IST_TALEPEDEN_LOKASYON_ID", lokasyonId);
				prms.Add("IST_BIRIM_BOLUM", birimBolum);
				prms.Add("IST_BIRIM_BOLUM_YETKILISI", birimBolumYetkilisi);

				klas.cmd(query1, prms.PARAMS);

				using(var cnn = klas.baglan())
                {
					int SonTalepId = cnn.QueryFirstOrDefault<int>("SELECT TOP 1 TB_IS_TALEP_ID FROM orjin.TB_IS_TALEBI ORDER BY  TB_IS_TALEP_ID DESC");
					if (SonTalepId > 0)
					{
						var itl = new IsTalebiLog
						{
							ITL_IS_TANIM_ID = SonTalepId,
							ITL_KULLANICI_ID = talepEdenId,
							ITL_TARIH = DateTime.Now,
							ITL_SAAT = DateTime.Now.ToString(C.DB_TIME_FORMAT),
							ITL_ISLEM = "Yeni iş talebi",
							ITL_ACIKLAMA = String.Format("Talep no: '{0}' - Konu :'{1}'", istKod,konu),
							ITL_ISLEM_DURUM = "AÇIK",
							ITL_TALEP_ISLEM = "Yeni İş Talebi",
							ITL_OLUSTURAN_ID = talepEdenId,
							ITL_OLUSTURMA_TARIH = DateTime.Now
						};
						cnn.Insert(itl);
					}
				}

				string query2 = @" update orjin.TB_NUMARATOR set NMR_NUMARA = @NMR_NUMARA where NMR_KOD = 'IST_KOD' ";
				prms.Clear();
				prms.Add("NMR_NUMARA", (istKodNumber+1));
				klas.cmd(query2, prms.PARAMS);
				return "true";
			}
			catch (Exception e)
			{
				klas.kapat();
                return "Ekleme başarısız !";
			}
		}

        [Route("api/talepIptalEt")]
        [HttpGet]
		public void IsTalepIptalEt([FromUri] int talepID, [FromUri] int userId, [FromUri] string talepNo, [FromUri] string userName)
		{
			    string isTalepLogQuery =
				    @"
                    insert into orjin.TB_IS_TALEBI_LOG (
                    ITL_IS_TANIM_ID,
                    ITL_KULLANICI_ID,
                    ITL_TARIH,
                    ITL_SAAT,
                    ITL_ISLEM,
                    ITL_ACIKLAMA,
                    ITL_ISLEM_DURUM,
                    ITL_TALEP_ISLEM,
                    ITL_OLUSTURAN_ID ) values (";

			    isTalepLogQuery += $" {talepID} , ";
			    isTalepLogQuery += $" {userId} , ";
			    isTalepLogQuery += $" '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , "; // Changed date format
			    isTalepLogQuery += $" '{DateTime.Now.ToString("HH:mm:ss")}' , ";
			    isTalepLogQuery += " 'İptal' , ";
			    isTalepLogQuery += $" 'Talep no : {talepNo} - Konu : {userName} tarafından iptal edildi' , ";
			    isTalepLogQuery += " 'İPTAL EDİLDİ' , ";
			    isTalepLogQuery += " 'İptal' , ";
			    isTalepLogQuery += $" {userId} )";

			    try
			    {
				    var util = new Util();
				    using (var cnn = util.baglan())
				    {
					    var parametreler = new DynamicParameters();
					    parametreler.Add("IS_TALEP_ID", talepID);
					    // Log data is recorded
					    cnn.Execute(isTalepLogQuery, parametreler);
					    // Job request status is being canceled
					    cnn.Execute("update orjin.TB_IS_TALEBI set IST_DURUM_ID = 5 WHERE TB_IS_TALEP_ID = @IS_TALEP_ID", parametreler);
				    }
			    }
			    catch (Exception)
			    {
				    throw;
			    }
		    }

	}
}
