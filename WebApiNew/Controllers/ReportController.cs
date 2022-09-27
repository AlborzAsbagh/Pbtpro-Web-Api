using FastReport.Data;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using WebApiNew.Filters;

namespace WebApiNew.Controllers
{
    
    [MyBasicAuthenticationFilter]
    public class ReportController : Controller
    {       

        [MyBasicAuthenticationFilter]
        public ActionResult Index(string ismno, int frmid = 1)
        {

            try
            {

                RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));
                Config.WebMode = true;

                var frxFile = HttpContext.Server.MapPath("~/forms/Is_Emri_Formu.frx");
                
                FastReport.Export.PdfSimple.PDFSimpleExport pdfExport;

                var util = new Util();

                MemoryStream strm = new MemoryStream();

                using (var fastReport = new FastReport.Report())
                {
                    MsSqlDataConnection sqlConnection = new MsSqlDataConnection();
                    sqlConnection.ConnectionString = util.GetConnectionString();
                    //sqlConnection.CreateAllTables();
                    fastReport.Dictionary.Connections.Add(sqlConnection);
                    fastReport.Load(frxFile);
                    fastReport.Dictionary.Connections[0].ConnectionString = util.GetConnectionString();
                    fastReport.Dictionary.Report.SetParameterValue("ISMNO", ismno);
                    fastReport.SetParameterValue("ISMNO", ismno);
                    fastReport.SetParameterValue("TB_FIRMA_ID", frmid);
                    if (fastReport.Prepare())
                    {
                        using (pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport())
                        {
                            //pdfExport.Export(fastReport, pdfOutputFile);
                            fastReport.Export(pdfExport, strm);
                            fastReport.Dispose();
                            pdfExport.Dispose();
                            strm.Position = 0;
                        }
                    }
                }

                HttpContext.Response.AddHeader("content-disposition", "inline; report_" + ismno + ".pdf");
                return new FileStreamResult(strm, "application/pdf");
            }
            catch (Exception e)
            {
                return Content(e.Message);
            }
        }

        [Route("api/IsmRaporIndir")]
        public ActionResult DownloadReport(string ismno, int frmid = 1)
        {
            try
            {

                RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));
                Config.WebMode = true;

                var rootPath = HttpContext.Server.MapPath("~");

                var frxFile = @"C:\Users\ORJIN\Desktop\Is_Emri_Formu.frx";
                var pdfOutputFile = rootPath + @"testdata.pdf";
                FastReport.Export.PdfSimple.PDFSimpleExport pdfExport;

                var util = new Util();

                MemoryStream strm = new MemoryStream();

                using (var fastReport = new FastReport.Report())
                {
                    MsSqlDataConnection sqlConnection = new MsSqlDataConnection();
                    sqlConnection.ConnectionString = util.GetConnectionString();
                    //sqlConnection.CreateAllTables();
                    fastReport.Dictionary.Connections.Add(sqlConnection);
                    fastReport.Load(frxFile);
                    fastReport.Dictionary.Connections[0].ConnectionString = util.GetConnectionString();
                    fastReport.Dictionary.Report.SetParameterValue("ISMNO", ismno);
                    fastReport.SetParameterValue("ISMNO", ismno);
                    fastReport.SetParameterValue("TB_FIRMA_ID", frmid);
                    if (fastReport.Prepare())
                    {
                        using (pdfExport = new FastReport.Export.PdfSimple.PDFSimpleExport())
                        {
                            //pdfExport.Export(fastReport, pdfOutputFile);
                            fastReport.Export(pdfExport, strm);
                            fastReport.Dispose();
                            pdfExport.Dispose();
                            strm.Position = 0;
                        }
                    }
                }

                HttpContext.Response.AddHeader("content-disposition", "inline; report_"+ismno+".pdf");
                return File(strm, "application/pdf", "report_"+ismno+".pdf");
            }
            catch (Exception e)
            {
                return Json(e);
            }
        }

    }
}