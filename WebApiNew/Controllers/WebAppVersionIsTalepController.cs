using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using WebApiNew.Filters;
using WebApiNew.Models;


/*
 * 
 * 
 * 
 *      IS TALEP Controller For Web App Versions
 *      ( Mobil ve Web Arasındaki ortak fonksiyonlar kendi kontrollerinde yazılmıştır . )
 *
 *
 *
 */


namespace WebApiNew.Controllers
{

	[MyBasicAuthenticationFilter]
	public class WebAppVersionIsTalepController : ApiController
	{
		Util klas = new Util();
		string query = "";
		SqlCommand cmd = null;
		Parametreler prms = new Parametreler();


		[Route("api/GetIsTalepFullList")]
		[HttpPost]
		public object GetIsTalepFullList([FromUri] string parametre, [FromBody] JObject filters, [FromUri] int pagingDeger = 1 , [FromUri] int? lokasyonId = 0)
		{
			int pagingIlkDeger = pagingDeger == 1 ? 1 : ((pagingDeger * 10) - 10);
			int pagingSonDeger = pagingIlkDeger == 1 ? 10 : ((pagingDeger * 10));
			int toplamIsTalepSayisi = 0;
			int counter = 0;
			string toplamIsTalepSayisiQuery = "";

			List<IsTalep> listem = new List<IsTalep>();
			try
			{
				query = @" SELECT * FROM ( SELECT * , ROW_NUMBER() OVER (ORDER BY TB_IS_TALEP_ID DESC) AS subRow FROM orjin.VW_IS_TALEP where 1=1 ";
				toplamIsTalepSayisiQuery = @" select count(*) from (select * from orjin.VW_IS_TALEP where 1=1 ";

				if (!string.IsNullOrEmpty(parametre))
				{
					query += $" and ( IST_KOD like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" and ( IST_KOD like '%{parametre}%' or ";
					query += $" IST_KONU like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" IST_KONU like '%{parametre}%' or ";
					query += $" IST_BILDIREN_LOKASYON like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" IST_BILDIREN_LOKASYON like '%{parametre}%' or ";
					query += $" IST_MAKINE_KOD like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" IST_MAKINE_KOD like '%{parametre}%' or ";
					query += $" IST_TALEP_EDEN_ADI like '%{parametre}%' ) "; toplamIsTalepSayisiQuery += $" IST_TALEP_EDEN_ADI like '%{parametre}%' ) ";
				}

				if ((filters["customfilters"] as JObject) != null && (filters["customfilters"] as JObject).Count > 0)
				{
					query += " and ( ";
					toplamIsTalepSayisiQuery += " and ( ";
					counter = 0;
					foreach (var property in filters["customfilters"] as JObject)
					{
						query += $" {property.Key} LIKE '%{property.Value}%' ";
						toplamIsTalepSayisiQuery += $" {property.Key} LIKE '%{property.Value}%' ";
						if (counter < (filters["customfilters"] as JObject).Count - 1)
						{
							query += " and ";
							toplamIsTalepSayisiQuery += " and ";
						}
						counter++;
					}
					query += " ) ";
					toplamIsTalepSayisiQuery += " ) ";
				}

				if (lokasyonId > 0 && lokasyonId != null)
				{
					query += $" and IST_BILDIREN_LOKASYON_ID = {lokasyonId} ";
					toplamIsTalepSayisiQuery += $" and IST_BILDIREN_LOKASYON_ID = {lokasyonId} ";
				}

				if( (filters["durumlar"] as JObject) != null && (filters["durumlar"] as JObject).Count > 0)
				{
					query += " and (  ";
					toplamIsTalepSayisiQuery += " and (  ";
					counter = 0;
					foreach (var property in filters["durumlar"] as JObject)
					{
						query += $" IST_DURUM_ID LIKE '%{property.Value}%' ";
						toplamIsTalepSayisiQuery += $" IST_DURUM_ID LIKE '%{property.Value}%' ";

						if (counter < (filters["durumlar"] as JObject).Count - 1)
						{
							query += " or ";
							toplamIsTalepSayisiQuery += " or ";
						}
						counter++;
					}
					query += " ) ";
					toplamIsTalepSayisiQuery += " ) ";
				}

				query += $" ) RowIndex WHERE RowIndex.subRow >= {pagingIlkDeger} AND RowIndex.subRow < {pagingSonDeger}";
				toplamIsTalepSayisiQuery += ") as TotalIsTalepSayisi";

				using (var cnn = klas.baglan())
				{
					listem = cnn.Query<IsTalep>(query).ToList();
					cmd = new SqlCommand(toplamIsTalepSayisiQuery, cnn);
					toplamIsTalepSayisi = (int)cmd.ExecuteScalar();
				}
				klas.kapat();
				return Json(new { page = (int)Math.Ceiling((decimal)toplamIsTalepSayisi / 10), is_talep_listesi = listem });

			}
			catch (Exception e)
			{
				klas.kapat();
				return Json(new { error = e.Message });
			}
		}



