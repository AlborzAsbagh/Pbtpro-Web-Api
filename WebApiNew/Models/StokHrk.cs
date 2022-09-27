using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    [DataContract]
    public class StokHrk
    {
        [DataMember]
        public int TB_STOK_HRK_ID { get; set; }

        [DataMember]
        public int SHR_STOK_FIS_DETAY_ID { get; set; }

        [DataMember]
        public int SHR_STOK_ID { get; set; }

        [DataMember]
        public int SHR_BIRIM_KOD_ID { get; set; }

        [DataMember]
        public int SHR_DEPO_ID { get; set; }

        [DataMember]
        public DateTime? SHR_TARIH { get; set; }

        [DataMember]
        public string SHR_SAAT { get; set; }

        [DataMember]
        public double SHR_MIKTAR { get; set; }

        [DataMember]
        public double SHR_ANA_BIRIM_MIKTAR { get; set; }

        [DataMember]
        public double SHR_BIRIM_FIYAT { get; set; }

        [DataMember]
        public byte SHR_KDV_ORAN { get; set; }

        [DataMember]
        public double SHR_KDV_TUTAR { get; set; }

        [DataMember]
        public int SHR_OTV_ORAN { get; set; }

        [DataMember]
        public double SHR_OTV_TUTAR { get; set; }

        [DataMember]
        public double SHR_INDIRIM_ORAN { get; set; }

        [DataMember]
        public double SHR_INDIRIM_TUTAR { get; set; }

        [DataMember]
        public string SHR_KDV_DH { get; set; }

        [DataMember]
        public double SHR_ARA_TOPLAM { get; set; }

        [DataMember]
        public double SHR_TOPLAM { get; set; }

        [DataMember]
        public string SHR_HRK_ACIKLAMA { get; set; }

        [DataMember]
        public string SHR_ACIKLAMA { get; set; }

        [DataMember]
        public string SHR_GC { get; set; }

        [DataMember]
        public int SHR_REF_ID { get; set; }

        [DataMember]
        public string SHR_REF_GRUP { get; set; }

        [DataMember]
        public int SHR_PARABIRIMI_ID { get; set; }

        [DataMember]
        public double SHR_PARABIRIMI_KUR { get; set; }

        [DataMember]
        public int SHR_OLUSTURAN_ID { get; set; }

        [DataMember]
        public DateTime? SHR_OLUSTURMA_TARIH { get; set; }

        [DataMember]
        public int SHR_DEGISTIREN_ID { get; set; }

        [DataMember]
        public DateTime? SHR_DEGISTIRME_TARIH { get; set; }

        [DataMember]
        public int SHR_CARI_ID { get; set; }

        [DataMember]
        public DateTime? SHR_TESLIM_TARIHI { get; set; }

        [DataMember]
        public int SHR_TALEP_EDEN_PERSONEL_ID { get; set; }

        [DataMember]
        public int SHR_ALTERNATIF_STOK_ID { get; set; }

        [DataMember]
        public int SHR_MAKINE_ID { get; set; }

        [DataMember]
        public int SHR_AKTARIM_LOG_ID { get; set; }

        [DataMember]
        public string SHR_DEPO_KOD { get; set; }

        [DataMember]
        public string SHR_DEPO_TANIM { get; set; }

        [DataMember]
        public string SHR_BIRIM { get; set; }

        [DataMember]
        public string SHR_STOK_KOD { get; set; }

        [DataMember]
        public string SHR_STOK_TANIM { get; set; }

        [DataMember]
        public string SHR_STOK_FIS_NO { get; set; }

        [DataMember]
        public string SHR_ACIKLAMALAR { get; set; }

        [DataMember]
        public string SHR_PARABIRIMI { get; set; }

        [DataMember]
        public string SHR_OLUSTURAN { get; set; }

        [DataMember]
        public string SHR_DEGISTIREN { get; set; }

        [DataMember]
        public string SHR_FIRMA_KOD { get; set; }

        [DataMember]
        public string SHR_FIRMA { get; set; }

        [DataMember]
        public string ISM_ISEMRI_NO { get; set; }

        [DataMember]
        public string LOK_TANIM { get; set; }

        [DataMember]
        public string FIS_TIP { get; set; }

        [DataMember]
        public string TIP { get; set; }


    }
}