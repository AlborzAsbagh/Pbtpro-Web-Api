using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    [DataContract]
    public class SayacOkuma
    {
        [DataMember]
        public int TB_SAYAC_OKUMA_ID { get; set; }

        [DataMember]
        public int SYO_SAYAC_ID { get; set; }

        [DataMember]
        public DateTime? SYO_TARIH { get; set; }

        [DataMember]
        public string SYO_SAAT { get; set; }

        [DataMember]
        public double SYO_OKUNAN_SAYAC { get; set; }

        [DataMember]
        public double SYO_FARK_SAYAC { get; set; }

        [DataMember]
        public string SYO_ACIKLAMA { get; set; }

        [DataMember]
        public bool SYO_ELLE_GIRIS { get; set; }

        [DataMember]
        public int SYO_OLUSTURAN_ID { get; set; }

        [DataMember]
        public DateTime? SYO_OLUSTURMA_TARIH { get; set; }

        [DataMember]
        public int SYO_DEGISTIREN_ID { get; set; }

        [DataMember]
        public DateTime? SYO_DEGISTIRME_TARIH { get; set; }

        [DataMember]
        public string SYO_HAREKET_TIP { get; set; }

        [DataMember]
        public int SYO_REF_ID { get; set; }

        [DataMember]
        public string SYO_REF_GRUP { get; set; }

        [DataMember]
        public bool SYO_SAYAC_GUNCELLE { get; set; }

        [DataMember]
        public int SYO_MAKINE_PUANTAJ_ID { get; set; }

        [DataMember]
        public int SYO_PROJE_ID { get; set; }

        [DataMember]
        public int SYO_LOKASYON_ID { get; set; }
        [DataMember]
        public string SYO_LOK_TANIM { get; set; }

        [DataMember]
        public string SYO_MAKINE { get; set; }

        [DataMember]
        public string MES_TANIM { get; set; }

        [DataMember]
        public string MES_SAYAC_BIRIM { get; set; }

        [DataMember]
        public string MES_SAYAC_TIP { get; set; }

        [DataMember]
        public int SYO_MAKINE_ID { get; set; }

		[DataMember]
		public String ResimIDleri { get; set; }




	}
}