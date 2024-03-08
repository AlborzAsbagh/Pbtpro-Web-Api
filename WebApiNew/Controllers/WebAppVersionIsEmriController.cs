using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using Newtonsoft.Json.Linq;
using WebApiNew.App_GlobalResources;
using WebApiNew.Filters;
using WebApiNew.Models;
using WebApiNew.Utility.Abstract;


/*
 * 
 * 
 * 
 *      IS EMRI Controller For Web App Versions
 *      ( Mobil ve Web Arasındaki ortak fonksiyonlar kendi kontrollerinde yazılmıştır . )
 *
 *
 *
 */

namespace WebApiNew.Controllers
{
	[MyBasicAuthenticationFilter]
	public class WebAppVersionIsEmriController : ApiController
	{
		Util klas = new Util();
		Parametreler prms = new Parametreler();
		private readonly ILogger _logger;
		string query = "";
		SqlCommand cmd = null;
		List<Prm> parametreler = new List<Prm>();

		public WebAppVersionIsEmriController(ILogger logger)
		{
			_logger = logger;
		}


		[Route("api/GetIsEmriFullList")]
		[HttpPost]
		public Object GetIsEmriFullList([FromUri] int id, [FromUri] string parametre, [FromBody] JObject filtreler, [FromUri] int pagingDeger = 1)
		{

			List<WebVersionIsEmriModel> listem = new List<WebVersionIsEmriModel>();

			int pagingIlkDeger = pagingDeger == 1 ? 1 : ((pagingDeger * 50) - 50);
			int pagingSonDeger = pagingIlkDeger == 1 ? 50 : ((pagingDeger * 50));
			int toplamIsEmriSayisi = 0;
			string MAIN_QUERY = "";
			string TOTAL_IS_EMRI_QUERY = "";
			string isParametreForTotalIsEmri = "";
			SqlCommand cmd = null;

			try
			{
				MAIN_QUERY = "SELECT * from (select ROW_NUMBER() over (order by TB_ISEMRI_ID DESC) as RowIndex , * FROM VW_WEB_VERSION_ISEMRI WHERE 1 = 1 ";

				string lokasyonQuery = "", durumQuery = "", isemritipQuery = "", customfilterQuery = "";
				int counter = 0;

				if (filtreler != null || (!string.IsNullOrEmpty(parametre)))
				{
					JObject isemritipleri = filtreler?["isemritipleri"] as JObject;
					JObject durumlar = filtreler?["durumlar"] as JObject;
					JObject lokasyonlar = filtreler?["lokasyonlar"] as JObject;
					JObject customfilter = filtreler?["customfilter"] as JObject;

					if (isemritipleri != null && isemritipleri.Count != 0)
					{
						isemritipQuery = " ( ";
						counter = 0;

						foreach (var property in isemritipleri)
						{
							isemritipQuery += " ISEMRI_TIP LIKE '" + property.Value + "%' ";

							if (counter < isemritipleri.Count - 1)
							{
								isemritipQuery += " or ";
							}

							counter++;
						}

						isemritipQuery += " ) ";
					}

					if (durumlar != null && durumlar.Count != 0)
					{
						durumQuery = " ( ";
						counter = 0;

						foreach (var property in durumlar)
						{
							durumQuery += " DURUM LIKE '" + property.Value + "%' ";

							if (counter < durumlar.Count - 1)
							{
								durumQuery += " or ";
							}
							counter++;
						}
						durumQuery += " ) ";
					}

					if (lokasyonlar != null && lokasyonlar.Count != 0)
					{
						lokasyonQuery = " ( ";
						counter = 0;

						foreach (var property in lokasyonlar)
						{
							lokasyonQuery += " LOKASYON LIKE '" + property.Value + "%' ";
							if (counter < lokasyonlar.Count - 1)
							{
								lokasyonQuery += " or ";
							}
							counter++;
						}
						lokasyonQuery += " ) ";
					}

					if (customfilter != null && customfilter.Count != 0)
					{
						customfilterQuery = " ( ";
						counter = 0;
						foreach (var property in customfilter)
						{
							customfilterQuery += $" {property.Key} LIKE '" + property.Value + "%'";
							if (counter < customfilter.Count - 1)
							{
								customfilterQuery += " or ";
							}
							counter++;
						}
						customfilterQuery += " ) ";

					}

					if (!string.IsNullOrEmpty(parametre))
					{
						MAIN_QUERY += $" AND ( KAPALI  LIKE '%" + parametre + "%' or " +
									  $" ISEMRI_NO  LIKE '%" + parametre + "%' or " +
									  $" KONU  LIKE '%" + parametre + "%' or " +
									  $" ISEMRI_TIP  LIKE '%" + parametre + "%' or " +
									  $" DURUM  LIKE '%" + parametre + "%' or " +
									  $" LOKASYON  LIKE '%" + parametre + "%' or " +
									  $" MAKINE_KODU  LIKE '%" + parametre + "%' or " +
									  $" MAKINE_TANIMI  LIKE '%" + parametre + "%' or " +
									  $" IS_TIPI  LIKE '%" + parametre + "%' or " +
									  $" IS_NEDENI  LIKE '%" + parametre + "%' or " +
									  $" ATOLYE  LIKE '%" + parametre + "%' or " +
									  $" PERSONEL_ADI LIKE '%" + parametre + "%' ) ";

						isParametreForTotalIsEmri = $" AND ( KAPALI  LIKE '%" + parametre + "%' or " +
									  $" ISEMRI_NO  LIKE '%" + parametre + "%' or " +
									  $" KONU  LIKE '%" + parametre + "%' or " +
									  $" ISEMRI_TIP  LIKE '%" + parametre + "%' or " +
									  $" DURUM  LIKE '%" + parametre + "%' or " +
									  $" LOKASYON  LIKE '%" + parametre + "%' or " +
									  $" MAKINE_KODU  LIKE '%" + parametre + "%' or " +
									  $" MAKINE_TANIMI  LIKE '%" + parametre + "%' or " +
									  $" IS_TIPI  LIKE '%" + parametre + "%' or " +
									  $" IS_NEDENI  LIKE '%" + parametre + "%' or " +
									  $" ATOLYE  LIKE '%" + parametre + "%' or " +
									  $" PERSONEL_ADI LIKE '%" + parametre + "%' ) ";
					}

					if (isemritipleri != null && isemritipleri.Count != 0) MAIN_QUERY += " AND " + isemritipQuery;
					if (durumlar != null && durumlar.Count != 0) MAIN_QUERY += " AND " + durumQuery;
					if (lokasyonlar != null && lokasyonlar.Count != 0) MAIN_QUERY += " AND " + lokasyonQuery;
					if (customfilter != null && customfilter.Count != 0) MAIN_QUERY += " AND " + customfilterQuery;

					MAIN_QUERY += $" ) as SubRow where SubRow.RowIndex >= {pagingIlkDeger} and SubRow.RowIndex < {pagingSonDeger} ";

					TOTAL_IS_EMRI_QUERY = " select count(*) from (select * from dbo.VW_WEB_VERSION_ISEMRI where 1=1 ";
					TOTAL_IS_EMRI_QUERY += isParametreForTotalIsEmri;

					if (isemritipleri != null && isemritipleri.Count != 0) TOTAL_IS_EMRI_QUERY += " AND " + isemritipQuery;
					if (durumlar != null && durumlar.Count != 0) TOTAL_IS_EMRI_QUERY += " AND " + durumQuery;
					if (lokasyonlar != null && lokasyonlar.Count != 0) TOTAL_IS_EMRI_QUERY += " AND " + lokasyonQuery;
					if (customfilter != null && customfilter.Count != 0) TOTAL_IS_EMRI_QUERY += " AND " + customfilterQuery;

					TOTAL_IS_EMRI_QUERY += " ) as TotalIsEmriSayisi ";
				}

				else
				{

					MAIN_QUERY = $" SELECT * FROM ( SELECT ROW_NUMBER() OVER (ORDER BY TB_ISEMRI_ID DESC) as RowIndex , * FROM VW_WEB_VERSION_ISEMRI ) as SubRow where SubRow.RowIndex >= {pagingIlkDeger} and SubRow.RowIndex < {pagingSonDeger} ";
					TOTAL_IS_EMRI_QUERY = " select count(*) from (select * from dbo.VW_WEB_VERSION_ISEMRI where 1=1) as TotalIsEmriSayisi ";
				}


				using(var cnn = klas.baglan())
				{
					listem = cnn.Query<WebVersionIsEmriModel>(MAIN_QUERY).ToList();
					cmd = new SqlCommand(TOTAL_IS_EMRI_QUERY, cnn);
					toplamIsEmriSayisi = (int)cmd.ExecuteScalar();
				}

				klas.kapat();
				return Json(new { page = (int)Math.Ceiling((decimal)toplamIsEmriSayisi / 50), list = listem });
			}
			catch (Exception e)
			{
				klas.kapat();
				return Json(e.Message);
			}
		}

		[Route("api/GetIsEmriById")]
		[HttpGet]
		public Object GetIsEmriById([FromUri] int isEmriId)
		{
			Util klas = new Util();
			List<WebVersionIsEmriModel> listem = new List<WebVersionIsEmriModel>();
			string query = @"select * from dbo.VW_WEB_VERSION_ISEMRI where TB_ISEMRI_ID = @TB_ISEMRI_ID";
			using (var conn = klas.baglan())
			{
				listem = conn.Query<WebVersionIsEmriModel>(query, new { @TB_ISEMRI_ID = isEmriId }).ToList();
			}
			return listem;

		}

		[Route("api/IsEmriDurumVarsayilanYap")]
		[HttpGet]
		public Object IsEmriDurumVarsayilanYap([FromUri] int kodId,[FromUri] bool isVarsayilan)
		{
			try
			{
				query = $" update orjin.TB_KOD set KOD_ISM_DURUM_VARSAYILAN = 0 where KOD_GRUP = 32801 ";
				query += $" update orjin.TB_KOD set KOD_ISM_DURUM_VARSAYILAN = 1 where TB_KOD_ID = {kodId} and KOD_GRUP = 32801 ";

				using (var con = klas.baglan())
				{
					cmd = new SqlCommand(query, con);
					cmd.ExecuteNonQuery();
				}
				klas.kapat();
				return Json(new { has_error = false , status_code = 200 ,  status = "Process completed successfully !" });
			}
			catch (Exception e)
			{
				klas.kapat();
				return Json(new { has_error = true , status_code = 500 , status = e.Message });
			}

		}

