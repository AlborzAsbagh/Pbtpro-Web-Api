using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using Newtonsoft.Json;
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

		//Makine Ekle 
		[Route("api/AddMakine")]
		[HttpPost]
		public async Task<Object> Post([FromBody] JObject entity , [FromUri] int ID)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if(entity != null && entity.Count > 0)
					{
						query = " insert into orjin.TB_MAKINE ( ";
						foreach (var item in entity)
						{
							//if (item.Key.Equals("MakineSayacList") ||
							//		item.Key.Equals("MakineOtonomBakimList") ||
							//			item.Key.Equals("MakinePeriyodikBakimList")) continue;

							if (count < entity.Count-1) query += $" {item.Key} , ";
							else query += $" {item.Key} ";
							count ++;
						}

						query += " ) values ( ";
						count = 0;

						foreach (var item in entity)
						{
							//if (item.Key.Equals("MakineSayacList") || 
							//		item.Key.Equals("MakineOtonomBakimList") || 
							//			item.Key.Equals("MakinePeriyodikBakimList")) continue;

							if (count < entity.Count - 1) query += $" '{item.Value}' , ";
							else query += $" '{item.Value}' ";
							count++;
						}
						query += " ) ";
						await cnn.ExecuteAsync(query);
						//int makineId = await cnn.QueryFirstAsync<int>("SELECT MAX(TB_MAKINE_ID) FROM orjin.TB_MAKINE");


						//if (entity["MakineSayacList"] is JArray makineSayacList && makineSayacList.Count > 0)
						//{
						//	Bildirim bldr = new Bildirim();

						//	for(int i = 0; i < makineSayacList.Count; i++) 
						//	{
						//		Sayac sayac = JsonConvert.DeserializeObject<Sayac>(makineSayacList[i].ToString());
						//		bldr = YeniSayacEkle(sayac, makineId);
						//		if (!bldr.Durum) return Json(new { has_error = true, status_code = 500, status = bldr.Aciklama });
						//	}
							
						//}
						
					}
				}
				 return Json(new { has_error = false, status_code = 201, status = "Added Successfully" });
			}
			catch (Exception e) {

				return Json(new { has_error = true, status_code = 500, status = e.Message });
			}

		}


		//Makine Guncelle 
		[Route("api/UpdateMakine")]
		[HttpPost]
		public async Task<Object> MakineGuncelle([FromBody] JObject entity, [FromUri] int ID)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_MAKINE_ID")) >= 1)
					{
						query = " update orjin.TB_MAKINE set ";
						foreach (var item in entity)
						{
							//if (item.Key.Equals("MakineSayacList") ||
							//		item.Key.Equals("MakineOtonomBakimList") ||
							//			item.Key.Equals("MakinePeriyodikBakimList") || 
													if(item.Key.Equals("TB_MAKINE_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" where TB_MAKINE_ID = {Convert.ToInt32(entity.GetValue("TB_MAKINE_ID"))}";

						await cnn.ExecuteAsync(query);
						//int makineId = await cnn.QueryFirstAsync<int>("SELECT MAX(TB_MAKINE_ID) FROM orjin.TB_MAKINE");


						//if (entity["MakineSayacList"] is JArray makineSayacList && makineSayacList.Count > 0)
						//{
						//	Bildirim bldr = new Bildirim();

						//	for (int i = 0; i < makineSayacList.Count; i++)
						//	{
						//		Sayac sayac = JsonConvert.DeserializeObject<Sayac>(makineSayacList[i].ToString());
						//		bldr = YeniSayacEkle(sayac, makineId);
						//		if (!bldr.Durum) return Json(new { has_error = true, status_code = 500, status = bldr.Aciklama });
						//	}

						//}

					}
					else return Json(new { has_error = true, status_code = 400, status = "Missing coming data." });

				}
				return Json(new { has_error = false, status_code = 200, status = "Entity has updated successfully." });
			}
			catch (Exception e)
			{

				return Json(new { has_error = true, status_code = 500, status = e.Message });
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
							  @MDL_DEGISTIRME_TARIH )
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
							  @MRK_DEGISTIRME_TARIH )
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
						   [TB_MAKINE_OPERATOR_ID]
						  ,[MKO_MAKINE_ID]
						  ,[MKO_TARIH]
						  ,[MKO_SAAT]
						  ,[MKO_KAYNAK_OPERATOR_ID]
						  ,[MKO_HEDEF_OPERATOR_ID]
						  ,[MKO_ACIKLAMA]
						  ,[MKO_OLUSTURAN_ID]
						  ,[MKO_OLUSTURMA_TARIH]
						  ,[MKO_DEGISTIREN_ID]
						  ,[MKO_DEGISTIRME_TARIH]
						  ,[MKO_SAYAC_BIRIMI]
						  ,[MKO_GUNCEL_SAYAC_DEGERI]
						  ,PRS_HEDEF_KOD.PRS_ISIM as MKO_HEDEF_OPERATOR_KOD
						  ,PRS_KAYNAK_KOD.PRS_ISIM as MKO_KAYNAK_OPERATOR_KOD
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


		//Yeni Sayaca Ekle
		[Route("api/AddSayac")]
		[HttpPost]
		public Bildirim YeniSayacEkle([FromBody] Sayac entity , long makineId = 0)
		{
			Bildirim bildirim = new Bildirim();
			try
			{
				if(entity.TB_SAYAC_ID < 1)
				{
					#region Insertion
					query = @" insert into orjin.TB_SAYAC 
								(
								 MES_TANIM
								,MES_BIRIM_KOD_ID
								,MES_TIP_KOD_ID
								,MES_GUNCEL_DEGER
								,MES_VARSAYILAN
								,MES_SANAL_SAYAC
								,MES_SANAL_SAYAC_ARTIS
								,MES_SANAL_SAYAC_BASLANGIC_TARIH
								,MES_GUNCELLEME_SEKLI
								,MES_OZEL_ALAN_1
								,MES_OZEL_ALAN_2
								,MES_OZEL_ALAN_3
								,MES_OZEL_ALAN_4
								,MES_OZEL_ALAN_5
								,MES_OZEL_ALAN_6
								,MES_OZEL_ALAN_7
								,MES_OZEL_ALAN_8
								,MES_OZEL_ALAN_9
								,MES_OZEL_ALAN_10
								,MES_ACIKLAMA
								,MES_REF_ID
								,MES_REF_GRUP
								,MES_OLUSTURAN_ID
								,MES_OLUSTURMA_TARIH
								,MES_MAKINE_ID
								) 
								values 
								(  
								 @MES_TANIM
								,@MES_BIRIM_KOD_ID
								,@MES_TIP_KOD_ID
								,@MES_GUNCEL_DEGER
								,@MES_VARSAYILAN
								,@MES_SANAL_SAYAC
								,@MES_SANAL_SAYAC_ARTIS
								,@MES_SANAL_SAYAC_BASLANGIC_TARIH
								,@MES_GUNCELLEME_SEKLI
								,@MES_OZEL_ALAN_1
								,@MES_OZEL_ALAN_2
								,@MES_OZEL_ALAN_3
								,@MES_OZEL_ALAN_4
								,@MES_OZEL_ALAN_5
								,@MES_OZEL_ALAN_6
								,@MES_OZEL_ALAN_7
								,@MES_OZEL_ALAN_8
								,@MES_OZEL_ALAN_9
								,@MES_OZEL_ALAN_10
								,@MES_ACIKLAMA
								,@MES_REF_ID
								,@MES_REF_GRUP
								,@MES_OLUSTURAN_ID
								,@MES_OLUSTURMA_TARIH
								,@MES_MAKINE_ID ) ";

					prms.Clear();
					if (makineId == 0) prms.Add("@MES_MAKINE_ID", entity.MES_MAKINE_ID);
					else prms.Add("@MES_MAKINE_ID", makineId);
					prms.Add("@MES_TANIM",entity.MES_TANIM);
					prms.Add("@MES_BIRIM_KOD_ID",entity.MES_BIRIM_KOD_ID);
					prms.Add("@MES_TIP_KOD_ID",entity.MES_TIP_KOD_ID);
					prms.Add("@MES_GUNCEL_DEGER",entity.MES_GUNCEL_DEGER);
					prms.Add("@MES_VARSAYILAN",entity.MES_VARSAYILAN);
					prms.Add("@MES_SANAL_SAYAC",entity.MES_SANAL_SAYAC);
					prms.Add("@MES_SANAL_SAYAC_ARTIS",entity.MES_SANAL_SAYAC_ARTIS);
					prms.Add("@MES_SANAL_SAYAC_BASLANGIC_TARIH",entity.MES_SANAL_SAYAC_BASLANGIC_TARIH);
					prms.Add("@MES_GUNCELLEME_SEKLI",entity.MES_GUNCELLEME_SEKLI);
					prms.Add("@MES_OZEL_ALAN_1",entity.MES_OZEL_ALAN_1);
					prms.Add("@MES_OZEL_ALAN_2",entity.MES_OZEL_ALAN_2);
					prms.Add("@MES_OZEL_ALAN_3",entity.MES_OZEL_ALAN_3);
					prms.Add("@MES_OZEL_ALAN_4",entity.MES_OZEL_ALAN_4);
					prms.Add("@MES_OZEL_ALAN_5",entity.MES_OZEL_ALAN_5);
					prms.Add("@MES_OZEL_ALAN_6",entity.MES_OZEL_ALAN_6);
					prms.Add("@MES_OZEL_ALAN_7",entity.MES_OZEL_ALAN_7);
					prms.Add("@MES_OZEL_ALAN_8",entity.MES_OZEL_ALAN_8);
					prms.Add("@MES_OZEL_ALAN_9",entity.MES_OZEL_ALAN_9);
					prms.Add("@MES_OZEL_ALAN_10",entity.MES_OZEL_ALAN_10);
					prms.Add("@MES_ACIKLAMA",entity.MES_ACIKLAMA);
					prms.Add("@MES_REF_ID",entity.MES_REF_ID);
					prms.Add("@MES_REF_GRUP",entity.MES_REF_GRUP);
					prms.Add("@MES_OLUSTURAN_ID",entity.MES_OLUSTURAN_ID);
					prms.Add("@MES_OLUSTURMA_TARIH",DateTime.Now);
					klas.baglan();
					klas.cmd(query, prms.PARAMS);
					bildirim.Aciklama = "Added Successfully";
					bildirim.Durum = true;
					return bildirim;
					#endregion 
				}
				else
				{
					#region Update
					query = @" update orjin.TB_SAYAC set
								 MES_TANIM = @MES_TANIM
								,MES_BIRIM_KOD_ID = @MES_BIRIM_KOD_ID
								,MES_TIP_KOD_ID = @MES_TIP_KOD_ID
								,MES_GUNCEL_DEGER = @MES_GUNCEL_DEGER
								,MES_VARSAYILAN = @MES_VARSAYILAN
								,MES_SANAL_SAYAC = @MES_SANAL_SAYAC
								,MES_SANAL_SAYAC_ARTIS = @MES_SANAL_SAYAC_ARTIS 
								,MES_SANAL_SAYAC_BASLANGIC_TARIH = @MES_SANAL_SAYAC_BASLANGIC_TARIH
								,MES_GUNCELLEME_SEKLI = @MES_GUNCELLEME_SEKLI
								,MES_OZEL_ALAN_1 = @MES_OZEL_ALAN_1
								,MES_OZEL_ALAN_2 = @MES_OZEL_ALAN_2
								,MES_OZEL_ALAN_3 = @MES_OZEL_ALAN_3
								,MES_OZEL_ALAN_4 = @MES_OZEL_ALAN_4
								,MES_OZEL_ALAN_5 = @MES_OZEL_ALAN_5
								,MES_OZEL_ALAN_6 = @MES_OZEL_ALAN_6
								,MES_OZEL_ALAN_7 = @MES_OZEL_ALAN_7
								,MES_OZEL_ALAN_8 = @MES_OZEL_ALAN_8
								,MES_OZEL_ALAN_9 = @MES_OZEL_ALAN_9
								,MES_OZEL_ALAN_10 = @MES_OZEL_ALAN_10
								,MES_ACIKLAMA = @MES_ACIKLAMA
								,MES_REF_ID = @MES_REF_ID
								,MES_REF_GRUP = @MES_REF_GRUP
								,MES_DEGISTIREN_ID = @MES_DEGISTIREN_ID
								,MES_DEGISTIRME_TARIH = @MES_DEGISTIRME_TARIH
								,MES_MAKINE_ID  = @MES_MAKINE_ID where TB_SAYAC_ID = @TB_SAYAC_ID";

					prms.Clear();
					if (makineId == 0) prms.Add("@MES_MAKINE_ID", entity.MES_MAKINE_ID);
					else prms.Add("@MES_MAKINE_ID", makineId);
					prms.Add("@TB_SAYAC_ID", entity.TB_SAYAC_ID);
					prms.Add("@MES_TANIM", entity.MES_TANIM);
					prms.Add("@MES_BIRIM_KOD_ID", entity.MES_BIRIM_KOD_ID);
					prms.Add("@MES_TIP_KOD_ID", entity.MES_TIP_KOD_ID);
					prms.Add("@MES_GUNCEL_DEGER", entity.MES_GUNCEL_DEGER);
					prms.Add("@MES_VARSAYILAN", entity.MES_VARSAYILAN);
					prms.Add("@MES_SANAL_SAYAC", entity.MES_SANAL_SAYAC);
					prms.Add("@MES_SANAL_SAYAC_ARTIS", entity.MES_SANAL_SAYAC_ARTIS);
					prms.Add("@MES_SANAL_SAYAC_BASLANGIC_TARIH", entity.MES_SANAL_SAYAC_BASLANGIC_TARIH);
					prms.Add("@MES_GUNCELLEME_SEKLI", entity.MES_GUNCELLEME_SEKLI);
					prms.Add("@MES_OZEL_ALAN_1", entity.MES_OZEL_ALAN_1);
					prms.Add("@MES_OZEL_ALAN_2", entity.MES_OZEL_ALAN_2);
					prms.Add("@MES_OZEL_ALAN_3", entity.MES_OZEL_ALAN_3);
					prms.Add("@MES_OZEL_ALAN_4", entity.MES_OZEL_ALAN_4);
					prms.Add("@MES_OZEL_ALAN_5", entity.MES_OZEL_ALAN_5);
					prms.Add("@MES_OZEL_ALAN_6", entity.MES_OZEL_ALAN_6);
					prms.Add("@MES_OZEL_ALAN_7", entity.MES_OZEL_ALAN_7);
					prms.Add("@MES_OZEL_ALAN_8", entity.MES_OZEL_ALAN_8);
					prms.Add("@MES_OZEL_ALAN_9", entity.MES_OZEL_ALAN_9);
					prms.Add("@MES_OZEL_ALAN_10", entity.MES_OZEL_ALAN_10);
					prms.Add("@MES_ACIKLAMA", entity.MES_ACIKLAMA);
					prms.Add("@MES_REF_ID", entity.MES_REF_ID);
					prms.Add("@MES_REF_GRUP", entity.MES_REF_GRUP);
					prms.Add("@MES_DEGISTIREN_ID", entity.MES_DEGISTIREN_ID);
					prms.Add("@MES_DEGISTIRME_TARIH", DateTime.Now);
					klas.baglan();
					klas.cmd(query, prms.PARAMS);
					bildirim.Aciklama = "Updated Successfully";
					bildirim.Durum = true;
					return bildirim;
					#endregion

				}
			}
			catch (Exception e) 
			{
				bildirim.Aciklama = e.Message;
				bildirim.Durum = false;
				return bildirim;
			}
		}
	}
}
