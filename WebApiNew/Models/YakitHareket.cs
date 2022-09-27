using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    [DataContract]
    public class YakitHareket
    {
        [DataMember]
        public int TB_YAKIT_HRK_ID { get; set; }

        [DataMember]
        public int YKH_MAKINE_ID { get; set; }

        [DataMember]
        public DateTime? YKH_TARIH { get; set; }

        [DataMember]
        public string YKH_SAAT { get; set; }

        [DataMember]
        public double YKH_SON_KM { get; set; }

        [DataMember]
        public double YKH_ALINAN_KM { get; set; }

        [DataMember]
        public double YKH_FARK_KM { get; set; }

        [DataMember]
        public double YKH_MIKTAR { get; set; }

        [DataMember]
        public double YKH_FIYAT { get; set; }

        [DataMember]
        public double YKH_KDV_TUTAR { get; set; }

        [DataMember]
        public double YKH_TUTAR { get; set; }

        [DataMember]
        public string YKH_FATURA_NO { get; set; }

        [DataMember]
        public DateTime? YKH_FATURA_TARIH { get; set; }

        [DataMember]
        public bool YKH_OZEL_KULLANIM { get; set; }

        [DataMember]
        public bool YKH_STOK_KULLANIM { get; set; }

        [DataMember]
        public int YKH_STOK_ID { get; set; }

        [DataMember]
        public int YKH_DEPO_ID { get; set; }

        [DataMember]
        public int YKH_ISTASYON_KOD_ID { get; set; }

        [DataMember]
        public int YKH_PERSONEL_ID { get; set; }

        [DataMember]
        public string YKH_PERSONEL { get; set; }

        [DataMember]
        public int YKH_GUZERGAH_ID { get; set; }

        [DataMember]
        public int YKH_YAKIT_TIP_ID { get; set; }

        [DataMember]
        public int YKH_FIRMA_ID { get; set; }

        [DataMember]
        public bool YKH_DEPO_FULLENDI { get; set; }

        [DataMember]
        public double YKH_DEPO_YAKIT_MIKTAR { get; set; }

        [DataMember]
        public double YKH_TUKETIM { get; set; }

        [DataMember]
        public double YKH_HARCANAN_YAKIT_MIKTAR { get; set; }

        [DataMember]
        public string YKH_SURUS_BILGI { get; set; }

        [DataMember]
        public double YKH_BEKLENEN_TUKETIM_ORAN { get; set; }

        [DataMember]
        public double YKH_GERCEKLESEN_TUKETIM_ORAN { get; set; }

        [DataMember]
        public double YKH_SAPMA_ORAN { get; set; }

        [DataMember]
        public double YKH_SAPMA_KM { get; set; }

        [DataMember]
        public double YKH_ONGORULEN_LT { get; set; }

        [DataMember]
        public double YKH_SAPMA_LT { get; set; }

        [DataMember]
        public double YKH_ONGORULEN_KM { get; set; }

        [DataMember]
        public double YKH_ONGORULEN_TUTAR { get; set; }

        [DataMember]
        public double YKH_GERCEKLESEN_TUTAR { get; set; }

        [DataMember]
        public double YKH_SAPMA_TUTAR { get; set; }

        [DataMember]
        public int YKH_LOKASYON_ID { get; set; }

        [DataMember]
        public string YKH_ACIKLAMA { get; set; }

        [DataMember]
        public int YKH_OLUSTURAN_ID { get; set; }

        [DataMember]
        public DateTime? YKH_OLUSTURMA_TARIH { get; set; }

        [DataMember]
        public int YKH_DEGISTIREN_ID { get; set; }

        [DataMember]
        public DateTime? YKH_DEGISTIRME_TARIH { get; set; }

        [DataMember]
        public int YKH_SAYAC_ID { get; set; }

        [DataMember]
        public int YKH_SAYAC_BIRIM_ID { get; set; }

        [DataMember]
        public int YKH_MASRAF_MERKEZI_ID { get; set; }

        [DataMember]
        public string YKH_MASRAF_MERKEZI { get; set; }

        [DataMember]
        public int YKH_PROJE_ID { get; set; }

        [DataMember]
        public int YKH_HAKEDIS_ID { get; set; }

        [DataMember]
        public double YKH_INDIRIM_ORAN { get; set; }

        [DataMember]
        public double YKH_INDIRIM_TUTAR { get; set; }

        [DataMember]
        public string YKH_SAYAC { get; set; }

        [DataMember]
        public string YKH_SAYAC_BIRIM { get; set; }

        [DataMember]
        public string YKH_YAKIT { get; set; }

        [DataMember]
        public string YKH_TANK { get; set; }

        [DataMember]
        public string YKH_MAKINE { get; set; }

        [DataMember]
        public string YKH_BIRIM { get; set; }

        [DataMember]
        public string YKH_LOKASYON { get; set; }
        [DataMember]
        public string YKH_PROJE { get; set; }

        [DataMember]
        public int ResimVarsayilanID { get; set; }

        [DataMember]
        public List<int> ResimIDleri { get; set; }

    }

}