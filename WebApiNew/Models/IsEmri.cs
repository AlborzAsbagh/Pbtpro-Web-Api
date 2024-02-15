using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class IsEmri
    {
        [DataMember]
        public int TB_ISEMRI_ID { get; set; }

        [DataMember]
        public string ISM_ISEMRI_NO { get; set; }

        [DataMember]
        public int ISM_MAKINE_ID { get; set; }

        [DataMember]
        public DateTime? ISM_BILDIRIM_TARIH { get; set; }

        [DataMember]
        public string ISM_BILDIRIM_SAAT { get; set; }

        [DataMember]
        public DateTime? ISM_DUZENLEME_TARIH { get; set; }

        [DataMember]
        public string ISM_DUZENLEME_SAAT { get; set; }

        [DataMember]
        public DateTime? ISM_BASLAMA_TARIH { get; set; }

        [DataMember]
        public string ISM_BASLAMA_SAAT { get; set; }

        [DataMember]
        public DateTime? ISM_BITIS_TARIH { get; set; }

        [DataMember]
        public string ISM_BITIS_SAAT { get; set; }

        [DataMember]
        public int ISM_LOKASYON_ID { get; set; }

        [DataMember]
        public string ISM_KONU { get; set; }

		[DataMember]
		public string ISM_OLUSTURAN { get; set; }

		[DataMember]
        public int ISM_ATOLYE_ID { get; set; }

        [DataMember]
        public int ISM_EKIPMAN_ID { get; set; }

        [DataMember]
        public int ISM_TIP_ID { get; set; }

        [DataMember]
        public string ISM_LOKASYON { get; set; }

        [DataMember]
        public string ISM_ATOLYE { get; set; }

        [DataMember]
        public string ISM_EKIPMAN { get; set; }

        [DataMember]
        public string ISM_MAKINE_KOD { get; set; }

        [DataMember]
        public string ISM_MAKINE_TANIMI { get; set; }

        [DataMember]
        public string ISM_MAKINE_PLAKA { get; set; }

        [DataMember]
        public string ISM_TIP { get; set; }

        [DataMember]
        public int ISM_OLUSTURAN_ID { get; set; }

        [DataMember]
        public DateTime? ISM_OLUSTURMA_TARIH { get; set; }

        [DataMember]
        public int ISM_DEGISTIREN_ID { get; set; }

        [DataMember]
        public DateTime? ISM_DEGISTIRME_TARIH { get; set; }

        [DataMember]
        public double ISM_SURE_CALISMA { get; set; }

        [DataMember]
        public Boolean ISM_KAPATILDI { get; set; }

        [DataMember]
        public string ISM_IS_TALEP_KODU { get; set; }

        [DataMember]
        public int ISM_PROJE_ID { get; set; }

        [DataMember]
        public string ISM_PROJE { get; set; }

        [DataMember]
        public int ISM_ONCELIK_ID { get; set; }

        [DataMember]
        public string ISM_ONCELIK { get; set;}

		[DataMember]
        public int ISM_REF_ID { get; set; }

        [DataMember]
        public string ISM_REF_GRUP { get; set; }

        [DataMember]
        public int ISM_MASRAF_MERKEZ_ID { get; set; }

        [DataMember]
        public int ISM_TIP_KOD_ID { get; set; }

        [DataMember]
        public int ISM_DURUM_KOD_ID { get; set; }

        [DataMember]
        public string ISM_DURUM { get; set; }

        [DataMember]
        public double ISM_SAYAC_DEGER { get; set; }

        [DataMember]
        public string ISM_PUAN { get; set; }

        [DataMember]
        public string ISM_IS_SONUC { get; set; }

        [DataMember]
        public string ISM_SONUC { get; set; }

        [DataMember]
        public int ISM_KAPAT_MAKINE_DURUM_KOD_ID { get; set; }

        [DataMember]
        public int ISM_SONUC_KOD_ID { get; set; }

        [DataMember]
        public List<IsEmriPersonel> IsEmriPersonelList { get; set; }

        [DataMember]
        public List<IsEmriKontrolList> IsEmriKontrolList { get; set; } 

		[DataMember]
        public List<IsEmriMalzeme> IsEmriMalzemeList { get; set; }

		[DataMember]
		public List<IsEmriDurus> IsEmriDurusList { get; set; }

		[DataMember]
		public List<IsEmriAracGerec> IsEmriAracGerecList { get; set; }

		[DataMember]
		public List<Olcum> IsEmriOlcumDegeriList { get; set; }

		[DataMember]
        public string ISM_BILDIREN { get; set; }

        [DataMember]
        public int ISM_MAKINE_DURUM_KOD_ID { get; set; }

        [DataMember]
        public string ISM_MAKINE_DURUM { get; set; }

        [DataMember]
        public string ISM_MAKINE_GUVENLIK_NOTU { get; set; }

        [DataMember]
        public string ISM_ACIKLAMA { get; set; }

        [DataMember]
        public DateTime? ISM_IS_TARIH { get; set; }

        [DataMember]
        public string ISM_IS_SAAT { get; set; }

        [DataMember]
        public string ISM_NOT { get; set; }

        [DataMember]
        public string ISM_OZEL_ALAN_1 { get; set; }

        [DataMember]
        public string ISM_OZEL_ALAN_2 { get; set; }

        [DataMember]
        public string ISM_OZEL_ALAN_3 { get; set; }

        [DataMember]
        public string ISM_OZEL_ALAN_4 { get; set; }

        [DataMember]
        public string ISM_OZEL_ALAN_5 { get; set; }

        [DataMember]
        public string ISM_OZEL_ALAN_6 { get; set; }

        [DataMember]
        public string ISM_OZEL_ALAN_7 { get; set; }

        [DataMember]
        public string ISM_OZEL_ALAN_8 { get; set; }

        [DataMember]
        public string ISM_OZEL_ALAN_9 { get; set; }

        [DataMember]
        public string ISM_OZEL_ALAN_10 { get; set; }

        [DataMember]
        public int ISM_OZEL_ALAN_11_KOD_ID { get; set; }

        [DataMember]
        public int ISM_OZEL_ALAN_12_KOD_ID { get; set; }

        [DataMember]
        public int ISM_OZEL_ALAN_13_KOD_ID { get; set; }

        [DataMember]
        public int ISM_OZEL_ALAN_14_KOD_ID { get; set; }

        [DataMember]
        public int ISM_OZEL_ALAN_15_KOD_ID { get; set; }

        [DataMember]
        public double ISM_OZEL_ALAN_16 { get; set; }

        [DataMember]
        public double ISM_OZEL_ALAN_17 { get; set; }

        [DataMember]
        public double ISM_OZEL_ALAN_18 { get; set; }

        [DataMember]
        public double ISM_OZEL_ALAN_19 { get; set; }

        [DataMember]
        public double ISM_OZEL_ALAN_20 { get; set; }

        [DataMember]
        public string ISM_OZEL_ALAN_11_KOD_TANIM { get; set; }

        [DataMember]
        public string ISM_OZEL_ALAN_12_KOD_TANIM { get; set; }

        [DataMember]
        public string ISM_OZEL_ALAN_13_KOD_TANIM { get; set; }

        [DataMember]
        public string ISM_OZEL_ALAN_14_KOD_TANIM { get; set; }

        [DataMember]
        public string ISM_OZEL_ALAN_15_KOD_TANIM { get; set; }

        [DataMember]
        public string IS_EMRI_DURUMU_TANIM { get; set;}

		//----------------------------------------------------------

		[DataMember]
        public double ISM_MALIYET_MLZ { get; set; }
        [DataMember]
        public double ISM_MALIYET_PERSONEL { get; set; }
        [DataMember]
        public double ISM_MALIYET_DURUS { get; set; }
        [DataMember]
        public double ISM_MALIYET_DISSERVIS { get; set; }
        [DataMember]
        public double ISM_MALIYET_DIGER { get; set; }
        [DataMember]
        public double ISM_MALIYET_TOPLAM { get; set; }
        [DataMember]
        public double ISM_MALIYET_PLAN_MLZ { get; set; }
        [DataMember]
        public double ISM_MALIYET_PLAN_PERSONEL { get; set; }
        [DataMember]
        public double ISM_MALIYET_PLAN_DURUS { get; set; }
        [DataMember]
        public double ISM_MALIYET_PLAN_DISSERVIS { get; set; }
        [DataMember]
        public double ISM_MALIYET_PLAN_DIGER { get; set; }
        [DataMember]
        public double ISM_MALIYET_PLAN_TOPLAM { get; set; }
        [DataMember]
        public int ISM_SURE_MUDAHALE_LOJISTIK { get; set; }
        [DataMember]
        public int ISM_SURE_MUDAHALE_SEYAHAT { get; set; }
        [DataMember]
        public int ISM_SURE_MUDAHALE_ONAY { get; set; }
        [DataMember]
        public int ISM_SURE_MUDAHALE_DIGER { get; set; }
        [DataMember]
        public int ISM_SURE_BEKLEME { get; set; }
        [DataMember]
        public int ISM_SURE_TOPLAM { get; set; }
        [DataMember]
        public int ISM_SURE_PLAN_TALEP { get; set; }
        [DataMember]
        public int ISM_SURE_PLAN_MUDAHALE { get; set; }
        [DataMember]
        public int ISM_SURE_PLAN_CALISMA { get; set; }
        [DataMember]
        public int ISM_SURE_PLAN_DURUS { get; set; }
        [DataMember]
        public int ISM_SURE_PLAN_BEKLEME { get; set; }
        [DataMember]
        public int ISM_SURE_PLAN_TOPLAM { get; set; }
        [DataMember]
        public int ISM_TAKVIM_ID { get; set; }
        [DataMember]
        public int ISM_TALIMAT_ID { get; set; }
        [DataMember]
        public string ISM_TALIMAT { get; set; }
        [DataMember]
        public int ISM_GUVENLIK_ID { get; set; }
        [DataMember]
        public int ISM_IPTAL_NEDEN_KOD_ID { get; set; }
        [DataMember]
        public int ISM_IPTAL_ONAY_PERSONEL_ID { get; set; }
        [DataMember]
        public string ISM_IPTAL_ACIKLAMA { get; set; }
        [DataMember]
        public int ISM_NEDEN_KOD_ID { get; set; }
        [DataMember]
        public int ISM_ILETISIM_SEKLI_KOD_ID { get; set; }
        [DataMember]
        public string ISM_DURUM_ACIKLAMA { get; set; }
        [DataMember]
        public string ISM_IRTIBAT_TEL { get; set; }
        [DataMember]
        public string ISM_EMAIL { get; set; }
        [DataMember]
        public string ISM_YAPILDI { get; set; }
        [DataMember]
        public int ISM_YAPILAMAMA_NEDEN_KOD_ID { get; set; }
        [DataMember]
        public DateTime? ISM_PLAN_BASLAMA_TARIH { get; set; }
        [DataMember]
        public string ISM_PLAN_BASLAMA_SAAT { get; set; }
        [DataMember]
        public DateTime? ISM_PLAN_BITIS_TARIH { get; set; }
        [DataMember]
        public string ISM_PLAN_BITIS_SAAT { get; set; }
        [DataMember]
        public string ISM_IS_TIP { get; set; }
        [DataMember]
        public int ISM_BAGLI_ISEMRI_ID { get; set; }
        [DataMember]
        public string ISM_IS_BILDIREN { get; set; }
        [DataMember]
        public int ISM_PROJE_KOD_ID { get; set; }
        [DataMember]
        public string ISM_REFERANS_NO { get; set; }
        [DataMember]
        public int ISM_TAMAMLANMA_ORAN { get; set; }
        [DataMember]
        public int ISM_FIRMA_ID { get; set; }
        [DataMember]
        public int ISM_FIRMA_SOZLESME_ID { get; set; }
        [DataMember]
        public string ISM_EVRAK_NO { get; set; }
        [DataMember]
        public bool ISM_GARANTI_KAPSAMINDA { get; set; }
        [DataMember]
        public int ISM_IS_DEPARTMAN_KOD_ID { get; set; }
        [DataMember]
        public int ISM_IS_ILETISIM_SEKLI_KOD_ID { get; set; }
        [DataMember]
        public string ISM_IS_EMAIL { get; set; }
        [DataMember]
        public string ISM_IS_IRTIBAT_TEL { get; set; }
        [DataMember]
        public int ISM_SURE_ISCILIK { get; set; }
        [DataMember]
        public int ISM_SURE_PLAN_MUDAHALE_LOJISTIK { get; set; }
        [DataMember]
        public int ISM_SURE_PLAN_MUDAHALE_SEYAHAT { get; set; }
        [DataMember]
        public int ISM_SURE_PLAN_MUDAHALE_ONAY { get; set; }
        [DataMember]
        public int ISM_SURE_PLAN_MUDAHALE_DIGER { get; set; }
        [DataMember]
        public DateTime? ISM_PLAN_BASLAMA_TARIH_ILK { get; set; }
        [DataMember]
        public bool ISM_OTOMATIK_ISEMRI { get; set; }
        [DataMember]
        public int ISM_TASERON_FIRMA_ID { get; set; }
        [DataMember]
        public int ISM_HAKEDIS_ID { get; set; }
        [DataMember]
        public DateTime? ISM_EVRAK_TARIHI { get; set; }
        [DataMember]
        public double ISM_MALIYET_KDV { get; set; }
        [DataMember]
        public double ISM_MALIYET_INDIRIM { get; set; }
        [DataMember]
        public DateTime? ISM_KAPANMA_YDK_TARIH { get; set; }
        [DataMember]
        public string ISM_KAPANMA_YDK_SAAT { get; set; }
        [DataMember]
        public string ISM_IS_TALEP_NO { get; set; }
        [DataMember]
        public string ISM_IS_TALEP_BINA { get; set; }
        [DataMember]
        public string ISM_IS_TALEP_KAT { get; set; }

        //Mobile New Features Added---------------------------------------

        [DataMember]
        public int MalzemeCount { get; set; }

        [DataMember]
        public int KontrolListCount { get; set; }

        [DataMember]
        public int PersonelCount { get; set; }

        [DataMember]
        public int DurusCount { get; set; }

        [DataMember]
        public String ResimIDleri { get; set; }

        [DataMember]
        public int ResimVarsayilanID { get; set; }

    }
}