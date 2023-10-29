using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
	[DataContract]
	public class Takvim
	{
		[DataMember]
		public int TB_TAKVIM_ID { get; set; }

		[DataMember]
		public string TKV_TANIM { get; set; }

		[DataMember]
		public bool TKV_AKTIF { get; set; }

		[DataMember]
		public string TKV_HAFTA_CALISMA_GUN { get; set; }

		[DataMember]
		public string TKV_ACIKLAMA { get; set; }

		[DataMember]
		public string TKV_YIL { get; set; }

		[DataMember]
		public int TKV_OLUSTURAN_ID { get; set; }

		[DataMember]
		public DateTime TKV_OLUSTURMA_TARIH { get; set; }

		[DataMember]
		public int TKV_DEGISTIREN_ID { get; set; }

		[DataMember]
		public DateTime TKV_DEGISTIRME_TARIH { get; set; }
	}
}