using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class Personel
    {
        [DataMember] public int TB_PERSONEL_ID { get; set; }

        [DataMember] public int PRS_OLUSTURAN_ID { get; set; }

        [DataMember] public int PRS_DEGISTIREN_ID { get; set; }

        [DataMember] public int PRS_RESIM_ID { get; set; }

        [DataMember] public string PRS_ADRES { get; set; }

        [DataMember] public string PRS_IL { get; set; }

        [DataMember] public string PRS_ILCE { get; set; }

        [DataMember] public string PRS_ACIKLAMA { get; set; }

        [DataMember] public string PRS_ULKE { get; set; }

        [DataMember] public string PRS_POSTA_KOD { get; set; }

		[DataMember] public string PRS_PERSONEL_KOD { get; set; }

        [DataMember] public string PRS_ISIM { get; set; }

        [DataMember] public string PRS_UNVAN { get; set; }

        [DataMember] public bool PRS_AKTIF { get; set; }

        [DataMember] public double PRS_SAAT_UCRET { get; set; }

        [DataMember] public bool PRS_TEKNISYEN { get; set; }

        [DataMember] public bool PRS_SURUCU { get; set; }

        [DataMember] public bool PRS_OPERATOR { get; set; }

        [DataMember] public bool PRS_BAKIM { get; set; }

        [DataMember] public bool PRS_DIGER { get; set; }

        [DataMember] public bool PRS_SANTIYE { get; set; }

        [DataMember] public string PRS_DEPARTMAN { get; set; }

        [DataMember] public string PRS_TIP { get; set; }

        [DataMember] public string PRS_RESIM_IDLERI { get; set; }

        [DataMember] public int PRS_PERSONEL_TIP_KOD_ID { get; set; }

        [DataMember] public int PRS_DEPARTMAN_ID { get; set; }

        [DataMember] public int PRS_GOREV_KOD_ID { get; set; }

        [DataMember] public int PRS_FIRMA_ID { get; set; }

        [DataMember] public string PRS_FIRMA { get; set; }

	    [DataMember] public string PRS_GOREV { get; set; }

        [DataMember] public int PRS_LOKASYON_ID { get; set; }

        [DataMember] public string PRS_LOKASYON { get; set; }

        [DataMember] public string PRS_LOKASYON_TUM_YOL { get; set; }

        [DataMember] public int PRS_MASRAF_MERKEZI_ID { get; set; }

        [DataMember] public string PRS_MASRAF_MERKEZI { get; set; }

        [DataMember] public string PRS_TELEFON { get; set; }

        [DataMember] public string PRS_TELEFON1 { get; set; }

        [DataMember] public string PRS_FAX { get; set; }

        [DataMember] public string PRS_DAHILI { get; set; }

        [DataMember] public string PRS_GSM { get; set; }

        [DataMember] public string PRS_EMAIL { get; set; }

        [DataMember] public string PRS_ATOLYE { get; set; }

		[DataMember] public int PRS_ATOLYE_ID { get; set; }

		[DataMember] public int PRS_OZEL_ALAN_1 { get; set; }

		[DataMember] public string PRS_OZEL_ALAN_2 { get; set; }

		[DataMember] public string PRS_OZEL_ALAN_3 { get; set; }

		[DataMember] public string PRS_OZEL_ALAN_4 { get; set; }

		[DataMember] public string PRS_OZEL_ALAN_5 { get; set; }

		[DataMember] public float PRS_OZEL_ALAN_6 { get; set; }

		[DataMember] public float PRS_OZEL_ALAN_7 { get; set; }

		[DataMember] public float PRS_OZEL_ALAN_8 { get; set; }

		[DataMember] public float PRS_OZEL_ALAN_9 { get; set; }

		[DataMember] public float PRS_OZEL_ALAN_10 { get; set; }

        [DataMember] public int PRS_IDARI_PERSONEL_ID { get; set; }

		[DataMember] public string PRS_IDARI_PERSONEL_YAZI { get; set; }

		[DataMember] public string PRS_CINSIYET { get; set; }

		[DataMember] public string PRS_KAN_GRUP { get; set; }

		[DataMember] public string PRS_SSK_NO { get; set; }

		[DataMember] public string PRS_VERGI_NO { get; set; }

		[DataMember] public string PRS_EGITIM_DURUMU { get; set; }

		[DataMember] public string PRS_MEZUN_OLDUGU_OKUL { get; set; }

		[DataMember] public string PRS_MEZUN_OLDUGU_BOLUM { get; set; }

        [DataMember] public int PRS_DIL_KOD_ID { get; set; }

		[DataMember] public string PRS_DIL { get; set; }

		[DataMember] public int PRS_UYRUK_KOD_ID { get; set; }

		[DataMember] public string PRS_UYRUK { get; set; }

		[DataMember] public int PRS_UCRET_TIPI { get; set; }

        [DataMember] public string PRS_UCRET_TIPI_YAZI { get; set; }

        [DataMember] public DateTime? PRS_MEZUNIYET_TARIH { get; set; }

        [DataMember] public DateTime? PRS_ISE_BASLAMA { get; set; }

        [DataMember] public DateTime? PRS_AYRILMATARIH { get; set; }

        [DataMember] public DateTime? PRS_OLUSTURMA_TARIH { get; set; }

        [DataMember] public DateTime? PRS_DEGISTIRME_TARIH { get; set; }

		[DataMember] public DateTime? PRS_DOGUM_TARIH { get; set; }

		[DataMember] public DateTime? PRS_KIMLIK_VERILIS_TARIH { get; set; }

		[DataMember] public DateTime? PRS_EHLIYET_BELGE_TARIHI { get; set; }

		[DataMember] public float PRS_FAZLA_MESAI { get; set; }

        [DataMember] public float PRS_BIRIM_UCRET { get; set; }

        [DataMember] public string PRS_TCKIMLIK_NO { get; set; }

        [DataMember] public string PRS_KIMLIK_SERINO { get; set; }

        [DataMember] public string PRS_BABA_ADI { get; set; }

        [DataMember] public string PRS_ANNE_ADI { get; set; }

        [DataMember] public string PRS_DOGUM_YERI { get; set; }

        [DataMember] public string PRS_DINI { get; set; }

        [DataMember] public string PRS_KIMLIK_KAYIT_NO { get; set; }

        [DataMember] public string PRS_KAYITLI_OLDUGU_IL { get; set; }

        [DataMember] public string PRS_KAYITLI_OLDUGU_ILCE { get; set; }

        [DataMember] public string PRS_MEDENI_HALI { get; set; }

        [DataMember] public string PRS_MAHALLE_KOY { get; set; }

        [DataMember] public string PRS_KIMLIK_CILT_NO { get; set; }

        [DataMember] public string PRS_KIMLIK_AILE_SIRA_NO { get; set; }

        [DataMember] public string PRS_KIMLIK_SIRA_NO { get; set; }

        [DataMember] public string PRS_KIMLIK_VERILDIGI_YER { get; set; }

        [DataMember] public string PRS_KIMLIK_VERILIS_NEDENI { get; set; }

        [DataMember] public string PRS_EHLIYET { get; set; }

        [DataMember] public string PRS_EHLIYET_SINIF { get; set; }

        [DataMember] public string PRS_EHLIYET_VERILDIGI_IL_ILCE { get; set; }

        [DataMember] public string PRS_EHLIYET_SERI_NO { get; set; }

        [DataMember] public float PRS_CEZAPUAN { get; set; }

        [DataMember] public string PRS_EHLIYET_KULLANDIGI_CIHAZ_PROTEZ { get; set; }

        [DataMember] public string PRS_EHLIYETNO { get; set; }



	}
}