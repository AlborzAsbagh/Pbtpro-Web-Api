using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiNew.Models
{

    public class IsTalepParametre
    {
        public int TB_IS_TALEBI_PARAMETRE_ID { get; set; }
        public int ISP_KAYIT_SAYISI { get; set; }
        public int ISP_YENILEME_SURE { get; set; }
        public int ISP_ACILIS_DURUM { get; set; }
        public int ISP_KULLANICI_TANIM { get; set; }
        public bool ISP_TALEP_KOD { get; set; }
        public bool ISP_TALEP_TARIHI { get; set; }
        public bool ISP_TALEPTE_BULUNAN { get; set; }
        public bool ISP_BINA { get; set; }
        public bool ISP_KAT { get; set; }
        public bool ISP_ONCELIK { get; set; }
        public bool ISP_MAKINE_KOD { get; set; }
        public bool ISP_EKIPMAN_KOD { get; set; }
        public bool ISP_IRTIBAT_TEL { get; set; }
        public bool ISP_MAIL { get; set; }
        public bool ISP_ILETISIM_SEKLI { get; set; }
        public bool ISP_BILDIRIM_TIPI { get; set; }
        public bool ISP_IS_KATEGORI { get; set; }
        public bool ISP_SERVIS_NEDEN { get; set; }
        public bool ISP_IS_TAKIPCI { get; set; }
        public bool ISP_PLANLANAN_BASLAMA_TARIH { get; set; }
        public bool ISP_PLANLANAN_BITIS_TARIH { get; set; }
        public bool ISP_KONU { get; set; }
        public bool ISP_ACIKLAMA { get; set; }
        public bool ISP_OZEL_ALAN_1 { get; set; }
        public bool ISP_OZEL_ALAN_2 { get; set; }
        public bool ISP_OZEL_ALAN_3 { get; set; }
        public bool ISP_OZEL_ALAN_4 { get; set; }
        public bool ISP_OZEL_ALAN_5 { get; set; }
        public bool ISP_OZEL_ALAN_6 { get; set; }
        public bool ISP_OZEL_ALAN_7 { get; set; }
        public bool ISP_OZEL_ALAN_8 { get; set; }
        public bool ISP_OZEL_ALAN_9 { get; set; }
        public bool ISP_OZEL_ALAN_10 { get; set; }
        public int ISP_ONCELIK_ID { get; set; }
        public string ISP_ACILIS_EKRAN { get; set; }
        public string ISP_MAIL_SERVER { get; set; }
        public string ISP_MAIL_PORT { get; set; }
        public int ISP_GIRIS_YENILEME_SURE { get; set; }
        public int ISP_ISEMRI_TIPI_ID { get; set; }
        public bool ISP_ZOR_ISEMRI_TIPI_ID { get; set; }
        public bool ISP_ZOR_MAKINE_DURUM_KOD_ID { get; set; }
        public int ISP_OLUSTURAN_ID { get; set; }
        public DateTime? ISP_OLUSTURMA_TARIH { get; set; }
        public int ISP_DEGISTIREN_ID { get; set; }
        public DateTime? ISP_DEGISTIRME_TARIH { get; set; }
        public string ISP_MAIL_ADRES { get; set; }
        public string ISP_MAIL_SIFRE { get; set; }
        public bool ISP_SSL { get; set; }
        public bool ISP_DUZENLEME_TARIH_DEGISIMI { get; set; }
        public bool ISP_ACIKTALEP_BILDIRIM { get; set; }
        public bool ISP_DEPARTMAN { get; set; }
        public bool ISP_LOKASYON { get; set; }
    }

}