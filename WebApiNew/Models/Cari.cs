using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiNew.Models
{
    public class Cari
    {
        public int TB_CARI_ID { get; set; }
        public string CAR_KOD { get; set; }
        public string CAR_TANIM { get; set; }
        public int CAR_SEKTOR_KOD_ID { get; set; }
        public int CAR_BOLGE_KOD_ID { get; set; }
        public string CAR_ADRES { get; set; }
        public string CAR_SEHIR { get; set; }
        public string CAR_ILCE { get; set; }
        public string CAR_POSTA_KOD { get; set; }
        public string CAR_ULKE { get; set; }
        public string CAR_ILGILI { get; set; }
        public string CAR_TEL1 { get; set; }
        public string CAR_TEL2 { get; set; }
        public string CAR_FAKS1 { get; set; }
        public string CAR_FAKS2 { get; set; }
        public string CAR_GSM { get; set; }
        public string CAR_WEB { get; set; }
        public string CAR_EMAIL { get; set; }
        public int CAR_TIP_KOD_ID { get; set; }
        public string CAR_VERGI_DAIRE { get; set; }
        public string CAR_VERGI_NO { get; set; }
        public string CAR_TERMIN_SURE { get; set; }
        public double CAR_KREDI_LIMIT { get; set; }
        public string CAR_MUHASEBE_KOD { get; set; }
        public int CAR_HIZMET_DEGER_KOD_ID { get; set; }
        public string CAR_SARTNAME_NO { get; set; }
        public string CAR_SOZLESME_NO { get; set; }
        public bool CAR_TEDARIKCI { get; set; }
        public bool CAR_MUSTERI { get; set; }
        public bool CAR_NAKLIYECI { get; set; }
        public bool CAR_SERVIS { get; set; }
        public bool CAR_SUBE { get; set; }
        public bool CAR_DIGER { get; set; }
        public double CAR_ISLEM_HACMI { get; set; }
        public string CAR_ACIKLAMA { get; set; }
        public bool CAR_AKTIF { get; set; }
        public int CAR_OLUSTURAN_ID { get; set; }
        public DateTime? CAR_OLUSTURMA_TARIH { get; set; }
        public int CAR_DEGISTIREN_ID { get; set; }
        public DateTime? CAR_DEGISTIRME_TARIH { get; set; }
        public int CAR_LOKASYON_ID { get; set; }
        public bool CAR_TASERON { get; set; }
        public string CAR_OZEL_ALAN_1 { get; set; }
        public string CAR_OZEL_ALAN_2 { get; set; }
        public string CAR_OZEL_ALAN_3 { get; set; }
        public string CAR_OZEL_ALAN_4 { get; set; }
        public string CAR_OZEL_ALAN_5 { get; set; }
        public double CAR_OZEL_ALAN_6 { get; set; }
        public double CAR_OZEL_ALAN_7 { get; set; }
        public double CAR_OZEL_ALAN_8 { get; set; }
        public double CAR_OZEL_ALAN_9 { get; set; }
        public double CAR_OZEL_ALAN_10 { get; set; }
        public int CAR_FINANS_CARI_ID { get; set; }
        public double CAR_INDIRIM_ORAN { get; set; }
        public int CAR_BELGE { get; set; }
        public int CAR_RESIM { get; set; }
        public string CAR_SEKTOR { get; set; }
        public string CAR_BOLGE { get; set; }
        public string CAR_TIP { get; set; }
        public string CAR_HIZMET_DEGER { get; set; }
        public string CAR_LOKASYON { get; set; }
    }

}