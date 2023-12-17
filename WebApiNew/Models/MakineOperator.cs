using System;
using System.Runtime.Serialization;

public class MakineOperator
{
	[DataMember]
	public int TB_MAKINE_OPERATOR_ID { get; set; }

	[DataMember]
	public int MKO_MAKINE_ID { get; set; }

	[DataMember]
	public DateTime? MKO_TARIH { get; set; }

	[DataMember]
	public string MKO_SAAT { get; set; }

	[DataMember]
	public int MKO_KAYNAK_OPERATOR_ID { get; set; }

	[DataMember]
	public int MKO_HEDEF_OPERATOR_ID { get; set; }

	[DataMember]
	public string MKO_ACIKLAMA { get; set; }

	[DataMember]
	public int MKO_OLUSTURAN_ID { get; set; }

	[DataMember]
	public DateTime? MKO_OLUSTURMA_TARIH { get; set; }

	[DataMember]
	public int MKO_DEGISTIREN_ID { get; set; }

	[DataMember]
	public DateTime? MKO_DEGISTIRME_TARIH { get; set; }

	[DataMember]
	public string MKO_SAYAC_BIRIMI { get; set; }

	[DataMember]
	public int MKO_GUNCEL_SAYAC_DEGERI { get; set; }

	[DataMember]
	public string MKO_HEDEF_OPERATOR_KOD { get; set; }

	[DataMember]
	public string MKO_KAYNAK_OPERATOR_KOD { get; set; }


}