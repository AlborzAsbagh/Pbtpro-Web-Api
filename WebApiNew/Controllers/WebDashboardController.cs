using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Dapper;
using WebApiNew.Filters;

namespace WebApiNew.Controllers
{
	[MyBasicAuthenticationFilter]
	public class WebDashboardController : ApiController
	{
		Util klas = new Util();
		string query = "";
		List<Prm> parametreler = new List<Prm>();
		Parametreler prms = new Parametreler();

		[Route("api/GetDashboardCards")]
		[HttpGet]
		public object GetDashboardCards([FromUri] int ID)
		{
			try
			{
				prms.Clear();
				prms.Add("ISM_ID", ID);
				WebDashboardCards entity = new WebDashboardCards();
				query = @"
						  SELECT
						  (SELECT count(*) FROM orjin.TB_ISEMRI WHERE ISM_KAPATILDI = 0) AS ACIK_IS_EMIRLERI,
						  (SELECT count(*) FROM orjin.TB_IS_TALEBI WHERE IST_DURUM_ID != 4 AND IST_DURUM_ID != 5) AS DEVAM_EDEN_IS_TALEPLERI,
						  (SELECT count(*) FROM orjin.TB_STOK stk where stk.STK_MIKTAR < stk.STK_MIN_MIKTAR) AS DUSUK_STOKLU_MALZEMELER  ,
						  (SELECT count(*) FROM orjin.TB_MAKINE where MKN_AKTIF = 1  ) AS MAKINE_SAYISI";

				DataTable dt = klas.GetDataTable(query, prms.PARAMS);
				entity.ACIK_IS_EMIRLERI = Convert.ToInt32(dt.Rows[0]["ACIK_IS_EMIRLERI"]);
				entity.DEVAM_EDEN_IS_TALEPLERI = Convert.ToInt32(dt.Rows[0]["DEVAM_EDEN_IS_TALEPLERI"]);
				entity.DUSUK_STOKLU_MALZEMELER = Convert.ToInt32(dt.Rows[0]["DUSUK_STOKLU_MALZEMELER"]);
				entity.MAKINE_SAYISI = Convert.ToInt32(dt.Rows[0]["MAKINE_SAYISI"]);
				return entity;
			}
			catch(Exception ex) 
			{
				return Json(new { error = ex.Message });
			}
		}

		[Route("api/GetMakineTipEnvanter")]
		[HttpGet]
		public object GetMakineTipEnvanter([FromUri] int ID)
		{
			List<MakineTipEnvanteri> makineTipList = new List<MakineTipEnvanteri>();
			prms.Clear();
			prms.Add("ISM_ID", ID);
			try
			{
				query = " SELECT * FROM orjin.UDF_WEB_DASH_TIPE_GORE_MAKINE_SAYISI() ";
				DataTable dt = klas.GetDataTable(query, prms.PARAMS);

				for (int i = 0; i < dt.Rows.Count; i++)
				{
					makineTipList.Add(new MakineTipEnvanteri
						(

						Convert.ToInt32(dt.Rows[i]["TB_KOD_ID"]),
						Convert.ToString(dt.Rows[i]["MAKINE_TIPI"]),
						Convert.ToInt32(dt.Rows[i]["MAKINE_SAYISI"])

						));
				}
				return makineTipList;
			}
			catch(Exception ex) 
			{
				return Json(new { ex.Message });
			}
		}

		[Route("api/GetIsTalepTipEnvanter")]
		[HttpGet]
		public object GetIsTalepTipEnvanter([FromUri] int ID)
		{
			List<IsTalebiTipi> isTalebiTipList = new List<IsTalebiTipi>();
			prms.Clear();
			prms.Add("ISM_ID", ID);
			try
			{
				query = " SELECT * FROM orjin.UDF_WEB_DASH_IS_TALEP_TIP_SAYISI() ";
				DataTable dt = klas.GetDataTable(query, prms.PARAMS);

				for (int i = 0; i < dt.Rows.Count; i++)
				{
					isTalebiTipList.Add(new IsTalebiTipi
						(

						Convert.ToInt32(dt.Rows[i]["TB_KOD_ID"]),
						Convert.ToString(dt.Rows[i]["TALEP_TIPI"]),
						Convert.ToInt32(dt.Rows[i]["TALEP_SAYISI"])

						));
				}
				return isTalebiTipList;
			}
			catch (Exception ex)
			{
				return Json(new { ex.Message });
			}
		}

		[Route("api/GetIsEmriTipEnvanter")]
		[HttpGet]
		public object GetIsEmriTipEnvanter([FromUri] int ID)
		{
			List<IsEmriTipi> isEmriTipList = new List<IsEmriTipi>();
			prms.Clear();
			prms.Add("ISM_ID", ID);
			try
			{
				query = " SELECT * FROM orjin.UDF_WEB_DASH_ISEMRI_TIP_SAYISI() ";
				DataTable dt = klas.GetDataTable(query, prms.PARAMS);

				for (int i = 0; i < dt.Rows.Count; i++)
				{
					isEmriTipList.Add(new IsEmriTipi
						(

						Convert.ToInt32(dt.Rows[i]["TB_ISEMRI_TIP_ID"]),
						Convert.ToString(dt.Rows[i]["ISEMRI_TIPI"]),
						Convert.ToInt32(dt.Rows[i]["ISEMRI_SAYISI"])

						));
				}
				return isEmriTipList;
			}
			catch (Exception ex)
			{
				return Json(new { ex.Message });
			}
		}

