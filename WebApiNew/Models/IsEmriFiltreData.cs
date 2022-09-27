using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class IsEmriFiltreData
    {
        [DataMember] public List<IsEmriTip> IsEmriTipList { get; set; }
        [DataMember] public List<Lokasyon> LokasyonList { get; set; }
        [DataMember] public List<Atolye> AtolyeList { get; set; }
        [DataMember] public List<Personel> PersonelList { get; set; }
        [DataMember] public List<Proje> ProjeList { get; set; }
    }
}