using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using Dapper;
using Newtonsoft.Json.Linq;
using WebApiNew.Filters;
using WebApiNew.Models;
using WebApiNew.Utility.Abstract;


/*
 * 
 * 
 * 
 *      IS EMRI Controller For Web App Versions
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
		private readonly System.Windows.Forms.RichTextBox RTB = new System.Windows.Forms.RichTextBox();
		private readonly ILogger _logger;
		string query = "";
		SqlCommand cmd = null;

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
							isemritipQuery += " ISM_IS_EMRI_TIPI LIKE '%" + property.Value + "%' ";

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
							durumQuery += " ISM_DURUM LIKE '%" + property.Value + "%' ";

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
							lokasyonQuery += " ISM_LOKASYON LIKE '%" + property.Value + "%' ";
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
							customfilterQuery += $" {property.Key} LIKE '%" + property.Value + "%'";
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
						MAIN_QUERY += $" AND ( ISM_KAPALI  LIKE '%" + parametre + "%' or " +
									  $" ISM_ISEMRI_NO  LIKE '%" + parametre + "%' or " +
									  $" ISM_KONU  LIKE '%" + parametre + "%' or " +
									  $" ISM_IS_EMRI_TIPI  LIKE '%" + parametre + "%' or " +
									  $" ISM_DURUM  LIKE '%" + parametre + "%' or " +
									  $" ISM_LOKASYON  LIKE '%" + parametre + "%' or " +
									  $" ISM_MAKINE_KODU  LIKE '%" + parametre + "%' or " +
									  $" ISM_MAKINE_TANIMI  LIKE '%" + parametre + "%' or " +
									  $" ISM_IS_TIPI  LIKE '%" + parametre + "%' or " +
									  $" ISM_IS_NEDENI  LIKE '%" + parametre + "%' or " +
									  $" ISM_ATOLYE  LIKE '%" + parametre + "%' or " +
									  $" ISM_PERSONEL_ADI LIKE '%" + parametre + "%' ) ";

						isParametreForTotalIsEmri = $" AND ( ISM_KAPALI  LIKE '%" + parametre + "%' or " +
									  $" ISM_ISEMRI_NO  LIKE '%" + parametre + "%' or " +
									  $" ISM_KONU  LIKE '%" + parametre + "%' or " +
									  $" ISM_IS_EMRI_TIPI  LIKE '%" + parametre + "%' or " +
									  $" ISM_DURUM  LIKE '%" + parametre + "%' or " +
									  $" ISM_LOKASYON  LIKE '%" + parametre + "%' or " +
									  $" ISM_MAKINE_KODU  LIKE '%" + parametre + "%' or " +
									  $" ISM_MAKINE_TANIMI  LIKE '%" + parametre + "%' or " +
									  $" ISM_IS_TIPI  LIKE '%" + parametre + "%' or " +
									  $" ISM_IS_NEDENI  LIKE '%" + parametre + "%' or " +
									  $" ISM_ATOLYE  LIKE '%" + parametre + "%' or " +
									  $" ISM_PERSONEL_ADI LIKE '%" + parametre + "%' ) ";
					}

					if (isemritipleri != null && isemritipleri.Count != 0) MAIN_QUERY += " AND " + isemritipQuery;
					if (durumlar != null && durumlar.Count != 0) MAIN_QUERY += " AND " + durumQuery;
					if (lokasyonlar != null && lokasyonlar.Count != 0) MAIN_QUERY += " AND " + lokasyonQuery;
					if (customfilter != null && customfilter.Count != 0) MAIN_QUERY += " AND " + customfilterQuery;

					MAIN_QUERY += @" ) as SubRow where SubRow.RowIndex >= @PAGING_ILK_DEGER and SubRow.RowIndex < @PAGING_SON_DEGER ";

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

					MAIN_QUERY = @" SELECT * FROM ( SELECT ROW_NUMBER() OVER (ORDER BY TB_ISEMRI_ID DESC) as RowIndex , * FROM VW_WEB_VERSION_ISEMRI ) as SubRow 
		                               where SubRow.RowIndex >= @PAGING_ILK_DEGER and SubRow.RowIndex < @PAGING_SON_DEGER ";
					TOTAL_IS_EMRI_QUERY = " select count(*) from (select * from dbo.VW_WEB_VERSION_ISEMRI where 1=1) as TotalIsEmriSayisi ";
				}
				prms.Clear();
				prms.Add("ISEMRI_ID", id);
				prms.Add("PAGING_ILK_DEGER", pagingIlkDeger);
				prms.Add("PAGING_SON_DEGER", pagingSonDeger);
				DataTable dt = klas.GetDataTable(MAIN_QUERY, prms.PARAMS);
				for (var i = 0; i < dt.Rows.Count; i++)
				{
					WebVersionIsEmriModel entity = new WebVersionIsEmriModel();
					entity.TB_ISEMRI_ID = (int)dt.Rows[i]["TB_ISEMRI_ID"];
					entity.KAPALI = dt.Rows[i]["ISM_KAPALI"] != DBNull.Value ? (bool)dt.Rows[i]["ISM_KAPALI"] : false;
					entity.ONCELIK = dt.Rows[i]["ISM_ONCELIK"] != DBNull.Value ? (string)dt.Rows[i]["ISM_ONCELIK"] : ""; ;
					entity.BELGE = dt.Rows[i]["ISM_BELGE"] != DBNull.Value ? (int)dt.Rows[i]["ISM_BELGE"] : 0;
					entity.RESIM = dt.Rows[i]["ISM_RESIM"] != DBNull.Value ? (int)dt.Rows[i]["ISM_RESIM"] : 0;
					entity.ISEMRI_NO = dt.Rows[i]["ISM_ISEMRI_NO"] != DBNull.Value ? (string)dt.Rows[i]["ISM_ISEMRI_NO"] : "";
					entity.MALZEME = dt.Rows[i]["ISM_MALZEME"] != DBNull.Value ? (int)dt.Rows[i]["ISM_MALZEME"] : 0;
					entity.PERSONEL = dt.Rows[i]["ISM_PERSONEL"] != DBNull.Value ? (int)dt.Rows[i]["ISM_PERSONEL"] : 0;
					entity.DURUS = dt.Rows[i]["ISM_DURUS"] != DBNull.Value ? (int)dt.Rows[i]["ISM_DURUS"] : 0;
					entity.OUTER_NOT = dt.Rows[i]["ISM_DISARIDAKI_NOT"] != DBNull.Value ? (string)dt.Rows[i]["ISM_DISARIDAKI_NOT"] : "";
					entity.DUZENLEME_TARIH = dt.Rows[i]["ISM_DUZENLEME_TARIH"] != DBNull.Value ? (DateTime)dt.Rows[i]["ISM_DUZENLEME_TARIH"] : DateTime.MinValue;
					entity.DUZENLEME_SAAT = dt.Rows[i]["ISM_DUZENLEME_SAAT"] != DBNull.Value ? (string)dt.Rows[i]["ISM_DUZENLEME_SAAT"] : "";
					entity.KONU = dt.Rows[i]["ISM_KONU"] != DBNull.Value ? (string)dt.Rows[i]["ISM_KONU"] : "";
					entity.ISEMRI_TIP = dt.Rows[i]["ISM_IS_EMRI_TIPI"] != DBNull.Value ? (string)dt.Rows[i]["ISM_IS_EMRI_TIPI"] : "";
					entity.DURUM = dt.Rows[i]["ISM_DURUM"] != DBNull.Value ? (string)dt.Rows[i]["ISM_DURUM"] : "";
					entity.LOKASYON = dt.Rows[i]["ISM_LOKASYON"] != DBNull.Value ? (string)dt.Rows[i]["ISM_LOKASYON"] : "";
					entity.PLAN_BASLAMA_TARIH = dt.Rows[i]["ISM_PLAN_BASLAMA_TARIH"] != DBNull.Value ? (DateTime)dt.Rows[i]["ISM_PLAN_BASLAMA_TARIH"] : DateTime.MinValue;
					entity.PLAN_BASLAMA_SAAT = dt.Rows[i]["ISM_PLAN_BASLAMA_SAAT"] != DBNull.Value ? (string)dt.Rows[i]["ISM_PLAN_BASLAMA_SAAT"] : "";
					entity.PLAN_BITIS_TARIH = dt.Rows[i]["ISM_PLAN_BITIS_TARIH"] != DBNull.Value ? (DateTime)dt.Rows[i]["ISM_PLAN_BITIS_TARIH"] : DateTime.MinValue;
					entity.PLAN_BITIS_SAAT = dt.Rows[i]["ISM_PLAN_BITIS_SAAT"] != DBNull.Value ? (string)dt.Rows[i]["ISM_PLAN_BITIS_SAAT"] : "";
					entity.BASLAMA_TARIH = dt.Rows[i]["ISM_BASLAMA_TARIH"] != DBNull.Value ? (DateTime)dt.Rows[i]["ISM_BASLAMA_TARIH"] : DateTime.MinValue;
					entity.BASLAMA_SAAT = dt.Rows[i]["ISM_BASLAMA_SAAT"] != DBNull.Value ? (string)dt.Rows[i]["ISM_BASLAMA_SAAT"] : "";
					entity.ISM_BITIS_TARIH = dt.Rows[i]["ISM_BITIS_TARIH"] != DBNull.Value ? (DateTime)dt.Rows[i]["ISM_BITIS_TARIH"] : DateTime.MinValue;
					entity.ISM_BITIS_SAAT = dt.Rows[i]["ISM_BITIS_SAAT"] != DBNull.Value ? (string)dt.Rows[i]["ISM_BITIS_SAAT"] : "";
					entity.IS_SURESI = dt.Rows[i]["ISM_IS_SURESI"] != DBNull.Value ? (int)dt.Rows[i]["ISM_IS_SURESI"] : 0;
					entity.TAMAMLANMA = dt.Rows[i]["ISM_TAMAMLANMA"] != DBNull.Value ? (int)dt.Rows[i]["ISM_TAMAMLANMA"] : 0;
					entity.GARANTI = dt.Rows[i]["ISM_GARANTI"] != DBNull.Value ? (bool)dt.Rows[i]["ISM_GARANTI"] : false;
					entity.MAKINE_KODU = dt.Rows[i]["ISM_MAKINE_KODU"] != DBNull.Value ? (string)dt.Rows[i]["ISM_MAKINE_KODU"] : "";
					entity.MAKINE_TANIMI = dt.Rows[i]["ISM_MAKINE_TANIMI"] != DBNull.Value ? (string)dt.Rows[i]["ISM_MAKINE_TANIMI"] : "";
					entity.MAKINE_PLAKA = dt.Rows[i]["ISM_MAKINE_PLAKA"] != DBNull.Value ? (string)dt.Rows[i]["ISM_MAKINE_PLAKA"] : "";
					entity.MAKINE_DURUM = dt.Rows[i]["ISM_MAKINE_DURUM"] != DBNull.Value ? (string)dt.Rows[i]["ISM_MAKINE_DURUM"] : "";
					entity.MAKINE_TIP = dt.Rows[i]["ISM_MAKINE_TIP"] != DBNull.Value ? (string)dt.Rows[i]["ISM_MAKINE_TIP"] : "";
					entity.EKIPMAN = dt.Rows[i]["ISM_EKIPMAN"] != DBNull.Value ? (string)dt.Rows[i]["ISM_EKIPMAN"] : "";
					entity.IS_TIPI = dt.Rows[i]["ISM_IS_TIPI"] != DBNull.Value ? (string)dt.Rows[i]["ISM_IS_TIPI"] : "";
					entity.IS_NEDENI = dt.Rows[i]["ISM_IS_NEDENI"] != DBNull.Value ? (string)dt.Rows[i]["ISM_IS_NEDENI"] : "";
					entity.ATOLYE = dt.Rows[i]["ISM_ATOLYE"] != DBNull.Value ? (string)dt.Rows[i]["ISM_ATOLYE"] : "";
					entity.TALIMAT = dt.Rows[i]["ISM_TALIMAT"] != DBNull.Value ? (string)dt.Rows[i]["ISM_TALIMAT"] : "";
					entity.KAPANIS_TARIHI = dt.Rows[i]["ISM_KAPANIS_TARIHI"] != DBNull.Value ? (DateTime)dt.Rows[i]["ISM_KAPANIS_TARIHI"] : DateTime.MinValue;
					entity.KAPANIS_SAATI = dt.Rows[i]["ISM_KAPANIS_SAATI"] != DBNull.Value ? (string)dt.Rows[i]["ISM_KAPANIS_SAATI"] : "";
					entity.TAKVIM = dt.Rows[i]["ISM_TAKVIM"] != DBNull.Value ? (string)dt.Rows[i]["ISM_TAKVIM"] : "";
					entity.MASRAF_MERKEZI = dt.Rows[i]["ISM_MASRAF_MERKEZI"] != DBNull.Value ? (string)dt.Rows[i]["ISM_MASRAF_MERKEZI"] : "";
					entity.FRIMA = dt.Rows[i]["ISM_FIRMA"] != DBNull.Value ? (string)dt.Rows[i]["ISM_FIRMA"] : "";
					entity.IS_TALEP_NO = dt.Rows[i]["ISM_IS_TALEP_KOD"] != DBNull.Value ? (string)dt.Rows[i]["ISM_IS_TALEP_KOD"] : "";
					entity.IS_TALEP_EDEN = dt.Rows[i]["ISM_IS_TALEP_EDEN"] != DBNull.Value ? (string)dt.Rows[i]["ISM_IS_TALEP_EDEN"] : "";
					entity.IS_TALEP_TARIH = dt.Rows[i]["ISM_IS_TALEP_TARIH"] != DBNull.Value ? (DateTime)dt.Rows[i]["ISM_IS_TALEP_TARIH"] : DateTime.MinValue;
					entity.OZEL_ALAN_1 = dt.Rows[i]["ISM_OZEL_ALAN_1"] != DBNull.Value ? (string)dt.Rows[i]["ISM_OZEL_ALAN_1"] : "";
					entity.OZEL_ALAN_2 = dt.Rows[i]["ISM_OZEL_ALAN_2"] != DBNull.Value ? (string)dt.Rows[i]["ISM_OZEL_ALAN_2"] : "";
					entity.OZEL_ALAN_3 = dt.Rows[i]["ISM_OZEL_ALAN_3"] != DBNull.Value ? (string)dt.Rows[i]["ISM_OZEL_ALAN_3"] : "";
					entity.OZEL_ALAN_4 = dt.Rows[i]["ISM_OZEL_ALAN_4"] != DBNull.Value ? (string)dt.Rows[i]["ISM_OZEL_ALAN_4"] : "";
					entity.OZEL_ALAN_5 = dt.Rows[i]["ISM_OZEL_ALAN_5"] != DBNull.Value ? (string)dt.Rows[i]["ISM_OZEL_ALAN_5"] : "";
					entity.OZEL_ALAN_6 = dt.Rows[i]["ISM_OZEL_ALAN_6"] != DBNull.Value ? (string)dt.Rows[i]["ISM_OZEL_ALAN_6"] : "";
					entity.OZEL_ALAN_7 = dt.Rows[i]["ISM_OZEL_ALAN_7"] != DBNull.Value ? (string)dt.Rows[i]["ISM_OZEL_ALAN_7"] : "";
					entity.OZEL_ALAN_8 = dt.Rows[i]["ISM_OZEL_ALAN_8"] != DBNull.Value ? (string)dt.Rows[i]["ISM_OZEL_ALAN_8"] : "";
					entity.OZEL_ALAN_9 = dt.Rows[i]["ISM_OZEL_ALAN_9"] != DBNull.Value ? (string)dt.Rows[i]["ISM_OZEL_ALAN_9"] : "";
					entity.OZEL_ALAN_10 = dt.Rows[i]["ISM_OZEL_ALAN_10"] != DBNull.Value ? (string)dt.Rows[i]["ISM_OZEL_ALAN_10"] : "";
					entity.OZEL_ALAN_11 = dt.Rows[i]["ISM_OZEL_ALAN_11"] != DBNull.Value ? (string)dt.Rows[i]["ISM_OZEL_ALAN_11"] : "";
					entity.OZEL_ALAN_12 = dt.Rows[i]["ISM_OZEL_ALAN_12"] != DBNull.Value ? (string)dt.Rows[i]["ISM_OZEL_ALAN_12"] : "";
					entity.OZEL_ALAN_13 = dt.Rows[i]["ISM_OZEL_ALAN_13"] != DBNull.Value ? (string)dt.Rows[i]["ISM_OZEL_ALAN_13"] : "";
					entity.OZEL_ALAN_14 = dt.Rows[i]["ISM_OZEL_ALAN_14"] != DBNull.Value ? (string)dt.Rows[i]["ISM_OZEL_ALAN_14"] : "";
					entity.OZEL_ALAN_15 = dt.Rows[i]["ISM_OZEL_ALAN_15"] != DBNull.Value ? (string)dt.Rows[i]["ISM_OZEL_ALAN_15"] : "";
					entity.OZEL_ALAN_16 = dt.Rows[i]["ISM_OZEL_ALAN_16"] != DBNull.Value ? (double)dt.Rows[i]["ISM_OZEL_ALAN_16"] : 0;
					entity.OZEL_ALAN_17 = dt.Rows[i]["ISM_OZEL_ALAN_17"] != DBNull.Value ? (double)dt.Rows[i]["ISM_OZEL_ALAN_17"] : 0;
					entity.OZEL_ALAN_18 = dt.Rows[i]["ISM_OZEL_ALAN_18"] != DBNull.Value ? (double)dt.Rows[i]["ISM_OZEL_ALAN_18"] : 0;
					entity.OZEL_ALAN_19 = dt.Rows[i]["ISM_OZEL_ALAN_19"] != DBNull.Value ? (double)dt.Rows[i]["ISM_OZEL_ALAN_19"] : 0;
					entity.OZEL_ALAN_20 = dt.Rows[i]["ISM_OZEL_ALAN_20"] != DBNull.Value ? (double)dt.Rows[i]["ISM_OZEL_ALAN_20"] : 0;
					entity.BILDIRILEN_KAT = dt.Rows[i]["ISM_BILDIRILEN_KAT"] != DBNull.Value ? (int)dt.Rows[i]["ISM_BILDIRILEN_KAT"] : 0;
					entity.BILDIRILEN_BINA = dt.Rows[i]["ISM_BILDIRILEN_BINA"] != DBNull.Value ? (int)dt.Rows[i]["ISM_BILDIRILEN_BINA"] : 0;
					entity.PERSONEL_ADI = dt.Rows[i]["ISM_PERSONEL_ADI"] != DBNull.Value ? (string)dt.Rows[i]["ISM_PERSONEL_ADI"] : "";
					entity.TAM_LOKASYON = dt.Rows[i]["ISM_TAM_LOKASYON"] != DBNull.Value ? (string)dt.Rows[i]["ISM_TAM_LOKASYON"] : "";
					entity.GUNCEL_SAYAC_DEGER = dt.Rows[i]["ISM_GUNCEL_SAYAC_DEGER"] != DBNull.Value ? (int)dt.Rows[i]["ISM_GUNCEL_SAYAC_DEGER"] : 0;
					entity.ICERDEKI_NOT = dt.Rows[i]["ISM_ICERDEKI_NOT"] != DBNull.Value ? (string)dt.Rows[i]["ISM_ICERDEKI_NOT"] : "";

					listem.Add(entity);
				}

				cmd = new SqlCommand(TOTAL_IS_EMRI_QUERY, klas.baglan());
				toplamIsEmriSayisi = (int)cmd.ExecuteScalar();

				klas.kapat();
				return Json(new { page = (int)Math.Ceiling((decimal)toplamIsEmriSayisi / 50), list = listem });
			}
			catch (Exception e)
			{
				klas.kapat();
				return Json(e.Message);
			}
		}

		//Get Is Emri Durum List For Web App Version
		[Route("api/GetIsEmriDurum")]
		[HttpGet]
		public Object GetIsEmriDurum()
		{
			string query = @"SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP=32801";
			var klas = new Util();
			List<Kod> listem = new List<Kod>();
			using (var cnn = klas.baglan())
			{
				listem = cnn.Query<Kod>(query).ToList();
			}

			return Json(new { isEmriDurumlari = listem});
		}



		//Add Is Emri Durum Web For App Version
		[Route("api/AddIsEmriDurum")]
		[HttpPost]
		public Object AddIsEmriDurum([FromUri] string yeniDurum)
		{
			try
			{
				query = " insert into orjin.TB_KOD (KOD_GRUP , KOD_TANIM , KOD_AKTIF , KOD_GOR , KOD_DEGISTIR , KOD_SIL ) ";
				query += $" values ( 32801 , '{yeniDurum}' , 1 , 1 , 1 ,1 ) ";

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


		//Is Emri Durum Varsayilan Yap
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
				return Json(new { success = "Ayarlama başarılı ! " });
			}
			catch (Exception e)
			{
				klas.kapat();
				return Json(new { error = " Ayarlama başarısız ! " });
			}

		}

		//Add Is Emri Durum Degisikligi Web App Version
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

					klas.cmd(query, prms.PARAMS);
					klas.kapat();
					return Json(new { success = "Ekleme başarılı " });
				}
				else
				{
					return null;

				}
			} 
			catch (Exception ex) 
			{
				klas.kapat();
				return Json(new { error = " Ekleme başarısız " });
			}
		}


		//Get Is Tipi
		[Route("api/GetIsTipi")]
		[HttpGet]
		public Object GetIsTipi()
		{
			string query = @"SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP=32440";
			var klas = new Util();
			List<Kod> listem = new List<Kod>();
			using (var cnn = klas.baglan())
			{
				listem = cnn.Query<Kod>(query).ToList();
			}

			return Json(new { is_tipi = listem });
		}

		//Add Is Tipi
		[Route("api/AddIsTipi")]
		[HttpGet]
		public Object AddIsTipi([FromUri] string isTipi)
		{
			try
			{
				query = " insert into orjin.TB_KOD (KOD_GRUP , KOD_TANIM , KOD_AKTIF , KOD_GOR , KOD_DEGISTIR , KOD_SIL ) ";
				query += $" values ( 32440 , '{isTipi}' , 1 , 1 , 1 ,1 ) ";

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

		//Get Is Emri Nedeni
		[Route("api/GetIsEmriNedeni")]
		[HttpGet]
		public Object GetIsEmriNedeni()
		{
			string query = @"SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP=32452";
			var klas = new Util();
			List<Kod> listem = new List<Kod>();
			using (var cnn = klas.baglan())
			{
				listem = cnn.Query<Kod>(query).ToList();
			}

			return Json(new { is_emri_nedeni = listem });
		}

		//Add Is Emri Nedeni
		[Route("api/AddIsEmriNedeni")]
		[HttpGet]
		public Object AddIsEmriNedeni([FromUri] string isNedeni)
		{
			try
			{
				query = " insert into orjin.TB_KOD (KOD_GRUP , KOD_TANIM , KOD_AKTIF , KOD_GOR , KOD_DEGISTIR , KOD_SIL ) ";
				query += $" values ( 32452 , '{isNedeni}' , 1 , 1 , 1 ,1 ) ";

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

		//Get Is Emri Ozel Alanlar Content (11,12,13,14,15)
		[Route("api/GetIsEmriOzelAlanlar")]
		[HttpGet]
		public Object GetIsEmriOzelAlanlar([FromUri] int KodGrup)
		{
			string query = $"SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP={KodGrup}";
			var klas = new Util();
			List<Kod> listem = new List<Kod>();
			using (var cnn = klas.baglan())
			{
				listem = cnn.Query<Kod>(query).ToList();
			}

			return Json(new { is_emri_ozel_alanlar = listem });
		}

		//Add Ozel Alanlar Content (11,12,13,14,15)
		[Route("api/AddOzelAlanlarContent")]
		[HttpGet]
		public Object AddOzelAlanlarContent([FromUri] string content, [FromUri] int kodGrup)
		{
			try
			{
				query = " insert into orjin.TB_KOD (KOD_GRUP , KOD_TANIM , KOD_AKTIF , KOD_GOR , KOD_DEGISTIR , KOD_SIL ) ";
				query += $" values ( {kodGrup} , '{content}' , 1 , 1 , 1 ,1 ) ";

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


