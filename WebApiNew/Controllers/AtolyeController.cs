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

namespace WebApiNew.Controllers
{

	[MyBasicAuthenticationFilter]
	public class AtolyeController : ApiController
	{
		List<Prm> parametreler = new List<Prm>();
		Parametreler prms = new Parametreler();
		Util klas = new Util();
		SqlCommand cmd = null;
		string query = "";

		[Route("api/GetAtolyeList")]
		[HttpGet]
		public List<Atolye> GetAtolyeList(int kulID)
		{
			query =
			  $"select * from orjin.TB_ATOLYE where orjin.UDF_ATOLYE_YETKI_KONTROL(TB_ATOLYE_ID, {kulID}) = 1";
			List<Atolye> listem = new List<Atolye>();
			try
			{
				using (var cnn = klas.baglan())
				{
					listem = cnn.Query<Atolye>(query).ToList();
				}
				return listem;
			}
			catch (Exception ex)
			{
				return listem;
			}
		}


		[Route("api/AddAtolye")]
		[HttpPost]
		public async Task<object> AddAtolye([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0)
					{
						query = " insert into orjin.TB_ATOLYE  ( ATL_OLUSTURMA_TARIH , ";
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


		[Route("api/UpdateAtolye")]
		[HttpPost]
		public async Task<Object> UpdateAtolye([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_ATOLYE_ID")) >= 1)
					{
						query = " update orjin.TB_ATOLYE set ";
						foreach (var item in entity)
						{

							if (item.Key.Equals("TB_ATOLYE_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , ATL_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ";
						query += $" where TB_ATOLYE_ID = {Convert.ToInt32(entity.GetValue("TB_ATOLYE_ID"))}";

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