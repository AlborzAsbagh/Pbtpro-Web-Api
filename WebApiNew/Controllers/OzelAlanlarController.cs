using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using Newtonsoft.Json.Linq;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{

	[MyBasicAuthenticationFilter]
	public class OzelAlanlarController : ApiController
	{
		List<Prm> parametreler = new List<Prm>();
		Parametreler prms = new Parametreler();
		Util klas = new Util();
		SqlCommand cmd = null;
		string query = "";

		[Route("api/OzelAlan")]
		[HttpGet]
		public OzelAlan Ozelalan(string form)
		{
			string query = @"SELECT * FROM orjin.TB_OZEL_ALAN WHERE OZL_FORM = @FORM";
			var prms = new { @FORM = form };
			try
			{
				var util = new Util();
				using (var conn = util.baglan())
				{
					var ozelAlan = conn.QueryFirst<OzelAlan>(query, prms);
					return ozelAlan;
				}
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		[Route("api/OzelAlanTopicGuncelle")]
		[HttpPost]
		public async Task<object> OzelAlanTopicGuncelle([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && 
						( Convert.ToString(entity.GetValue("OZL_FORM")) != "" && entity.GetValue("OZL_FORM") != null))
					{
						query = " update orjin.TB_OZEL_ALAN set ";
						foreach (var item in entity)
						{

							if (item.Key.Equals("OZL_FORM")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , OZL_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ";
						query += $" where OZL_FORM = '{Convert.ToString(entity.GetValue("OZL_FORM"))}'";

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