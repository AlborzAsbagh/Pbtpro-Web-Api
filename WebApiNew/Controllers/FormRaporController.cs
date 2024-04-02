using Dapper;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using WebApiNew.Filters;
using WebApiNew.Models;


namespace WebApiNew.Controllers
{
	[JwtAuthenticationFilter]
	public class FormRaporController : Controller
	{
		Util klas = new Util();
		YetkiController yetki = new YetkiController();
		string query = "";

		[System.Web.Http.Route("api/GetFormByType")]
		[System.Web.Http.HttpGet]
		public ActionResult GetFormByType([FromUri]long id , [FromUri] int tipId)
		{
		
			try
			{
				object entity = null;
				string fileName, viewName = "";
				switch (tipId)
				{
					case 1:
						entity = GetIsEmriFormById(id);
						fileName = "IsEmriForm.pdf";
						viewName = "IsEmriFormByIdToPdf";
					break;

					case 2:
						entity = GetIsTalepFormById(id);
						fileName = "IsTalepForm.pdf";
						viewName = "IsTalepFormByIdToPdf";
					break;

					case 3:
						entity = GetBakimFormByIdToPdf(id);
						fileName = "BakimForm.pdf";
						viewName = "BakimFormByIdToPdf";
					break;

					case 4:
						entity = GetArizaFormByIdToPdf(id);
						fileName = "ArizaForm.pdf";
						viewName = "ArizaFormByIdToPdf";
					break;

					default:
						return HttpNotFound();

				}

					if (entity != null)
					{
						return new ViewAsPdf(viewName, entity)
							{
								FileName = fileName
							};
					}
					else
					{
							return HttpNotFound();
					}
				}
				catch (Exception ex)
				{
					return HttpNotFound(ex.Message);
				}
		}

