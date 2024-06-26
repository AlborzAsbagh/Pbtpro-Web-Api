﻿using System;
using System.Collections.Generic;
using System.Data;
using WebApiNew.Utility.Abstract;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json.Linq;
using WebApiNew.Filters;
using WebApiNew.Models;


/*
 * 
 * 
 * 
 *      IS TALEP Controller For Web App Versions
 *      ( Mobil ve Web Arasındaki ortak fonksiyonlar kendi kontrollerinde yazılmıştır . )
 *
 *
 *
 */


namespace WebApiNew.Controllers
{

	[JwtAuthenticationFilter]
	public class WebAppVersionIsTalepController : ApiController
	{
		private readonly ILogger _logger;
		Util klas = new Util();
		string query = "";
		SqlCommand cmd = null;
		Parametreler prms = new Parametreler();
		List<Prm> parametreler = new List<Prm>();
		YetkiController yetki = new YetkiController();

		public WebAppVersionIsTalepController(ILogger logger)
		{
			_logger = logger;
		}

		[Route("api/GetIsTalepFullList")]
		[HttpPost]
		public object GetIsTalepFullList([FromUri] string parametre, [FromBody] JObject filters, [FromUri] int pagingDeger = 1 , 
			[FromUri] int? lokasyonId = 0 , [FromUri] int? pageSize = 10)
		{
			int pagingIlkDeger = (int)(pagingDeger == 1 ? 1 : ((pagingDeger * pageSize) - pageSize));
			int pagingSonDeger = (int)(pagingIlkDeger == 1 ? pageSize : pagingDeger * pageSize );
			int toplamIsTalepSayisi = 0;
			int counter = 0;
			string toplamIsTalepSayisiQuery = "";

			List<IsTalep> listem = new List<IsTalep>();
			try
			{
				query = Queries.IST_FETCH_QUERY;
				toplamIsTalepSayisiQuery = Queries.IST_FETCH_COUNT_QUERY;

				if (!string.IsNullOrEmpty(parametre))
				{
					query += $" and ( tlp.IST_KOD like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" and ( tlp.IST_KOD like '%{parametre}%' or ";
					query += $" tlp.IST_TANIMI like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" tlp.IST_TANIMI like '%{parametre}%' or ";
					query += $" lok.LOK_TANIM like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" lok.LOK_TANIM like '%{parametre}%' or ";
					query += $" mkn.MKN_KOD like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" mkn.MKN_KOD like '%{parametre}%' or ";
					query += $" tlp.IST_ACIKLAMA like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" tlp.IST_ACIKLAMA like '%{parametre}%' or ";
					query += $" isk.ISK_ISIM like '%{parametre}%' ) "; toplamIsTalepSayisiQuery += $" isk.ISK_ISIM like '%{parametre}%' ) ";
				}

				if ((filters["customfilters"] as JObject) != null && (filters["customfilters"] as JObject).Count > 0)
				{
					query += " and ( ";
					toplamIsTalepSayisiQuery += " and ( ";
					counter = 0;
					foreach (var property in filters["customfilters"] as JObject)
					{
						if (property.Key == "startDate")
						{
							query += $" tlp.IST_ACILIS_TARIHI >= '{property.Value}' ";
							toplamIsTalepSayisiQuery += $" tlp.IST_ACILIS_TARIHI >= '{property.Value}' ";
						}
						else if (property.Key == "endDate")
						{
							query += $" tlp.IST_ACILIS_TARIHI <= '{property.Value}' ";
							toplamIsTalepSayisiQuery += $" tlp.IST_ACILIS_TARIHI <= '{property.Value}' ";
						}
						else
						{
							query += $" {property.Key} LIKE '%{property.Value}%' ";
							toplamIsTalepSayisiQuery += $" {property.Key} LIKE '%{property.Value}%' ";
						}

						if (counter < (filters["customfilters"] as JObject).Count - 1)
						{
							query += " and ";
							toplamIsTalepSayisiQuery += " and ";
						}
						counter++;
					}
					query += " ) ";
					toplamIsTalepSayisiQuery += " ) ";
				}

				if (lokasyonId > 0 && lokasyonId != null)
				{
					query += $" and tlp.IST_BILDIREN_LOKASYON_ID = {lokasyonId} ";
					toplamIsTalepSayisiQuery += $" and tlp.IST_BILDIREN_LOKASYON_ID = {lokasyonId} ";
				}

				if( (filters["durumlar"] as JObject) != null && (filters["durumlar"] as JObject).Count > 0)
				{
					query += " and (  ";
					toplamIsTalepSayisiQuery += " and (  ";
					counter = 0;
					foreach (var property in filters["durumlar"] as JObject)
					{
						query += $" tlp.IST_DURUM_ID LIKE '%{property.Value}%' ";
						toplamIsTalepSayisiQuery += $" tlp.IST_DURUM_ID LIKE '%{property.Value}%' ";

						if (counter < (filters["durumlar"] as JObject).Count - 1)
						{
							query += " or ";
							toplamIsTalepSayisiQuery += " or ";
						}
						counter++;
					}
					query += " ) ";
					toplamIsTalepSayisiQuery += " ) ";
				}

				query += $" ) SELECT * FROM RowNumberedResults WHERE RowIndex BETWEEN {pagingIlkDeger} AND {pagingSonDeger - 1};";
				toplamIsTalepSayisiQuery += ") as TotalIsTalepSayisi";

				using (var cnn = klas.baglan())
				{
					listem = cnn.Query<IsTalep>(query , new { @KUL_ID = UserInfo.USER_ID}).ToList();
					toplamIsTalepSayisi = cnn.QueryFirstOrDefault<Int32>(toplamIsTalepSayisiQuery, new { @KUL_ID = UserInfo.USER_ID});
				}
				klas.kapat();
				return Json(new { page = (int)Math.Ceiling((decimal)((decimal)toplamIsTalepSayisi / pageSize)), is_talep_listesi = listem, kayit_sayisi = toplamIsTalepSayisi });

			}
			catch (Exception e)
			{
				klas.kapat();
				return Json(new { error = e.Message });
			}
		}



		[Route("api/AddIsTalep")]
		[HttpPost]
		public async Task<object> AddIsTalep([FromBody] JObject entity)
		{
			if (!(Boolean)yetki.isAuthorizedToAdd(PagesAuthCodes.IS_TALEPLERI_TANIMLARI))
				return Json(new { has_error = true, status_code = 401, status = "Unathorized to add !" });

			int count = 0;
			try
			{
				
				using (var cnn = klas.baglan())
				{
					var isemritipId = cnn.QueryFirstOrDefault<int>("SELECT TOP 1 ISP_ISEMRI_TIPI_ID FROM orjin.TB_IS_TALEBI_PARAMETRE");
					if (entity != null && entity.Count > 0)
					{
						query = " insert into orjin.TB_IS_TALEBI  ( IST_OLUSTURMA_TARIH , IST_ISEMRI_TIP_ID , IST_OLUSTURAN_ID , ";
						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" {item.Key} , ";
							else query += $" {item.Key} ";
							count++;
						}

						query += $" ) values ( '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , {isemritipId} , {UserInfo.USER_ID} , ";
						count = 0;

						foreach (var item in entity)
						{
							
							if (count < entity.Count - 1) query += $" '{item.Value}' , ";
							else query += $" '{item.Value}' ";
							count++;
						}
						query += " ) ";
						await cnn.ExecuteAsync(query);

						int SonTalepId = cnn.QueryFirstOrDefault<int>("SELECT TOP 1 TB_IS_TALEP_ID FROM orjin.TB_IS_TALEBI ORDER BY  TB_IS_TALEP_ID DESC");
						if (SonTalepId > 0)
						{
							var itl = new IsTalebiLog
							{
								ITL_IS_TANIM_ID = SonTalepId,
								ITL_KULLANICI_ID = Convert.ToInt32(entity.GetValue("IST_TALEP_EDEN_ID")),
								ITL_TARIH = Convert.ToDateTime(entity.GetValue("IST_ACILIS_TARIHI")),
								ITL_SAAT = Convert.ToString(entity.GetValue("IST_ACILIS_SAATI")),
								ITL_ISLEM = "Yeni iş talebi",
								ITL_ACIKLAMA = String.Format("Talep no: '{0}' - Konu :'{1}'", Convert.ToString(entity.GetValue("IST_KOD")),
									Convert.ToString(entity.GetValue("IST_TANIMI"))),
								ITL_ISLEM_DURUM = "AÇIK",
								ITL_TALEP_ISLEM = "Yeni İş Talebi",
								ITL_OLUSTURAN_ID = Convert.ToInt32(entity.GetValue("IST_OLUSTURAN_ID")),
								ITL_OLUSTURMA_TARIH = DateTime.Now
							};
							cnn.Insert(itl);
						}

						return Json(new { has_error = false, status_code = 201, status = " Added Successfully " });
					}
					else return Json(new { has_error = false, status_code = 400, status = "Bad Request ( entity may be null or 0 lentgh)" });
				}
			}
			catch (Exception ex)
			{
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}


		[Route("api/UpdateIsTalep")]
		[HttpPost]
		public async Task<Object> UpdateIsTalep([FromBody] JObject entity)
		{
			if (!(Boolean)yetki.isAuthorizedToUpdate(PagesAuthCodes.IS_TALEPLERI_TANIMLARI))
				return Json(new { has_error = true, status_code = 401, status = "Unathorized to update !" });

			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_IS_TALEP_ID")) >= 1)
					{
						query = " update orjin.TB_IS_TALEBI set ";
						foreach (var item in entity)
						{

							if (item.Key.Equals("TB_IS_TALEP_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , IST_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , IST_DEGISTIREN_ID = {UserInfo.USER_ID} ";
						query += $" where TB_IS_TALEP_ID = {Convert.ToInt32(entity.GetValue("TB_IS_TALEP_ID"))}";

						await cnn.ExecuteAsync(query);

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


		
		[Route("api/GetIsTalepById")]
		[HttpGet]
		public object GetIsTalepById([FromUri] int isTalepId)
		{
			Util klas = new Util();
			List<IsTalep> listem = new List<IsTalep>();
			string query = @"select * from orjin.VW_IS_TALEP where TB_IS_TALEP_ID = @TB_IS_TALEP_ID";
			using (var conn = klas.baglan())
			{
				listem = conn.Query<IsTalep>(query, new { @TB_IS_TALEP_ID = isTalepId }).ToList();
			}
			return listem;
		}
		
		// Add one by one
		public void IsTalepIptalKapatProcess(IsTalepIptalKapatModel entity)
		{
			string isTalepLogQuery =
				@"
                    insert into orjin.TB_IS_TALEBI_LOG (
                    ITL_IS_TANIM_ID,
                    ITL_KULLANICI_ID,
                    ITL_TARIH,
                    ITL_SAAT,
                    ITL_ISLEM,
                    ITL_ACIKLAMA,
                    ITL_ISLEM_DURUM,
                    ITL_TALEP_ISLEM,
                    ITL_OLUSTURAN_ID ) values (";

			isTalepLogQuery += $" {entity.TB_IS_TALEP_ID} , ";
			isTalepLogQuery += $" {UserInfo.USER_ID} , ";
			isTalepLogQuery += $" '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , "; // Changed date format
			isTalepLogQuery += $" '{DateTime.Now.ToString("HH:mm:ss")}' , ";
			isTalepLogQuery += $" '{entity.ITL_ISLEM}' , ";
			isTalepLogQuery += $" '{entity.ITL_ACIKLAMA}' , ";
			isTalepLogQuery += $" '{entity.ITL_ISLEM_DURUM}' , ";
			isTalepLogQuery += $" '{entity.ITL_TALEP_ISLEM}' , ";
			isTalepLogQuery += $" {UserInfo.USER_ID} )";

			try
			{
				var util = new Util();
				using (var cnn = util.baglan())
				{
					var parametreler = new DynamicParameters();
					// Log data is recorded
					cnn.Execute(isTalepLogQuery, parametreler);
					// Job request status is being canceled or closed
					cnn.Execute($"update orjin.TB_IS_TALEBI set IST_DURUM_ID = {entity.ITL_ISLEM_ID} WHERE TB_IS_TALEP_ID = {entity.TB_IS_TALEP_ID}", parametreler);

					// Job cancellation or close reason & date & time
					if(entity.ITL_ISLEM_ID == 5)
					{
						parametreler.Add("IS_TALEP_ID", entity.TB_IS_TALEP_ID);
						parametreler.Add("IST_IPTAL_NEDEN", entity.IST_IPTAL_NEDEN);
						parametreler.Add("IST_IPTAL_TARIH", entity.IST_IPTAL_TARIH);
						parametreler.Add("IST_IPTAL_SAAT", entity.IST_IPTAL_SAAT);

						cnn.Execute("update orjin.TB_IS_TALEBI set IST_IPTAL_NEDEN = @IST_IPTAL_NEDEN , " +
						"IST_IPTAL_TARIH = @IST_IPTAL_TARIH , " +
						"IST_IPTAL_SAAT = @IST_IPTAL_SAAT " +
						" WHERE TB_IS_TALEP_ID = @IS_TALEP_ID", parametreler);
					}
					else if(entity.ITL_ISLEM_ID == 4)
					{
						parametreler.Add("IS_TALEP_ID", entity.TB_IS_TALEP_ID);
						parametreler.Add("IST_SONUC", entity.IST_SONUC);
						parametreler.Add("IST_KAPAMA_TARIHI", entity.IST_KAPAMA_TARIHI);
						parametreler.Add("IST_KAPAMA_SAATI", entity.IST_KAPAMA_SAATI);

						cnn.Execute("update orjin.TB_IS_TALEBI set IST_SONUC = @IST_SONUC , " +
						"IST_KAPAMA_TARIHI = @IST_KAPAMA_TARIHI , " +
						"IST_KAPAMA_SAATI= @IST_KAPAMA_SAATI " +
						" WHERE TB_IS_TALEP_ID = @IS_TALEP_ID", parametreler);
					}
					
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		[Route("api/IsTalepIptalEtKapat")]
		[HttpPost]
		public object IsTalepIptalEtKapat([FromBody] List<IsTalepIptalKapatModel> entities)
		{
			if (!(Boolean)yetki.isAuthorizedToAdd(PagesAuthCodes.IS_TALEPLERI_TANIMLARI) ||
				!(Boolean)yetki.isAuthorizedToUpdate(PagesAuthCodes.IS_TALEPLERI_TANIMLARI))
				return Json(new { has_error = true, status_code = 401, status = "Unathorized to add or update !" });
			try
			{
				if(entities != null && entities.Count != 0)
				{
					foreach (var entity in entities)
					{
						IsTalepIptalKapatProcess(entity);
					}
					return Json(new { has_error = false, status_code = 200, status = "Process Completed Successfully" });
				}
				else return Json(new { has_error = false, status_code = 400, status = "Bad Request ( entity may be null or 0 lentgh)" });
			}
			catch(Exception ex)
			{
				return  Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}


		// Add one by one
		public async Task<Bildirim> IsTalepToIsEmriProcess(IsTalepToIsEmriModel isTalepToIsEmriModel)
		{
			int isEmriTipId = -1;
			Bildirim bldr = new Bildirim();
			try
			{
					using(var conn = klas.baglan())
					{
						isEmriTipId = conn.QueryFirstOrDefault<Int32>(@" select ISP_ISEMRI_TIPI_ID from orjin.TB_IS_TALEBI_PARAMETRE ");
					

					parametreler.Clear();
					parametreler.Add(new Prm("TB_IS_TALEP_ID", isTalepToIsEmriModel.TALEP_ID));
					DataRow drTalep = klas.GetDataRow("select * from orjin.VW_IS_TALEP where TB_IS_TALEP_ID = @TB_IS_TALEP_ID", parametreler);
					parametreler.Clear();
					parametreler.Add(new Prm("TB_MAKINE_ID", drTalep["IST_MAKINE_ID"]));
					DataRow drMakine = klas.GetDataRow("select * from orjin.VW_MAKINE where TB_MAKINE_ID = @TB_MAKINE_ID ", parametreler);
					IsEmri entity = new IsEmri();
					IsEmriController ismCont = new IsEmriController(_logger);
					entity.ISM_DUZENLEME_TARIH = DateTime.Now;
					entity.ISM_DUZENLEME_SAAT = DateTime.Now.ToString(C.DB_TIME_FORMAT);
					entity.ISM_ATOLYE_ID = isTalepToIsEmriModel.ATOLYE_ID;
					entity.ISM_ONCELIK_ID = Util.getFieldInt(drTalep, "IST_ONCELIK_ID");
					entity.ISM_BILDIREN = Util.getFieldString(drTalep, "IST_TALEP_EDEN_ADI");
					entity.ISM_IS_TARIH = Util.getFieldDateTime(drTalep, "IST_ACILIS_TARIHI");
					entity.ISM_IS_SAAT = Util.getFieldString(drTalep, "IST_ACILIS_SAATI");
					entity.ISM_IS_SONUC = Util.getFieldString(drTalep, "IST_ACIKLAMA");
					entity.ISM_IS_TALEP_NO = Util.getFieldString(drTalep, "IST_KOD");
					entity.ISM_ISEMRI_NO = (await ismCont.GetIsEmriKodGetir()).Tanim;

					entity.ISM_DUZENLEME_TARIH = DateTime.Now;
					entity.ISM_DUZENLEME_SAAT = DateTime.Now.ToString(C.DB_TIME_FORMAT);
					entity.ISM_TIP_ID = isEmriTipId;
					entity.ISM_BASLAMA_TARIH = null;
					entity.ISM_BASLAMA_SAAT = "";
					entity.ISM_BITIS_TARIH = null;
					entity.ISM_BITIS_SAAT = "";
					entity.ISM_REF_ID = -1;
					entity.ISM_REF_GRUP = "";
					parametreler.Clear();
					entity.ISM_DURUM_KOD_ID = Convert.ToInt32(klas.GetDataCell("SELECT TB_KOD_ID FROM orjin.TB_KOD WHERE KOD_GRUP='32801' AND KOD_TANIM='AÇIK'", parametreler));
					if (drMakine != null)
					{
						entity.ISM_LOKASYON_ID = Util.getFieldInt(drMakine, "MKN_LOKASYON_ID");
						entity.ISM_MAKINE_ID = Util.getFieldInt(drMakine, "TB_MAKINE_ID");
						entity.ISM_PROJE_ID = Util.getFieldInt(drMakine, "MKN_PROJE_ID");
						entity.ISM_MASRAF_MERKEZ_ID = Util.getFieldInt(drMakine, "MKN_MASRAF_MERKEZ_KOD_ID");
						entity.ISM_MAKINE_DURUM_KOD_ID = Util.getFieldInt(drTalep, "IST_MAKINE_DURUM_KOD_ID");
						entity.ISM_MAKINE_GUVENLIK_NOTU = Util.getFieldString(drMakine, "MKN_GUVENLIK_NOTU");
					}
					else
					{
						entity.ISM_LOKASYON_ID = Util.getFieldInt(drTalep, "IST_BILDIREN_LOKASYON_ID");
						entity.ISM_MAKINE_ID = -1;
						entity.ISM_PROJE_ID = -1;
						entity.ISM_MAKINE_GUVENLIK_NOTU = "";
						entity.ISM_MASRAF_MERKEZ_ID = -1;
						entity.ISM_MAKINE_DURUM_KOD_ID = -1;
					}
					entity.ISM_EKIPMAN_ID = Util.getFieldInt(drTalep, "IST_EKIPMAN_ID");


					entity.ISM_KONU = Util.getFieldString(drTalep, "IST_TANIMI");
					entity.ISM_ATOLYE_ID = isTalepToIsEmriModel.ATOLYE_ID;

					entity.ISM_OLUSTURAN_ID = UserInfo.USER_ID;
					entity.ISM_ACIKLAMA = String.Format("'{0}' koldu iş talebi", drTalep["IST_KOD"].ToString());
					await ismCont.Post(entity);
					parametreler.Clear();
					int _isemriID = Convert.ToInt32(klas.GetDataCell("SELECT MAX(TB_ISEMRI_ID) FROM orjin.TB_ISEMRI", parametreler));
					// iş talep durumu değiştiriliyor.
					parametreler.Clear();
					parametreler.Add(new Prm("TB_IS_TALEP_ID", drTalep["TB_IS_TALEP_ID"]));
					parametreler.Add(new Prm("IST_ISEMRI_ID", _isemriID));
					klas.cmd("UPDATE orjin.TB_IS_TALEBI SET IST_ISEMRI_ID=@IST_ISEMRI_ID , IST_DURUM_ID=3 WHERE TB_IS_TALEP_ID= @TB_IS_TALEP_ID", parametreler);
				
					// iş talep personel ekleniyor
					if(isTalepToIsEmriModel.TEKNISYEN_IDS.Count > 0)
					{
						parametreler.Clear();
						parametreler.Add(new Prm("TB_IS_TALEP_ID", drTalep["TB_IS_TALEP_ID"]));
						foreach(int i in isTalepToIsEmriModel.TEKNISYEN_IDS)
						{
							klas.cmd($"INSERT INTO orjin.TB_IS_TALEBI_TEKNISYEN (ITK_IS_TALEBI_ID,ITK_TEKNISYEN_ID,ITK_FAZLA_MESAI_VAR,ITK_MAIL_GONDERILDI) VALUES(@TB_IS_TALEP_ID,{i},0,0)", parametreler);
							// iş emri personel i ekleniyor
							IsEmriPersonel ismPersonel = new IsEmriPersonel();
							ismPersonel.IDK_ISEMRI_ID = _isemriID;
							ismPersonel.IDK_REF_ID = i;
							ismPersonel.IDK_OLUSTURAN_ID = UserInfo.USER_ID;
							ismCont.PersonelListKaydet(ismPersonel);
						}
					}
					// iş talep log yazılıyor
					//atama logu
					string query = @"INSERT INTO [orjin].[TB_IS_TALEBI_LOG]
											   ([ITL_IS_TANIM_ID]
											   ,[ITL_KULLANICI_ID]
											   ,[ITL_TARIH]
											   ,[ITL_SAAT]
											   ,[ITL_ISLEM]
											   ,[ITL_ACIKLAMA]
											   ,[ITL_ISLEM_DURUM]
											   ,[ITL_TALEP_ISLEM]
											   ,[ITL_OLUSTURAN_ID]
											   ,[ITL_OLUSTURMA_TARIH])
										 VALUES
											   (@ITL_IS_TANIM_ID,
												@ITL_KULLANICI_ID,
												@ITL_TARIH, 
												@ITL_SAAT,
												@ITL_ISLEM, 
												@ITL_ACIKLAMA, 
												@ITL_ISLEM_DURUM, 
												@ITL_TALEP_ISLEM,
												@ITL_OLUSTURAN_ID,
												@ITL_OLUSTURMA_TARIH)";
					parametreler.Clear();
					if(isTalepToIsEmriModel.TEKNISYEN_IDS != null && isTalepToIsEmriModel.TEKNISYEN_IDS.Count > 0) 
					{
						foreach (int i in isTalepToIsEmriModel.TEKNISYEN_IDS)
						{
							string PRS_ISIM = klas.GetDataCell($"select PRS_ISIM from orjin.TB_PERSONEL where TB_PERSONEL_ID={i}", parametreler);
							prms.Clear();

							prms.Add("ITL_IS_TANIM_ID", isTalepToIsEmriModel.TALEP_ID);
							prms.Add("ITL_KULLANICI_ID", UserInfo.USER_ID);
							prms.Add("ITL_TARIH", DateTime.Now);
							prms.Add("ITL_SAAT", DateTime.Now.ToString(C.DB_TIME_FORMAT));
							if (isTalepToIsEmriModel.ATOLYE_ID > 0)
							{
								AtolyeController atlCont = new AtolyeController();
								string atolyeTanim = atlCont.AtolyeListesi().FirstOrDefault(a => a.TB_ATOLYE_ID == isTalepToIsEmriModel.ATOLYE_ID).ATL_TANIM;
								prms.Add("ITL_ISLEM", "Atölye Ataması");
								prms.Add("ITL_TALEP_ISLEM", "Atölye Ataması");
								prms.Add("ITL_ACIKLAMA", String.Format("Atölye: {0}  İş Emri Numarası: {1}", atolyeTanim, entity.ISM_ISEMRI_NO));
							}
							else
							{
								prms.Add("ITL_ISLEM", "Teknisyen Ataması");
								prms.Add("ITL_TALEP_ISLEM", "Teknisyen Ataması");
								prms.Add("ITL_ACIKLAMA", String.Format("Teknisyen : {0} İş Emri Numarası: {1}", PRS_ISIM, entity.ISM_ISEMRI_NO));
							}
							prms.Add("ITL_ISLEM_DURUM", "DEVAM EDIYOR");
							prms.Add("ITL_OLUSTURAN_ID", UserInfo.USER_ID);
							prms.Add("ITL_OLUSTURMA_TARIH", DateTime.Now);
							klas.cmd(query, prms.PARAMS);
						}
					} 
					else
					{
						if (isTalepToIsEmriModel.ATOLYE_ID > 0)
						{
							AtolyeController atlCont = new AtolyeController();
							string atolyeTanim = atlCont.AtolyeListesi().FirstOrDefault(a => a.TB_ATOLYE_ID == isTalepToIsEmriModel.ATOLYE_ID).ATL_TANIM;
							prms.Add("ITL_ISLEM", "Atölye Ataması");
							prms.Add("ITL_TALEP_ISLEM", "Atölye Ataması");
							prms.Add("ITL_ACIKLAMA", String.Format("Atölye: {0}  İş Emri Numarası: {1}", atolyeTanim, entity.ISM_ISEMRI_NO));
						}
					}

					bldr.Durum = true;
					bldr.Aciklama = entity.ISM_ISEMRI_NO;
					bldr.Id = _isemriID;
					return bldr;
				}
			}
			catch (Exception e)
			{
				klas.kapat();
				bldr.Durum = false;
				bldr.Aciklama = e.Message;
				bldr.Id = -1;
				return bldr;
			}

		}


		// Webden gelen istekte kaç tane teknesiyen seçilebilir
		[Route("api/IsTalepToIsEmri")]
		[HttpPost]
		public async Task<object> IsTalepToIsEmri([FromBody] List<IsTalepToIsEmriModel> entities)
		{
			if (!(Boolean)yetki.isAuthorizedToAdd(PagesAuthCodes.IS_TALEPLERI_TANIMLARI) || 
				!(Boolean)yetki.isAuthorizedToUpdate(PagesAuthCodes.IS_TALEPLERI_TANIMLARI))
				return Json(new { has_error = true, status_code = 401, status = "Unathorized to add or update !" });

			List<Bildirim> list = new List<Bildirim>();
			try
			{
				if (entities != null && entities.Count != 0)
				{
					foreach (var entity in entities)
					{
						Bildirim bldr = await IsTalepToIsEmriProcess(entity);
						if (!bldr.Durum) return Json(new { has_error = true, status_code = 500, status = bldr.Aciklama });
						list.Add(bldr);
					}
					
					return Json(new { has_error = false, status_code = 200, isEmriNolari = list });
				}
				else return Json(new { has_error = true, status_code = 400, status = "Bad Request ( entity may be null or 0 lentgh)" });
			}
			catch(Exception ex)
			{
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}

		[Route("api/GetIsTalepTeknisyenList")]
		[HttpGet]
		public object GetIsTalepTeknisyenList([FromUri] int isTalepId)
		{
			Util klas = new Util();
			List<IsTalebiTeknisyen> listem = new List<IsTalebiTeknisyen>();
			string query = @"select itk.TB_IS_TALEBI_TEKNISYEN_ID , itk.ITK_IS_TALEBI_ID , 
							  prs.PRS_ISIM as ITK_PERSONEL_ISIM , vrd.VAR_TANIM as ITK_VARDIYA_TANIM , 
							  itk.ITK_SURE , itk.ITK_SAAT_UCRETI , itk.ITK_FAZLA_MESAI_VAR , 
                              itk.ITK_FAZLA_MESAI_SURE , itk.ITK_FAZLA_MESAI_SAAT_UCRETI , itk.ITK_MALIYET
							  from orjin.TB_IS_TALEBI_TEKNISYEN itk
							  left join orjin.TB_PERSONEL prs on prs.TB_PERSONEL_ID = itk.ITK_TEKNISYEN_ID
							  left join orjin.TB_VARDIYA vrd on vrd.TB_VARDIYA_ID = itk.ITK_VARDIYA
							  where ITK_IS_TALEBI_ID = @IS_TALEP_ID";

			using (var conn = klas.baglan())
			{
				listem = conn.Query<IsTalebiTeknisyen>(query, new { @IS_TALEP_ID = isTalepId }).ToList();
			}
			return listem;
		}

		[Route("api/IsTalepTarihceByKod")]
		[HttpGet]
		public object GetIsTalepTarihceByID([FromUri] string talepKod)
		{
			try
			{
				using(var cnn = klas.baglan())
				{
					int talepID = -1;
					talepID = cnn.QueryFirstOrDefault<int>($"SELECT TB_IS_TALEP_ID FROM orjin.TB_IS_TALEBI where IST_KOD = '{talepKod}'");
					parametreler.Clear();
					parametreler.Add(new Prm("ITL_IS_TANIM_ID", talepID));
					string sql = "select * from orjin.TB_IS_TALEBI_LOG where ITL_IS_TANIM_ID=@ITL_IS_TANIM_ID";
					List<IsTalebiLog> listem = new List<IsTalebiLog>();
					DataTable dt = klas.GetDataTable(sql, parametreler);
					for (int i = 0; i < dt.Rows.Count; i++)
					{
						IsTalebiLog entity = new IsTalebiLog();
						entity.ITL_ACIKLAMA = dt.Rows[i]["ITL_ACIKLAMA"] != DBNull.Value ? dt.Rows[i]["ITL_ACIKLAMA"].ToString() : "";
						entity.TB_IS_TALEP_LOG_ID = Util.getFieldInt(dt.Rows[i], "TB_IS_TALEP_LOG_ID");
						entity.ITL_KULLANICI_ID = Util.getFieldInt(dt.Rows[i], "ITL_KULLANICI_ID");
						entity.ITL_IS_TANIM_ID = Util.getFieldInt(dt.Rows[i], "ITL_IS_TANIM_ID");
						entity.ITL_TARIH = Util.getFieldDateTime(dt.Rows[i], "ITL_TARIH");
						entity.ITL_TALEP_ISLEM = Util.getFieldString(dt.Rows[i], "ITL_TALEP_ISLEM");
						entity.ITL_SAAT = Util.getFieldString(dt.Rows[i], "ITL_SAAT");
						entity.ITL_ISLEM_DURUM = Util.getFieldString(dt.Rows[i], "ITL_ISLEM_DURUM");
						entity.ITL_ISLEM = Util.getFieldString(dt.Rows[i], "ITL_ISLEM");
						entity.ITL_ACIKLAMA = Util.getFieldString(dt.Rows[i], "ITL_ACIKLAMA");
						entity.ITL_OLUSTURMA_TARIH = DateTime.Now;
						listem.Add(entity);
					}

					return listem;
				}
			}
			catch(Exception e)
			{
				return Json(new { has_error = true, status_code = 500, status = e.Message });
			}
		}
	}
}
