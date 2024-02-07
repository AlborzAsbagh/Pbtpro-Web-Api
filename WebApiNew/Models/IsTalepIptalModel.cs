
using System.Runtime.Serialization;
using System;

public class IsTalepIptalModel
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
}