using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class PBakimMalzeme
    {
        [DataMember]
        public int TB_PERIYODIK_BAKIM_MLZ_ID { get; set; }

        [DataMember]
        public int PBM_PERIYODIK_BAKIM_ID { get; set; }

        [DataMember]
        public int PBM_STOK_ID { get; set; }

        [DataMember]
        public int PBM_BIRIM_KOD_ID { get; set; }

        [DataMember]
        public double PBM_BIRIM_FIYAT { get; set; }

        [DataMember]
        public double PBM_MIKTAR { get; set; }

        [DataMember]
        public double PBM_TUTAR { get; set; }

        [DataMember]
        public string PBM_ACIKLAMA { get; set; }

        [DataMember]
        public string PBM_STOK_TANIM { get; set; }

        [DataMember]
        public string PBM_BIRIM { get; set; }

        [DataMember]
        public string PBM_STOK_KOD { get; set; }

    }
}