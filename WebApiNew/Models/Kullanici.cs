using System.Runtime.Serialization;

namespace WebApiNew.Models
{   [DataContract]
    public class Kullanici
    {
        public Kullanici()
        {
            lokasyonId = -1;
        }
        [DataMember]
        public int TB_KULLANICI_ID { get; set; }

        [DataMember]
        public int KLL_PERSONEL_ID { get; set; }

        [DataMember]
        public int resimId { get; set; }

        [DataMember]
        public int lokasyonId { get; set; }

        [DataMember]
        public string KLL_TANIM { get; set; }

        [DataMember]
        public string KLL_KOD { get; set; }

        [DataMember]
        public string KLL_SIFRE { get; set; }

        [DataMember]
        public string apiVer { get; set; }

        [DataMember]
        public string dbName { get; set; }

        [DataMember]
        public bool KLL_AKTIF { get; set; }

        [DataMember]
        public string KLL_MAIL { get; set; }

        [DataMember]
        public string KLL_MOBILCIHAZ_ID { get; set; }

        [DataMember]
        public Personel KLL_PERSONEL { get; set; }
    }

}