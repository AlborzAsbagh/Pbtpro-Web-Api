using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dapper;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    public class OnayController : ApiController
    {
        [Route("api/Onay/GetOnayCounts")]
        [HttpGet]
        public OnayCounts GetOnayCounts([FromUri]int userId)
        {
            var list=new List<object>();
            var util=new Util();
            using (var conn = util.baglan())
            {
                return conn.QueryFirstOrDefault<OnayCounts>(@"SELECT 
(SELECT COUNT(*) FROM orjin.TB_MAKINE_LOKASYON WHERE (coalesce(MKL_HEDEF_LOKASYON_ID,0) in (-1,0) OR orjin.UDF_LOKASYON_YETKI_KONTROL(MKL_HEDEF_LOKASYON_ID,@KLL_ID) = 1   OR MKL_OLUSTURAN_ID = @KLL_ID)  AND MKL_DURUM_ID = 1) AS MKN_TRANSFER_ONAY,
(SELECT COUNT(*) FROM orjin.TB_STOK_FIS WHERE SFS_GC = 'T' AND orjin.UDF_LOKASYON_YETKI_KONTROL(SFS_LOKASYON_ID, @KLL_ID) = 1  AND SFS_DURUM_ID = 1 AND SFS_MODUL_NO = 1	  ) AS MLZ_TRANSFER_ONAY,
(SELECT COUNT(*) FROM orjin.TB_STOK_FIS WHERE SFS_GC = 'T' AND orjin.UDF_LOKASYON_YETKI_KONTROL(SFS_LOKASYON_ID, @KLL_ID) = 1  AND SFS_DURUM_ID = 1 AND SFS_MODUL_NO = 2	  ) AS YKT_TRANSFER_ONAY", new {KLL_ID=userId});
            }
            
        }
        
    }
}
