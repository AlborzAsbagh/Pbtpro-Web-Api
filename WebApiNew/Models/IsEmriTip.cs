using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class IsEmriTip
    {
        
        [DataMember]
        public int TB_ISEMRI_TIP_ID { get; set; }
        
        [DataMember]
        public string IMT_TANIM { get; set; }
        
        [DataMember]
        public bool IMT_LOKASYON { get; set; }
        
        [DataMember]
        public bool IMT_MAKINE { get; set; }
        
        [DataMember]
        public bool IMT_EKIPMAN { get; set; }
        
        [DataMember]
        public bool IMT_PROSEDUR { get; set; }
        
        [DataMember]
        public bool IMT_IS_TIP { get; set; }
        
        [DataMember]
        public bool IMT_IS_NEDEN { get; set; }
        
        [DataMember]
        public bool IMT_ATOLYE { get; set; }
        
        [DataMember]
        public bool IMT_TAKVIM { get; set; }
        
        [DataMember]
        public bool IMT_TALIMAT { get; set; }
        
        [DataMember]
        public bool IMT_PLAN_TARIH { get; set; }
        
        [DataMember]
        public bool IMT_MASRAF_MERKEZ { get; set; }
        
        [DataMember]
        public bool IMT_PROJE { get; set; }
        
        [DataMember]
        public int IMT_OLUSTURAN_ID { get; set; }
        
        [DataMember]
        public DateTime? IMT_OLUSTURMA_TARIH { get; set; }
        
        [DataMember]
        public int IMT_DEGISTIREN_ID { get; set; }
        
        [DataMember]
        public DateTime? IMT_DEGISTIRME_TARIH { get; set; }
        
        [DataMember]
        public bool IMT_VARSAYILAN { get; set; }
        
        [DataMember]
        public string IMT_CAGRILACAK_PROSEDUR { get; set; }
        
        [DataMember]
        public bool IMT_DETAY_TAB { get; set; }
        
        [DataMember]
        public bool IMT_KONTROL_TAB { get; set; }
        
        [DataMember]
        public bool IMT_PERSONEL_TAB { get; set; }
        
        [DataMember]
        public bool IMT_MALZEME_TAB { get; set; }
        
        [DataMember]
        public bool IMT_DURUS_TAB { get; set; }
        
        [DataMember]
        public bool IMT_SURE_TAB { get; set; }
        
        [DataMember]
        public bool IMT_MALIYET_TAB { get; set; }
        
        [DataMember]
        public bool IMT_EKIPMAN_TAB { get; set; }
        
        [DataMember]
        public bool IMT_OLCUM_TAB { get; set; }
        
        [DataMember]
        public bool IMT_ARAC_GEREC_TAB { get; set; }
        
        [DataMember]
        public bool IMT_OZEL_ALAN_TAB { get; set; }
        
        [DataMember]
        public string IMT_RENK { get; set; }
        
        [DataMember]
        public bool IMT_KAPANMA_ZAMANI { get; set; }
        
        [DataMember]
        public bool IMT_SONUC { get; set; }
        
        [DataMember]
        public bool IMT_BAKIM_PUAN { get; set; }
        
        [DataMember]
        public bool IMT_MAKINE_DURUM { get; set; }
        
        [DataMember]
        public bool IMT_SON_UYGULANAN_SAYAC { get; set; }
        
        [DataMember]
        public bool IMT_OKUNAN_SAYAC { get; set; }
        
        [DataMember]
        public string IMT_YAZI_RENGI { get; set; }
        
        [DataMember]
        public string IMT_YAZI_TIPI { get; set; }
        
        [DataMember]
        public byte IMT_TIP_ARIZA { get; set; }
        
        [DataMember]
        public bool IMT_DETAY_TAB_ZORUNLU { get; set; }
        
        [DataMember]
        public bool IMT_KONTROL_TAB_ZORUNLU { get; set; }
        
        [DataMember]
        public bool IMT_PERSONEL_TAB_ZORUNLU { get; set; }
        
        [DataMember]
        public bool IMT_DURUS_TAB_ZORUNLU { get; set; }
        
        [DataMember]
        public bool IMT_MALZEME_TAB_ZORUNLU { get; set; }
        
        [DataMember]
        public bool IMT_EKIPMAN_TAB_ZORUNLU { get; set; }
        
        [DataMember]
        public bool IMT_OLCUM_TAB_ZORUNLU { get; set; }
        
        [DataMember]
        public bool IMT_ARAC_GEREC_TAB_ZORUNLU { get; set; }
        
        [DataMember]
        public byte IMT_MALZEME_FIYAT_TIP { get; set; }
        
        [DataMember]
        public int IMT_VARSAYILAN_MALZEME_MIKTAR { get; set; }
        
        [DataMember]
        public bool IMT_AKTIF { get; set; }
        
        [DataMember]
        public bool IMT_ONCELIK { get; set; }
        
        [DataMember]
        public bool IMT_FIRMA { get; set; }
        
        [DataMember]
        public bool IMT_MAKINE_DURUM_DETAY { get; set; }
        
        [DataMember]
        public bool IMT_SOZLESME { get; set; }
        
        [DataMember]
        public bool IMT_SAYAC_DEGERI { get; set; }
        
        [DataMember]
        public bool IMT_KONU { get; set; }
        
        [DataMember]
        public bool IMT_PLAN_BITIS { get; set; }
        
        [DataMember]
        public bool IMT_PERSONEL_SURE { get; set; }
        
        [DataMember]
        public bool IMT_REFERANS_NO { get; set; }
        
        [DataMember]
        public bool IMT_EVRAK_NO { get; set; }
        
        [DataMember]
        public bool IMT_EVRAK_TARIHI { get; set; }
        
        [DataMember]
        public bool IMT_MALIYET { get; set; }
        
        [DataMember]
        public bool IMT_ACIKLAMA_USTTAB { get; set; }
        
        [DataMember]
        public bool IMT_OZEL_ALAN_1 { get; set; }
        
        [DataMember]
        public bool IMT_OZEL_ALAN_2 { get; set; }
        
        [DataMember]
        public bool IMT_OZEL_ALAN_3 { get; set; }

		[DataMember]
		public bool IMT_OZEL_ALAN_4 { get; set; }

		[DataMember]
		public bool IMT_OZEL_ALAN_5 { get; set; }

		[DataMember]
		public bool IMT_OZEL_ALAN_6 { get; set; }

		[DataMember]
		public bool IMT_OZEL_ALAN_7 { get; set; }

		[DataMember]
		public bool IMT_OZEL_ALAN_8 { get; set; }

		[DataMember]
		public bool IMT_OZEL_ALAN_9 { get; set; }

		[DataMember]
		public bool IMT_OZEL_ALAN_10 { get; set; }

		[DataMember]
        public bool IMT_OZEL_ALAN_11 { get; set; }
        
        [DataMember]
        public bool IMT_OZEL_ALAN_12 { get; set; }
        
        [DataMember]
        public bool IMT_OZEL_ALAN_13 { get; set; }

		[DataMember]
		public bool IMT_OZEL_ALAN_14 { get; set; }

		[DataMember]
		public bool IMT_OZEL_ALAN_15 { get; set; }

		[DataMember]
        public bool IMT_OZEL_ALAN_16 { get; set; }
        
        [DataMember]
        public bool IMT_OZEL_ALAN_17 { get; set; }

		[DataMember]
		public bool IMT_OZEL_ALAN_18 { get; set; }

		[DataMember]
		public bool IMT_OZEL_ALAN_19 { get; set; }

		[DataMember]
		public bool IMT_OZEL_ALAN_20 { get; set; }

		[DataMember]
        public bool IMT_MAKINE_KAPAT { get; set; }
        
        [DataMember]
        public bool IMT_EKIPMAN_KAPAT { get; set; }
        
        [DataMember]
        public bool IMT_MAKINE_DURUM_KAPAT { get; set; }
        
        [DataMember]
        public bool IMT_SAYAC_DEGER_KAPAT { get; set; }
        
        [DataMember]
        public bool IMT_PROSEDUR_KAPAT { get; set; }
        
        [DataMember]
        public bool IMT_IS_TIPI_KAPAT { get; set; }
        
        [DataMember]
        public bool IMT_IS_NEDENI_KAPAT { get; set; }
        
        [DataMember]
        public bool IMT_KONU_KAPAT { get; set; }
        
        [DataMember]
        public bool IMT_ONCELIK_KAPAT { get; set; }
        
        [DataMember]
        public bool IMT_ATOLYE_KAPAT { get; set; }
        
        [DataMember]
        public bool IMT_PROJE_KAPAT { get; set; }
        
        [DataMember]
        public bool IMT_REFNO_KAPAT { get; set; }
        
        [DataMember]
        public bool IMT_FIRMA_KAPAT { get; set; }
        
        [DataMember]
        public bool IMT_SOZLESME_KAPAT { get; set; }
        
        [DataMember]
        public bool IMT_TOPLAM_MALIYET_ZORUNLU { get; set; }
    }
}