using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class Lokasyon
    {
        public Lokasyon()
        {
            LOK_TANIM = "";
            LOK_TUM_YOL = "";
            LOK_RENK = "";
            LOK_EMAIL = "";
            LOK_ACIKLAMA = "";
        }

        [DataMember]
        public int TB_LOKASYON_ID { get; set; }

        [DataMember]
        public int LOK_ANA_LOKASYON_ID { get; set; }

        [DataMember]
        public string LOK_TANIM { get; set; }

        [DataMember]
        public string LOK_TUM_YOL { get; set; }

        [DataMember]
        public string LOK_RENK { get; set; }

        [DataMember]
        public string LOK_EMAIL { get; set; }

        [DataMember]
        public string LOK_ACIKLAMA { get; set; }

        [DataMember]
        public int LOK_CARI_ID { get; set; }

        [DataMember]
        public int LOK_OLUSTURAN_ID { get; set; }

        [DataMember]
        public DateTime? LOK_OLUSTURMA_TARIH { get; set; }

        [DataMember]
        public int LOK_DEGISTIREN_ID { get; set; }

        [DataMember]
        public DateTime? LOK_DEGISTIRME_TARIH { get; set; }

        [DataMember]
        public int LOK_PERSONEL_ID { get; set; }

        [DataMember]
        public int LOK_KATEGORI_KOD_ID { get; set; }

        [DataMember]
        public int LOK_MASRAF_MERKEZ_KOD_ID { get; set; }

        [DataMember]
        public bool? LOK_AKTIF { get; set; }

        [DataMember]
        public int LOK_BINA_KOD_ID { get; set; }

        [DataMember]
        public int LOK_KAT_KOD_ID { get; set; }

        [DataMember]
        public int LOK_MALZEME_DEPO_ID { get; set; }

        [DataMember]
        public int LOK_TIP_ID { get; set; }

        [DataMember]
        public bool LOK_HAS_NEXT { get ; set; }
    }

    public class LokasyonWebAppModel
    {
		[DataMember]
		public int TB_LOKASYON_ID { get; set; }

		[DataMember]
		public int LOK_ANA_LOKASYON_ID { get; set; }

		[DataMember]
		public string LOK_TANIM { get; set; }

		[DataMember]
		public string LOK_TUM_YOL { get; set; }

		[DataMember]
		public string LOK_RENK { get; set; }

		[DataMember]
		public string LOK_EMAIL { get; set; }

		[DataMember]
		public string LOK_ACIKLAMA { get; set; }

		[DataMember]
		public int LOK_CARI_ID { get; set; }

		[DataMember]
		public int LOK_OLUSTURAN_ID { get; set; }

		[DataMember]
		public DateTime? LOK_OLUSTURMA_TARIH { get; set; }

		[DataMember]
		public int LOK_DEGISTIREN_ID { get; set; }

		[DataMember]
		public DateTime? LOK_DEGISTIRME_TARIH { get; set; }

		[DataMember]
		public int LOK_PERSONEL_ID { get; set; }

		[DataMember]
		public int LOK_KATEGORI_KOD_ID { get; set; }

		[DataMember]
		public int LOK_MASRAF_MERKEZ_KOD_ID { get; set; }

		[DataMember]
		public bool? LOK_AKTIF { get; set; }

		[DataMember]
		public int LOK_BINA_KOD_ID { get; set; }

		[DataMember]
		public int LOK_KAT_KOD_ID { get; set; }

		[DataMember]
		public int LOK_MALZEME_DEPO_ID { get; set; }

		[DataMember]
		public int LOK_TIP_ID { get; set; }

        [DataMember]
		public string LOK_MASRAF_MERKEZ { get; set; }

		[DataMember]
		public string LOK_PERSONEL { get; set; }

		[DataMember]
		public string LOK_KAT { get; set; }

		[DataMember]
		public string LOK_BINA { get; set; }

		[DataMember]
		public string ANA_LOK_TANIM { get; set; }

		[DataMember]
		public string LOK_TIP { get; set; }

		[DataMember]
		public string LOK_DEPO { get; set; }


	}
}