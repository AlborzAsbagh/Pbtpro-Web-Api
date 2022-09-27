using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class Sayac
    {
        [DataMember]
        public int TB_SAYAC_ID { get; set; }

        [DataMember]
        public string MES_TANIM { get; set; }

        [DataMember]
        public int MES_BIRIM_KOD_ID { get; set; }

        [DataMember]
        public string MES_SAYAC_SEKLI { get; set; }

        [DataMember]
        public DateTime? MES_SON_OKUMA_TARIH { get; set; }

        [DataMember]
        public string MES_SON_OKUMA_SAAT { get; set; }

        [DataMember]
        public double MES_GUNCEL_DEGER { get; set; }

        [DataMember]
        public double MES_TAHMINI_ARTIS_DEGER { get; set; }

        [DataMember]
        public DateTime? MES_BASLANGIC_TARIH { get; set; }

        [DataMember]
        public string MES_BASLANGIC_SAAT { get; set; }

        [DataMember]
        public double MES_BASLANGIC_DEGER { get; set; }

        [DataMember]
        public int MES_REF_ID { get; set; }

        [DataMember]
        public string MES_REF_GRUP { get; set; }

        [DataMember]
        public int MES_OLUSTURAN_ID { get; set; }

        [DataMember]
        public DateTime? MES_OLUSTURMA_TARIH { get; set; }

        [DataMember]
        public int MES_DEGISTIREN_ID { get; set; }

        [DataMember]
        public DateTime? MES_DEGISTIRME_TARIH { get; set; }

        [DataMember]
        public int MES_TIP_KOD_ID { get; set; }

        [DataMember]
        public bool MES_SANAL_SAYAC { get; set; }

        [DataMember]
        public double MES_SANAL_SAYAC_ARTIS { get; set; }

        [DataMember]
        public DateTime? MES_SANAL_SAYAC_BASLANGIC_TARIH { get; set; }

        [DataMember]
        public string MES_ACIKLAMA { get; set; }

        [DataMember]
        public short MES_GUNCELLEME_SEKLI { get; set; }

        [DataMember]
        public bool MES_VARSAYILAN { get; set; }

        [DataMember]
        public int MES_MAKINE_ID { get; set; }

        [DataMember]
        public string MES_OZEL_ALAN_1 { get; set; }

        [DataMember]
        public string MES_OZEL_ALAN_2 { get; set; }

        [DataMember]
        public string MES_OZEL_ALAN_3 { get; set; }

        [DataMember]
        public string MES_OZEL_ALAN_4 { get; set; }

        [DataMember]
        public string MES_OZEL_ALAN_5 { get; set; }

        [DataMember]
        public double MES_OZEL_ALAN_6 { get; set; }

        [DataMember]
        public double MES_OZEL_ALAN_7 { get; set; }

        [DataMember]
        public double MES_OZEL_ALAN_8 { get; set; }

        [DataMember]
        public double MES_OZEL_ALAN_9 { get; set; }

        [DataMember]
        public double MES_OZEL_ALAN_10 { get; set; }

        [DataMember]
        public string MES_SAYAC_BIRIM { get; set; }

        [DataMember]
        public string MES_SAYAC_TIP { get; set; }
    }
}