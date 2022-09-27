using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class Stok
    {
        [DataMember]
        public int TB_STOK_ID { get; set; }
        [DataMember]
        public int STK_BIRIM_KOD_ID { get; set; }
        [DataMember]
        public int STK_GRUP_KOD_ID { get; set; }
        [DataMember]
        public int STK_MARKA_KOD_ID { get; set; }
        [DataMember]
        public int STK_MODEL_KOD_ID { get; set; }
        [DataMember]
        public int STK_TIP_KOD_ID { get; set; }
        [DataMember]
        public int STK_DEPO_LOKASYON_ID { get; set; }
        [DataMember]
        public string STK_KOD { get; set; }
        [DataMember]
        public string STK_TANIM { get; set; }
        [DataMember]
        public double STK_MIN_MIKTAR { get; set; }
        [DataMember]
        public double STK_MAX_MIKTAR { get; set; }
        [DataMember]
        public double STK_GIREN_MIKTAR { get; set; }
        [DataMember]
        public double STK_CIKAN_MIKTAR { get; set; }
        [DataMember]
        public double STK_MIKTAR { get; set; }
        [DataMember]
        public double STK_REZERV_MIKTAR { get; set; }
        [DataMember]
        public double STK_KULLANILABILIR_MIKTAR { get; set; }
        [DataMember]
        public bool STK_AKTIF { get; set; }
        [DataMember]
        public string STK_ACIKLAMA { get; set; }
        [DataMember]
        public byte STK_KDV_ORAN { get; set; }
        [DataMember]
        public byte STK_OTV_ORAN { get; set; }
        [DataMember]
        public string STK_S_TIP { get; set; }
        [DataMember]
        public bool STK_ENVANTERDE_GOSTER { get; set; }
        [DataMember]
        public bool STK_SERINO_TAKIP { get; set; }
        [DataMember]
        public string STK_CIKIS_FIYAT_SEKLI { get; set; }
        [DataMember]
        public double STK_CIKIS_FIYAT_DEGERI { get; set; }
        [DataMember]
        public string STK_GIRIS_FIYAT_SEKLI { get; set; }
        [DataMember]
        public double STK_GIRIS_FIYAT_DEGERI { get; set; }
        [DataMember]
        public int STK_OLUSTURAN_ID { get; set; }
        [DataMember]
        public DateTime? STK_OLUSTURMA_TARIH { get; set; }
        [DataMember]
        public int STK_DEGISTIREN_ID { get; set; }
        [DataMember]
        public DateTime? STK_DEGISTIRME_TARIH { get; set; }
        [DataMember]
        public bool STK_YEDEK_PARCA { get; set; }
        [DataMember]
        public bool STK_SARF_MALZEME { get; set; }
        [DataMember]
        public bool STK_STOKSUZ_MALZEME { get; set; }
        [DataMember]
        public bool STK_KRITIK_MALZEME { get; set; }
        [DataMember]
        public bool STK_TEHLIKELI_MALZEME { get; set; }
        [DataMember]
        public int STK_TEHLIKE_SINIF_KOD_ID { get; set; }
        [DataMember]
        public int STK_GARANTI_SURE { get; set; }
        [DataMember]
        public int STK_RAF_OMRU { get; set; }
        [DataMember]
        public int STK_DEPO_ID { get; set; }
        [DataMember]
        public int STK_LOKASYON_ID { get; set; }
        [DataMember]
        public int STK_GARANTI_SURE_BIRIM_ID { get; set; }
        [DataMember]
        public int STK_RAF_OMRU_BIRIM_ID { get; set; }
        [DataMember]
        public int STK_ATOLYE_ID { get; set; }
        [DataMember]
        public int STK_MODUL_NO { get; set; }
        [DataMember]
        public int STK_SINIF_ID { get; set; }
        [DataMember]
        public string STK_KDV_DH { get; set; }
        [DataMember]
        public string STK_URETICI_KOD { get; set; }
        [DataMember]
        public string STK_OZEL_ALAN_1 { get; set; }
        [DataMember]
        public string STK_OZEL_ALAN_2 { get; set; }
        [DataMember]
        public string STK_OZEL_ALAN_3 { get; set; }
        [DataMember]
        public string STK_OZEL_ALAN_4 { get; set; }
        [DataMember]
        public string STK_OZEL_ALAN_5 { get; set; }
        [DataMember]
        public string STK_OZEL_ALAN_6 { get; set; }
        [DataMember]
        public string STK_OZEL_ALAN_7 { get; set; }
        [DataMember]
        public string STK_OZEL_ALAN_8 { get; set; }
        [DataMember]
        public string STK_OZEL_ALAN_9 { get; set; }
        [DataMember]
        public string STK_OZEL_ALAN_10 { get; set; }
        [DataMember]
        public int STK_OZEL_ALAN_11_KOD_ID { get; set; }
        [DataMember]
        public int STK_OZEL_ALAN_12_KOD_ID { get; set; }
        [DataMember]
        public int STK_OZEL_ALAN_13_KOD_ID { get; set; }
        [DataMember]
        public int STK_OZEL_ALAN_14_KOD_ID { get; set; }
        [DataMember]
        public int STK_OZEL_ALAN_15_KOD_ID { get; set; }
        [DataMember]
        public string STK_OZEL_ALAN_11 { get; set; }
        [DataMember]
        public string STK_OZEL_ALAN_12 { get; set; }
        [DataMember]
        public string STK_OZEL_ALAN_13 { get; set; }
        [DataMember]
        public string STK_OZEL_ALAN_14 { get; set; }
        [DataMember]
        public string STK_OZEL_ALAN_15 { get; set; }
        [DataMember]
        public double STK_OZEL_ALAN_16 { get; set; }
        [DataMember]
        public double STK_OZEL_ALAN_17 { get; set; }
        [DataMember]
        public double STK_OZEL_ALAN_18 { get; set; }
        [DataMember]
        public double STK_OZEL_ALAN_19 { get; set; }
        [DataMember]
        public double STK_OZEL_ALAN_20 { get; set; }
        [DataMember]
        public int STK_GELIR_ID { get; set; }
        [DataMember]
        public int STK_GIDER_ID { get; set; }
        [DataMember]
        public int STK_AKTARIM_LOG_ID { get; set; }
        [DataMember]
        public bool STK_YAG { get; set; }
        [DataMember]
        public string STK_RUSCA_KOD { get; set; }
        [DataMember]
        public string STK_RUSCA_TANIM { get; set; }
        [DataMember]
        public bool STK_FILTRE { get; set; }
        [DataMember]
        public int STK_SIPARIS_MIKTARI { get; set; }
        [DataMember]
        public int STK_MASRAF_MERKEZI_ID { get; set; }
        [DataMember]
        public bool STK_FIFO_UYGULA { get; set; }
        [DataMember]
        public int STK_STOK_DURUM { get; set; }
        [DataMember]
        public double STK_KRITIK_STOK_MIKTAR { get; set; }
        [DataMember]
        public int STK_BELGE { get; set; }
        [DataMember]
        public int STK_RESIM { get; set; }
        [DataMember]
        public string STK_BIRIM { get; set; }
        [DataMember]
        public string STK_GRUP { get; set; }
        [DataMember]
        public string STK_MARKA { get; set; }
        [DataMember]
        public string STK_MODEL { get; set; }
        [DataMember]
        public string STK_TIP { get; set; }
        [DataMember]
        public string STK_SINIF { get; set; }
        [DataMember]
        public string STK_DEPO_LOKASYON { get; set; }
        [DataMember]
        public string STK_TEHLIKE_SINIF { get; set; }
        [DataMember]
        public string STK_GARANTI_SURE_BIRIM { get; set; }
        [DataMember]
        public string STK_RAF_OMRU_BIRIM { get; set; }
        [DataMember]
        public string STK_DEPO { get; set; }
        [DataMember]
        public string STK_LOKASYON { get; set; }
        [DataMember]
        public string STK_ATOLYE { get; set; }
        [DataMember]
        public string STK_GIRIS_FIYAT_SEKLI_YAZI { get; set; }
        [DataMember]
        public string STK_CIKIS_FIYAT_SEKLI_YAZI { get; set; }
        [DataMember]
        public string STK_GELIR { get; set; }
        [DataMember]
        public string STK_GIDER { get; set; }
        [DataMember]
        public int STK_FATURA_SAYI { get; set; }

        [DataMember]
        public int RSM_VAR_ID { get; set; }

        [DataMember]
        public string RSM_IDS { get; set; }

        [DataMember]
        public List<int> ResimIDleri {
            get
            {
                List<int> resimIdler = new List<int>();
                if (!String.IsNullOrWhiteSpace(RSM_IDS))
                {
                    string[] ids = RSM_IDS.Split(';');
                    for (int j = 0; j < ids.Length; j++)
                    {
                        resimIdler.Add(Convert.ToInt32(ids[j]));
                    }
                }

                return resimIdler;
            }
        }

        [DataMember]
        public int ResimVarsayilanID => RSM_VAR_ID;
    }
}