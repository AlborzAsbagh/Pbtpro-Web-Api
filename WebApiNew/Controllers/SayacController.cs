using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using WebApiNew.App_GlobalResources;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    
    [MyBasicAuthenticationFilter]
    public class SayacController : ApiController
    {

        Util klas = new Util();
        Parametreler prms = new Parametreler();
        public List<Sayac> Get([FromUri] int MakineID)
        {
            prms.Clear();
            prms.Add("MAK_ID", MakineID);
            string query = @"select * from orjin.VW_SAYAC where MES_REF_ID= @MAK_ID";
            DataTable dt = klas.GetDataTable(query, prms.PARAMS);
            List<Sayac> listem = new List<Sayac>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Sayac entity = new Sayac();
                entity.MES_SAYAC_BIRIM = Util.getFieldString(dt.Rows[i], "MES_SAYAC_BIRIM");
                entity.MES_SAYAC_TIP = Util.getFieldString(dt.Rows[i], "MES_SAYAC_TIP");
                entity.MES_TANIM = Util.getFieldString(dt.Rows[i], "MES_TANIM");
                entity.MES_GUNCEL_DEGER = Util.getFieldDouble(dt.Rows[i], "MES_GUNCEL_DEGER");
                entity.MES_BIRIM_KOD_ID = Util.getFieldInt(dt.Rows[i], "MES_BIRIM_KOD_ID");
                entity.MES_TIP_KOD_ID = Util.getFieldInt(dt.Rows[i], "MES_TIP_KOD_ID");
                entity.TB_SAYAC_ID = Util.getFieldInt(dt.Rows[i], "TB_SAYAC_ID");
                entity.MES_VARSAYILAN = Util.getFieldBool(dt.Rows[i], "MES_VARSAYILAN");
                entity.MES_SANAL_SAYAC = Util.getFieldBool(dt.Rows[i], "MES_SANAL_SAYAC");
				entity.MES_ACIKLAMA = Util.getFieldString(dt.Rows[i], "MES_ACIKLAMA");
				entity.MES_BASLANGIC_TARIH = Util.getFieldDateTime(dt.Rows[i], "MES_BASLANGIC_TARIH");
				entity.MES_BASLANGIC_DEGER = Util.getFieldInt(dt.Rows[i], "MES_BASLANGIC_DEGER");
				entity.MES_TAHMINI_ARTIS_DEGER = Util.getFieldInt(dt.Rows[i], "MES_TAHMINI_ARTIS_DEGER");
				entity.MES_SANAL_SAYAC_ARTIS = Util.getFieldInt(dt.Rows[i], "MES_SANAL_SAYAC_ARTIS");
				listem.Add(entity);
            }
            return listem;
        }

        [Route("api/MakineSayacBilgisiGetir")]
        [HttpGet]
        public double MakineSayacBilgisiGetir([FromUri] int sayacokumaID, [FromUri] int sayacId, [FromUri] string tarih, [FromUri] string saat)
        {
            prms.Clear();
            prms.Add("TB_SAYAC_OKUMA_ID", sayacokumaID);
            prms.Add("SYO_SAYAC_ID", sayacId);
            prms.Add("TARIH", Convert.ToDateTime(tarih).ToString("yyyy-MM-dd"));
            prms.Add("SAAT", saat);
            double sonSayacDeger = 0;
            string query = @" SELECT TOP 1 ISNULL(SYO_OKUNAN_SAYAC,0) FROM orjin.TB_SAYAC_OKUMA 
                            WHERE TB_SAYAC_OKUMA_ID <> @TB_SAYAC_OKUMA_ID AND SYO_SAYAC_ID = @SYO_SAYAC_ID AND 
                                  convert(datetime, SYO_TARIH + ' ' + SYO_SAAT, 120) < @TARIH +' '+ @SAAT ORDER BY convert(datetime,SYO_TARIH + ' ' + SYO_SAAT , 120)DESC ";

            sonSayacDeger = Convert.ToDouble(klas.GetDataCell(query, prms.PARAMS));
            return sonSayacDeger;
        }
        [Route("api/SayacOkumaList")]
        [HttpPost]
        public List<SayacOkuma> SayacOkumaList([FromBody]Filtre filtre, [FromUri] int ilkDeger, [FromUri] int sonDeger ,[FromUri] bool sortAsc)
        {
            prms.Clear();
            List<SayacOkuma> listem = new List<SayacOkuma>();
            string sort = sortAsc ? "ASC" : "DESC";
            string query = @";WITH mTable AS
            (select so.*,M.TB_MAKINE_ID as SYO_MAKINE_ID,MES_SAYAC_BIRIM,MES_TANIM,MES_SAYAC_TIP,('[' +M.MKN_KOD + '] ' + M.MKN_TANIM) AS SYO_MAKINE,
            STUFF(( SELECT ';' + CONVERT(varchar(11), R.TB_RESIM_ID) FROM orjin.TB_RESIM R WHERE R.RSM_REF_GRUP = 'SAYAC_OKUMA' AND R.RSM_REF_ID = TB_SAYAC_OKUMA_ID FOR XML PATH('')),1,1,'') AS ResimIDleri,
            (select LOK_TANIM from orjin.TB_LOKASYON where TB_LOKASYON_ID = SYO_LOKASYON_ID) AS SYO_LOK_TANIM,ROW_NUMBER() 
            OVER(ORDER BY SYO_TARIH " + sort+", SYO_SAAT "+sort+") AS satir from orjin.TB_SAYAC_OKUMA so left join orjin.VW_SAYAC s on " +
            "TB_SAYAC_ID= SYO_SAYAC_ID left join orjin.TB_MAKINE m on TB_MAKINE_ID=MES_REF_ID where 1=1 ";

            if (filtre != null)
            {
                if (filtre.MakineID > 0)
                {
                    prms.Add("TB_MAKINE_ID", filtre.MakineID);
                    query = query + " and m.TB_MAKINE_ID=@TB_MAKINE_ID";
                }
                if (filtre.LokasyonID > 0)
                {
                    prms.Add("SYO_LOKASYON_ID", filtre.LokasyonID);
                    query = query + " and SYO_LOKASYON_ID=@SYO_LOKASYON_ID";
                }

                if (filtre.BasTarih != "" && filtre.BitTarih != "")
                {
                    DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                    DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                    prms.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
                    prms.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
                    query = query + " AND SYO_TARIH BETWEEN  @BAS_TARIH and @BIT_TARIH";
                }
                else
                if (filtre.BasTarih != "")
                {
                    DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                    prms.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
                    query = query + " AND SYO_TARIH >= @BAS_TARIH  ";
                }
                else
                if (filtre.BitTarih != "")
                {
                    DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                    prms.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
                    query = query + " AND SYO_TARIH <= @BIT_TARIH";
                }
                if (filtre.Kelime != "")
                {
                    prms.Add("KELIME", filtre.Kelime);
                    query = query + " AND  (M.MKN_KOD like '%'+@KELIME+'%' OR M.MKN_TANIM like '%'+@KELIME+'%'OR MES_TANIM like '%'+@KELIME+'%')";
                }
            }
            prms.Add("ILK_DEGER", ilkDeger);
            prms.Add("SON_DEGER", sonDeger);
            query = query + ")SELECT * FROM mTable  where satir > @ILK_DEGER and satir <= @SON_DEGER";
            DataTable dt = klas.GetDataTable(query, prms.PARAMS);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SayacOkuma entity = new SayacOkuma();
                entity.TB_SAYAC_OKUMA_ID = Convert.ToInt32(dt.Rows[i]["TB_SAYAC_OKUMA_ID"]);
                entity.SYO_ACIKLAMA = Util.getFieldString(dt.Rows[i], "SYO_ACIKLAMA");
                entity.SYO_REF_GRUP = Util.getFieldString(dt.Rows[i], "SYO_REF_GRUP");
                entity.SYO_SAAT = Util.getFieldString(dt.Rows[i], "SYO_SAAT");
                entity.MES_TANIM = Util.getFieldString(dt.Rows[i], "MES_TANIM");
                entity.MES_SAYAC_TIP = Util.getFieldString(dt.Rows[i], "MES_SAYAC_TIP");
                entity.MES_SAYAC_BIRIM = Util.getFieldString(dt.Rows[i], "MES_SAYAC_BIRIM");
                entity.SYO_MAKINE = Util.getFieldString(dt.Rows[i], "SYO_MAKINE");
                entity.SYO_LOK_TANIM = Util.getFieldString(dt.Rows[i], "SYO_LOK_TANIM");
                entity.SYO_REF_ID = Util.getFieldInt(dt.Rows[i],"SYO_REF_ID");
                entity.SYO_DEGISTIREN_ID = Util.getFieldInt(dt.Rows[i],"SYO_DEGISTIREN_ID");
                entity.SYO_DEGISTIRME_TARIH = Util.getFieldDateTime(dt.Rows[i],"SYO_DEGISTIRME_TARIH");
                entity.SYO_FARK_SAYAC = dt.Rows[i]["SYO_FARK_SAYAC"] != DBNull.Value ? Convert.ToDouble(dt.Rows[i]["SYO_FARK_SAYAC"]) : 0;
                entity.SYO_OKUNAN_SAYAC = dt.Rows[i]["SYO_OKUNAN_SAYAC"] != DBNull.Value ? Convert.ToDouble(dt.Rows[i]["SYO_OKUNAN_SAYAC"]) : 0;
                entity.SYO_LOKASYON_ID = dt.Rows[i]["SYO_LOKASYON_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["SYO_LOKASYON_ID"]) : -1;
                entity.SYO_SAYAC_ID = dt.Rows[i]["SYO_SAYAC_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["SYO_SAYAC_ID"]) : -1;
                entity.SYO_TARIH = Util.getFieldDateTime(dt.Rows[i],"SYO_TARIH");
                entity.SYO_MAKINE_ID = Util.getFieldInt(dt.Rows[i], "SYO_MAKINE_ID");
                entity.ResimIDleri = Util.getFieldString(dt.Rows[i], "ResimIDleri");
                listem.Add(entity);
            }
            return listem;
        }
        [Route("api/SayacSil")]
        [HttpPost]
        public Bildirim SayacHareketSil([FromUri] int sayachareketID)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                prms.Clear();
                prms.Add("TB_SAYAC_OKUMA_ID", sayachareketID);
                klas.cmd("DELETE FROM orjin.TB_SAYAC_OKUMA WHERE TB_SAYAC_OKUMA_ID = @TB_SAYAC_OKUMA_ID", prms.PARAMS);
                bildirimEntity.Aciklama = "Sayac hareketi başarılı bir şekilde silindi.";
                bildirimEntity.MsgId = Bildirim.MSG_SYO_SIL_BASARILI;
                bildirimEntity.Durum = true;
            }
            catch (Exception e)
            {
                bildirimEntity.Aciklama = string.Format(Localization.errorFormatted,e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_SYO_SIL_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }
            return bildirimEntity;
        }
        [Route("api/SayacKayit")]
        [HttpPost]
		public async Task<Bildirim> SayacKayit([FromBody] SayacOkuma entity)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                if (entity.TB_SAYAC_OKUMA_ID < 1)
                {// ekle
                    string query = @"INSERT INTO orjin.TB_SAYAC_OKUMA
                                                   (SYO_SAYAC_ID
                                                   ,SYO_TARIH
                                                   ,SYO_SAAT
                                                   ,SYO_OKUNAN_SAYAC
                                                   ,SYO_FARK_SAYAC
                                                   ,SYO_ACIKLAMA
                                                   ,SYO_ELLE_GIRIS
                                                   ,SYO_OLUSTURAN_ID
                                                   ,SYO_OLUSTURMA_TARIH
                                                   ,SYO_HAREKET_TIP
                                                   ,SYO_REF_ID
                                                   ,SYO_REF_GRUP
                                                   ,SYO_SAYAC_GUNCELLE
                                                   ,SYO_MAKINE_PUANTAJ_ID
                                                   ,SYO_PROJE_ID
                                                   ,SYO_LOKASYON_ID) values   
                                                   (@SYO_SAYAC_ID
                                                   ,@SYO_TARIH
                                                   ,@SYO_SAAT
                                                   ,@SYO_OKUNAN_SAYAC
                                                   ,@SYO_FARK_SAYAC
                                                   ,@SYO_ACIKLAMA
                                                   ,@SYO_ELLE_GIRIS
                                                   ,@SYO_OLUSTURAN_ID
                                                   ,@SYO_OLUSTURMA_TARIH                                                  
                                                   ,@SYO_HAREKET_TIP
                                                   ,@SYO_REF_ID
                                                   ,@SYO_REF_GRUP
                                                   ,@SYO_SAYAC_GUNCELLE
                                                   ,@SYO_MAKINE_PUANTAJ_ID
                                                   ,@SYO_PROJE_ID
                                                   ,@SYO_LOKASYON_ID)";

                    prms.Clear();
                    prms.Add("@SYO_SAYAC_ID", entity.SYO_SAYAC_ID);
                    prms.Add("@SYO_TARIH", entity.SYO_TARIH);
                    prms.Add("@SYO_SAAT", entity.SYO_SAAT);
                    prms.Add("@SYO_OKUNAN_SAYAC", entity.SYO_OKUNAN_SAYAC);
                    prms.Add("@SYO_FARK_SAYAC", entity.SYO_FARK_SAYAC);
                    prms.Add("@SYO_ACIKLAMA", entity.SYO_ACIKLAMA);
                    prms.Add("@SYO_ELLE_GIRIS", entity.SYO_ELLE_GIRIS);
                    prms.Add("@SYO_HAREKET_TIP", entity.SYO_HAREKET_TIP);
                    prms.Add("@SYO_REF_ID", entity.SYO_REF_ID);
                    prms.Add("@SYO_REF_GRUP", entity.SYO_REF_GRUP);
                    prms.Add("@SYO_SAYAC_GUNCELLE", "1");
                    prms.Add("@SYO_MAKINE_PUANTAJ_ID", entity.SYO_MAKINE_PUANTAJ_ID);
                    prms.Add("@SYO_PROJE_ID", entity.SYO_PROJE_ID);
                    prms.Add("@SYO_LOKASYON_ID", entity.SYO_LOKASYON_ID);
                    prms.Add("@SYO_OLUSTURAN_ID", entity.SYO_OLUSTURAN_ID);
                    prms.Add("@SYO_OLUSTURMA_TARIH", DateTime.Now);
                    klas.cmd(query, prms.PARAMS);
                    var util = new Util();
					using(var cnn = util.baglan())
                    {
						bildirimEntity.Id = await cnn.QueryFirstAsync<int>("SELECT MAX(TB_SAYAC_OKUMA_ID) FROM orjin.TB_SAYAC_OKUMA");
					}
					bildirimEntity.Aciklama = "Sayac kaydı başarılı bir şekilde gerçekleştirildi.";
                    bildirimEntity.MsgId = Bildirim.MSG_SYO_KAYIT_OK;
                    bildirimEntity.Durum = true;
                }
                else // güncelle 
                {
                    string query = @"UPDATE orjin.TB_SAYAC_OKUMA SET 
                                                    SYO_SAYAC_ID            = @SYO_SAYAC_ID
                                                   ,SYO_TARIH               = @SYO_TARIH
                                                   ,SYO_SAAT                = @SYO_SAAT
                                                   ,SYO_OKUNAN_SAYAC        = @SYO_OKUNAN_SAYAC
                                                   ,SYO_FARK_SAYAC          = @SYO_FARK_SAYAC
                                                   ,SYO_ACIKLAMA            = @SYO_ACIKLAMA
                                                   ,SYO_ELLE_GIRIS          = @SYO_ELLE_GIRIS
                                                   ,SYO_DEGISTIREN_ID       = @SYO_DEGISTIREN_ID
                                                   ,SYO_DEGISTIRME_TARIH    = @SYO_DEGISTIRME_TARIH
                                                   ,SYO_HAREKET_TIP         = @SYO_HAREKET_TIP    
                                                   ,SYO_REF_ID              = @SYO_REF_ID
                                                   ,SYO_REF_GRUP            = @SYO_REF_GRUP
                                                   ,SYO_SAYAC_GUNCELLE      = @SYO_SAYAC_GUNCELLE                                                
                                                   ,SYO_MAKINE_PUANTAJ_ID   = @SYO_MAKINE_PUANTAJ_ID
                                                   ,SYO_PROJE_ID            = @SYO_PROJE_ID 
                                                   ,SYO_LOKASYON_ID         = @SYO_LOKASYON_ID  WHERE TB_SAYAC_OKUMA_ID=@TB_SAYAC_OKUMA_ID ";
                    prms.Clear();

                    prms.Add("@TB_SAYAC_OKUMA_ID", entity.TB_SAYAC_OKUMA_ID);
                    prms.Add("@SYO_SAYAC_ID", entity.SYO_SAYAC_ID);
                    prms.Add("@SYO_TARIH", entity.SYO_TARIH);
                    prms.Add("@SYO_SAAT", entity.SYO_SAAT);
                    prms.Add("@SYO_OKUNAN_SAYAC", entity.SYO_OKUNAN_SAYAC);
                    prms.Add("@SYO_FARK_SAYAC", entity.SYO_FARK_SAYAC);
                    prms.Add("@SYO_ACIKLAMA", entity.SYO_ACIKLAMA);
                    prms.Add("@SYO_ELLE_GIRIS", entity.SYO_ELLE_GIRIS);
                    prms.Add("@SYO_HAREKET_TIP", entity.SYO_HAREKET_TIP);
                    prms.Add("@SYO_REF_ID", entity.SYO_REF_ID);
                    prms.Add("@SYO_REF_GRUP", entity.SYO_REF_GRUP);
                    prms.Add("@SYO_SAYAC_GUNCELLE", entity.SYO_SAYAC_GUNCELLE);
                    prms.Add("@SYO_MAKINE_PUANTAJ_ID", entity.SYO_MAKINE_PUANTAJ_ID);
                    prms.Add("@SYO_PROJE_ID", entity.SYO_PROJE_ID);
                    prms.Add("@SYO_LOKASYON_ID", entity.SYO_LOKASYON_ID);
                    prms.Add("@SYO_DEGISTIREN_ID", entity.SYO_DEGISTIREN_ID);
                    prms.Add("@SYO_DEGISTIRME_TARIH", DateTime.Now);
                    klas.cmd(query, prms.PARAMS);
                    bildirimEntity.Aciklama = "Sayac güncelleme başarılı bir şekilde gerçekleştirildi.";
                    bildirimEntity.MsgId = Bildirim.MSG_SYO_GUNCELLE_OK;
                    bildirimEntity.Durum = true;

                }
            }
            catch (Exception e)
            {
                klas.kapat();
                bildirimEntity.Aciklama = "Sayac kaydı sırasında hata oluştu.Hata : " + e.Message;
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirimEntity.Durum = false;
            }
            return bildirimEntity;
        }


    }
}
