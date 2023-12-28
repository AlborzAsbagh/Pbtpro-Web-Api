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
		public string MKN_ARIZA_GRUP { get; set; }
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

	} 
} 

