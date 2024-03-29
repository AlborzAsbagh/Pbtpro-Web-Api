using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using WebApiNew.Models;
using Dapper;
using WebApiNew.Filters;
using WebApiNew.App_GlobalResources;
using WebApiNew.Utility.Abstract;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace WebApiNew.Controllers
{
    
    [JwtAuthenticationFilter]
    public class DosyaController : ApiController
    {
		private readonly ILogger _logger;

		Util klas = new Util();
		string query = "";
		Parametreler prms = new Parametreler();

		public DosyaController(ILogger logger)
		{
			_logger = logger;
		}

		[Route("api/GetDosyaList")]
		[HttpGet]
		public object GetDosyaList([FromUri]int refId , [FromUri]string refGrup )
        {
			Util klas = new Util();
			List<Dosya> listem = new List<Dosya>();
			string query = $"SELECT * , (select DST_TANIM from dbo.TB_DOSYA_TIP where TB_DOSYA_TIP_ID = DSY_DOSYA_TIP_ID ) as DSY_DOSYA_TIP" +
				$" FROM dbo.TB_DOSYA where DSY_REF_ID = {refId} and DSY_REF_GRUP = '{refGrup}' ";
			using (var conn = klas.baglan())
			{
				listem = conn.Query<Dosya>(query).ToList();
			}
			return listem;
		}

        [Route("api/GetFilesByRefId")]
        [HttpGet]
        public IEnumerable<Dosya> GetFilesByRefId([FromUri] int page, [FromUri] int pageSize,[FromUri] string refGrup,[FromUri]int refId)
        {
            var start = page * pageSize;
            var end = start + pageSize;
            var sql = @";WITH MTABLE AS(
                                                SELECT TB_DOSYA_ID
                                                      ,DSY_TANIM
                                                      ,DSY_DOSYA_TIP_ID
                                                      ,DSY_AKTIF
                                                      ,DSY_SURELI
                                                      ,DSY_BITIS_TARIH
                                                      ,DSY_ACIKLAMA
                                                      ,DSY_DOSYA_AD
                                                      ,DSY_DOSYA_TURU
                                                      ,DSY_DOSYA_UZANTI
                                                      ,DSY_DOSYA_BOYUT
                                                      ,DSY_DOSYA_OLUSTURMA_TARIH
                                                      ,DSY_DOSYA_DEGISTIRME_TARIH
                                                      ,DSY_DOSYA_YOL
                                                      ,DSY_ARSIV_AD
                                                      ,DSY_SIFRELI
                                                      ,DSY_SIFRE
                                                      ,DSY_OLUSTURAN_ID
                                                      ,DSY_OLUSTURMA_TARIH
                                                      ,DSY_DEGISTIREN_ID
                                                      ,DSY_DEGISTIRME_TARIH
                                                      ,DSY_REF_ID
                                                      ,DSY_REF_GRUP
                                                      ,DSY_YER_ID
                                                      ,DYS_ETIKET
                                                      ,DSY_HATIRLAT
                                                      ,DSY_HATIRLAT_TARIH
                                                      ,DSY_TARIH
	                                                  ,ROW_NUMBER() OVER (ORDER BY DSY_TARIH DESC) AS RN
                                                  FROM dbo.TB_DOSYA WHERE DSY_REF_GRUP = @REF_GRUP AND DSY_REF_ID=@REF_ID
                                                  ) SELECT * FROM MTABLE WHERE rn> @startpos and rn<= @endpos";

            IEnumerable<Dosya> mlist;
            var util = new Util();
            using (var conn = util.baglan())
            {
                mlist = conn.Query<Dosya>(sql, new {startpos = start, endpos = end,REF_GRUP=refGrup,REF_ID=refId});
            }

            return mlist;
        }

		[Route("api/UpdateDosyaById")]
		[HttpPost]
		public async Task<Object> UpdateDosyaById([FromBody] JObject entity)
		{
			
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_DOSYA_ID")) >= 1)
					{
						query = " update dbo.TB_DOSYA set ";
						foreach (var item in entity)
						{

							if (item.Key.Equals("TB_DOSYA_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , DSY_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , DSY_DEGISTIREN_ID = {UserInfo.USER_ID} ";
						query += $" where TB_DOSYA_ID = {Convert.ToInt32(entity.GetValue("TB_DOSYA_ID"))}";

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

		[Route("api/GetFileByID")]
		[HttpGet]
		[AllowAnonymous]
		public HttpResponseMessage DosyaGetirByID(int id)
		{
			var util = new Util();
			using (var conn = util.baglan())
			{
				string path = conn.QueryFirst<String>("select PRM_DEGER from orjin.TB_PARAMETRE where PRM_KOD = '000002'");
				var dosya = conn.QueryFirst<Dosya>("select * from dbo.TB_DOSYA where TB_DOSYA_ID = @ID", new { @ID = id });
				string filePath = path + "\\" + dosya.DSY_DOSYA_AD;
				string extension = dosya.DSY_DOSYA_UZANTI;

				HttpResponseMessage notfound = new HttpResponseMessage(HttpStatusCode.NotFound);
				if (File.Exists(filePath))
				{
					try
					{
						HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
						byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
						result.Content = new ByteArrayContent(fileBytes);
						string mimeType = GetMimeTypeByExtension(extension);
						result.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeType);
						result.Content.Headers.Add("File-Type", extension);
						result.Content.Headers.Add("File-Name", dosya.DSY_DOSYA_AD);
						return result;
					}
					catch (Exception e)
					{
						throw e;
					}
				}
				else
				{
					return notfound;
				}
			}
		}

		private string GetMimeTypeByExtension(string extension)
		{
			var mimeTypeMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				{ "pdf", "application/pdf" },
				{ "docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
				{ "txt", "text/plain" },
				{ "xls", "application/vnd.ms-excel" },
				{ "xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
			};

			if (mimeTypeMap.TryGetValue(extension, out var mimeType))
			{
				return mimeType;
			}
			else
			{
				return "application/octet-stream";
			}
		}

		[Route("api/File/{id}")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DownloadFileByID([FromUri] int id)
        {
            var util = new Util();
            using (var conn = util.baglan())
            {
                string path = conn.QueryFirst<String>("select PRM_DEGER from orjin.TB_PARAMETRE where PRM_KOD = '000002'");
                var dosya = conn.QueryFirst<Dosya>("select * from dbo.TB_DOSYA where TB_DOSYA_ID = @ID",new {@ID=id});
                string filePath = path + "\\" + dosya.DSY_ARSIV_AD;
                string extension = dosya.DSY_DOSYA_UZANTI;


                HttpResponseMessage notfound = new HttpResponseMessage(HttpStatusCode.NotFound);
                if (File.Exists(filePath))
                {
                    try
                    {

                        HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                        byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                        result.Content = new ByteArrayContent(fileBytes);
                        result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        result.Content.Headers.ContentDisposition.FileName = $"{dosya.DSY_TANIM}.{extension}" ;
                        result.Content.Headers.ContentType = new MediaTypeHeaderValue($"application/{extension}");
                        return result;

                    }
                    catch
                        (Exception  e)
                    {
                        throw e;
                    }
                }
                else
                {
                    return notfound;
                }
            }
        }

		[Route("api/GetFileIds")]
		[HttpGet]
		public List<Int32> Get([FromUri] int RefID, [FromUri] string RefGrup)
		{
			string query = @"select TB_DOSYA_ID from dbo.TB_DOSYA where DSY_REF_ID = @RefID and DSY_REF_GRUP = @RefGrup";
			List<Int32> listem = new List<Int32>();
			using (var cnn = klas.baglan())
			{
				listem = cnn.Query<Int32>(query, new { RefID, RefGrup }).ToList();
			}
			return listem;
		}

		[Route("api/AddFile")]
		[HttpPost]
		public Bildirim AddFile([FromBody] Dosya entity)
		{
			Bildirim bildirimEntity = new Bildirim();
			try
			{
				if (entity.TB_DOSYA_ID < 1)
				{// ekle
					string query = @"INSERT INTO dbo.TB_DOSYA
                                                  ( DSY_REF_ID
                                                   ,DSY_REF_GRUP
                                                   ,DSY_DOSYA_UZANTI
                                                   ,DSY_DOSYA_AD
                                                   ,DSY_DOSYA_YOL
                                                   ,DSY_DOSYA_BOYUT
                                                   ,DSY_OLUSTURAN_ID
                                                   ,DSY_OLUSTURMA_TARIH
                                                    ) values   
                                                   (@DSY_REF_ID
                                                   ,@DSY_REF_GRUP
                                                   ,@DSY_DOSYA_UZANTI
                                                   ,@DSY_DOSYA_AD
                                                   ,@DSY_DOSYA_YOL
                                                   ,@DSY_DOSYA_BOYUT
                                                   ,@DSY_OLUSTURAN_ID
                                                   ,@DSY_OLUSTURMA_TARIH  )";
					prms.Clear();
					prms.Add("@DSY_REF_ID", entity.DSY_REF_ID);
					prms.Add("@DSY_REF_GRUP", entity.DSY_REF_GRUP);
					prms.Add("@DSY_DOSYA_UZANTI", entity.DSY_DOSYA_UZANTI);
					prms.Add("@DSY_DOSYA_AD", entity.DSY_DOSYA_AD);
					prms.Add("@DSY_DOSYA_YOL", entity.DSY_DOSYA_YOL);
					prms.Add("@DSY_DOSYA_BOYUT", entity.DSY_DOSYA_BOYUT);
					prms.Add("@DSY_OLUSTURAN_ID", entity.DSY_OLUSTURAN_ID);
					prms.Add("@DSY_OLUSTURMA_TARIH", entity.DSY_OLUSTURMA_TARIH);
					klas.cmd(query, prms.PARAMS);

					bildirimEntity.Aciklama = "Dosya kaydı başarılı bir şekilde gerçekleştirildi.";
					bildirimEntity.MsgId = Bildirim.MSG_ISLEM_BASARILI;
					bildirimEntity.Durum = true;
				}
			}
			catch (Exception e)
			{
				klas.kapat();
				bildirimEntity.Aciklama = String.Format(Localization.errorFormatted, e.Message);
				bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
				bildirimEntity.HasExtra = true;
				bildirimEntity.Durum = false;
				_logger.Info("Dosya/UpdateRefIds");
				_logger.Error(bildirimEntity.Aciklama);
				_logger.Trace(e.StackTrace);
			}
			return bildirimEntity;
		}

		[Route("api/UploadFile")]
		[HttpPost]
		public Object UploadFile([FromUri] int refid, [FromUri] string refGrup)
		{
			Dictionary<string, object> dict = new Dictionary<string, object>();
			Bildirim bldrm = new Bildirim();
			List<long> idlistt = new List<long>();
			try
			{
				var httpRequest = HttpContext.Current.Request;
				prms.Clear();
				string dosyaYolu = klas.GetDataCell("select PRM_DEGER from orjin.TB_PARAMETRE where PRM_KOD = '000002'", prms.PARAMS);

				if (Directory.Exists(dosyaYolu) && Util.IsDirectoryWritable(dosyaYolu))
				{
					foreach (string file in httpRequest.Files)
					{
						int val = new Int32();
						int sonIDarti1 = Int32.TryParse(klas.GetDataCell("select max(TB_DOSYA_ID) from dbo.TB_DOSYA", prms.PARAMS), out val) ? val + 1 : 1;
						//HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

						var extension = "";
						string yeniDosyaAdi = String.Format("{0}_{1}_000{2}", refGrup, refid, sonIDarti1);
						var postedFile = httpRequest.Files[file];
						if (postedFile != null && postedFile.ContentLength > 0)
						{                            // int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  
							var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
							extension = ext.ToLower();
							
							var filePath = dosyaYolu + "\\" + yeniDosyaAdi + extension;
							using (postedFile.InputStream)
							{
								postedFile.SaveAs(filePath);
							}

							Dosya entity = new Dosya();
							entity.DSY_DOSYA_AD = yeniDosyaAdi + extension;
							entity.DSY_DOSYA_YOL = dosyaYolu + "\\" + entity.DSY_DOSYA_YOL;
							entity.DSY_DOSYA_BOYUT = postedFile.ContentLength;
							entity.DSY_OLUSTURAN_ID = UserInfo.USER_ID;
							entity.DSY_OLUSTURMA_TARIH = DateTime.Now;
							entity.DSY_REF_GRUP = refGrup;
							entity.DSY_REF_ID = refid;
							entity.DSY_DOSYA_UZANTI = extension.Replace(".", "");
							AddFile(entity);
							bldrm.Durum = true;
							bldrm.MsgId = Bildirim.MSG_ISLEM_BASARILI;
							bldrm.Id = Convert.ToInt32(klas.GetDataCell("select max(TB_DOSYA_ID) from dbo.TB_DOSYA", prms.PARAMS));
							idlistt.Add(bldrm.Id);
							
						}
					}
					return Json(new { has_error = true, status_code = 201, status = " File uploaded successfully !" });
				}
				else
				{
					_logger.Info("DosyaYukle");
					_logger.Error(bldrm.Aciklama);
					return Json(new { has_error = true, status_code = 400, status = "Undefined file path !" });
				}
			}
			catch (Exception ex)
			{
				_logger.Error(ex.Message);
				_logger.Error(ex);
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}


		[Route("api/AddDosyaTip")]
		[HttpPost]
		public async Task<object> AddDosyaTip([FromBody] JObject entity)
		{
			
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0)
					{
						query = " insert into dbo.TB_DOSYA_TIP  ( DST_OLUSTURMA_TARIH , DST_OLUSTURAN_ID ,  ";
						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" {item.Key} , ";
							else query += $" {item.Key} ";
							count++;
						}

						query += $" ) values ( '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , {UserInfo.USER_ID} ,";
						count = 0;

						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" '{item.Value}' , ";
							else query += $" '{item.Value}' ";
							count++;
						}
						query += " ) ";
						await cnn.ExecuteAsync(query);

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

		[Route("api/UpdateDosyaTip")]
		[HttpPost]
		public async Task<Object> UpdateDosyaTip([FromBody] JObject entity)
		{
		
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_DOSYA_TIP_ID")) >= 1)
					{
						query = " update dbo.TB_DOSYA_TIP set ";
						foreach (var item in entity)
						{

							if (item.Key.Equals("TB_DOSYA_TIP_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , DST_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , DST_DEGISTIREN_ID = {UserInfo.USER_ID} ";
						query += $" where TB_DOSYA_TIP_ID = {Convert.ToInt32(entity.GetValue("TB_DOSYA_TIP_ID"))}";

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

		[Route("api/GetDosyaTipleri")]
		[HttpGet]
		public object GetDosyaTipleri()
		{
			Util klas = new Util();
			List<DosyaTip> listem = new List<DosyaTip>();
			string query = @"select * from dbo.TB_DOSYA_TIP";
			using (var conn = klas.baglan())
			{
				listem = conn.Query<DosyaTip>(query).ToList();
			}
			return listem;
		}
	}
}