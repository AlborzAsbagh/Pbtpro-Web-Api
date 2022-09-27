using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    [DataContract]
    public class Parametre
    {
        [DataMember]
        public int TB_PARAMETRE_ID { get; set; }

        [DataMember]
        public int PRM_GRUP_ID { get; set; }

        [DataMember]
        public string PRM_KOD { get; set; }

        [DataMember]
        public string PRM_TANIM { get; set; }

        [DataMember]
        public string PRM_DEGER { get; set; }
    }
}