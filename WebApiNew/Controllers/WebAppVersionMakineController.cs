using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using Dapper;
using Newtonsoft.Json.Linq;
using WebApiNew.Filters;
using WebApiNew.Models;


/*
 * 
 * 
 * 
 *      MAKINE Controller For Web App Versions
 *
 *
 *
 */


namespace WebApiNew.Controllers
{

	[MyBasicAuthenticationFilter]
	public class WebAppVersionMakineController : ApiController
	{
		Util klas = new Util();
		string query = "";
		SqlCommand cmd = null;


		//Get Makine Full List For Web App Version
		[Route("api/GetMakineFullList")]
		[HttpPost]
		public object GetMakineFullList([FromUri] int? lokasyonId, [FromUri] string parametre , [FromBody] JObject filters , [FromUri] int pagingDeger = 1) 
		{
			int pagingIlkDeger = pagingDeger == 1 ? 1 : ((pagingDeger * 10) - 10);
			int pagingSonDeger = pagingIlkDeger == 1 ? 10 : ((pagingDeger * 10));
			int toplamMakineSayisi = 0;
			int counter = 0;
			string toplamMakineSayisiQuery = ""; 

			List<WebVersionMakineModel> listem = new List<WebVersionMakineModel>();
			try
			{
				query = @" SELECT * FROM ( SELECT * , ROW_NUMBER() OVER (ORDER BY TB_MAKINE_ID DESC) AS subRow FROM dbo.VW_WEB_VERSION_MAKINE where 1=1";
				toplamMakineSayisiQuery = @"select count(*) from (select * from dbo.VW_WEB_VERSION_MAKINE where 1=1" ;

				if (!string.IsNullOrEmpty(parametre))
				{
					query += $" and ( MKN_KOD like '%{parametre}%' or "; toplamMakineSayisiQuery += $" and ( MKN_KOD like '%{parametre}%' or ";
					query += $" MKN_TANIM like '%{parametre}%' or "; toplamMakineSayisiQuery += $" MKN_TANIM like '%{parametre}%' or ";
					query += $" MAKINE_LOKASYON like '%{parametre}%' or "; toplamMakineSayisiQuery += $" MAKINE_LOKASYON like '%{parametre}%' or ";
					query += $" MAKINE_TIP like '%{parametre}%' or "; toplamMakineSayisiQuery += $" MAKINE_TIP like '%{parametre}%' or ";
					query += $" MAKINE_KATEGORI like '%{parametre}%' or "; toplamMakineSayisiQuery += $" MAKINE_KATEGORI like '%{parametre}%' or ";
					query += $" MAKINE_MARKA like '%{parametre}%' or "; toplamMakineSayisiQuery += $" MAKINE_MARKA like '%{parametre}%' or ";
					query += $" MAKINE_MODEL like '%{parametre}%' or "; toplamMakineSayisiQuery += $" MAKINE_MODEL like '%{parametre}%' or ";
					query += $" MAKINE_SERI_NO like '%{parametre}%' ) "; toplamMakineSayisiQuery += $" MAKINE_SERI_NO like '%{parametre}%' ) ";
				}
				if((filters["customfilters"] as JObject) != null && (filters["customfilters"] as JObject).Count > 0)
				{
					query += " and ( ";
					toplamMakineSayisiQuery += " and ( ";
					counter = 0;
					foreach (var property in filters["customfilters"] as JObject) 
					{
						query += $" {property.Key} LIKE '%{property.Value}%' ";
						toplamMakineSayisiQuery += $" {property.Key} LIKE '%{property.Value}%' ";
						if(counter < (filters["customfilters"] as JObject).Count - 1)
						{
							query += " and ";
							toplamMakineSayisiQuery += " and ";
						}
						counter++;
					}
					query += " ) ";
					toplamMakineSayisiQuery += " ) ";
				}
				if (lokasyonId > 0 && lokasyonId != null)
				{
					query += $" and MAKINE_LOKASYON_ID = {lokasyonId} ";
					toplamMakineSayisiQuery += $" and MAKINE_LOKASYON_ID = {lokasyonId} ";
				}
				query+= $" ) RowIndex WHERE RowIndex.subRow >= {pagingIlkDeger} AND RowIndex.subRow < {pagingSonDeger}";
				toplamMakineSayisiQuery += ") as TotalMakineSayisi";

				using (var cnn = klas.baglan())
				{
					listem = cnn.Query<WebVersionMakineModel>(query).ToList();
					cmd = new SqlCommand(toplamMakineSayisiQuery, cnn);
					toplamMakineSayisi = (int) cmd.ExecuteScalar();
				}
				klas.kapat();
				return Json(new {page = (int)Math.Ceiling((decimal)toplamMakineSayisi/10) ,makine_listesi = listem});

			} catch(Exception e)
			{
				klas.kapat();
				return Json(new { error = e.Message });
			}
		}

	

		//Get Makine Durum For Web App Version
		[Route("api/GetMakineDurum")]
		[HttpGet]
		public Object GetMakineDurum()
		{
			List<Kod> listem = new List<Kod>();
			query = $"SELECT * FROM [orjin].[TB_KOD] where KOD_GRUP = 32505";
			using (var con = klas.baglan())
			{
				listem = con.Query<Kod>(query).ToList();
			}
			klas.kapat();
			return Json(new { MAKINE_DURUM = listem });
		}


		//Add Makine Durum Web App Version
		[Route ("api/AddMakineDurum")]
		[HttpPost]
		public Object AddMakineDurum([FromUri] string yeniDurum) 
		{
			try
			{
				query = " insert into orjin.TB_KOD (KOD_GRUP , KOD_TANIM , KOD_AKTIF , KOD_GOR , KOD_DEGISTIR , KOD_SIL ) " ;
				query += $" values ( 32505 , '{yeniDurum}' , 1 , 1 , 1 ,1) ";

				using(var con = klas.baglan())
				{
					cmd = new SqlCommand(query, con);
					cmd.ExecuteNonQuery();
				}
				klas.kapat();
				return Json(new  { success = "Ekleme başarılı " });
			} catch (Exception e)
			{
				klas.kapat();
				return Json(new { error = " Ekleme başarısız " });
			}
		}


		
	}
}
