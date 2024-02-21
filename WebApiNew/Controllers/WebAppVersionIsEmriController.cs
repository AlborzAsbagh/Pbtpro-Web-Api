using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using Dapper;
using Newtonsoft.Json.Linq;
using WebApiNew.Filters;
using WebApiNew.Models;
using WebApiNew.Utility.Abstract;


/*
 * 
 * 
 * 
 *      IS EMRI Controller For Web App Versions
 *      ( Mobil ve Web Arasındaki ortak fonksiyonlar kendi kontrollerinde yazılmıştır . )
 *
 *
 *
 */

namespace WebApiNew.Controllers
{
	[MyBasicAuthenticationFilter]
	public class WebAppVersionIsEmriController : ApiController
	{
		Util klas = new Util();
		Parametreler prms = new Parametreler();
		private readonly ILogger _logger;
		string query = "";
		SqlCommand cmd = null;

		public WebAppVersionIsEmriController(ILogger logger)
		{
			_logger = logger;
		}


		[Route("api/GetIsEmriFullList")]
		[HttpPost]
		public Object GetIsEmriFullList([FromUri] int id, [FromUri] string parametre, [FromBody] JObject filtreler, [FromUri] int pagingDeger = 1)
		{

			List<WebVersionIsEmriModel> listem = new List<WebVersionIsEmriModel>();

			int pagingIlkDeger = pagingDeger == 1 ? 1 : ((pagingDeger * 50) - 50);
			int pagingSonDeger = pagingIlkDeger == 1 ? 50 : ((pagingDeger * 50));
			int toplamIsEmriSayisi = 0;
			string MAIN_QUERY = "";
			string TOTAL_IS_EMRI_QUERY = "";
			string isParametreForTotalIsEmri = "";
			SqlCommand cmd = null;

			try
			{
				MAIN_QUERY = "SELECT * from (select ROW_NUMBER() over (order by TB_ISEMRI_ID DESC) as RowIndex , * FROM VW_WEB_VERSION_ISEMRI WHERE 1 = 1 ";

				string lokasyonQuery = "", durumQuery = "", isemritipQuery = "", customfilterQuery = "";
				int counter = 0;

				if (filtreler != null || (!string.IsNullOrEmpty(parametre)))
				{
					JObject isemritipleri = filtreler?["isemritipleri"] as JObject;
					JObject durumlar = filtreler?["durumlar"] as JObject;
					JObject lokasyonlar = filtreler?["lokasyonlar"] as JObject;
					JObject customfilter = filtreler?["customfilter"] as JObject;

					if (isemritipleri != null && isemritipleri.Count != 0)
					{
						isemritipQuery = " ( ";
						counter = 0;

						foreach (var property in isemritipleri)
						{
							isemritipQuery += " ISEMRI_TIP LIKE '" + property.Value + "%' ";

							if (counter < isemritipleri.Count - 1)
							{
								isemritipQuery += " or ";
							}

							counter++;
						}

						isemritipQuery += " ) ";
					}

					if (durumlar != null && durumlar.Count != 0)
					{
						durumQuery = " ( ";
						counter = 0;

						foreach (var property in durumlar)
						{
							durumQuery += " DURUM LIKE '" + property.Value + "%' ";

							if (counter < durumlar.Count - 1)
							{
								durumQuery += " or ";
							}
							counter++;
						}
						durumQuery += " ) ";
					}

					if (lokasyonlar != null && lokasyonlar.Count != 0)
					{
						lokasyonQuery = " ( ";
						counter = 0;

						foreach (var property in lokasyonlar)
						{
							lokasyonQuery += " LOKASYON LIKE '" + property.Value + "%' ";
							if (counter < lokasyonlar.Count - 1)
							{
								lokasyonQuery += " or ";
							}
							counter++;
						}
						lokasyonQuery += " ) ";
					}

					if (customfilter != null && customfilter.Count != 0)
					{
						customfilterQuery = " ( ";
						counter = 0;
						foreach (var property in customfilter)
						{
							customfilterQuery += $" {property.Key} LIKE '" + property.Value + "%'";
							if (counter < customfilter.Count - 1)
							{
								customfilterQuery += " or ";
							}
							counter++;
						}
						customfilterQuery += " ) ";

					}

					if (!string.IsNullOrEmpty(parametre))
					{
						MAIN_QUERY += $" AND ( KAPALI  LIKE '%" + parametre + "%' or " +
									  $" ISEMRI_NO  LIKE '%" + parametre + "%' or " +
									  $" KONU  LIKE '%" + parametre + "%' or " +
									  $" ISEMRI_TIP  LIKE '%" + parametre + "%' or " +
									  $" DURUM  LIKE '%" + parametre + "%' or " +
									  $" LOKASYON  LIKE '%" + parametre + "%' or " +
									  $" MAKINE_KODU  LIKE '%" + parametre + "%' or " +
									  $" MAKINE_TANIMI  LIKE '%" + parametre + "%' or " +
									  $" IS_TIPI  LIKE '%" + parametre + "%' or " +
									  $" IS_NEDENI  LIKE '%" + parametre + "%' or " +
									  $" ATOLYE  LIKE '%" + parametre + "%' or " +
									  $" PERSONEL_ADI LIKE '%" + parametre + "%' ) ";

						isParametreForTotalIsEmri = $" AND ( KAPALI  LIKE '%" + parametre + "%' or " +
									  $" ISEMRI_NO  LIKE '%" + parametre + "%' or " +
									  $" KONU  LIKE '%" + parametre + "%' or " +
									  $" ISEMRI_TIP  LIKE '%" + parametre + "%' or " +
									  $" DURUM  LIKE '%" + parametre + "%' or " +
									  $" LOKASYON  LIKE '%" + parametre + "%' or " +
									  $" MAKINE_KODU  LIKE '%" + parametre + "%' or " +
									  $" MAKINE_TANIMI  LIKE '%" + parametre + "%' or " +
									  $" IS_TIPI  LIKE '%" + parametre + "%' or " +
									  $" IS_NEDENI  LIKE '%" + parametre + "%' or " +
									  $" ATOLYE  LIKE '%" + parametre + "%' or " +
									  $" PERSONEL_ADI LIKE '%" + parametre + "%' ) ";
					}

					if (isemritipleri != null && isemritipleri.Count != 0) MAIN_QUERY += " AND " + isemritipQuery;
					if (durumlar != null && durumlar.Count != 0) MAIN_QUERY += " AND " + durumQuery;
					if (lokasyonlar != null && lokasyonlar.Count != 0) MAIN_QUERY += " AND " + lokasyonQuery;
					if (customfilter != null && customfilter.Count != 0) MAIN_QUERY += " AND " + customfilterQuery;

					MAIN_QUERY += $" ) as SubRow where SubRow.RowIndex >= {pagingIlkDeger} and SubRow.RowIndex < {pagingSonDeger} ";

					TOTAL_IS_EMRI_QUERY = " select count(*) from (select * from dbo.VW_WEB_VERSION_ISEMRI where 1=1 ";
					TOTAL_IS_EMRI_QUERY += isParametreForTotalIsEmri;

					if (isemritipleri != null && isemritipleri.Count != 0) TOTAL_IS_EMRI_QUERY += " AND " + isemritipQuery;
					if (durumlar != null && durumlar.Count != 0) TOTAL_IS_EMRI_QUERY += " AND " + durumQuery;
					if (lokasyonlar != null && lokasyonlar.Count != 0) TOTAL_IS_EMRI_QUERY += " AND " + lokasyonQuery;
					if (customfilter != null && customfilter.Count != 0) TOTAL_IS_EMRI_QUERY += " AND " + customfilterQuery;

					TOTAL_IS_EMRI_QUERY += " ) as TotalIsEmriSayisi ";
				}

				else
				{

					MAIN_QUERY = $" SELECT * FROM ( SELECT ROW_NUMBER() OVER (ORDER BY TB_ISEMRI_ID DESC) as RowIndex , * FROM VW_WEB_VERSION_ISEMRI ) as SubRow where SubRow.RowIndex >= {pagingIlkDeger} and SubRow.RowIndex < {pagingSonDeger} ";
					TOTAL_IS_EMRI_QUERY = " select count(*) from (select * from dbo.VW_WEB_VERSION_ISEMRI where 1=1) as TotalIsEmriSayisi ";
				}


				using(var cnn = klas.baglan())
				{
					listem = cnn.Query<WebVersionIsEmriModel>(MAIN_QUERY).ToList();
					cmd = new SqlCommand(TOTAL_IS_EMRI_QUERY, cnn);
					toplamIsEmriSayisi = (int)cmd.ExecuteScalar();
				}

				klas.kapat();
				return Json(new { page = (int)Math.Ceiling((decimal)toplamIsEmriSayisi / 50), list = listem });
			}
			catch (Exception e)
			{
				klas.kapat();
				return Json(e.Message);
			}
		}


