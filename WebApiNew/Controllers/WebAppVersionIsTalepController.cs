using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using Newtonsoft.Json.Linq;
using WebApiNew.Filters;
using WebApiNew.Models;


/*
 * 
 * 
 * 
 *      IS TALEP Controller For Web App Versions
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
				query = @" SELECT * FROM ( SELECT * , ROW_NUMBER() OVER (ORDER BY TB_IS_TALEP_ID DESC) AS subRow FROM orjin.VW_IS_TALEP where 1=1";
				toplamIsTalepSayisiQuery = @"select count(*) from (select * from orjin.VW_IS_TALEP where 1=1";

				if (!string.IsNullOrEmpty(parametre))
				{
					query += $" and ( IST_KOD like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" and ( IST_KOD like '%{parametre}%' or ";
					query += $" IST_KONU like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" IST_KONU like '%{parametre}%' or ";
					query += $" IST_BILDIREN_LOKASYON like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" IST_BILDIREN_LOKASYON like '%{parametre}%' or ";
					query += $" IST_MAKINE_KOD like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" IST_MAKINE_KOD like '%{parametre}%' or ";
					query += $" IST_TALEP_EDEN_ADI like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" IST_TALEP_EDEN_ADI like '%{parametre}%' or ";
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

	}
}
