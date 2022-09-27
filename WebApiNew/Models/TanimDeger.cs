using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class TanimDeger
    {
        [DataMember]
        public int TanimDegerID { get; set; }
        [DataMember]
        public string Tanim { get; set; }
        [DataMember]
        public double Deger { get; set; }
    }
}