		public object GetIsEmriFormById([FromUri] long id)
		{
			List<WebVersionIsEmriForm> listem = new List<WebVersionIsEmriForm>();
			Bildirim bldr = new Bildirim();
			try
			{
				query = @" SELECT 
							IE.TB_ISEMRI_ID,
							IE.ISM_ISEMRI_NO,
							IE.ISM_BASLAMA_TARIH,
							TIP.IMT_TANIM AS ISM_TIP,
							orjin.UDF_KOD_TANIM(IE.ISM_DURUM_KOD_ID) AS ISM_DURUM,
							BAGLI_ISM.ISM_ISEMRI_NO AS ISM_BAGLI_ISEMRI,
							ATL.ATL_TANIM AS ISM_ATOLYE,
							LOK.LOK_TANIM AS ISM_LOKASYON,
							MKN.MKN_KOD AS ISM_MAKINE_KOD,
							MKN.MKN_TANIM AS ISM_MAKINE_TANIM,
							EKP.EKP_TANIM AS ISM_EKIPMAN_TANIM,
							EKP.EKP_KOD AS ISM_EKIPMAN_KOD,
							SOC.SOC_TANIM AS ISM_ONCELIK,
							IE.ISM_MAKINE_GUVENLIK_NOTU,
							IE.ISM_ACIKLAMA,
							orjin.UDF_KOD_TANIM(IE.ISM_MAKINE_DURUM_KOD_ID) AS ISM_MAKINE_DURUM,
							IST.IST_ACILIS_TARIHI AS ISM_TALEP_TARIH,
							KULLANICI.ISK_ISIM AS ISM_TALEP_EDEN,


							IS_TANIM.IST_KOD as ISM_PROSEDUR,
							IS_TANIM.IST_ACIKLAMA as ISM_PROSEDUR_ACIKLAMA,
							orjin.UDF_KOD_TANIM(IS_TANIM.IST_TIP_KOD_ID) as ISM_PROSEDUR_TIP,
							PRS.PRS_PERSONEL_KOD as ISM_PERSONEL_KOD, 
							PRS.PRS_ISIM as ISM_PERSONEL_ISIM, 
							ISEMRI_KAYNAK.IDK_SURE as ISM_PERSONEL_SURE,
							ISEMRI_KAYNAK.IDK_FAZLA_MESAI_SURE as ISM_PERSONEL_FAZLA_MESAI_SURE,
							ISEMRI_KAYNAK.IDK_MALIYET as ISM_PERSONEL_MALIYET
						FROM 
							orjin.TB_ISEMRI IE
						LEFT JOIN orjin.TB_ISEMRI_TIP TIP ON TIP.TB_ISEMRI_TIP_ID = IE.ISM_TIP_ID
						LEFT JOIN orjin.TB_ISEMRI BAGLI_ISM ON BAGLI_ISM.TB_ISEMRI_ID = IE.ISM_BAGLI_ISEMRI_ID
						LEFT JOIN orjin.TB_ATOLYE ATL ON ATL.TB_ATOLYE_ID = IE.ISM_ATOLYE_ID
						LEFT JOIN orjin.TB_LOKASYON LOK ON LOK.TB_LOKASYON_ID = IE.ISM_LOKASYON_ID
						LEFT JOIN orjin.TB_MAKINE MKN ON MKN.TB_MAKINE_ID = IE.ISM_MAKINE_ID
						LEFT JOIN orjin.TB_EKIPMAN EKP ON EKP.TB_EKIPMAN_ID = IE.ISM_EKIPMAN_ID
						LEFT JOIN orjin.TB_SERVIS_ONCELIK SOC ON SOC.TB_SERVIS_ONCELIK_ID = IE.ISM_ONCELIK_ID
						LEFT JOIN orjin.TB_IS_TALEBI IST ON IST.IST_ISEMRI_ID = IE.TB_ISEMRI_ID
						LEFT JOIN orjin.TB_IS_TALEBI_KULLANICI KULLANICI ON KULLANICI.TB_IS_TALEBI_KULLANICI_ID = IST.IST_TALEP_EDEN_ID
						LEFT JOIN orjin.TB_IS_TANIM IS_TANIM ON IS_TANIM.TB_IS_TANIM_ID = IE.ISM_REF_ID
						LEFT JOIN (
							SELECT IDK_ISEMRI_ID, MIN(TB_ISEMRI_KAYNAK_ID) AS MIN_PERSONEL_ID
							FROM orjin.TB_ISEMRI_KAYNAK
							GROUP BY IDK_ISEMRI_ID
						) MIN_PRS ON MIN_PRS.IDK_ISEMRI_ID = IE.TB_ISEMRI_ID
						LEFT JOIN orjin.TB_ISEMRI_KAYNAK ISEMRI_KAYNAK ON ISEMRI_KAYNAK.IDK_ISEMRI_ID = IE.TB_ISEMRI_ID AND ISEMRI_KAYNAK.TB_ISEMRI_KAYNAK_ID = MIN_PRS.MIN_PERSONEL_ID
						LEFT JOIN orjin.TB_PERSONEL PRS ON PRS.TB_PERSONEL_ID = ISEMRI_KAYNAK.IDK_REF_ID
						WHERE 
							IE.TB_ISEMRI_ID = @ISEMRI_ID ";

				using (var cnn = klas.baglan())
				{
					listem = cnn.Query<WebVersionIsEmriForm>(query, new { @ISEMRI_ID = id }).ToList();
				}
				return listem[0];
			}

			catch (Exception e)
			{
				bldr.Error = true;
				bldr.Aciklama = e.Message;
				return bldr;
			}
		}

