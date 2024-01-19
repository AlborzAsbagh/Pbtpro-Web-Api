using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    public class OlcumParametre
    {
        public int TB_PERIYODIK_BAKIM_OLCUM_PARAMETRE_ID { get; set; }
        public int PBC_PERIYODIK_BAKIM_ID { get; set; }
        public int PBC_SIRA_NO { get; set; }
        public string PBC_TANIM { get; set; }
        public int PBC_BIRIM_KOD_ID { get; set; }
        public string PBC_BIRIM { get; set; }
        public double PBC_HEDEF_DEGER { get; set; }
        public double PBC_MIN_MAX_DEGER { get; set; }
        public double PBC_MIN_DEGER { get; set; }
        public double PBC_MAX_DEGER { get; set; }
        public short PBC_FORMAT { get; set; }
        public int PBC_OLUSTURAN_ID { get; set; }
        public DateTime? PBC_OLUSTURMA_TARIH { get; set; }
        public int PBC_DEGISTIREN_ID { get; set; }
        public DateTime? PBC_DEGISTIRME_TARIH { get; set; }
    }
	
    public  class OlcumParametreWebApp
	{
        [DataMember]
        public int TB_IS_TANIM_OLCUM_PARAMETRE_ID { get; set; }

		[DataMember]
		public int IOC_IS_TANIM_ID { get; set; }

		[DataMember]
		public int IOC_SIRA_NO { get; set; }

		[DataMember]
		public string IOC_TANIM { get; set; }

		[DataMember]
		public int IOC_BIRIM_KOD_ID { get; set; }

		[DataMember]
		public string IOC_BIRIM { get; set; }

		[DataMember]
		public float IOC_HEDEF_DEGER { get; set; }

		[DataMember]
		public float IOC_MIN_MAX_DEGER { get; set; }

		[DataMember]
		public float IOC_MIN_DEGER { get; set; }

		[DataMember]
		public float IOC_MAX_DEGER { get; set; }

		[DataMember]
		public short IOC_FORMAT { get; set; }

		[DataMember]
		public int IOC_OLUSTURAN_ID { get; set; }

		[DataMember]
		public DateTime? IOC_OLUSTURMA_TARIH { get; set; }

		[DataMember]
		public int IOC_DEGISTIREN_ID { get; set; }

		[DataMember]
		public DateTime? IOC_DEGISTIRME_TARIH { get; set; }

	}

}

      
      