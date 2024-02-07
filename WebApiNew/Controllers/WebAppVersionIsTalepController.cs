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

	[MyBasicAuthenticationFilter]
	public class WebAppVersionIsTalepController : ApiController
	{
		private readonly ILogger _logger;
		Util klas = new Util();
		string query = "";
		SqlCommand cmd = null;
		Parametreler prms = new Parametreler();
		List<Prm> parametreler = new List<Prm>();

		public WebAppVersionIsTalepController(ILogger logger)
		{
			_logger = logger;
		}

		[Route("api/GetIsTalepFullList")]
		[HttpPost]
		public object GetIsTalepFullList([FromUri] string parametre, [FromBody] JObject filters, [FromUri] int pagingDeger = 1 , [FromUri] int? lokasyonId = 0)
		{
			int pagingIlkDeger = pagingDeger == 1 ? 1 : ((pagingDeger * 10) - 10);
			int pagingSonDeger = pagingIlkDeger == 1 ? 10 : ((pagingDeger * 10));
			int toplamIsTalepSayisi = 0;
			int counter = 0;
			string toplamIsTalepSayisiQuery = "";

			List<IsTalep> listem = new List<IsTalep>();
			try
			{
				query = @" SELECT * FROM ( SELECT * , ROW_NUMBER() OVER (ORDER BY TB_IS_TALEP_ID DESC) AS subRow FROM orjin.VW_IS_TALEP where 1=1 ";
				toplamIsTalepSayisiQuery = @" select count(*) from (select * from orjin.VW_IS_TALEP where 1=1 ";

				if (!string.IsNullOrEmpty(parametre))
				{
					query += $" and ( IST_KOD like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" and ( IST_KOD like '%{parametre}%' or ";
					query += $" IST_KONU like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" IST_KONU like '%{parametre}%' or ";
					query += $" IST_BILDIREN_LOKASYON like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" IST_BILDIREN_LOKASYON like '%{parametre}%' or ";
					query += $" IST_MAKINE_KOD like '%{parametre}%' or "; toplamIsTalepSayisiQuery += $" IST_MAKINE_KOD like '%{parametre}%' or ";
					query += $" IST_TALEP_EDEN_ADI like '%{parametre}%' ) "; toplamIsTalepSayisiQuery += $" IST_TALEP_EDEN_ADI like '%{parametre}%' ) ";
				}

				if ((filters["customfilters"] as JObject) != null && (filters["customfilters"] as JObject).Count > 0)
				{
					query += " and ( ";
					toplamIsTalepSayisiQuery += " and ( ";
					counter = 0;
					foreach (var property in filters["customfilters"] as JObject)
					{
						query += $" {property.Key} LIKE '%{property.Value}%' ";
						toplamIsTalepSayisiQuery += $" {property.Key} LIKE '%{property.Value}%' ";
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
					query += $" and IST_BILDIREN_LOKASYON_ID = {lokasyonId} ";
					toplamIsTalepSayisiQuery += $" and IST_BILDIREN_LOKASYON_ID = {lokasyonId} ";
				}

				if( (filters["durumlar"] as JObject) != null && (filters["durumlar"] as JObject).Count > 0)
				{
					query += " and (  ";
					toplamIsTalepSayisiQuery += " and (  ";
					counter = 0;
					foreach (var property in filters["durumlar"] as JObject)
					{
						query += $" IST_DURUM_ID LIKE '%{property.Value}%' ";
						toplamIsTalepSayisiQuery += $" IST_DURUM_ID LIKE '%{property.Value}%' ";

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

				query += $" ) RowIndex WHERE RowIndex.subRow >= {pagingIlkDeger} AND RowIndex.subRow < {pagingSonDeger}";
				toplamIsTalepSayisiQuery += ") as TotalIsTalepSayisi";

				using (var cnn = klas.baglan())
				{
					listem = cnn.Query<IsTalep>(query).ToList();
					cmd = new SqlCommand(toplamIsTalepSayisiQuery, cnn);
					toplamIsTalepSayisi = (int)cmd.ExecuteScalar();
				}
				klas.kapat();
				return Json(new { page = (int)Math.Ceiling((decimal)toplamIsTalepSayisi / 10), is_talep_listesi = listem });

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
			int count = 0;
			var isemritipId = -1;
			var userId = 0;
			try
			{
			
				using (var cnn = klas.baglan())
				{
					if (entity.GetValue("IST_ISEMRI_TIP_ID") == null || 
						Convert.ToInt32(entity.GetValue("IST_ISEMRI_TIP_ID")) == 0 ||
						Convert.ToInt32(entity.GetValue("IST_ISEMRI_TIP_ID")) == -1 )
					{
						isemritipId = cnn.QueryFirstOrDefault<int>("SELECT TOP 1 ISP_ISEMRI_TIPI_ID FROM orjin.TB_IS_TALEBI_PARAMETRE");
					}
					else isemritipId = Convert.ToInt32(entity.GetValue("IST_ISEMRI_TIP_ID"));

					userId = cnn.QueryFirstOrDefault<int>(@" select isk.TB_IS_TALEBI_KULLANICI_ID
	                                FROM [PBTPRO_MASTER].[orjin].[TB_KULLANICI] kll
	                                left join [PBTPRO_1].[orjin].[TB_PERSONEL] prs on prs.TB_PERSONEL_ID = kll.KLL_PERSONEL_ID " +
									$" left join [PBTPRO_1].[orjin].[TB_IS_TALEBI_KULLANICI] isk on prs.TB_PERSONEL_ID = isk.ISK_PERSONEL_ID where kll.TB_KULLANICI_ID = {Convert.ToInt32(entity.GetValue("USER_ID"))}");

					if (entity != null && entity.Count > 0)
					{
						query = " insert into orjin.TB_IS_TALEBI  ( IST_OLUSTURMA_TARIH , IST_ISEMRI_TIP_ID , IST_TALEP_EDEN_ID , ";
						foreach (var item in entity)
						{
							if (item.Key.Equals("USER_ID") || item.Key.Equals("IST_ISEMRI_TIP_ID") ) continue;
							if (count < entity.Count - 3) query += $" {item.Key} , ";
							else query += $" {item.Key} ";
							count++;
						}

						query += $" ) values ( '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , {isemritipId} , {userId} , ";
						count = 0;

						foreach (var item in entity)
						{
							if (item.Key.Equals("USER_ID") || item.Key.Equals("IST_ISEMRI_TIP_ID")) continue;
							if (count < entity.Count - 3) query += $" '{item.Value}' , ";
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
								ITL_KULLANICI_ID = userId,
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

						return Json(new { has_error = false, status_code = 201, status = "Added Successfully" });
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
						query += $" , IST_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ";
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
		
		public void IsTalepIptalProcess(IsTalepIptalModel entity)
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
			isTalepLogQuery += $" {entity.KLL_ID} , ";
			isTalepLogQuery += $" '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , "; // Changed date format
			isTalepLogQuery += $" '{DateTime.Now.ToString("HH:mm:ss")}' , ";
			isTalepLogQuery += " 'İptal' , ";
			isTalepLogQuery += $" 'Talep no : {entity.IST_TALEP_NO} - Konu : {entity.KLL_ADI} tarafından iptal edildi' , ";
			isTalepLogQuery += " 'İPTAL EDİLDİ' , ";
			isTalepLogQuery += " 'İptal' , ";
			isTalepLogQuery += $" {entity.KLL_ID} )";

			try
			{
				var util = new Util();
				using (var cnn = util.baglan())
				{
					var parametreler = new DynamicParameters();
					parametreler.Add("IS_TALEP_ID", entity.TB_IS_TALEP_ID);
					parametreler.Add("IST_IPTAL_NEDEN", entity.IST_IPTAL_NEDEN);
					parametreler.Add("IST_IPTAL_TARIH", entity.IST_IPTAL_TARIH);
					parametreler.Add("IST_IPTAL_SAAT", entity.IST_IPTAL_SAAT);
					// Log data is recorded
					cnn.Execute(isTalepLogQuery, parametreler);
					// Job request status is being canceled
					cnn.Execute($"update orjin.TB_IS_TALEBI set IST_DURUM_ID = 5 WHERE TB_IS_TALEP_ID = {entity.TB_IS_TALEP_ID}", parametreler);

					// Job cancellation reason & date & time
					cnn.Execute("update orjin.TB_IS_TALEBI set IST_IPTAL_NEDEN = @IST_IPTAL_NEDEN , " +
						"IST_IPTAL_TARIH = @IST_IPTAL_TARIH , " +
						"IST_IPTAL_SAAT = @IST_IPTAL_SAAT," +
						" WHERE TB_IS_TALEP_ID = @IS_TALEP_ID", parametreler);
					
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		[Route("api/IsTalepIptalEt")]
		[HttpPost]
		public object IsTalepIptalEt([FromBody] List<IsTalepIptalModel> entities)
		{
			try
			{
				if(entities != null && entities.Count != 0)
				{
					foreach (var entity in entities)
					{
						IsTalepIptalProcess(entity);
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


		// Webden gelen istekte kaç tane teknesiyen seçilebilir
		[Route("api/IsTalepToIsEmri")]
		[HttpPost]
		public async Task<string> IsTalepToIsEmri([FromUri] int talepID, [FromUri] int userId, [FromUri] int atolyeId , [FromBody] List<int> teknisyenIds)
		{
			string isemriNo = "";
			try
			{
				parametreler.Clear();
				parametreler.Add(new Prm("TB_IS_TALEP_ID", talepID));
				DataRow drTalep = klas.GetDataRow("select * from orjin.VW_IS_TALEP where TB_IS_TALEP_ID = @TB_IS_TALEP_ID", parametreler);
				parametreler.Clear();
				parametreler.Add(new Prm("TB_MAKINE_ID", drTalep["IST_MAKINE_ID"]));
				DataRow drMakine = klas.GetDataRow("select * from orjin.VW_MAKINE where TB_MAKINE_ID = @TB_MAKINE_ID ", parametreler);
				IsEmri entity = new IsEmri();
				IsEmriController ismCont = new IsEmriController(_logger);
				entity.ISM_DUZENLEME_TARIH = DateTime.Now;
				entity.ISM_DUZENLEME_SAAT = DateTime.Now.ToString(C.DB_TIME_FORMAT);
				entity.ISM_ATOLYE_ID = atolyeId;
				entity.ISM_ONCELIK_ID = Util.getFieldInt(drTalep, "IST_ONCELIK_ID");
				entity.ISM_BILDIREN = Util.getFieldString(drTalep, "IST_TALEP_EDEN_ADI");
				entity.ISM_IS_TARIH = Util.getFieldDateTime(drTalep, "IST_ACILIS_TARIHI");
				entity.ISM_IS_SAAT = Util.getFieldString(drTalep, "IST_ACILIS_SAATI");
				entity.ISM_IS_SONUC = Util.getFieldString(drTalep, "IST_ACIKLAMA");
				entity.ISM_ISEMRI_NO = (await ismCont.GetIsEmriKodGetir()).Tanim;

				entity.ISM_DUZENLEME_TARIH = DateTime.Now;
				entity.ISM_DUZENLEME_SAAT = DateTime.Now.ToString(C.DB_TIME_FORMAT);
				entity.ISM_TIP_ID = Util.getFieldInt(drTalep, "IST_ISEMRI_TIP_ID");
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
				entity.ISM_ATOLYE_ID = atolyeId;

				entity.ISM_OLUSTURAN_ID = userId;
				entity.ISM_ACIKLAMA = String.Format("'{0}' koldu iş talebi", drTalep["IST_KOD"].ToString());
				await ismCont.Post(entity, userId);
				parametreler.Clear();
				int _isemriID = Convert.ToInt32(klas.GetDataCell("SELECT MAX(TB_ISEMRI_ID) FROM orjin.TB_ISEMRI", parametreler));
				// iş talep durumu değiştiriliyor.
				parametreler.Clear();
				parametreler.Add(new Prm("TB_IS_TALEP_ID", drTalep["TB_IS_TALEP_ID"]));
				parametreler.Add(new Prm("IST_ISEMRI_ID", _isemriID));
				klas.cmd("UPDATE orjin.TB_IS_TALEBI SET IST_ISEMRI_ID=@IST_ISEMRI_ID , IST_DURUM_ID=3 WHERE TB_IS_TALEP_ID= @TB_IS_TALEP_ID", parametreler);

				// iş talep personel ekleniyor
				if(teknisyenIds.Count > 0)
				{
					parametreler.Clear();
					parametreler.Add(new Prm("TB_IS_TALEP_ID", drTalep["TB_IS_TALEP_ID"]));
					foreach(int i in teknisyenIds)
					{
						parametreler.Add(new Prm("ITK_TEKNISYEN_ID", i));
						klas.cmd("INSERT INTO orjin.TB_IS_TALEBI_TEKNISYEN (ITK_IS_TALEBI_ID,ITK_TEKNISYEN_ID,ITK_FAZLA_MESAI_VAR,ITK_MAIL_GONDERILDI) VALUES(@TB_IS_TALEP_ID,@ITK_TEKNISYEN_ID,0,0)", parametreler);
						// iş emri personel i ekleniyor
						IsEmriPersonel ismPersonel = new IsEmriPersonel();
						ismPersonel.IDK_ISEMRI_ID = _isemriID;
						ismPersonel.IDK_REF_ID = i;
						ismPersonel.IDK_OLUSTURAN_ID = userId;
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
				foreach (int i in teknisyenIds)
				{
					parametreler.Add(new Prm("TB_PERSONEL_ID", i));
					string PRS_ISIM = klas.GetDataCell("select PRS_ISIM from orjin.TB_PERSONEL where TB_PERSONEL_ID=@TB_PERSONEL_ID", parametreler);
					prms.Clear();

					prms.Add("ITL_IS_TANIM_ID", talepID);
					prms.Add("ITL_KULLANICI_ID", userId);
					prms.Add("ITL_TARIH", DateTime.Now);
					prms.Add("ITL_SAAT", DateTime.Now.ToString(C.DB_TIME_FORMAT));
					if (atolyeId > -1)
					{
						AtolyeController atlCont = new AtolyeController();
						string atolyeTanim = atlCont.AtolyeListesi(userId).FirstOrDefault(a => a.TB_ATOLYE_ID == atolyeId).ATL_TANIM;
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
					prms.Add("ITL_OLUSTURAN_ID", userId);
					prms.Add("ITL_OLUSTURMA_TARIH", DateTime.Now);
					klas.cmd(query, prms.PARAMS);
				}
				isemriNo = entity.ISM_ISEMRI_NO;
				return isemriNo;
			}
			catch (Exception)
			{
				klas.kapat();
				return isemriNo;
			}

		}

	}
}
