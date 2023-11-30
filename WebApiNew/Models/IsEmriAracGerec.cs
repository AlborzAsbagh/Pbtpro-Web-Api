using System;
using System.Runtime.Serialization;

public class IsEmriAracGerec
{
	[DataMember]
	public int TB_ISEMRI_ARAC_GEREC_ID { get; set; }

	[DataMember]
	public int IAG_ISEMRI_ID { get; set; }

	[DataMember]
	public int IAG_ARAC_GEREC_ID { get; set; }

	[DataMember]
	public int IAG_OLUSTURAN_ID { get; set; }

	[DataMember]
	public DateTime IAG_OLUSTURMA_TARIH { get; set; }

	[DataMember]
	public int IAG_DEGISTIREN_ID { get; set; }

	[DataMember]
	public DateTime IAG_DEGISTIRME_TARIH { get; set; }
}