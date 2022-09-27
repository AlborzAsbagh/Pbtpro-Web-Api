using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class Atolye
    {   [DataMember]
        public int TB_ATOLYE_ID { get; set; }
        [DataMember]
        public string ATL_KOD { get; set; }
        [DataMember]
        public string ATL_TANIM { get; set; }
    }
}