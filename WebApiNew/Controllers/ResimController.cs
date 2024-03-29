using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Dapper;
using Dapper.Contrib.Extensions;
using WebApiNew.Models;
using WebApiNew;
using WebApiNew.App_GlobalResources;
using WebApiNew.Filters;
using WebApiNew.Utility.Abstract;

namespace WebApiNew.Controllers
{

    [JwtAuthenticationFilter]
    public class ResimController : ApiController
    {
        private readonly ILogger _logger;

        Util klas = new Util();
        Parametreler prms = new Parametreler();

        public ResimController(ILogger logger)
        {
            _logger = logger;
        }

        [Route("api/GetResimIds")]
        [HttpGet]
        public List<Int32> Get([FromUri] int RefID, [FromUri] string RefGrup)
        {
            string query = @"select TB_RESIM_ID from orjin.TB_RESIM where RSM_REF_ID = @RefID and RSM_REF_GRUP = @RefGrup";
            List<Int32> listem = new List<Int32>();
            using(var cnn = klas.baglan())
            {
                listem = cnn.Query<Int32>(query,new {  RefID, RefGrup }).ToList();
            }
            return listem;
        }

        [Route("api/ResimKayit")]
        [HttpPost]
        public Bildirim ResimKayit([FromBody] Resim entity)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                if (entity.TB_RESIM_ID < 1)
                {// ekle
                    string query = @"INSERT INTO orjin.TB_RESIM
                                                  ( RSM_REF_ID
                                                   ,RSM_REF_GRUP
                                                   ,RSM_VARSAYILAN
                                                   ,RSM_UZANTI
                                                   ,RSM_RESIM_AD
                                                   ,RSM_YOL
                                                   ,RSM_ARSIV_AD
                                                   ,RSM_ARSIV_YOL
                                                   ,RSM_BOYUT
                                                   ,RSM_ETIKET
                                                   ,RSM_OLUSTURAN_ID
                                                   ,RSM_OLUSTURMA_TARIH ) values   
                                                   (@RSM_REF_ID
                                                   ,@RSM_REF_GRUP
                                                   ,@RSM_VARSAYILAN
                                                   ,@RSM_UZANTI
                                                   ,@RSM_RESIM_AD
                                                   ,@RSM_YOL
                                                   ,@RSM_ARSIV_AD
                                                   ,@RSM_ARSIV_YOL
                                                   ,@RSM_BOYUT
                                                   ,@RSM_ETIKET
                                                   ,@RSM_OLUSTURAN_ID
                                                   ,@RSM_OLUSTURMA_TARIH )";
                    prms.Clear();
                    prms.Add("@RSM_REF_ID", entity.RSM_REF_ID);
                    prms.Add("@RSM_REF_GRUP", entity.RSM_REF_GRUP);
                    prms.Add("@RSM_VARSAYILAN", entity.RSM_VARSAYILAN);
                    prms.Add("@RSM_UZANTI", entity.RSM_UZANTI);
                    prms.Add("@RSM_RESIM_AD", entity.RSM_RESIM_AD);
                    prms.Add("@RSM_YOL", entity.RSM_YOL);
                    prms.Add("@RSM_ARSIV_AD", entity.RSM_ARSIV_AD);
                    prms.Add("@RSM_ARSIV_YOL", entity.RSM_ARSIV_YOL);
                    prms.Add("@RSM_BOYUT", entity.RSM_BOYUT);
                    prms.Add("@RSM_ETIKET", entity.RSM_ETIKET);
                    prms.Add("@RSM_OLUSTURAN_ID", entity.RSM_OLUSTURAN_ID);
                    prms.Add("@RSM_OLUSTURMA_TARIH", DateTime.Now);
                    klas.cmd(query, prms.PARAMS);

                    bildirimEntity.Aciklama = "Resim kaydı başarılı bir şekilde gerçekleştirildi.";
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
                _logger.Info("Resim/UpdateRefIds");
                _logger.Error(bildirimEntity.Aciklama);
                _logger.Trace(e.StackTrace);
            }
            return bildirimEntity;
        }



        [Route("api/Resim/UpdateRefIds")]
        [HttpGet]
        public Bildirim UpdateRefIds([FromUri] int refId, [FromUri] int[] ids)
        {
            try
            {
                var util = new Util();
                using (var cnn = util.baglan())
                {
                    var result = cnn.Execute("UPDATE orjin.TB_RESIM SET RSM_REF_ID = @REF_ID WHERE TB_RESIM_ID IN @IDS", new { REF_ID = refId, IDS = ids });

                    return new Bildirim
                    {
                        Durum = true,
                        MsgId = Bildirim.MSG_ISLEM_BASARILI
                    };
                }

            }
            catch (Exception e)
            {
                _logger.Info("Resim/UpdateRefIds");
                _logger.Error(e);
                return new Bildirim
                {
                    Durum = false,
                    MsgId = Bildirim.MSG_ISLEM_HATA,
                    Aciklama = e.Message
                };
            }
        }

