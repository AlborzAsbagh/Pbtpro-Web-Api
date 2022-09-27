using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class Proje
    {
         [DataMember] public int TB_PROJE_ID { get; set; }

         [DataMember] public string PRJ_KOD { get; set; }

         [DataMember] public string PRJ_TANIM { get; set; }
    }

    public class ProjeIs
    {
        public int TB_PROJE_IS_ID { get; set; }
        public string PIS_KODU { get; set; }
        public string PIS_TANIMI { get; set; }
        public string PIS_BIRIM_KOD { get; set; }
        public int PIS_TIP_KOD_ID { get; set; }
        public int PIS_BIRIM_KOD_ID { get; set; }
        public double PIS_MIKTAR { get; set; }
        public DateTime? PIS_BASLAMA_TARIH { get; set; }
        public string PIS_BASLAMA_SAAT { get; set; }
        public DateTime? PIS_BITIS_TARIH { get; set; }
        public string PIS_BITIS_SAAT { get; set; }
        public string PIS_SURE { get; set; }
        public int PIS_YETKILI_ID { get; set; }
        public int PIS_YER_KOD_ID { get; set; }
        public int PIS_DURUM_KOD_ID { get; set; }
        public double PIS_MALIYET { get; set; }
        public int PIS_MASRAF_YERI_ID { get; set; }
        public string PIS_ACIKLAMA { get; set; }
        public int PIS_IS_REF_ID { get; set; }
        public int PIS_UST_GRUP_ID { get; set; }
        public string PIS_YETKILI { get; set; }
        public int PIS_ICON_ID { get; set; }
        public int PIS_PROJE_REF_ID { get; set; }
        public int PIS_OLUSTURAN_ID { get; set; }
        public DateTime? PIS_OLUSTURMA_TARIH { get; set; }
        public int PIS_DEGISTIREN_ID { get; set; }
        public DateTime? PIS_DEGISTIRME_TARIH { get; set; }
        public DateTime? PIS_PLANLANAN_BASLAMA_TARIH { get; set; }
        public DateTime? PIS_PLANLANAN_BITIS_TARIH { get; set; }
        public string PIS_PLANLANAN_SURE { get; set; }
        public double PIS_PLANLANAN_MIKTAR { get; set; }
        public double PIS_PLANLANAN_TUTAR { get; set; }
        public int PIS_MIKTAR_YUZDE { get; set; }
        public int PIS_MALIYET_YUZDE { get; set; }
        public double PIS_KALAN_MIKTAR { get; set; }
        public double PIS_KALAN_MALIYET { get; set; }
        public int PIS_SURE_YUZDE { get; set; }
        public string PIS_GRUP_IS { get; set; }
        public double PIS_BIRIM_UCRET { get; set; }
        public int PIS_GRUP_KOD_ID { get; set; }
        public double PIS_KATSAYI_YUZDE { get; set; }
        public byte PIS_TUR { get; set; }
        public int PIS_KAYNAK_ID { get; set; }
        public int PIS_KAYNAK_GRUP_KOD_ID { get; set; }
        public string PIS_OZEL_ALAN_1 { get; set; }
        public string PIS_OZEL_ALAN_2 { get; set; }
        public string PIS_OZEL_ALAN_3 { get; set; }
        public string PIS_OZEL_ALAN_4 { get; set; }
        public string PIS_OZEL_ALAN_5 { get; set; }
        public double PIS_OZEL_ALAN_6 { get; set; }
        public double PIS_OZEL_ALAN_7 { get; set; }
        public double PIS_OZEL_ALAN_8 { get; set; }
        public double PIS_OZEL_ALAN_9 { get; set; }
        public double PIS_OZEL_ALAN_10 { get; set; }
    }

}