		[Route("api/IsEmriDurumVarsayilanYap")]
		[HttpGet]
		public Object IsEmriDurumVarsayilanYap([FromUri] int kodId,[FromUri] bool isVarsayilan)
		{
			try
			{
				query = $" update orjin.TB_KOD set KOD_ISM_DURUM_VARSAYILAN = 0 where KOD_GRUP = 32801 ";
				query += $" update orjin.TB_KOD set KOD_ISM_DURUM_VARSAYILAN = 1 where TB_KOD_ID = {kodId} and KOD_GRUP = 32801 ";

				using (var con = klas.baglan())
				{
					cmd = new SqlCommand(query, con);
					cmd.ExecuteNonQuery();
				}
				klas.kapat();
				return Json(new { has_error = false , status_code = 200 ,  status = "Process completed successfully !" });
			}
			catch (Exception e)
			{
				klas.kapat();
				return Json(new { has_error = true , status_code = 500 , status = e.Message });
			}

		}

		[Route("api/AddIsEmriDurumDegisikligi")]
		[HttpPost]
		public Object AddIsEmriDurumDegisikligi([FromBody] IsEmriLog entity)
		{
			try
			{
				query = " insert into orjin.TB_ISEMRI_LOG";
				query += @" (ISL_ISEMRI_ID , ISL_KULLANICI_ID , ISL_TARIH , ISL_SAAT , ISL_ISLEM , ISL_ACIKLAMA , ISL_DURUM_ESKI_KOD_ID ,
						ISL_DURUM_YENI_KOD_ID , ISL_OLUSTURAN_ID , ISL_OLUSTURMA_TARIH , ISL_DEGISTIRME_TARIH ) values ";

				if (entity != null)
				{
					klas.baglan();
					query += @" (@ISL_ISEMRI_ID , @ISL_KULLANICI_ID , @ISL_TARIH , @ISL_SAAT , @ISL_ISLEM , @ISL_ACIKLAMA , @ISL_DURUM_ESKI_KOD_ID ,
						@ISL_DURUM_YENI_KOD_ID , @ISL_OLUSTURAN_ID , @ISL_OLUSTURMA_TARIH , @ISL_DEGISTIRME_TARIH ) ";
					prms.Clear();
					prms.Add("ISL_ISEMRI_ID", entity.ISL_ISEMRI_ID);
					prms.Add("ISL_KULLANICI_ID", entity.ISL_KULLANICI_ID);
					prms.Add("ISL_TARIH", DateTime.Now);
					prms.Add("ISL_SAAT", DateTime.Now.ToString("HH:mm:ss"));
					prms.Add("ISL_ISLEM", entity.ISL_ISLEM);
					prms.Add("ISL_ACIKLAMA", entity.ISL_ACIKLAMA);
					prms.Add("ISL_DURUM_ESKI_KOD_ID", entity.ISL_DURUM_ESKI_KOD_ID);
					prms.Add("ISL_DURUM_YENI_KOD_ID", entity.ISL_DURUM_YENI_KOD_ID);
					prms.Add("ISL_OLUSTURAN_ID", entity.ISL_OLUSTURAN_ID);
					prms.Add("ISL_OLUSTURMA_TARIH", DateTime.Now);
					prms.Add("ISL_DEGISTIRME_TARIH", DateTime.Now);

					klas.cmd(query, prms.PARAMS);
					klas.kapat();
					return Json(new { success = "Ekleme başarılı " });
				}
				else
				{
					return null;

				}
			} 
			catch (Exception ex) 
			{
				klas.kapat();
				return Json(new { error = " Ekleme başarısız " });
			}
		}

		
		[Route("api/IsEmriEkleVarsiylanlar")]
		[HttpGet]
		public Object IsEmriEkleVarsiylanlar()
		{
			string query = @"SELECT TB_KOD_ID AS ID, KOD_TANIM AS TANIM, 'IS_EMRI_DURUM_VARSAYILAN' AS TABLO_TANIMI
								FROM orjin.TB_KOD
								WHERE KOD_ISM_DURUM_VARSAYILAN = 1 AND KOD_GRUP = 32801

								UNION ALL

								SELECT TB_ISEMRI_TIP_ID AS ID, IMT_TANIM AS TANIM, 'IS_EMRI_TIP_VARSAYILAN' AS TABLO_TANIMI
								FROM orjin.TB_ISEMRI_TIP
								WHERE IMT_VARSAYILAN = 1 

								UNION ALL

								SELECT TB_SERVIS_ONCELIK_ID AS ID, SOC_TANIM AS TANIM, 'SERVIS_ONCELIK' AS TABLO_TANIMI
								FROM orjin.TB_SERVIS_ONCELIK
								WHERE SOC_VARSAYILAN = 1";
			var klas = new Util();
			List<IsEmriEkleVarsayilanDegerler> listem = new List<IsEmriEkleVarsayilanDegerler>();
			using (var cnn = klas.baglan())
			{
				listem = cnn.Query<IsEmriEkleVarsayilanDegerler>(query).ToList();
			}

			return Json(new { is_emri_varsayilanlar = listem });
		}
	}
}


