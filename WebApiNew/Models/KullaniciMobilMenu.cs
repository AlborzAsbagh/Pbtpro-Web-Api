using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    [DataContract]
    public class KullaniciMobilMenu
    {
        [DataMember]
        public int TB_KULLANICI_MOBIL_MENU_ID { get; set; }

        [DataMember]
        public int KMM_KULLANICI_ID { get; set; }

        [DataMember]
        public int KMM_MOBIL_MENU_ID { get; set; }

        [DataMember]
        public bool KMM_GOR { get; set; }
    }
}