		[Route("api/AddIsTalep")]
		[HttpPost]
		public async Task<object> AddIsTalep([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0)
					{
						query = " insert into orjin.TB_IS_TALEBI  ( IST_OLUSTURMA_TARIH , ";
						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" {item.Key} , ";
							else query += $" {item.Key} ";
							count++;
						}

						query += $" ) values ( '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , ";
						count = 0;

						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" '{item.Value}' , ";
							else query += $" '{item.Value}' ";
							count++;
						}
						query += " ) ";
						await cnn.ExecuteAsync(query);

						int SonTalepId = cnn.QueryFirstOrDefault<int>("SELECT TOP 1 TB_IS_TALEP_ID FROM orjin.TB_IS_TALEBI ORDER BY  TB_IS_TALEP_ID DESC");
						if (SonTalepId > 0)
						{
							var itl = new IsTalebiLog
							{
								ITL_IS_TANIM_ID = SonTalepId,
								ITL_KULLANICI_ID = Convert.ToInt32(entity.GetValue("IST_TALEP_EDEN_ID")),
								ITL_TARIH = Convert.ToDateTime(entity.GetValue("IST_ACILIS_TARIHI")),
								ITL_SAAT = Convert.ToString(entity.GetValue("IST_ACILIS_SAATI")),
								ITL_ISLEM = "Yeni iş talebi",
								ITL_ACIKLAMA = String.Format("Talep no: '{0}' - Konu :'{1}'", Convert.ToString(entity.GetValue("IST_KOD")),
									Convert.ToString(entity.GetValue("IST_TANIMI"))),
								ITL_ISLEM_DURUM = "AÇIK",
								ITL_TALEP_ISLEM = "Yeni İş Talebi",
								ITL_OLUSTURAN_ID = Convert.ToInt32(entity.GetValue("IST_OLUSTURAN_ID")),
								ITL_OLUSTURMA_TARIH = DateTime.Now
							};
							cnn.Insert(itl);
						}

						return Json(new { has_error = false, status_code = 201, status = "Added Successfully" });
					}
					else return Json(new { has_error = false, status_code = 400, status = "Bad Request ( entity may be null or 0 lentgh)" });
				}
			}
			catch (Exception ex)
			{
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}


		[Route("api/UpdateIsTalep")]
		[HttpPost]
		public async Task<Object> UpdateIsTalep([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_IS_TALEP_ID")) >= 1)
					{
						query = " update orjin.TB_IS_TALEBI set ";
						foreach (var item in entity)
						{

							if (item.Key.Equals("TB_IS_TALEP_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , IST_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ";
						query += $" where TB_IS_TALEP_ID = {Convert.ToInt32(entity.GetValue("TB_IS_TALEP_ID"))}";

						await cnn.ExecuteAsync(query);

					}
					else return Json(new { has_error = true, status_code = 400, status = "Missing coming data." });

				}
				return Json(new { has_error = false, status_code = 200, status = "Entity has updated successfully." });
			}
			catch (Exception e)
			{

				return Json(new { has_error = true, status_code = 500, status = e.Message });
			}

		}


		
		[Route("api/GetIsTalepById")]
		[HttpGet]
		public object GetIsTalepById([FromUri] int isTalepId)
		{
			Util klas = new Util();
			List<IsTalep> listem = new List<IsTalep>();
			string query = @"select * from orjin.VW_IS_TALEP where TB_IS_TALEP_ID = @TB_IS_TALEP_ID";
			using (var conn = klas.baglan())
			{
				listem = conn.Query<IsTalep>(query, new { @TB_IS_TALEP_ID = isTalepId }).ToList();
			}
			return listem;
		}


		// Ek olarak iptal nedeni , tarih ve saati eklendi .

		[Route("api/IsTalepIptalEt")]
		[HttpGet]
		public void IsTalepIptalEt( 
			[FromUri] int talepID, 
			[FromUri] int userId, 
			[FromUri] string talepNo, 
			[FromUri] string userName,
			[FromUri] string iptalNeden, 
			[FromUri] DateTime? iptalTarih, 
			[FromUri] string iptalSaat )
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
					parametreler.Add("IST_IPTAL_NEDEN", iptalNeden);
					parametreler.Add("IST_IPTAL_TARIH", iptalTarih);
					parametreler.Add("IST_IPTAL_SAAT", iptalSaat);
					// Log data is recorded
					cnn.Execute(isTalepLogQuery, parametreler);
					// Job request status is being canceled
					cnn.Execute($"update orjin.TB_IS_TALEBI set IST_DURUM_ID = 5 WHERE TB_IS_TALEP_ID = {talepID}", parametreler);

					// Job cancellation reason & date & time
					cnn.Execute("update orjin.TB_IS_TALEBI set IST_IPTAL_NEDEN = @IST_IPTAL_NEDEN , " +
						"IST_IPTAL_TARIH = @IST_IPTAL_TARIH , " +
						"IST_IPTAL_SAAT = @IST_IPTAL_SAAT WHERE TB_IS_TALEP_ID = @IS_TALEP_ID", parametreler);
					
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

	}
}
