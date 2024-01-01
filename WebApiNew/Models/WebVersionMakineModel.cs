using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
	[DataContract]
	public class WebVersionMakineModel
	{
		[DataMember]
		public int TB_MAKINE_ID { get; set; }
		[DataMember]
		public int MKN_BELGE { get; set; }
		[DataMember]
		public bool MKN_BELGE_VAR { get; set; }
		[DataMember]
		public int MKN_RESIM { get; set; }
		[DataMember]
		public bool MKN_RESIM_VAR { get; set; }
		[DataMember]
		public bool MKN_PERIYODIK_BAKIM { get; set; }
		[DataMember]
		public string MKN_KOD { get; set; }
		[DataMember]

		public string MKN_TANIM { get; set; }
		[DataMember]
		public bool MKN_AKTIF { get; set; }
		[DataMember]
		public int MKN_DURUM_KOD_ID { get; set; }
		[DataMember]
		public string MKN_DURUM { get; set; }
		[DataMember]
		public string MKN_ARAC_TIP { get; set; }
		[DataMember]
		public int MKN_LOKASYON_ID { get; set; }
		[DataMember]
		public string MKN_LOKASYON { get; set; }
		[DataMember]
		public int MKN_TIP_KOD_ID { get; set; }
		[DataMember]
		public string MKN_TIP { get; set; }
		[DataMember]
		public int MKN_KATEGORI_KOD_ID { get; set; }
		[DataMember]
		public string MKN_KATEGORI { get; set; }
		[DataMember]
		public int MKN_MARKA_KOD_ID { get; set; }
		[DataMember]
		public int MKN_MODEL_KOD_ID { get; set; }
		[DataMember]
		public string MKN_MARKA { get; set; }
		[DataMember]
		public string MKN_MODEL { get; set; }
		[DataMember]
		public int MKN_MASTER_ID { get; set; }
		[DataMember]
		public string MKN_MASTER_MAKINE_KOD { get; set; }
		[DataMember]
		public string MKN_MASTER_MAKINE_TANIM { get; set; }
		[DataMember]
		public int MKN_TAKVIM_ID { get; set; }
		[DataMember]
		public string MKN_TAKVIM { get; set; }
		[DataMember]
		public string MKN_URETIM_YILI { get; set; }
		[DataMember]
		public int MKN_MASRAF_MERKEZ_KOD_ID { get; set; }
		[DataMember]
		public string MKN_MASRAF_MERKEZ { get; set; }
		[DataMember]
		public int MKN_ATOLYE_ID { get; set; }

		[DataMember]
		public string MKN_ATOLYE { get; set; }

		[DataMember]
		public string MKN_BAKIM_GRUP { get; set; }

		[DataMember]
		public int MKN_BAKIM_GRUP_ID { get; set; }

		[DataMember]
		public string MKN_ARIZA_GRUP { get; set; }

		[DataMember]
		public int MKN_ARIZA_GRUP_ID { get; set; }

		[DataMember]
		public int MKN_ONCELIK_ID { get; set; }
		[DataMember]
		public string MKN_ONCELIK { get; set; }
		[DataMember]
		public int ARIZA_SIKLIGI { get; set; }
		[DataMember]
		public int ARIZA_SAYISI { get; set; }
		[DataMember]
		public string MKN_OZEL_ALAN_1 { get; set; }
		[DataMember]
		public string MKN_OZEL_ALAN_2 { get; set; }
		[DataMember]
		public string MKN_OZEL_ALAN_3 { get; set; }
		[DataMember]
		public string MKN_OZEL_ALAN_4 { get; set; }
		[DataMember]
		public string MKN_OZEL_ALAN_5 { get; set; }
		[DataMember]
		public string MKN_OZEL_ALAN_6 { get; set; }
		[DataMember]
		public string MKN_OZEL_ALAN_7 { get; set; }
		[DataMember]
		public string MKN_OZEL_ALAN_8 { get; set; }
		[DataMember]
		public string MKN_OZEL_ALAN_9 { get; set; }
		[DataMember]
		public string MKN_OZEL_ALAN_10 { get; set; }

		[DataMember]
		public string MKN_OZEL_ALAN_11 { get; set; }

		[DataMember]
		public string MKN_OZEL_ALAN_12 { get; set; }

		[DataMember]
		public string MKN_OZEL_ALAN_13 { get; set; }

		[DataMember]
		public string MKN_OZEL_ALAN_14 { get; set; }

		[DataMember]
		public string MKN_OZEL_ALAN_15 { get; set; }

		[DataMember]
		public int MKN_OZEL_ALAN_11_KOD_ID { get; set; }
		[DataMember]
		public int MKN_OZEL_ALAN_12_KOD_ID { get; set; }
		[DataMember]
		public int MKN_OZEL_ALAN_13_KOD_ID { get; set; }
		[DataMember]
		public int MKN_OZEL_ALAN_14_KOD_ID { get; set; }
		[DataMember]
		public int MKN_OZEL_ALAN_15_KOD_ID { get; set; }
		[DataMember]
		public float MKN_OZEL_ALAN_16 { get; set; }
		[DataMember]
		public float MKN_OZEL_ALAN_17 { get; set; }
		[DataMember]
		public float MKN_OZEL_ALAN_18 { get; set; }
		[DataMember]
		public float MKN_OZEL_ALAN_19 { get; set; }
		[DataMember]
		public float MKN_OZEL_ALAN_20 { get; set; }

		[DataMember]
		public string MKN_SERI_NO { get; set; }

		[DataMember]
		public string MKN_LOKASYON_TUM_YOL { get; set; }

		[DataMember]
		public string MKN_URETICI { get; set; }

		[DataMember]
		public DateTime? MKN_GARANTI_BITIS { get; set; }

		[DataMember]
		public float MKN_DURUS_MALIYET { get;set; }

		[DataMember]
		public int MKN_YILLIK_PLANLANAN_CALISMA_SURESI { get; set; }

		[DataMember]
		public bool MKN_KALIBRASYON_VAR { get; set; }

		[DataMember]
		public bool MKN_KRITIK_MAKINE { get; set; }

		[DataMember]
		public bool MKN_GUC_KAYNAGI { get; set; }

		[DataMember]
		public bool MKN_IS_TALEP { get; set; }

		[DataMember]
		public bool MKN_YAKIT_KULLANIM { get; set; }

		[DataMember]
		public bool MKN_OTONOM_BAKIM { get; set; }

		[DataMember]
		public int MKN_SERVIS_SAGLAYICI_KOD_ID { get; set; }

		[DataMember]
		public string MKN_SERVIS_SAGLAYICI { get; set; }

		[DataMember]
		public int MKN_SERVIS_SEKLI_KOD_ID { get;set; }

		[DataMember]
		public string MKN_SERVIS_SEKLI { get; set; }

		[DataMember]
		public int MKN_TEKNIK_SERVIS_KOD_ID { get; set; }

		[DataMember]
		public string MKN_TEKNIK_SERVIS { get; set; }

		[DataMember]
		public int MKN_FIZIKSEL_DURUM_KOD_ID { get; set; }

		[DataMember]
		public string MKN_FIZIKSEL_DURUM { get; set; }

		[DataMember]
		public string MKN_RISK_PUAN { get; set; }

		[DataMember]
		public DateTime MKN_KURULUM_TARIH { get; set; }

		[DataMember]
		public int MKN_ISLETIM_SISTEMI_KOD_ID { get; set; }

		[DataMember]
		public string MKN_ISLETIM_SISTEMI { get; set; }

		[DataMember]
		public string MKN_IP_NO { get; set; }

		[DataMember]
		public float MKN_AGIRLIK { get; set; }

		[DataMember]
		public float MKN_HACIM { get; set; }

		[DataMember]
		public int MKN_AGIRLIK_BIRIM_KOD_ID { get; set; }

		[DataMember]
		public string MKN_AGIRLIK_BIRIM { get; set; }

		[DataMember]
		public int MKN_HACIM_BIRIM_KOD_ID { get; set; }

		[DataMember]
		public string MKN_HACIM_BIRIM { get; set ; }


		[DataMember]
		public int MKN_KAPASITE_BIRIM_KOD_ID { get; set; }

		[DataMember]
		public string MKN_KAPASITE_BIRIM { get;set; }



		[DataMember]
		public string MKN_KAPASITE { get; set; }

		[DataMember]
		public string MKN_ELEKTRIK_TUKETIM { get; set; }

		[DataMember]
		public int MKN_ELEKTRIK_TUKETIM_BIRIM_KOD_ID { get; set; }

		[DataMember]
		public string MKN_ELEKTRIK_TUKETIM_BIRIM { get; set; }


		[DataMember]
		public string MKN_VOLTAJ { get; set; }

		[DataMember]
		public string MKN_GUC { get; set; }

		[DataMember]
		public string MKN_FAZ { get; set; }

		[DataMember]
		public int MKN_VALF_TIP_KOD_ID { get; set; }

		[DataMember]
		public int MKN_VALF_BOYUT_KOD_ID { get; set; }

		[DataMember]
		public string MKN_VALF_TIPI { get; set; }

		[DataMember]
		public string MKN_VALF_BOYUT { get; set; }

		[DataMember]
		public int MKN_GIRIS_BOYUT_KOD_ID { get; set; }

		[DataMember]
		public int MKN_CIKIS_BOYUT_KOD_ID { get; set; }

		[DataMember]
		public string MKN_GIRIS_BOYUT { get; set; }

		[DataMember]
		public string MKN_CIKIS_BOYUT { get; set; }

		[DataMember]
		public int MKN_KONNEKTOR_KOD_ID { get; set; }

		[DataMember]
		public string MKN_KONNEKTOR { get; set; }

		[DataMember]
		public int MKN_BASINC_KOD_ID { get;set; }

		[DataMember]
		public string MKN_BASINC { get; set; }

		[DataMember]
		public float MKN_BASINC_MIKTAR { get; set; }

		[DataMember]
		public int MKN_BASINC_BIRIM_KOD_ID { get; set; }

		[DataMember]
		public string MKN_BASINC_BIRIM { get; set; }

		[DataMember]
		public float MKN_DEVIR { get; set; }

		[DataMember]
		public string MKN_TEKNIK_MOTOR_GUCU { get; set; }

		[DataMember]
		public string MKN_TEKNIK_SILINDIR_SAYISI { get; set; }


		[DataMember]
		public int MKN_ALIS_FIRMA_ID { get; set; }

		[DataMember]
		public int MKN_SATIS_FIRMA_ID { get; set; }

		[DataMember]
		public string MKN_ALIS_FIRMA { get; set; }

		[DataMember]
		public string MKN_SATIS_FIRMA { get; set; }

		[DataMember]
		public bool MKN_SATIS { get; set; }

		[DataMember]
		public bool MKN_KIRA { get; set; }

		[DataMember]
		public DateTime MKN_ALIS_TARIH { get; set; }

		[DataMember]
		public float MKN_ALIS_FIYAT { get; set; }

		[DataMember]
		public string MKN_FATURA_NO { get; set; }

		[DataMember]
		public DateTime? MKN_FATURA_TARIH { get; set; }

		[DataMember]
		public float MKN_FATURA_TUTAR { get; set; }

		[DataMember]
		public float MKN_KREDI_MIKTARI { get; set; }

		[DataMember]
		public float KREDI_ORANI { get; set; }

		[DataMember]
		public DateTime? MKN_KREDI_BASLAMA_TARIHI { get; set; }

		[DataMember]
		public DateTime? MKN_KREDI_BITIS_TARIHI { get; set; }

		[DataMember]
		public DateTime? MKN_KIRA_BASLANGIC_TARIH { get; set; }

		[DataMember]
		public DateTime? MKN_KIRA_BITIS_TARIH { get; set; }

		[DataMember]
		public float MKN_KIRA_SURE { get; set; }

		[DataMember]
		public string MKN_KIRA_PERIYOD { get; set; }

		[DataMember]
		public float MKN_KIRA_TUTAR { get; set; }

		[DataMember]
		public string MKN_KIRA_ACIKLAMA { get; set; }

		[DataMember]
		public DateTime? MKN_SATIS_TARIH { get; set; }

		[DataMember]
		public string MKN_SATIS_NEDEN { get; set; }

		[DataMember]
		public string MKN_SATIS_YER { get; set;}

		[DataMember]
		public string MKN_SATIS_ACIKLAMA { get; set; }

		[DataMember]
		public float MKN_SATIS_FIYAT { get; set; }

		[DataMember]
		public string MKN_GENEL_NOT { get; set; }

		[DataMember]
		public string MKN_GUVENLIK_NOT { get; set; }


	} 
} 

