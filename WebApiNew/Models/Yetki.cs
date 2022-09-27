using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    [DataContract]
    public class Yetki
    {
        [DataMember]
        public int TB_KULLANICI_YETKI_ID { get; set; }
        [DataMember]
        public int? KYT_KULLANICI_ID { get; set; }
        [DataMember]
        public string KYT_YETKI_KOD { get; set; }
        [DataMember]
        public string KYT_YETKI_TANIM { get; set; }
        [DataMember]
        public bool? KYT_EKLE { get; set; }
        [DataMember]
        public bool? KYT_SIL { get; set; }
        [DataMember]
        public bool? KYT_DEGISTIR { get; set; }
        [DataMember]
        public bool? KYT_GOR { get; set; }
        [DataMember]
        public int? KYT_OLUSTURAN_ID { get; set; }
        [DataMember]
        public DateTime? KYT_OLUSTURMA_TARIH { get; set; }
        [DataMember]
        public int? KYT_DEGISTIREN_ID { get; set; }
        [DataMember]
        public DateTime? KYT_DEGISTIRME_TARIH { get; set; }
    }
}