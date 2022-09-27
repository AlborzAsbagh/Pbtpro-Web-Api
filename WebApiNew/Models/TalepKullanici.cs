using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class TalepKullanici
    {
        [DataMember]
        public int TB_IS_TALEBI_KULLANICI_ID { get; set; }
        [DataMember]
        public string ISK_KOD { get; set; }
        [DataMember]
        public string ISK_ISIM { get; set; }
        [DataMember]
        public int ISK_LOKASYON_ID { get; set; }
        [DataMember]
        public int ISK_PERSONEL_ID { get; set; }
    }
}