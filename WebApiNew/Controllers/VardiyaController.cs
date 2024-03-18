using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.WebPages;
using Dapper;
using Newtonsoft.Json.Linq;
using WebApiNew.App_GlobalResources;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{

	[JwtAuthenticationFilter]
	public class VardiyaController : ApiController
	{
		Util klas = new Util();
		string query = "";
		YetkiController yetki = new YetkiController();

		[HttpGet]
		[Route("api/GetVardiyaList")]
		public object GetVardiyaList()
		{
			string query = @" select * , 
							  (select LOK_TANIM from orjin.TB_LOKASYON where TB_LOKASYON_ID = v.VAR_LOKASYON_ID) as VAR_LOKASYON , 
							  (select PRJ_TANIM from orjin.TB_PROJE where TB_PROJE_ID = v.VAR_PROJE_ID) as VAR_PROJE , 
							  (select KOD_TANIM from orjin.TB_KOD where TB_KOD_ID = v.VAR_VARDIYA_TIPI_KOD_ID) as VAR_VARDIYA_TIPI  
							  from orjin.TB_VARDIYA v";
			List<Vardiya> listem = new List<Vardiya>();
			try
			{
				using (var cnn = klas.baglan())
				{
					listem = cnn.Query<Vardiya>(query).ToList();
				}
				return Json(new { VARDIYA_LISTE = listem });
			}
			catch (Exception ex)
			{
				return Json(new { error = ex.Message });
			}
		}


		[HttpPost]
		[Route("api/AddVardiya")]
		public async Task<object> AddVardiya(JObject entity)
		{
			if (!(Boolean)yetki.isAuthorizedToAdd(PagesAuthCodes.VARDIYA_TANIMLARI))
				return Json(new { has_error = true, status_code = 401, status = "Unathorized to add !" });

			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0)
					{
						query = " insert into orjin.TB_VARDIYA ( VAR_OLUSTURMA_TARIH , VAR_OLUSTURAN_ID , ";
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


		[HttpPost]
		[Route("api/UpdateVardiya")]
		public async Task<object> UpdateVardiya(JObject entity)
		{
			if (!(Boolean)yetki.isAuthorizedToUpdate(PagesAuthCodes.VARDIYA_TANIMLARI))
				return Json(new { has_error = true, status_code = 401, status = "Unathorized to update !" });

			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_VARDIYA_ID")) >= 1)
					{
						query = " update orjin.TB_VARDIYA set ";
						foreach (var item in entity)
						{

							if (item.Key.Equals("TB_VARDIYA_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , VAR_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , VAR_DEGISTIREN_ID = {UserInfo.USER_ID}";
						query += $" where TB_VARDIYA_ID = {Convert.ToInt32(entity.GetValue("TB_VARDIYA_ID"))}";

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