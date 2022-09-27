using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class Kod
    {
        [DataMember]
        public int TB_KOD_ID { get; set; }

        [DataMember]
        public string KOD_GRUP { get; set; }

        [DataMember]
        public string KOD_TANIM { get; set; }
    }
}