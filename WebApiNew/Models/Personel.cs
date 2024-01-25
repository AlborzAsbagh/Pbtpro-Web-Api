using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class Personel
    {
        [DataMember] public int TB_PERSONEL_ID { get; set; }

        [DataMember] public int PRS_RESIM_ID { get; set; }

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

	    [DataMember] public string PRS_GOREV { get; set; }

        [DataMember] public int PRS_LOKASYON_ID { get; set; }

        [DataMember] public string PRS_LOKASYON { get; set; }

        [DataMember] public string PRS_LOKASYON_TUM_YOL { get; set; }

        [DataMember] public int PRS_MASRAF_MERKEZI_ID { get; set; }

        [DataMember] public string PRS_MASRAF_MERKEZI { get; set; }

        [DataMember] public string PRS_TELEFON { get; set; }

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

		[DataMember] public string PRS_IDARI_PERSONEL { get; set; }
	}
}