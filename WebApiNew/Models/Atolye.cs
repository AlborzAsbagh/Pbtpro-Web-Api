using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class Atolye
    {   
        [DataMember]
        public int TB_ATOLYE_ID { get; set; }

        [DataMember]
        public string ATL_KOD { get; set; }

        [DataMember]
        public string ATL_TANIM { get; set; }

        [DataMember]
        public string ATL_TEL { get; set; }

        [DataMember]
        public string ATL_YETKILI { get; set;}

        [DataMember]
        public DateTime ATL_DEGISTIRME_TARIH { get; set; }

        [DataMember]
        public string ATL_DEGISTIREN_ID { get; set; }

        [DataMember]
        public DateTime ATL_OLUSTURMA_TARIH { get; set; }

        [DataMember]
        public int ATL_OLUSTURAN_ID { get; set; }

        [DataMember]
        public string ATL_ACIKLAMA { get ; set; }

        [DataMember]
        public bool ATL_AKTIF { get; set; }

        [DataMember]
        public int ATL_ATOLYE_GRUP_ID { get; set; }

        [DataMember]
        public string ATL_YETKILI_MAIL { get; set; }

        [DataMember]
        public string ALT_GRUP_TANIM { get; set; }

        [DataMember]
        public int ATL_RESIM { get; set; }

        [DataMember]
        public string ALT_BELGE { get; set; }


	}
}