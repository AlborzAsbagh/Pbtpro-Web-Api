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
	[MyBasicAuthenticationFilter]
	public class IsTalepParametreController : ApiController
	{
		Util klas = new Util();
		string query = "";
		public List<IsTalepParametre> Get()
		{
			Util klas = new Util();
			List<IsTalepParametre> listem = new List<IsTalepParametre>();
			string query = @" select * from orjin.TB_IS_TALEBI_PARAMETRE ";
			using (var conn = klas.baglan())
			{
				listem = conn.Query<IsTalepParametre>(query).ToList();
			}
			return listem;
		}

		[Route("api/UpdateIsTalepParametre")]
		[HttpPost]
		public async Task<Object> UpdateIsTalepParametre([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0)
					{
						query = " update orjin.TB_IS_TALEBI_PARAMETRE set ";
						foreach (var item in entity)
						{

							if (count < entity.Count - 1) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , ISP_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ";
						query += $" where TB_IS_TALEBI_PARAMETRE_ID = 1 ";

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
