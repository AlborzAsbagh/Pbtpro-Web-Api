using System;
using System.Runtime.Serialization;

public class Model
{
	[DataMember]
	public int TB_MODEL_ID { get; set; }

	[DataMember]
	public int MDL_MARKA_ID { get; set; }

	[DataMember]
	public string MDL_MODEL { get;  set; }

	[DataMember]
	public int MDL_OLUSTURAN_ID { get; set; }

	[DataMember]
	public DateTime? MDL_OLUSTURMA_TARIH { get; set; }

	[DataMember]
	public int MDL_DEGISTIREN_ID { get; set; }

	[DataMember]
	public DateTime? MDL_DEGISTIRME_TARIH { get; set; }
} 