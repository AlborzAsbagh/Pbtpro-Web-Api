using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
	[DataContract]
	public class IsTalepKullanici
	{
		
		[DataMember]
		public int TB_IS_TALEBI_KULLANICI_ID { get; set; }

		[DataMember]
		public string ISK_KOD { get; set; }

		[DataMember]
		public string ISK_ISIM { get; set; }

		[DataMember]
		public int ISK_DEPARTMAN_ID { get; set; }

		[DataMember]
		public int ISK_KULLANICI_TIP_KOD_ID { get; set; }

		[DataMember]
		public string ISK_UNVAN { get; set; }

		[DataMember]
		public int ISK_LOKASYON_ID { get; set; }

		[DataMember]
		public string ISK_LOKASYON { get; set; }

		[DataMember]
		public string ISK_ADRES { get; set; }

		[DataMember]
		public string ISK_IL { get; set; }

		[DataMember]
		public string ISK_ILCE { get; set; }

		[DataMember]
		public string ISK_TELEFON_1 { get; set; }

		[DataMember]
		public string ISK_TELEFON_2 { get; set; }

		[DataMember]
		public string ISK_FAX { get; set; }

		[DataMember]
		public string ISK_GSM { get; set; }

		[DataMember]
		public string ISK_DAHILI { get; set; }

		[DataMember]
		public string ISK_MAIL { get; set; }

		[DataMember]
		public bool ISK_AKTIF { get; set; }

		[DataMember]
		public string ISK_ACIKLAMA { get; set; }

		[DataMember]
		public string ISK_SIFRE { get; set; }

		[DataMember]
		public int ISK_KAYIT_SAYISI { get; set; }

		[DataMember]
		public int ISK_ACILIS_DURUM { get; set; }

		[DataMember]
		public string ISK_MAIL_SIFRE { get; set; }

		[DataMember]
		public int ISK_YENILEME_SURESI { get; set; }

		[DataMember]
		public int ISK_OLUSTURAN_ID { get; set; }

		[DataMember]
		public int ISK_DEGISTIREN_ID { get; set; }

		[DataMember]
		public DateTime? ISK_OLUSTURMA_TARIH { get; set; }

		[DataMember]
		public DateTime? ISK_DEGISTIRME_TARIH { get; set; }

		[DataMember]
		public int ISK_PERSONEL_ID { get; set; }

		[DataMember]
		public string ISK_KULLANICI_TIP { get; set; }

		[DataMember]
		public string ISK_PERSONEL_ISIM { get; set; }




	}

}