		[Route("api/AddIsEmriDurumDegisikligi")]
		[HttpPost]
		public Object AddIsEmriDurumDegisikligi([FromBody] IsEmriLog entity)
		{
			try
			{
				query = " insert into orjin.TB_ISEMRI_LOG";
				query += @" (ISL_ISEMRI_ID , ISL_KULLANICI_ID , ISL_TARIH , ISL_SAAT , ISL_ISLEM , ISL_ACIKLAMA , ISL_DURUM_ESKI_KOD_ID ,
						ISL_DURUM_YENI_KOD_ID , ISL_OLUSTURAN_ID , ISL_OLUSTURMA_TARIH , ISL_DEGISTIRME_TARIH ) values ";

				if (entity != null)
				{
					klas.baglan();
					query += @" (@ISL_ISEMRI_ID , @ISL_KULLANICI_ID , @ISL_TARIH , @ISL_SAAT , @ISL_ISLEM , @ISL_ACIKLAMA , @ISL_DURUM_ESKI_KOD_ID ,
						@ISL_DURUM_YENI_KOD_ID , @ISL_OLUSTURAN_ID , @ISL_OLUSTURMA_TARIH , @ISL_DEGISTIRME_TARIH ) ";

					query += @" update orjin.TB_ISEMRI set ISM_DURUM_KOD_ID = @ISM_DURUM_KOD_ID where TB_ISEMRI_ID = @TB_ISEMRI_ID ";

					prms.Clear();
					prms.Add("ISL_ISEMRI_ID", entity.ISL_ISEMRI_ID);
					prms.Add("ISL_KULLANICI_ID", entity.ISL_KULLANICI_ID);
					prms.Add("ISL_TARIH", DateTime.Now);
					prms.Add("ISL_SAAT", DateTime.Now.ToString("HH:mm:ss"));
					prms.Add("ISL_ISLEM", entity.ISL_ISLEM);
					prms.Add("ISL_ACIKLAMA", entity.ISL_ACIKLAMA);
					prms.Add("ISL_DURUM_ESKI_KOD_ID", entity.ISL_DURUM_ESKI_KOD_ID);
					prms.Add("ISL_DURUM_YENI_KOD_ID", entity.ISL_DURUM_YENI_KOD_ID);
					prms.Add("ISL_OLUSTURAN_ID", entity.ISL_OLUSTURAN_ID);
					prms.Add("ISL_OLUSTURMA_TARIH", DateTime.Now);
					prms.Add("ISL_DEGISTIRME_TARIH", DateTime.Now);
					prms.Add("TB_ISEMRI_ID", entity.ISL_ISEMRI_ID);
					prms.Add("ISM_DURUM_KOD_ID", entity.ISL_DURUM_YENI_KOD_ID);

					klas.cmd(query, prms.PARAMS);
					klas.kapat();
					return Json(new { has_error = false, status_code = 200, status = "Entity Updated Successfully" });
				}
				else
				{
					return Json(new { has_error = false, status_code = 400, status = "Bad Request ( entity may be null or 0 lentgh)" });

				}
			} 
			catch (Exception ex) 
			{
				klas.kapat();
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}

		
		[Route("api/IsEmriEkleVarsiylanlar")]
		[HttpGet]
		public async Task<Object> IsEmriEkleVarsiylanlar()
		{
			string query = @"SELECT TB_KOD_ID AS ID, KOD_TANIM AS TANIM, 'IS_EMRI_DURUM_VARSAYILAN' AS TABLO_TANIMI
								FROM orjin.TB_KOD
								WHERE KOD_ISM_DURUM_VARSAYILAN = 1 AND KOD_GRUP = 32801

								UNION ALL

								SELECT TB_ISEMRI_TIP_ID AS ID, IMT_TANIM AS TANIM, 'IS_EMRI_TIP_VARSAYILAN' AS TABLO_TANIMI
								FROM orjin.TB_ISEMRI_TIP
								WHERE IMT_VARSAYILAN = 1 

								UNION ALL

								SELECT TB_SERVIS_ONCELIK_ID AS ID, SOC_TANIM AS TANIM, 'SERVIS_ONCELIK' AS TABLO_TANIMI
								FROM orjin.TB_SERVIS_ONCELIK
								WHERE SOC_VARSAYILAN = 1";
			var klas = new Util();
			List<IsEmriEkleVarsayilanDegerler> listem = new List<IsEmriEkleVarsayilanDegerler>();

			string query2 = "  select * from orjin.TB_ISEMRI_TIP where IMT_VARSAYILAN = 1 ";

			IsEmriTip entity = new IsEmriTip();
			using (var cnn = klas.baglan())
			{
				listem = cnn.Query<IsEmriEkleVarsayilanDegerler>(query).ToList();
				entity = await cnn.QueryFirstOrDefaultAsync<IsEmriTip>(query2);
			}

			return Json(new { is_emri_varsayilanlar = listem , is_emri_tip_varsayilan = entity});
		}

