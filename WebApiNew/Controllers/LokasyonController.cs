using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Dapper;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
	[MyBasicAuthenticationFilter]
	public class LokasyonController : ApiController
	{
		public List<Lokasyon> Get([FromUri] int ID)
		{
			Util klas = new Util();
			List<Lokasyon> listem = new List<Lokasyon>();
			string query = @"select * from orjin.TB_LOKASYON where orjin.UDF_LOKASYON_YETKI_KONTROL(TB_LOKASYON_ID,@KUL_ID) = 1";
			using (var conn = klas.baglan())
			{
				listem = conn.Query<Lokasyon>(query, new { @KUL_ID = ID }).ToList();
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

	}
}
