using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using WebApiNew.App_GlobalResources;

namespace WebApiNew.Controllers
{
    
    public class HomeController : Controller
    {
        List<WebApiNew.Prm> parametreler = new List<WebApiNew.Prm>();

        
        public ActionResult Index()
        {
            Localization.Culture = Thread.CurrentThread.CurrentUICulture;
            Util klas = new Util();
            ViewBag.Title = Localization.IndexTitle;
            ViewBag.ApiVer = new indexController.AccessCheck().version;
            try
            {
                klas.baglan();
                klas.kapat();
                ViewBag.BaglantiDurumu = Localization.DbConnectionSuccessful;
            }
            catch (Exception e)
            {
                klas.kapat();
                ViewBag.BaglantiDurumu = Localization.DbConnectionFail;
            }
            try
            {
                if (isImagePathValid())
                {
                    ViewBag.ResimYolu = Localization.AccessImagePathOk;
                }
                else ViewBag.ResimYolu = Localization.AccessImagePathFail;
            }            
            catch (Exception e)
            {
                
                ViewBag.ResimYolu =String.Format(Localization.AccessImagePathFailFormatted,e.Message);
            }


            return View();
        }
        public ActionResult Document(int id)
        {
            ViewBag.DocumentId = id;
            return View();
        }

        public bool isImagePathValid()
        {
            try
            {
                Util klas = new Util();
                string resimYolu = klas.GetDataCell("select PRM_DEGER from orjin.TB_PARAMETRE where PRM_KOD = '000004'",new List<Prm>());
                string[] dirs = Directory.GetDirectories(resimYolu);
                if (Directory.Exists(resimYolu) && Util.IsDirectoryWritable(resimYolu,false))
                    return true;
                else return false;
            }catch (Exception)
            {
                return false;
            }
        }
        public void OmegaApk()
        {
            FileInfo fileinfo = new FileInfo(Server.MapPath("~/files/omega.apk"));
            Response.AddHeader("Content-Length", fileinfo.Length.ToString());
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment; filename=omega.apk");
            Response.TransmitFile(fileinfo.FullName);
            Response.End();
        }
        public void PbtApk()
        {
            FileInfo fileinfo = new FileInfo(Server.MapPath("~/files/pbtpro.apk"));
            Response.AddHeader("Content-Length", fileinfo.Length.ToString());
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment; filename=pbtpro.apk");
            Response.TransmitFile(fileinfo.FullName);
            Response.End();
        }
    }
}
