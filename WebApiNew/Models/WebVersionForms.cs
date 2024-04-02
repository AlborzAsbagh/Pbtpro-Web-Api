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

	[DataMember]
	public string ISM_PROSEDUR { get; set; }

	[DataMember]
	public string ISM_PROSEDUR_ACIKLAMA { get; set; }

	[DataMember]
	public string ISM_PROSEDUR_TIP { get; set; }

	[DataMember]
	public string ISM_PERSONEL_KOD { get; set; }

	[DataMember]
	public string ISM_PERSONEL_ISIM { get; set; }

	[DataMember]
	public int ISM_PERSONEL_SURE { get; set; }

	[DataMember]
	public int ISM_PERSONEL_FAZLA_MESAI_SURE { get; set; }

	[DataMember]
	public float ISM_PERSONEL_MALIYET { get; set; }	



}

[DataContract]
public class WebVersionIsTalpForm
{
	[DataMember]
	public int TB_IS_TALEP_ID { get; set; }

	[DataMember]
	public string IST_ISEMRI_NO { get; set; }

	[DataMember]
	public string IST_KOD { get; set; }

	[DataMember]
	public DateTime? IST_BASLAMA_TARIHI { get; set; }

	[DataMember]
	public string IST_IS_TAKIPCISI { get; set; }

	[DataMember]
	public string IST_TALEP_EDEN { get; set; }

	[DataMember]
	public string IST_ONCELIK { get; set; }

	[DataMember]
	public string IST_BILDIRILEN_BINA { get; set; }

	[DataMember]
	public string IST_BILDIRILEN_KAT { get; set; }

	[DataMember]
	public string IST_SERVIS { get; set; }

	[DataMember]
	public string IST_KATEGORI { get; set; }

	[DataMember]
	public string IST_IRTIBAT { get; set; }

	[DataMember]
	public string IST_MAKINE_TANIM { get; set; }

	[DataMember]
	public string IST_MAKINE_KOD { get; set; }

	[DataMember]
	public string IST_EKIPMAN_TANIM { get; set; }

	[DataMember]
	public string IST_EKIPMAN_KOD { get; set; }

	[DataMember]
	public string IST_IRTIBAT_TELEFON { get; set; }

	[DataMember]
	public string IST_MAIL_ADRES { get; set; }

	[DataMember]
	public string IST_KONU { get; set; }

	[DataMember]
	public string IST_ACIKLAMA { get; set; }
}

public class WebVersionBakimForm
{
	[DataMember]
	public int TB_IS_TANIM_ID { get; set; }

	[DataMember]
	public string IST_KOD { get; set; }

	[DataMember]
	public string IST_TANIM { get; set; }

	[DataMember]
	public string IST_TIP { get; set; }

	[DataMember]
	public string IST_GRUP { get; set; }

	[DataMember]
	public string IST_NEDEN { get; set; }

	[DataMember]
	public string IST_ONCELIK { get; set; }

	[DataMember]
	public string IST_TALIMAT { get; set; }

	[DataMember]
	public string IST_ATOLYE { get; set; }

	[DataMember]
	public string IST_FIRMA { get; set; }

	[DataMember]
	public int IST_CALISMA_SURE { get; set; }

	[DataMember]
	public int IST_DURUS_SURE { get; set; }

	[DataMember]
	public string IST_PERSONEL { get; set; }

}