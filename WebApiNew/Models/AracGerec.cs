using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
	[DataContract]
	public class AracGerec
	{
		[DataMember]
		public int TB_ARAC_GEREC_ID { get; set; }

		[DataMember]
		public string ARG_KOD { get; set; }

		[DataMember]
		public string ARG_TANIM { get; set; }

		[DataMember]
		public int ARG_TIP_KOD_ID { get; set; }

		[DataMember]
		public int ARG_BIRIM_KOD_ID { get; set; }

		[DataMember]
		public string ARG_SERI_NO { get; set; }

		[DataMember]
		public string ARG_ACIKLAMA { get; set; }

		[DataMember]
		public decimal ARG_MIKTAR { get; set; }

		[DataMember]
		public int ARG_YER_KOD_ID { get; set; }

		[DataMember]
		public bool ARG_KALIBRASYON_VAR { get; set; }

		[DataMember]
		public DateTime ARG_KALIBRASYON_TARIHI { get; set; }

		[DataMember]
		public bool ARG_ZIMMET_VAR { get; set; }

		[DataMember]
		public int ARG_ZIMMETLI_ID { get; set; }

		[DataMember]
		public DateTime ARG_SAYIM_TARIH { get; set; }

		[DataMember]
		public string ARG_SAYIM_SAAT { get; set; }

		[DataMember]
		public string ARG_SAYIM_ACIKLAMA { get; set; }

		[DataMember]
		public int ARG_REF_ID { get; set; }

		[DataMember]
		public bool ARG_AKTIF { get; set; }

		[DataMember]
		public DateTime ARG_DEGISTIRME_TARIH { get; set; }

		[DataMember]
		public int ARG_DEGISTIREN_ID { get; set; }

		[DataMember]
		public DateTime ARG_OLUSTURMA_TARIH { get; set; }

		[DataMember]
		public int ARG_OLUSTURAN_ID { get; set; }

		[DataMember]
		public string ARG_OZEL_ALAN_1 { get; set; }

		[DataMember]
		public string ARG_OZEL_ALAN_2 { get; set; }

		[DataMember]
		public string ARG_OZEL_ALAN_3 { get; set; }

		[DataMember]
		public string ARG_OZEL_ALAN_4 { get; set; }

		[DataMember]
		public string ARG_OZEL_ALAN_5 { get; set; }

		[DataMember]
		public string ARG_OZEL_ALAN_6 { get; set; }

		[DataMember]
		public string ARG_OZEL_ALAN_7 { get; set; }

		[DataMember]
		public string ARG_OZEL_ALAN_8 { get; set; }

		[DataMember]
		public string ARG_OZEL_ALAN_9 { get; set; }

		[DataMember]
		public string ARG_OZEL_ALAN_10 { get; set; }

		[DataMember]
		public string ARG_BELGE { get; set; }

		[DataMember]
		public string ARG_RESIM { get; set; }

		[DataMember]
		public string ARG_TIP_TANIM { get; set; }

		[DataMember]
		public string ARG_BIRIM_TANIM { get; set; }

		[DataMember]
		public string ARG_YER_TANIM { get; set; }

		[DataMember]
		public int ARG_STOK { get; set; }

		[DataMember]
		public int ARG_KULLANILAN { get; set; }
	}
}
