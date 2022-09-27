using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class PeriyodikBakim
    {
        [DataMember]
        public int TB_PERIYODIK_BAKIM_ID { get; set; }

        [DataMember]
        public string PBK_KOD { get; set; }

        [DataMember]
        public string PBK_TANIM { get; set; }

        [DataMember]
        public bool PBK_AKTIF { get; set; }

        [DataMember]
        public int PBK_TIP_KOD_ID { get; set; }

        [DataMember]
        public int PBK_GRUP_KOD_ID { get; set; }

        [DataMember]
        public bool PBK_ISEMRI_VAR { get; set; }

    }
}