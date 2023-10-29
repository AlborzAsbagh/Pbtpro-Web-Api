using System;
using System.Runtime.Serialization;

[DataContract]
public class Talimat
{
	[DataMember]
	public int TB_TALIMAT_ID { get; set; }

	[DataMember]
	public string TLM_KOD { get; set; }

	[DataMember]
	public string TLM_TANIM { get; set; }

	[DataMember]
	public string TLM_BA { get; set; }

	[DataMember]
	public int TLM_HAZIRLAYAN_PERSONEL_ID { get; set; }

	[DataMember]
	public DateTime TLM_YURURLUK_TARIH { get; set; }

	[DataMember]
	public int TLM_SORUMLU_PERSONEL_ID { get; set; }

	[DataMember]
	public string TLM_REV_NO { get; set; }

	[DataMember]
	public DateTime TLM_REV_TARIH { get; set; }

	[DataMember]
	public int TLM_REV_EDEN_PERSONEL_ID { get; set; }

	[DataMember]
	public string TLM_REV_NEDEN { get; set; }

	[DataMember]
	public string TLM_REFERANS { get; set; }

	[DataMember]
	public string TLM_DAGITIM { get; set; }

	[DataMember]
	public string TLM_ACIKLAMA { get; set; }

	[DataMember]
	public string TLM_TALIMAT { get; set; }

	[DataMember]
	public int TLM_OLUSTURAN_ID { get; set; }

	[DataMember]
	public DateTime TLM_OLUSTURMA_TARIH { get; set; }

	[DataMember]
	public int TLM_DEGISTIREN_ID { get; set; }

	[DataMember]
	public DateTime TLM_DEGISTIRME_TARIH { get; set; }

	[DataMember]
	public int TLM_TIP_KOD_ID { get; set; }

	[DataMember]
	public string TLM_BELGE { get; set; }

	[DataMember]
	public byte TLM_RESIM { get; set; }

	[DataMember]
	public string TLM_HAZIRLAYAN { get; set; }

	[DataMember]
	public string TLM_SORUMLU { get; set; }

	[DataMember]
	public string TLM_REVIZE_EDEN { get; set; }

	[DataMember]
	public string TLM_TIP { get; set; }
}
