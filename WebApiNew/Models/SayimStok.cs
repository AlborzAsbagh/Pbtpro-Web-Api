using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    [DataContract]
    public class SayimStok
    {
        [DataMember]
        public int TB_STOK_SAYIM_DETAY_ID { get; set; }

        [DataMember]
        public int SSD_STOK_SAYIM_ID { get; set; }

        [DataMember]
        public int SSD_STOK_ID { get; set; }

        [DataMember]
        public double SSD_STOK_MIKTAR { get; set; }

        [DataMember]
        public double SSD_SAYIM_MIKTAR { get; set; }

        [DataMember]
        public double SSD_FARK_MIKTAR { get; set; }

        [DataMember]
        public int SSD_OLUSTURAN_ID { get; set; }

        [DataMember]
        public DateTime? SSD_OLUSTURMA_TARIH { get; set; }

        [DataMember]
        public int SSD_DEGISTIREN_ID { get; set; }

        [DataMember]
        public DateTime? SSD_DEGISTIRME_TARIH { get; set; }

        [DataMember]
        public string STK_TANIM { get; set; }

        [DataMember]
        public string STK_KOD { get; set; }

        [DataMember]
        public string STK_URETICI_KOD { get; set; }

        [DataMember]
        public string STK_TIP { get; set; }

        [DataMember]
        public string STK_GRUP { get; set; }

        [DataMember]
        public string STK_MARKA { get; set; }

        [DataMember]
        public string STK_MODEL { get; set; }

        [DataMember]
        public string STK_DEPO_LOKASYON { get; set; }
        [DataMember]
        public string STK_BIRIM { get; set; }

    }
}