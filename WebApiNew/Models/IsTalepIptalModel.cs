
using System.Runtime.Serialization;
using System;

public class IsTalepIptalKapatModel
{
	[DataMember]
	public int TB_IS_TALEP_ID { get; set; }

	[DataMember]
	public int KLL_ID { get; set; }

	[DataMember]
	public string IST_TALEP_NO { get; set; }

	[DataMember]
	public string KLL_ADI { get; set; }

	[DataMember]
	public int IST_IPTAL_NEDEN_KOD_ID { get; set; }

	[DataMember]
	public string IST_IPTAL_NEDEN { get; set; }

	[DataMember]
	public DateTime? IST_IPTAL_TARIH { get; set; }

	[DataMember]
	public string IST_IPTAL_SAAT { get; set; }

	[DataMember]
	public DateTime? IST_KAPAMA_TARIHI { get; set; }

	[DataMember]
	public string IST_KAPAMA_SAATI { get; set; }

	[DataMember]
	public string ITL_TALEP_ISLEM { get; set; }

	[DataMember]
	public string ITL_ISLEM_DURUM { get; set; }

	[DataMember]
	public string ITL_ISLEM { get; set; }

	[DataMember]
	public int ITL_ISLEM_ID { get; set; }

	[DataMember]
	public string ITL_ACIKLAMA { get; set; }

	[DataMember]
	public string IST_SONUC { get; set; }
}