using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
	[DataContract]
	public class PersonelSantiye
	{
		[DataMember]
		public int TB_PERSONEL_SANTIYE_ID { get; set; }

		[DataMember]
		public int PSS_PERSONEL_ID { get; set; }

		[DataMember]
		public int PSS_SANTIYE_ID { get; set; }

		[DataMember]
		public string PSS_ACIKLAMA { get; set; }

		[DataMember]
		public int PSS_OLUSTURAN_ID { get; set; }

		[DataMember]
		public DateTime? PSS_OLUSTURMA_TARIH { get; set; }

		[DataMember]
		public int PSS_DEGISTIREN_ID { get; set; }

		[DataMember]
		public DateTime? PSS_DEGISTIRME_TARIH { get; set; }

		[DataMember]
		public int PSS_AYRILMA_NEDEN_KOD_ID { get; set; }

		[DataMember]
		public string SANTIYE { get; set; }

		[DataMember]
		public string PERSONEL { get; set; }

		[DataMember]
		public string PSS_AYRILMA_NEDEN { get; set; }



	}
}