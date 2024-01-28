using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
	[DataContract]
	public class PersonelSertifika
	{
		[DataMember]
		public int TB_PERSONEL_SERTIFIKA_ID { get; set; }

		[DataMember]
		public int PSE_PERSONEL_ID { get; set; }

		[DataMember]
		public int PSE_SERTIFIKA_TIP_KOD_ID { get; set; }

		[DataMember]
		public string PSE_BELGE_NO { get; set; }

		[DataMember]
		public DateTime? PSE_VERILIS_TARIH { get; set; }

		[DataMember]
		public DateTime? PSE_BITIS_TARIH { get; set; }

		[DataMember]
		public string PSE_ACIKLAMA { get; set; }

		[DataMember]
		public int PSE_OLUSTURAN_ID { get; set; }

		[DataMember]
		public DateTime? PSE_OLUSTURMA_TARIH { get; set; }

		[DataMember]
		public int PSE_DEGISTIREN_ID { get; set; }

		[DataMember]
		public DateTime? PSE_DEGISTIRME_TARIH { get; set; }

		[DataMember]
		public string PSE_SERTIFIKA_TIP { get; set; }


	}
}