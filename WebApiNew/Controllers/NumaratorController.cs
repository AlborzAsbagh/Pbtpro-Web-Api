using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using Dapper;
using WebApiNew.Filters;

namespace WebApiNew.Controllers
{
	/* 
	 * 
	 * 
	 * For All Modules In Web App Version 
	 *
	 *
	 */


	[JwtAuthenticationFilter]
	public class NumaratorController : ApiController
	{
		Util klas = new Util();
		Parametreler prms = new Parametreler();
		string query = "";
		SqlCommand cmd = null;

		[Route("api/ModulKoduGetir")]
		[HttpGet]
		public String ModulKoduGetir([FromUri]string modulKodu)
		{
			try
			{
				var util = new Util();
				using (var conn = util.baglan())
				{
					var sql = $"  UPDATE orjin.TB_NUMARATOR SET NMR_NUMARA = NMR_NUMARA+1 WHERE NMR_KOD = '{modulKodu}' " + 
                        "SELECT NMR_ON_EK+right(replicate('0',NMR_HANE_SAYISI)+CAST(NMR_NUMARA AS VARCHAR(MAX)),NMR_HANE_SAYISI) as deger FROM orjin.TB_NUMARATOR " +
                        $" WHERE NMR_KOD = '{modulKodu}'";
					var kod = conn.Query<String>(sql).FirstOrDefault();
					return kod;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