		public object GetIsTalepFormById([FromUri] long id)
		{
			List<WebVersionIsTalpForm> listem = new List<WebVersionIsTalpForm>();
			Bildirim bldr = new Bildirim();
			try
			{
				query = @" 
						SELECT
						  IST.TB_IS_TALEP_ID,
						  IST.IST_KOD ,
						  IST.IST_BASLAMA_TARIHI,
						  KLL.KLL_TANIM AS IST_TALEP_EDEN,
						  ISK.ISK_ISIM AS IST_IS_TAKIPCISI,
						  ISM.ISM_ISEMRI_NO AS IST_ISEMRI_NO,
						  SOC.SOC_TANIM AS IST_ONCELIK,
						  KOD1.KOD_TANIM AS IST_BILDIRILEN_BINA,
						  KOD2.KOD_TANIM AS IST_BILDIRILEN_KAT,
						  KOD3.KOD_TANIM AS IST_SERVIS,
						  KOD4.KOD_TANIM AS IST_KATEGORI,
						  KOD5.KOD_TANIM AS IST_IRTIBAT,
						  MKN.MKN_TANIM AS IST_MAKINE_TANIM,
						  MKN.MKN_KOD AS IST_MAKINE_KOD,
						  EKP.EKP_TANIM AS IST_EKIPMAN_TANIM,
						  EKP.EKP_KOD AS IST_EKIPMAN_KOD,
						  IST.IST_IRTIBAT_TELEFON,
						  IST.IST_MAIL_ADRES,
						  IST.IST_KONU,
						  IST.IST_ACIKLAMA
						FROM
						  orjin.TB_IS_TALEBI IST
						LEFT JOIN PBTPRO_MASTER.orjin.TB_KULLANICI KLL ON IST.IST_TALEP_EDEN_ID = KLL.TB_KULLANICI_ID
						LEFT JOIN orjin.TB_IS_TALEBI_KULLANICI ISK ON IST.IST_IS_TAKIPCISI_ID = ISK.TB_IS_TALEBI_KULLANICI_ID
						LEFT JOIN orjin.TB_ISEMRI ISM ON IST.IST_ISEMRI_ID = ISM.TB_ISEMRI_ID
						LEFT JOIN orjin.TB_SERVIS_ONCELIK SOC ON IST.IST_ONCELIK_ID = SOC.TB_SERVIS_ONCELIK_ID
						LEFT JOIN orjin.TB_KOD KOD1 ON IST.IST_BILDIRILEN_BINA = KOD1.TB_KOD_ID
						LEFT JOIN orjin.TB_KOD KOD2 ON IST.IST_BILDIRILEN_KAT = KOD2.TB_KOD_ID
						LEFT JOIN orjin.TB_KOD KOD3 ON IST.IST_SERVIS_NEDENI_KOD_ID = KOD3.TB_KOD_ID
						LEFT JOIN orjin.TB_KOD KOD4 ON IST.IST_KOTEGORI_KODI_ID = KOD4.TB_KOD_ID
						LEFT JOIN orjin.TB_KOD KOD5 ON IST.IST_IRTIBAT_KOD_KOD_ID = KOD5.TB_KOD_ID
						LEFT JOIN orjin.TB_MAKINE MKN ON IST.IST_MAKINE_ID = MKN.TB_MAKINE_ID
						LEFT JOIN orjin.TB_EKIPMAN EKP ON IST.IST_EKIPMAN_ID = EKP.TB_EKIPMAN_ID
						WHERE
						  IST.TB_IS_TALEP_ID = @IS_TALEP_ID
						";
				using (var cnn = klas.baglan())
				{
					listem = cnn.Query<WebVersionIsTalpForm>(query, new { @IS_TALEP_ID = id }).ToList();
				}
				return listem[0];
			}

			catch (Exception e)
			{
				bldr.Error = true;
				bldr.Aciklama = e.Message;
				return bldr;
			}
		}

