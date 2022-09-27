using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class Ekipman
    {
        [DataMember]
        public int TB_EKIPMAN_ID { get; set; }

        [DataMember]
        public string EKP_KOD { get; set; }

        [DataMember]
        public string EKP_TANIM { get; set; }

        [DataMember]
        public int EKP_REF_ID { get; set; }
        [DataMember]
        public string EKP_REF_GRUP { get; set; }
    }
}