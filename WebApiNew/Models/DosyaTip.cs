
using System;
using System.Runtime.Serialization;

public class DosyaTip
{
	[DataMember]
    public int TB_DOSYA_TIP_ID { get; set; }

	[DataMember]
	public string DST_TANIM { get; set; }

	[DataMember]
	public int DST_OLUSTURAN_ID { get; set; }

	[DataMember]
	public DateTime? DST_OLUSTURMA_TARIH { get; set; }

	[DataMember]
	public DateTime? DST_DEGISTIRME_TARIH { get; set; }

	[DataMember]
	public int DST_DEGISTIREN_ID { get; set; }

	[DataMember]
	public string DST_ACIKLAMA { get; set; }

} 