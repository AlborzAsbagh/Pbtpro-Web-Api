using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using WebApiNew.Models;
using Dapper;
using WebApiNew.Filters;

namespace WebApiNew.Controllers
{
    
    [JwtAuthenticationFilter]
    public class DosyaController : ApiController
    {
        public IEnumerable<Dosya> Get([FromUri] int start, [FromUri] int end)
        {
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
                                                  FROM dbo.TB_DOSYA
                                                  ) SELECT * FROM MTABLE WHERE rn> @startpos and rn<= @endpos";

            IEnumerable<Dosya> mlist;
            var util = new Util();
            using (var conn = util.baglan())
            {
                mlist = conn.Query<Dosya>(sql, new {@startpos = start, @endpos = end});
            }

            return mlist;
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


        [Route("api/DosyaGetirByID")]
        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage DosyaGetirByID(int id)
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
                        //result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                        //result.Content.Headers.ContentDisposition.FileName = $"{dosya.DSY_TANIM}.{extension}" ;
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


    }
}