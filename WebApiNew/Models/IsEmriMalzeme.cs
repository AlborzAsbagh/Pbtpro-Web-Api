using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class IsEmriMalzeme
    {
        [DataMember]
        public int TB_ISEMRI_MLZ_ID { get; set; }

        [DataMember]
        public int IDM_ISEMRI_ID { get; set; }

        [DataMember]
        public DateTime? IDM_TARIH { get; set; }

        [DataMember]
        public string IDM_SAAT { get; set; }

        [DataMember]
        public int IDM_STOK_ID { get; set; }

        [DataMember]
        public int IDM_DEPO_ID { get; set; }

        [DataMember]
        public int IDM_BIRIM_KOD_ID { get; set; }

        [DataMember]
        public int IDM_STOK_TIP_KOD_ID { get; set; }

        [DataMember]
        public bool IDM_STOK_DUS { get; set; }

        [DataMember]
        public string IDM_STOK_TANIM { get; set; }

        [DataMember]
        public double IDM_BIRIM_FIYAT { get; set; }

        [DataMember]
        public double IDM_MIKTAR { get; set; }

        [DataMember]
        public double IDM_TUTAR { get; set; }

        [DataMember]
        public int IDM_REF_ID { get; set; }

        [DataMember]
        public int IDM_STOK_KULLANIM_SEKLI { get; set; }

        [DataMember]
        public string IDM_MALZEME_STOKTAN { get; set; }

        [DataMember]
        public int IDM_ALTERNATIF_STOK_ID { get; set; }

        [DataMember]
        public int IDM_MARKA_KOD_ID { get; set; }

        [DataMember]
        public string IDM_BIRIM { get; set; }

        [DataMember]
        public string IDM_STOK_KOD { get; set; }

        [DataMember]
        public string IDM_DEPO { get; set; }

        [DataMember]
        public int IDM_OLUSTURAN_ID { get; set; }

        [DataMember]
        public DateTime? IDM_OLUSTURMA_TARIH { get; set; }

        [DataMember]
        public int IDM_DEGISTIREN_ID { get; set; }

        [DataMember]
        public DateTime? IDM_DEGISTIRME_TARIH { get; set; }

        [DataMember]
        public Boolean IDM_SILINDI { get; set; }

        [DataMember]
        public string IDM_ISEMRI_NO { get; set; }

    }
}