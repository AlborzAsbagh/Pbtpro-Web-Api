using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class MasrafMerkezi
    {
        [DataMember]
        public int TB_MASRAF_MERKEZ_ID { get; set; }

        [DataMember]
        public string MAM_KOD { get; set; }

        [DataMember]
        public string MAM_TANIM { get; set; }

        [DataMember]
        public string MAM_ACIKLAMA { get; set; }

        [DataMember]
        public int MAM_USTGRUP_ID { get; set; }


	}
}