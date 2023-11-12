using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class Depo
    {
        [DataMember]
        public int TB_DEPO_ID { get; set; }

        [DataMember]
        public int DEP_STOK_ID { get; set; }

        [DataMember]
        public string DEP_KOD { get; set; }

        [DataMember]
        public string DEP_TANIM { get; set; }

        [DataMember]
        public bool DEP_AKTIF { get; set; }

        [DataMember]
        public int DEP_LOKASYON_ID { get; set; }

        [DataMember]
        public int DEP_ATOLYE_ID { get; set; }

        [DataMember]
        public int DEP_MODUL_NO { get; set; }

        [DataMember]
        public double DEP_KAPASITE { get; set; }

        [DataMember]
        public double DEP_KRITIK_MIKTAR { get; set; }

        [DataMember]
        public double DEP_MIKTAR { get; set; }

        [DataMember]
        public string DEP_STOK_BIRIM { get; set; }
	}

    //Web App Version

    public class DeopWebApp
    {
		[DataMember]
		public int TB_DEPO_ID { get; set; }

		[DataMember]
		public string DEP_KOD { get; set; }

		[DataMember]
		public string DEP_TANIM { get; set; }

		[DataMember]
		public string SORUMLU_PERSONEL { get; set; }

		[DataMember]
		public string ATOLYE_TANIM { get; set; }

		[DataMember]
		public string LOKASYON_TANIM { get; set; }

	} 
}