		[Route("api/GetIsEmriDurumEnvanter")]
		[HttpGet]
		public object GetIsEmriDurumEnvanter([FromUri] int ID)
		{
			List<IsEmriDurumu> isEmriDurumList = new List<IsEmriDurumu>();
			prms.Clear();
			prms.Add("ISM_ID", ID);
			try
			{
				query = " SELECT * FROM orjin.UDF_WEB_DASH_ISEMRI_DURUM_SAYISI() ";
				DataTable dt = klas.GetDataTable(query, prms.PARAMS);

				for (int i = 0; i < dt.Rows.Count; i++)
				{
					isEmriDurumList.Add(new IsEmriDurumu
						(

						Convert.ToInt32(dt.Rows[i]["TB_KOD_ID"]),
						Convert.ToString(dt.Rows[i]["ISEMRI_DURUMU"]),
						Convert.ToInt32(dt.Rows[i]["ISEMRI_SAYISI"])

						));
				}
				return isEmriDurumList;
			}
			catch (Exception ex)
			{
				return Json(new { ex.Message });
			}
		}

		[Route("api/GetIsTalepDurumEnvanter")]
		[HttpGet]
		public object GetIsTalepDurumEnvanter([FromUri] int ID)
		{
			List<IsTalepDurumu> isTalepDurumList = new List<IsTalepDurumu>();
			prms.Clear();
			prms.Add("ISM_ID", ID);
			try
			{
				query = " SELECT * FROM orjin.UDF_WEB_DASH_IS_TALEP_DURUM_SAYISI() ";
				DataTable dt = klas.GetDataTable(query, prms.PARAMS);

				for (int i = 0; i < dt.Rows.Count; i++)
				{
					isTalepDurumList.Add(new IsTalepDurumu
						(

						Convert.ToInt32(dt.Rows[i]["IST_DURUM_ID"]),
						Convert.ToInt32(dt.Rows[i]["IS_TALEP_SAYISI"])

						));
				}
				return isTalepDurumList;
			}
			catch (Exception ex)
			{
				return Json(new { ex.Message });
			}
		}

		[Route("api/GetTamamlanmisIsEmirleriIsTalepleri")]
		[HttpGet]
		public object GetTamamlanmisIsEmirleriIsTalepleri([FromUri] int ID , [FromUri] int year)
		{
			List<TamamlananIsEmrileriIsTalepleri> listem = new List<TamamlananIsEmrileriIsTalepleri> ();

			prms.Clear();
			prms.Add("ISM_ID", ID);
			prms.Add("YEAR", year);

			try
			{
				query = @" SELECT * FROM orjin.UDF_WEB_DASH_AYLIK_TAMAMLANAN_GENEL(@YEAR) ";
				DataTable dt = klas.GetDataTable(query, prms.PARAMS);

				for (int i = 0; i < dt.Rows.Count; i++)
				{
					listem.Add(new TamamlananIsEmrileriIsTalepleri(
							Convert.ToInt32(dt.Rows[i]["AY"]),
							Convert.ToInt32(dt.Rows[i]["DEGER"]),
							Convert.ToString(dt.Rows[i]["TIP"])
						));
				}

				return listem;
			}
			catch (Exception ex)
			{
				return Json(new { ex.Message });
			}
		}

		[Route("api/GetAylikBakimIsEmriMaliyet")]
		[HttpGet]
		public object GetAylikBakimIsEmriMaliyet([FromUri] int ID, [FromUri] int year)
		{
			List<AylikBakimIsEmrileri> aylikBakimIsEmriMaliyet = new List<AylikBakimIsEmrileri>();
			prms.Clear();
			prms.Add("ISM_ID", ID);
			prms.Add("YEAR", year);
			try
			{
				query = @" SELECT * FROM orjin.UDF_WEB_DASH_AYLIK_BAKIM_MALIYET(@YEAR) ";
				DataTable dt = klas.GetDataTable(query, prms.PARAMS);

				for (int i = 0; i < dt.Rows.Count; i++)
				{
					aylikBakimIsEmriMaliyet.Add(new AylikBakimIsEmrileri
						(

						Convert.ToInt32(dt.Rows[i]["AY"]),
						Convert.ToInt32(dt.Rows[i]["AYLIK_BAKIM_ISEMRI_MALIYET"])

						));
				}
				return aylikBakimIsEmriMaliyet;
			}
			catch (Exception ex)
			{
				return Json(new { ex.Message });
			}
		}

	}
}