		[Route("api/GetIsTipi")]
		[HttpGet]
		public List<Kod> GetIsTipi(int? isTanimId = 0)
		{
			prms.Clear();

			if(isTanimId == 0) { 
				prms.Add("BKM_TIP", 32440); 
				prms.Add("ARZ_TIP", 32401); 
				query = @"select * from orjin.TB_KOD where KOD_GRUP = @BKM_TIP or KOD_GRUP = @ARZ_TIP ";
			}
			if (isTanimId == 1)
			{
				prms.Add("ARZ_TIP", 32401);
				query = @"select * from orjin.TB_KOD where KOD_GRUP = @ARZ_TIP ";
			}
			if (isTanimId == 2)
			{
				prms.Add("BKM_TIP", 32440);
				query = @"select * from orjin.TB_KOD where KOD_GRUP = @BKM_TIP ";
			}

			DataTable dt = klas.GetDataTable(query, prms.PARAMS);
			List<Kod> listem = new List<Kod>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				Kod entity = new Kod();
				entity.TB_KOD_ID = (int)dt.Rows[i]["TB_KOD_ID"];
				entity.KOD_GRUP = Util.getFieldString(dt.Rows[i], "KOD_GRUP");
				entity.KOD_TANIM = Util.getFieldString(dt.Rows[i], "KOD_TANIM");
				entity.KOD_ISM_DURUM_VARSAYILAN = Util.getFieldBool(dt.Rows[i], "KOD_ISM_DURUM_VARSAYILAN");
				listem.Add(entity);
			}
			return listem;
		}

		[Route("api/GetIsNeden")]
		[HttpGet]
		public List<Kod> GetIsNeden(int? isTanimId = 0)
		{
			prms.Clear();

			if (isTanimId == 0)
			{
				prms.Add("BKM_NEDEN", 32452);
				prms.Add("ARZ_NEDEN", 32413);
				query = @"select * from orjin.TB_KOD where KOD_GRUP = @BKM_NEDEN or KOD_GRUP = @ARZ_NEDEN ";
			}
			if (isTanimId == 1)
			{
				prms.Add("ARZ_NEDEN", 32413);
				query = @"select * from orjin.TB_KOD where KOD_GRUP = @ARZ_NEDEN ";
			}
			if (isTanimId == 2)
			{
				prms.Add("BKM_NEDEN", 32452);
				query = @"select * from orjin.TB_KOD where KOD_GRUP = @BKM_NEDEN ";
			}

			DataTable dt = klas.GetDataTable(query, prms.PARAMS);
			List<Kod> listem = new List<Kod>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				Kod entity = new Kod();
				entity.TB_KOD_ID = (int)dt.Rows[i]["TB_KOD_ID"];
				entity.KOD_GRUP = Util.getFieldString(dt.Rows[i], "KOD_GRUP");
				entity.KOD_TANIM = Util.getFieldString(dt.Rows[i], "KOD_TANIM");
				entity.KOD_ISM_DURUM_VARSAYILAN = Util.getFieldBool(dt.Rows[i], "KOD_ISM_DURUM_VARSAYILAN");
				listem.Add(entity);
			}
			return listem;
		}

		[Route("api/AddIsTip")]
		[HttpPost]
		public Object AddIsTip([FromUri] string entity, [FromUri] int isTanimId)
		{
			try
			{
				if(isTanimId == 0 || isTanimId == 2)
				{
					query = " insert into orjin.TB_KOD (KOD_GRUP , KOD_TANIM , KOD_AKTIF , KOD_GOR , KOD_DEGISTIR , KOD_SIL ) ";
					query += $" values ( '32440' , '{entity}' , 1 , 1 , 1 ,1) ";
				}
				else if(isTanimId == 1) 
				{
					query = " insert into orjin.TB_KOD (KOD_GRUP , KOD_TANIM , KOD_AKTIF , KOD_GOR , KOD_DEGISTIR , KOD_SIL ) ";
					query += $" values ( '32401' , '{entity}' , 1 , 1 , 1 ,1) ";
				}

				using (var con = klas.baglan())
				{
					cmd = new SqlCommand(query, con);
					cmd.ExecuteNonQuery();
				}
				klas.kapat();
				return Json(new { status_code = 201, status = "Added Successfully" });

			}
			catch (Exception ex)
			{
				klas.kapat();
				return Json(new { status_code = 500, status = ex.Message });
			}
		}

		[Route("api/AddIsNeden")]
		[HttpPost]
		public Object AddIsNeden([FromUri] string entity, [FromUri] int isTanimId)
		{
			try
			{
				if (isTanimId == 0 || isTanimId == 2)
				{
					query = " insert into orjin.TB_KOD (KOD_GRUP , KOD_TANIM , KOD_AKTIF , KOD_GOR , KOD_DEGISTIR , KOD_SIL ) ";
					query += $" values ( '32452' , '{entity}' , 1 , 1 , 1 ,1) ";
				}
				else if (isTanimId == 1)
				{
					query = " insert into orjin.TB_KOD (KOD_GRUP , KOD_TANIM , KOD_AKTIF , KOD_GOR , KOD_DEGISTIR , KOD_SIL ) ";
					query += $" values ( '32413' , '{entity}' , 1 , 1 , 1 ,1) ";
				}

				using (var con = klas.baglan())
				{
					cmd = new SqlCommand(query, con);
					cmd.ExecuteNonQuery();
				}
				klas.kapat();
				return Json(new { status_code = 201, status = "Added Successfully" });

			}
			catch (Exception ex)
			{
				klas.kapat();
				return Json(new { status_code = 500, status = ex.Message });
			}
		}

		[Route("api/AddUpdateIsEmriKontrolList")]
		[HttpPost]
		public object KontrolListKaydet([FromBody] IsEmriKontrolList entity, long isEmriId = 0)
		{
			string plainText, rtfText;
			string durum = "";
			try
			{
				if (entity.DKN_ACIKLAMA != null && entity.DKN_ACIKLAMA.StartsWith(@"{\rtf"))
				{

					System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox();
					rtfText = entity.DKN_ACIKLAMA;
					rtBox.Rtf = rtfText;
					plainText = rtBox.Text;
				}

				else { plainText = entity.DKN_ACIKLAMA; }

				if (entity.TB_ISEMRI_KONTROLLIST_ID < 1)
				{
					// Yeni Ekle
					string query = @"INSERT INTO orjin.TB_ISEMRI_KONTROLLIST
                               (DKN_ISEMRI_ID
                               ,DKN_SIRANO
                               ,DKN_YAPILDI
                               ,DKN_TANIM
                               ,DKN_OLUSTURAN_ID
                               ,DKN_OLUSTURMA_TARIH
                               ,DKN_YAPILDI_TARIH
                               ,DKN_BITIS_TARIH
                               ,DKN_BITIS_SAAT
                               ,DKN_MALIYET                              
                               ,DKN_YAPILDI_SAAT
                               ,DKN_YAPILDI_PERSONEL_ID
                               ,DKN_YAPILDI_MESAI_KOD_ID
                               ,DKN_YAPILDI_ATOLYE_ID
                               ,DKN_YAPILDI_SURE
                               ,DKN_ACIKLAMA
                               ,DKN_REF_ID)
                         VALUES (@DKN_ISEMRI_ID
                               ,@DKN_SIRANO
                               ,@DKN_YAPILDI
                               ,@DKN_TANIM
                               ,@DKN_OLUSTURAN_ID
                               ,@DKN_OLUSTURMA_TARIH
                               ,@DKN_YAPILDI_TARIH
                               ,@DKN_BITIS_TARIH
                               ,@DKN_BITIS_SAAT
                               ,@DKN_MALIYET                              
                               ,@DKN_YAPILDI_SAAT
                               ,@DKN_YAPILDI_PERSONEL_ID
                               ,@DKN_YAPILDI_MESAI_KOD_ID
                               ,@DKN_YAPILDI_ATOLYE_ID
                               ,@DKN_YAPILDI_SURE
                               ,@DKN_ACIKLAMA
                               ,@DKN_REF_ID)";
					prms.Clear();
					if (isEmriId == 0) prms.Add("DKN_ISEMRI_ID", entity.DKN_ISEMRI_ID);
					else prms.Add("DKN_ISEMRI_ID", isEmriId);
					prms.Add("DKN_SIRANO", entity.DKN_SIRANO);
					prms.Add("DKN_YAPILDI", entity.DKN_YAPILDI);
					prms.Add("DKN_TANIM", entity.DKN_TANIM);
					prms.Add("DKN_OLUSTURAN_ID", entity.DKN_OLUSTURAN_ID);
					prms.Add("DKN_OLUSTURMA_TARIH", DateTime.Now);
					prms.Add("DKN_YAPILDI_TARIH", entity.DKN_YAPILDI_TARIH);
					prms.Add("DKN_BITIS_TARIH", entity.DKN_BITIS_TARIH);
					prms.Add("DKN_BITIS_SAAT", entity.DKN_BITIS_SAAT != null ? entity.DKN_BITIS_SAAT : "");
					prms.Add("DKN_MALIYET", 0);
					prms.Add("DKN_YAPILDI_SAAT", entity.DKN_YAPILDI_SAAT != null ? entity.DKN_YAPILDI_SAAT : "");
					prms.Add("DKN_YAPILDI_PERSONEL_ID", entity.DKN_YAPILDI_PERSONEL_ID);
					prms.Add("DKN_YAPILDI_MESAI_KOD_ID", entity.DKN_YAPILDI_MESAI_KOD_ID);
					prms.Add("DKN_YAPILDI_ATOLYE_ID", entity.DKN_YAPILDI_ATOLYE_ID);
					prms.Add("DKN_YAPILDI_SURE", entity.DKN_YAPILDI_SURE);
					prms.Add("DKN_ACIKLAMA", plainText);
					prms.Add("DKN_REF_ID", entity.DKN_REF_ID);
					klas.cmd(query, prms.PARAMS);

					durum = "Entity Added Successfully !";
				}
				else
				{
					// Güncelle
					string query = @"UPDATE orjin.TB_ISEMRI_KONTROLLIST SET
                                DKN_SIRANO = @DKN_SIRANO
                               ,DKN_YAPILDI=@DKN_YAPILDI
                               ,DKN_TANIM = @DKN_TANIM
                               ,DKN_DEGISTIREN_ID = @DKN_DEGISTIREN_ID
                               ,DKN_DEGISTIRME_TARIH = @DKN_DEGISTIRME_TARIH 
                               ,DKN_ACIKLAMA = @DKN_ACIKLAMA
                               ,DKN_YAPILDI_MESAI_KOD_ID = @DKN_YAPILDI_MESAI_KOD_ID
                               ,DKN_YAPILDI_PERSONEL_ID = @DKN_YAPILDI_PERSONEL_ID
                               ,DKN_YAPILDI_ATOLYE_ID = @DKN_YAPILDI_ATOLYE_ID
                               ,DKN_YAPILDI_SURE = @DKN_YAPILDI_SURE
                               ,DKN_REF_ID = @DKN_REF_ID WHERE TB_ISEMRI_KONTROLLIST_ID = @TB_ISEMRI_KONTROLLIST_ID";
					prms.Clear();
					prms.Add("TB_ISEMRI_KONTROLLIST_ID", entity.TB_ISEMRI_KONTROLLIST_ID);
					prms.Add("DKN_YAPILDI", entity.DKN_YAPILDI);
					prms.Add("DKN_SIRANO", entity.DKN_SIRANO);
					prms.Add("DKN_TANIM", entity.DKN_TANIM);
					prms.Add("DKN_DEGISTIREN_ID", entity.DKN_OLUSTURAN_ID);
					prms.Add("DKN_DEGISTIRME_TARIH", DateTime.Now);
					prms.Add("DKN_ACIKLAMA", plainText);
					prms.Add("DKN_YAPILDI_MESAI_KOD_ID", entity.DKN_YAPILDI_MESAI_KOD_ID);
					prms.Add("DKN_YAPILDI_PERSONEL_ID", entity.DKN_YAPILDI_PERSONEL_ID);
					prms.Add("DKN_YAPILDI_ATOLYE_ID", entity.DKN_YAPILDI_ATOLYE_ID);
					prms.Add("DKN_YAPILDI_SURE", entity.DKN_YAPILDI_SURE);
					prms.Add("DKN_REF_ID", entity.DKN_REF_ID);
					klas.cmd(query, prms.PARAMS);
					durum = "Entity Updated Successfully ! ";
					
				}
				return Json(new { has_error = false, status_code = 200, status = durum });
			}
			catch (Exception ex)
			{
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}

		}

		[Route("api/AddUpdateIsemriMalzeme")]
		[HttpPost]
		public object MalzemeListKaydet(IsEmriMalzeme entity, long isEmriId = 0)
		{
			string durum = "";
			try
			{
				if (entity.TB_ISEMRI_MLZ_ID < 1)
				{
					#region Kaydet

					string qu1 = @"INSERT INTO orjin.TB_ISEMRI_MLZ
                                                       (IDM_ISEMRI_ID
                                                       ,IDM_TARIH
                                                       ,IDM_SAAT
                                                       ,IDM_STOK_ID
                                                       ,IDM_DEPO_ID
                                                       ,IDM_BIRIM_KOD_ID
                                                       ,IDM_STOK_TIP_KOD_ID
                                                       ,IDM_STOK_DUS
                                                       ,IDM_STOK_TANIM
                                                       ,IDM_BIRIM_FIYAT
                                                       ,IDM_MIKTAR
                                                       ,IDM_TUTAR
                                                       ,IDM_OLUSTURAN_ID
                                                       ,IDM_OLUSTURMA_TARIH                                                      
                                                       ,IDM_GARANTI_BAS_TARIH                                                     
                                                       ,IDM_GARANTI_BIT_TARIH                                                    
                                                       ,IDM_REF_ID
                                                       ,IDM_STOK_KULLANIM_SEKLI
                                                       ,IDM_MALZEME_STOKTAN
                                                       ,IDM_ALTERNATIF_STOK_ID
                                                       ,IDM_MARKA_KOD_ID)
                                                 VALUES (@IDM_ISEMRI_ID
                                                       ,@IDM_TARIH
                                                       ,@IDM_SAAT
                                                       ,@IDM_STOK_ID
                                                       ,@IDM_DEPO_ID
                                                       ,@IDM_BIRIM_KOD_ID
                                                       ,@IDM_STOK_TIP_KOD_ID
                                                       ,@IDM_STOK_DUS
                                                       ,@IDM_STOK_TANIM
                                                       ,@IDM_BIRIM_FIYAT
                                                       ,@IDM_MIKTAR
                                                       ,@IDM_TUTAR
                                                       ,@IDM_OLUSTURAN_ID
                                                       ,@IDM_OLUSTURMA_TARIH                                                      
                                                       ,@IDM_GARANTI_BAS_TARIH                                                      
                                                       ,@IDM_GARANTI_BIT_TARIH                                                     
                                                       ,@IDM_REF_ID
                                                       ,@IDM_STOK_KULLANIM_SEKLI
                                                       ,@IDM_MALZEME_STOKTAN
                                                       ,-1
                                                       ,-1)";
					prms.Clear();
					if (isEmriId == 0) prms.Add("@IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
					else prms.Add("@IDM_ISEMRI_ID", isEmriId);
					prms.Add("@IDM_TARIH", entity.IDM_TARIH);
					prms.Add("@IDM_SAAT", entity.IDM_SAAT);
					prms.Add("@IDM_STOK_ID", entity.IDM_STOK_ID);
					prms.Add("@IDM_DEPO_ID", entity.IDM_DEPO_ID);
					prms.Add("@IDM_BIRIM_KOD_ID", entity.IDM_BIRIM_KOD_ID);
					prms.Add("@IDM_STOK_TIP_KOD_ID", entity.IDM_STOK_TIP_KOD_ID);
					prms.Add("@IDM_STOK_DUS", entity.IDM_STOK_DUS);
					prms.Add("@IDM_STOK_TANIM", entity.IDM_STOK_TANIM);
					prms.Add("@IDM_BIRIM_FIYAT", entity.IDM_BIRIM_FIYAT);
					prms.Add("@IDM_MIKTAR", entity.IDM_MIKTAR);
					prms.Add("@IDM_TUTAR", entity.IDM_TUTAR);
					prms.Add("@IDM_OLUSTURAN_ID", entity.IDM_OLUSTURAN_ID);
					prms.Add("@IDM_OLUSTURMA_TARIH", DateTime.Now);
					prms.Add("@IDM_GARANTI_BAS_TARIH", entity.IDM_GARANTI_BAS_TARIH);
					prms.Add("@IDM_GARANTI_BIT_TARIH", entity.IDM_GARANTI_BIT_TARIH);
					prms.Add("@IDM_REF_ID", entity.IDM_REF_ID); // iş tanım veya periyodik bakım id
					GenelListeController gn = new GenelListeController();
					entity.IDM_STOK_KULLANIM_SEKLI = Convert.ToInt32(gn.ParametreDeger("320112").PRM_DEGER);
					prms.Add("@IDM_STOK_KULLANIM_SEKLI",
						entity.IDM_STOK_KULLANIM_SEKLI); // 1 ise iş emri açıkken düş 2 işe iş emri kapatılırken düş
					prms.Add("@IDM_MALZEME_STOKTAN", entity.IDM_MALZEME_STOKTAN); // Düşsün
					klas.cmd(qu1, prms.PARAMS);
					string qu2 =
						"UPDATE orjin.TB_ISEMRI SET ISM_MALIYET_MLZ = ISM_MALIYET_MLZ + Cast(@Maliyet AS FLOAT), ISM_MALIYET_TOPLAM =ISM_MALIYET_TOPLAM +  Cast(@Maliyet AS FLOAT) where TB_ISEMRI_ID = @IDM_ISEMRI_ID";
					prms.Clear();
					prms.Add("@Maliyet", entity.IDM_TUTAR);
					prms.Add("@IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
					klas.cmd(qu2, prms.PARAMS);
					klas.kapat();
					if (entity.IDM_STOK_DUS && entity.IDM_DEPO_ID > 0 && entity.IDM_STOK_KULLANIM_SEKLI == 1 &&
						(entity.IDM_MALZEME_STOKTAN.Trim() == "Düşsün" ||
						 entity.IDM_MALZEME_STOKTAN.Trim() == "Sorsun"))
					{
						StokHareketIslemi(entity);
					}

					durum = "Entity Added Successfully !";

					#endregion
				}
				else
				{
					#region Güncelle

					parametreler.Clear();
					parametreler.Add(new Prm("TB_ISEMRI_MLZ_ID", entity.TB_ISEMRI_MLZ_ID));
					double eskiMaliyet = Convert.ToDouble(klas.GetDataCell(
						"select coalesce(IDM_TUTAR,0) from orjin.TB_ISEMRI_MLZ WHERE TB_ISEMRI_MLZ_ID=@TB_ISEMRI_MLZ_ID",
						parametreler));
					string qu3 = @"UPDATE orjin.TB_ISEMRI_MLZ SET
                                                        IDM_TARIH                         = @IDM_TARIH
                                                       ,IDM_SAAT                          = @IDM_SAAT
                                                       ,IDM_STOK_ID                       = @IDM_STOK_ID
                                                       ,IDM_DEPO_ID                       = @IDM_DEPO_ID
                                                       ,IDM_BIRIM_KOD_ID                  = @IDM_BIRIM_KOD_ID
                                                       ,IDM_STOK_TIP_KOD_ID               = @IDM_STOK_TIP_KOD_ID
                                                       ,IDM_STOK_DUS                      = @IDM_STOK_DUS
                                                       ,IDM_STOK_TANIM                    = @IDM_STOK_TANIM
                                                       ,IDM_BIRIM_FIYAT                   = @IDM_BIRIM_FIYAT
                                                       ,IDM_MIKTAR                        = @IDM_MIKTAR
                                                       ,IDM_TUTAR                         = @IDM_TUTAR
                                                       ,IDM_DEGISTIREN_ID                 = @IDM_DEGISTIREN_ID
                                                       ,IDM_DEGISTIRME_TARIH              = @IDM_DEGISTIRME_TARIH        
                                                       ,IDM_REF_ID                        = @IDM_REF_ID
                                                       ,IDM_STOK_KULLANIM_SEKLI           = @IDM_STOK_KULLANIM_SEKLI
                                                       ,IDM_MALZEME_STOKTAN               = @IDM_MALZEME_STOKTAN WHERE TB_ISEMRI_MLZ_ID = @TB_ISEMRI_MLZ_ID";
					prms.Clear();

					prms.Add("@IDM_TARIH", entity.IDM_TARIH);
					prms.Add("@IDM_SAAT", entity.IDM_SAAT);
					prms.Add("@IDM_STOK_ID", entity.IDM_STOK_ID);
					prms.Add("@IDM_DEPO_ID", entity.IDM_DEPO_ID);
					prms.Add("@IDM_BIRIM_KOD_ID", entity.IDM_BIRIM_KOD_ID);
					prms.Add("@IDM_STOK_TIP_KOD_ID", entity.IDM_STOK_TIP_KOD_ID);
					prms.Add("@IDM_STOK_DUS", entity.IDM_STOK_DUS);
					prms.Add("@IDM_STOK_TANIM", entity.IDM_STOK_TANIM);
					prms.Add("@IDM_BIRIM_FIYAT", entity.IDM_BIRIM_FIYAT);
					prms.Add("@IDM_MIKTAR", entity.IDM_MIKTAR);
					prms.Add("@IDM_TUTAR", entity.IDM_TUTAR);
					prms.Add("@IDM_DEGISTIREN_ID", entity.IDM_DEGISTIREN_ID);
					prms.Add("@IDM_DEGISTIRME_TARIH", DateTime.Now);
					prms.Add("@IDM_REF_ID", entity.IDM_REF_ID);
					prms.Add("@TB_ISEMRI_MLZ_ID", entity.TB_ISEMRI_MLZ_ID);

					GenelListeController gn = new GenelListeController();
					entity.IDM_STOK_KULLANIM_SEKLI = Convert.ToInt32(gn.ParametreDeger("320112").PRM_DEGER);

					prms.Add("@IDM_STOK_KULLANIM_SEKLI", entity.IDM_STOK_KULLANIM_SEKLI);
					prms.Add("@IDM_MALZEME_STOKTAN", entity.IDM_MALZEME_STOKTAN);
					klas.cmd(qu3, prms.PARAMS);

					#endregion

					if (entity.IDM_STOK_DUS && entity.IDM_DEPO_ID > 0 && entity.IDM_STOK_KULLANIM_SEKLI == 1 &&
						(entity.IDM_MALZEME_STOKTAN == "Düşşün"))
					{
						StokHareketIslemi(entity);
					}

					string qu4 =
						"UPDATE orjin.TB_ISEMRI SET ISM_MALIYET_MLZ = ISM_MALIYET_MLZ - Cast(@Maliyet AS FLOAT), ISM_MALIYET_TOPLAM =ISM_MALIYET_TOPLAM -  Cast(@Maliyet AS FLOAT) where TB_ISEMRI_ID = @IDM_ISEMRI_ID";
					prms.Clear();
					prms.Add("@Maliyet", eskiMaliyet);
					prms.Add("@IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
					klas.cmd(qu4, prms.PARAMS);
					string qu5 =
						"UPDATE orjin.TB_ISEMRI SET ISM_MALIYET_MLZ = ISM_MALIYET_MLZ + Cast(@Maliyet AS FLOAT), ISM_MALIYET_TOPLAM =ISM_MALIYET_TOPLAM +  Cast(@Maliyet AS FLOAT) where TB_ISEMRI_ID = @IDM_ISEMRI_ID";
					prms.Clear();
					prms.Add("@Maliyet", entity.IDM_TUTAR);
					prms.Add("@IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
					klas.cmd(qu5, prms.PARAMS);
					klas.kapat();
					durum = "Entity Updated Successfully ! ";
					
				}
				return Json(new { has_error = false, status_code = 200, status = durum });
			}
			catch (Exception ex)
			{
				klas.kapat();
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}

		}

		[Route("api/AddUpdateIsEmriDurus")]
		[HttpPost]
		public object IsEmriDurusKaydet([FromBody] IsEmriDurus entity, long isEmriId = 0)
		{
			string durum = "";
			try
			{
				if (entity.TB_MAKINE_DURUS_ID < 1)
				{
					#region Yeni Ekle

					string query = @"INSERT INTO orjin.TB_MAKINE_DURUS
                                   (MKD_ISEMRI_ID
                                   ,MKD_MAKINE_ID
                                   ,MKD_BASLAMA_TARIH
                                   ,MKD_BASLAMA_SAAT
                                   ,MKD_BITIS_TARIH
                                   ,MKD_BITIS_SAAT
                                   ,MKD_SURE
                                   ,MKD_SAAT_MALIYET
                                   ,MKD_TOPLAM_MALIYET
                                   ,MKD_NEDEN_KOD_ID          
                                   ,MKD_PLANLI
                                   ,MKD_OLUSTURAN_ID
                                   ,MKD_OLUSTURMA_TARIH
                                   ,MKD_PROJE_ID       
                                   ,MKD_LOKASYON_ID    
                                   ,MKD_ACIKLAMA)
                             VALUES
		                           (@MKD_ISEMRI_ID
                                   ,@MKD_MAKINE_ID
                                   ,@MKD_BASLAMA_TARIH
                                   ,@MKD_BASLAMA_SAAT
                                   ,@MKD_BITIS_TARIH
                                   ,@MKD_BITIS_SAAT
                                   ,@MKD_SURE
                                   ,@MKD_SAAT_MALIYET
                                   ,@MKD_TOPLAM_MALIYET
                                   ,@MKD_NEDEN_KOD_ID          
                                   ,@MKD_PLANLI
                                   ,@MKD_OLUSTURAN_ID
                                   ,@MKD_OLUSTURMA_TARIH
                                   ,@MKD_PROJE_ID       
                                   ,@MKD_LOKASYON_ID    
                                   ,@MKD_ACIKLAMA)";
					prms.Clear();

					if (isEmriId == 0) prms.Add("MKD_ISEMRI_ID", entity.MKD_ISEMRI_ID);
					else prms.Add("MKD_ISEMRI_ID", isEmriId);
					prms.Add("MKD_MAKINE_ID", entity.MKD_MAKINE_ID);
					prms.Add("MKD_BASLAMA_TARIH", entity.MKD_BASLAMA_TARIH);
					prms.Add("MKD_BASLAMA_SAAT", entity.MKD_BASLAMA_SAAT);
					prms.Add("MKD_BITIS_TARIH", entity.MKD_BITIS_TARIH);
					prms.Add("MKD_BITIS_SAAT", entity.MKD_BITIS_SAAT);
					prms.Add("MKD_SURE", entity.MKD_SURE);
					prms.Add("MKD_SAAT_MALIYET", entity.MKD_SAAT_MALIYET);
					prms.Add("MKD_TOPLAM_MALIYET", entity.MKD_TOPLAM_MALIYET);
					prms.Add("MKD_NEDEN_KOD_ID", entity.MKD_NEDEN_KOD_ID);
					prms.Add("MKD_PLANLI", entity.MKD_PLANLI);
					prms.Add("MKD_OLUSTURAN_ID", entity.MKD_OLUSTURAN_ID);
					prms.Add("MKD_OLUSTURMA_TARIH", DateTime.Now);
					prms.Add("MKD_PROJE_ID", entity.MKD_PROJE_ID);
					prms.Add("MKD_LOKASYON_ID", entity.MKD_LOKASYON_ID);
					prms.Add("MKD_ACIKLAMA", entity.MKD_ACIKLAMA);
					klas.cmd(query, prms.PARAMS);
					klas.kapat();

					#endregion

					durum = "Entity Added Successfully !";
				}
				else
				{
					#region Güncelle

					string query = @"UPDATE orjin.TB_MAKINE_DURUS SET
                                    MKD_MAKINE_ID                   =@MKD_MAKINE_ID
                                   ,MKD_BASLAMA_TARIH               =@MKD_BASLAMA_TARIH
                                   ,MKD_BASLAMA_SAAT                =@MKD_BASLAMA_SAAT
                                   ,MKD_BITIS_TARIH                 =@MKD_BITIS_TARIH
                                   ,MKD_BITIS_SAAT                  =@MKD_BITIS_SAAT
                                   ,MKD_SURE                        =@MKD_SURE
                                   ,MKD_SAAT_MALIYET                =@MKD_SAAT_MALIYET
                                   ,MKD_TOPLAM_MALIYET              =@MKD_TOPLAM_MALIYET
                                   ,MKD_NEDEN_KOD_ID                =@MKD_NEDEN_KOD_ID      
                                   ,MKD_PLANLI                      =@MKD_PLANLI
                                   ,MKD_DEGISTIREN_ID               =@MKD_DEGISTIREN_ID
                                   ,MKD_DEGISTIRME_TARIH            =@MKD_DEGISTIRME_TARIH
                                   ,MKD_PROJE_ID                    =@MKD_PROJE_ID       
                                   ,MKD_LOKASYON_ID                 =@MKD_LOKASYON_ID 
                                   ,MKD_ACIKLAMA                    =@MKD_ACIKLAMA WHERE TB_MAKINE_DURUS_ID = @TB_MAKINE_DURUS_ID";
					prms.Clear();
					prms.Add("MKD_MAKINE_ID", entity.MKD_MAKINE_ID);
					prms.Add("MKD_BASLAMA_TARIH", entity.MKD_BASLAMA_TARIH);
					prms.Add("MKD_BASLAMA_SAAT", entity.MKD_BASLAMA_SAAT);
					prms.Add("MKD_BITIS_TARIH", entity.MKD_BITIS_TARIH);
					prms.Add("MKD_BITIS_SAAT", entity.MKD_BITIS_SAAT);
					prms.Add("MKD_SURE", entity.MKD_SURE);
					prms.Add("MKD_SAAT_MALIYET", entity.MKD_SAAT_MALIYET);
					prms.Add("MKD_TOPLAM_MALIYET", entity.MKD_TOPLAM_MALIYET);
					prms.Add("MKD_NEDEN_KOD_ID", entity.MKD_NEDEN_KOD_ID);
					prms.Add("MKD_PLANLI", entity.MKD_PLANLI);
					prms.Add("MKD_DEGISTIREN_ID", entity.MKD_DEGISTIREN_ID);
					prms.Add("MKD_DEGISTIRME_TARIH", entity.MKD_DEGISTIRME_TARIH);
					prms.Add("MKD_PROJE_ID", entity.MKD_PROJE_ID);
					prms.Add("MKD_LOKASYON_ID", entity.MKD_LOKASYON_ID);
					prms.Add("TB_MAKINE_DURUS_ID", entity.TB_MAKINE_DURUS_ID);
					prms.Add("MKD_ACIKLAMA", entity.MKD_ACIKLAMA);
					klas.cmd(query, prms.PARAMS);
					klas.kapat();

					durum = "Entity Updated Successfully ! ";
				}

				#endregion
				return Json(new { has_error = false, status_code = 200, status = durum });
			}
			catch (Exception ex)
			{
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}

		[Route("api/AddUpdateIsEmriPersonel")]
		[HttpPost]
		public object PersonelListKaydetYeni(IsEmriPersonel entity, long isEmriId)
		{
			string durum = "";
			parametreler.Clear();
			parametreler.Add(new Prm("IDK_ISEMRI_ID", isEmriId));
			parametreler.Add(new Prm("IDK_REF_ID", entity.IDK_REF_ID));
			try
			{
				if (entity.TB_ISEMRI_KAYNAK_ID < 1)
				{
					#region Kaydet

					string query = @"INSERT INTO orjin.TB_ISEMRI_KAYNAK
                                    (  IDK_ISEMRI_ID
                                      ,IDK_REF_ID   
                                      ,IDK_SURE
                                      ,IDK_SAAT_UCRETI
                                      ,IDK_MALIYET
                                      ,IDK_MASRAF_MERKEZI_ID
                                      ,IDK_ACIKLAMA
                                      ,IDK_FAZLA_MESAI_VAR
                                      ,IDK_FAZLA_MESAI_SURE
                                      ,IDK_FAZLA_MESAI_SAAT_UCRETI  
                                      ,IDK_VARDIYA                                   
                                      ,IDK_OLUSTURAN_ID
                                      ,IDK_OLUSTURMA_TARIH)
                                    VALUES (  
                                       @IDK_ISEMRI_ID
                                      ,@IDK_REF_ID   
                                      ,@IDK_SURE
                                      ,@IDK_SAAT_UCRETI
                                      ,@IDK_MALIYET
                                      ,@IDK_MASRAF_MERKEZI_ID
                                      ,@IDK_ACIKLAMA
                                      ,@IDK_FAZLA_MESAI_VAR
                                      ,@IDK_FAZLA_MESAI_SURE
                                      ,@IDK_FAZLA_MESAI_SAAT_UCRETI 
                                      ,@IDK_VARDIYA                                    
                                      ,@IDK_OLUSTURAN_ID
                                      ,@IDK_OLUSTURMA_TARIH)";
					prms.Clear();
					prms.Add("@IDK_ISEMRI_ID", isEmriId);
					prms.Add("@IDK_REF_ID", entity.IDK_REF_ID);
					prms.Add("@IDK_SURE", entity.IDK_SURE);
					prms.Add("@IDK_SAAT_UCRETI", entity.IDK_SAAT_UCRETI);
					prms.Add("@IDK_MALIYET", entity.IDK_MALIYET);
					prms.Add("@IDK_OLUSTURAN_ID", entity.IDK_OLUSTURAN_ID);
					prms.Add("@IDK_VARDIYA", entity.IDK_VARDIYA);
					prms.Add("@IDK_OLUSTURMA_TARIH", DateTime.Now);
					prms.Add("@IDK_MASRAF_MERKEZI_ID", entity.IDK_MASRAF_MERKEZI_ID);
					prms.Add("@IDK_ACIKLAMA", entity.IDK_ACIKLAMA);
					prms.Add("@IDK_FAZLA_MESAI_VAR", entity.IDK_FAZLA_MESAI_VAR);
					prms.Add("@IDK_FAZLA_MESAI_SURE", entity.IDK_FAZLA_MESAI_SURE);
					prms.Add("@IDK_FAZLA_MESAI_SAAT_UCRETI", entity.IDK_FAZLA_MESAI_SAAT_UCRETI);
					klas.cmd(query, prms.PARAMS);

					#endregion

					string qu =
						"UPDATE orjin.TB_ISEMRI SET ISM_MALIYET_PERSONEL = ISM_MALIYET_PERSONEL + Cast(@Maliyet AS FLOAT), ISM_MALIYET_TOPLAM =ISM_MALIYET_TOPLAM +  Cast(@Maliyet AS FLOAT) where TB_ISEMRI_ID = @IDK_ISEMRI_ID";
					prms.Clear();
					prms.Add("@Maliyet", entity.IDK_MALIYET);
					prms.Add("@IDK_ISEMRI_ID", isEmriId);
					klas.cmd(qu, prms.PARAMS);
					durum = "Entity Added Successfully !";
					// Bildirim Gönder
				}
				else
				{
					#region Guncelle

					string query = @" UPDATE orjin.TB_ISEMRI_KAYNAK set

                                       IDK_REF_ID = @IDK_REF_ID   
                                      ,IDK_SURE = @IDK_SURE
                                      ,IDK_SAAT_UCRETI = @IDK_SAAT_UCRETI
                                      ,IDK_MALIYET = @IDK_MALIYET
                                      ,IDK_MASRAF_MERKEZI_ID = @IDK_MASRAF_MERKEZI_ID
                                      ,IDK_ACIKLAMA = @IDK_ACIKLAMA
                                      ,IDK_FAZLA_MESAI_VAR = @IDK_FAZLA_MESAI_VAR
                                      ,IDK_FAZLA_MESAI_SURE = @IDK_FAZLA_MESAI_SURE
                                      ,IDK_FAZLA_MESAI_SAAT_UCRETI = @IDK_FAZLA_MESAI_SAAT_UCRETI  
                                      ,IDK_VARDIYA  = @IDK_VARDIYA                                 
                                      ,IDK_DEGISTIREN_ID = @IDK_DEGISTIREN_ID
                                      ,IDK_DEGISTIRME_TARIH = @IDK_DEGISTIRME_TARIH where TB_ISEMRI_KAYNAK_ID = @TB_ISEMRI_KAYNAK_ID ";
					prms.Clear();
					prms.Add("@TB_ISEMRI_KAYNAK_ID", entity.TB_ISEMRI_KAYNAK_ID);
					prms.Add("@IDK_REF_ID", entity.IDK_REF_ID);
					prms.Add("@IDK_SURE", entity.IDK_SURE);
					prms.Add("@IDK_SAAT_UCRETI", entity.IDK_SAAT_UCRETI);
					prms.Add("@IDK_MALIYET", entity.IDK_MALIYET);
					prms.Add("@IDK_DEGISTIREN_ID", entity.IDK_DEGISTIREN_ID);
					prms.Add("@IDK_VARDIYA", entity.IDK_VARDIYA);
					prms.Add("@IDK_DEGISTIRME_TARIH", DateTime.Now);
					prms.Add("@IDK_MASRAF_MERKEZI_ID", entity.IDK_MASRAF_MERKEZI_ID);
					prms.Add("@IDK_ACIKLAMA", entity.IDK_ACIKLAMA);
					prms.Add("@IDK_FAZLA_MESAI_VAR", entity.IDK_FAZLA_MESAI_VAR);
					prms.Add("@IDK_FAZLA_MESAI_SURE", entity.IDK_FAZLA_MESAI_SURE);
					prms.Add("@IDK_FAZLA_MESAI_SAAT_UCRETI", entity.IDK_FAZLA_MESAI_SAAT_UCRETI);
					klas.cmd(query, prms.PARAMS);

					#endregion

					string qu =
						"UPDATE orjin.TB_ISEMRI SET ISM_MALIYET_PERSONEL = ISM_MALIYET_PERSONEL + Cast(@Maliyet AS FLOAT), ISM_MALIYET_TOPLAM =ISM_MALIYET_TOPLAM +  Cast(@Maliyet AS FLOAT) where TB_ISEMRI_ID = @IDK_ISEMRI_ID";
					prms.Clear();
					prms.Add("@Maliyet", entity.IDK_MALIYET);
					prms.Add("@IDK_ISEMRI_ID", isEmriId);
					klas.cmd(qu, prms.PARAMS);
					durum = "Entity Updated Successfully ! ";
					// Bildirim Gönder
				}
				return Json(new { has_error = false, status_code = 200, status = durum });
			}
			catch (Exception ex)
			{
				klas.kapat();
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}

		}

		[Route("api/AddUpdateIsEmriAracGerec")]
		[HttpPost]
		public object AracGerecListKaydetYeni(IsEmriAracGerec entity, long isEmriId)
		{
			string durum = "";
			string query = "";
			try
			{
				if (entity.TB_ISEMRI_ARAC_GEREC_ID < 1)
				{
					#region Kaydet
					query = @" insert into orjin.TB_ISEMRI_ARAC_GEREC 
                           (IAG_ISEMRI_ID , 
                            IAG_ARAC_GEREC_ID , 
                            IAG_OLUSTURAN_ID , 
                            IAG_OLUSTURMA_TARIH , 
                            IAG_DEGISTIREN_ID , 
                            IAG_DEGISTIRME_TARIH ) 
                            values (
                                @IAG_ISEMRI_ID ,
                                @IAG_ARAC_GEREC_ID , 
                                @IAG_OLUSTURAN_ID , 
                                @IAG_OLUSTURMA_TARIH , 
                                @IAG_DEGISTIREN_ID , 
                                @IAG_DEGISTIRME_TARIH) ";
					prms.Clear();
					prms.Add("@IAG_ISEMRI_ID", isEmriId);
					prms.Add("@IAG_ARAC_GEREC_ID", entity.IAG_ARAC_GEREC_ID);
					prms.Add("@IAG_OLUSTURAN_ID", entity.IAG_OLUSTURAN_ID);
					prms.Add("@IAG_OLUSTURMA_TARIH", DateTime.Now);
					prms.Add("@IAG_DEGISTIREN_ID", entity.IAG_DEGISTIREN_ID);
					prms.Add("@IAG_DEGISTIRME_TARIH", DateTime.Now);
					#endregion
					klas.baglan();
					klas.cmd(query, prms.PARAMS);
					durum = "Entity Added Successfully !";
				}
				else
				{
					#region Guncelle
					query = @" update orjin.TB_ISEMRI_ARAC_GEREC set
                              IAG_ARAC_GEREC_ID = @IAG_ARAC_GEREC_ID 
                            , IAG_DEGISTIREN_ID = @IAG_DEGISTIREN_ID 
                            , IAG_DEGISTIRME_TARIH = @IAG_DEGISTIRME_TARIH 
                            where TB_ISEMRI_ARAC_GEREC_ID = @TB_ISEMRI_ARAC_GEREC_ID
                        ";

					prms.Clear();
					prms.Add("@TB_ISEMRI_ARAC_GEREC_ID", entity.TB_ISEMRI_ARAC_GEREC_ID);
					prms.Add("@IAG_ARAC_GEREC_ID", entity.IAG_ARAC_GEREC_ID);
					prms.Add("@IAG_DEGISTIREN_ID", entity.IAG_DEGISTIREN_ID);
					prms.Add("@IAG_DEGISTIRME_TARIH", DateTime.Now);
					#endregion
					klas.baglan();
					klas.cmd(query, prms.PARAMS);
					durum = "Entity Updated Successfully ! ";

				}
				return Json(new { has_error = false, status_code = 200, status = durum });
			}
			catch (Exception ex)
			{
				klas.kapat();
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}

		[Route("api/AddUpdateIsEmriOlcumDegeri")]
		[HttpPost]
		public object OlcumDegeriListKadetYeni(Olcum entity, long isEmriId)
		{
			string durum = "";
			string query = "";
			try
			{
				if (entity.TB_ISEMRI_OLCUM_ID < 1)
				{
					#region Kaydet
					query = @" insert into orjin.TB_ISEMRI_OLCUM 
                           (IDO_SIRANO , 
                            IDO_ISEMRI_ID , 
                            IDO_TANIM , 
                            IDO_BIRIM_KOD_ID,
                            IDO_FORMAT,
                            IDO_HEDEF_DEGER,
                            IDO_MIN_MAX_DEGER,
                            IDO_MIN_DEGER,
                            IDO_MAX_DEGER,
                            IDO_OLCUM_DEGER,
                            IDO_FARK,
                            IDO_DURUM,
                            IDO_TARIH,
                            IDO_SAAT,
                            IDO_OLUSTURAN_ID,
                            IDO_OLUSTURMA_TARIH,
                            IDO_DEGISTIREN_ID,
                            IDO_DEGISTIRME_TARIH,
                            IDO_REF_ID
                            ) 
                            values (
                                @IDO_SIRANO , 
                                @IDO_ISEMRI_ID , 
                                @IDO_TANIM , 
                                @IDO_BIRIM_KOD_ID, 
                                @IDO_FORMAT, 
                                @IDO_HEDEF_DEGER, 
                                @IDO_MIN_MAX_DEGER, 
                                @IDO_MIN_DEGER, 
                                @IDO_MAX_DEGER, 
                                @IDO_OLCUM_DEGER, 
                                @IDO_FARK,  
                                @IDO_DURUM, 
                                @IDO_TARIH, 
                                @IDO_SAAT, 
                                @IDO_OLUSTURAN_ID, 
                                @IDO_OLUSTURMA_TARIH, 
                                @IDO_DEGISTIREN_ID, 
                                @IDO_DEGISTIRME_TARIH, 
                                @IDO_REF_ID) ";
					prms.Clear();
					prms.Add("@IDO_SIRANO", entity.IDO_SIRANO);
					prms.Add("@IDO_ISEMRI_ID", isEmriId);
					prms.Add("@IDO_TANIM", entity.IDO_TANIM);
					prms.Add("@IDO_BIRIM_KOD_ID", entity.IDO_BIRIM_KOD_ID);
					prms.Add("@IDO_FORMAT", entity.IDO_FORMAT);
					prms.Add("@IDO_HEDEF_DEGER", entity.IDO_HEDEF_DEGER);
					prms.Add("@IDO_MIN_MAX_DEGER", entity.IDO_MIN_MAX_DEGER);
					prms.Add("@IDO_MIN_DEGER", entity.IDO_MIN_DEGER);
					prms.Add("@IDO_MAX_DEGER", entity.IDO_MAX_DEGER);
					prms.Add("@IDO_OLCUM_DEGER", entity.IDO_OLCUM_DEGER);
					prms.Add("@IDO_FARK", entity.IDO_FARK);
					prms.Add("@IDO_DURUM", entity.IDO_DURUM);
					prms.Add("@IDO_TARIH", entity.IDO_TARIH);
					prms.Add("@IDO_SAAT", entity.IDO_SAAT);
					prms.Add("@IDO_OLUSTURAN_ID", entity.IDO_OLUSTURAN_ID);
					prms.Add("@IDO_OLUSTURMA_TARIH", DateTime.Now);
					prms.Add("@IDO_DEGISTIREN_ID", entity.IDO_DEGISTIREN_ID);
					prms.Add("@IDO_DEGISTIRME_TARIH", DateTime.Now);
					prms.Add("@IDO_REF_ID", -1);
					#endregion

					klas.baglan();
					klas.cmd(query, prms.PARAMS);
					durum = "Entity Added Successfully !";
				}

				else
				{
					#region Guncelle
					query = @" update orjin.TB_ISEMRI_OLCUM  set
                             IDO_SIRANO  = @IDO_SIRANO   
                           , IDO_TANIM = @IDO_TANIM 
                           , IDO_BIRIM_KOD_ID = @IDO_BIRIM_KOD_ID
                           , IDO_FORMAT = @IDO_FORMAT
                           , IDO_HEDEF_DEGER = @IDO_HEDEF_DEGER
                           , IDO_MIN_MAX_DEGER = @IDO_MIN_MAX_DEGER
                           , IDO_MIN_DEGER = @IDO_MIN_DEGER
                           , IDO_MAX_DEGER = @IDO_MAX_DEGER
                           , IDO_OLCUM_DEGER = @IDO_OLCUM_DEGER
                           , IDO_FARK = @IDO_FARK
                           , IDO_DURUM = @IDO_DURUM
                           , IDO_TARIH = @IDO_TARIH
                           , IDO_SAAT = @IDO_SAAT
                           , IDO_DEGISTIREN_ID = @IDO_DEGISTIREN_ID
                           , IDO_DEGISTIRME_TARIH = @IDO_DEGISTIRME_TARIH
                           , IDO_REF_ID = @IDO_REF_ID where TB_ISEMRI_OLCUM_ID = @TB_ISEMRI_OLCUM_ID";
					prms.Clear();
					prms.Add("@IDO_SIRANO", entity.IDO_SIRANO);
					prms.Add("@TB_ISEMRI_OLCUM_ID", entity.TB_ISEMRI_OLCUM_ID);
					prms.Add("@IDO_TANIM", entity.IDO_TANIM);
					prms.Add("@IDO_BIRIM_KOD_ID", entity.IDO_BIRIM_KOD_ID);
					prms.Add("@IDO_FORMAT", entity.IDO_FORMAT);
					prms.Add("@IDO_HEDEF_DEGER", entity.IDO_HEDEF_DEGER);
					prms.Add("@IDO_MIN_MAX_DEGER", entity.IDO_MIN_MAX_DEGER);
					prms.Add("@IDO_MIN_DEGER", entity.IDO_MIN_DEGER);
					prms.Add("@IDO_MAX_DEGER", entity.IDO_MAX_DEGER);
					prms.Add("@IDO_OLCUM_DEGER", entity.IDO_OLCUM_DEGER);
					prms.Add("@IDO_FARK", entity.IDO_FARK);
					prms.Add("@IDO_DURUM", entity.IDO_DURUM);
					prms.Add("@IDO_TARIH", entity.IDO_TARIH);
					prms.Add("@IDO_SAAT", entity.IDO_SAAT);
					prms.Add("@IDO_DEGISTIREN_ID", entity.IDO_DEGISTIREN_ID);
					prms.Add("@IDO_DEGISTIRME_TARIH", DateTime.Now);
					prms.Add("@IDO_REF_ID", -1);
					#endregion

					klas.baglan();
					klas.cmd(query, prms.PARAMS);
					durum = "Entity Updated Successfully ! ";
				}
				return Json(new { has_error = false, status_code = 200, status = durum });
			}
			catch (Exception ex)
			{
				klas.kapat();
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}

		[Route("api/FetchIsEmriKontrolList")]
		[HttpGet]
		public List<IsEmriKontrolList> FetchIsEmriKontrolList([FromUri] int isemriID)
		{
			string rtfText, plainText;

			prms.Clear();
			prms.Add("ISM_ID", isemriID);
			string sql = "select * from orjin.VW_ISEMRI_KONTROLLIST where DKN_ISEMRI_ID = @ISM_ID";
			List<IsEmriKontrolList> listem = new List<IsEmriKontrolList>();
			DataTable dt = klas.GetDataTable(sql, prms.PARAMS);

			for (int i = 0; i < dt.Rows.Count; i++)
			{
				IsEmriKontrolList entity = new IsEmriKontrolList();
				entity.TB_ISEMRI_KONTROLLIST_ID = Convert.ToInt32(dt.Rows[i]["TB_ISEMRI_KONTROLLIST_ID"]);
				entity.DKN_SIRANO = dt.Rows[i]["DKN_SIRANO"] != DBNull.Value ? dt.Rows[i]["DKN_SIRANO"].ToString() : "";
				entity.DKN_TANIM = dt.Rows[i]["DKN_TANIM"] != DBNull.Value ? dt.Rows[i]["DKN_TANIM"].ToString() : "";
				entity.DKN_MALIYET = Convert.ToDouble(dt.Rows[i]["DKN_MALIYET"]);
				entity.DKN_ISEMRI_ID = Convert.ToInt32(dt.Rows[i]["DKN_ISEMRI_ID"]);
				entity.DKN_YAPILDI = Convert.ToBoolean(dt.Rows[i]["DKN_YAPILDI"]);
				entity.DKN_YAPILDI_SURE = Convert.ToInt32(dt.Rows[i]["DKN_YAPILDI_SURE"]);
				entity.DKN_YAPILDI_PERSONEL_ID = Convert.ToInt32(dt.Rows[i]["DKN_YAPILDI_PERSONEL_ID"]);
				entity.DKN_YAPILDI_ATOLYE_ID = Convert.ToInt32(dt.Rows[i]["DKN_YAPILDI_ATOLYE_ID"]);

				if (dt.Rows[i]["DKN_YAPILDI_TARIH"] != DBNull.Value)
					entity.DKN_YAPILDI_TARIH = (DateTime)dt.Rows[i]["DKN_YAPILDI_TARIH"];
				else
					entity.DKN_YAPILDI_TARIH = null;

				if (dt.Rows[i]["DKN_BITIS_TARIH"] != DBNull.Value)
					entity.DKN_BITIS_TARIH = (DateTime)dt.Rows[i]["DKN_BITIS_TARIH"];
				else
					entity.DKN_BITIS_TARIH = null;

				entity.DKN_YAPILDI_SAAT = dt.Rows[i]["DKN_YAPILDI_SAAT"] != DBNull.Value ? dt.Rows[i]["DKN_YAPILDI_SAAT"].ToString() : "";
				entity.DKN_PERSONEL_ISIM = dt.Rows[i]["DKN_PERSONEL_ISIM"] != DBNull.Value ? dt.Rows[i]["DKN_PERSONEL_ISIM"].ToString() : "";
				entity.DKN_ATOLYE_TANIM = dt.Rows[i]["DKN_ATOLYE_TANIM"] != DBNull.Value ? dt.Rows[i]["DKN_ATOLYE_TANIM"].ToString() : "";
				entity.DKN_BITIS_SAAT = dt.Rows[i]["DKN_BITIS_SAAT"] != DBNull.Value ? dt.Rows[i]["DKN_BITIS_SAAT"].ToString() : "";

				if (dt.Rows[i]["DKN_ACIKLAMA"] != DBNull.Value && dt.Rows[i]["DKN_ACIKLAMA"].ToString().StartsWith(@"{\rtf"))
				{

					System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox();
					rtfText = dt.Rows[i]["DKN_ACIKLAMA"].ToString();
					rtBox.Rtf = rtfText;
					plainText = rtBox.Text;
					entity.DKN_ACIKLAMA = plainText;
				}
				else { entity.DKN_ACIKLAMA = dt.Rows[i]["DKN_ACIKLAMA"] != DBNull.Value ? dt.Rows[i]["DKN_ACIKLAMA"].ToString() : ""; }


				listem.Add(entity);
			}

			return listem;
		}

		[Route("api/FetchIsEmriAracGerec")]
		[HttpGet]
		public Object FetchIsEmriAracGerec([FromUri] int isemriID)
		{
			string query = " select ARG.* from orjin.TB_ISEMRI_ARAC_GEREC IAG " +
				$" left join orjin.VW_ARAC_GEREC ARG on IAG.IAG_ARAC_GEREC_ID = ARG.TB_ARAC_GEREC_ID where IAG_ISEMRI_ID = {isemriID} ";
			List<AracGerec> listem = new List<AracGerec>();
			try
			{
				using (var cnn = klas.baglan())
				{
					listem = cnn.Query<AracGerec>(query).ToList();
				}
				return Json(new { ARAC_GEREC_LISTE = listem });
			}
			catch (Exception ex)
			{
				return Json(new { error = ex.Message });
			}
		}


		[Route("api/FetchIsEmriOlcumDegeri")]
		[HttpGet]
		public Object FetchIsEmriOlcumDegeri([FromUri] int isemriID)
		{
			string query = " select ido.*, kod.KOD_TANIM as IDO_BIRIM from orjin.TB_ISEMRI_OLCUM ido " +
				$" left join orjin.TB_KOD kod on kod.TB_KOD_ID = ido.IDO_BIRIM_KOD_ID where IDO_ISEMRI_ID = {isemriID} ";
			List<Olcum> listem = new List<Olcum>();
			try
			{
				using (var cnn = klas.baglan())
				{
					listem = cnn.Query<Olcum>(query).ToList();
				}
				return Json(new { OLCUM_DEGER_LISTE = listem });
			}
			catch (Exception ex)
			{
				return Json(new { error = ex.Message });
			}
		}

		[Route("api/FetchIsEmriDurusList")]
		[HttpGet]
		public List<IsEmriDurus> FetchIsEmriDurusList([FromUri] int isemriID)
		{
			var util = new Util();
			var prms = new DynamicParameters();
			string sql = @"SELECT  
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
                                                FROM orjin.VW_MAKINE_DURUS MKD
                                                LEFT JOIN orjin.TB_LOKASYON L ON L.TB_LOKASYON_ID=MKD.MKD_LOKASYON_ID
                                                LEFT JOIN orjin.TB_MAKINE M ON M.TB_MAKINE_ID=MKD.MKD_MAKINE_ID
                                                LEFT JOIN orjin.TB_PROJE P ON P.TB_PROJE_ID=MKD.MKD_PROJE_ID
                                                 WHERE 1=1 ";

			if (isemriID > 0)
			{
				sql += " AND MKD.MKD_ISEMRI_ID=@ISM_ID";
				prms.Add("ISM_ID", isemriID);
			}

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

		[HttpPost]
		[Route("api/UpdateIsEmri")]
		public object UpdateIsEmri([FromBody] IsEmri entity)
		{
			var util = new Util();
			using (var cnn = util.baglan())
			{
				try
				{
					if (entity.TB_ISEMRI_ID > 0)
					{
						#region Guncelle

						string query = @"UPDATE orjin.TB_ISEMRI SET
                                       ISM_ISEMRI_NO =@ISM_ISEMRI_NO 
                                      ,ISM_MAKINE_ID = @ISM_MAKINE_ID                                      
                                      ,ISM_BASLAMA_TARIH = @ISM_BASLAMA_TARIH
                                      ,ISM_BASLAMA_SAAT = @ISM_BASLAMA_SAAT
                                      ,ISM_DUZENLEME_TARIH = @ISM_DUZENLEME_TARIH
                                      ,ISM_DUZENLEME_SAAT = @ISM_DUZENLEME_SAAT
                                      ,ISM_BITIS_TARIH = @ISM_BITIS_TARIH
                                      ,ISM_BITIS_SAAT = @ISM_BITIS_SAAT                             
                                      ,ISM_PLAN_BASLAMA_TARIH   = @ISM_PLAN_BASLAMA_TARIH
                                      ,ISM_PLAN_BASLAMA_SAAT    = @ISM_PLAN_BASLAMA_SAAT
                                      ,ISM_PLAN_BITIS_TARIH     = @ISM_PLAN_BITIS_TARIH
                                      ,ISM_PLAN_BITIS_SAAT      = @ISM_PLAN_BITIS_SAAT
                                      ,ISM_KONU = @ISM_KONU
                                      ,ISM_ACIKLAMA = @ISM_ACIKLAMA
                                      ,ISM_LOKASYON_ID = @ISM_LOKASYON_ID
                                      ,ISM_PROJE_ID=@ISM_PROJE_ID   
                                      ,ISM_TIP_KOD_ID=@ISM_TIP_KOD_ID
                                      ,ISM_ONCELIK_ID=@ISM_ONCELIK_ID
                                      ,ISM_ATOLYE_ID=@ISM_ATOLYE_ID
                                      ,ISM_MASRAF_MERKEZ_ID=@ISM_MASRAF_MERKEZ_ID
                                      ,ISM_REF_ID=@ISM_REF_ID
                                      ,ISM_REF_GRUP=@ISM_REF_GRUP
                                      ,ISM_TIP_ID = @ISM_TIP_ID 
                                      ,ISM_DEGISTIREN_ID=@ISM_DEGISTIREN_ID
                                      ,ISM_DEGISTIRME_TARIH=@ISM_DEGISTIRME_TARIH
                                      ,ISM_SURE_CALISMA = @ISM_SURE_CALISMA 
                                      ,ISM_DURUM_KOD_ID=@ISM_DURUM_KOD_ID
                                      ,ISM_SAYAC_DEGER=@ISM_SAYAC_DEGER
                                      ,ISM_BILDIREN = @ISM_BILDIREN                             
                                      ,ISM_MAKINE_DURUM_KOD_ID = @ISM_MAKINE_DURUM_KOD_ID
                                      ,ISM_MAKINE_GUVENLIK_NOTU = @ISM_MAKINE_GUVENLIK_NOTU
                                      ,ISM_IS_TARIH = @ISM_IS_TARIH
                                      ,ISM_IS_SAAT = @ISM_IS_SAAT
                                      ,ISM_KAPATILDI = @ISM_KAPATILDI
                                      ,ISM_TAMAMLANMA_ORAN = @ISM_TAMAMLANMA_ORAN
                                      ,ISM_FIRMA_ID = @ISM_FIRMA_ID
                                      ,ISM_FIRMA_SOZLESME_ID = @ISM_FIRMA_SOZLESME_ID
                                      ,ISM_NEDEN_KOD_ID = @ISM_NEDEN_KOD_ID
                                      ,ISM_TALIMAT_ID = @ISM_TALIMAT_ID
                                      ,ISM_GARANTI_KAPSAMINDA = @ISM_GARANTI_KAPSAMINDA
                                      ,ISM_EKIPMAN_ID = @ISM_EKIPMAN_ID
                                      ,ISM_MALIYET_MLZ = @ISM_MALIYET_MLZ
                                      ,ISM_MALIYET_PERSONEL = @ISM_MALIYET_PERSONEL
                                      ,ISM_MALIYET_DISSERVIS = @ISM_MALIYET_DISSERVIS
                                      ,ISM_MALIYET_DIGER = @ISM_MALIYET_DIGER
                                      ,ISM_MALIYET_INDIRIM = @ISM_MALIYET_INDIRIM
                                      ,ISM_MALIYET_KDV = @ISM_MALIYET_KDV 
                                      ,ISM_MALIYET_TOPLAM = @ISM_MALIYET_TOPLAM
                                      ,ISM_SURE_MUDAHALE_LOJISTIK = @ISM_SURE_MUDAHALE_LOJISTIK
                                      ,ISM_SURE_MUDAHALE_SEYAHAT = @ISM_SURE_MUDAHALE_SEYAHAT
                                      ,ISM_SURE_MUDAHALE_ONAY = @ISM_SURE_MUDAHALE_ONAY
                                      ,ISM_SURE_BEKLEME = @ISM_SURE_BEKLEME
                                      ,ISM_SURE_MUDAHALE_DIGER = @ISM_SURE_MUDAHALE_DIGER
                                      ,ISM_SURE_PLAN_MUDAHALE = @ISM_SURE_PLAN_MUDAHALE
                                      ,ISM_SURE_PLAN_CALISMA = @ISM_SURE_PLAN_CALISMA
                                      ,ISM_SURE_TOPLAM = @ISM_SURE_TOPLAM
                                      ,ISM_BAGLI_ISEMRI_ID = @ISM_BAGLI_ISEMRI_ID
                                      ,ISM_EVRAK_NO = @ISM_EVRAK_NO
                                      ,ISM_EVRAK_TARIHI = @ISM_EVRAK_TARIHI
                                      ,ISM_REFERANS_NO = @ISM_REFERANS_NO
                                      ,ISM_TAKVIM_ID = @ISM_TAKVIM_ID
                                      ,ISM_OZEL_ALAN_1  =@ISM_OZEL_ALAN_1
                                      ,ISM_OZEL_ALAN_2  =@ISM_OZEL_ALAN_2
                                      ,ISM_OZEL_ALAN_3  =@ISM_OZEL_ALAN_3
                                      ,ISM_OZEL_ALAN_4  =@ISM_OZEL_ALAN_4
                                      ,ISM_OZEL_ALAN_5  =@ISM_OZEL_ALAN_5
                                      ,ISM_OZEL_ALAN_6  =@ISM_OZEL_ALAN_6
                                      ,ISM_OZEL_ALAN_7  =@ISM_OZEL_ALAN_7
                                      ,ISM_OZEL_ALAN_8  =@ISM_OZEL_ALAN_8
                                      ,ISM_OZEL_ALAN_9  =@ISM_OZEL_ALAN_9
                                      ,ISM_OZEL_ALAN_10 =@ISM_OZEL_ALAN_10
                                      ,ISM_OZEL_ALAN_11_KOD_ID = @ISM_OZEL_ALAN_11_KOD_ID
                                      ,ISM_OZEL_ALAN_12_KOD_ID = @ISM_OZEL_ALAN_12_KOD_ID
                                      ,ISM_OZEL_ALAN_13_KOD_ID = @ISM_OZEL_ALAN_13_KOD_ID
                                      ,ISM_OZEL_ALAN_14_KOD_ID = @ISM_OZEL_ALAN_14_KOD_ID
                                      ,ISM_OZEL_ALAN_15_KOD_ID = @ISM_OZEL_ALAN_15_KOD_ID
                                      ,ISM_OZEL_ALAN_16 =@ISM_OZEL_ALAN_16
                                      ,ISM_OZEL_ALAN_17 =@ISM_OZEL_ALAN_17
                                      ,ISM_OZEL_ALAN_18 =@ISM_OZEL_ALAN_18
                                      ,ISM_OZEL_ALAN_19 =@ISM_OZEL_ALAN_19
                                      ,ISM_OZEL_ALAN_20 =@ISM_OZEL_ALAN_20
                                       WHERE TB_ISEMRI_ID = @TB_ISEMRI_ID";
						prms.Clear();
						prms.Add("@TB_ISEMRI_ID", entity.TB_ISEMRI_ID);
						prms.Add("@ISM_ISEMRI_NO", entity.ISM_ISEMRI_NO);
						prms.Add("@ISM_BASLAMA_TARIH", entity.ISM_BASLAMA_TARIH);
						prms.Add("@ISM_BASLAMA_SAAT", entity.ISM_BASLAMA_SAAT);
						prms.Add("@ISM_BITIS_TARIH", entity.ISM_BITIS_TARIH);
						prms.Add("@ISM_BITIS_SAAT", entity.ISM_BITIS_SAAT);
						prms.Add("@ISM_PLAN_BASLAMA_TARIH", entity.ISM_PLAN_BASLAMA_TARIH);
						prms.Add("@ISM_PLAN_BASLAMA_SAAT", entity.ISM_PLAN_BASLAMA_SAAT);
						prms.Add("@ISM_PLAN_BITIS_TARIH", entity.ISM_PLAN_BITIS_TARIH);
						prms.Add("@ISM_PLAN_BITIS_SAAT", entity.ISM_PLAN_BITIS_SAAT);
						prms.Add("@ISM_DUZENLEME_TARIH", entity.ISM_DUZENLEME_TARIH);
						prms.Add("@ISM_DUZENLEME_SAAT", entity.ISM_DUZENLEME_SAAT);
						prms.Add("@ISM_KONU", entity.ISM_KONU);
						prms.Add("@ISM_ACIKLAMA", entity.ISM_ACIKLAMA);
						prms.Add("@ISM_MAKINE_ID", entity.ISM_MAKINE_ID);
						prms.Add("@ISM_LOKASYON_ID", entity.ISM_LOKASYON_ID);
						prms.Add("@ISM_PROJE_ID", entity.ISM_PROJE_ID);
						prms.Add("@ISM_TIP_KOD_ID", entity.ISM_TIP_KOD_ID);
						prms.Add("@ISM_ONCELIK_ID", entity.ISM_ONCELIK_ID);
						prms.Add("@ISM_ATOLYE_ID", entity.ISM_ATOLYE_ID);
						prms.Add("@ISM_MASRAF_MERKEZ_ID", entity.ISM_MASRAF_MERKEZ_ID);
						prms.Add("@ISM_REF_ID", entity.ISM_REF_ID);
						prms.Add("@ISM_REF_GRUP", entity.ISM_REF_GRUP);
						prms.Add("@ISM_TIP_ID", entity.ISM_TIP_ID);
						prms.Add("@ISM_DEGISTIREN_ID", entity.ISM_DEGISTIREN_ID);
						prms.Add("@ISM_SURE_CALISMA", entity.ISM_SURE_CALISMA);
						prms.Add("@ISM_DEGISTIRME_TARIH", DateTime.Now);
						prms.Add("@ISM_DURUM_KOD_ID", entity.ISM_DURUM_KOD_ID);
						prms.Add("@ISM_SAYAC_DEGER", entity.ISM_SAYAC_DEGER);
						prms.Add("@ISM_BILDIREN", entity.ISM_BILDIREN);
						prms.Add("@ISM_MAKINE_DURUM_KOD_ID", entity.ISM_MAKINE_DURUM_KOD_ID);
						prms.Add("@ISM_MAKINE_GUVENLIK_NOTU", entity.ISM_MAKINE_GUVENLIK_NOTU);
						prms.Add("@ISM_IS_TARIH", entity.ISM_IS_TARIH);
						prms.Add("@ISM_IS_SAAT", entity.ISM_IS_SAAT);
						prms.Add("@ISM_KAPATILDI", entity.ISM_KAPATILDI);
						prms.Add("@ISM_TAMAMLANMA_ORAN", entity.ISM_TAMAMLANMA_ORAN);
						prms.Add("@ISM_FIRMA_ID", entity.ISM_FIRMA_ID);
						prms.Add("@ISM_FIRMA_SOZLESME_ID", entity.ISM_FIRMA_SOZLESME_ID);
						prms.Add("@ISM_NEDEN_KOD_ID", entity.ISM_NEDEN_KOD_ID);
						prms.Add("@ISM_TALIMAT_ID", entity.ISM_TALIMAT_ID);
						prms.Add("@ISM_GARANTI_KAPSAMINDA", entity.ISM_GARANTI_KAPSAMINDA);
						prms.Add("@ISM_EKIPMAN_ID", entity.ISM_EKIPMAN_ID);
						prms.Add("@ISM_MALIYET_MLZ", entity.ISM_MALIYET_MLZ);
						prms.Add("@ISM_MALIYET_PERSONEL", entity.ISM_MALIYET_PERSONEL);
						prms.Add("@ISM_MALIYET_DISSERVIS", entity.ISM_MALIYET_DISSERVIS);
						prms.Add("@ISM_MALIYET_DIGER", entity.ISM_MALIYET_DIGER);
						prms.Add("@ISM_MALIYET_INDIRIM", entity.ISM_MALIYET_INDIRIM);
						prms.Add("@ISM_MALIYET_KDV", entity.ISM_MALIYET_KDV);
						prms.Add("@ISM_MALIYET_TOPLAM", entity.ISM_MALIYET_TOPLAM);
						prms.Add("@ISM_SURE_MUDAHALE_LOJISTIK", entity.ISM_SURE_MUDAHALE_LOJISTIK);
						prms.Add("@ISM_SURE_MUDAHALE_SEYAHAT", entity.ISM_SURE_MUDAHALE_SEYAHAT);
						prms.Add("@ISM_SURE_MUDAHALE_ONAY", entity.ISM_SURE_MUDAHALE_ONAY);
						prms.Add("@ISM_SURE_BEKLEME", entity.ISM_SURE_BEKLEME);
						prms.Add("@ISM_SURE_MUDAHALE_DIGER", entity.ISM_SURE_MUDAHALE_DIGER);
						prms.Add("@ISM_SURE_PLAN_MUDAHALE", entity.ISM_SURE_PLAN_MUDAHALE);
						prms.Add("@ISM_SURE_PLAN_CALISMA", entity.ISM_SURE_PLAN_CALISMA);
						prms.Add("@ISM_SURE_TOPLAM", entity.ISM_SURE_TOPLAM);
						prms.Add("@ISM_BAGLI_ISEMRI_ID", entity.ISM_BAGLI_ISEMRI_ID);
						prms.Add("@ISM_EVRAK_NO", entity.ISM_EVRAK_NO);
						prms.Add("@ISM_EVRAK_TARIHI", entity.ISM_EVRAK_TARIHI);
						prms.Add("@ISM_REFERANS_NO", entity.ISM_REFERANS_NO);
						prms.Add("@ISM_TAKVIM_ID", entity.ISM_TAKVIM_ID);
						prms.Add("@ISM_OZEL_ALAN_1", entity.ISM_OZEL_ALAN_1);
						prms.Add("@ISM_OZEL_ALAN_2", entity.ISM_OZEL_ALAN_2);
						prms.Add("@ISM_OZEL_ALAN_3", entity.ISM_OZEL_ALAN_3);
						prms.Add("@ISM_OZEL_ALAN_4", entity.ISM_OZEL_ALAN_4);
						prms.Add("@ISM_OZEL_ALAN_5", entity.ISM_OZEL_ALAN_5);
						prms.Add("@ISM_OZEL_ALAN_6", entity.ISM_OZEL_ALAN_6);
						prms.Add("@ISM_OZEL_ALAN_7", entity.ISM_OZEL_ALAN_7);
						prms.Add("@ISM_OZEL_ALAN_8", entity.ISM_OZEL_ALAN_8);
						prms.Add("@ISM_OZEL_ALAN_9", entity.ISM_OZEL_ALAN_9);
						prms.Add("@ISM_OZEL_ALAN_10", entity.ISM_OZEL_ALAN_10);
						prms.Add("@ISM_OZEL_ALAN_11_KOD_ID", entity.ISM_OZEL_ALAN_11_KOD_ID);
						prms.Add("@ISM_OZEL_ALAN_12_KOD_ID", entity.ISM_OZEL_ALAN_12_KOD_ID);
						prms.Add("@ISM_OZEL_ALAN_13_KOD_ID", entity.ISM_OZEL_ALAN_13_KOD_ID);
						prms.Add("@ISM_OZEL_ALAN_14_KOD_ID", entity.ISM_OZEL_ALAN_14_KOD_ID);
						prms.Add("@ISM_OZEL_ALAN_15_KOD_ID", entity.ISM_OZEL_ALAN_15_KOD_ID);
						prms.Add("@ISM_OZEL_ALAN_16", entity.ISM_OZEL_ALAN_16);
						prms.Add("@ISM_OZEL_ALAN_17", entity.ISM_OZEL_ALAN_17);
						prms.Add("@ISM_OZEL_ALAN_18", entity.ISM_OZEL_ALAN_18);
						prms.Add("@ISM_OZEL_ALAN_19", entity.ISM_OZEL_ALAN_19);
						prms.Add("@ISM_OZEL_ALAN_20", entity.ISM_OZEL_ALAN_20);
						klas.cmd(query, prms.PARAMS);

						#endregion
						return Json(new { has_error = false, status_code = 200, status = "Entity Updated Successfully" });
					}

					return Json(new { has_error = false, status_code = 400, status = "Bad Request ( entity may be null or 0 lentgh)" });
				}
				catch (Exception ex)
				{
					return Json(new { has_error = true, status_code = 500, status = ex.Message });
				}
			}
		}


		private void StokHareketIslemi(IsEmriMalzeme entity)
		{
			try
			{
				prms.Clear();
				prms.Add("IDM_STOK_ID", entity.IDM_STOK_ID);
				prms.Add("IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
				int deger = Convert.ToInt32(klas.GetDataCell(
					"select count(*) from orjin.TB_STOK_HRK WHERE SHR_REF_GRUP='ISEMRI_MLZ' AND SHR_STOK_ID=@IDM_STOK_ID  and SHR_REF_ID=@IDM_ISEMRI_ID",
					prms.PARAMS));
				if (deger < 1)
				{
					//hareket kaydı
					string query = @"INSERT INTO orjin.TB_STOK_HRK
                               (SHR_STOK_FIS_DETAY_ID
                               ,SHR_STOK_ID
                               ,SHR_BIRIM_KOD_ID
                               ,SHR_DEPO_ID
                               ,SHR_TARIH
                               ,SHR_SAAT
                               ,SHR_MIKTAR
                               ,SHR_ANA_BIRIM_MIKTAR
                               ,SHR_BIRIM_FIYAT
                               ,SHR_KDV_ORAN
                               ,SHR_KDV_TUTAR
                               ,SHR_OTV_ORAN
                               ,SHR_OTV_TUTAR
                               ,SHR_INDIRIM_ORAN
                               ,SHR_INDIRIM_TUTAR
                               ,SHR_KDV_DH
                               ,SHR_ARA_TOPLAM
                               ,SHR_TOPLAM
                               ,SHR_HRK_ACIKLAMA
                               ,SHR_ACIKLAMA
                               ,SHR_GC
                               ,SHR_REF_ID
                               ,SHR_REF_GRUP
                               ,SHR_PARABIRIMI_ID
                               ,SHR_OLUSTURAN_ID
                               ,SHR_OLUSTURMA_TARIH)
                               VALUES (-1
                                       ,@SHR_STOK_ID
                                       ,@SHR_BIRIM_KOD_ID
                                       ,@SHR_DEPO_ID
                                       ,@SHR_TARIH
                                       ,@SHR_SAAT
                                       ,@SHR_MIKTAR
                                       ,@SHR_ANA_BIRIM_MIKTAR
                                       ,@SHR_BIRIM_FIYAT
                                       ,0
                                       ,0
                                       ,0
                                       ,0
                                       ,0
                                       ,0
                                       ,0
                                       ,@SHR_ARA_TOPLAM
                                       ,@SHR_TOPLAM
                                       ,@SHR_HRK_ACIKLAMA
                                       ,@SHR_ACIKLAMA
                                       ,@SHR_GC
                                       ,@SHR_REF_ID
                                       ,@SHR_REF_GRUP
                                       ,-1
                                       ,@SHR_OLUSTURAN_ID
                                       ,@SHR_OLUSTURMA_TARIH)";
					prms.Clear();
					prms.Add("IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
					string ISM_NO =
						klas.GetDataCell(
							"SELECT TOP 1 ISM_ISEMRI_NO FROM orjin.TB_ISEMRI WHERE TB_ISEMRI_ID = @IDM_ISEMRI_ID",
							prms.PARAMS);
					prms.Clear();

					prms.Add("@SHR_STOK_ID", entity.IDM_STOK_ID);
					prms.Add("@SHR_BIRIM_KOD_ID", entity.IDM_BIRIM_KOD_ID);
					prms.Add("@SHR_DEPO_ID", entity.IDM_DEPO_ID);
					prms.Add("@SHR_TARIH", entity.IDM_TARIH);
					prms.Add("@SHR_SAAT", entity.IDM_SAAT);
					prms.Add("@SHR_MIKTAR", entity.IDM_MIKTAR);
					prms.Add("@SHR_ANA_BIRIM_MIKTAR", entity.IDM_MIKTAR);
					prms.Add("@SHR_BIRIM_FIYAT", entity.IDM_BIRIM_FIYAT);
					prms.Add("@SHR_ARA_TOPLAM", entity.IDM_TUTAR);
					prms.Add("@SHR_TOPLAM", entity.IDM_TUTAR);
					prms.Add("@SHR_HRK_ACIKLAMA", ISM_NO + " nolu iş emri - malzeme kullanımı");
					prms.Add("@SHR_ACIKLAMA", ISM_NO + " nolu iş emri - malzeme kullanımı");
					prms.Add("@SHR_GC", "C");
					prms.Add("@SHR_REF_ID", entity.IDM_ISEMRI_ID);
					prms.Add("@SHR_REF_GRUP", "ISEMRI_MLZ");
					prms.Add("@SHR_OLUSTURAN_ID", entity.IDM_OLUSTURAN_ID);
					prms.Add("@SHR_OLUSTURMA_TARIH", DateTime.Now);
					klas.cmd(query, prms.PARAMS);

					// depo stok güncelleniyor            
					string qu1 =
						"UPDATE orjin.TB_DEPO_STOK SET DPS_CIKAN_MIKTAR = DPS_CIKAN_MIKTAR + Cast(@Miktar AS FLOAT), DPS_KULLANILABILIR_MIKTAR = DPS_KULLANILABILIR_MIKTAR - Cast(@Miktar AS FLOAT), DPS_MIKTAR = DPS_MIKTAR - Cast(@Miktar AS FLOAT)  WHERE DPS_DEPO_ID = @IDM_DEPO_ID  AND DPS_STOK_ID = @IDM_STOK_ID";
					prms.Clear();
					prms.Add("@Miktar", entity.IDM_MIKTAR);
					prms.Add("@IDM_DEPO_ID", entity.IDM_DEPO_ID);
					prms.Add("@IDM_STOK_ID", entity.IDM_STOK_ID);
					klas.cmd(qu1, prms.PARAMS);
					// stok güncelleniyor.
					string qu2 =
						"UPDATE orjin.TB_STOK SET STK_CIKAN_MIKTAR = STK_CIKAN_MIKTAR + Cast(@Miktar AS FLOAT), STK_MIKTAR = STK_MIKTAR - Cast(@Miktar AS FLOAT)  WHERE TB_STOK_ID = @IDM_STOK_ID";
					prms.Clear();
					prms.Add("@Miktar", entity.IDM_MIKTAR);
					prms.Add("@IDM_STOK_ID", entity.IDM_STOK_ID);
					klas.cmd(qu2, prms.PARAMS);
					klas.kapat();
				}
				else
				{
					prms.Clear();
					prms.Add("IDM_STOK_ID", entity.IDM_STOK_ID);
					prms.Add("IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
					double eskiMiktar = Convert.ToDouble(klas.GetDataCell(
						"select coalesce(SHR_MIKTAR,0) from orjin.TB_STOK_HRK WHERE SHR_REF_GRUP='ISEMRI_MLZ' AND SHR_STOK_ID=@IDM_STOK_ID and SHR_REF_ID = @IDM_ISEMRI_ID",
						prms.PARAMS));
					double yeniMiktar = entity.IDM_MIKTAR - eskiMiktar;
					//hareket kaydı
					string query = @"UPDATE orjin.TB_STOK_HRK SET
                                SHR_STOK_ID                            = @SHR_STOK_ID
                               ,SHR_BIRIM_KOD_ID                       = @SHR_BIRIM_KOD_ID
                               ,SHR_DEPO_ID                            = @SHR_DEPO_ID
                               ,SHR_TARIH                              = @SHR_TARIH
                               ,SHR_SAAT                               = @SHR_SAAT
                               ,SHR_MIKTAR                             = @SHR_MIKTAR
                               ,SHR_ANA_BIRIM_MIKTAR                   = @SHR_ANA_BIRIM_MIKTAR
                               ,SHR_BIRIM_FIYAT                        = @SHR_BIRIM_FIYAT
                               ,SHR_ARA_TOPLAM                         = @SHR_ARA_TOPLAM
                               ,SHR_TOPLAM                             = @SHR_TOPLAM
                               ,SHR_HRK_ACIKLAMA                       = (SELECT TOP 1 ISM_ISEMRI_NO FROM orjin.TB_ISEMRI WHERE TB_ISEMRI_ID =@SHR_REF_ID) + ' nolu iş emri - malzeme kullanımı' 
                               ,SHR_ACIKLAMA                           = (SELECT TOP 1 ISM_ISEMRI_NO FROM orjin.TB_ISEMRI WHERE TB_ISEMRI_ID =@SHR_REF_ID) + ' nolu iş emri - malzeme kullanımı'
                               ,SHR_GC                                 = @SHR_GC
                               ,SHR_REF_ID                             = @SHR_REF_ID
                               ,SHR_REF_GRUP                           = @SHR_REF_GRUP
                               ,SHR_DEGISTIREN_ID                      = @SHR_DEGISTIREN_ID
                               ,SHR_DEGISTIRME_TARIH                   = @SHR_DEGISTIRME_TARIH where SHR_REF_GRUP='ISEMRI_MLZ' AND SHR_STOK_ID = @SHR_STOK_ID and SHR_REF_ID=@SHR_REF_ID";

					prms.Clear();
					prms.Add("@SHR_STOK_ID", entity.IDM_STOK_ID);
					prms.Add("@SHR_BIRIM_KOD_ID", entity.IDM_BIRIM_KOD_ID);
					prms.Add("@SHR_DEPO_ID", entity.IDM_DEPO_ID);
					prms.Add("@SHR_TARIH", entity.IDM_TARIH);
					prms.Add("@SHR_SAAT", entity.IDM_SAAT);
					prms.Add("@SHR_MIKTAR", entity.IDM_MIKTAR);
					prms.Add("@SHR_ANA_BIRIM_MIKTAR", entity.IDM_MIKTAR);
					prms.Add("@SHR_BIRIM_FIYAT", entity.IDM_BIRIM_FIYAT);
					prms.Add("@SHR_ARA_TOPLAM", entity.IDM_TUTAR);
					prms.Add("@SHR_TOPLAM", entity.IDM_TUTAR);
					prms.Add("@SHR_GC", "C");
					prms.Add("@SHR_REF_ID", entity.IDM_ISEMRI_ID);
					prms.Add("@SHR_REF_GRUP", "ISEMRI_MLZ");
					prms.Add("@SHR_DEGISTIREN_ID", entity.IDM_DEGISTIREN_ID);
					prms.Add("@SHR_DEGISTIRME_TARIH", DateTime.Now);
					klas.cmd(query, prms.PARAMS);
					// depo stok güncelleniyor                   
					string qu1 =
						"UPDATE orjin.TB_DEPO_STOK SET DPS_CIKAN_MIKTAR = DPS_CIKAN_MIKTAR + Cast(@Miktar AS FLOAT), DPS_KULLANILABILIR_MIKTAR = DPS_KULLANILABILIR_MIKTAR - Cast(@Miktar AS FLOAT), DPS_MIKTAR = DPS_MIKTAR - Cast(@Miktar AS FLOAT)  WHERE DPS_DEPO_ID = @IDM_DEPO_ID  AND DPS_STOK_ID = @IDM_STOK_ID";
					prms.Clear();
					prms.Add("@Miktar", yeniMiktar);
					prms.Add("@IDM_DEPO_ID", entity.IDM_DEPO_ID);
					prms.Add("@IDM_STOK_ID", entity.IDM_STOK_ID);
					klas.cmd(qu1, prms.PARAMS);

					// stok güncelleniyor.
					string qu2 =
						"UPDATE orjin.TB_STOK SET STK_CIKAN_MIKTAR = STK_CIKAN_MIKTAR + Cast(@Miktar AS FLOAT), STK_MIKTAR = STK_MIKTAR - Cast(@Miktar AS FLOAT)  WHERE TB_STOK_ID = @IDM_STOK_ID";
					prms.Clear();
					prms.Add("@Miktar", yeniMiktar);
					prms.Add("@IDM_STOK_ID", entity.IDM_STOK_ID);
					klas.cmd(qu2, prms.PARAMS);
				}
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}


