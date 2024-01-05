using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class IsTanim
    {
        [DataMember]
        public int TB_IS_TANIM_ID { get; set; }
        [DataMember]
        public string IST_KOD { get; set; }
        [DataMember]
        public string IST_TANIM { get; set; }
        [DataMember]
        public string IST_DURUM { get; set; }
        [DataMember]
        public bool IST_AKTIF { get; set; }
        [DataMember]
        public int IST_TIP_KOD_ID { get; set; }
		[DataMember]
		public string IST_TIP { get; set; }
		[DataMember]
        public int IST_GRUP_KOD_ID { get; set; }
		[DataMember]
		public string IST_GRUP { get; set; }
		[DataMember]
        public int IST_ATOLYE_ID { get; set; }
        [DataMember]
        public double IST_CALISMA_SURE { get; set; }
        [DataMember]
        public double IST_DURUS_SURE { get; set; }
        [DataMember]
        public int IST_PERSONEL_SAYI { get; set; }
        [DataMember] 
        public int IST_ONCELIK_ID { get; set; }
        [DataMember]
        public int IST_TALIMAT_ID { get; set; }
        [DataMember]
        public string IST_ACIKLAMA { get; set; }
        [DataMember]
        public int IST_OLUSTURAN_ID { get; set; }
        [DataMember]
        public DateTime? IST_OLUSTURMA_TARIH { get; set; }
        [DataMember]
        public int IST_DEGISTIREN_ID { get; set; }
        [DataMember]
        public DateTime? IST_DEGISTIRME_TARIH { get; set; }
        [DataMember]
        public double IST_MALZEME_MALIYET { get; set; }
        [DataMember]
        public double IST_ISCILIK_MALIYET { get; set; }
        [DataMember]
        public double IST_GENEL_GIDER_MALIYET { get; set; }
        [DataMember]
        public double IST_TOPLAM_MALIYET { get; set; }
        [DataMember]
        public int IST_NEDEN_KOD_ID { get; set; }
        [DataMember]
        public int IST_FIRMA_ID { get; set; }
        [DataMember]
        public bool IST_IS_TALEPTE_GORUNSUN { get; set; }
        [DataMember]
        public int IST_MASRAF_MERKEZ_ID { get; set; }
        [DataMember]
        public int IST_SURE_LOJISTIK { get; set; }
        [DataMember]
        public int IST_SURE_SEYAHAT { get; set; }
        [DataMember]
        public int IST_SURE_ONAY { get; set; }
        [DataMember]
        public int IST_SURE_BEKLEME { get; set; }
        [DataMember]
        public int IST_SURE_DIGER { get; set; }
        [DataMember]
        public string IST_OZEL_ALAN_1 { get; set; }
        [DataMember]
        public string IST_OZEL_ALAN_2 { get; set; }
        [DataMember]
        public string IST_OZEL_ALAN_3 { get; set; }
        [DataMember]
        public string IST_OZEL_ALAN_4 { get; set; }
        [DataMember]
        public string IST_OZEL_ALAN_5 { get; set; }
        [DataMember]
        public double IST_OZEL_ALAN_7_KOD_ID { get; set; }
        [DataMember]
        public double IST_OZEL_ALAN_9 { get; set; }
        [DataMember]
        public double IST_OZEL_ALAN_10 { get; set; }
        [DataMember]
        public int IST_OZEL_ALAN_6_KOD_ID { get; set; }
        [DataMember]
        public int IST_OZEL_ALAN_8_KOD_ID { get; set; }
        [DataMember]
        public bool IST_UYAR { get; set; }
        [DataMember]
        public int IST_UYARI_SIKLIGI { get; set; }
        [DataMember]
        public int IST_LOKASYON_ID { get; set; }
        [DataMember]
        public bool IST_OTONOM_BAKIM { get; set; }
        [DataMember]
        public int IST_KONTROL_SAYI { get; set; }
    }
}