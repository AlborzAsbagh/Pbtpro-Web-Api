using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using System.Windows.Forms;
using Dapper;
using Dapper.Contrib.Extensions;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    
    [MyBasicAuthenticationFilter]
    public class MakineController : ApiController
    {
        Util klas = new Util();
        Parametreler prms = new Parametreler();
        
        [Route("api/Makine")]
        [HttpGet]
        public object GetViaDapper([FromUri] int ilkDeger, [FromUri] int sonDeger, [FromUri] int ID, [FromUri] int Tip, [FromUri] int kategori, [FromUri] string prm, [FromUri] Boolean b, [FromUri] int lokasyonID, [FromUri]int YakitKullanimi,[FromUri] Boolean otonombakim)
        {
            var prms = new DynamicParameters();
            string query = @";WITH MTABLE AS(
                                SELECT 
                                TB_MAKINE_ID
                                ,MKN.MKN_KOD 					
                                ,MKN_TANIM 					 					
                                ,MKN_TIP_KOD_ID 					
                                ,TIP.KOD_TANIM MKN_TIP 					
                                ,LOK.LOK_TANIM MKN_LOKASYON 				
                                ,OPR.PRS_ISIM MKN_OPERATOR 				
                                ,MKN_OPERATOR_PERSONEL_ID 	
                                ,LOK.LOK_TUM_YOL MKN_TAM_LOKASYON 			
                                ,MES.MES_GUNCEL_DEGER MKN_SAYAC_GUNCEL_DEGER 		
                                ,MES.MES_SAYAC_BIRIM MKN_SAYAC_BIRIM 			
                                ,KTG.KOD_TANIM MKN_KATEGORI 
                                ,(SELECT COUNT(TB_ISEMRI_ID) FROM orjin.TB_ISEMRI WHERE ISM_MAKINE_ID= TB_MAKINE_ID AND ISM_KAPATILDI=0) MKN_ACIK_ISEMRI_SAYISI
                                ,(SELECT COUNT(TB_ISEMRI_ID) FROM orjin.TB_ISEMRI WHERE ISM_MAKINE_ID= TB_MAKINE_ID AND ISM_KAPATILDI=1) MKN_KAPALI_ISEMRI_SAYISI
                                ,(SELECT COUNT(TB_IS_TALEP_ID) FROM orjin.TB_IS_TALEBI WHERE IST_MAKINE_ID= TB_MAKINE_ID AND IST_DURUM_ID IN (0,1)) MKN_ACIK_ISTALEP_SAYISI
                                ,(SELECT COUNT(TB_IS_TALEP_ID) FROM orjin.TB_IS_TALEBI WHERE IST_MAKINE_ID= TB_MAKINE_ID AND IST_DURUM_ID NOT IN (0,1)) MKN_KAPALI_ISTALEP_SAYISI 
                                ,MKN_DURUM_KOD_ID				
                                ,DRM.KOD_TANIM MKN_DURUM 					
                                ,MRK.KOD_TANIM MKN_MARKA 					
                                ,MDL.KOD_TANIM MKN_MODEL 					
                                ,MKN_LOKASYON_ID 			
                                ,MKN_URETIM_YILI 			
                                ,MKN_SERI_NO 				
                                ,ARC.ARC_PLAKA MKN_ARAC_PLAKA 				
                                ,PRJ.PRJ_TANIM MKN_PROJE 					
                                ,MKN_PROJE_ID
                                ,(select COALESCE(TB_RESIM_ID,0) from orjin.TB_RESIM where RSM_VARSAYILAN = 1 AND RSM_REF_GRUP = 'MAKINE' and RSM_REF_ID = TB_MAKINE_ID) as RSM_VAR_ID 
                                ,stuff((SELECT ';' + CONVERT(varchar(11), R.TB_RESIM_ID)    FROM orjin.TB_RESIM R    WHERE R.RSM_REF_GRUP = 'MAKINE' and R.RSM_REF_ID = TB_MAKINE_ID    FOR XML PATH('')), 1, 1, '') [RSM_IDS]	
                                ,ROW_NUMBER() OVER (ORDER BY MKN.TB_MAKINE_ID) AS RowNum 	
                                FROM orjin.TB_MAKINE MKN
                                LEFT JOIN orjin.TB_LOKASYON LOK on MKN.MKN_LOKASYON_ID = LOK.TB_LOKASYON_ID
                                LEFT JOIN orjin.TB_KOD MRK ON MKN.MKN_MARKA_KOD_ID=MRK.TB_KOD_ID
                                LEFT JOIN orjin.TB_KOD MDL ON MKN.MKN_MODEL_KOD_ID=MDL.TB_KOD_ID
                                LEFT JOIN orjin.TB_KOD KTG ON MKN.MKN_KATEGORI_KOD_ID=KTG.TB_KOD_ID
                                LEFT JOIN orjin.TB_KOD DRM ON MKN.MKN_DURUM_KOD_ID=DRM.TB_KOD_ID
                                LEFT JOIN orjin.TB_KOD TIP ON MKN.MKN_TIP_KOD_ID=TIP.TB_KOD_ID
                                LEFT JOIN orjin.TB_PROJE PRJ ON MKN.MKN_PROJE_ID=PRJ.TB_PROJE_ID
                                LEFT JOIN orjin.TB_ARAC ARC ON MKN.TB_MAKINE_ID=ARC.ARC_MAKINE_ID
                                LEFT JOIN orjin.VW_SAYAC MES ON (MES.MES_REF_ID = MKN.TB_MAKINE_ID) AND (MES_VARSAYILAN = 1)
                                LEFT JOIN orjin.TB_PERSONEL OPR ON MKN.MKN_OPERATOR_PERSONEL_ID=OPR.TB_PERSONEL_ID
                                WHERE
                                MKN.MKN_AKTIF = 1 AND
                                orjin.UDF_LOKASYON_YETKI_KONTROL(ISNULL(MKN_LOKASYON_ID,-1),@KUL_ID) = 1 AND 
                                orjin.UDF_ATOLYE_YETKI_KONTROL(ISNULL(MKN_ATOLYE_ID,-1), @KUL_ID ) = 1 ";
            if (Tip != -1)
            {
                prms.Add("MKN_TIP_KOD_ID", Tip);
                query = query + " and MKN_TIP_KOD_ID = @MKN_TIP_KOD_ID";
            }
            if (kategori != -1)
            {
                prms.Add("MKN_KATEGORI_KOD_ID", kategori);
                query = query + " and MKN_KATEGORI_KOD_ID = @MKN_KATEGORI_KOD_ID";
            }
            if (YakitKullanimi > -1)
            {
                prms.Add("MKN_YAKIT_KULLANIM", YakitKullanimi);
                query = query + " and MKN_YAKIT_KULLANIM = @MKN_YAKIT_KULLANIM";
            }
            if (otonombakim)
            {
                query = query + " and MKN_OTONOM_BAKIM = 1";
            }
            if (lokasyonID != -1)
            {
                prms.Add("MKN_LOKASYON_ID", lokasyonID);
                query = query + " and MKN_LOKASYON_ID IN (select (TB_LOKASYON_ID) from orjin.UDF_LOKASYON_ALT_AGAC(@MKN_LOKASYON_ID))";
            }
            if (!String.IsNullOrEmpty(prm))
            {
                if (b)
                {
                    prms.Add("MKN_KOD", prm);
                    query = query + " and MKN_KOD = @MKN_KOD";
                }
                else
                {
                    prms.Add("KELIME", prm);
                    query = query + @" and (
                    MKN_KOD         LIKE '%'+@KELIME+'%' OR 
                    ARC.ARC_PLAKA   LIKE '%'+@KELIME+'%' OR 
                    LOK.LOK_TANIM   LIKE '%'+@KELIME+'%' OR 
                    MKN_TANIM       LIKE '%'+@KELIME+'%' OR 
                    MRK.KOD_TANIM   LIKE '%'+@KELIME+'%' OR 
                    KTG.KOD_TANIM   LIKE '%'+@KELIME+'%' OR 
                    DRM.KOD_TANIM   LIKE '%'+@KELIME+'%' OR 
                    '*' = @KELIME)";
                }
            }
            prms.Add("ILK_DEGER", ilkDeger);
            prms.Add("SON_DEGER", sonDeger);
            prms.Add("KUL_ID", ID);
            query = query + @")  SELECT * FROM MTABLE WHERE RowNum > @ILK_DEGER AND RowNum <= @SON_DEGER";



            IEnumerable<Makine> listem = new List<Makine>();
            try
            {
                using (var conn = klas.baglan())
                {
                    listem = conn.Query<Makine>(query, prms);
                }
            }
            catch (Exception e)
            {
                return e;
            }
            return listem;
        }

        [Route("api/MakineBakim")]
        [HttpGet]
        public object GetMknBkm([FromUri] int ilkDeger, [FromUri] int sonDeger, [FromUri] int ID, [FromUri] int Tip, [FromUri] int kategori, [FromUri] string prm, [FromUri] Boolean b, [FromUri] int lokasyonID, [FromUri] int YakitKullanimi, [FromUri] Boolean otonombakim, [FromUri] int prsId)
        {
            var prms = new DynamicParameters();
            string query = @";WITH MTABLE AS(
                                SELECT 
                                TB_MAKINE_ID
                                ,MKN.MKN_KOD 					
                                ,MKN_TANIM 					 					
                                ,MKN_TIP_KOD_ID 					
                                ,TIP.KOD_TANIM MKN_TIP 					
                                ,LOK.LOK_TANIM MKN_LOKASYON 				
                                ,OPR.PRS_ISIM MKN_OPERATOR 				
                                ,MKN_OPERATOR_PERSONEL_ID 	
                                ,LOK.LOK_TUM_YOL MKN_TAM_LOKASYON 			
                                ,MES.MES_GUNCEL_DEGER MKN_SAYAC_GUNCEL_DEGER 		
                                ,MES.MES_SAYAC_BIRIM MKN_SAYAC_BIRIM 			
                                ,KTG.KOD_TANIM MKN_KATEGORI 
                                ,(SELECT COUNT(TB_ISEMRI_ID) FROM orjin.TB_ISEMRI WHERE ISM_MAKINE_ID= TB_MAKINE_ID AND ISM_KAPATILDI=0) MKN_ACIK_ISEMRI_SAYISI
                                ,(SELECT COUNT(TB_ISEMRI_ID) FROM orjin.TB_ISEMRI WHERE ISM_MAKINE_ID= TB_MAKINE_ID AND ISM_KAPATILDI=1) MKN_KAPALI_ISEMRI_SAYISI
                                ,(SELECT COUNT(TB_IS_TALEP_ID) FROM orjin.TB_IS_TALEBI WHERE IST_MAKINE_ID= TB_MAKINE_ID AND IST_DURUM_ID IN (0,1)) MKN_ACIK_ISTALEP_SAYISI
                                ,(SELECT COUNT(TB_IS_TALEP_ID) FROM orjin.TB_IS_TALEBI WHERE IST_MAKINE_ID= TB_MAKINE_ID AND IST_DURUM_ID NOT IN (0,1)) MKN_KAPALI_ISTALEP_SAYISI 
                                ,MKN_DURUM_KOD_ID				
                                ,DRM.KOD_TANIM MKN_DURUM 					
                                ,MRK.KOD_TANIM MKN_MARKA 					
                                ,MDL.KOD_TANIM MKN_MODEL 					
                                ,MKN_LOKASYON_ID 			
                                ,MKN_URETIM_YILI 			
                                ,MKN_SERI_NO 				
                                ,ARC.ARC_PLAKA MKN_ARAC_PLAKA 				
                                ,PRJ.PRJ_TANIM MKN_PROJE 					
                                ,MKN_PROJE_ID
                                ,(select COALESCE(TB_RESIM_ID,0) from orjin.TB_RESIM where RSM_VARSAYILAN = 1 AND RSM_REF_GRUP = 'MAKINE' and RSM_REF_ID = TB_MAKINE_ID) as RSM_VAR_ID 
                                ,stuff((SELECT ';' + CONVERT(varchar(11), R.TB_RESIM_ID)    FROM orjin.TB_RESIM R    WHERE R.RSM_REF_GRUP = 'MAKINE' and R.RSM_REF_ID = TB_MAKINE_ID    FOR XML PATH('')), 1, 1, '') [RSM_IDS]	
                                ,ROW_NUMBER() OVER (ORDER BY MKN.TB_MAKINE_ID) AS RowNum 	
                                FROM orjin.TB_MAKINE MKN
                                LEFT JOIN orjin.TB_LOKASYON LOK on MKN.MKN_LOKASYON_ID = LOK.TB_LOKASYON_ID
                                LEFT JOIN orjin.TB_KOD MRK ON MKN.MKN_MARKA_KOD_ID=MRK.TB_KOD_ID
                                LEFT JOIN orjin.TB_KOD MDL ON MKN.MKN_MODEL_KOD_ID=MDL.TB_KOD_ID
                                LEFT JOIN orjin.TB_KOD KTG ON MKN.MKN_KATEGORI_KOD_ID=KTG.TB_KOD_ID
                                LEFT JOIN orjin.TB_KOD DRM ON MKN.MKN_DURUM_KOD_ID=DRM.TB_KOD_ID
                                LEFT JOIN orjin.TB_KOD TIP ON MKN.MKN_TIP_KOD_ID=TIP.TB_KOD_ID
                                LEFT JOIN orjin.TB_PROJE PRJ ON MKN.MKN_PROJE_ID=PRJ.TB_PROJE_ID
                                LEFT JOIN orjin.TB_ARAC ARC ON MKN.TB_MAKINE_ID=ARC.ARC_MAKINE_ID
                                LEFT JOIN orjin.VW_SAYAC MES ON (MES.MES_REF_ID = MKN.TB_MAKINE_ID) AND (MES_VARSAYILAN = 1)
                                LEFT JOIN orjin.TB_PERSONEL OPR ON MKN.MKN_OPERATOR_PERSONEL_ID=OPR.TB_PERSONEL_ID
								LEFT JOIN orjin.TB_MAKINE_BAKIM MBAK ON MKN.TB_MAKINE_ID =MBAK.MAB_MAKINE_ID
								LEFT JOIN orjin.TB_IS_TANIM IST  ON IST.TB_IS_TANIM_ID = MAB_BAKIM_ID
							
                                WHERE
                                orjin.UDF_LOKASYON_YETKI_KONTROL(ISNULL(MKN_LOKASYON_ID,-1), @KUL_ID) = 1 AND 
                                orjin.UDF_ATOLYE_YETKI_KONTROL(ISNULL(MKN_ATOLYE_ID,-1), @KUL_ID) = 1 
								AND IST_PERSONEL_ID = @IST_PERSONEL_ID ";
           
            if (Tip != -1)
            {
                prms.Add("MKN_TIP_KOD_ID", Tip);
                query = query + " and MKN_TIP_KOD_ID = @MKN_TIP_KOD_ID";
            }
            if (kategori != -1)
            {
                prms.Add("MKN_KATEGORI_KOD_ID", kategori);
                query = query + " and MKN_KATEGORI_KOD_ID = @MKN_KATEGORI_KOD_ID";
            }
            if (YakitKullanimi > -1)
            {
                prms.Add("MKN_YAKIT_KULLANIM", YakitKullanimi);
                query = query + " and MKN_YAKIT_KULLANIM = @MKN_YAKIT_KULLANIM";
            }
            if (otonombakim)
            {
                query = query + " and MKN_OTONOM_BAKIM = 1";
            }
            if (lokasyonID != -1)
            {
                prms.Add("MKN_LOKASYON_ID", lokasyonID);
                query = query + " and MKN_LOKASYON_ID IN (select (TB_LOKASYON_ID) from orjin.UDF_LOKASYON_ALT_AGAC(@MKN_LOKASYON_ID))";
            }
            if (!String.IsNullOrEmpty(prm))
            {
                if (b)
                {
                    prms.Add("MKN_KOD", prm);
                    query = query + " and MKN_KOD = @MKN_KOD";
                }
                else
                {
                    prms.Add("KELIME", prm);
                    query = query + @" and (
                    MKN_KOD         LIKE '%'+@KELIME+'%' OR 
                    ARC.ARC_PLAKA   LIKE '%'+@KELIME+'%' OR 
                    LOK.LOK_TANIM   LIKE '%'+@KELIME+'%' OR 
                    MKN_TANIM       LIKE '%'+@KELIME+'%' OR 
                    MRK.KOD_TANIM   LIKE '%'+@KELIME+'%' OR 
                    KTG.KOD_TANIM   LIKE '%'+@KELIME+'%' OR 
                    DRM.KOD_TANIM   LIKE '%'+@KELIME+'%' OR 
                    '*' = @KELIME)";
                }
            }
            prms.Add("IST_PERSONEL_ID", prsId);
            prms.Add("ILK_DEGER", ilkDeger);
            prms.Add("SON_DEGER", sonDeger);
            prms.Add("KUL_ID", ID);
            query = query + @")  SELECT * FROM MTABLE WHERE RowNum > @ILK_DEGER AND RowNum <= @SON_DEGER";



            IEnumerable<Makine> listem = new List<Makine>();
            try
            {
                using (var conn = klas.baglan())
                {
                    listem = conn.Query<Makine>(query, prms);
                }
            }
            catch (Exception e)
            {
                return e;
            }
            return listem;
        }


        [Route("api/MakineAlt/{ID}")]
        [HttpGet]
        public object GetForWeb([FromUri] int ID, [FromUri] int ilkDeger, [FromUri] int sonDeger,  [FromUri] int lokasyonID=-1, string search=null)
        {
            var prms = new DynamicParameters();
            string query = @";WITH MTABLE AS(
                                SELECT 
                                TB_MAKINE_ID
                                ,MKN.MKN_KOD 					
                                ,MKN_TANIM 					 					
                                ,MKN_TIP_KOD_ID 					
                                ,TIP.KOD_TANIM MKN_TIP 					
                                ,LOK.LOK_TANIM MKN_LOKASYON 				
                                ,OPR.PRS_ISIM MKN_OPERATOR 				
                                ,MKN_OPERATOR_PERSONEL_ID 	
                                ,LOK.LOK_TUM_YOL MKN_TAM_LOKASYON 			
                                ,MES.MES_GUNCEL_DEGER MKN_SAYAC_GUNCEL_DEGER 		
                                ,MES.MES_SAYAC_BIRIM MKN_SAYAC_BIRIM 			
                                ,KTG.KOD_TANIM MKN_KATEGORI 
                                ,(SELECT COUNT(TB_ISEMRI_ID) FROM orjin.TB_ISEMRI WHERE ISM_MAKINE_ID= TB_MAKINE_ID AND ISM_KAPATILDI=0) MKN_ACIK_ISEMRI_SAYISI
                                ,(SELECT COUNT(TB_ISEMRI_ID) FROM orjin.TB_ISEMRI WHERE ISM_MAKINE_ID= TB_MAKINE_ID AND ISM_KAPATILDI=1) MKN_KAPALI_ISEMRI_SAYISI
                                ,(SELECT COUNT(TB_IS_TALEP_ID) FROM orjin.TB_IS_TALEBI WHERE IST_MAKINE_ID= TB_MAKINE_ID AND IST_DURUM_ID IN (0,1)) MKN_ACIK_ISTALEP_SAYISI
                                ,(SELECT COUNT(TB_IS_TALEP_ID) FROM orjin.TB_IS_TALEBI WHERE IST_MAKINE_ID= TB_MAKINE_ID AND IST_DURUM_ID NOT IN (0,1)) MKN_KAPALI_ISTALEP_SAYISI 
                                ,MKN_DURUM_KOD_ID				
                                ,DRM.KOD_TANIM MKN_DURUM 					
                                ,MRK.KOD_TANIM MKN_MARKA 					
                                ,MDL.KOD_TANIM MKN_MODEL 					
                                ,MKN_LOKASYON_ID 			
                                ,MKN_URETIM_YILI 			
                                ,MKN_SERI_NO 				
                                ,ARC.ARC_PLAKA MKN_ARAC_PLAKA 				
                                ,PRJ.PRJ_TANIM MKN_PROJE 					
                                ,MKN_PROJE_ID
                                ,(select COALESCE(TB_RESIM_ID,0) from orjin.TB_RESIM where RSM_VARSAYILAN = 1 AND RSM_REF_GRUP = 'MAKINE' and RSM_REF_ID = TB_MAKINE_ID) as RSM_VAR_ID 
                                ,stuff((SELECT ';' + CONVERT(varchar(11), R.TB_RESIM_ID)    FROM orjin.TB_RESIM R    WHERE R.RSM_REF_GRUP = 'MAKINE' and R.RSM_REF_ID = TB_MAKINE_ID    FOR XML PATH('')), 1, 1, '') [RSM_IDS]	
                                ,ROW_NUMBER() OVER (ORDER BY MKN.TB_MAKINE_ID) AS RowNum 	
                                FROM orjin.TB_MAKINE MKN
                                LEFT JOIN orjin.TB_LOKASYON LOK on MKN.MKN_LOKASYON_ID = LOK.TB_LOKASYON_ID
                                LEFT JOIN orjin.TB_KOD MRK ON MKN.MKN_MARKA_KOD_ID=MRK.TB_KOD_ID
                                LEFT JOIN orjin.TB_KOD MDL ON MKN.MKN_MODEL_KOD_ID=MDL.TB_KOD_ID
                                LEFT JOIN orjin.TB_KOD KTG ON MKN.MKN_KATEGORI_KOD_ID=KTG.TB_KOD_ID
                                LEFT JOIN orjin.TB_KOD DRM ON MKN.MKN_DURUM_KOD_ID=DRM.TB_KOD_ID
                                LEFT JOIN orjin.TB_KOD TIP ON MKN.MKN_TIP_KOD_ID=TIP.TB_KOD_ID
                                LEFT JOIN orjin.TB_PROJE PRJ ON MKN.MKN_PROJE_ID=PRJ.TB_PROJE_ID
                                LEFT JOIN orjin.TB_ARAC ARC ON MKN.TB_MAKINE_ID=ARC.ARC_MAKINE_ID
                                LEFT JOIN orjin.VW_SAYAC MES ON (MES.MES_REF_ID = MKN.TB_MAKINE_ID) AND (MES_VARSAYILAN = 1)
                                LEFT JOIN orjin.TB_PERSONEL OPR ON MKN.MKN_OPERATOR_PERSONEL_ID=OPR.TB_PERSONEL_ID
                                WHERE
                                MKN.MKN_AKTIF = 1 AND
                                orjin.UDF_LOKASYON_YETKI_KONTROL(ISNULL(MKN_LOKASYON_ID,-1),@KUL_ID) = 1 AND 
                                orjin.UDF_ATOLYE_YETKI_KONTROL(ISNULL(MKN_ATOLYE_ID,-1), @KUL_ID ) = 1 ";
            var where = "";
            if (lokasyonID != -1)
            {
                prms.Add("MKN_LOKASYON_ID", lokasyonID);
                where += " and MKN_LOKASYON_ID IN (select (TB_LOKASYON_ID) from orjin.UDF_LOKASYON_ALT_AGAC(@MKN_LOKASYON_ID))";
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                prms.Add("KELIME", search);
                where += @" and (
                MKN_KOD         LIKE '%'+@KELIME+'%' OR 
                ARC.ARC_PLAKA   LIKE '%'+@KELIME+'%' OR 
                LOK.LOK_TANIM   LIKE '%'+@KELIME+'%' OR 
                MKN_TANIM       LIKE '%'+@KELIME+'%' OR 
                MRK.KOD_TANIM   LIKE '%'+@KELIME+'%' OR 
                KTG.KOD_TANIM   LIKE '%'+@KELIME+'%' OR 
                DRM.KOD_TANIM   LIKE '%'+@KELIME+'%')";
                
            }

            query += where;
            prms.Add("ILK_DEGER", ilkDeger);
            prms.Add("SON_DEGER", sonDeger);
            prms.Add("KUL_ID", ID);
            query  += @")  SELECT * FROM MTABLE WHERE RowNum > @ILK_DEGER AND RowNum <= @SON_DEGER;";

            query += $@"SELECT COUNT(*)	
                                FROM orjin.TB_MAKINE MKN
                                LEFT JOIN orjin.TB_LOKASYON LOK on MKN.MKN_LOKASYON_ID = LOK.TB_LOKASYON_ID
                                LEFT JOIN orjin.TB_KOD MRK ON MKN.MKN_MARKA_KOD_ID=MRK.TB_KOD_ID
                                LEFT JOIN orjin.TB_KOD MDL ON MKN.MKN_MODEL_KOD_ID=MDL.TB_KOD_ID
                                LEFT JOIN orjin.TB_KOD KTG ON MKN.MKN_KATEGORI_KOD_ID=KTG.TB_KOD_ID
                                LEFT JOIN orjin.TB_KOD DRM ON MKN.MKN_DURUM_KOD_ID=DRM.TB_KOD_ID
                                LEFT JOIN orjin.TB_KOD TIP ON MKN.MKN_TIP_KOD_ID=TIP.TB_KOD_ID
                                LEFT JOIN orjin.TB_PROJE PRJ ON MKN.MKN_PROJE_ID=PRJ.TB_PROJE_ID
                                LEFT JOIN orjin.TB_ARAC ARC ON MKN.TB_MAKINE_ID=ARC.ARC_MAKINE_ID
                                LEFT JOIN orjin.VW_SAYAC MES ON (MES.MES_REF_ID = MKN.TB_MAKINE_ID) AND (MES_VARSAYILAN = 1)
                                LEFT JOIN orjin.TB_PERSONEL OPR ON MKN.MKN_OPERATOR_PERSONEL_ID=OPR.TB_PERSONEL_ID
                                WHERE
                                MKN.MKN_AKTIF = 1 AND
                                orjin.UDF_LOKASYON_YETKI_KONTROL(ISNULL(MKN_LOKASYON_ID,-1),@KUL_ID) = 1 AND 
                                orjin.UDF_ATOLYE_YETKI_KONTROL(ISNULL(MKN_ATOLYE_ID,-1), @KUL_ID ) = 1 {where};";


            IEnumerable<Makine> listem = new List<Makine>();
            var count = 0;
            try
            {
                using (var conn = klas.baglan())
                {
                    var result = conn.QueryMultiple(query, prms);
                    listem = result.Read<Makine>();
                    count = result.ReadFirstOrDefault<int>();
                }
            }
            catch (Exception e)
            {
                return new { success = false,message=e.Message};
            }

            return new { success = true,data = listem,totalCount=count};
        }


        [Route("api/MakineTipleri")]
        [HttpGet]
        public List<TanimDeger> GetKategoriSayilari([FromUri] int ID)
        {
            prms.Clear();
            prms.Add("KUL_ID", ID);
            string query = @"SELECT * FROM (select TB_KOD_ID,KOD_TANIM, (SELECT COUNT(TB_MAKINE_ID) FROM orjin.TB_MAKINE WHERE MKN_AKTIF = 1 AND MKN_TIP_KOD_ID = TB_KOD_ID AND orjin.UDF_LOKASYON_YETKI_KONTROL(MKN_LOKASYON_ID,@KUL_ID)=1) as Sayi from orjin.TB_KOD where KOD_GRUP = 32501) AS TABLOM WHERE sayi>0";
            DataTable dt = klas.GetDataTable(query, prms.PARAMS);
            List<TanimDeger> listem = new List<TanimDeger>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                TanimDeger entity = new TanimDeger();
                entity.TanimDegerID = (int)dt.Rows[i]["TB_KOD_ID"];
                entity.Deger = Util.getFieldDouble(dt.Rows[i], "Sayi");
                entity.Tanim = Util.getFieldString(dt.Rows[i], "KOD_TANIM");
                listem.Add(entity);
            }
            return listem;
        }
        [Route("api/MakineKategori")]
        [HttpGet]
        public List<TanimDeger> MakineKategori([FromUri] int ID)
        {
            prms.Clear();
            prms.Add("KUL_ID", ID);
            string query = @"SELECT * FROM (select TB_KOD_ID,KOD_TANIM, (SELECT COUNT(TB_MAKINE_ID) FROM orjin.TB_MAKINE WHERE MKN_AKTIF = 1 AND MKN_KATEGORI_KOD_ID = TB_KOD_ID AND orjin.UDF_LOKASYON_YETKI_KONTROL(MKN_LOKASYON_ID,@KUL_ID)=1 AND orjin.UDF_ATOLYE_YETKI_KONTROL(MKN_ATOLYE_ID,@KUL_ID)=1) as Sayi from orjin.TB_KOD where KOD_GRUP = 32502) AS TABLOM WHERE sayi>0";
            DataTable dt = klas.GetDataTable(query, prms.PARAMS);
            List<TanimDeger> listem = new List<TanimDeger>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                TanimDeger entity = new TanimDeger();
                entity.TanimDegerID = (int)dt.Rows[i]["TB_KOD_ID"];
                entity.Deger = Util.getFieldDouble(dt.Rows[i], "Sayi");
                entity.Tanim = Util.getFieldString(dt.Rows[i], "KOD_TANIM");
                listem.Add(entity);
            }
            return listem;
        }
        [Route("api/MakineListByLokasyon")]
        [HttpGet]
        public List<Makine> GetMakineListByLokasyon([FromUri] int LokID)
        {
            prms.Clear();
            prms.Add("LOK_ID", LokID);
            List<Makine> listem = new List<Makine>();
            string query = @"select * from orjin.TB_MAKINE where MKN_LOKASYON_ID IN (select (TB_LOKASYON_ID) from [orjin].[UDF_LOKASYON_ALT_AGAC](@LOK_ID))";
            DataTable dt = klas.GetDataTable(query, prms.PARAMS);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Makine entity = new Makine();
                entity.TB_MAKINE_ID = Convert.ToInt32(dt.Rows[i]["TB_MAKINE_ID"]);
                entity.MKN_KOD = Util.getFieldString(dt.Rows[i], "MKN_KOD");
                entity.MKN_TANIM = Util.getFieldString(dt.Rows[i], "MKN_TANIM");
                listem.Add(entity);
            }
            return listem;
        }
        [Route("api/MakineMalzemeListesi")]
        [HttpGet]
        public List<IsEmriMalzeme> MakineMalzemeListesi([FromUri] int makineID)
        {
            prms.Clear();
            prms.Add("ISM_MAKINE_ID", makineID);
            string sql = "select *,(select top 1 STK_KOD from orjin.TB_STOK where TB_STOK_ID = IDM_STOK_ID) as IDM_STOK_KOD ,(SELECT TOP 1 ISM_ISEMRI_NO FROM orjin.TB_ISEMRI WHERE TB_ISEMRI_ID =IDM_ISEMRI_ID) AS IDM_ISEMRI_NO from orjin.VW_ISEMRI_MLZ  where IDM_ISEMRI_ID IN (select TB_ISEMRI_ID FROM orjin.TB_ISEMRI WHERE ISM_MAKINE_ID=@ISM_MAKINE_ID)";
            List<IsEmriMalzeme> listem = new List<IsEmriMalzeme>();
            DataTable dt = klas.GetDataTable(sql, prms.PARAMS);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IsEmriMalzeme entity = new IsEmriMalzeme();
                entity.IDM_STOK_KOD = Util.getFieldString(dt.Rows[i], "IDM_STOK_KOD");
                entity.IDM_STOK_TANIM = Util.getFieldString(dt.Rows[i], "IDM_STOK_TANIM");
                entity.TB_ISEMRI_MLZ_ID = Util.getFieldInt(dt.Rows[i], "TB_ISEMRI_MLZ_ID");
                entity.IDM_ISEMRI_ID = Util.getFieldInt(dt.Rows[i], "IDM_ISEMRI_ID");
                entity.IDM_STOK_ID = Util.getFieldInt(dt.Rows[i], "IDM_STOK_ID");
                entity.IDM_DEPO_ID = Util.getFieldInt(dt.Rows[i], "IDM_DEPO_ID");
                entity.IDM_STOK_TIP_KOD_ID = Util.getFieldInt(dt.Rows[i], "IDM_STOK_TIP_KOD_ID");
                entity.IDM_BIRIM_KOD_ID = Util.getFieldInt(dt.Rows[i], "IDM_BIRIM_KOD_ID");
                entity.IDM_BIRIM_FIYAT = Util.getFieldDouble(dt.Rows[i], "IDM_BIRIM_FIYAT");
                entity.IDM_TUTAR = Util.getFieldDouble(dt.Rows[i], "IDM_TUTAR");
                entity.IDM_MIKTAR = Util.getFieldDouble(dt.Rows[i], "IDM_MIKTAR");
                entity.IDM_DEPO = Util.getFieldString(dt.Rows[i], "IDM_DEPO");
                entity.IDM_TARIH = Util.getFieldDateTime(dt.Rows[i], "IDM_TARIH");
                entity.IDM_SAAT = Util.getFieldString(dt.Rows[i], "IDM_SAAT");
                entity.IDM_ISEMRI_NO = Util.getFieldString(dt.Rows[i], "IDM_ISEMRI_NO");
                entity.IDM_STOK_DUS = Util.getFieldBool(dt.Rows[i], "IDM_STOK_DUS");

                entity.IDM_STOK_KULLANIM_SEKLI = Util.getFieldInt(dt.Rows[i], "IDM_STOK_KULLANIM_SEKLI");
                prms.Clear();
                prms.Add("TB_KOD_ID", entity.IDM_BIRIM_KOD_ID);
                entity.IDM_BIRIM = klas.GetDataCell("select KOD_TANIM from orjin.TB_KOD where TB_KOD_ID=@TB_KOD_ID", prms.PARAMS);
                listem.Add(entity);
            }
            return listem;
        }

        [Route("api/MakineMalzemeListesiFiltered")]
        [HttpPost]
        public List<IsEmriMalzeme> MakineMalzemeListesiFiltered([FromBody]Filtre filtre, [FromUri] int makineID)
        {
            prms.Clear();
            prms.Add("ISM_MAKINE_ID", makineID);
            string sql = "select M.*, S.STK_KOD AS IDM_STOK_KOD, IE.ISM_ISEMRI_NO AS IDM_ISEMRI_NO from orjin.VW_ISEMRI_MLZ M INNER JOIN orjin.TB_STOK S ON S.TB_STOK_ID = M.IDM_STOK_ID INNER JOIN orjin.TB_ISEMRI IE ON M.IDM_ISEMRI_ID = IE.TB_ISEMRI_ID where IDM_ISEMRI_ID IN (select TB_ISEMRI_ID FROM orjin.TB_ISEMRI WHERE ISM_MAKINE_ID=@ISM_MAKINE_ID) ";

            if (filtre != null)
            {
                if (filtre.DepoID > -1)
                {
                    prms.Add("IDM_DEPO_ID", filtre.DepoID);
                    sql += " AND IDM_DEPO_ID = @IDM_DEPO_ID";
                }
                if (filtre.isEmriTipId > -1)
                {
                    prms.Add("STK_TIP_KOD_ID", filtre.isEmriTipId);
                    sql += " AND S.STK_TIP_KOD_ID = @STK_TIP_KOD_ID";
                }
                if (filtre.nedenID > -1)
                {
                    prms.Add("STK_GRUP_KOD_ID", filtre.nedenID);
                    sql += " AND S.STK_GRUP_KOD_ID = @STK_GRUP_KOD_ID";
                }
                if (filtre.durumID > -1)
                {
                    prms.Add("IDM_STOK_ID", filtre.durumID);
                    sql += " AND IDM_STOK_ID = @IDM_STOK_ID";
                }
                if (filtre.BasTarih != "" && filtre.BitTarih != "")
                {
                    DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                    DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                    prms.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
                    prms.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
                    sql = sql + " AND IST_ACILIS_TARIHI BETWEEN  @BAS_TARIH and @BIT_TARIH";
                }
                else
                if (filtre.BasTarih != "")
                {
                    DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                    prms.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
                    sql = sql + " AND IST_ACILIS_TARIHI >= @BAS_TARIH ";
                }
                else
                if (filtre.BitTarih != "")
                {
                    DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                    prms.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
                    sql = sql + " AND IST_ACILIS_TARIHI <= @BIT_TARIH";
                }
                if (filtre.Kelime != "")
                {
                    prms.Add("KELIME", filtre.Kelime);
                    sql += " AND  (IE.ISM_ISEMRI_NO like '%'+@KELIME+'%' OR IDM_DEPO like '%'+@KELIME+'%' OR IDM_STOK_TANIM like '%'+@KELIME+'%' OR S.STK_KOD like '%'+@KELIME+'%' )";
                }

            }

            List<IsEmriMalzeme> listem = new List<IsEmriMalzeme>();
            DataTable dt = klas.GetDataTable(sql, prms.PARAMS);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IsEmriMalzeme entity = new IsEmriMalzeme();
                entity.IDM_STOK_KOD = Util.getFieldString(dt.Rows[i], "IDM_STOK_KOD");
                entity.IDM_STOK_TANIM = Util.getFieldString(dt.Rows[i], "IDM_STOK_TANIM");
                entity.TB_ISEMRI_MLZ_ID = Util.getFieldInt(dt.Rows[i], "TB_ISEMRI_MLZ_ID");
                entity.IDM_ISEMRI_ID = Util.getFieldInt(dt.Rows[i], "IDM_ISEMRI_ID");
                entity.IDM_STOK_ID = Util.getFieldInt(dt.Rows[i], "IDM_STOK_ID");
                entity.IDM_DEPO_ID = Util.getFieldInt(dt.Rows[i], "IDM_DEPO_ID");
                entity.IDM_STOK_TIP_KOD_ID = Util.getFieldInt(dt.Rows[i], "IDM_STOK_TIP_KOD_ID");
                entity.IDM_BIRIM_KOD_ID = Util.getFieldInt(dt.Rows[i], "IDM_BIRIM_KOD_ID");
                entity.IDM_BIRIM_FIYAT = Util.getFieldDouble(dt.Rows[i], "IDM_BIRIM_FIYAT");
                entity.IDM_TUTAR = Util.getFieldDouble(dt.Rows[i], "IDM_TUTAR");
                entity.IDM_MIKTAR = Util.getFieldDouble(dt.Rows[i], "IDM_MIKTAR");
                entity.IDM_DEPO = Util.getFieldString(dt.Rows[i], "IDM_DEPO");
                entity.IDM_TARIH = Util.getFieldDateTime(dt.Rows[i], "IDM_TARIH");
                entity.IDM_SAAT = Util.getFieldString(dt.Rows[i], "IDM_SAAT");
                entity.IDM_ISEMRI_NO = Util.getFieldString(dt.Rows[i], "IDM_ISEMRI_NO");
                entity.IDM_STOK_DUS = Util.getFieldBool(dt.Rows[i], "IDM_STOK_DUS");

                entity.IDM_STOK_KULLANIM_SEKLI = Util.getFieldInt(dt.Rows[i], "IDM_STOK_KULLANIM_SEKLI");
                prms.Clear();
                prms.Add("TB_KOD_ID", entity.IDM_BIRIM_KOD_ID);
                entity.IDM_BIRIM = klas.GetDataCell("select KOD_TANIM from orjin.TB_KOD where TB_KOD_ID=@TB_KOD_ID", prms.PARAMS);
                listem.Add(entity);
            }
            return listem;
        }

        [Route("api/MakineDetaySayilar")]
        [HttpGet]
        public Sayilar MakineDetaySayilar([FromUri] int makineID)
        {
            var user =(Kullanici) ActionContext.ActionArguments[C.KEY_USER]; 
            var util = new Util();
            try
            {
                string query= @"SELECT COUNT(TB_ISEMRI_ID) FROM orjin.TB_ISEMRI WHERE ISM_MAKINE_ID = @MAK_ID;
                                SELECT COUNT(TB_ISEMRI_MLZ_ID) FROM orjin.VW_ISEMRI_MLZ  where IDM_ISEMRI_ID IN (select TB_ISEMRI_ID FROM orjin.TB_ISEMRI WHERE ISM_MAKINE_ID = @MAK_ID);
                                SELECT COUNT(TB_SAYAC_OKUMA_ID) FROM orjin.TB_SAYAC_OKUMA WHERE SYO_SAYAC_ID IN (select TB_SAYAC_ID FROM orjin.TB_SAYAC WHERE MES_REF_ID = @MAK_ID);
                                SELECT COUNT(TB_YAKIT_HRK_ID) FROM orjin.TB_YAKIT_HRK WHERE YKH_MAKINE_ID = @MAK_ID;
                                SELECT COUNT(TB_DOSYA_ID) FROM dbo.TB_DOSYA WHERE DSY_AKTIF = 1 AND DSY_REF_GRUP = 'MAKINE' AND DSY_REF_ID = @MAK_ID;
                                SELECT COUNT(*) FROM orjin.TB_MAKINE_BAKIM_TARIHCE MBT
                                                  INNER JOIN orjin.TB_MAKINE_BAKIM MB ON MB.TB_MAKINE_BAKIM_ID=MBT.MBT_MAKINE_BAKIM_ID 
                                                  INNER JOIN orjin.TB_MAKINE M ON M.TB_MAKINE_ID=MB.MAB_MAKINE_ID 
                                                  WHERE MB.MAB_MAKINE_ID=@MAK_ID AND 
                                                        MBT.MBT_OLUSTURAN_ID > 0 AND
                                                    orjin.UDF_LOKASYON_YETKI_KONTROL(M.MKN_LOKASYON_ID,@KLL_ID)=1

                            SELECT COUNT(*) from PBTPRO_1.orjin.TB_COZUM_KATALOG CK
                            LEFT JOIN PBTPRO_1.orjin.TB_ISEMRI I ON (CK.CMK_REF_ID=TB_ISEMRI_ID)
                            LEFT JOIN PBTPRO_1.orjin.TB_KOD K1 ON (CK.CMK_TESHIS_ID=K1.TB_KOD_ID)
                            LEFT JOIN PBTPRO_1.orjin.TB_KOD K2 ON (CK.CMK_NEDEN_ID=K2.TB_KOD_ID)
                            WHERE ISM_MAKINE_ID = @MAK_ID
                                
                                                                                                                                                                                ";
                
                Sayilar entity = new Sayilar();
                var prms = new DynamicParameters();
                prms.Add("MAK_ID", makineID);
                prms.Add("KLL_ID", user.TB_KULLANICI_ID);
                using (var cnn = util.baglan())
                {
                    var result = cnn.QueryMultiple(query,prms);
                    entity.ArizaBakim = result.ReadFirstOrDefault<int>();
                    entity.DegisenMalzeme = result.ReadFirstOrDefault<int>();
                    entity.SayacHareketleri = result.ReadFirstOrDefault<int>();
                    entity.Yakithareketleri = result.ReadFirstOrDefault<int>();
                    entity.Dosya = result.ReadFirstOrDefault<int>();
                    entity.OtonomBakimTarihce = result.ReadFirstOrDefault<int>();
                    entity.CozumKataloglarSayisi = result.ReadFirstOrDefault<int>();
                    return entity;
                }

            }
            catch (Exception)
            {

                throw;
            }
        }
        [Route("api/MakineKodGetir")]
        [HttpGet]
        public String MakineKodGetir()
        {
            try
            {
                var util = new Util();
                using (var conn = util.baglan())
                {
                    var sql = @"  
                        UPDATE orjin.TB_NUMARATOR SET NMR_NUMARA = NMR_NUMARA+1 WHERE NMR_KOD = 'MKN_KOD';
                        SELECT 
                        NMR_ON_EK+right(replicate('0',NMR_HANE_SAYISI)+CAST(NMR_NUMARA AS VARCHAR(MAX)),NMR_HANE_SAYISI) as deger FROM orjin.TB_NUMARATOR
                        WHERE NMR_KOD = 'MKN_KOD'";
                    var kod = conn.Query<String>(sql).FirstOrDefault();
                    return kod;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Route("api/MakineUpsert")]
        [HttpPost]
        public Bildirim MakineUpsert([FromBody] MakineUpsert makine)
        {
            var bildirim = new Bildirim();
            var isUpdate = makine.TB_MAKINE_ID > 0;
            try
            {
                var util = new Util();
                using (var conn = util.baglan())
                {
                    if (isUpdate)
                    {
                        bildirim.Durum = conn.Update(makine);
                        bildirim.Aciklama = "Makine bilgileri başarılı bir şekilde güncellendi!";
                        bildirim.MsgId = Bildirim.MSG_MKN_GUNCELLE_OK;
                    }
                    else
                    {
                        bildirim.Id = conn.Insert(makine);
                        bildirim.Durum = true;
                        bildirim.Aciklama = "Makine başarılı bir şekilde kaydedildi!";
                        bildirim.MsgId = Bildirim.MSG_MKN_KAYIT_OK;
                    }

                    return bildirim;
                }
            }
            catch (Exception e)
            {
                bildirim.Aciklama = e.Message;
                bildirim.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirim.Error = true;
                bildirim.Durum = false;
                return bildirim;
            }
        }
        [Route("api/MakineCozumKatalog")]
        [HttpGet]
        public List<CozumKatalogModel> GetCozumKatalogListByMakine([FromUri] int makineID)
        {
            prms.Clear();
            prms.Add("MAKINE_ID", makineID);
            string sql = @" SELECT CMK_KONU,CMK_TESHIS_ID,CMK_NEDEN_ID, IML.TB_IS_TANIM_MAKINE_ID,IML_IS_TANIM_ID,IML_MAKINE_ID FROM orjin.TB_IS_TANIM_MAKINE_LISTE IML
                            INNER JOIN orjin.TB_IS_TANIM_COZUM_KATALOG ISC ON IML.IML_IS_TANIM_ID = ISC.ISC_IS_TANIM_ID
                            INNER JOIN orjin.TB_COZUM_KATALOG CMK ON CMK.TB_COZUM_KATALOG_ID = ISC.ISC_COZUMKATALOG_ID
                            INNER JOIN orjin.TB_KOD K1 ON (CMK.CMK_TESHIS_ID=K1.TB_KOD_ID)
                            INNER JOIN orjin.TB_KOD K2 ON (CMK.CMK_NEDEN_ID=K2.TB_KOD_ID)
  
                            WHERE IML_MAKINE_ID = @MAKINE_ID ";

            List<CozumKatalogModel> listem = new List<CozumKatalogModel>();
            DataTable dt = klas.GetDataTable(sql, prms.PARAMS);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CozumKatalogModel entity = new CozumKatalogModel();
                entity.TB_COZUM_KATALOG_ID = Convert.ToInt32(dt.Rows[i]["TB_COZUM_KATALOG_ID"]);
                entity.CMK_KOD = (dt.Rows[i]["CMK_KOD"]).ToString();
                entity.CMK_KONU = Util.getFieldString(dt.Rows[i], "CMK_KONU");
                entity.CMK_TESHIS = dt.Rows[i]["TESHIS"] != DBNull.Value ? dt.Rows[i]["TESHIS"].ToString() : "";
                entity.CMK_NEDEN = dt.Rows[i]["NEDEN"] != DBNull.Value ? dt.Rows[i]["NEDEN"].ToString() : "";
                listem.Add(entity);
            }

            return listem;
        }

    }
}
