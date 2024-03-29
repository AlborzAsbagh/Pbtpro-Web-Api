using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using Newtonsoft.Json.Linq;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
	[JwtAuthenticationFilter]
	public class LokasyonController : ApiController
	{
		Util klas = new Util();
		YetkiController yetki = new YetkiController();
		string query = "";
		public List<Lokasyon> Get()
		{
			Util klas = new Util();
			List<Lokasyon> listem = new List<Lokasyon>();
			string query = @"select * from orjin.TB_LOKASYON where orjin.UDF_LOKASYON_YETKI_KONTROL(TB_LOKASYON_ID,@KUL_ID) = 1";
			using (var conn = klas.baglan())
			{
				listem = conn.Query<Lokasyon>(query, new { @KUL_ID = UserInfo.USER_ID }).ToList();
			}
			return listem;
		}

		[Route("api/getLokasyonById")]
		public List<string> getLokasyonById([FromUri] int ID)
		{
			Util klas = new Util();
			List<string> listem = new List<string>();
			string query = @"select LOK_TANIM from orjin.TB_LOKASYON where TB_LOKASYON_ID = @LOKASYON_ID";
			using (var conn = klas.baglan())
			{
				listem = conn.Query<string>(query, new { @LOKASYON_ID = ID }).ToList();
			}
			return listem;
		}

		[Route("api/getIsTalebiKod")]
		public List<string> getIsTalebiKod(string nmrkod)
		{
			Util klas = new Util();
			List<string> listem = new List<string>();
			string query = @"SELECT NMR_NUMARA FROM orjin.TB_NUMARATOR where NMR_KOD = @NMR_KOD";
			using (var conn = klas.baglan())
			{
				listem = conn.Query<string>(query, new { @NMR_KOD = nmrkod }).ToList();
			}
			return listem;
		}

		
		[Route("api/getLokasyonlar")]
		[HttpGet] 
		public List<string> getLokasyonlar()
		{
			Util klas = new Util();
			List<string> listem = new List<string>();
			string query = @"select LOK_TANIM from orjin.TB_LOKASYON";
			using (var conn = klas.baglan())
			{
				listem = conn.Query<string>(query).ToList();
			}
			return listem;
		}

		[Route("api/getLokasyonListPageView")]
		[HttpGet]
		public List<Lokasyon> GetLokasyonListPageView([FromUri] int ID, [FromUri] int anaLokasyonId , [FromUri] string searchText)
		{
			Util klas = new Util();
			string query = @"SELECT 
							TB_LOKASYON.*,
							CASE 
								WHEN EXISTS (
									SELECT 1 
									FROM orjin.TB_LOKASYON AS sub 
									WHERE sub.LOK_ANA_LOKASYON_ID = TB_LOKASYON.TB_LOKASYON_ID
								) THEN 1
								ELSE 0
							END AS LOK_HAS_NEXT
						FROM 
							orjin.TB_LOKASYON 
						WHERE 
							orjin.UDF_LOKASYON_YETKI_KONTROL( TB_LOKASYON_ID , @KUL_ID ) = 1 ";

			List<Lokasyon> listem = new List<Lokasyon>();
			if(anaLokasyonId > -1 && (searchText == "" || searchText == null))
			{
				query += $" and LOK_ANA_LOKASYON_ID = {anaLokasyonId} ";
			}
			else
			{
				query += $" and ( LOK_TANIM like '%{searchText}%' ) ";

			}
			using (var conn = klas.baglan())
			{
				listem = conn.Query<Lokasyon>(query , new { @KUL_ID = ID }).ToList();
			}
			return listem;
		}

		// Lokasyon Ekle Web App
		[Route("api/addLokasyon")]
		[HttpPost]
		public async Task<object> AddLokasyon([FromBody] JObject entity)
		{
			if (!(Boolean)yetki.isAuthorizedToAdd(PagesAuthCodes.LOKASYON_TANIMLARI))
				return Json(new { has_error = true, status_code = 401, status = "Unathorized to add !" });

			int count = 0;
			try
			{
				using(var cnn = klas.baglan())
				{
					if(entity != null && entity.Count > 0)
					{
						query = " insert into orjin.TB_LOKASYON  ( LOK_OLUSTURMA_TARIH , LOK_OLUSTURAN_ID , ";
						foreach(var item in entity)
						{
							if (count < entity.Count - 1) query += $" {item.Key} , ";
							else query += $" {item.Key} ";
							count++;
						}

						query += $" ) values ( '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , {UserInfo.USER_ID} , ";
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
			catch(Exception ex)
			{
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}

		// Lokasyon Guncelle Web App 
		[Route("api/UpdateLokasyon")]
		[HttpPost]
		public async Task<Object> LokasyonGuncelle([FromBody] JObject entity)
		{
			if (!(Boolean)yetki.isAuthorizedToUpdate(PagesAuthCodes.LOKASYON_TANIMLARI))
				return Json(new { has_error = true, status_code = 401, status = "Unathorized to update !" });

			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_LOKASYON_ID")) >= 1)
					{
						query = " update orjin.TB_LOKASYON set ";
						foreach (var item in entity)
						{
							
							if (item.Key.Equals("TB_LOKASYON_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , LOK_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , LOK_DEGISTIREN_ID = {UserInfo.USER_ID} ";
						query += $" where TB_LOKASYON_ID = {Convert.ToInt32(entity.GetValue("TB_LOKASYON_ID"))}";

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

		//Get Lokasyon List For Web App ( Lokasyon Sayfasi Icin )
		[Route("api/GetLokasyonList")]
		[HttpGet]
		public object GetLokasyonList()
		{
			Util klas = new Util();
			List<LokasyonWebAppModel> listem = new List<LokasyonWebAppModel>();
			string query = @"select * from orjin.VW_LOKASYON where orjin.UDF_LOKASYON_YETKI_KONTROL(TB_LOKASYON_ID,@KUL_ID) = 1";
			using (var conn = klas.baglan())
			{
				listem = conn.Query<LokasyonWebAppModel>(query, new { @KUL_ID = UserInfo.USER_ID }).ToList();
			}
			return listem;
		}

		[Route("api/GetLokasyonTipleri")]
		[HttpGet]
		public object GetLokasyonTipleri()
		{
			Util klas = new Util();
			List<LokasyonTip> listem = new List<LokasyonTip>();
			string query = @"select * from orjin.TB_LOKASYON_TIP";
			using (var conn = klas.baglan())
			{
				listem = conn.Query<LokasyonTip>(query).ToList();
			}
			return listem;
		}


		// Lokasyon Tip Ekle Web App
		[Route("api/AddLokasyonTip")]
		[HttpPost]
		public async Task<object> AddLokasyonTip([FromBody] JObject entity)
		{
			if (!(Boolean)yetki.isAuthorizedToAdd(PagesAuthCodes.LOKASYON_TANIMLARI) || 
				!(Boolean)yetki.isAuthorizedToUpdate(PagesAuthCodes.LOKASYON_TANIMLARI))

				return Json(new { has_error = true, status_code = 401, status = "Unathorized to update or add !" });

			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0)
					{
						query = " insert into orjin.TB_LOKASYON_TIP  ( LOT_OLUSTURMA_TARIH , LOT_OLUSTURAN_ID , ";
						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" {item.Key} , ";
							else query += $" {item.Key} ";
							count++;
						}

						query += $" ) values ( '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , {UserInfo.USER_ID} ,";
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


		// Lokasyon Guncelle Web App 
		[Route("api/UpdateLokasyonTip")]
		[HttpPost]
		public async Task<Object> UpdateLokasyonTip([FromBody] JObject entity)
		{
			if (!(Boolean)yetki.isAuthorizedToAdd(PagesAuthCodes.LOKASYON_TANIMLARI) ||
				!(Boolean)yetki.isAuthorizedToUpdate(PagesAuthCodes.LOKASYON_TANIMLARI))

				return Json(new { has_error = true, status_code = 401, status = "Unathorized to update or add !" });

			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_LOKASYON_TIP_ID")) >= 1)
					{
						query = " update orjin.TB_LOKASYON_TIP set ";
						foreach (var item in entity)
						{

							if (item.Key.Equals("TB_LOKASYON_TIP_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , LOT_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , LOT_DEGISTIREN_ID = {UserInfo.USER_ID} ";
						query += $" where TB_LOKASYON_TIP_ID = {Convert.ToInt32(entity.GetValue("TB_LOKASYON_TIP_ID"))}";

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
	}
}
