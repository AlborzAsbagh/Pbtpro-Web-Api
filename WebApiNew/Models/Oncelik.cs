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
    }
}