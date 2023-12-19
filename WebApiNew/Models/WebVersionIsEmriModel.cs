using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
	[DataContract]
	public class WebVersionIsEmriModel
	{
		[DataMember]
		public int TB_ISEMRI_ID { get; set; }
	
		[DataMember]
		public bool KAPALI { get; set; } 

		[DataMember]
		public string ONCELIK { get; set; } 

		[DataMember]
		public int BELGE { get; set; } 

		[DataMember]
		public int RESIM { get; set; } 

		[DataMember]
		public string ISEMRI_NO { get; set; }

		[DataMember]
		public int MALZEME { get; set; } 

		[DataMember]
		public int PERSONEL { get; set; } 

		[DataMember]
		public int DURUS { get; set; } 

		[DataMember]
		public string OUTER_NOT { get; set; } 

		[DataMember]
		public DateTime? DUZENLEME_TARIH { get; set; } 

		[DataMember]
		public string DUZENLEME_SAAT { get; set; } 

		[DataMember]
		public string KONU { get; set; } 

		[DataMember]
		public string ISEMRI_TIP { get; set; } 

		[DataMember]
		public string DURUM { get; set; } 

		[DataMember]
		public string LOKASYON { get; set; } 

		[DataMember]
		public DateTime? PLAN_BASLAMA_TARIH { get; set; } 

		[DataMember]
		public string PLAN_BASLAMA_SAAT { get; set; }  

		[DataMember]
		public DateTime? PLAN_BITIS_TARIH { get; set; } 

		[DataMember]
		public string PLAN_BITIS_SAAT { get; set; } 

		[DataMember]
		public DateTime? BASLAMA_TARIH { get; set; } 

		[DataMember]
		public string BASLAMA_SAAT { get; set; }  

		[DataMember]
		public DateTime? ISM_BITIS_TARIH { get; set; } 

		[DataMember]
		public string ISM_BITIS_SAAT { get; set; } 

		[DataMember]
		public int IS_SURESI { get; set; } 

		[DataMember]
		public int TAMAMLANMA { get; set; } 

		[DataMember]
		public bool GARANTI { get; set;} 

		[DataMember]
		public string MAKINE_KODU { get; set; } 

		[DataMember]
		public string MAKINE_TANIMI { get; set; } 

		[DataMember]
		public string MAKINE_PLAKA { get; set; } 

		[DataMember]
		public string MAKINE_DURUM { get; set; } 

		[DataMember]
		public string MAKINE_TIP { get; set; } 

		[DataMember]
		public string EKIPMAN { get; set; } 

		[DataMember]
		public string IS_TIPI { get; set; } 

		[DataMember]
		public string IS_NEDENI { get; set;} 

		[DataMember]
		public string ATOLYE { get; set; } 

		[DataMember]
		public string TALIMAT { get; set; } 

		[DataMember]
		public DateTime? KAPANIS_TARIHI { get; set; } 

		[DataMember]
		public string KAPANIS_SAATI { get; set; } 

		[DataMember]
		public string TAKVIM { get; set; } 

		[DataMember]
		public string MASRAF_MERKEZI { get; set; } 

		[DataMember]
		public string FRIMA { get; set; } 

		[DataMember]
		public string IS_TALEP_NO { get; set; } 

		[DataMember]
		public string IS_TALEP_EDEN { get; set; } 

		[DataMember]
		public DateTime? IS_TALEP_TARIH { get; set; } 

		[DataMember]
		public double ISM_MALIYET_MLZ { get; set; } 

		[DataMember]
		public double ISM_MALIYET_PERSONEL { get; set; } 

		[DataMember]
		public double ISM_MALIYET_DISSERVIS { get; set; } 

		[DataMember]
		public double ISM_MALIYET_DIGER { get; set; } 

		[DataMember]
		public double ISM_MALIYET_INDIRIM { get; set; } 

		[DataMember]
		public double ISM_MALIYET_KDV { get; set; } 

		[DataMember]
		public double ISM_MALIYET_TOPLAM { get; set; }  

		[DataMember]
		public int ISM_SURE_MUDAHALE_LOJISTIK { get; set; } 

		[DataMember]
		public int ISM_SURE_MUDAHALE_SEYAHAT { get; set; } 

		[DataMember]
		public int ISM_SURE_MUDAHALE_ONAY { get; set; } 

		[DataMember]
		public int ISM_SURE_BEKLEME { get; set; } 

		[DataMember]
		public int ISM_SURE_MUDAHALE_DIGER { get; set; } 

		[DataMember]
		public int ISM_SURE_PLAN_MUDAHALE { get; set; } 

		[DataMember]
		public int ISM_SURE_PLAN_CALISMA { get; set; } 

		[DataMember]
		public int ISM_SURE_TOPLAM { get; set; }  

		[DataMember]
		public int ISM_TIP_ID { get; set; } 

		[DataMember]
		public int ISM_DURUM_KOD_ID { get; set; } 

		[DataMember]
		public int ISM_BAGLI_ISEMRI_ID { get; set; } 

		[DataMember]
		public int ISM_LOKASYON_ID { get; set; } 

		[DataMember]
		public int ISM_MAKINE_ID { get; set; } 

		[DataMember]
		public int ISM_EKIPMAN_ID { get; set; } 

		[DataMember]
		public int ISM_MAKINE_DURUM_KOD_ID { get; set; }  

		[DataMember]
		public int ISM_REF_ID { get; set; } 

		[DataMember]
		public int ISM_TIP_KOD_ID { get; set; } 

		[DataMember]
		public int ISM_NEDEN_KOD_ID { get; set; } 

		[DataMember]
		public int ISM_ONCELIK_ID { get; set; } 

		[DataMember]
		public int ISM_ATOLYE_ID { get; set; } 

		[DataMember]
		public int ISM_TAKVIM_ID { get; set; } 

		[DataMember]
		public int ISM_TALIMAT_ID { get; set; } 

		[DataMember]
		public int ISM_MASRAF_MERKEZ_ID { get; set; } 

		[DataMember]
		public int ISM_PROJE_ID { get; set; } 

		[DataMember]
		public int ISM_FIRMA_ID { get; set; } 

		[DataMember]
		public int ISM_FIRMA_SOZLESME_ID { get; set; } 

		[DataMember]
		public int ISM_OZEL_ALAN_11_KOD_ID { get; set; } 

		[DataMember]
		public int ISM_OZEL_ALAN_12_KOD_ID { get; set; } 

		[DataMember]
		public int ISM_OZEL_ALAN_13_KOD_ID { get; set; }  

		[DataMember]
		public int ISM_OZEL_ALAN_14_KOD_ID { get; set; } 

		[DataMember]
		public int ISM_OZEL_ALAN_15_KOD_ID { get; set; } 

		[DataMember]
		public string ISM_SOZLESME_TANIM { get; set; } 

		[DataMember]
		public string ISM_PROJE_KOD { get; set; } 

		[DataMember]
		public string ISM_ATOLYE_KOD { get; set; } 

		[DataMember]
		public string ISM_PROSEDUR_KOD { get; set; } 

		[DataMember]
		public DateTime? ISM_GARANTI_BITIS { get; set; } 

		[DataMember]
		public string ISM_BAGLI_ISEMRI_NO { get; set; } 

		[DataMember]
		public string ISM_EVRAK_NO { get; set; } 

		[DataMember]
		public DateTime? ISM_EVRAK_TARIHI { get; set; } 

		[DataMember]
		public string ISM_REFERANS_NO { get; set; } 

		[DataMember]
		public string OZEL_ALAN_1 { get; set; } 

		[DataMember]
		public string OZEL_ALAN_2 { get; set; } 

		[DataMember]
		public string OZEL_ALAN_3 { get; set; } 

		[DataMember]
		public string OZEL_ALAN_4 { get; set; } 

		[DataMember]
		public string OZEL_ALAN_5 { get; set; } 

		[DataMember]
		public string OZEL_ALAN_6 { get; set; } 

		[DataMember]
		public string OZEL_ALAN_7 { get; set; } 

		[DataMember]
		public string OZEL_ALAN_8 { get; set; } 

		[DataMember]
		public string OZEL_ALAN_9 { get; set; } 

		[DataMember]
		public string OZEL_ALAN_10 { get; set; } 

		[DataMember]
		public string OZEL_ALAN_11 { get; set; } 

		[DataMember]
		public string OZEL_ALAN_12 { get; set; } 

		[DataMember]
		public string OZEL_ALAN_13 { get; set; } 

		[DataMember]
		public string OZEL_ALAN_14 { get; set; } 

		[DataMember]
		public string OZEL_ALAN_15 { get; set; } 

		[DataMember]
		public double OZEL_ALAN_16 { get; set; } 

		[DataMember]
		public double OZEL_ALAN_17 { get; set; } 

		[DataMember]
		public double OZEL_ALAN_18 { get; set; }

		[DataMember]
		public double OZEL_ALAN_19 { get; set; } 

		[DataMember]
		public double OZEL_ALAN_20 { get; set; } 

		[DataMember]
		public string BILDIRILEN_KAT { get; set; } 

		[DataMember]
		public string BILDIRILEN_BINA { get; set; } 

		[DataMember]
		public string PERSONEL_ADI { get; set; } 

		[DataMember]
		public string TAM_LOKASYON { get; set; } 

		[DataMember]
		public int GUNCEL_SAYAC_DEGER { get; set; } 

		[DataMember]
		public string ICERDEKI_NOT { get; set; } 
	}
}

