using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    [DataContract]
    public class MobilMenu
    {
        [DataMember]
        public int TB_MOBIL_MENU_ID { get; set; }

        [DataMember]
        public string MME_KOD { get; set; }

        [DataMember]
        public string MME_TANIM { get; set; }

        [DataMember]
        public string MME_PRGORAM { get; set; }
    }
}