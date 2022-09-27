using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    [DataContract]
    public class SatinAlmaAyar
    {
        [DataMember]
        public int TB_SATINALMA_AYAR_ID { get; set; }

        [DataMember]
        public bool STA_ONAY_AKTIF { get; set; }

        [DataMember]
        public bool STA_SIP_ONAY_AKTIF { get; set; }

        [DataMember]
        public string STA_TALEP_ACIK_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_TEKLIF_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_SIPARIS_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_KARSILANIYOR_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_KAPANDI_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_IPTAL_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_ONAY_BEKLIYOR_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_ONAYLANDI_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_ONAYLANMADI_RENK { get; set; }

        [DataMember]
        public string STA_SIPARIS_ACIK_RENK { get; set; }

        [DataMember]
        public string STA_SIPARIS_KARSILANIYOR_RENK { get; set; }

        [DataMember]
        public string STA_SIPARIS_KAPANDI_RENK { get; set; }

        [DataMember]
        public string STA_SIPARIS_IPTAL_RENK { get; set; }

        [DataMember]
        public string STA_SIPARIS_ONAY_BEKLIYOR_RENK { get; set; }

        [DataMember]
        public string STA_SIPARIS_ONAYLANDI_RENK { get; set; }

        [DataMember]
        public string STA_SIPARIS_ONAYLANMADI_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_ACIK_YAZI_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_TEKLIF_YAZI_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_SIPARIS_YAZI_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_KARSILANIYOR_YAZI_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_KAPANDI_YAZI_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_IPTAL_YAZI_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_ONAY_BEKLIYOR_YAZI_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_ONAYLANDI_YAZI_RENK { get; set; }

        [DataMember]
        public string STA_TALEP_ONAYLANMADI_YAZI_RENK { get; set; }

        [DataMember]
        public string STA_SIPARIS_ACIK_YAZI_RENK { get; set; }

        [DataMember]
        public string STA_SIPARIS_KARSILANIYOR_YAZI_RENK { get; set; }

        [DataMember]
        public string STA_SIPARIS_KAPANDI_YAZI_RENK { get; set; }

        [DataMember]
        public string STA_SIPARIS_IPTAL_YAZI_RENK { get; set; }

        [DataMember]
        public string STA_SIPARIS_ONAY_BEKLIYOR_YAZI_RENK { get; set; }

        [DataMember]
        public string STA_SIPARIS_ONAYLANDI_YAZI_RENK { get; set; }

        [DataMember]
        public string STA_SIPARIS_ONAYLANMADI_YAZI_RENK { get; set; }

        [DataMember]
        public bool STA_TALEP_ACIK_DEGISTIR { get; set; }

        [DataMember]
        public bool STA_TALEP_ONAY_BEKLIYOR_DEGISTIR { get; set; }

        [DataMember]
        public bool STA_TALEP_ONAYLANDI_DEGISTIR { get; set; }

        [DataMember]
        public bool STA_TALEP_TEKLIF_DEGISTIR { get; set; }

        [DataMember]
        public int STA_OLUSTURAN_ID { get; set; }

        [DataMember]
        public DateTime? STA_OLUSTURMA_TARIH { get; set; }

        [DataMember]
        public int STA_DEGISTIREN_ID { get; set; }

        [DataMember]
        public DateTime? STA_DEGISTIRME_TARIH { get; set; }
    }
}