using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiNew.Models;
using WebApiNew;
using System.Threading.Tasks;
using System.Web;
using Dapper;
using WebApiNew.App_GlobalResources;
using WebApiNew.Filters;

namespace WebApiNew.Controllers
{
    
    [MyBasicAuthenticationFilter]
    public class KullaniciController : ApiController
    {
        Util klas = new Util();
        Parametreler prms = new Parametreler();
        [Route("api/KullanicCihazIDGuncelle")]
        [HttpPost]
        public Bildirim KullanicCihazIDGuncelle([FromUri] int kulID, [FromUri] string cihazID)
        {
            Bildirim bildirimEntity = new Bildirim();
            prms.Clear();
            prms.Add("@KUL_ID", kulID);
            prms.Add("@CHZ_ID", cihazID);
            try
            {
                klas.MasterBaglantisi = true;
                klas.cmd("UPDATE orjin.TB_KULLANICI SET KLL_MOBILCIHAZ_ID = @CHZ_ID where TB_KULLANICI_ID = @KUL_ID", prms.PARAMS);
                bildirimEntity.Aciklama = "Kullanici cihaz güncelleme başarılı bir şekilde gerçekleştirildi.";
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
        [Route("api/KullaniciGuncelle")]
        [HttpPost]
        public Bildirim KullaniciGuncelle([FromBody] Kullanici kul, [FromUri] string oldPass, bool updatePass)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                if (updatePass)
                {
                    prms.Clear();
                    prms.Add("KUL_ID", kul.TB_KULLANICI_ID);
                    klas.MasterBaglantisi = true;
                    string queryCheck = "SELECT * FROM orjin.TB_KULLANICI where TB_KULLANICI_ID = @KUL_ID";
                    if (oldPass == null || oldPass == "")
                        queryCheck += " AND (KLL_SIFRE IS NULL OR KLL_SIFRE = '')";
                    else
                    {
                        prms.Add("@OLD_PASS", oldPass);
                        queryCheck += " AND KLL_SIFRE = @OLD_PASS";
                    }
                    if (klas.GetDataRow(queryCheck, prms.PARAMS) == null)
                    {
                        bildirimEntity.Aciklama = "Girdiğiniz şifre hatalı! Şifrenizi kontrol edip tekrar deneyin. ";
                        bildirimEntity.MsgId = Bildirim.MSG_KLL_SIFRE_HATA;
                        bildirimEntity.Durum = false;
                        klas.MasterBaglantisi = false;
                        return bildirimEntity;
                    }
                }

                klas.MasterBaglantisi = true;

                string query = @"UPDATE orjin.TB_KULLANICI SET KLL_TANIM = @KLL_TANIM";
                if (kul.KLL_SIFRE != null && updatePass)
                    query += ", KLL_SIFRE = @KLL_SIFRE";
                query += @", KLL_DEGISTIREN_ID = @KLL_DEG_ID
                            , KLL_DEGISTIRME_TARIH = @KLL_DEG_TARIH WHERE TB_KULLANICI_ID = @KLL_DEG_ID";
                prms.Clear();
                if (kul.KLL_SIFRE != null && updatePass)
                {
                    if (kul.KLL_SIFRE == "")
                        prms.Add("@KLL_SIFRE", null);
                    else
                        prms.Add("@KLL_SIFRE", kul.KLL_SIFRE);
                }
                prms.Add("@KLL_TANIM", kul.KLL_TANIM);
                prms.Add("@KLL_DEG_ID", kul.TB_KULLANICI_ID);
                prms.Add("@KLL_DEG_TARIH", DateTime.Now);
                klas.cmd(query, prms.PARAMS);
                bildirimEntity.Aciklama = "Kullanici bilgisi güncelleme başarılı bir şekilde gerçekleştirildi.";
                bildirimEntity.MsgId = Bildirim.MSG_KLL_GUNCELLE_OK;
                bildirimEntity.Durum = true;
            }
            catch (Exception e)
            {
                bildirimEntity.Aciklama = String.Format(Localization.errorFormatted, e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }
            klas.MasterBaglantisi = false;
            return bildirimEntity;
        }

        [Route("api/KullaniciGuncelleFoto")]
        [HttpPost]
        public Bildirim KullaniciGuncelleFoto([FromUri] int ID,[FromUri]int ResimId)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {                
                    klas.MasterBaglantisi = true;
                    prms.Clear();
                    prms.Add("TB_KULLANICI_ID", ID);
                    string personelId = klas.GetDataCell("SELECT KLL_PERSONEL_ID FROM orjin.TB_KULLANICI WHERE TB_KULLANICI_ID = @TB_KULLANICI_ID", prms.PARAMS);
                    klas.MasterBaglantisi = false;
                    if (personelId != null && Convert.ToInt32(personelId) > 0)
                    {                        
                        prms.Clear();
                        prms.Add("PER_ID", personelId);
                        klas.cmd("UPDATE orjin.TB_RESIM SET RSM_VARSAYILAN = 0 WHERE RSM_REF_GRUP = 'PERSONEL' AND RSM_REF_ID = @PER_ID", prms.PARAMS);
                        prms.Add("RESIM_ID", ResimId);
                        klas.cmd("UPDATE orjin.TB_RESIM SET RSM_VARSAYILAN = 1 WHERE RSM_REF_GRUP = 'PERSONEL' AND RSM_REF_ID = @PER_ID AND TB_RESIM_ID = @RESIM_ID", prms.PARAMS);
                        
                    }
                    else
                    {
                        bildirimEntity.Aciklama += Localization.PersonelBilgiYokKayitEdilemedi;
                        bildirimEntity.Durum = false;
                        return bildirimEntity;
                    }                
            }
            catch (Exception e)
            {
                bildirimEntity.Durum = false;
                bildirimEntity.Aciklama += String.Format(Localization.ResimKayitHata, e.Message);
                bildirimEntity.MsgId = Bildirim.SHOW_MAIN_DESCRIPTION;
            }
            klas.MasterBaglantisi = false;
            return bildirimEntity;
        }


