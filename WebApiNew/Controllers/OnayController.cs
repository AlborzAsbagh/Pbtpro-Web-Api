using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Windows.Forms;
using Dapper;
using Microsoft.Ajax.Utilities;
using Unity.Policy;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    public class OnayController : ApiController
    {
        [Route("api/Onay/GetOnayCounts")]
        [HttpGet]
        public OnayCounts GetOnayCounts([FromUri] int userId)
        {
            var list = new List<object>();
            Parametreler prms = new Parametreler();
            var util = new Util();
            Util klas = new Util();
            List<StokFis> listem = new List<StokFis>();
            using (var conn = util.baglan())
            {
               
             return conn.QueryFirstOrDefault<OnayCounts>(@" SELECT 
 (SELECT COUNT(*) FROM orjin.TB_MAKINE_LOKASYON WHERE (coalesce(MKL_HEDEF_LOKASYON_ID,0) in (-1,0) OR orjin.UDF_LOKASYON_YETKI_KONTROL(MKL_HEDEF_LOKASYON_ID,@KLL_ID) = 1   OR MKL_OLUSTURAN_ID = @KLL_ID)  AND MKL_DURUM_ID = 1) AS MKN_TRANSFER_ONAY , " +

@" ( SELECT COUNT(*) FROM orjin.VW_STOK_FIS STF LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID 
      WHERE SFS_ISLEM_TIP = '09' 
      AND SFS_MODUL_NO = 1 
      AND SOL_SIRA_NO = SOL_SIRA_NO 
      AND SOL_PERSONEL_ID = @KLL_ID
      AND SFS_TALEP_DURUM_ID = 7 
     AND SOL_ONAY_DURUM_ID = SOL_ONAY_DURUM_ID) AS MLZ_TRANSFER_ONAY , " +

@" (SELECT COUNT(*) FROM orjin.TB_STOK_FIS WHERE SFS_GC = 'T' AND orjin.UDF_LOKASYON_YETKI_KONTROL(SFS_LOKASYON_ID, @KLL_ID) = 1  AND SFS_DURUM_ID = 1 AND SFS_MODUL_NO = 2	  ) AS YKT_TRANSFER_ONAY", new { KLL_ID = userId });
            }

        }
    }
}


//COUNT(*) FROM orjin.VW_STOK_FIS STF
//LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID

//WHERE SFS_ISLEM_TIP = '09'

//AND SFS_MODUL_NO = 1

//AND SOL_SIRA_NO = SOL_SIRA_NO

//AND SOL_PERSONEL_ID = @KLL_ID

//AND SFS_TALEP_DURUM_ID = 7

//AND SOL_ONAY_DURUM_ID = SOL_ONAY_DURUM_ID
