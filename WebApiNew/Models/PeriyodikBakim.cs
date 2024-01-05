using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class PeriyodikBakim
    {
        [DataMember]
        public int TB_PERIYODIK_BAKIM_ID { get; set; }

        [DataMember]
        public string PBK_KOD { get; set; }

        [DataMember]
        public string PBK_TANIM { get; set; }

        [DataMember]
        public bool PBK_AKTIF { get; set; }

        [DataMember]
        public int PBK_TIP_KOD_ID { get; set; }

        [DataMember]
        public int PBK_GRUP_KOD_ID { get; set; }

        [DataMember]
        public bool PBK_ISEMRI_VAR { get; set; }

        [DataMember]
        public string PBK_ISEMRI_NO { get; set; }

        [DataMember]
        public DateTime? PBK_SON_UYGULAMA_TARIH { get; set; }

		[DataMember]
		public DateTime? PBK_HEDEF_UYGULAMA_TARIH { get; set; }

        [DataMember]
        public int PBK_GUNCEL_SAYAC_DEGERI { get; set; }

        [DataMember]
        public int PBK_HEDEF_SAYAC { get; set; }

        [DataMember]
        public int PBK_SON_UYGULAMA_SAYAC { get; set; }

        [DataMember]
        public int PBK_HATIRLAT_SAYAC { get; set; }

        [DataMember]
        public int PBK_HATIRLAT_TARIH { get; set; }

        [DataMember]
        public string PBK_SAYAC_TANIM { get; set; }

        [DataMember]
        public string PBK_GUNCEL_SAYAC { get; set; }

	}
}