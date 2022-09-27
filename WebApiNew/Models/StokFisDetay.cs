using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    [DataContract]
    public class StokFisDetay
    {
        [DataMember]
        public int TB_STOK_FIS_DETAY_ID { get; set; }

        [DataMember]
        public int SFD_STOK_FIS_ID { get; set; }

        [DataMember]
        public string SFD_SATIR_TIP { get; set; }

        [DataMember]
        public int SFD_STOK_ID { get; set; }

        [DataMember]
        public int SFD_BIRIM_KOD_ID { get; set; }

        [DataMember]
        public double SFD_MIKTAR { get; set; }

        [DataMember]
        public double SFD_ANA_BIRIM_MIKTAR { get; set; }

        [DataMember]
        public double SFD_BIRIM_FIYAT { get; set; }

        [DataMember]
        public int SFD_FIS_GRID_KONUM { get; set; }

        [DataMember]
        public byte SFD_KDV_ORAN { get; set; }

        [DataMember]
        public double SFD_KDV_TUTAR { get; set; }

        [DataMember]
        public byte SFD_OTV_ORAN { get; set; }

        [DataMember]
        public double SFD_OTV_TUTAR { get; set; }

        [DataMember]
        public double SFD_INDIRIM_ORAN { get; set; }

        [DataMember]
        public double SFD_INDIRIM_TUTAR { get; set; }

        [DataMember]
        public string SFD_KDV_DH { get; set; }

        [DataMember]
        public double SFD_ARA_TOPLAM { get; set; }

        [DataMember]
        public double SFD_TOPLAM { get; set; }

        [DataMember]
        public double SFD_ALINAN_MIKTAR { get; set; }

        [DataMember]
        public double SFD_ALINAN_ANA_BIRIM_MIKTAR { get; set; }

        [DataMember]
        public int SFD_OLUSTURAN_ID { get; set; }

        [DataMember]
        public DateTime? SFD_OLUSTURMA_TARIH { get; set; }

        [DataMember]
        public int SFD_DEGISTIREN_ID { get; set; }

        [DataMember]
        public DateTime? SFD_DEGISTIRME_TARIH { get; set; }

        [DataMember]
        public int SFD_CARI_ID { get; set; }

        [DataMember]
        public DateTime? SFD_TESLIM_TARIHI { get; set; }

        [DataMember]
        public int SFD_TALEP_EDEN_PERSONEL_ID { get; set; }

        [DataMember]
        public int SFD_ALTERNATIF_STOK_ID { get; set; }

        [DataMember]
        public int SFD_MAKINE_ID { get; set; }

        [DataMember]
        public string SFD_KARSILAMA_SEKLI { get; set; }

        [DataMember]
        public double SFD_IPTAL_MIKTARI { get; set; }

        [DataMember]
        public double SFD_STOKTAN_KULLANIM { get; set; }

        [DataMember]
        public double SFD_SATINALMA_MIKTARI { get; set; }

        [DataMember]
        public double SFD_GIREN_MIKTAR { get; set; }

        [DataMember]
        public double SFD_KALAN_MIKTAR { get; set; }

        [DataMember]
        public string SFD_ACIKLAMA { get; set; }

        [DataMember]
        public int SFD_DURUM { get; set; }

        [DataMember]
        public int SFD_SINIF_ID { get; set; }

        [DataMember]
        public int SFD_PROJE_ID { get; set; }

        [DataMember]
        public DateTime? SFD_TALEP_TARIHI { get; set; }

        [DataMember]
        public DateTime? SFD_KAPAMA_TARIHI { get; set; }

        [DataMember]
        public DateTime? SFD_IPTAL_TARIHI { get; set; }

        [DataMember]
        public int SFD_TALEP_ID { get; set; }

        [DataMember]
        public int SFD_HAKEDIS_ID { get; set; }

        [DataMember]
        public string SFD_STOK_KOD { get; set; }

        [DataMember]
        public string SFD_STOK { get; set; }

        [DataMember]
        public string SFD_TALEP_EDEN_PERSONEL { get; set; }

        [DataMember]
        public string SFD_CARI { get; set; }

        [DataMember]
        public string SFD_DURUM_YAZI { get; set; }

        [DataMember]
        public string SFD_MAKINE_KOD { get; set; }

        [DataMember]
        public string SFD_MAKINE_TANIM { get; set; }

        [DataMember]
        public string SFD_BIRIM { get; set; }

    }
}