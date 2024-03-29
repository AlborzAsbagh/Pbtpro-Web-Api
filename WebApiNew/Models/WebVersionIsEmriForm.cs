using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WebApiNew.Models;

[DataContract]
public class WebVersionIsEmriForm
{
	[DataMember]
	public int TB_ISEMRI_ID { get; set; }

	[DataMember]
	public string ISM_ISEMRI_NO { get; set; }

	[DataMember]
	public DateTime? ISM_BASLAMA_TARIH { get; set; }

	[DataMember]
	public string ISM_TIP { get; set; }

	[DataMember]
	public string ISM_DURUM { get; set; }

	[DataMember]
	public string ISM_BAGLI_ISEMRI { get; set; }

	[DataMember]
	public string ISM_ATOLYE { get; set; }

	[DataMember]
	public string ISM_LOKASYON { get; set; }

	[DataMember]
	public string ISM_MAKINE_KOD { get; set; }

	[DataMember]
	public string ISM_MAKINE_TANIM { get; set; }

	[DataMember]
	public string ISM_EKIPMAN_TANIM { get; set; }

	[DataMember]
	public string ISM_EKIPMAN_KOD { get; set; }

	[DataMember]
	public string ISM_ONCELIK { get; set; }

	[DataMember]
	public string ISM_MAKINE_GUVENLIK_NOTU { get; set; }

	[DataMember]
	public string ISM_ACIKLAMA { get; set; }

	[DataMember]
	public string ISM_MAKINE_DURUM { get; set; }

	[DataMember]
	public DateTime? ISM_TALEP_TARIH { get; set; }

	[DataMember]
	public string ISM_TALEP_EDEN { get; set; }

}