        [Route("api/HDCihazIDGuncelle")]
        [HttpPost]
        public Bildirim HDCihazIDGuncelle([FromUri] int kulID, [FromUri] string cihazID)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                prms.Clear();
                prms.Add("@CHZ_ID", cihazID);
                prms.Add("@KUL_ID", kulID);
                klas.MasterBaglantisi = false;
                klas.cmd("UPDATE orjin.TB_IS_TALEBI_KULLANICI SET ISK_MOBILCIHAZ_ID = @CHZ_ID where TB_IS_TALEBI_KULLANICI_ID = @KUL_ID", prms.PARAMS);
                bildirimEntity.Aciklama = "Kullanici cihaz güncelleme başarılı bir şekilde gerçekleştirildi.";
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
        
        [Route("api/GetUserNameById")]
        [HttpGet]
        public async Task<string> GetUserNameById([FromUri] int kulID)
        {
            var util = new Util();
            string name = null;
            util.MasterBaglantisi = true;
            using (var cnn=util.baglan())
            {
                name= await cnn.QueryFirstOrDefaultAsync<string>("SELECT KLL_TANIM FROM orjin.TB_KULLANICI WHERE TB_KULLANICI_ID = @KLL_ID",new {KLL_ID=kulID});
            }

            return name;
        }

		
		[Route("api/GetKullaniciList")]
		[HttpGet]
		public object GetKullaniciList()
		{
			Util klas = new Util();
			List<Kullanici> listem = new List<Kullanici>();
			string query = @"select * from [PBTPRO_MASTER].[orjin].[VW_KULLANICI] where KLL_AKTIF = 1";
			using (var conn = klas.baglan())
			{
				listem = conn.Query<Kullanici>(query).ToList();
			}
			return listem;
		}
	}
}
