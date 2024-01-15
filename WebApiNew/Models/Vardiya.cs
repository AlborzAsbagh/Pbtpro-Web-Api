using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    public class Vardiya
    {
        [DataMember]
        public int TB_VARDIYA_ID { get; set; }
		[DataMember]
		public string VAR_TANIM { get; set; }
		[DataMember]
		public int VAR_LOKASYON_ID { get; set; }
		[DataMember]
		public string VAR_LOKASYON { get; set; }
		[DataMember]
		public int VAR_PROJE_ID { get; set; }
		[DataMember]
		public string VAR_PROJE { get; set; }
		[DataMember]
		public string VAR_ACIKLAMA { get; set; }
		[DataMember]
		public string VAR_BASLAMA_SAATI { get; set; }
		[DataMember]
		public string VAR_BITIS_SAATI { get; set; }
		[DataMember]
		public int VAR_MOLA_SURESI { get; set; }
		[DataMember]
		public int VAR_VARDIYA_TIPI_KOD_ID { get; set; }
		[DataMember]
		public string VAR_VARDIYA_TIPI { get; set; }
		[DataMember]
		public short VAR_VARSAYILAN { get; set; }
		[DataMember]
		public int VAR_RENK { get; set; }
		[DataMember]
		public int VAR_OLUSTURAN_ID { get; set; }
		[DataMember]
		public DateTime? VAR_OLUSTURMA_TARIH { get; set; }
		[DataMember]
		public int VAR_DEGISTIREN_ID { get; set; }
		[DataMember]
		public DateTime? VAR_DEGISTIRME_TARIH { get; set; }
    }
}