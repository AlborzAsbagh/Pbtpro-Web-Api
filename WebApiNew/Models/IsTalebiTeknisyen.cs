using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{

	[DataContract]
	public class IsTalebiTeknisyen
	{
		[DataMember]
		public int TB_IS_TALEBI_TEKNISYEN_ID { get; set; }

		[DataMember]
		public int ITK_IS_TALEBI_ID { get; set; }

		[DataMember]
		public string ITK_PERSONEL_ISIM { get; set; }

		[DataMember]
		public string ITK_VARDIYA_TANIM { get; set; }

		[DataMember]
		public int ITK_SURE { get; set; }

		[DataMember]
		public int ITK_SAAT_UCRETI { get; set; }

		[DataMember]
		public bool ITK_FAZLA_MESAI_VAR { get; set; }

		[DataMember]
		public int ITK_FAZLA_MESAI_SURE { get; set; }

		[DataMember]
		public int ITK_FAZLA_MESAI_SAAT_UCRETI { get; set; }

		[DataMember]
		public int ITK_MALIYET { get; set; }

	}
}
