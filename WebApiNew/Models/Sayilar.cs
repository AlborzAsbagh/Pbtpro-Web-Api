using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    [DataContract]
    public class Sayilar
    {
        [DataMember]
        public int ArizaBakim { get; set; }
        [DataMember]
        public int DegisenMalzeme { get; set; }
        [DataMember]
        public int Yakithareketleri { get; set; }
        [DataMember]
        public int SayacHareketleri { get; set; }
        [DataMember]
        public int KontrolList { get; set; }
        [DataMember]
        public int PersonelSayisi { get; set; }
        [DataMember]
        public int MalzemeSayisi { get; set; }
        [DataMember]
        public int DurusSayisi { get; set; }
        [DataMember]
        public int KontrolListYapildi { get; set; }
        [DataMember]
        public int KontrolListYapilmadi { get; set; }
        [DataMember]
        public int Dosya { get; set; }
        [DataMember]
        public int OtonomBakimTarihce { get; set; }
        [DataMember]
        public int CozumKataloglarSayisi { get; set; }

    }
}