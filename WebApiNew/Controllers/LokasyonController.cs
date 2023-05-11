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
                listem=conn.Query<Lokasyon>(query,new {@KUL_ID=ID }).ToList();
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




	}
}
