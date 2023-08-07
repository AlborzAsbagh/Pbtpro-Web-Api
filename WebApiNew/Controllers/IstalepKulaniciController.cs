using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    public class IstalepKulaniciController : ApiController
    {
		[Route("api/YeniKullaniciEkle")]
        [HttpPost]
        public Bildirim YeniKullaniciEkle([FromUri] string kullaniciAdi , [FromUri] string kullaniciTel , 
            [FromUri] string kullaniciLokasyon , [FromUri] string kullaniciMail)
        {
            Bildirim bld = new Bildirim();
			Util klas = new Util();
			List<Prm> parametreler = new List<Prm>();
			Parametreler prms = new Parametreler();
			try
			{
				string query = $" declare @@lokasyonId INT select @@lokasyonId =  TB_LOKASYON_ID from orjin.TB_LOKASYON where LOK_TANIM = '{kullaniciLokasyon}' ";
				query += " insert into orjin.TB_IS_TALEBI_KULLANICI ( ISK_ISIM , ISK_TELEFON_1 , ISK_LOKASYON_ID , ISK_MAIL ) ";
				query += $" values ('{kullaniciAdi}','{kullaniciTel}', @@lokasyonId,'{kullaniciMail}') ";
				klas.cmd(query, prms.PARAMS);
				bld.Durum = true;
				bld.Aciklama = "Yeni kullanıcı başarılı şekilde eklendi.";
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
