using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class DepoStok
    {
        [DataMember]
        public int TB_DEPO_STOK_ID { get; set; }

        [DataMember]
        public int DPS_DEPO_ID { get; set; }

        [DataMember]
        public int DPS_STOK_ID { get; set; }

        [DataMember]
        public double DPS_GIREN_MIKTAR { get; set; }

        [DataMember]
        public double DPS_CIKAN_MIKTAR { get; set; }

        [DataMember]
        public double DPS_MIKTAR { get; set; }

        [DataMember]
        public double DPS_REZERV_MIKTAR { get; set; }

        [DataMember]
        public double DPS_KULLANILABILIR_MIKTAR { get; set; }

        [DataMember]
        public string DPS_DEPO { get; set; }

        [DataMember]
        public string DPS_STOK { get; set; }

        [DataMember]
        public string DPS_STOK_KOD { get; set; }

        [DataMember]
        public int DPS_MALZEME_TIP_ID { get; set; }

        [DataMember]
        public string DPS_MALZEME_TIP { get; set; }

        [DataMember]
        public int DPS_BIRIM_ID { get; set; }

        [DataMember]
        public double DPS_FIYAT { get; set; }

        [DataMember]
        public int DPS_OLUSTURAN_ID { get; set; }

        [DataMember]
        public DateTime? DPS_OLUSTURMA_TARIH { get; set; }

        [DataMember]
        public int DPS_DEGISTIREN_ID { get; set; }

        [DataMember]
        public DateTime? DPS_DEGISTIRME_TARIH { get; set; }

        [DataMember]
        public string DPS_STOK_BIRIM { get; set; }

        [DataMember]
        public string DPS_STOK_SINIF { get; set; }

    }

    // For Web App
	public class DepoStokWebApp
	{
		[DataMember]
		public int TB_DEPO_STOK_ID { get; set; }

        [DataMember]
        public int TB_STOK_ID { get; set; }

        [DataMember]
        public string STK_KOD { get; set; }

        [DataMember]
        public string STK_TANIM { get; set; }

        [DataMember]
        public string STK_TIP { get; set; }

		[DataMember]
		public string STK_BIRIM { get; set; }

		[DataMember]
		public string STK_DEPO { get; set; }

		[DataMember]
		public string STK_GRUP { get; set; }

		[DataMember]
		public string STK_LOKASYON { get; set; }

		[DataMember]
		public int STK_BIRIM_KOD_ID { get; set; }

		[DataMember]
		public int STK_MARKA_KOD_ID { get; set; }

		[DataMember]
		public int STK_MODEL_KOD_ID { get; set; }

		[DataMember]
		public int STK_ATOLYE_ID { get; set; }

		[DataMember]
		public string STK_ATOLYE { get; set; }

		[DataMember]
		public string STK_MARKA { get; set; }

		[DataMember]
		public string STK_MODEL { get; set; }

		[DataMember]
		public string STK_SINIF { get; set; }

        [DataMember]
        public int STK_GIRIS_FIYAT_DEGERI { get; set; }

        [DataMember]
        public int STK_MALIYET { get; set; }

        [DataMember]
        public bool STK_STOKSUZ_MALZEME { get; set; }

        [DataMember]
        public string STK_BARKOD_NO { get; set; }



	}
}