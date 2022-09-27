using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class PBakimKontrolList
    {
        [DataMember]
        public int TB_PERIYODIK_BAKIM_KONTROLLIST_ID { get; set; }

        [DataMember]
        public int PKN_PERIYODIK_BAKIM_ID { get; set; }

        [DataMember]
        public string PKN_SIRANO { get; set; }

        [DataMember]
        public bool PKN_YAPILDI { get; set; }

        [DataMember]
        public string PKN_TANIM { get; set; }

        [DataMember]
        public float PKN_MALIYET { get; set; }

        [DataMember]
        public string PKN_ACIKLAMA { get; set; }
    }
}