using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Windows.Forms;
using Unity;
using Unity.Policy;
using WebApiNew.App_GlobalResources;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    
    [MyBasicAuthenticationFilter]
    public class StokFisController : ApiController
    {
        Util klas = new Util();
        Parametreler prms = new Parametreler();
        [Route("api/StokFisListesi")]
        [HttpPost]
        public List<StokFis> StokFisListesi([FromBody]Filtre filtre, [FromUri] int page, [FromUri] int pageSize, [FromUri] string IslemTip, [FromUri] int tabDurumID, int ID)
        {

            int ilkdeger = page * pageSize;
            int sondeger = ilkdeger + pageSize;
            prms.Clear();
            prms.Add("KUL_ID", ID);
            prms.Add("SFS_ISLEM_TIP", IslemTip);
            List<StokFis> listem = new List<StokFis>();
            string query = @";WITH mTable AS
            (select *
                ,(select COALESCE(TB_RESIM_ID,-1) from orjin.TB_RESIM where RSM_VARSAYILAN = 1 AND RSM_REF_GRUP = 'PERSONEL' and RSM_REF_ID = TB_PERSONEL_ID ) AS PRS_RESIM_ID 
                ,ROW_NUMBER() OVER (ORDER BY SFS_TARIH DESC, SFS_SAAT DESC) AS RowNum 
            from orjin.VW_STOK_FIS SF LEFT JOIN orjin.VW_PERSONEL P ON SF.SFS_TALEP_EDEN_PERSONEL_ID=P.TB_PERSONEL_ID  where orjin.UDF_LOKASYON_YETKI_KONTROL(SFS_LOKASYON_ID,@KUL_ID) = 1 and SFS_ISLEM_TIP = @SFS_ISLEM_TIP";

            if (filtre != null)
            {
                if (filtre.MakineID > 0)
                {
                    prms.Add("SFS_MAKINE_ID", filtre.MakineID);
                    query = query + " and SFS_MAKINE_ID=@SFS_MAKINE_ID";
                }
                if (filtre.durumID > 0)
                {
                    prms.Add("SFS_TALEP_DURUM_ID", filtre.durumID);
                    query = query + " and SFS_TALEP_DURUM_ID=@SFS_TALEP_DURUM_ID";
                }
                if (tabDurumID != -1)
                {
                    if (tabDurumID == 2)
                        query = query + " and SFS_TALEP_DURUM_ID IN (2,3,4,7,8)";
                    else if (tabDurumID == 3)
                        query = query + " and SFS_TALEP_DURUM_ID IN (5,6,9)";
                    else
                        query = query + " and SFS_TALEP_DURUM_ID IN (1)";
                }
                if (filtre.LokasyonID > 0)
                {
                    prms.Add("SFS_LOKASYON_ID", filtre.LokasyonID);
                    query = query + " and SFS_LOKASYON_ID=@SFS_LOKASYON_ID";
                }
                if (filtre.ProjeID > 0)
                {
                    prms.Add("SFS_PROJE_ID", filtre.ProjeID);
                    query = query + " and SFS_PROJE_ID=@SFS_PROJE_ID";
                }

                if (filtre.nedenID > 0)
                {
                    prms.Add("SFS_TALEP_NEDEN_KOD_ID", filtre.nedenID);
                    query = query + " and SFS_TALEP_NEDEN_KOD_ID=@SFS_TALEP_NEDEN_KOD_ID";
                }

                if (filtre.PersonelID > 0)
                {
                    prms.Add("SFS_TALEP_EDEN_PERSONEL_ID", filtre.PersonelID);
                    query = query + " and SFS_TALEP_EDEN_PERSONEL_ID =@SFS_TALEP_EDEN_PERSONEL_ID";
                }

                if (filtre.BasTarih != "" && filtre.BitTarih != "")
                {
                    DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                    DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                    prms.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
                    prms.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
                    query = query + " AND SFS_TARIH BETWEEN  @BAS_TARIH and @BIT_TARIH";
                }
                else
                if (filtre.BasTarih != "")
                {
                    DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                    prms.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
                    query = query + " AND SFS_TARIH >= @BAS_TARIH ";
                }
                else
                if (filtre.BitTarih != "")
                {
                    DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                    prms.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
                    query = query + " AND SFS_TARIH <= @BIT_TARIH";
                }
                if (filtre.Kelime != "")
                {
                    prms.Add("KELIME", filtre.Kelime);
                    query = query + @" AND     (SFS_FIS_NO      like '%'+@KELIME+'%' OR 
                                                SFS_TALEP_NEDEN like '%'+@KELIME+'%' OR 
                                                SFS_MAKINE      like '%'+@KELIME+'%' OR 
                                                SFS_LOKASYON    like '%'+@KELIME+'%' OR 
                                                SFS_BOLUM       like '%'+@KELIME+'%' OR 
                                                SFS_BASLIK      like '%'+@KELIME+'%' OR 
                                                SFS_TALEP_NEDEN like '%'+@KELIME+'%')";
                }
            }
            prms.Add("ILK_DEGER", ilkdeger);
            prms.Add("SON_DEGER", sondeger);
            query += " )SELECT * FROM mTable WHERE RowNum > @ILK_DEGER AND RowNum <= @SON_DEGER";
            var rtb=new System.Windows.Forms.RichTextBox();
            using (var conn = klas.baglanCmd())
            {
                var dprms = new DynamicParameters();
                prms.PARAMS.ForEach(p => dprms.Add(p.ParametreAdi, p.ParametreDeger));
                listem = conn.Query<StokFis, Personel, StokFis>(query, map: (s, p) =>
                  {
                      try
                      {
                            rtb.Rtf = s.SFS_ACIKLAMA ?? "";
                            s.SFS_ACIKLAMA = rtb.Text;
                      }
                      catch (Exception e)
                      {
                          
                      }

                      s.PERSONEL = p;
                      s.SFS_BASLIK = s.SFS_BASLIK ?? "";
                      s.SFS_BOLUM = s.SFS_BOLUM ?? "";
                      s.SFS_CARI = s.SFS_CARI ?? "";
                      s.SFS_CIKIS_DEPO=s.SFS_CIKIS_DEPO ?? "";
                      s.SFS_DURUM = s.SFS_DURUM ?? "";
                      s.SFS_FATURA_IRSALIYE_NO = s.SFS_FATURA_IRSALIYE_NO ?? "";
                      s.SFS_FIS_NO = s.SFS_FIS_NO ?? "";
                      s.SFS_GC = s.SFS_GC ?? "";
                      s.SFS_GIRIS_DEPO = s.SFS_GIRIS_DEPO ?? "";
                      s.SFS_ISLEM_TIP = s.SFS_ISLEM_TIP ?? "";
                      s.SFS_ISLEM_TIP_DEGER = s.SFS_ISLEM_TIP_DEGER ?? "";
                      s.SFS_KOD_ONCELIK = s.SFS_KOD_ONCELIK ?? "";
                      s.SFS_LOKASYON = s.SFS_LOKASYON ?? "";
                      s.SFS_MAKINE = s.SFS_MAKINE ?? "";
                      s.SFS_MAKINE_KOD = s.SFS_MAKINE_KOD ?? "";
                      s.SFS_ONAY_PERSONEL = s.SFS_ONAY_PERSONEL ?? "";
                      s.SFS_ONCELIK=s.SFS_ONCELIK ?? "";
                      s.SFS_PROFJE_TANIM=s.SFS_PROFJE_TANIM ?? "";
                      s.SFS_REFERANS=s.SFS_REFERANS ?? "";
                      s.SFS_REF_GRUP=s.SFS_REF_GRUP ?? "";
                      s.SFS_SATINALMA_TALEP_EDEN=s.SFS_SATINALMA_TALEP_EDEN ?? "";
                      s.SFS_S_TIP=s.SFS_S_TIP ?? "";
                      s.SFS_TALEP_EDEN=s.SFS_TALEP_EDEN ?? "";
                      s.SFS_TALEP_EDILEN=s.SFS_TALEP_EDILEN ?? "";
                      s.SFS_TALEP_FIS_NO=s.SFS_TALEP_FIS_NO ?? "";
                      s.SFS_TALEP_ISEMRI_NO=s.SFS_TALEP_ISEMRI_NO ?? "";
                      s.SFS_TALEP_NEDEN=s.SFS_TALEP_NEDEN ?? "";
                      s.SFS_TALEP_SIPARIS_NO=s.SFS_TALEP_SIPARIS_NO ?? "";
                      s.SFS_TESLIM_ALAN=s.SFS_TESLIM_ALAN ?? "";
                      s.SFS_TESLIM_YERI=s.SFS_TESLIM_YERI ?? "";
                      s.SFS_TESLIM_YERI_TANIM=s.SFS_TESLIM_YERI_TANIM ?? "";
                      if (p != null)
                      {
                          p.PRS_LOKASYON = p.PRS_LOKASYON ?? "";
                          p.PRS_TIP = p.PRS_TIP ?? "";
                          p.PRS_DEPARTMAN = p.PRS_DEPARTMAN ?? "";
                      }

                      return s;
                  },
                splitOn: "TB_PERSONEL_ID",
                param: dprms
                ).ToList();
            }
            return listem;
        }

        [Route("api/StokFisKayit")]
        [HttpPost]
        public Bildirim StokFisKayit([FromBody] StokFis entity)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {

                if (entity.TB_STOK_FIS_ID < 1)
                {// ekle

                    string query = @"INSERT INTO orjin.TB_STOK_FIS
                                                   ( SFS_GIRIS_DEPO_ID
                                                    ,SFS_CIKIS_DEPO_ID
                                                    ,SFS_CARI_ID
                                                    ,SFS_ISLEM_TIP_KOD_ID
                                                    ,SFS_TARIH
                                                    ,SFS_SAAT
                                                    ,SFS_FIS_NO
                                                    ,SFS_ARA_TOPLAM
                                                    ,SFS_INDIRIM_TOPLAM
                                                    ,SFS_KDV_TOPLAM
                                                    ,SFS_OTV_TOPLAM
                                                    ,SFS_YUVARLAMA_TUTAR
                                                    ,SFS_GENEL_TOPLAM      
                                                    ,SFS_REF_ID      
                                                    ,SFS_ISLEM_TIP
                                                    ,SFS_PARABIRIMI_ID
                                                    ,SFS_PARABIRIMI_KUR
                                                    ,SFS_IPTAL
                                                    ,SFS_KAPALI
                                                    ,SFS_FATURA
                                                    ,SFS_INDIRIM_ORAN
                                                    ,SFS_GC
                                                    ,SFS_OLUSTURAN_ID
                                                    ,SFS_OLUSTURMA_TARIH    
                                                    ,SFS_TALEP_EDEN_PERSONEL_ID
                                                    ,SFS_TALEP_TARIH
                                                    ,SFS_TALEP_NEDEN_KOD_ID      
                                                    ,SFS_TALEP_ONCELIK_ID 
                                                    ,SFS_TALEP_ONAY_PERSONEL_ID
                                                    ,SFS_MODUL_NO
                                                    ,SFS_TALEP_DURUM_ID
                                                    ,SFS_OKUNDU      
                                                    ,SFS_MAKINE_ID
                                                    ,SFS_LOKASYON_ID
                                                    ,SFS_PROJE_ID
                                                    ,SFS_TEKLIF_SAATI     
                                                    ,SFS_BASLIK ) 
                                        values (     @SFS_GIRIS_DEPO_ID
                                                    ,@SFS_CIKIS_DEPO_ID
                                                    ,@SFS_CARI_ID
                                                    ,@SFS_ISLEM_TIP_KOD_ID
                                                    ,@SFS_TARIH
                                                    ,@SFS_SAAT
                                                    ,@SFS_FIS_NO
                                                    ,0
                                                    ,0
                                                    ,0
                                                    ,0
                                                    ,0
                                                    ,0      
                                                    ,-1      
                                                    ,@SFS_ISLEM_TIP
                                                    ,-1
                                                    ,0
                                                    ,0
                                                    ,0
                                                    ,0
                                                    ,0
                                                    ,@SFS_GC
                                                    ,@SFS_OLUSTURAN_ID
                                                    ,@SFS_OLUSTURMA_TARIH    
                                                    ,@SFS_TALEP_EDEN_PERSONEL_ID
                                                    ,@SFS_TALEP_TARIH
                                                    ,@SFS_TALEP_NEDEN_KOD_ID      
                                                    ,@SFS_TALEP_ONCELIK_ID    
                                                    ,-1
                                                    ,@SFS_MODUL_NO
                                                    ,@SFS_TALEP_DURUM_ID
                                                    ,1      
                                                    ,@SFS_MAKINE_ID
                                                    ,@SFS_LOKASYON_ID
                                                    ,@SFS_PROJE_ID
                                                    ,@SFS_TEKLIF_SAATI     
                                                    ,@SFS_BASLIK)";
                    prms.Clear();
                    prms.Add("@SFS_GIRIS_DEPO_ID", entity.SFS_GIRIS_DEPO_ID);
                    prms.Add("@SFS_CIKIS_DEPO_ID", entity.SFS_CIKIS_DEPO_ID);
                    prms.Add("@SFS_CARI_ID", entity.SFS_CARI_ID);
                    prms.Add("@SFS_ISLEM_TIP_KOD_ID", entity.SFS_ISLEM_TIP_KOD_ID);
                    prms.Add("@SFS_TARIH", entity.SFS_TARIH);
                    prms.Add("@SFS_SAAT", entity.SFS_SAAT);
                    prms.Add("@SFS_FIS_NO", entity.SFS_FIS_NO);
                    prms.Add("@SFS_ISLEM_TIP", entity.SFS_ISLEM_TIP);
                    prms.Add("@SFS_GC", entity.SFS_GC);
                    prms.Add("@SFS_OLUSTURAN_ID", entity.SFS_OLUSTURAN_ID);
                    prms.Add("@SFS_OLUSTURMA_TARIH", DateTime.Now);
                    prms.Add("@SFS_TALEP_EDEN_PERSONEL_ID", entity.SFS_TALEP_EDEN_PERSONEL_ID);
                    prms.Add("@SFS_TALEP_TARIH", entity.SFS_TALEP_TARIH);
                    prms.Add("@SFS_TALEP_ONCELIK_ID", entity.SFS_TALEP_ONCELIK_ID);
                    prms.Add("@SFS_TALEP_NEDEN_KOD_ID", entity.SFS_TALEP_NEDEN_KOD_ID);
                    prms.Add("@SFS_MODUL_NO", entity.SFS_MODUL_NO);
                    prms.Add("@SFS_TALEP_DURUM_ID", entity.SFS_TALEP_DURUM_ID);
                    prms.Add("@SFS_MAKINE_ID", entity.SFS_MAKINE_ID);
                    prms.Add("@SFS_LOKASYON_ID", entity.SFS_LOKASYON_ID);
                    prms.Add("@SFS_PROJE_ID", entity.SFS_PROJE_ID);
                    prms.Add("@SFS_TEKLIF_SAATI", entity.SFS_TEKLIF_SAATI);
                    prms.Add("@SFS_BASLIK", entity.SFS_BASLIK);
                    klas.cmd(query, prms.PARAMS);


                    bildirimEntity.Aciklama = "Malzeme talep kaydı başarılı bir şekilde gerçekleştirildi.";
                    bildirimEntity.MsgId = Bildirim.MSG_SFS_KAYIT_OK;
                    bildirimEntity.Durum = true;
                    prms.Clear();
                    bildirimEntity.Id = Convert.ToInt32(klas.GetDataCell("select max(TB_STOK_FIS_ID) from orjin.TB_STOK_FIS", prms.PARAMS));
                    // Bildirim Gönder
                    try
                    {
                        prms.Clear();
                        prms.Add("LOK_ID", entity.SFS_LOKASYON_ID);
                        prms.Add("TB_KULLANICI_ID", entity.SFS_OLUSTURAN_ID);

                        klas.MasterBaglantisi = true;
                        string queryBld = @"select * from orjin.TB_KULLANICI where KLL_ROLBILGISI ='SATINALMA' AND KLL_MOBIL_BILDIRI=1 AND KLL_MOBIL_KULLANICI = 1 and TB_KULLANICI_ID<>@TB_KULLANICI_ID";
                        if (entity.SFS_LOKASYON_ID > 0)
                        {
                            queryBld = queryBld + " and orjin.UDF_LOKASYON_YETKI_KONTROL(@LOK_ID,TB_KULLANICI_ID) = 1";
                        }

                        string[] cihazIDler = klas.GetDataTable(queryBld, prms.PARAMS).AsEnumerable().Select(r => r.Field<string>("KLL_MOBILCIHAZ_ID")).ToArray();
                        klas.MasterBaglantisi = false;
                        if (cihazIDler != null && cihazIDler.Length > 0)
                        {
                            Util.SendNotificationToTopic(entity.SFS_FIS_NO, Localization.YeniMalzemeTalebi, string.Format(Localization.NoluMalzemeTalebiOlusturuldu,entity.SFS_FIS_NO), Util.malzemeTalep, cihazIDler);
                        }
                    }
                    catch (Exception)
                    {
                        klas.kapat();
                    }
                }
                else//güncelle 
                {

                    string query = @"UPDATE orjin.TB_STOK_FIS SET 
                                                     SFS_GIRIS_DEPO_ID            =@SFS_GIRIS_DEPO_ID
                                                    ,SFS_CIKIS_DEPO_ID            =@SFS_CIKIS_DEPO_ID
                                                    ,SFS_CARI_ID                  =@SFS_CARI_ID
                                                    ,SFS_ISLEM_TIP_KOD_ID         =@SFS_ISLEM_TIP_KOD_ID
                                                    ,SFS_TARIH                    =@SFS_TARIH
                                                    ,SFS_SAAT                     =@SFS_SAAT
                                                    ,SFS_FIS_NO                   =@SFS_FIS_NO                                                  
                                                    ,SFS_ISLEM_TIP                =@SFS_ISLEM_TIP                                                 
                                                    ,SFS_GC                       =@SFS_GC
                                                    ,SFS_DEGISTIREN_ID            =@SFS_DEGISTIREN_ID
                                                    ,SFS_DEGISTIRME_TARIH         =@SFS_DEGISTIRME_TARIH    
                                                    ,SFS_TALEP_EDEN_PERSONEL_ID   =@SFS_TALEP_EDEN_PERSONEL_ID
                                                    ,SFS_TALEP_TARIH              =@SFS_TALEP_TARIH
                                                    ,SFS_TALEP_NEDEN_KOD_ID       =@SFS_TALEP_NEDEN_KOD_ID      
                                                    ,SFS_TALEP_ONCELIK_ID         =@SFS_TALEP_ONCELIK_ID   
                                                    ,SFS_MODUL_NO                 =@SFS_MODUL_NO
                                                    ,SFS_TALEP_DURUM_ID           =@SFS_TALEP_DURUM_ID
                                                    ,SFS_MAKINE_ID                =@SFS_MAKINE_ID
                                                    ,SFS_LOKASYON_ID              =@SFS_LOKASYON_ID
                                                    ,SFS_PROJE_ID                 =@SFS_PROJE_ID
                                                    ,SFS_TEKLIF_SAATI             =@SFS_TEKLIF_SAATI     
                                                    ,SFS_BASLIK                   =@SFS_BASLIK WHERE TB_STOK_FIS_ID=@TB_STOK_FIS_ID";

                    prms.Clear();
                    prms.Add("@TB_STOK_FIS_ID", entity.TB_STOK_FIS_ID);
                    prms.Add("@SFS_GIRIS_DEPO_ID", entity.SFS_GIRIS_DEPO_ID);
                    prms.Add("@SFS_CIKIS_DEPO_ID", entity.SFS_CIKIS_DEPO_ID);
                    prms.Add("@SFS_CARI_ID", entity.SFS_CARI_ID);
                    prms.Add("@SFS_ISLEM_TIP_KOD_ID", entity.SFS_ISLEM_TIP_KOD_ID);
                    prms.Add("@SFS_TARIH", entity.SFS_TARIH);
                    prms.Add("@SFS_SAAT", entity.SFS_SAAT);
                    prms.Add("@SFS_FIS_NO", entity.SFS_FIS_NO);
                    prms.Add("@SFS_ISLEM_TIP", entity.SFS_ISLEM_TIP);
                    prms.Add("@SFS_GC", entity.SFS_GC);
                    prms.Add("@SFS_DEGISTIREN_ID", entity.SFS_DEGISTIREN_ID);
                    prms.Add("@SFS_DEGISTIRME_TARIH", DateTime.Now);
                    prms.Add("@SFS_TALEP_EDEN_PERSONEL_ID", entity.SFS_TALEP_EDEN_PERSONEL_ID);
                    prms.Add("@SFS_TALEP_TARIH", entity.SFS_TALEP_TARIH);
                    prms.Add("@SFS_TALEP_NEDEN_KOD_ID", entity.SFS_TALEP_NEDEN_KOD_ID);
                    prms.Add("@SFS_TALEP_ONCELIK_ID", entity.SFS_TALEP_ONCELIK_ID);
                    prms.Add("@SFS_MODUL_NO", entity.SFS_MODUL_NO);
                    prms.Add("@SFS_TALEP_DURUM_ID", entity.SFS_TALEP_DURUM_ID);
                    prms.Add("@SFS_MAKINE_ID", entity.SFS_MAKINE_ID);
                    prms.Add("@SFS_LOKASYON_ID", entity.SFS_LOKASYON_ID);
                    prms.Add("@SFS_PROJE_ID", entity.SFS_PROJE_ID);
                    prms.Add("@SFS_TEKLIF_SAATI", entity.SFS_TEKLIF_SAATI);
                    prms.Add("@SFS_BASLIK", entity.SFS_BASLIK);
                    klas.cmd(query, prms.PARAMS);
                    bildirimEntity.Aciklama = "Malzeme Talep güncelleme başarılı bir şekilde gerçekleştirildi.";
                    bildirimEntity.MsgId = Bildirim.MSG_SFS_GUNCELLE_OK;
                    bildirimEntity.Durum = true;
                }

            }
            catch (Exception e)
            {
                klas.kapat();
                bildirimEntity.Aciklama = String.Format(Localization.errorFormatted,e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }
            return bildirimEntity;
        }
        [Route("api/StokFisMalzemeSayi")]
        [HttpGet]
        public int StokFisMalzemeSayi([FromUri] int _FisID)
        {
            int _sayi = 0;
            try
            {
                prms.Clear();
                prms.Add("SFD_STOK_FIS_ID", _FisID);
                _sayi = Convert.ToInt32(klas.GetDataCell("select COUNT(*) from orjin.TB_STOK_FIS_DETAY where SFD_STOK_FIS_ID =@SFD_STOK_FIS_ID", prms.PARAMS));
                return _sayi;
            }
            catch (Exception)
            { return _sayi = 0; }
        }
        [Route("api/StokFisDetayList")]
        [HttpGet]
        public List<StokFisDetay> StokFisDetayList([FromUri] int _fisID)
        {
            prms.Clear();
            prms.Add("SFD_STOK_FIS_ID", _fisID);
            string sql = "select * from orjin.VW_STOK_FIS_DETAY where SFD_STOK_FIS_ID=@SFD_STOK_FIS_ID";
            List<StokFisDetay> listem = new List<StokFisDetay>();
            DataTable dt = klas.GetDataTable(sql, prms.PARAMS);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                StokFisDetay entity = new StokFisDetay();
                entity.TB_STOK_FIS_DETAY_ID = Convert.ToInt32(dt.Rows[i]["TB_STOK_FIS_DETAY_ID"]);
                entity.SFD_STOK_FIS_ID = Convert.ToInt32(dt.Rows[i]["SFD_STOK_FIS_ID"]);
                entity.SFD_ACIKLAMA = Util.getFieldString(dt.Rows[i], "SFD_ACIKLAMA");
                entity.SFD_STOK_KOD = dt.Rows[i]["SFD_STOK_KOD"] != DBNull.Value ? dt.Rows[i]["SFD_STOK_KOD"].ToString() : "";
                entity.SFD_STOK = dt.Rows[i]["SFD_STOK"] != DBNull.Value ? dt.Rows[i]["SFD_STOK"].ToString() : "";
                entity.SFD_TALEP_EDEN_PERSONEL = dt.Rows[i]["SFD_TALEP_EDEN_PERSONEL"] != DBNull.Value ? dt.Rows[i]["SFD_TALEP_EDEN_PERSONEL"].ToString() : "";
                entity.SFD_DURUM_YAZI = dt.Rows[i]["SFD_DURUM_YAZI"] != DBNull.Value ? dt.Rows[i]["SFD_DURUM_YAZI"].ToString() : "";
                entity.SFD_MAKINE_KOD = dt.Rows[i]["SFD_MAKINE_KOD"] != DBNull.Value ? dt.Rows[i]["SFD_MAKINE_KOD"].ToString() : "";
                entity.SFD_MAKINE_TANIM = dt.Rows[i]["SFD_MAKINE_TANIM"] != DBNull.Value ? dt.Rows[i]["SFD_MAKINE_TANIM"].ToString() : "";
                entity.SFD_KARSILAMA_SEKLI = dt.Rows[i]["SFD_KARSILAMA_SEKLI"] != DBNull.Value ? dt.Rows[i]["SFD_KARSILAMA_SEKLI"].ToString() : "";
                entity.SFD_DURUM = dt.Rows[i]["SFD_DURUM"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["SFD_DURUM"]) : 0;
                entity.SFD_STOK_ID = dt.Rows[i]["SFD_STOK_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["SFD_STOK_ID"]) : -1;
                entity.SFD_BIRIM_KOD_ID = dt.Rows[i]["SFD_BIRIM_KOD_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["SFD_BIRIM_KOD_ID"]) : -1;
                entity.SFD_MAKINE_ID = dt.Rows[i]["SFD_MAKINE_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["SFD_MAKINE_ID"]) : -1;
                entity.SFD_FIS_GRID_KONUM = dt.Rows[i]["SFD_FIS_GRID_KONUM"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["SFD_FIS_GRID_KONUM"]) : 0;
                entity.SFD_MIKTAR = dt.Rows[i]["SFD_MIKTAR"] != DBNull.Value ? Convert.ToDouble(dt.Rows[i]["SFD_MIKTAR"]) : 0;
                entity.SFD_BIRIM = dt.Rows[i]["SFD_BIRIM"] != DBNull.Value ? dt.Rows[i]["SFD_BIRIM"].ToString() : "";
                listem.Add(entity);
            }

            return listem;
        }
        [Route("api/StokFisDetayKayit")]
        [HttpPost]
        public Bildirim StokFisDetayKayit([FromBody] StokFisDetay entity)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                if (entity.TB_STOK_FIS_DETAY_ID < 1)
                {// ekle


                    string query = @"INSERT INTO orjin.TB_STOK_FIS_DETAY
                                                   ( SFD_STOK_FIS_ID
                                                    ,SFD_SATIR_TIP
                                                    ,SFD_STOK_ID
                                                    ,SFD_BIRIM_KOD_ID
                                                    ,SFD_MIKTAR
                                                    ,SFD_ANA_BIRIM_MIKTAR
                                                    ,SFD_BIRIM_FIYAT
                                                    ,SFD_FIS_GRID_KONUM
                                                    ,SFD_KDV_ORAN
                                                    ,SFD_KDV_TUTAR
                                                    ,SFD_OTV_ORAN
                                                    ,SFD_OTV_TUTAR
                                                    ,SFD_INDIRIM_ORAN
                                                    ,SFD_INDIRIM_TUTAR
                                                    ,SFD_KDV_DH
                                                    ,SFD_ARA_TOPLAM
                                                    ,SFD_TOPLAM
                                                    ,SFD_ALINAN_MIKTAR
                                                    ,SFD_ALINAN_ANA_BIRIM_MIKTAR
                                                    ,SFD_OLUSTURAN_ID
                                                    ,SFD_OLUSTURMA_TARIH                                                   
                                                    ,SFD_CARI_ID     
                                                    ,SFD_ALTERNATIF_STOK_ID
                                                    ,SFD_MAKINE_ID
                                                    ,SFD_KARSILAMA_SEKLI
                                                    ,SFD_IPTAL_MIKTARI
                                                    ,SFD_STOKTAN_KULLANIM
                                                    ,SFD_SATINALMA_MIKTARI
                                                    ,SFD_GIREN_MIKTAR
                                                    ,SFD_KALAN_MIKTAR
                                                    ,SFD_DURUM
                                                    ,SFD_SINIF_ID ) 
                                        values (     @SFD_STOK_FIS_ID
                                                    ,@SFD_SATIR_TIP
                                                    ,@SFD_STOK_ID
                                                    ,@SFD_BIRIM_KOD_ID
                                                    ,@SFD_MIKTAR
                                                    ,@SFD_ANA_BIRIM_MIKTAR
                                                    ,@SFD_BIRIM_FIYAT
                                                    ,@SFD_FIS_GRID_KONUM
                                                    ,0
                                                    ,0
                                                    ,0
                                                    ,0
                                                    ,0
                                                    ,0
                                                    ,@SFD_KDV_DH
                                                    ,0
                                                    ,0
                                                    ,0
                                                    ,0
                                                    ,@SFD_OLUSTURAN_ID
                                                    ,@SFD_OLUSTURMA_TARIH                                                    
                                                    ,@SFD_CARI_ID     
                                                    ,-1
                                                    ,@SFD_MAKINE_ID
                                                    ,@SFD_KARSILAMA_SEKLI
                                                    ,0
                                                    ,0
                                                    ,@SFD_SATINALMA_MIKTARI
                                                    ,0
                                                    ,0
                                                    ,@SFD_DURUM
                                                    ,-1)";
                    prms.Clear();
                    prms.Add("@SFD_STOK_FIS_ID", entity.SFD_STOK_FIS_ID);
                    prms.Add("@SFD_SATIR_TIP", entity.SFD_SATIR_TIP);
                    prms.Add("@SFD_STOK_ID", entity.SFD_STOK_ID);
                    prms.Add("@SFD_BIRIM_KOD_ID", entity.SFD_BIRIM_KOD_ID);
                    prms.Add("@SFD_MIKTAR", entity.SFD_MIKTAR);
                    prms.Add("@SFD_ANA_BIRIM_MIKTAR", entity.SFD_ANA_BIRIM_MIKTAR);
                    prms.Add("@SFD_BIRIM_FIYAT", entity.SFD_BIRIM_FIYAT);
                    prms.Add("@SFD_FIS_GRID_KONUM", entity.SFD_FIS_GRID_KONUM);
                    prms.Add("@SFD_OLUSTURAN_ID", entity.SFD_OLUSTURAN_ID);
                    prms.Add("@SFD_OLUSTURMA_TARIH", DateTime.Now);
                    prms.Add("@SFD_CARI_ID", entity.SFD_CARI_ID);
                    prms.Add("@SFD_MAKINE_ID", entity.SFD_MAKINE_ID);
                    prms.Add("@SFD_KARSILAMA_SEKLI", entity.SFD_KARSILAMA_SEKLI);
                    prms.Add("@SFD_IPTAL_MIKTARI", entity.SFD_IPTAL_MIKTARI);
                    prms.Add("@SFD_STOKTAN_KULLANIM", entity.SFD_STOKTAN_KULLANIM);
                    prms.Add("@SFD_SATINALMA_MIKTARI", entity.SFD_MIKTAR);
                    prms.Add("@SFD_DURUM", entity.SFD_DURUM);
                    prms.Add("@SFD_KDV_DH", entity.SFD_KDV_DH);
                    klas.cmd(query, prms.PARAMS);
                    bildirimEntity.Aciklama = "Talep malzeme kaydı başarılı bir şekilde gerçekleştirildi.";
                    bildirimEntity.MsgId = Bildirim.MSG_SFS_MLZ_KAYIT_OK;
                    bildirimEntity.Durum = true;
                }
                else // güncelle 
                {

                    string query = @"UPDATE orjin.TB_STOK_FIS_DETAY SET 
                                                     SFD_STOK_FIS_ID         =@SFD_STOK_FIS_ID
                                                    ,SFD_SATIR_TIP           =@SFD_SATIR_TIP
                                                    ,SFD_STOK_ID             =@SFD_STOK_ID
                                                    ,SFD_BIRIM_KOD_ID        =@SFD_BIRIM_KOD_ID
                                                    ,SFD_MIKTAR              =@SFD_MIKTAR
                                                    ,SFD_ANA_BIRIM_MIKTAR    =@SFD_ANA_BIRIM_MIKTAR
                                                    ,SFD_BIRIM_FIYAT         =@SFD_BIRIM_FIYAT                                                  
                                                    ,SFD_FIS_GRID_KONUM      =@SFD_FIS_GRID_KONUM                                                 
                                                    ,SFD_OLUSTURAN_ID        =@SFD_OLUSTURAN_ID
                                                    ,SFD_OLUSTURMA_TARIH     =@SFD_OLUSTURMA_TARIH
                                                    ,SFD_CARI_ID             =@SFD_CARI_ID                                                      
                                                    ,SFD_MAKINE_ID           =@SFD_MAKINE_ID
                                                    ,SFD_KARSILAMA_SEKLI     =@SFD_KARSILAMA_SEKLI      
                                                    ,SFD_SATINALMA_MIKTARI   =@SFD_SATINALMA_MIKTARI   
                                                    ,SFD_DURUM               =@SFD_DURUM ,SFD_ACIKLAMA=@SFD_ACIKLAMA  WHERE TB_STOK_FIS_DETAY_ID = @TB_STOK_FIS_DETAY_ID";
                    prms.Clear();
                    prms.Add("@TB_STOK_FIS_DETAY_ID", entity.TB_STOK_FIS_DETAY_ID);
                    prms.Add("@SFD_STOK_FIS_ID", entity.SFD_STOK_FIS_ID);
                    prms.Add("@SFD_SATIR_TIP", entity.SFD_SATIR_TIP);
                    prms.Add("@SFD_STOK_ID", entity.SFD_STOK_ID);
                    prms.Add("@SFD_BIRIM_KOD_ID", entity.SFD_BIRIM_KOD_ID);
                    prms.Add("@SFD_MIKTAR", entity.SFD_MIKTAR);
                    prms.Add("@SFD_ANA_BIRIM_MIKTAR", entity.SFD_ANA_BIRIM_MIKTAR);
                    prms.Add("@SFD_BIRIM_FIYAT", entity.SFD_BIRIM_FIYAT);
                    prms.Add("@SFD_FIS_GRID_KONUM", entity.SFD_FIS_GRID_KONUM);
                    prms.Add("@SFD_OLUSTURAN_ID", entity.SFD_OLUSTURAN_ID);
                    prms.Add("@SFD_OLUSTURMA_TARIH", DateTime.Now);
                    prms.Add("@SFD_CARI_ID", entity.SFD_CARI_ID);
                    prms.Add("@SFD_MAKINE_ID", entity.SFD_MAKINE_ID);
                    prms.Add("@SFD_KARSILAMA_SEKLI", entity.SFD_KARSILAMA_SEKLI);
                    prms.Add("@SFD_IPTAL_MIKTARI", entity.SFD_IPTAL_MIKTARI);
                    prms.Add("@SFD_STOKTAN_KULLANIM", entity.SFD_STOKTAN_KULLANIM);
                    prms.Add("@SFD_SATINALMA_MIKTARI", entity.SFD_MIKTAR);
                    prms.Add("@SFD_DURUM", entity.SFD_DURUM);
                    prms.Add("@SFD_ACIKLAMA", entity.SFD_ACIKLAMA);
                    klas.cmd(query, prms.PARAMS);
                    bildirimEntity.Aciklama = "Talep malzeme güncelleme başarılı bir şekilde gerçekleştirildi.";
                    bildirimEntity.MsgId = Bildirim.MSG_SFS_MLZ_GUNCELLE_OK;
                    bildirimEntity.Durum = true;
                }
            }
            catch (Exception e)
            {
                klas.kapat();
                bildirimEntity.Aciklama = string.Format(Localization.errorFormatted, e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }
            return bildirimEntity;
        }
        [Route("api/StokFisDetaySil")]
        [HttpPost]
        public Bildirim StokFisDetaySil([FromUri] int fisDetayID)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                prms.Clear();
                prms.Add("TB_STOK_FIS_DETAY_ID", fisDetayID);
                klas.cmd("DELETE FROM orjin.TB_STOK_FIS_DETAY WHERE TB_STOK_FIS_DETAY_ID = @TB_STOK_FIS_DETAY_ID", prms.PARAMS);
                bildirimEntity.Aciklama = "Talep malzeme silme başarılı bir şekilde gerçekleştirildi.";
                bildirimEntity.MsgId = Bildirim.MSG_SFS_MLZ_SIL_OK;
                bildirimEntity.Durum = true;
            }
            catch (Exception e)
            {
                bildirimEntity.Aciklama = "Talep malzeme silme sırasında hata oluştu.Hata : " + e.Message;
                bildirimEntity.MsgId = Bildirim.MSG_SFS_MLZ_SIL_HATA;
                bildirimEntity.Durum = false;
            }
            return bildirimEntity;
        }
        [Route("api/TalepKodGetir")]
        public TanimDeger TalepKodGetir()
        {
            var sql = "";
            prms.Clear();
            klas.cmd("UPDATE orjin.TB_NUMARATOR SET NMR_NUMARA = NMR_NUMARA+1 WHERE NMR_KOD = 'SFS_TALEP_FIS_NO'", prms.PARAMS);
            string ss = "";
            sql = @"select 
                        NMR_ON_EK+right(replicate('0',NMR_HANE_SAYISI)+CAST(NMR_NUMARA AS VARCHAR(MAX)),NMR_HANE_SAYISI) as deger FROM orjin.TB_NUMARATOR
                        WHERE NMR_KOD = 'SFS_TALEP_FIS_NO'";
            DataRow dr = klas.GetDataRow(sql, prms.PARAMS);
            if (dr != null)
            {
                ss = dr["deger"].ToString();
            }
            TanimDeger entity = new TanimDeger();
            entity.Tanim = ss;
            return entity;
        }
        [Route("api/StokFisSil")]
        [HttpPost]
        public Bildirim StokFisSil([FromUri] int fisID)
        {
            Bildirim bildirimEntity = new Bildirim();
            prms.Clear();
            prms.Add("TB_STOK_FIS_ID", fisID);
            try
            {
                klas.cmd("DELETE FROM orjin.TB_STOK_FIS_DETAY WHERE SFD_STOK_FIS_ID = @TB_STOK_FIS_ID", prms.PARAMS);
                klas.cmd("DELETE FROM orjin.TB_STOK_FIS WHERE TB_STOK_FIS_ID = @TB_STOK_FIS_ID", prms.PARAMS);
                bildirimEntity.Aciklama = "Talep silme başarılı bir şekilde gerçekleştirildi.";
                bildirimEntity.MsgId = Bildirim.MSG_SFS_SIL_OK;
                bildirimEntity.Durum = true;
            }
            catch (Exception e)
            {
                bildirimEntity.Aciklama = String.Format(Localization.errorFormatted,e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_SFS_SIL_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }
            return bildirimEntity;
        }
        [Route("api/StokFisOnayListesi")]
        [HttpPost]
        public List<StokFis> StokFisOnayListesi([FromBody] Filtre filtre, [FromUri] int page, [FromUri] int pageSize, [FromUri] string IslemTip, [FromUri] int tabDurumID, [FromUri] int ID)
        {
           

            int ilkdeger = page * pageSize;
            int sondeger = ilkdeger + pageSize;
            prms.Clear();
            prms.Add("KUL_ID", ID);
            prms.Add("SFS_ISLEM_TIP", IslemTip);
            List<StokFis> listem = new List<StokFis>();
            string query = " ( select * FROM orjin.VW_STOK_FIS STF " +
               " LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID  " +
               "  LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID  " +
               $" WHERE SFS_ISLEM_TIP = {IslemTip} " +
                " AND SFS_MODUL_NO = 1 " +
                $" AND SOL_PERSONEL_ID = {ID}";

            //if (filtre != null)
            //{
            //    if (filtre.MakineID > 0)
            //    {
            //        prms.Add("SFS_MAKINE_ID", filtre.MakineID);
            //        query = query + " and SFS_MAKINE_ID=@SFS_MAKINE_ID";
            //    }
            //    if (filtre.durumID > 0)
            //    {
            //        prms.Add("SFS_TALEP_DURUM_ID", filtre.durumID);
            //        query = query + " and SFS_TALEP_DURUM_ID=@SFS_TALEP_DURUM_ID";
            //    }

            if (tabDurumID != -1)
            {
                if (tabDurumID == 1)
                    query = query + " and SFS_TALEP_DURUM_ID = 7 AND ((SOL_ONAY_DURUM_ID != 8 AND SOL_ONAY_DURUM_ID != 9) or SOL_ONAY_DURUM_ID is null) ";
                else if (tabDurumID == 2)
                    query = query + " and SOL_ONAY_DURUM_ID IN (8,9) ";
            }

            //    if (filtre.LokasyonID > 0)
            //    {
            //        prms.Add("SFS_LOKASYON_ID", filtre.LokasyonID);
            //        query = query + " and SFS_LOKASYON_ID=@SFS_LOKASYON_ID";
            //    }
            //    if (filtre.ProjeID > 0)
            //    {
            //        prms.Add("SFS_PROJE_ID", filtre.ProjeID);
            //        query = query + " and SFS_PROJE_ID=@SFS_PROJE_ID";
            //    }

            //    if (filtre.nedenID > 0)
            //    {
            //        prms.Add("SFS_TALEP_NEDEN_KOD_ID", filtre.nedenID);
            //        query = query + " and SFS_TALEP_NEDEN_KOD_ID=@SFS_TALEP_NEDEN_KOD_ID";
            //    }

            //    if (filtre.PersonelID > 0)
            //    {
            //        prms.Add("SFS_TALEP_EDEN_PERSONEL_ID", filtre.PersonelID);
            //        query = query + " and SFS_TALEP_EDEN_PERSONEL_ID =@SFS_TALEP_EDEN_PERSONEL_ID";
            //    }

            //    if (filtre.BasTarih != "" && filtre.BitTarih != "")
            //    {
            //        DateTime bas = Convert.ToDateTime(filtre.BasTarih);
            //        DateTime bit = Convert.ToDateTime(filtre.BitTarih);
            //        prms.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
            //        prms.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
            //        query = query + " AND SFS_TARIH BETWEEN  @BAS_TARIH and @BIT_TARIH";
            //    }
            //    else
            //    if (filtre.BasTarih != "")
            //    {
            //        DateTime bas = Convert.ToDateTime(filtre.BasTarih);
            //        prms.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
            //        query = query + " AND SFS_TARIH >= @BAS_TARIH ";
            //    }
            //    else
            //    if (filtre.BitTarih != "")
            //    {
            //        DateTime bit = Convert.ToDateTime(filtre.BitTarih);
            //        prms.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
            //        query = query + " AND SFS_TARIH <= @BIT_TARIH";
            //    }
            //    if (filtre.Kelime != "")
            //    {
            //        prms.Add("KELIME", filtre.Kelime);
            //        query = query + @" AND     (SFS_FIS_NO      like '%'+@KELIME+'%' OR 
            //                                    SFS_TALEP_NEDEN like '%'+@KELIME+'%' OR 
            //                                    SFS_MAKINE      like '%'+@KELIME+'%' OR 
            //                                    SFS_LOKASYON    like '%'+@KELIME+'%' OR 
            //                                    SFS_BOLUM       like '%'+@KELIME+'%' OR 
            //                                    SFS_BASLIK      like '%'+@KELIME+'%' OR 
            //                                    SFS_TALEP_NEDEN like '%'+@KELIME+'%')";
            //    }
            //}

            prms.Add("ILK_DEGER", ilkdeger);
            prms.Add("SON_DEGER", sondeger);
            query += $" ) SELECT * FROM mTable WHERE RowNum > {ilkdeger} AND RowNum <= {sondeger} ";
            

            using (var command = new SqlCommand(query, klas.baglanCmd()))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader["STO_SIRA_NO"] == DBNull.Value || Convert.ToInt32(reader["STO_SIRA_NO"]) == 1 || Convert.ToInt32(reader["STO_SIRA_NO"]) == 0)
                        {
                            var s = new StokFis();
                            s.TB_STOK_FIS_ID = Convert.ToInt32(reader["TB_STOK_FIS_ID"]);
                            s.SFS_BASLIK = reader["SFS_BASLIK"].ToString();
                            s.SFS_BOLUM = reader["SFS_BOLUM"].ToString();
                            s.SFS_CARI = reader["SFS_CARI"].ToString();
                            s.SFS_CIKIS_DEPO = reader["SFS_CIKIS_DEPO"].ToString();
                            if (reader["SOL_ONAY_DURUM_ID"] != DBNull.Value)
                            {
                                if (Convert.ToInt32(reader["SOL_ONAY_DURUM_ID"]) == 9)
                                {
                                    s.SFS_DURUM = "ONAYLANMADI";
                                }
                                else if (Convert.ToInt32(reader["SOL_ONAY_DURUM_ID"]) == 8)
                                {
                                    s.SFS_DURUM = "ONAYLANDI";
                                }
                                else
                                {
                                    s.SFS_DURUM = "ONAY BEKLİYOR";
                                }
                            } else { s.SFS_DURUM = "ONAY BEKLİYOR"; }

                            s.SFS_FATURA_IRSALIYE_NO = reader["SFS_FATURA_IRSALIYE_NO"].ToString();
                            s.SFS_FIS_NO = reader["SFS_FIS_NO"].ToString();
                            s.SFS_GC = reader["SFS_GC"].ToString();
                            s.SFS_GIRIS_DEPO = reader["SFS_GIRIS_DEPO"].ToString();
                            s.SFS_ISLEM_TIP = reader["SFS_ISLEM_TIP"].ToString();
                            s.SFS_ISLEM_TIP_DEGER = reader["SFS_ISLEM_TIP_DEGER"].ToString();
                            s.SFS_KOD_ONCELIK = reader["SFS_KOD_ONCELIK"].ToString();
                            s.SFS_LOKASYON = reader["SFS_LOKASYON"].ToString();
                            s.SFS_MAKINE = reader["SFS_MAKINE"].ToString();
                            s.SFS_MAKINE_KOD = reader["SFS_MAKINE_KOD"].ToString();
                            s.SFS_ONAY_PERSONEL = reader["SFS_ONAY_PERSONEL"].ToString();
                            s.SFS_ONCELIK = reader["SFS_ONCELIK"].ToString();
                            s.SFS_PROFJE_TANIM = reader["SFS_PROFJE_TANIM"].ToString();
                            s.SFS_REFERANS = reader["SFS_REFERANS"].ToString();
                            s.SFS_REF_GRUP = reader["SFS_REF_GRUP"].ToString();
                            s.SFS_SATINALMA_TALEP_EDEN = reader["SFS_SATINALMA_TALEP_EDEN"].ToString();
                            s.SFS_S_TIP = reader["SFS_S_TIP"].ToString();
                            s.SFS_TALEP_EDEN = reader["SFS_TALEP_EDEN"].ToString();
                            s.SFS_TALEP_EDILEN = reader["SFS_TALEP_EDILEN"].ToString();
                            s.SFS_TALEP_FIS_NO = reader["SFS_TALEP_FIS_NO"].ToString();
                            s.SFS_TALEP_ISEMRI_NO = reader["SFS_TALEP_ISEMRI_NO"].ToString();
                            s.SFS_TALEP_NEDEN = reader["SFS_TALEP_NEDEN"].ToString();
                            s.SFS_TALEP_SIPARIS_NO = reader["SFS_TALEP_SIPARIS_NO"].ToString();
                            s.SFS_TESLIM_ALAN = reader["SFS_TESLIM_ALAN"].ToString();
                            s.SFS_TESLIM_YERI = reader["SFS_TESLIM_YERI"].ToString();
                            s.SFS_TESLIM_YERI_TANIM = reader["SFS_TESLIM_YERI_TANIM"].ToString();
                            s.SFS_TARIH = Convert.ToDateTime(reader["SFS_TARIH"]);
                            s.SFS_SAAT = (reader["SFS_SAAT"]).ToString();
                            listem.Add(s);
                        }
                        else
                        {
                           
                            prms.Clear();
                            prms.Add("STOK_FIS_ID", Convert.ToInt32(reader["TB_STOK_FIS_ID"]));
                            int isStokFis ;
                            int stokFisSayisi = 0;

                            if (reader["SOL_ONAY_DURUM_ID"] != DBNull.Value && ((Convert.ToInt32(reader["SOL_ONAY_DURUM_ID"]) == 9) || Convert.ToInt32(reader["SOL_ONAY_DURUM_ID"]) == 8))
                            {
                                var s = new StokFis();
                                s.TB_STOK_FIS_ID = Convert.ToInt32(reader["TB_STOK_FIS_ID"]);
                                s.SFS_BASLIK = reader["SFS_BASLIK"].ToString();
                                s.SFS_BOLUM = reader["SFS_BOLUM"].ToString();
                                s.SFS_CARI = reader["SFS_CARI"].ToString();
                                s.SFS_CIKIS_DEPO = reader["SFS_CIKIS_DEPO"].ToString();
                                if (reader["SOL_ONAY_DURUM_ID"] != DBNull.Value)
                                {
                                    if (Convert.ToInt32(reader["SOL_ONAY_DURUM_ID"]) == 9)
                                    {
                                        s.SFS_DURUM = "ONAYLANMADI";
                                    }
                                    else if (Convert.ToInt32(reader["SOL_ONAY_DURUM_ID"]) == 8)
                                    {
                                        s.SFS_DURUM = "ONAYLANDI";
                                    }
                                    else
                                    {
                                        s.SFS_DURUM = "ONAY BEKLİYOR";
                                    }
                                }
                                else { s.SFS_DURUM = "ONAY BEKLİYOR"; }

                                s.SFS_FATURA_IRSALIYE_NO = reader["SFS_FATURA_IRSALIYE_NO"].ToString();
                                s.SFS_FIS_NO = reader["SFS_FIS_NO"].ToString();
                                s.SFS_GC = reader["SFS_GC"].ToString();
                                s.SFS_GIRIS_DEPO = reader["SFS_GIRIS_DEPO"].ToString();
                                s.SFS_ISLEM_TIP = reader["SFS_ISLEM_TIP"].ToString();
                                s.SFS_ISLEM_TIP_DEGER = reader["SFS_ISLEM_TIP_DEGER"].ToString();
                                s.SFS_KOD_ONCELIK = reader["SFS_KOD_ONCELIK"].ToString();
                                s.SFS_LOKASYON = reader["SFS_LOKASYON"].ToString();
                                s.SFS_MAKINE = reader["SFS_MAKINE"].ToString();
                                s.SFS_MAKINE_KOD = reader["SFS_MAKINE_KOD"].ToString();
                                s.SFS_ONAY_PERSONEL = reader["SFS_ONAY_PERSONEL"].ToString();
                                s.SFS_ONCELIK = reader["SFS_ONCELIK"].ToString();
                                s.SFS_PROFJE_TANIM = reader["SFS_PROFJE_TANIM"].ToString();
                                s.SFS_REFERANS = reader["SFS_REFERANS"].ToString();
                                s.SFS_REF_GRUP = reader["SFS_REF_GRUP"].ToString();
                                s.SFS_SATINALMA_TALEP_EDEN = reader["SFS_SATINALMA_TALEP_EDEN"].ToString();
                                s.SFS_S_TIP = reader["SFS_S_TIP"].ToString();
                                s.SFS_TALEP_EDEN = reader["SFS_TALEP_EDEN"].ToString();
                                s.SFS_TALEP_EDILEN = reader["SFS_TALEP_EDILEN"].ToString();
                                s.SFS_TALEP_FIS_NO = reader["SFS_TALEP_FIS_NO"].ToString();
                                s.SFS_TALEP_ISEMRI_NO = reader["SFS_TALEP_ISEMRI_NO"].ToString();
                                s.SFS_TALEP_NEDEN = reader["SFS_TALEP_NEDEN"].ToString();
                                s.SFS_TALEP_SIPARIS_NO = reader["SFS_TALEP_SIPARIS_NO"].ToString();
                                s.SFS_TESLIM_ALAN = reader["SFS_TESLIM_ALAN"].ToString();
                                s.SFS_TESLIM_YERI = reader["SFS_TESLIM_YERI"].ToString();
                                s.SFS_TESLIM_YERI_TANIM = reader["SFS_TESLIM_YERI_TANIM"].ToString();
                                s.SFS_TARIH = Convert.ToDateTime(reader["SFS_TARIH"]);
                                s.SFS_SAAT = (reader["SFS_SAAT"]).ToString();
                                listem.Add(s);
                            }  

                            else
                            {

                                if (Convert.ToInt32(reader["STO_SIRA_NO"]) == 2)
                                {

                                    isStokFis = Convert.ToInt32(klas.GetDataCell(
                                   @" select count(*) FROM orjin.VW_STOK_FIS STF 
                                    LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID 
                                    LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID 
                                    where SFS_ISLEM_TIP = '09'
                                    AND SFS_MODUL_NO = 1 
                                    AND TB_STOK_FIS_ID = @STOK_FIS_ID 
                                    AND STO_SIRA_NO = 1", prms.PARAMS));

                                    if (isStokFis > 0)
                                    {
                                        stokFisSayisi = Convert.ToInt32(klas.GetDataCell(
                                      @" select count(*) FROM orjin.VW_STOK_FIS STF 
                                        LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID 
                                        LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID 
                                        where SFS_ISLEM_TIP = '09'
                                        AND SFS_MODUL_NO = 1 
                                        AND TB_STOK_FIS_ID = @STOK_FIS_ID 
                                        AND (SOL_ONAY_DURUM_ID = 8) 
                                        AND STO_SIRA_NO = 1", prms.PARAMS));
                                    }
                                    else
                                    {
                                        var s = new StokFis();
                                        s.TB_STOK_FIS_ID = Convert.ToInt32(reader["TB_STOK_FIS_ID"]);
                                        s.SFS_BASLIK = reader["SFS_BASLIK"].ToString();
                                        s.SFS_BOLUM = reader["SFS_BOLUM"].ToString();
                                        s.SFS_CARI = reader["SFS_CARI"].ToString();
                                        s.SFS_CIKIS_DEPO = reader["SFS_CIKIS_DEPO"].ToString();
                                        s.SFS_DURUM = reader["SFS_DURUM"].ToString();
                                        s.SFS_FATURA_IRSALIYE_NO = reader["SFS_FATURA_IRSALIYE_NO"].ToString();
                                        s.SFS_FIS_NO = reader["SFS_FIS_NO"].ToString();
                                        s.SFS_GC = reader["SFS_GC"].ToString();
                                        s.SFS_GIRIS_DEPO = reader["SFS_GIRIS_DEPO"].ToString();
                                        s.SFS_ISLEM_TIP = reader["SFS_ISLEM_TIP"].ToString();
                                        s.SFS_ISLEM_TIP_DEGER = reader["SFS_ISLEM_TIP_DEGER"].ToString();
                                        s.SFS_KOD_ONCELIK = reader["SFS_KOD_ONCELIK"].ToString();
                                        s.SFS_LOKASYON = reader["SFS_LOKASYON"].ToString();
                                        s.SFS_MAKINE = reader["SFS_MAKINE"].ToString();
                                        s.SFS_MAKINE_KOD = reader["SFS_MAKINE_KOD"].ToString();
                                        s.SFS_ONAY_PERSONEL = reader["SFS_ONAY_PERSONEL"].ToString();
                                        s.SFS_ONCELIK = reader["SFS_ONCELIK"].ToString();
                                        s.SFS_PROFJE_TANIM = reader["SFS_PROFJE_TANIM"].ToString();
                                        s.SFS_REFERANS = reader["SFS_REFERANS"].ToString();
                                        s.SFS_REF_GRUP = reader["SFS_REF_GRUP"].ToString();
                                        s.SFS_SATINALMA_TALEP_EDEN = reader["SFS_SATINALMA_TALEP_EDEN"].ToString();
                                        s.SFS_S_TIP = reader["SFS_S_TIP"].ToString();
                                        s.SFS_TALEP_EDEN = reader["SFS_TALEP_EDEN"].ToString();
                                        s.SFS_TALEP_EDILEN = reader["SFS_TALEP_EDILEN"].ToString();
                                        s.SFS_TALEP_FIS_NO = reader["SFS_TALEP_FIS_NO"].ToString();
                                        s.SFS_TALEP_ISEMRI_NO = reader["SFS_TALEP_ISEMRI_NO"].ToString();
                                        s.SFS_TALEP_NEDEN = reader["SFS_TALEP_NEDEN"].ToString();
                                        s.SFS_TALEP_SIPARIS_NO = reader["SFS_TALEP_SIPARIS_NO"].ToString();
                                        s.SFS_TESLIM_ALAN = reader["SFS_TESLIM_ALAN"].ToString();
                                        s.SFS_TESLIM_YERI = reader["SFS_TESLIM_YERI"].ToString();
                                        s.SFS_TESLIM_YERI_TANIM = reader["SFS_TESLIM_YERI_TANIM"].ToString();
                                        s.SFS_TARIH = Convert.ToDateTime(reader["SFS_TARIH"]);
                                        s.SFS_SAAT = (reader["SFS_SAAT"]).ToString();
                                        listem.Add(s);
                                    }
                                    if (stokFisSayisi > 0)
                                    {
                                        var s = new StokFis();
                                        s.TB_STOK_FIS_ID = Convert.ToInt32(reader["TB_STOK_FIS_ID"]);
                                        s.SFS_BASLIK = reader["SFS_BASLIK"].ToString();
                                        s.SFS_BOLUM = reader["SFS_BOLUM"].ToString();
                                        s.SFS_CARI = reader["SFS_CARI"].ToString();
                                        s.SFS_CIKIS_DEPO = reader["SFS_CIKIS_DEPO"].ToString();
                                        s.SFS_DURUM = reader["SFS_DURUM"].ToString();
                                        s.SFS_FATURA_IRSALIYE_NO = reader["SFS_FATURA_IRSALIYE_NO"].ToString();
                                        s.SFS_FIS_NO = reader["SFS_FIS_NO"].ToString();
                                        s.SFS_GC = reader["SFS_GC"].ToString();
                                        s.SFS_GIRIS_DEPO = reader["SFS_GIRIS_DEPO"].ToString();
                                        s.SFS_ISLEM_TIP = reader["SFS_ISLEM_TIP"].ToString();
                                        s.SFS_ISLEM_TIP_DEGER = reader["SFS_ISLEM_TIP_DEGER"].ToString();
                                        s.SFS_KOD_ONCELIK = reader["SFS_KOD_ONCELIK"].ToString();
                                        s.SFS_LOKASYON = reader["SFS_LOKASYON"].ToString();
                                        s.SFS_MAKINE = reader["SFS_MAKINE"].ToString();
                                        s.SFS_MAKINE_KOD = reader["SFS_MAKINE_KOD"].ToString();
                                        s.SFS_ONAY_PERSONEL = reader["SFS_ONAY_PERSONEL"].ToString();
                                        s.SFS_ONCELIK = reader["SFS_ONCELIK"].ToString();
                                        s.SFS_PROFJE_TANIM = reader["SFS_PROFJE_TANIM"].ToString();
                                        s.SFS_REFERANS = reader["SFS_REFERANS"].ToString();
                                        s.SFS_REF_GRUP = reader["SFS_REF_GRUP"].ToString();
                                        s.SFS_SATINALMA_TALEP_EDEN = reader["SFS_SATINALMA_TALEP_EDEN"].ToString();
                                        s.SFS_S_TIP = reader["SFS_S_TIP"].ToString();
                                        s.SFS_TALEP_EDEN = reader["SFS_TALEP_EDEN"].ToString();
                                        s.SFS_TALEP_EDILEN = reader["SFS_TALEP_EDILEN"].ToString();
                                        s.SFS_TALEP_FIS_NO = reader["SFS_TALEP_FIS_NO"].ToString();
                                        s.SFS_TALEP_ISEMRI_NO = reader["SFS_TALEP_ISEMRI_NO"].ToString();
                                        s.SFS_TALEP_NEDEN = reader["SFS_TALEP_NEDEN"].ToString();
                                        s.SFS_TALEP_SIPARIS_NO = reader["SFS_TALEP_SIPARIS_NO"].ToString();
                                        s.SFS_TESLIM_ALAN = reader["SFS_TESLIM_ALAN"].ToString();
                                        s.SFS_TESLIM_YERI = reader["SFS_TESLIM_YERI"].ToString();
                                        s.SFS_TESLIM_YERI_TANIM = reader["SFS_TESLIM_YERI_TANIM"].ToString();
                                        s.SFS_TARIH = Convert.ToDateTime(reader["SFS_TARIH"]);
                                        s.SFS_SAAT = (reader["SFS_SAAT"]).ToString();
                                        listem.Add(s);
                                    }

                                }
                                else if (Convert.ToInt32(reader["STO_SIRA_NO"]) == 3)
                                {
                                    int approvedFirstOrder = 0;
                                    int firstOrder = 0;
                                    int approvedSecondOrder = 0;
                                    int secondOrder = 0;

                                    isStokFis = Convert.ToInt32(klas.GetDataCell(
                                   @" select count(*) FROM orjin.VW_STOK_FIS STF 
                                        LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID 
                                        LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID 
                                        where SFS_ISLEM_TIP = '09'
                                        AND SFS_MODUL_NO = 1 
                                        AND TB_STOK_FIS_ID = @STOK_FIS_ID 
                                        AND STO_SIRA_NO in (1,2)", prms.PARAMS));

                                    if (isStokFis > 0)
                                    {
                                        firstOrder = Convert.ToInt32(klas.GetDataCell(@" select count(*) FROM orjin.VW_STOK_FIS STF 
                                        LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID 
                                        LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID 
                                        where SFS_ISLEM_TIP = '09'
                                        AND SFS_MODUL_NO = 1 
                                        AND TB_STOK_FIS_ID = @STOK_FIS_ID 
                                        AND STO_SIRA_NO = 1", prms.PARAMS));

                                        approvedFirstOrder = Convert.ToInt32(klas.GetDataCell(@" select count(*) FROM orjin.VW_STOK_FIS STF 
                                        LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID 
                                        LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID 
                                        where SFS_ISLEM_TIP = '09'
                                        AND SFS_MODUL_NO = 1 
                                        AND TB_STOK_FIS_ID = @STOK_FIS_ID 
                                        AND (SOL_ONAY_DURUM_ID = 8) 
                                        AND STO_SIRA_NO = 1", prms.PARAMS));

                                        secondOrder = Convert.ToInt32(klas.GetDataCell(@" select count(*) FROM orjin.VW_STOK_FIS STF 
                                        LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID 
                                        LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID 
                                        where SFS_ISLEM_TIP = '09'
                                        AND SFS_MODUL_NO = 1 
                                        AND TB_STOK_FIS_ID = @STOK_FIS_ID 
                                        AND STO_SIRA_NO = 2", prms.PARAMS));

                                        approvedSecondOrder = Convert.ToInt32(klas.GetDataCell(@" select count(*) FROM orjin.VW_STOK_FIS STF 
                                        LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID 
                                        LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID 
                                        where SFS_ISLEM_TIP = '09'
                                        AND SFS_MODUL_NO = 1 
                                        AND TB_STOK_FIS_ID = @STOK_FIS_ID 
                                        AND (SOL_ONAY_DURUM_ID = 8) 
                                        AND STO_SIRA_NO = 2", prms.PARAMS));
                                    }
                                    else
                                    {
                                        var s = new StokFis();
                                        s.TB_STOK_FIS_ID = Convert.ToInt32(reader["TB_STOK_FIS_ID"]);
                                        s.SFS_BASLIK = reader["SFS_BASLIK"].ToString();
                                        s.SFS_BOLUM = reader["SFS_BOLUM"].ToString();
                                        s.SFS_CARI = reader["SFS_CARI"].ToString();
                                        s.SFS_CIKIS_DEPO = reader["SFS_CIKIS_DEPO"].ToString();
                                        s.SFS_DURUM = reader["SFS_DURUM"].ToString();
                                        s.SFS_FATURA_IRSALIYE_NO = reader["SFS_FATURA_IRSALIYE_NO"].ToString();
                                        s.SFS_FIS_NO = reader["SFS_FIS_NO"].ToString();
                                        s.SFS_GC = reader["SFS_GC"].ToString();
                                        s.SFS_GIRIS_DEPO = reader["SFS_GIRIS_DEPO"].ToString();
                                        s.SFS_ISLEM_TIP = reader["SFS_ISLEM_TIP"].ToString();
                                        s.SFS_ISLEM_TIP_DEGER = reader["SFS_ISLEM_TIP_DEGER"].ToString();
                                        s.SFS_KOD_ONCELIK = reader["SFS_KOD_ONCELIK"].ToString();
                                        s.SFS_LOKASYON = reader["SFS_LOKASYON"].ToString();
                                        s.SFS_MAKINE = reader["SFS_MAKINE"].ToString();
                                        s.SFS_MAKINE_KOD = reader["SFS_MAKINE_KOD"].ToString();
                                        s.SFS_ONAY_PERSONEL = reader["SFS_ONAY_PERSONEL"].ToString();
                                        s.SFS_ONCELIK = reader["SFS_ONCELIK"].ToString();
                                        s.SFS_PROFJE_TANIM = reader["SFS_PROFJE_TANIM"].ToString();
                                        s.SFS_REFERANS = reader["SFS_REFERANS"].ToString();
                                        s.SFS_REF_GRUP = reader["SFS_REF_GRUP"].ToString();
                                        s.SFS_SATINALMA_TALEP_EDEN = reader["SFS_SATINALMA_TALEP_EDEN"].ToString();
                                        s.SFS_S_TIP = reader["SFS_S_TIP"].ToString();
                                        s.SFS_TALEP_EDEN = reader["SFS_TALEP_EDEN"].ToString();
                                        s.SFS_TALEP_EDILEN = reader["SFS_TALEP_EDILEN"].ToString();
                                        s.SFS_TALEP_FIS_NO = reader["SFS_TALEP_FIS_NO"].ToString();
                                        s.SFS_TALEP_ISEMRI_NO = reader["SFS_TALEP_ISEMRI_NO"].ToString();
                                        s.SFS_TALEP_NEDEN = reader["SFS_TALEP_NEDEN"].ToString();
                                        s.SFS_TALEP_SIPARIS_NO = reader["SFS_TALEP_SIPARIS_NO"].ToString();
                                        s.SFS_TESLIM_ALAN = reader["SFS_TESLIM_ALAN"].ToString();
                                        s.SFS_TESLIM_YERI = reader["SFS_TESLIM_YERI"].ToString();
                                        s.SFS_TESLIM_YERI_TANIM = reader["SFS_TESLIM_YERI_TANIM"].ToString();
                                        s.SFS_TARIH = Convert.ToDateTime(reader["SFS_TARIH"]);
                                        s.SFS_SAAT = (reader["SFS_SAAT"]).ToString();
                                        listem.Add(s);
                                    }
                                    if( firstOrder > 0 )
                                    {
                                        if(approvedFirstOrder > 0)
                                        {
                                            if(secondOrder > 0)
                                            {
                                                if(approvedSecondOrder > 0)
                                                {
                                                    var s = new StokFis();
                                                    s.TB_STOK_FIS_ID = Convert.ToInt32(reader["TB_STOK_FIS_ID"]);
                                                    s.SFS_BASLIK = reader["SFS_BASLIK"].ToString();
                                                    s.SFS_BOLUM = reader["SFS_BOLUM"].ToString();
                                                    s.SFS_CARI = reader["SFS_CARI"].ToString();
                                                    s.SFS_CIKIS_DEPO = reader["SFS_CIKIS_DEPO"].ToString();
                                                    s.SFS_DURUM = reader["SFS_DURUM"].ToString();
                                                    s.SFS_FATURA_IRSALIYE_NO = reader["SFS_FATURA_IRSALIYE_NO"].ToString();
                                                    s.SFS_FIS_NO = reader["SFS_FIS_NO"].ToString();
                                                    s.SFS_GC = reader["SFS_GC"].ToString();
                                                    s.SFS_GIRIS_DEPO = reader["SFS_GIRIS_DEPO"].ToString();
                                                    s.SFS_ISLEM_TIP = reader["SFS_ISLEM_TIP"].ToString();
                                                    s.SFS_ISLEM_TIP_DEGER = reader["SFS_ISLEM_TIP_DEGER"].ToString();
                                                    s.SFS_KOD_ONCELIK = reader["SFS_KOD_ONCELIK"].ToString();
                                                    s.SFS_LOKASYON = reader["SFS_LOKASYON"].ToString();
                                                    s.SFS_MAKINE = reader["SFS_MAKINE"].ToString();
                                                    s.SFS_MAKINE_KOD = reader["SFS_MAKINE_KOD"].ToString();
                                                    s.SFS_ONAY_PERSONEL = reader["SFS_ONAY_PERSONEL"].ToString();
                                                    s.SFS_ONCELIK = reader["SFS_ONCELIK"].ToString();
                                                    s.SFS_PROFJE_TANIM = reader["SFS_PROFJE_TANIM"].ToString();
                                                    s.SFS_REFERANS = reader["SFS_REFERANS"].ToString();
                                                    s.SFS_REF_GRUP = reader["SFS_REF_GRUP"].ToString();
                                                    s.SFS_SATINALMA_TALEP_EDEN = reader["SFS_SATINALMA_TALEP_EDEN"].ToString();
                                                    s.SFS_S_TIP = reader["SFS_S_TIP"].ToString();
                                                    s.SFS_TALEP_EDEN = reader["SFS_TALEP_EDEN"].ToString();
                                                    s.SFS_TALEP_EDILEN = reader["SFS_TALEP_EDILEN"].ToString();
                                                    s.SFS_TALEP_FIS_NO = reader["SFS_TALEP_FIS_NO"].ToString();
                                                    s.SFS_TALEP_ISEMRI_NO = reader["SFS_TALEP_ISEMRI_NO"].ToString();
                                                    s.SFS_TALEP_NEDEN = reader["SFS_TALEP_NEDEN"].ToString();
                                                    s.SFS_TALEP_SIPARIS_NO = reader["SFS_TALEP_SIPARIS_NO"].ToString();
                                                    s.SFS_TESLIM_ALAN = reader["SFS_TESLIM_ALAN"].ToString();
                                                    s.SFS_TESLIM_YERI = reader["SFS_TESLIM_YERI"].ToString();
                                                    s.SFS_TESLIM_YERI_TANIM = reader["SFS_TESLIM_YERI_TANIM"].ToString();
                                                    s.SFS_TARIH = Convert.ToDateTime(reader["SFS_TARIH"]);
                                                    s.SFS_SAAT = (reader["SFS_SAAT"]).ToString();
                                                    listem.Add(s);
                                                }
                                            }
                                            else
                                            {
                                                var s = new StokFis();
                                                s.TB_STOK_FIS_ID = Convert.ToInt32(reader["TB_STOK_FIS_ID"]);
                                                s.SFS_BASLIK = reader["SFS_BASLIK"].ToString();
                                                s.SFS_BOLUM = reader["SFS_BOLUM"].ToString();
                                                s.SFS_CARI = reader["SFS_CARI"].ToString();
                                                s.SFS_CIKIS_DEPO = reader["SFS_CIKIS_DEPO"].ToString();
                                                s.SFS_DURUM = reader["SFS_DURUM"].ToString();
                                                s.SFS_FATURA_IRSALIYE_NO = reader["SFS_FATURA_IRSALIYE_NO"].ToString();
                                                s.SFS_FIS_NO = reader["SFS_FIS_NO"].ToString();
                                                s.SFS_GC = reader["SFS_GC"].ToString();
                                                s.SFS_GIRIS_DEPO = reader["SFS_GIRIS_DEPO"].ToString();
                                                s.SFS_ISLEM_TIP = reader["SFS_ISLEM_TIP"].ToString();
                                                s.SFS_ISLEM_TIP_DEGER = reader["SFS_ISLEM_TIP_DEGER"].ToString();
                                                s.SFS_KOD_ONCELIK = reader["SFS_KOD_ONCELIK"].ToString();
                                                s.SFS_LOKASYON = reader["SFS_LOKASYON"].ToString();
                                                s.SFS_MAKINE = reader["SFS_MAKINE"].ToString();
                                                s.SFS_MAKINE_KOD = reader["SFS_MAKINE_KOD"].ToString();
                                                s.SFS_ONAY_PERSONEL = reader["SFS_ONAY_PERSONEL"].ToString();
                                                s.SFS_ONCELIK = reader["SFS_ONCELIK"].ToString();
                                                s.SFS_PROFJE_TANIM = reader["SFS_PROFJE_TANIM"].ToString();
                                                s.SFS_REFERANS = reader["SFS_REFERANS"].ToString();
                                                s.SFS_REF_GRUP = reader["SFS_REF_GRUP"].ToString();
                                                s.SFS_SATINALMA_TALEP_EDEN = reader["SFS_SATINALMA_TALEP_EDEN"].ToString();
                                                s.SFS_S_TIP = reader["SFS_S_TIP"].ToString();
                                                s.SFS_TALEP_EDEN = reader["SFS_TALEP_EDEN"].ToString();
                                                s.SFS_TALEP_EDILEN = reader["SFS_TALEP_EDILEN"].ToString();
                                                s.SFS_TALEP_FIS_NO = reader["SFS_TALEP_FIS_NO"].ToString();
                                                s.SFS_TALEP_ISEMRI_NO = reader["SFS_TALEP_ISEMRI_NO"].ToString();
                                                s.SFS_TALEP_NEDEN = reader["SFS_TALEP_NEDEN"].ToString();
                                                s.SFS_TALEP_SIPARIS_NO = reader["SFS_TALEP_SIPARIS_NO"].ToString();
                                                s.SFS_TESLIM_ALAN = reader["SFS_TESLIM_ALAN"].ToString();
                                                s.SFS_TESLIM_YERI = reader["SFS_TESLIM_YERI"].ToString();
                                                s.SFS_TESLIM_YERI_TANIM = reader["SFS_TESLIM_YERI_TANIM"].ToString();
                                                s.SFS_TARIH = Convert.ToDateTime(reader["SFS_TARIH"]);
                                                s.SFS_SAAT = (reader["SFS_SAAT"]).ToString();
                                                listem.Add(s);
                                            }
                                        }

                                    }
                                    if( secondOrder > 0 )
                                    {
                                        if(approvedSecondOrder > 0)
                                        {
                                            var s = new StokFis();
                                            s.TB_STOK_FIS_ID = Convert.ToInt32(reader["TB_STOK_FIS_ID"]);
                                            s.SFS_BASLIK = reader["SFS_BASLIK"].ToString();
                                            s.SFS_BOLUM = reader["SFS_BOLUM"].ToString();
                                            s.SFS_CARI = reader["SFS_CARI"].ToString();
                                            s.SFS_CIKIS_DEPO = reader["SFS_CIKIS_DEPO"].ToString();
                                            s.SFS_DURUM = reader["SFS_DURUM"].ToString();
                                            s.SFS_FATURA_IRSALIYE_NO = reader["SFS_FATURA_IRSALIYE_NO"].ToString();
                                            s.SFS_FIS_NO = reader["SFS_FIS_NO"].ToString();
                                            s.SFS_GC = reader["SFS_GC"].ToString();
                                            s.SFS_GIRIS_DEPO = reader["SFS_GIRIS_DEPO"].ToString();
                                            s.SFS_ISLEM_TIP = reader["SFS_ISLEM_TIP"].ToString();
                                            s.SFS_ISLEM_TIP_DEGER = reader["SFS_ISLEM_TIP_DEGER"].ToString();
                                            s.SFS_KOD_ONCELIK = reader["SFS_KOD_ONCELIK"].ToString();
                                            s.SFS_LOKASYON = reader["SFS_LOKASYON"].ToString();
                                            s.SFS_MAKINE = reader["SFS_MAKINE"].ToString();
                                            s.SFS_MAKINE_KOD = reader["SFS_MAKINE_KOD"].ToString();
                                            s.SFS_ONAY_PERSONEL = reader["SFS_ONAY_PERSONEL"].ToString();
                                            s.SFS_ONCELIK = reader["SFS_ONCELIK"].ToString();
                                            s.SFS_PROFJE_TANIM = reader["SFS_PROFJE_TANIM"].ToString();
                                            s.SFS_REFERANS = reader["SFS_REFERANS"].ToString();
                                            s.SFS_REF_GRUP = reader["SFS_REF_GRUP"].ToString();
                                            s.SFS_SATINALMA_TALEP_EDEN = reader["SFS_SATINALMA_TALEP_EDEN"].ToString();
                                            s.SFS_S_TIP = reader["SFS_S_TIP"].ToString();
                                            s.SFS_TALEP_EDEN = reader["SFS_TALEP_EDEN"].ToString();
                                            s.SFS_TALEP_EDILEN = reader["SFS_TALEP_EDILEN"].ToString();
                                            s.SFS_TALEP_FIS_NO = reader["SFS_TALEP_FIS_NO"].ToString();
                                            s.SFS_TALEP_ISEMRI_NO = reader["SFS_TALEP_ISEMRI_NO"].ToString();
                                            s.SFS_TALEP_NEDEN = reader["SFS_TALEP_NEDEN"].ToString();
                                            s.SFS_TALEP_SIPARIS_NO = reader["SFS_TALEP_SIPARIS_NO"].ToString();
                                            s.SFS_TESLIM_ALAN = reader["SFS_TESLIM_ALAN"].ToString();
                                            s.SFS_TESLIM_YERI = reader["SFS_TESLIM_YERI"].ToString();
                                            s.SFS_TESLIM_YERI_TANIM = reader["SFS_TESLIM_YERI_TANIM"].ToString();
                                            s.SFS_TARIH = Convert.ToDateTime(reader["SFS_TARIH"]);
                                            s.SFS_SAAT = (reader["SFS_SAAT"]).ToString();
                                            listem.Add(s);
                                        }
                                    }

                                }
                                
                            }
                        }
                    }
                }
            }
            return listem;

        }


        [Route("api/StokFisDetayRefresh")]
        [HttpPost]
        public StokFis StokFisDetayRefresh([FromUri] int userId, [FromUri] int fisId)
        {
            prms.Clear();
            prms.Add("KLL_ID", userId);
            prms.Add("FIS_ID", fisId);
            StokFis stokFis = new StokFis();
            string query = $" SELECT * FROM orjin.VW_STOK_FIS STF" +
               " LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID " +
                " WHERE  SFS_ISLEM_TIP = '09' " +
                " AND SFS_MODUL_NO = 1 " +
                $" AND SOL_PERSONEL_ID = {userId} " +
                $" AND TB_STOK_FIS_ID = {fisId}";

            using (var command = new SqlCommand(query, klas.baglanCmd()))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        stokFis.TB_STOK_FIS_ID = Convert.ToInt32(reader["TB_STOK_FIS_ID"]);
                        stokFis.SFS_BASLIK = reader["SFS_BASLIK"].ToString();
                        stokFis.SFS_BOLUM = reader["SFS_BOLUM"].ToString();
                        stokFis.SFS_CARI = reader["SFS_CARI"].ToString();
                        stokFis.SFS_CIKIS_DEPO = reader["SFS_CIKIS_DEPO"].ToString();
                        if (reader["SOL_ONAY_DURUM_ID"] != DBNull.Value)
                        {
                            if (Convert.ToInt32(reader["SOL_ONAY_DURUM_ID"]) == 9)
                            {
                                stokFis.SFS_DURUM = "ONAYLANMADI";
                            }
                            else if (Convert.ToInt32(reader["SOL_ONAY_DURUM_ID"]) == 8)
                            {
                                stokFis.SFS_DURUM = "ONAYLANDI";
                            }
                            else
                            {
                                stokFis.SFS_DURUM = "ONAY BEKLİYOR";
                            }
                        }
                        else { stokFis.SFS_DURUM = "ONAY BEKLİYOR"; }
                        stokFis.SFS_FATURA_IRSALIYE_NO = reader["SFS_FATURA_IRSALIYE_NO"].ToString();
                        stokFis.SFS_FIS_NO = reader["SFS_FIS_NO"].ToString();
                        stokFis.SFS_GC = reader["SFS_GC"].ToString();
                        stokFis.SFS_GIRIS_DEPO = reader["SFS_GIRIS_DEPO"].ToString();
                        stokFis.SFS_ISLEM_TIP = reader["SFS_ISLEM_TIP"].ToString();
                        stokFis.SFS_ISLEM_TIP_DEGER = reader["SFS_ISLEM_TIP_DEGER"].ToString();
                        stokFis.SFS_KOD_ONCELIK = reader["SFS_KOD_ONCELIK"].ToString();
                        stokFis.SFS_LOKASYON = reader["SFS_LOKASYON"].ToString();
                        stokFis.SFS_MAKINE = reader["SFS_MAKINE"].ToString();
                        stokFis.SFS_MAKINE_KOD = reader["SFS_MAKINE_KOD"].ToString();
                        stokFis.SFS_ONAY_PERSONEL = reader["SFS_ONAY_PERSONEL"].ToString();
                        stokFis.SFS_ONCELIK = reader["SFS_ONCELIK"].ToString();
                        stokFis.SFS_PROFJE_TANIM = reader["SFS_PROFJE_TANIM"].ToString();
                        stokFis.SFS_REFERANS = reader["SFS_REFERANS"].ToString();
                        stokFis.SFS_REF_GRUP = reader["SFS_REF_GRUP"].ToString();
                        stokFis.SFS_SATINALMA_TALEP_EDEN = reader["SFS_SATINALMA_TALEP_EDEN"].ToString();
                        stokFis.SFS_S_TIP = reader["SFS_S_TIP"].ToString();
                        stokFis.SFS_TALEP_EDEN = reader["SFS_TALEP_EDEN"].ToString();
                        stokFis.SFS_TALEP_EDILEN = reader["SFS_TALEP_EDILEN"].ToString();
                        stokFis.SFS_TALEP_FIS_NO = reader["SFS_TALEP_FIS_NO"].ToString();
                        stokFis.SFS_TALEP_ISEMRI_NO = reader["SFS_TALEP_ISEMRI_NO"].ToString();
                        stokFis.SFS_TALEP_NEDEN = reader["SFS_TALEP_NEDEN"].ToString();
                        stokFis.SFS_TALEP_SIPARIS_NO = reader["SFS_TALEP_SIPARIS_NO"].ToString();
                        stokFis.SFS_TESLIM_ALAN = reader["SFS_TESLIM_ALAN"].ToString();
                        stokFis.SFS_TESLIM_YERI = reader["SFS_TESLIM_YERI"].ToString();
                        stokFis.SFS_TESLIM_YERI_TANIM = reader["SFS_TESLIM_YERI_TANIM"].ToString();
                        stokFis.SFS_TARIH = Convert.ToDateTime(reader["SFS_TARIH"]);
                        stokFis.SFS_SAAT = (reader["SFS_SAAT"]).ToString();
                    }
                }
            }
            return stokFis;   

        }


        [Route("api/MlzTransferOnaylananlar")]
        [HttpPost]
        public Bildirim MlzTransferOnaylananlar([FromUri] int id, [FromBody] List<StokFis> values)
        {
            prms.Clear();
            var hatali = false;
            Bildirim bildirim = new Bildirim();
            var idlist = new int[values.Count];
            for (var i = 0; i < values.Count; i++)
            {
                idlist[i] = values[i].TB_STOK_FIS_ID;
            }
            foreach (var stokFisid in idlist) {
                
                try
                {
                    int userSiraNo = Convert.ToInt32(klas.GetDataCell(@"select STO_SIRA_NO from orjin.TB_SATINALMA_TALEP_ONAY where STO_PERSONEL_ID = @KUL_ID", prms.PARAMS));
                    if (userSiraNo == 3)
                    {
                        klas.cmd($"UPDATE orjin.VW_STOK_FIS_DETAY SET SFD_DURUM = 8 WHERE SFD_STOK_FIS_ID = {stokFisid} ", prms.PARAMS);
                        klas.cmd($"UPDATE orjin.VW_STOK_FIS SET SFS_TALEP_DURUM_ID = 8  WHERE TB_STOK_FIS_ID = {stokFisid} ", prms.PARAMS);

                    }
                    else
                    {
                        int fisCountNumber = Convert.ToInt32(klas.GetDataCell(" select COUNT(*) FROM orjin.VW_STOK_FIS STF " +
                               " LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID " +
                               " LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID " +
                               " WHERE SFS_ISLEM_TIP = '09' " +
                               " AND SFS_MODUL_NO = 1 " +
                               " AND SFS_TALEP_DURUM_ID = 7 AND ((SOL_ONAY_DURUM_ID != 8 AND SOL_ONAY_DURUM_ID != 9) or SOL_ONAY_DURUM_ID is null) " +
                               $" AND TB_STOK_FIS_ID = {stokFisid} " +
                               $" AND STO_SIRA_NO  = {userSiraNo + 1} ", prms.PARAMS));

                        if (fisCountNumber == 0)
                        {
                            klas.cmd("UPDATE orjin.VW_STOK_FIS_DETAY SET SFD_DURUM = 8 WHERE SFD_STOK_FIS_ID = @TB_STOK_FIS_ID ", prms.PARAMS);
                            klas.cmd("UPDATE orjin.VW_STOK_FIS SET SFS_TALEP_DURUM_ID = 8  WHERE TB_STOK_FIS_ID = @TB_STOK_FIS_ID ", prms.PARAMS);
                        }
                    }
                    klas.cmd($"UPDATE orjin.TB_SATINALMA_ONAY_LISTE SET SOL_ONAY_DURUM_ID = 8  WHERE SOL_REF_ID = {stokFisid} AND SOL_PERSONEL_ID = {id}", prms.PARAMS);

                    klas.cmd($"INSERT INTO orjin.TB_SATINALMA_TARIHCE " +
                            $"(STR_ISLEM_ZAMANI, STR_ISLEM_YAPAN_ID, STR_ISLEM_DURUM_ID, STR_ISLEM, STR_TALEP_FIS_ID , STR_DETAY_ID) " +
                            $"VALUES (CURRENT_TIMESTAMP, {id} , 8 , 'MALZEME TALEBI ONAYLANDI', {stokFisid} , -1)", prms.PARAMS);
                }
                catch (Exception e)
                 {
                    hatali = true;
                    bildirim.Aciklama = String.Format(Localization.errorFormatted, e.Message);
                    bildirim.MsgId = Bildirim.MSG_SFS_SIL_HATA;
                    bildirim.HasExtra = true;
                    bildirim.Durum = false;
                    bildirim.Id = id;
                    break;
                }
            }
            if(!hatali)
            {
                bildirim.Aciklama = "Malzeme talebi(leri) başarılı bir şekilde onaylandı ";
                bildirim.MsgId = Bildirim.MSG_SFS_SIL_OK;
                bildirim.Durum = true;
                bildirim.Id = id;
            }
            return bildirim;
        }

        [Route("api/TekMlzTransferOnaylanan")]
        [HttpPost]
        public Bildirim TekMlzTransferOnaylanan([FromUri] int id, [FromUri] int fisID)
        {
            Bildirim bildirimEntity = new Bildirim();
            prms.Clear();
            prms.Add("TB_STOK_FIS_ID", fisID);
            prms.Add("KUL_ID", id);
            try
            {
                int userSiraNo = Convert.ToInt32(klas.GetDataCell(@"select STO_SIRA_NO from orjin.TB_SATINALMA_TALEP_ONAY where STO_PERSONEL_ID = @KUL_ID", prms.PARAMS));
                if (userSiraNo == 3)
                {
                    klas.cmd("UPDATE orjin.VW_STOK_FIS_DETAY SET SFD_DURUM = 8 WHERE SFD_STOK_FIS_ID = @TB_STOK_FIS_ID ", prms.PARAMS);
                    klas.cmd("UPDATE orjin.VW_STOK_FIS SET SFS_TALEP_DURUM_ID = 8  WHERE TB_STOK_FIS_ID = @TB_STOK_FIS_ID ", prms.PARAMS);

                } 
                else
                {
                    int fisCountNumber = Convert.ToInt32(klas.GetDataCell(" select COUNT(*) FROM orjin.VW_STOK_FIS STF " +
                           " LEFT JOIN orjin.TB_SATINALMA_ONAY_LISTE SAO ON SAO.SOL_REF_ID = STF.TB_STOK_FIS_ID " +
                           " LEFT JOIN orjin.TB_SATINALMA_TALEP_ONAY ON STO_PERSONEL_ID =SAO.SOL_PERSONEL_ID " +
                           " WHERE SFS_ISLEM_TIP = '09' " +
                           " AND SFS_MODUL_NO = 1 " +
                           " AND SFS_TALEP_DURUM_ID = 7 AND ((SOL_ONAY_DURUM_ID != 8 AND SOL_ONAY_DURUM_ID != 9) or SOL_ONAY_DURUM_ID is null) " +
                           $" AND TB_STOK_FIS_ID = {fisID} " +
                           $" AND STO_SIRA_NO  = {userSiraNo + 1} ", prms.PARAMS));

                    if(fisCountNumber == 0)
                    {
                        klas.cmd("UPDATE orjin.VW_STOK_FIS_DETAY SET SFD_DURUM = 8 WHERE SFD_STOK_FIS_ID = @TB_STOK_FIS_ID ", prms.PARAMS);
                        klas.cmd("UPDATE orjin.VW_STOK_FIS SET SFS_TALEP_DURUM_ID = 8  WHERE TB_STOK_FIS_ID = @TB_STOK_FIS_ID ", prms.PARAMS);
                    }
                }

                klas.cmd("UPDATE orjin.TB_SATINALMA_ONAY_LISTE SET SOL_ONAY_DURUM_ID = 8  WHERE SOL_REF_ID = @TB_STOK_FIS_ID AND SOL_PERSONEL_ID = @KUL_ID ", prms.PARAMS);

                klas.cmd("INSERT INTO orjin.TB_SATINALMA_TARIHCE " +
                    "  (STR_ISLEM_ZAMANI, STR_ISLEM_YAPAN_ID, STR_ISLEM_DURUM_ID, STR_ISLEM, STR_TALEP_FIS_ID , STR_DETAY_ID ) " +
                    " VALUES (CURRENT_TIMESTAMP, @KUL_ID , 8 , 'MALZEME TALEBI ONAYLANDI', @TB_STOK_FIS_ID , -1)", prms.PARAMS);

                bildirimEntity.Aciklama = "Malzeme talebi onaylandı";
                bildirimEntity.MsgId = Bildirim.MSG_SFS_SIL_OK;
                bildirimEntity.Durum = true;
            }
            catch (Exception e)
            {
                bildirimEntity.Aciklama = String.Format(Localization.errorFormatted, e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_SFS_SIL_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }
            return bildirimEntity;
        }

        [Route("api/MlzTransferOnaylanmayanlar")]
        [HttpPost]
        public Bildirim MlzTransferOnaylanmayanlar([FromUri] int id, [FromBody] List<StokFis> values)
        {
            prms.Clear();
            var hatali = false;
            Bildirim bildirim = new Bildirim();
            var idlist = new int[values.Count];
            for (var i = 0; i < values.Count; i++)
            {
                idlist[i] = values[i].TB_STOK_FIS_ID;
            }
            foreach (var stokFisid in idlist)
            {
                try
                {
                    klas.cmd($"UPDATE orjin.TB_SATINALMA_ONAY_LISTE SET SOL_ONAY_DURUM_ID = 9  WHERE SOL_REF_ID = {stokFisid} AND SOL_PERSONEL_ID = {id}", prms.PARAMS);

                    klas.cmd($"INSERT INTO orjin.TB_SATINALMA_TARIHCE " +
                    $"  (STR_ISLEM_ZAMANI, STR_ISLEM_YAPAN_ID, STR_ISLEM_DURUM_ID, STR_ISLEM, STR_TALEP_FIS_ID , STR_DETAY_ID) " +
                    $" VALUES (CURRENT_TIMESTAMP, {id} , 9 , 'MALZEME TALEBI ONAYLANMADI', {stokFisid} , -1)", prms.PARAMS);
                }
                catch (Exception e)
                {
                    hatali = true;
                    bildirim.Aciklama = String.Format(Localization.errorFormatted, e.Message);
                    bildirim.MsgId = Bildirim.MSG_SFS_SIL_HATA;
                    bildirim.HasExtra = true;
                    bildirim.Durum = false;
                    bildirim.Id = id;
                    break;
                }
            }
            if (!hatali)
            {
                bildirim.Aciklama = "Malzeme talebi(leri) onaylanmadı  ";
                bildirim.MsgId = Bildirim.MSG_SFS_SIL_OK;
                bildirim.Durum = true;
                bildirim.Id = id;
            }
            return bildirim;
        }

        [Route("api/TekMlzTransferOnaylanmayan")]
        [HttpPost]
        public Bildirim TekMlzTransferOnaylanmayan([FromUri] int id, [FromUri] int fisID)
        {
            Bildirim bildirimEntity = new Bildirim();
            prms.Clear();
            prms.Add("TB_STOK_FIS_ID", fisID);
            prms.Add("KUL_ID", id);
            try
            {
                klas.cmd("UPDATE orjin.TB_SATINALMA_ONAY_LISTE SET SOL_ONAY_DURUM_ID = 9  WHERE SOL_REF_ID = @TB_STOK_FIS_ID AND SOL_PERSONEL_ID = @KUL_ID ", prms.PARAMS);

                klas.cmd("INSERT INTO orjin.TB_SATINALMA_TARIHCE " +
                    "  (STR_ISLEM_ZAMANI, STR_ISLEM_YAPAN_ID, STR_ISLEM_DURUM_ID, STR_ISLEM, STR_TALEP_FIS_ID , STR_DETAY_ID) " +
                    " VALUES (CURRENT_TIMESTAMP, @KUL_ID , 9 , 'MALZEME TALEBI ONAYLANMADI', @TB_STOK_FIS_ID , -1) ", prms.PARAMS);

                bildirimEntity.Aciklama = "Malzeme talebi onaylanamdı";
                bildirimEntity.MsgId = Bildirim.MSG_SFS_SIL_OK;
                bildirimEntity.Durum = true;
            }
            catch (Exception e)
            {
                bildirimEntity.Aciklama = String.Format(Localization.errorFormatted, e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_SFS_SIL_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }
            return bildirimEntity;
        }

    }
}