        [Route("api/Resim/DuplicateImage")]
        [HttpGet]
        public Bildirim ResimCogalt([FromUri] int resimID, [FromUri] int refId, [FromUri] string refGrup)
        {
            try
            {
                var util = new Util();
                using (var cnn = util.baglan())
                {
                    var resim = cnn.QueryFirstOrDefault<Resim>("SELECT TOP 1 * FROM orjin.TB_RESIM WHERE TB_RESIM_ID=@ID", new { ID = resimID });
                    if (resim != null)
                    {
                        resim.TB_RESIM_ID = 0;
                        resim.RSM_REF_GRUP = C.REF_GRUP_IS_TALEP;
                        resim.RSM_REF_ID = refId;
                        var result = cnn.Insert(resim);
                        return new Bildirim
                        {
                            Durum = result > 0,
                            Id = result,
                            MsgId = result > 0 ? Bildirim.MSG_ISLEM_BASARILI : Bildirim.MSG_ISLEM_HATA,
                            Aciklama = result > 0 ? null : Localization.KayitEdilemedi
                        };
                    }
                    _logger.Info("Resim/DuplicateImage");
                    _logger.Error(Localization.KayitBulunamadi);
                    throw new Exception(Localization.KayitBulunamadi);

                }

            }
            catch (Exception e)
            {
                _logger.Info("Resim/DuplicateImage");
                _logger.Error(e);
                return new Bildirim
                {
                    Durum = false,
                    MsgId = Bildirim.MSG_ISLEM_HATA,
                    Aciklama = e.Message
                };
            }
        }

