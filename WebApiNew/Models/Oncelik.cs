using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class Oncelik
    {
        [DataMember]
        public int TB_SERVIS_ONCELIK_ID { get; set; }

        [DataMember]
        public string SOC_KOD { get; set; }

        [DataMember]
        public string SOC_TANIM { get; set; }

        [DataMember]
        public int SOC_IKON_INDEX_ID { get; set; }

        [DataMember]
        public bool SOC_VARSAYILAN { get; set; }

        [DataMember]
        public bool SOC_AKTIF { get; set; }

        [DataMember]
        public int SOC_COZUM_SURE_GUN { get; set; }

		[DataMember]
		public int SOC_COZUM_SURE_SAAT { get; set; }

		[DataMember]
		public int SOC_COZUM_SURE_DK { get; set; }

        [DataMember]
        public int SOC_GECIKME_SURE_DAKIKA { get; set; }

        [DataMember]
        public string SOC_GECIKME_RENK { get; set; }

        [DataMember]
        public int SOC_GECIKME_IKON_INDEX { get; set; }

        [DataMember]
        public int SOC_KRITIK_SURE_DAKIKA {get; set; }

        [DataMember]
        public string SOC_KRITIK_RENK { get; set; }

        [DataMember]
        public int SOC_KRITIK_IKON_INDEX { get; set; }


	}
}