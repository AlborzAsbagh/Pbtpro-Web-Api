using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    [DataContract]
    public class StokFis
    {
        [DataMember]
        public int TB_STOK_FIS_ID { get; set; }

        [DataMember]
        public int SFS_GIRIS_DEPO_ID { get; set; }

        [DataMember]
        public int SFS_CIKIS_DEPO_ID { get; set; }

        [DataMember]
        public int SFS_CARI_ID { get; set; }

        [DataMember]
        public int SFS_ISLEM_TIP_KOD_ID { get; set; }

        [DataMember]
        public DateTime? SFS_TARIH { get; set; }

        [DataMember]
        public string SFS_SAAT { get; set; }

        [DataMember]
        public string SFS_FIS_NO { get; set; }

        [DataMember]
        public string SFS_S_TIP { get; set; }

        [DataMember]
        public double SFS_ARA_TOPLAM { get; set; }

        [DataMember]
        public double SFS_INDIRIM_TOPLAM { get; set; }

        [DataMember]
        public double SFS_KDV_TOPLAM { get; set; }

        [DataMember]
        public double SFS_OTV_TOPLAM { get; set; }

        [DataMember]
        public double SFS_YUVARLAMA_TUTAR { get; set; }

        [DataMember]
        public double SFS_GENEL_TOPLAM { get; set; }

        [DataMember]
        public string SFS_ACIKLAMA { get; set; }

        [DataMember]
        public int SFS_REF_ID { get; set; }

        [DataMember]
        public string SFS_REF_GRUP { get; set; }

        [DataMember]
        public string SFS_ISLEM_TIP { get; set; }

        [DataMember]
        public int SFS_PARABIRIMI_ID { get; set; }

        [DataMember]
        public double SFS_PARABIRIMI_KUR { get; set; }

        [DataMember]
        public bool SFS_IPTAL { get; set; }

        [DataMember]
        public bool SFS_KAPALI { get; set; }

        [DataMember]
        public bool SFS_FATURA { get; set; }

        [DataMember]
        public double SFS_INDIRIM_ORAN { get; set; }

        [DataMember]
        public string SFS_GC { get; set; }

        [DataMember]
        public int SFS_OLUSTURAN_ID { get; set; }

        [DataMember]
        public DateTime? SFS_OLUSTURMA_TARIH { get; set; }

        [DataMember]
        public int SFS_DEGISTIREN_ID { get; set; }

        [DataMember]
        public DateTime? SFS_DEGISTIRME_TARIH { get; set; }

        [DataMember]
        public int SFS_TALEP_EDEN_PERSONEL_ID { get; set; }

        [DataMember]
        public DateTime? SFS_TALEP_TARIH { get; set; }

        [DataMember]
        public int SFS_TALEP_NEDEN_KOD_ID { get; set; }

        [DataMember]
        public string SFS_TALEP_ISEMRI_NO { get; set; }

        [DataMember]
        public int SFS_TALEP_ONCELIK_ID { get; set; }

        [DataMember]
        public DateTime? SFS_TALEP_TESLIM_TARIH { get; set; }

        [DataMember]
        public int SFS_TALEP_ONAY_PERSONEL_ID { get; set; }

        [DataMember]
        public string SFS_TALEP_FIS_NO { get; set; }

        [DataMember]
        public string SFS_FATURA_IRSALIYE_NO { get; set; }

        [DataMember]
        public string SFS_TESLIM_ALAN { get; set; }

        [DataMember]
        public double SFS_NAKLIYE_UCRETI { get; set; }

        [DataMember]
        public int SFS_MODUL_NO { get; set; }

        [DataMember]
        public int SFS_PROJE_ID { get; set; }

        [DataMember]
        public string SFS_REFERANS { get; set; }

        [DataMember]
        public double SFS_TOPLAM_MIKTAR { get; set; }

        [DataMember]
        public double SFS_TOPLAM_BIRIM_FIYAT { get; set; }

        [DataMember]
        public int SFS_TALEP_DURUM_ID { get; set; }

        [DataMember]
        public bool SFS_OKUNDU { get; set; }

        [DataMember]
        public DateTime SFS_KAPAMA_ZAMANI { get; set; }

        [DataMember]
        public int SFS_TALEP_ONCELIK { get; set; }

        [DataMember]
        public string SFS_TALEP_SIPARIS_NO { get; set; }

        [DataMember]
        public int SFS_MAKINE_ID { get; set; }

        [DataMember]
        public int SFS_LOKASYON_ID { get; set; }

        [DataMember]
        public string SFS_TEKLIF_SAATI { get; set; }

        [DataMember]
        public double SFS_OZEL_ALAN11 { get; set; }

        [DataMember]
        public double SFS_OZEL_ALAN12 { get; set; }

        [DataMember]
        public double SFS_OZEL_ALAN13 { get; set; }

        [DataMember]
        public int SFS_BOLUM_KOD_ID { get; set; }

        [DataMember]
        public int SFS_TESLIM_YERI_KOD_ID { get; set; }

        [DataMember]
        public string SFS_BASLIK { get; set; }

        [DataMember]
        public string SFS_TESLIM_YERI_TANIM { get; set; }

        [DataMember]
        public int SFS_TASERON_FIRMA_ID { get; set; }

        [DataMember]
        public int SFS_TALEP_EDILEN_KISI_ID { get; set; }

        [DataMember]
        public int SFS_BELGE { get; set; }

        [DataMember]
        public int SFS_RESIM { get; set; }

        [DataMember]
        public string SFS_GIRIS_DEPO { get; set; }

        [DataMember]
        public string SFS_CIKIS_DEPO { get; set; }

        [DataMember]
        public string SFS_ISLEM_TIP_DEGER { get; set; }

        [DataMember]
        public string SFS_CARI { get; set; }

        [DataMember]
        public string SFS_TALEP_NEDEN { get; set; }

        [DataMember]
        public string SFS_ONCELIK { get; set; }

        [DataMember]
        public string SFS_TALEP_EDEN { get; set; }

        [DataMember]
        public string SFS_ONAY_PERSONEL { get; set; }

        [DataMember]
        public string SFS_PROFJE_TANIM { get; set; }

        [DataMember]
        public string SFS_BOLUM { get; set; }

        [DataMember]
        public string SFS_TESLIM_YERI { get; set; }

        [DataMember]
        public string SIPARIS_NO { get; set; }

        [DataMember]
        public string SFS_SATINALMA_TALEP_EDEN { get; set; }

        [DataMember]
        public string SFS_TALEP_EDILEN { get; set; }

        [DataMember]
        public string SFS_MAKINE_KOD { get; set; }

        [DataMember]
        public string SFS_MAKINE { get; set; }

        [DataMember]
        public string SFS_DURUM { get; set; }

        [DataMember]
        public int SFS_ALAN_KILITLE { get; set; }

        [DataMember]
        public int SFS_TEKLIF_ICON { get; set; }

        [DataMember]
        public int SFS_SIPARIS_ICON { get; set; }

        [DataMember]
        public string SFS_LOKASYON { get; set; }

        [DataMember]
        public string SFS_KOD_ONCELIK { get; set; }

        [DataMember]
        public Personel PERSONEL { get; set; }
    }
}