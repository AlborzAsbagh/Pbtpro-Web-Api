using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
   [DataContract]
    public class YakitHareketKartAcilis
    {
        [DataMember]
        public List<Proje> ProjeList { get; set; }
        [DataMember]
        public List<MasrafMerkezi> MasrafMerkeziList { get; set; }
        [DataMember]
        public List<Lokasyon> LokasyonList { get; set; }
        [DataMember]
        public List<Parametre> ParametreList { get; set; }
    }
}