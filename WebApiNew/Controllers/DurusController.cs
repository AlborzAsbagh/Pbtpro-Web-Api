using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dapper;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    [MyBasicAuthenticationFilter]
    public class DurusController : ApiController
    {
		Util klas = new Util();
		string query = "";
		SqlCommand cmd = null;

		[Route("api/DurusListFiltered")]
        [HttpPost]
        public List<IsEmriDurus> DurusList([FromUri] int kllId, [FromUri] int page, [FromUri] int pageSize, [FromBody] Filtre filtre)
        {
            var util = new Util();
            var prms = new DynamicParameters();
            #region filtre

            var filtreQuery = "";
            if (filtre != null)
            {
                if (filtre.MakineID > 0)
                {
                    prms.Add("MKN_ID", filtre.MakineID);
                    filtreQuery += " AND MKD.MKD_MAKINE_ID = @MKN_ID";
                }
                if (filtre.LokasyonID > 0)
                {
                    prms.Add("LOK_ID", filtre.LokasyonID);
                    filtreQuery += " AND MKD.MKD_LOKASYON_ID = @LOK_ID";
                }
                if (filtre.ProjeID > 0)
                {
                    prms.Add("PRJ_ID", filtre.ProjeID);
                    filtreQuery += " AND MKD.MKD_PROJE_ID = @PRJ_ID";
                }
                if (filtre.nedenID > 0)
                {
                    prms.Add("NDN_ID", filtre.nedenID);
                    filtreQuery += " AND MKD.MKD_NEDEN_KOD_ID = @NDN_ID";
                }

                if (filtre.Kelime != "")
                {
                    prms.Add("KELIME", filtre.Kelime);
                    filtreQuery += @" AND  (L.LOK_TANIM   like '%'+@KELIME+'%' 
                                         OR M.MKN_KOD     like '%'+@KELIME+'%' 
                                         OR M.MKN_TANIM   like '%'+@KELIME+'%' 
                                         OR P.PRJ_KOD     like '%'+@KELIME+'%' 
                                         OR P.PRJ_TANIM   like '%'+@KELIME+'%'  
                                         OR MKD_ACIKLAMA  like '%'+@KELIME+'%'
                                         OR MKD_NEDEN     like '%'+@KELIME+'%'
                                            ) ";
                }

                if (filtre.BasTarih != "" && filtre.BitTarih != "")
                {
                    DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                    DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                    prms.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
                    prms.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
                    filtreQuery += " AND MKD_OLUSTURMA_TARIH BETWEEN  @BAS_TARIH and @BIT_TARIH";
                }
                else if (filtre.BasTarih != "")
                {
                    DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                    prms.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
                    filtreQuery += " AND MKD_OLUSTURMA_TARIH >=  @BAS_TARIH ";
                }
                else if (filtre.BitTarih != "")
                {
                    DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                    prms.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
                    filtreQuery += " AND MKD_OLUSTURMA_TARIH <= @BIT_TARIH ";
                }
            }

            #endregion
            prms.Add("KLL_ID", kllId);
            prms.Add("PAGE", page);
            prms.Add("PAGE_SIZE", pageSize);
            string sql = @"SELECT * FROM (SELECT 
                                                MKD.*
                                                ,L.TB_LOKASYON_ID
                                                ,L.LOK_TANIM
                                                ,M.TB_MAKINE_ID
                                                ,M.MKN_KOD
                                                ,M.MKN_TANIM
                                                ,M.MKN_LOKASYON_ID
                                                ,P.TB_PROJE_ID
                                                ,P.PRJ_KOD
                                                ,P.PRJ_TANIM
                                                ,ROW_NUMBER() OVER (ORDER BY MKD.MKD_BASLAMA_TARIH DESC, MKD_BASLAMA_SAAT DESC) ROW_NUM
                                                FROM orjin.VW_MAKINE_DURUS MKD
                                                LEFT JOIN orjin.TB_LOKASYON L ON L.TB_LOKASYON_ID=MKD.MKD_LOKASYON_ID
                                                LEFT JOIN orjin.TB_MAKINE M ON M.TB_MAKINE_ID=MKD.MKD_MAKINE_ID
                                                LEFT JOIN orjin.TB_PROJE P ON P.TB_PROJE_ID=MKD.MKD_PROJE_ID
                                                 WHERE  orjin.UDF_LOKASYON_YETKI_KONTROL(MKD.MKD_LOKASYON_ID , @KLL_ID) = 1 " + filtreQuery;

                  sql += @"
                                            ) MTABLE WHERE ROW_NUM BETWEEN @PAGE*@PAGE_SIZE+1 AND @PAGE*@PAGE_SIZE+@PAGE_SIZE";
            using (var cnn = util.baglan())
            {
                List<IsEmriDurus> listem = cnn.Query<IsEmriDurus, Lokasyon, Makine, Proje, IsEmriDurus>(sql, map: (i, l, m, p) =>
                {
                    i.MKD_NEDEN = i.MKD_NEDEN ?? "";
                    i.MKD_ACIKLAMA = Util.RemoveRtfFormatting(i.MKD_ACIKLAMA);
                    i.MKD_MAKINE = m;
                    i.MKD_LOKASYON = l;
                    i.MKD_PROJE = p;
                    return i;
                }, splitOn: "TB_LOKASYON_ID,TB_MAKINE_ID,TB_PROJE_ID", param: prms).ToList();
                return listem;
            }
        }


        [Route("api/GetDurusNedenleri")]
        [HttpGet]
        public Object GetDurusNedenleri()
        {
			string query = @"SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP=32300";
			var klas = new Util();
			List<Kod> listem = new List<Kod>();
			using (var cnn = klas.baglan())
			{
				listem = cnn.Query<Kod>(query).ToList();
			}

			return Json(new { durus_nedenleri = listem });
		}

		//Add Durus Nedeni
		[Route("api/AddDurusNedeni")]
		[HttpGet]
		public Object AddDurusNedeni([FromUri] string durusNedeni)
		{
			try
			{
				query = " insert into orjin.TB_KOD (KOD_GRUP , KOD_TANIM , KOD_AKTIF , KOD_GOR , KOD_DEGISTIR , KOD_SIL ) ";
				query += $" values ( 32300 , '{durusNedeni}' , 1 , 1 , 1 ,1 ) ";

				using (var con = klas.baglan())
				{
					cmd = new SqlCommand(query, con);
					cmd.ExecuteNonQuery();
				}
				klas.kapat();
				return Json(new { success = "Ekleme başarılı " });
			}
			catch (Exception e)
			{
				klas.kapat();
				return Json(new { error = " Ekleme başarısız " });
			}
		}
	}
}
