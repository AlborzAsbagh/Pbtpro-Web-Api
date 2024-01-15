using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
	
	public class LokasyonTip
	{
		[DataMember]
		public int TB_LOKASYON_TIP_ID { get; set; }

		[DataMember]
		public string LOT_TANIM { get; set; }

		[DataMember]
		public bool LOT_VARSAYILAN { get; set; }

		[DataMember]
		public int LOT_ICON_ID { get; set; }

		[DataMember]
		public int LOT_OLUSTURAN_ID { get; set; }

		[DataMember]
		public DateTime? LOT_OLUSTURMA_TARIH { get; set; }

		[DataMember]
		public int LOT_DEGISTIREN_ID { get; set; }

		[DataMember]
		public DateTime? LOT_DEGISTIRME_TARIH { get; set; }


	}
}