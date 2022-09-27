using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiNew.Models
{
    public class SantiyeCalismaAyar
    {
        public int TB_SANTIYE_CALISMA_AYAR_ID { get; set; }
        public bool SCA_ZORUNLU_TARIH { get; set; }
        public bool SCA_ZORUNLU_PROJE { get; set; }
        public bool SCA_ZORUNLU_SANTIYE { get; set; }
        public bool SCA_ZORUNLU_MAKINE_PERSONEL { get; set; }
        public bool SCA_ZORUNLU_CALISMA_TIPI { get; set; }
        public bool SCA_ZORUNLU_SURE { get; set; }
        public bool SCA_ZORUNLU_BIRIM_UCRET { get; set; }
        public bool SCA_ZORUNLU_TUTAR { get; set; }
        public bool SCA_ZORUNLU_ACIKLAMA { get; set; }
        public bool SCA_ZORUNLU_OZEL_ALAN_1 { get; set; }
        public bool SCA_ZORUNLU_OZEL_ALAN_2 { get; set; }
        public bool SCA_ZORUNLU_OZLE_ALAN_3 { get; set; }
        public bool SCA_ZORUNLU_OZEL_ALAN_4 { get; set; }
        public bool SCA_ZORUNLU_OZEL_ALAN_5 { get; set; }
        public bool SCA_ZORUNLU_OZEL_ALAN_6 { get; set; }
        public bool SCA_ZORUNLU_OZEL_ALAN_7 { get; set; }
        public bool SCA_ZORUNLU_OZEL_ALAN_8 { get; set; }
        public bool SCA_ZORUNLU_OZEL_ALAN_9 { get; set; }
        public bool SCA_ZORUNLU_OZEL_ALAN_10 { get; set; }
        public string SCA_FORMAT_SAAT { get; set; }
        public string SCA_FORMAT_GUN { get; set; }
        public string SCA_FORMAT_TUTAR { get; set; }
        public bool SCA_ZORUNLU_CALISMAMA_NEDEN { get; set; }
        public bool SCA_ZORUNLU_CALISMAMA_SURE { get; set; }
        public bool SCA_ZORUNLU_MESAI_SURE { get; set; }
        public bool SCA_ZORUNLU_MESAI_UCRET { get; set; }
        public bool SCA_ZORUNLU_PROJE_IS_TANIM { get; set; }
        public bool SCA_ZORUNLU_IS_CINSI { get; set; }
        public bool SCA_ZORUNLU_MIKTAR { get; set; }
        public bool SCA_ZORUNLU_BASLAMA_TARIH { get; set; }
        public bool SCA_ZORUNLU_BITIS_TARIH { get; set; }
        public bool SCA_ZORUNLU_MAKINE_SURE { get; set; }
        public string SCA_ACIKLAMA { get; set; }
        public bool SCA_AKTIF { get; set; }
        public int SCA_OLUSTURAN_ID { get; set; }
        public DateTime? SCA_OLUSTURMA_TARIH { get; set; }
        public int SCA_DEGISTIREN_ID { get; set; }
        public DateTime? SCA_DEGISTIRME_TARIH { get; set; }
        public bool SCA_ZORUNLU_VARDIYA { get; set; }
        public bool SCA_ZORUNLU_CALISMA_YERI { get; set; }
        public bool SCA_ZORUNLU_MAKINE { get; set; }
        public bool SCA_ZORUNLU_LOKASYON { get; set; }
        public bool SCA_PUANTAJ_BILGISI { get; set; }
        public bool SCA_ZORUNLU_SAYAC_BILGISI { get; set; }
        public string SCA_SAYAC_BILGISI_GOSTER { get; set; }
        public bool SCA_ZORUNLU_CALISMA_UCRETI { get; set; }
        public bool SCA_ISBILGILERI_GOSTER { get; set; }
        public bool SCA_OZELALANLARI_GOSTER { get; set; }
        public bool SCA_ACIKLAMA_GOSTER { get; set; }
        public bool SCA_YAKITISLEMLERI_GOSTER { get; set; }
        public bool SCA_VARSAYILAN { get; set; }
        public int SCA_ZORUNLU_CALISMA_KODU { get; set; }
        public int SCA_DURUSISLEMLERI_GOSTER { get; set; }
        public bool SCA_ZORUNLU_CALISMA_BELGE_NO { get; set; }
        public bool SCA_ZORUNLU_CALISMA_DURUM { get; set; }
        public bool SCA_ZORUNLU_CALISMA_FIRMA { get; set; }
        public bool SCA_PROJEBILGISI_GOSTER { get; set; }
        public bool SCA_ZORUNLU_CALISMA_IS_TANIMI { get; set; }
        public bool SCA_ZORUNLU_CALISMA_HESAPLAMA_TURU { get; set; }
        public bool SCA_ZORUNLU_CALISMA_SEFER_SAYISI { get; set; }
        public bool SCA_ZORUNLU_CALISMA_BIRIM_UCRETI { get; set; }
        public bool SCA_ZORUNLU_PROJE_IS_MIKTARI { get; set; }
        public bool SCA_ZORUNLU_PROJE_BIRIM_UCRETI { get; set; }
        public bool SCA_ZORUNLU_PROJE_TOPLAM_TUTAR { get; set; }
        public bool SCA_HARCAMABILGISI_GOSTER { get; set; }
        public bool SCA_MALZEMEBILGISI_GOSTER { get; set; }
        public bool SCA_OPERATORBILGISI_GOSTER { get; set; }
        public bool SCA_VARSAYILAN_BASLAMA { get; set; }
        public bool SCA_VARSAYILAN_BITIS { get; set; }
        public double SCA_VARSAYILAN_IS_MIKTAR { get; set; }
        public int SCA_VARSAYILAN_IS_BIRIM_KOD_ID { get; set; }
        public bool SCA_VARSAYILAN_IS_KONTROL { get; set; }
        public bool SCA_VARSAYILAN_HESAP_TURU { get; set; }
        public bool SCA_VARSAYILAN_HESAP_TURU_KONTROL { get; set; }
    }
}