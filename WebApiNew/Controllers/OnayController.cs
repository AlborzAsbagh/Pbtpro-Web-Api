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

            string query = " SELECT * FROM orjin.VW_STOK_FIS STF " +
               " LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID " +
               "  LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID = SAO.SOL_PERSONEL_ID  " +
               " WHERE SFS_ISLEM_TIP = '09' " +
                " AND SFS_MODUL_NO = 1 " +
                $" AND SOL_PERSONEL_ID = {userId} " +
                 " and SFS_TALEP_DURUM_ID = 7 AND ((SOL_ONAY_DURUM_ID != 8 AND SOL_ONAY_DURUM_ID != 9) or SOL_ONAY_DURUM_ID is null)";

            int fisCount = 0;
            using (var conn = util.baglan())
            {
                using (var command = new SqlCommand(query, util.baglanCmd()))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["STO_SIRA_NO"] == DBNull.Value || Convert.ToInt32(reader["STO_SIRA_NO"]) == 1 || Convert.ToInt32(reader["STO_SIRA_NO"]) == 0)
                            {
                                fisCount++;
                            }
                            else
                            {
                                prms.Clear();
                                prms.Add("STOK_FIS_ID", Convert.ToInt32(reader["TB_STOK_FIS_ID"]));
                                int isStokFis=0;
                                int stokFisSayisi = 0;
                                if (Convert.ToInt32(reader["STO_SIRA_NO"]) == 2)
                                {

                                    isStokFis = Convert.ToInt32(klas.GetDataCell(
                                       @" select count(*) FROM orjin.VW_STOK_FIS STF 
                                        LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID 
                                        LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID 
                                        where SFS_ISLEM_TIP = '09'
                                        AND SFS_MODUL_NO = 1 
                                        AND TB_STOK_FIS_ID = @STOK_FIS_ID 
                                        AND STO_SIRA_NO = 1", prms.PARAMS));

                                    if (isStokFis > 0)
                                    {
                                        stokFisSayisi = Convert.ToInt32(klas.GetDataCell(
                                          @" select count(*) FROM orjin.VW_STOK_FIS STF 
                                            LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID 
                                            LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID 
                                            where SFS_ISLEM_TIP = '09'
                                            AND SFS_MODUL_NO = 1 
                                            AND TB_STOK_FIS_ID = @STOK_FIS_ID 
                                            AND (SOL_ONAY_DURUM_ID = 8) 
                                            AND STO_SIRA_NO = 1", prms.PARAMS));
                                    }
                                    else
                                    {
                                        fisCount++;
                                    }
                                    if (stokFisSayisi > 0)
                                    {
                                        fisCount++;
                                    }

                                }
                                else if(Convert.ToInt32(reader["STO_SIRA_NO"]) == 3)
                                {
                                    int approvedFirstOrder = 0;
                                    int firstOrder = 0;
                                    int approvedSecondOrder = 0;
                                    int secondOrder = 0;

                                    isStokFis = Convert.ToInt32(klas.GetDataCell(
                                   @" select count(*) FROM orjin.VW_STOK_FIS STF 
                                        LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID 
                                        LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID 
                                        where SFS_ISLEM_TIP = '09'
                                        AND SFS_MODUL_NO = 1 
                                        AND TB_STOK_FIS_ID = @STOK_FIS_ID 
                                        AND STO_SIRA_NO in (1,2)", prms.PARAMS));

                                    if (isStokFis > 0)
                                    {
                                        firstOrder = Convert.ToInt32(klas.GetDataCell(@" select count(*) FROM orjin.VW_STOK_FIS STF 
                                        LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID 
                                        LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID 
                                        where SFS_ISLEM_TIP = '09'
                                        AND SFS_MODUL_NO = 1 
                                        AND TB_STOK_FIS_ID = @STOK_FIS_ID 
                                        AND STO_SIRA_NO = 1", prms.PARAMS));

                                        approvedFirstOrder = Convert.ToInt32(klas.GetDataCell(@" select count(*) FROM orjin.VW_STOK_FIS STF 
                                        LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID 
                                        LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID 
                                        where SFS_ISLEM_TIP = '09'
                                        AND SFS_MODUL_NO = 1 
                                        AND TB_STOK_FIS_ID = @STOK_FIS_ID 
                                        AND (SOL_ONAY_DURUM_ID = 8) 
                                        AND STO_SIRA_NO = 1", prms.PARAMS));

                                        secondOrder = Convert.ToInt32(klas.GetDataCell(@" select count(*) FROM orjin.VW_STOK_FIS STF 
                                        LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID 
                                        LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID 
                                        where SFS_ISLEM_TIP = '09'
                                        AND SFS_MODUL_NO = 1 
                                        AND TB_STOK_FIS_ID = @STOK_FIS_ID 
                                        AND STO_SIRA_NO = 2", prms.PARAMS));

                                        approvedSecondOrder = Convert.ToInt32(klas.GetDataCell(@" select count(*) FROM orjin.VW_STOK_FIS STF 
                                        LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID 
                                        LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID 
                                        where SFS_ISLEM_TIP = '09'
                                        AND SFS_MODUL_NO = 1 
                                        AND TB_STOK_FIS_ID = @STOK_FIS_ID 
                                        AND (SOL_ONAY_DURUM_ID = 8) 
                                        AND STO_SIRA_NO = 2", prms.PARAMS));
                                    }
                                    else
                                    {
                                        fisCount++;
                                    }
                                    if(firstOrder > 0)
                                    {
                                        if(approvedFirstOrder > 0)
                                        {
                                            if(secondOrder > 0)
                                            {
                                                if(approvedSecondOrder > 0)
                                                {
                                                    fisCount++;
                                                }
                                            }
                                            else
                                            {
                                                fisCount++;
                                            }
                                        }
                                        
                                    } 
                                    else if(secondOrder > 0)
                                    {
                                        if (approvedSecondOrder > 0) fisCount++;
                                    }
                                }
                            }
                        }
                    }

                    return conn.QueryFirstOrDefault<OnayCounts>(@" SELECT 
 (SELECT COUNT(*) FROM orjin.TB_MAKINE_LOKASYON WHERE (coalesce(MKL_HEDEF_LOKASYON_ID,0) in (-1,0) OR orjin.UDF_LOKASYON_YETKI_KONTROL(MKL_HEDEF_LOKASYON_ID,@KLL_ID) = 1   OR MKL_OLUSTURAN_ID = @KLL_ID)  AND MKL_DURUM_ID = 1) AS MKN_TRANSFER_ONAY , " +

$" ( SELECT {fisCount} ) AS MLZ_TRANSFER_ONAY , " +

@"(SELECT COUNT(*) FROM orjin.TB_STOK_FIS WHERE SFS_GC = 'T' AND orjin.UDF_LOKASYON_YETKI_KONTROL(SFS_LOKASYON_ID, @KLL_ID) = 1  AND SFS_DURUM_ID = 1 AND SFS_MODUL_NO = 2	  ) AS YKT_TRANSFER_ONAY", new { KLL_ID = userId });
                }

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
