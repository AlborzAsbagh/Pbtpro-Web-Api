using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    public class Cari
    {
        public int TB_CARI_ID { get; set; }
        public string CAR_KOD { get; set; }
        public string CAR_TANIM { get; set; }
        public int CAR_SEKTOR_KOD_ID { get; set; }
        public int CAR_BOLGE_KOD_ID { get; set; }
        public string CAR_ADRES { get; set; }
        public string CAR_SEHIR { get; set; }
        public string CAR_ILCE { get; set; }
        public string CAR_POSTA_KOD { get; set; }
        public string CAR_ULKE { get; set; }
        public string CAR_ILGILI { get; set; }
        public string CAR_TEL1 { get; set; }
        public string CAR_TEL2 { get; set; }
        public string CAR_FAKS1 { get; set; }
        public string CAR_FAKS2 { get; set; }
        public string CAR_GSM { get; set; }
        public string CAR_WEB { get; set; }
        public string CAR_EMAIL { get; set; }
        public int CAR_TIP_KOD_ID { get; set; }
        public string CAR_VERGI_DAIRE { get; set; }
        public string CAR_VERGI_NO { get; set; }
        public string CAR_TERMIN_SURE { get; set; }
        public double CAR_KREDI_LIMIT { get; set; }
        public string CAR_MUHASEBE_KOD { get; set; }
        public int CAR_HIZMET_DEGER_KOD_ID { get; set; }
        public string CAR_SARTNAME_NO { get; set; }
        public string CAR_SOZLESME_NO { get; set; }
        public bool CAR_TEDARIKCI { get; set; }
        public bool CAR_MUSTERI { get; set; }
        public bool CAR_NAKLIYECI { get; set; }
        public bool CAR_SERVIS { get; set; }
        public bool CAR_SUBE { get; set; }
        public bool CAR_DIGER { get; set; }
        public double CAR_ISLEM_HACMI { get; set; }
        public string CAR_ACIKLAMA { get; set; }
        public bool CAR_AKTIF { get; set; }
        public int CAR_OLUSTURAN_ID { get; set; }
        public DateTime? CAR_OLUSTURMA_TARIH { get; set; }
        public int CAR_DEGISTIREN_ID { get; set; }
        public DateTime? CAR_DEGISTIRME_TARIH { get; set; }
        public int CAR_LOKASYON_ID { get; set; }
        public bool CAR_TASERON { get; set; }
        public string CAR_OZEL_ALAN_1 { get; set; }
        public string CAR_OZEL_ALAN_2 { get; set; }
        public string CAR_OZEL_ALAN_3 { get; set; }
        public string CAR_OZEL_ALAN_4 { get; set; }
        public string CAR_OZEL_ALAN_5 { get; set; }
        public double CAR_OZEL_ALAN_6 { get; set; }
        public double CAR_OZEL_ALAN_7 { get; set; }
        public double CAR_OZEL_ALAN_8 { get; set; }
        public double CAR_OZEL_ALAN_9 { get; set; }
        public double CAR_OZEL_ALAN_10 { get; set; }
        public int CAR_FINANS_CARI_ID { get; set; }
        public double CAR_INDIRIM_ORAN { get; set; }
        public int CAR_BELGE { get; set; }
        public int CAR_RESIM { get; set; }
        public string CAR_SEKTOR { get; set; }
        public string CAR_BOLGE { get; set; }
        public string CAR_TIP { get; set; }
        public string CAR_HIZMET_DEGER { get; set; }
        public string CAR_LOKASYON { get; set; }
    }

    public class CariSozlesme
    {
		[DataMember]
		public int TB_CARI_SOZLESME_ID { get; set; }

		[DataMember]
		public int CAS_CARI_ID { get; set; }

		[DataMember]
		public string CAS_TANIM { get; set; }

		[DataMember]
		public DateTime CAS_BASLANGIC_TARIH { get; set; }

		[DataMember]
		public DateTime? CAS_BITIS_TARIH { get; set; }

		[DataMember]
		public int CAS_TIP_KOD_ID { get; set; }

		[DataMember]
		public int CAS_MUDAHALE_SURE { get; set; }

		[DataMember]
		public int CAS_ONARIM_SURE { get; set; }

		[DataMember]
		public int CAS_MALZEME_TEMIN_SURE { get; set; }

		[DataMember]
		public bool CAS_PARCA_DAHIL { get; set; }

		[DataMember]
		public bool CAS_AKTIF { get; set; }

		[DataMember]
		public string CAS_SOZLESME_ACIKLAMA { get; set; }

		[DataMember]
		public int CAS_OLUSTURAN_ID { get; set; }

		[DataMember]
		public DateTime CAS_OLUSTURMA_TARIH { get; set; }

		[DataMember]
		public int CAS_DEGISTIREN_ID { get; set; }

		[DataMember]
		public DateTime? CAS_DEGISTIRME_TARIH { get; set; }

		[DataMember]
		public string CAS_SOZLESME_NO { get; set; }

		[DataMember]
		public int CAS_KATEGORI_KOD_ID { get; set; }

		[DataMember]
		public string CAS_REFERANS_NO { get; set; }

		[DataMember]
		public decimal CAS_SOZLESME_BEDELI { get; set; }

		[DataMember]
		public int CAS_LOKASYON_ID { get; set; }

		[DataMember]
		public int CAS_MUDAHALE_SURESI_GUN { get; set; }

		[DataMember]
		public int CAS_MUDAHALE_SURESI_SAAT { get; set; }

		[DataMember]
		public int CAS_MUDAHALE_SURESI_DAKIKA { get; set; }

		[DataMember]
		public int CAS_ONARIM_SURESI_GUN { get; set; }

		[DataMember]
		public int CAS_ONARIM_SURESI_SAAT { get; set; }

		[DataMember]
		public int CAS_ONARIM_SURESI_DAKIKA { get; set; }

		[DataMember]
		public int CAS_PARCA_TEMIN_SURESI_GUN { get; set; }

		[DataMember]
		public int CAS_PARCA_TEMIN_SURESI_SAAT { get; set; }

		[DataMember]
		public int CAS_PARCA_TEMIN_SURESI_DAKIKA { get; set; }

		[DataMember]
		public string CAS_SOZLESME_ICERIGI { get; set; }

		[DataMember]
		public string CAS_OZEL_ALAN_1 { get; set; }

		[DataMember]
		public string CAS_OZEL_ALAN_2 { get; set; }

		[DataMember]
		public string CAS_OZEL_ALAN_3 { get; set; }

		[DataMember]
		public string CAS_OZEL_ALAN_4 { get; set; }

		[DataMember]
		public string CAS_OZEL_ALAN_5 { get; set; }

		[DataMember]
		public int CAS_MAZLEME_TIPI_KOD_ID { get; set; }

		[DataMember]
		public string CAS_IMZALAYAN { get; set; }

		[DataMember]
		public int CAS_ODEME_SEKLI_KOD_ID { get; set; }

		[DataMember]
		public int CAS_ODEME_VADESI { get; set; }

		[DataMember]
		public string CAS_REF_GRUP { get; set; }

		[DataMember]
		public int CAS_TESLIM_SURESI { get; set; }

		[DataMember]
		public int CAS_HATIRLATICI_SURE { get; set; }

		[DataMember]
		public int CAS_REF_ID { get; set; }

		[DataMember]
		public string CAS_OZEL_ALAN_6 { get; set; }

		[DataMember]
		public string CAS_OZEL_ALAN_7 { get; set; }

		[DataMember]
		public string CAS_OZEL_ALAN_8 { get; set; }

		[DataMember]
		public string CAS_OZEL_ALAN_9 { get; set; }

		[DataMember]
		public string CAS_OZEL_ALAN_10 { get; set; }

		[DataMember]
		public string CAS_TIP { get; set; }

		[DataMember]
		public string CAS_KATEGORI { get; set; }

		[DataMember]
		public string CAS_MALZEME_TIPI { get; set; }

		[DataMember]
		public string CAS_ODEME_SEKLI { get; set; }

		[DataMember]
		public string CAS_LOKASYON { get; set; }

		[DataMember]
		public string CAS_SOZLESME_DURUM { get; set; }

		[DataMember]
		public int CAS_SOZLESME_DURUM_ID { get; set; }

		[DataMember]
		public string CAS_KALAN_GUN { get; set; }

	}
}

