using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Web.Http;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    public class IstalepKulaniciController : ApiController
    {
		[Route("api/YeniKullaniciEkle")]
        [HttpPost]
        public Object YeniKullaniciEkle([FromBody] JObject yeniKullanici)
        {
            Bildirim bld = new Bildirim();
			Util klas = new Util();
			List<Prm> parametreler = new List<Prm>();
			Parametreler prms = new Parametreler();
			try
			{
			
				if (yeniKullanici != null && yeniKullanici.Count > 0)
				{
					string query = $" declare @@lokasyonId INT select @@lokasyonId =  TB_LOKASYON_ID from orjin.TB_LOKASYON where LOK_TANIM = '{yeniKullanici["userModel"]["kullaniciLok"]}' ";
					query += " insert into orjin.TB_IS_TALEBI_KULLANICI ( ISK_ISIM , ISK_TELEFON_1 , ISK_LOKASYON_ID , ISK_MAIL ) ";
					query += $" values ('{yeniKullanici["userModel"]["kullaniciIsmi"]}','{yeniKullanici["userModel"]["kullaniciTelefon"]}', @@lokasyonId,'{yeniKullanici["userModel"]["kullaniciEmail"]}') ";
					klas.cmd(query, prms.PARAMS);
					bld.Durum = true;
					bld.Aciklama = "Yeni kullanıcı başarılı şekilde eklendi.";
				} else
				{
					bld.Durum = false;
					bld.Aciklama = "Ekleme başarısız";
				}

			}
			catch (Exception e)
			{
				klas.kapat();
				bld.Durum = false;
				bld.Aciklama = e.Message;
			}

			return bld;
		}

	}
}
