using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{

    [DataContract]
    public class IsEmriLog
    {
        [DataMember]
        public int TB_ISEMRI_LOG_ID { get; set; }

        [DataMember]
        public int ISL_ISEMRI_ID { get; set; }

        [DataMember]
        public int ISL_KULLANICI_ID { get; set; }

        [DataMember]
        public string ISL_KULLANICI { get; set; }

        [DataMember]
        public DateTime? ISL_TARIH { get; set; }

        [DataMember]
        public string ISL_SAAT { get; set; }

        [DataMember]
        public string ISL_ISLEM { get; set; }

        [DataMember]
        public string ISL_ACIKLAMA { get; set; }

        [DataMember]
        public int ISL_DURUM_ESKI_KOD_ID { get; set; }

        [DataMember]
        public int ISL_DURUM_YENI_KOD_ID { get; set; }

        [DataMember]
        public int ISL_OLUSTURAN_ID { get; set; }

        [DataMember]
        public DateTime? ISL_OLUSTURMA_TARIH { get; set; }

        [DataMember]
        public string ISL_DURUM_ESKI_KOD_TANIM { get; set; }

        [DataMember]
        public string ISL_DURUM_YENI_KOD_TANIM { get; set; }

    }
}