        [Route("api/Resim/DeleteMulti")]
        [HttpGet]
        public Bildirim RemoveMultiple([FromUri] int[] ids)
        {
            try
            {
                var util = new Util();
                using (var cnn = util.baglan())
                {
                    var result = cnn.Execute("DELETE FROM orjin.TB_RESIM WHERE TB_RESIM_ID IN @IDS", new { IDS = ids });
                    var paths = cnn.Query<String>("SELECT RSM_ARSIV_YOL FROM orjin.TB_RESIM WHERE TB_RESIM_ID IN @IDS",new { IDS = ids }).ToList();
                    paths.ForEach(delegate(String yol) {
                        try
                        {
                            if (File.Exists(yol))
                            {
                                File.Delete(yol);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    });

                    return new Bildirim
                    {
                        Durum = true,
                        MsgId = Bildirim.MSG_ISLEM_BASARILI
                    };
                }
            }
            catch (Exception e)
            {
                _logger.Info("Resim/DuplicateImage");
                _logger.Error(e);
                return new Bildirim
                {
                    Durum = false,
                    MsgId = Bildirim.MSG_ISLEM_HATA,
                    Aciklama = e.Message
                };
            }
        }
        [Route("api/ResimSil")]
        [HttpGet]
        public Bildirim ResimSil([FromUri] int resimID)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                prms.Clear();
                prms.Add("RSM_ID", resimID);
                string yol = klas.GetDataCell("select RSM_ARSIV_YOL from orjin.TB_RESIM where TB_RESIM_ID = @RSM_ID", prms.PARAMS);
                if (File.Exists(yol))
                {
                    File.Delete(yol);
                }
                klas.cmd("DELETE FROM orjin.TB_RESIM WHERE TB_RESIM_ID = @RSM_ID", prms.PARAMS);
                bildirimEntity.Aciklama = "Resim başarılı bir şekilde silindi.";
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_BASARILI;
                bildirimEntity.Durum = true;
            }
            catch (Exception e)
            {
                bildirimEntity.Aciklama = String.Format(Localization.errorFormatted, e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }
            return bildirimEntity;
        }
        [Route("api/ResimVarsayilanYap")]
        [HttpPost]
        public Bildirim ResimVarsayilanYap([FromBody] int resimID, [FromUri] int refid, [FromUri] string refGrup)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                prms.Clear();
                prms.Add("RSM_REF_GRUP", refGrup);
                prms.Add("RSM_REF_ID", refid);
                klas.cmd("UPDATE orjin.TB_RESIM SET RSM_VARSAYILAN = 0 WHERE RSM_REF_GRUP = @RSM_REF_GRUP and RSM_REF_ID = @RSM_REF_ID", prms.PARAMS);
                prms.Clear();
                prms.Add("RSM_ID", resimID);
                klas.cmd("UPDATE orjin.TB_RESIM SET RSM_VARSAYILAN = 1 WHERE TB_RESIM_ID = @RSM_ID", prms.PARAMS);
                bildirimEntity.Aciklama = "Resim varsayılan olarak ayarlandı.";
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_BASARILI;
                bildirimEntity.Durum = true;
            }
            catch (Exception e)
            {

                bildirimEntity.Aciklama = string.Format(Localization.errorFormatted, e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
                _logger.Error(bildirimEntity.Aciklama);
                _logger.Trace(e.StackTrace);
            }
            return bildirimEntity;
        }
        [Route("api/UploadPhoto")]
        [HttpPost]
        public Object UploadPhoto( [FromUri] int refid, [FromUri] string refGrup)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            Bildirim bldrm = new Bildirim();
            List<long> idlistt = new List<long>();
            try
            {
                var httpRequest = HttpContext.Current.Request;
                prms.Clear();
                string resimYolu = klas.GetDataCell("select PRM_DEGER from orjin.TB_PARAMETRE where PRM_KOD = '000004'", prms.PARAMS);

                if (Directory.Exists(resimYolu)&&Util.IsDirectoryWritable(resimYolu))
                {
                    foreach (string file in httpRequest.Files)
                    {
                        int val = new Int32();
                        int sonIDarti1 = Int32.TryParse(klas.GetDataCell("select max(TB_RESIM_ID) from orjin.TB_RESIM", prms.PARAMS), out val) ? val + 1 : 1;
                        //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                        var extension = "";
                        string yeniResimAdi = String.Format("{0}_{1}_000{2}", refGrup, refid, sonIDarti1);
                        var postedFile = httpRequest.Files[file];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {                            // int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  
                            IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                            var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                            extension = ext.ToLower();
                            if (!AllowedFileExtensions.Contains(extension))
                                return Json(new {has_error = true , status_code = 400 , status = "Please upload images with (.jpg,.gif,.png) formats" });
                            
                            else
                            {
                                var filePath = resimYolu + "\\" + yeniResimAdi + extension;
                                using (postedFile.InputStream)
                                {
                                    postedFile.SaveAs(filePath);
                                }

                                Resim entity = new Resim();
                                entity.RSM_ARSIV_AD = yeniResimAdi + extension;
                                entity.RSM_ARSIV_YOL = resimYolu + "\\" + entity.RSM_ARSIV_AD;
                                entity.RSM_BOYUT = postedFile.ContentLength;
                                entity.RSM_OLUSTURAN_ID = UserInfo.USER_ID;
                                entity.RSM_OLUSTURMA_TARIH = DateTime.Now;
                                entity.RSM_REF_GRUP = refGrup;
                                entity.RSM_REF_ID = refid;
                                entity.RSM_RESIM_AD = postedFile.FileName;
                                entity.RSM_UZANTI = extension.Replace(".", "");
                                entity.RSM_VARSAYILAN = false;
                                entity.RSM_ETIKET = "";
                                entity.RSM_YOL = "";
                                ResimKayit(entity);
                                bldrm.Durum = true;
                                bldrm.MsgId = Bildirim.MSG_ISLEM_BASARILI;
                                bldrm.Id = Convert.ToInt32(klas.GetDataCell("select max(TB_RESIM_ID) from orjin.TB_RESIM", prms.PARAMS));
                                idlistt.Add(bldrm.Id);
                            }
                        }
                    }
					return Json(new { has_error = true, status_code = 201, status = " Image uploaded successfully !"  });
				}
                else
                {
                    _logger.Info("ResimYukle");
                    _logger.Error(bldrm.Aciklama);
					return Json(new { has_error = true, status_code = 400, status = Localization.AccessImagePathFail });
				}
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                _logger.Error(ex);
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
        }

