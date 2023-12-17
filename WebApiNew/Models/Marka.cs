using System;
using System.Runtime.Serialization;

public class Marka
{
	[DataMember]
	public int TB_MARKA_ID { get; set; }

	[DataMember]
	public string MRK_MARKA { get; set; }

	[DataMember]
	public int MRK_OLUSTURAN_ID { get; set; }

	[DataMember]
	public DateTime? MRK_OLUSTURMA_TARIH { get; set; }

	[DataMember]
	public int MRK_DEGISTIREN_ID { get; set; }

	[DataMember]
	public DateTime? MRK_DEGISTIRME_TARIH { get; set; }
}