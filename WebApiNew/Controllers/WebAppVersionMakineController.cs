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
		Parametreler prms = new Parametreler();


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
				query = @" SELECT * FROM ( SELECT * , ROW_NUMBER() OVER (ORDER BY TB_MAKINE_ID DESC) AS subRow FROM orjin.VW_MAKINE where 1=1";
				toplamMakineSayisiQuery = @"select count(*) from (select * from orjin.VW_MAKINE where 1=1" ;

				if (!string.IsNullOrEmpty(parametre))
				{
					query += $" and ( MKN_KOD like '%{parametre}%' or "; toplamMakineSayisiQuery += $" and ( MKN_KOD like '%{parametre}%' or ";
					query += $" MKN_TANIM like '%{parametre}%' or "; toplamMakineSayisiQuery += $" MKN_TANIM like '%{parametre}%' or ";
					query += $" MKN_LOKASYON like '%{parametre}%' or "; toplamMakineSayisiQuery += $" MKN_LOKASYON like '%{parametre}%' or ";
					query += $" MKN_TIP like '%{parametre}%' or "; toplamMakineSayisiQuery += $" MKN_TIP like '%{parametre}%' or ";
					query += $" MKN_KATEGORI like '%{parametre}%' or "; toplamMakineSayisiQuery += $" MKN_KATEGORI like '%{parametre}%' or ";
					query += $" MKN_MARKA like '%{parametre}%' or "; toplamMakineSayisiQuery += $" MKN_MARKA like '%{parametre}%' or ";
					query += $" MKN_MODEL like '%{parametre}%' or "; toplamMakineSayisiQuery += $" MKN_MODEL like '%{parametre}%' or ";
					query += $" MKN_SERI_NO like '%{parametre}%' ) "; toplamMakineSayisiQuery += $" MKN_SERI_NO like '%{parametre}%' ) ";
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
					query += $" and MKN_LOKASYON_ID = {lokasyonId} ";
					toplamMakineSayisiQuery += $" and MKN_LOKASYON_ID = {lokasyonId} ";
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

		[Route("api/AddMakineTip")]
		[HttpPost]

		public Object AddMakineTip([FromUri] string yeniTip)
		{
			try
			{
				query = " insert into orjin.TB_KOD (KOD_GRUP , KOD_TANIM , KOD_AKTIF , KOD_GOR , KOD_DEGISTIR , KOD_SIL ) ";
				query += $" values ( 32501 , '{yeniTip}' , 1 , 1 , 1 ,1) ";

				using( var con = klas.baglan())
				{
					cmd = new SqlCommand(query, con); 
					cmd.ExecuteNonQuery();
				}
				klas.kapat();
				return Json(new { status_code = 201 , status = "Added Successfully" });

			}
			catch(Exception ex)
			{
				klas.kapat();
				return Json(new { status_code = 201, status = ex.Message });
			}
		}

		[Route("api/AddMakineKategori")]
		[HttpPost]

		public Object AddMakineKategori([FromUri] string yeniKategori)
		{
			try
			{
				query = " insert into orjin.TB_KOD (KOD_GRUP , KOD_TANIM , KOD_AKTIF , KOD_GOR , KOD_DEGISTIR , KOD_SIL ) ";
				query += $" values ( 32502 , '{yeniKategori}' , 1 , 1 , 1 ,1) ";

				using (var con = klas.baglan())
				{
					cmd = new SqlCommand(query, con);
					cmd.ExecuteNonQuery();
				}
				klas.kapat();
				return Json(new { has_error = false ,status_code = 201, status = "Added Successfully" });

			}
			catch (Exception ex)
			{
				klas.kapat();
				return Json(new { has_error = true , status_code = 500, status = ex.Message });
			}
		}

		[Route("api/GetMakineModels")]
		[HttpGet]
		public Object GetMakineModelFullList()
		{
			try
			{
				query = "select * from orjin.TB_MODEL";
				List<Model> list = new List<Model>();
				using(var con = klas.baglan())
				{
					list = con.Query<Model>(query).ToList();
					klas.kapat();
					return Json(new {Makine_Model_List = list});
				}
			}
			catch(Exception ex)
			{
				klas.kapat();
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}

		[Route("api/GetMakineModelByMarkaId")]
		[HttpGet]
		public Object GetMakineModelByMarkaId([FromUri] int markaId)
		{
			try
			{
				query = $"select * from orjin.TB_MODEL where MDL_MARKA_ID = {markaId}";
				List<Model> list = new List<Model>();
				using (var con = klas.baglan())
				{
					list = con.Query<Model>(query).ToList();
					klas.kapat();
					return Json(new { Makine_Model_List = list });
				}
			}
			catch (Exception ex)
			{
				klas.kapat();
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}

		[Route("api/GetMakineMarks")]
		[HttpGet]
		public Object GetMakineMarkaFullList()
		{
			try
			{
				query = "select * from orjin.TB_MARKA";
				List<Marka> list = new List<Marka>();
				using (var con = klas.baglan())
				{
					list = con.Query<Marka>(query).ToList();
					klas.kapat();
					return Json(new { Makine_Marka_List = list });
				}
			}
			catch (Exception ex)
			{
				klas.kapat();
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}


		[Route("api/AddMakineModel")]
		[HttpPost]
		public Object AddMakineModel([FromBody] Model entity)
		{
			try
			{
				query = @" insert into orjin.TB_MODEL 
							( MDL_MARKA_ID,	
							  MDL_MODEL,	
							  MDL_OLUSTURAN_ID,	
							  MDL_OLUSTURMA_TARIH,	
							  MDL_DEGISTIREN_ID,	
							  MDL_DEGISTIRME_TARIH ) values
							(
							  @MDL_MARKA_ID,
							  @MDL_MODEL,
							  @MDL_OLUSTURAN_ID,
							  @MDL_OLUSTURMA_TARIH,
							  @MDL_DEGISTIREN_ID,
							  @MDL_DEGISTIRME_TARIH, )
							";
				prms.Clear();
				prms.Add("@MDL_MARKA_ID",entity.MDL_MARKA_ID);
				prms.Add("@MDL_MODEL",entity.MDL_MODEL);
				prms.Add("@MDL_OLUSTURAN_ID",entity.MDL_OLUSTURAN_ID);
				prms.Add("@MDL_OLUSTURMA_TARIH",DateTime.Now);
				prms.Add("@MDL_DEGISTIREN_ID",0);
				prms.Add("@MDL_DEGISTIRME_TARIH",null);
				klas.baglan();
				klas.cmd(query, prms.PARAMS);
				klas.kapat();
				return Json(new { has_error = false, status_code = 201, status = "Added Successfully" });
			}
			catch(Exception ex)
			{
				klas.kapat();
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}


		[Route("api/AddMakineMarka")]
		[HttpPost]
		public Object AddMakineMarka([FromBody] Marka entity)
		{
			try
			{
				query = @" insert into orjin.TB_MARKA 
							( MRK_MARKA,	
							  MRK_OLUSTURAN_ID,	
							  MRK_OLUSTURMA_TARIH,	
							  MRK_DEGISTIREN_ID,	
							  MRK_DEGISTIRME_TARIH ) values
							(
							  @MRK_MARKA,
							  @MRK_OLUSTURAN_ID,
							  @MRK_OLUSTURMA_TARIH,
							  @MRK_DEGISTIREN_ID,
							  @MRK_DEGISTIRME_TARIH, )
							";
				prms.Clear();
				prms.Add("@MRK_MARKA", entity.MRK_MARKA);
				prms.Add("@MRK_OLUSTURAN_ID", entity.MRK_OLUSTURAN_ID);
				prms.Add("@MRK_OLUSTURMA_TARIH", DateTime.Now);
				prms.Add("@MRK_DEGISTIREN_ID", 0);
				prms.Add("@MRK_DEGISTIRME_TARIH", null);
				klas.baglan();
				klas.cmd(query, prms.PARAMS);
				klas.kapat();
				return Json(new { has_error = false, status_code = 201, status = "Added Successfully" });
			}
			catch (Exception ex)
			{
				klas.kapat();
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}


		[Route("api/GetMakineOperators")]
		[HttpGet]
		public Object GetMakineOperators()
		{
			try
			{
				query = @"SELECT 
						   TB_MAKINE_OPERATOR_ID
						  ,MKO_MAKINE_ID
						  ,MKO_TARIH
						  ,MKO_SAAT
						  ,MKO_KAYNAK_OPERATOR_ID
						  ,MKO_HEDEF_OPERATOR_ID
						  ,MKO_ACIKLAMA
						  ,MKO_OLUSTURAN_ID
						  ,MKO_OLUSTURMA_TARIH
						  ,MKO_DEGISTIREN_ID
						  ,MKO_DEGISTIRME_TARIH
						  ,MKO_SAYAC_BIRIMI
						  ,MKO_GUNCEL_SAYAC_DEGERI
						  , PRS_HEDEF_KOD.PRS_ISIM as MKO_HEDEF_OPERATOR_KOD
						  , PRS_KAYNAK_KOD.PRS_ISIM as MKO_KAYNAK_OPERATOR_KOD
					  FROM orjin.TB_MAKINE_OPERATOR MKO 
					  left join orjin.TB_PERSONEL PRS_HEDEF_KOD ON MKO.MKO_HEDEF_OPERATOR_ID = PRS_HEDEF_KOD.TB_PERSONEL_ID
					  left join orjin.TB_PERSONEL PRS_KAYNAK_KOD ON MKO.MKO_KAYNAK_OPERATOR_ID = PRS_KAYNAK_KOD.TB_PERSONEL_ID";
				List<MakineOperator> list = new List<MakineOperator>();
				using (var cnn = klas.baglan())
				{
					list = cnn.Query<MakineOperator>(query).ToList();
					klas.kapat();
					return Json(new { Makine_Operator_List = list });

				}

			}

			catch(Exception ex)
			{
				klas.kapat();
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}
	}
}
