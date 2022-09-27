using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class DepoStok
    {
        [DataMember]
        public int TB_DEPO_STOK_ID { get; set; }

        [DataMember]
        public int DPS_DEPO_ID { get; set; }

        [DataMember]
        public int DPS_STOK_ID { get; set; }

        [DataMember]
        public double DPS_GIREN_MIKTAR { get; set; }

        [DataMember]
        public double DPS_CIKAN_MIKTAR { get; set; }

        [DataMember]
        public double DPS_MIKTAR { get; set; }

        [DataMember]
        public double DPS_REZERV_MIKTAR { get; set; }

        [DataMember]
        public double DPS_KULLANILABILIR_MIKTAR { get; set; }

        [DataMember]
        public string DPS_DEPO { get; set; }

        [DataMember]
        public string DPS_STOK { get; set; }

        [DataMember]
        public string DPS_STOK_KOD { get; set; }

        [DataMember]
        public int DPS_MALZEME_TIP_ID { get; set; }

        [DataMember]
        public string DPS_MALZEME_TIP { get; set; }

        [DataMember]
        public int DPS_BIRIM_ID { get; set; }

        [DataMember]
        public double DPS_FIYAT { get; set; }

        [DataMember]
        public int DPS_OLUSTURAN_ID { get; set; }

        [DataMember]
        public DateTime? DPS_OLUSTURMA_TARIH { get; set; }

        [DataMember]
        public int DPS_DEGISTIREN_ID { get; set; }

        [DataMember]
        public DateTime? DPS_DEGISTIRME_TARIH { get; set; }

        [DataMember]
        public string DPS_STOK_BIRIM { get; set; }

        [DataMember]
        public string DPS_STOK_SINIF { get; set; }

    }
}