using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using Dapper.Contrib.Extensions;

namespace WebApiNew.Models
{
    [Table("orjin.TB_MAKINE_LOKASYON")]
    public class MknLokasyonLog
    {
        public static readonly int DURUM_ONAY_BEKLEYEN = 1, DURUM_ONAYLANAN = 2, DURUM_ONAYLANMAYAN = 3; 

        [Key]
        public int TB_MAKINE_LOKASYON_ID { get; set; }
        public DateTime? MKL_TARIH { get; set; }
        public string MKL_SAAT { get; set; }
        public int? MKL_MAKINE_ID { get; set; }
        public int? MKL_KAYNAK_LOKASYON_ID { get; set; }
        public int? MKL_HEDEF_LOKASYON_ID { get; set; }
        public string MKL_ACIKLAMA { get; set; }
        public int? MKL_OLUSTURAN_ID { get; set; }
        public DateTime? MKL_OLUSTURMA_TARIH { get; set; }
        public int? MKL_DEGISTIREN_ID { get; set; }
        public DateTime? MKL_DEGISTIRME_TARIH { get; set; }
        public int MKL_NAKIL_SEKLI_KOD_ID { get; set; }
        public int MKL_PERSONEL_ID { get; set; }
        public int MKL_PROJE_ID { get; set; }
        public int MKL_ZIMMET_PERSONEL_ID { get; set; }
        public int MKL_ONAY_KULLANICI_ID { get; set; }
        public DateTime? MKL_ONAY_TARIH { get; set; }
        public string MKL_ONAY_SAAT { get; set; }
        public string MKL_ONAY_PARAMETRE { get; set; }
        public int MKL_IPTAL_KULLANICI_ID { get; set; }
        public DateTime? MKL_IPTAL_TARIH { get; set; }
        public string MKL_IPTAL_SAAT { get; set; }
        public int MKL_DURUM_ID { get; set; }
        public string MKL_ONAY_ACIKLAMA { get; set; }
        public int MKL_ONAY_MAKINE_DURUM_ID { get; set; }
        public double MKL_ONAY_SAYAC { get; set; }
        public int MKL_ONAY_NEDEN_KOD_ID { get; set; }
        [Write(false)]
        [Computed]
        public string MKL_MAKINE_KODU { get; set; }
        [Write(false)]
        [Computed]
        public string MKL_MAKINE_TANIM { get; set; }
        [Write(false)]
        [Computed]
        public string MKL_NAKIL_SEKLI_KOD { get; set; }
        [Write(false)]
        [Computed]
        public string MKL_SAYAC_BIRIM { get; set; }
        [Write(false)]
        [Computed]
        public string MKL_KAYNAK_LOKASYON { get; set; }
        [Write(false)]
        [Computed]
        public string MKL_HEDEF_LOKASYON { get; set; }
        [Write(false)]
        [Computed]
        public string MKL_PERSONEL_ISIM { get; set; }
        [Write(false)]
        [Computed]
        public string MKL_ZIMMET_PERSONEL { get; set; }
        [Write(false)]
        [Computed]
        public string MKL_PROJE { get; set; }
        [Write(false)]
        [Computed]
        public string MKL_ONAY_KULLANICI { get; set; }
        [Write(false)]
        [Computed]
        public string MKL_ONAY_DURUMU { get; set; }
    }
  
}