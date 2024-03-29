using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiNew.Models
{
    public class Dosya
    {
        public int TB_DOSYA_ID { get; set; }
        public string DSY_TANIM { get; set; }
        public string DSY_DOSYA_TIP { get; set; }
        public int DSY_DOSYA_TIP_ID { get; set; }
        public bool DSY_AKTIF { get; set; }
        public bool DSY_SURELI { get; set; }
        public DateTime? DSY_BITIS_TARIH { get; set; }
        public string DSY_ACIKLAMA { get; set; }
        public string DSY_DOSYA_AD { get; set; }
        public string DSY_DOSYA_TURU { get; set; }
        public string DSY_DOSYA_UZANTI { get; set; }
        public int DSY_DOSYA_BOYUT { get; set; }
        public DateTime? DSY_DOSYA_OLUSTURMA_TARIH { get; set; }
        public DateTime? DSY_DOSYA_DEGISTIRME_TARIH { get; set; }
        public string DSY_DOSYA_YOL { get; set; }
        public string DSY_ARSIV_AD { get; set; }
        public bool DSY_SIFRELI { get; set; }
        public string DSY_SIFRE { get; set; }
        public int DSY_OLUSTURAN_ID { get; set; }
        public DateTime? DSY_OLUSTURMA_TARIH { get; set; }
        public int DSY_DEGISTIREN_ID { get; set; }
        public DateTime? DSY_DEGISTIRME_TARIH { get; set; }
        public int DSY_REF_ID { get; set; }
        public string DSY_REF_GRUP { get; set; }
        public int DSY_YER_ID { get; set; }
        public string DYS_ETIKET { get; set; }
        public bool DSY_HATIRLAT { get; set; }
        public DateTime? DSY_HATIRLAT_TARIH { get; set; }
        public DateTime? DSY_TARIH { get; set; }
    }
}