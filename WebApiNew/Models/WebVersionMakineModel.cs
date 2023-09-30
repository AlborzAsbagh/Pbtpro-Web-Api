using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
	[DataContract]
	public class WebVersionMakineModel
	{
		[DataMember]
		public int TB_MAKINE_ID { get; set; }

		[DataMember] 
		public string MKN_KOD { get; set; }

		[DataMember]
		public string MKN_TANIM { get; set; }

		[DataMember]
		public string MAKINE_LOKASYON { get; set; }

		[DataMember]
		public string MAKINE_LOKASYON_ID { get; set; }

		[DataMember]
		public string MAKINE_TIP { get; set; }

		[DataMember]
		public string MAKINE_KATEGORI { get; set ; }

		[DataMember]
		public string MAKINE_MARKA { get; set; }

		[DataMember]
		public string MAKINE_MODEL { get; set; }

		[DataMember]
		public string MAKINE_SERI_NO { get; set; }
		
	}
}

