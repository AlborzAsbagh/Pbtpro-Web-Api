using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{   [DataContract]
    public class IsTalep
    {

        [DataMember]
        public int TB_IS_TALEP_ID { get; set; }
        [DataMember]
        public string IST_KOD { get; set; }
        [DataMember]
        public DateTime? IST_ACILIS_TARIHI { get; set; }
        [DataMember]
        public string IST_ACILIS_SAATI { get; set; }
        [DataMember]
        public DateTime? IST_GUNCELLEME_TARIHI { get; set; }
        [DataMember]
        public string IST_GUNCELLEME_SAATI { get; set; }
        [DataMember]
        public int IST_GUNCELEYEN_ID { get; set; }
        [DataMember]
        public DateTime? IST_KAPAMA_TARIHI { get; set; }
        [DataMember]
        public string IST_KAPAMA_SAATI { get; set; }
        [DataMember]
        public int IST_TALEP_EDEN_ID { get; set; }
        [DataMember]
        public int IST_IS_TAKIPCISI_ID { get; set; }
        [DataMember]
        public int IST_ATOLYE_GRUP_ID { get; set; }
        [DataMember]
        public int IST_TIP_KOD_ID { get; set; }
        [DataMember]
        public int IST_KOTEGORI_KODI_ID { get; set; }
        [DataMember]
        public int IST_SERVIS_NEDENI_KOD_ID { get; set; }
        [DataMember]
        public int IST_IRTIBAT_KOD_KOD_ID { get; set; }
        [DataMember]
        public int IST_BILDIRILEN_BINA { get; set; }
        [DataMember]
        public int IST_BILDIRILEN_KAT { get; set; }
        [DataMember]
        public string IST_TANIMI { get; set; }
        [DataMember]
        public string IST_KONU { get; set; }
        [DataMember]
        public string IST_NOT { get; set; }
        [DataMember]
        public int IST_DURUM_ID { get; set; }
        [DataMember]
        public int IST_ONCELIK_ID { get; set; }
        [DataMember]
        public DateTime? IST_PLANLANAN_BASLAMA_TARIHI { get; set; }
        [DataMember]
        public string IST_PLANLANAN_BASLAMA_SAATI { get; set; }
        [DataMember]
        public DateTime? IST_PLANLANAN_BITIS_TARIHI { get; set; }
        [DataMember]
        public string IST_PLANLANAN_BITIS_SAATI { get; set; }
        [DataMember]
        public int IST_BILDIREN_LOKASYON_ID { get; set; }
        [DataMember]
        public string IST_IRTIBAT_TELEFON { get; set; }
        [DataMember]
        public string IST_MAIL_ADRES { get; set; }
        [DataMember]
        public DateTime? IST_BASLAMA_TARIHI { get; set; }
        [DataMember]
        public string IST_BASLAMA_SAATI { get; set; }
        [DataMember]
        public DateTime? IST_BITIS_TARIHI { get; set; }
        [DataMember]
        public string IST_BITIS_SAATI { get; set; }
        [DataMember]
        public string IST_IPTAL_NEDEN { get; set; }
        [DataMember]
        public DateTime? IST_IPTAL_TARIH { get; set; }
        [DataMember]
        public string IST_IPTAL_SAAT { get; set; }
        [DataMember]
        public int IST_MAKINE_ID { get; set; }
        [DataMember]
        public int IST_EKIPMAN_ID { get; set; }
        [DataMember]
        public int IST_ISEMRI_ID { get; set; }
        [DataMember]
        public string IST_ACIKLAMA { get; set; }
        [DataMember]
        public string IST_SONUC { get; set; }
        [DataMember]
        public bool IST_AKTIF { get; set; }
        [DataMember]
        public int IST_BIRLESIM_ID { get; set; }
        [DataMember]
        public string IST_ACILIS_NEDEN { get; set; }
        [DataMember]
        public int IST_SABLON_ID { get; set; }
        [DataMember]
        public int IST_ARIZA_ID { get; set; }
        [DataMember]
        public int IST_MAKINE_DURUM_KOD_ID { get; set; }
        [DataMember]
        public int IST_ARIZA_TANIM_KOD_ID { get; set; }
        [DataMember]
        public bool IST_OKUNDU { get; set; }
        [DataMember]
        public int IST_ISEMRI_TIP_ID { get; set; }
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
        public string IST_OZEL_ALAN_6 { get; set; }
        [DataMember]
        public string IST_OZEL_ALAN_7 { get; set; }
        [DataMember]
        public string IST_OZEL_ALAN_8 { get; set; }
        [DataMember]
        public string IST_OZEL_ALAN_9 { get; set; }
        [DataMember]
        public string IST_OZEL_ALAN_10 { get; set; }
        [DataMember]
        public double? IST_OZEL_ALAN_11 { get; set; }
        [DataMember]
        public double? IST_OZEL_ALAN_12 { get; set; }
        [DataMember]
        public double? IST_OZEL_ALAN_13 { get; set; }
        [DataMember]
        public double? IST_OZEL_ALAN_14 { get; set; }
        [DataMember]
        public double? IST_OZEL_ALAN_15 { get; set; }
        [DataMember]
        public double? IST_OZEL_ALAN_16 { get; set; }
        [DataMember]
        public double? IST_OZEL_ALAN_17 { get; set; }
        [DataMember]
        public double? IST_OZEL_ALAN_18 { get; set; }
        [DataMember]
        public double? IST_OZEL_ALAN_19 { get; set; }
        [DataMember]
        public double? IST_OZEL_ALAN_20 { get; set; }
        [DataMember]
        public int IST_OLUSTURAN_ID { get; set; }
        [DataMember]
        public DateTime? IST_OLUSTURMA_TARIH { get; set; }
        [DataMember]
        public int IST_DEGISTIREN_ID { get; set; }
        [DataMember]
        public DateTime? IST_DEGISTIRME_TARIH { get; set; }
        [DataMember]
        public string IST_ON_DEGERLENDIRME { get; set; }
        [DataMember]
        public int IST_IS_DEVAM_DURUM_ID { get; set; }
        [DataMember]
        public byte IST_DEGERLENDIRME_PUAN { get; set; }
        [DataMember]
        public string IST_DEGERLENDIRME_ACIKLAMA { get; set; }
        [DataMember]
        public int IST_DEPARTMAN_ID { get; set; }
        [DataMember]
        public int IST_ILGILI_ATOLYE_ID { get; set; }
        [DataMember]
        public string IST_TALEP_EDEN_ADI { get; set; }
        [DataMember]
        public string IST_TAKIP_EDEN_ADI { get; set; }
        [DataMember]
        public string IST_ATOLYE_GRUBU_TANIMI { get; set; }
        [DataMember]
        public string IST_TIP_TANIM { get; set; }
        [DataMember]
        public string IST_KATEGORI_TANIMI { get; set; }
        [DataMember]
        public string IST_SERVIS_NEDENI { get; set; }
        [DataMember]
        public string IST_IRTIBAT { get; set; }
        [DataMember]
        public string IST_ONCELIK { get; set; }
        [DataMember]
        public int IST_ONCELIK_IKON_INDEX { get; set; }
        [DataMember]
        public string IST_BILDIREN_LOKASYON { get; set; }
        [DataMember]
        public int IST_NOT_ICON { get; set; }
        [DataMember]
        public int IST_TEKNISYEN_ID { get; set; }
        [DataMember]
        public string IST_TEKNISYEN_TANIM { get; set; }
        [DataMember]
        public string IST_ISEMRI_NO { get; set; }
        [DataMember]
        public int IST_BELGE { get; set; }
        [DataMember]
        public int IST_RESIM { get; set; }
        [DataMember]
        public int IST_BIRLESIM { get; set; }
        [DataMember]
        public string IST_MAKINE_KOD { get; set; }
        [DataMember]
        public string IST_MAKINE_TANIM { get; set; }
        [DataMember]
        public string IST_EKIPMAN_KOD { get; set; }
        [DataMember]
        public string IST_EKIPMAN_TANIM { get; set; }
        [DataMember]
        public string IST_KAT { get; set; }
        [DataMember]
        public string IST_BINA { get; set; }
        [DataMember]
        public string IST_DURUM_ADI { get; set; }
        [DataMember]
        public string IST_DURUM_ADI2 { get; set; }
        [DataMember]
        public int IST_KULLANICI_DEPARTMAN_ID { get; set; }
        [DataMember]
        public string IST_MAKINE_DURUM { get; set; }
        [DataMember]
        public string IST_ARIZA_TANIM_KOD { get; set; }
        [DataMember]
        public string ISEMRI_TIPI { get; set; }
        [DataMember]
        public string IST_BILDIREN_LOKASYON_TUM { get; set; }
        [DataMember]
        public string ISLEM_SURE { get; set; }
        [DataMember]
        public int ResimVarsayilanID { get; set; }
        [DataMember]
        public string ResimIDleri { get; set; }
        [DataMember]
        public int IST_TALEPEDEN_LOKASYON_ID { get; set; }

        [DataMember]
        public int USER_ID { get; set; }

    }

}