		public object GetBakimFormByIdToPdf([FromUri] long id)
		{
			List<WebVersionBakimForm> listem = new List<WebVersionBakimForm>();
			Bildirim bldr = new Bildirim();
			try
			{
				query = @" 
						SELECT 
						TB_IS_TANIM_ID,
						IST_KOD,
						IST_TANIM,
						orjin.UDF_KOD_TANIM(IST_TIP_KOD_ID) AS IST_TIP,
						orjin.UDF_KOD_TANIM(IST_GRUP_KOD_ID) AS IST_GRUP,
						orjin.UDF_KOD_TANIM(IST_NEDEN_KOD_ID) AS IST_NEDEN,
						orjin.UDF_KOD_TANIM(IST_ONCELIK_ID) AS IST_ONCELIK,
						T.TLM_TANIM AS IST_TALIMAT,
						A.ATL_TANIM AS IST_ATOLYE,
						C.CAR_TANIM AS IST_FIRMA,
						IST_CALISMA_SURE,
						IST_DURUS_SURE,
						P.PRS_ISIM AS IST_PERSONEL
					FROM 
						[PBTPRO_1].[orjin].[TB_IS_TANIM]
					LEFT JOIN orjin.TB_TALIMAT T ON T.TB_TALIMAT_ID = IST_TALIMAT_ID
					LEFT JOIN orjin.TB_ATOLYE A ON A.TB_ATOLYE_ID = IST_ATOLYE_ID
					LEFT JOIN orjin.TB_CARI C ON C.TB_CARI_ID = IST_FIRMA_ID
					LEFT JOIN orjin.TB_PERSONEL P ON P.TB_PERSONEL_ID = IST_PERSONEL_ID
					WHERE 
						IST_DURUM = 'BAKIM' 
						AND TB_IS_TANIM_ID = @IS_TANIM_ID;

						";
				using (var cnn = klas.baglan())
				{
					listem = cnn.Query<WebVersionBakimForm>(query, new { @IS_TANIM_ID = id }).ToList();
				}
				return listem[0];
			}

			catch (Exception e)
			{
				bldr.Error = true;
				bldr.Aciklama = e.Message;
				return bldr;
			}
		}

		public object GetArizaFormByIdToPdf([FromUri] long id)
		{
			List<WebVersionBakimForm> listem = new List<WebVersionBakimForm>();
			Bildirim bldr = new Bildirim();
			try
			{
				query = @" 
						SELECT 
						TB_IS_TANIM_ID,
						IST_KOD,
						IST_TANIM,
						orjin.UDF_KOD_TANIM(IST_TIP_KOD_ID) AS IST_TIP,
						orjin.UDF_KOD_TANIM(IST_GRUP_KOD_ID) AS IST_GRUP,
						orjin.UDF_KOD_TANIM(IST_NEDEN_KOD_ID) AS IST_NEDEN,
						orjin.UDF_KOD_TANIM(IST_ONCELIK_ID) AS IST_ONCELIK,
						T.TLM_TANIM AS IST_TALIMAT,
						A.ATL_TANIM AS IST_ATOLYE,
						C.CAR_TANIM AS IST_FIRMA,
						IST_CALISMA_SURE,
						IST_DURUS_SURE,
						P.PRS_ISIM AS IST_PERSONEL
					FROM 
						[PBTPRO_1].[orjin].[TB_IS_TANIM]
					LEFT JOIN orjin.TB_TALIMAT T ON T.TB_TALIMAT_ID = IST_TALIMAT_ID
					LEFT JOIN orjin.TB_ATOLYE A ON A.TB_ATOLYE_ID = IST_ATOLYE_ID
					LEFT JOIN orjin.TB_CARI C ON C.TB_CARI_ID = IST_FIRMA_ID
					LEFT JOIN orjin.TB_PERSONEL P ON P.TB_PERSONEL_ID = IST_PERSONEL_ID
					WHERE 
						IST_DURUM = 'ARIZA' 
						AND TB_IS_TANIM_ID = @IS_TANIM_ID;

						";
				using (var cnn = klas.baglan())
				{
					listem = cnn.Query<WebVersionBakimForm>(query, new { @IS_TANIM_ID = id }).ToList();
				}
				return listem[0];
			}

			catch (Exception e)
			{
				bldr.Error = true;
				bldr.Aciklama = e.Message;
				return bldr;
			}
		}
	}
}