		[Route("api/ResimGetirByID")]
		[HttpGet]
		[AllowAnonymous]
		public HttpResponseMessage ResimGetirByID(int id)
		{
			using (MemoryStream ms = new MemoryStream())
			{
				HttpContext context = HttpContext.Current;
				Util klas = new Util();
				prms.Clear();
				string prmresimYolu = klas.GetDataCell("select PRM_DEGER from orjin.TB_PARAMETRE where PRM_KOD = '000004'", prms.PARAMS);
				prms.Add("@ID", id);
				string ResimYolu = klas.GetDataCell("select RSM_ARSIV_AD from orjin.TB_RESIM where TB_RESIM_ID=@ID", prms.PARAMS);
				string filePath = prmresimYolu + "\\" + ResimYolu;
				string extension = Path.GetExtension(filePath).ToLower();

				if (!File.Exists(filePath))
				{
					filePath = context.Server.MapPath("/content/images/fallback.png");
					extension = ".png"; 
				}

				try
				{
					using (Image image = Image.FromFile(filePath))
					{
						ImageFormat format = GetImageFormat(extension);
						image.Save(ms, format != null ? format : ImageFormat.Png);
					}
				}
				catch (Exception e)
				{
					_logger.Error(e);
					
					throw;
				}

				HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
				result.Content = new ByteArrayContent(ms.ToArray());
				result.Content.Headers.ContentType = new MediaTypeHeaderValue(GetMimeType(extension));
				return result;
			}
		}

		private static string GetMimeType(string extension)
		{
			switch (extension)
			{
				case ".bmp": return "image/bmp";
				case ".gif": return "image/gif";
				case ".jpg":
				case ".jpeg": return "image/jpeg";
				case ".png": return "image/png";
				default: return "application/octet-stream"; 
			}
		}

		private static ImageFormat GetImageFormat(string extension)
		{
			switch (extension)
			{
				case ".bmp": return ImageFormat.Bmp;
				case ".gif": return ImageFormat.Gif;
				case ".jpg":
				case ".jpeg": return ImageFormat.Jpeg;
				case ".png": return ImageFormat.Png;
				default: return null; // You may want to handle this case differently
			}
		}


		/* public static ImageFormat GetImageFormat(string extension)
        {
            ImageFormat result = null;
            PropertyInfo prop = typeof(ImageFormat).GetProperties().FirstOrDefault(p => p.Name.Equals(extension, StringComparison.InvariantCultureIgnoreCase));
            if (prop != null)
            {
                result = prop.GetValue(prop) as ImageFormat;
            }
            return result;
        } */

       [Route("api/ResimSirasiGuncelle")]
        [HttpGet]
        public Bildirim ResimSirasiGuncelle([FromUri]String refGrup, [FromUri] int refId,[FromUri] int id_1, [FromUri] int id_2)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                prms.Clear();
                prms.Add("ID_1", id_1);
                prms.Add("ID_2", id_2);
                prms.Add("REF_GRUP",refGrup);
                prms.Add("REF_ID",refId);

                klas.cmd($"SELECT *,CASE WHEN TB_RESIM_ID = {id_1} then {id_2} ELSE {id_1} END AS JoinId INTO #Temp2  FROM  orjin.TB_RESIM" +
                    $" WHERE TB_RESIM_ID in ({id_1},{id_2}) AND RSM_REF_GRUP = '{refGrup}' AND RSM_REF_ID = {refId} " +
                 $"UPDATE y SET y.[RSM_REF_ID] = t.[RSM_REF_ID],  y.[RSM_REF_GRUP] = t.[RSM_REF_GRUP], y.[RSM_VARSAYILAN] = t.[RSM_VARSAYILAN], " +
                 $"y.[RSM_UZANTI] = t.[RSM_UZANTI], y.[RSM_RESIM_AD] = t.[RSM_RESIM_AD], y.[RSM_YOL] = t.[RSM_YOL]," +
                 $" y.[RSM_ARSIV_AD] = t.[RSM_ARSIV_AD], y.[RSM_ARSIV_YOL] = t.[RSM_ARSIV_YOL], y.[RSM_BOYUT] = t.[RSM_BOYUT]," +
                 $"y.[RSM_ETIKET] = t.[RSM_ETIKET], y.[RSM_OLUSTURAN_ID] = t.[RSM_OLUSTURAN_ID], y.[RSM_OLUSTURMA_TARIH] = t.[RSM_OLUSTURMA_TARIH], " +
                 $"y.[RSM_DEGISTIREN_ID] = t.[RSM_DEGISTIREN_ID], y.[RSM_DEGISTIRME_TARIH] = " +
                 $"t.[RSM_DEGISTIRME_TARIH] FROM orjin.TB_RESIM as y INNER JOIN #Temp2  " +
                 $"t ON y.TB_RESIM_ID = t.JoinId WHERE y.TB_RESIM_ID in ({id_1},{id_2}) AND y.RSM_REF_GRUP = '{refGrup}' AND y.RSM_REF_ID = {refId}", 
                 prms.PARAMS);

                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_BASARILI;
                bildirimEntity.Durum = true;
            }
            catch (Exception e)
            {
                bildirimEntity.Aciklama = String.Format(Localization.errorFormatted, e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }
            return bildirimEntity;
        }
    }
}
