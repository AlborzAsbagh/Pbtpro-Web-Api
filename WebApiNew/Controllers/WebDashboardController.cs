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
	public class WebDashboardController : ApiController
	{
		Util klas = new Util();
		string query = "";
		
		//Get Lokasyon List For Web App ( Lokasyon Sayfasi Icin )
		[Route("api/GetDashboardItems")]
		[HttpGet]
		public object GetDashboardItems([FromUri] int ID)
		{
			Util klas = new Util();
			List<WebDashboard> listem = new List<WebDashboard>();
			string query = @"
						  SELECT
						  (SELECT count(*) FROM orjin.TB_ISEMRI WHERE ISM_KAPATILDI = 0) AS ACIK_IS_EMIRLERI,
						  (SELECT count(*) FROM orjin.TB_IS_TALEBI WHERE IST_DURUM_ID != 4 AND IST_DURUM_ID != 5) AS DEVAM_EDEN_IS_TALEPLERI,
						  (SELECT count(*) FROM orjin.TB_STOK stk where stk.STK_MIKTAR < stk.STK_MIN_MIKTAR) AS DUSUK_STOKLU_MALZEMELER  ";
			using (var conn = klas.baglan())
			{
				listem = conn.Query<WebDashboard>(query, new { @KUL_ID = ID }).ToList();
			}
			return listem;
		}

	}
}
