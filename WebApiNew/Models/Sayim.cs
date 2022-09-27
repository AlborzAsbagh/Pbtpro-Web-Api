using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class Sayim
    {
        [DataMember]
        public int TB_STOK_SAYIM_ID { get; set; }

        [DataMember]
        public string SYM_FIS_NO { get; set; }

        [DataMember]
        public DateTime? SYM_TARIH { get; set; }

        [DataMember]
        public string SYM_SAAT { get; set; }

        [DataMember]
        public int SYM_DEPO_ID { get; set; }

        [DataMember]
        public int SYM_PERSONEL_ID { get; set; }

        [DataMember]
        public string SYM_ACIKLAMA { get; set; }

        [DataMember]
        public int SYM_OLUSTURAN_ID { get; set; }

        [DataMember]
        public DateTime? SYM_OLUSTURMA_TARIH { get; set; }

        [DataMember]
        public int SYM_DEGISTIREN_ID { get; set; }

        [DataMember]
        public DateTime? SYM_DEGISTIRME_TARIH { get; set; }

        [DataMember]
        public bool SYM_KAPALI { get; set; }

        [DataMember]
        public string SYM_PERSONEL { get; set; }

        [DataMember]
        public string SYM_DEPO { get; set; }

    }

}