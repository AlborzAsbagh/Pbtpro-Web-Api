using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class IsEmriKartAcilis
    {
        [DataMember] public string NEW_ISM_KOD { get; set; }
        [DataMember] public IsEmriTip VarsayilanIsemriTipi { get; set; }
        [DataMember] public List<IsEmriTip> IsEmriTipList { get; set; }
        [DataMember] public List<Proje> ProjeList { get; set; }
        [DataMember] public List<Oncelik> OncelikList { get; set; }
        [DataMember] public List<MasrafMerkezi> MasrafMerkeziList { get; set; }
        [DataMember] public List<Lokasyon> LokasyonList { get; set; }
        [DataMember] public List<IsTanim> BakimTanimlari { get; set; }
        [DataMember] public List<IsTanim> ArizaTanimlari { get; set; }
        [DataMember] public List<Kod> ArizaTipList { get; set; }
        [DataMember] public List<Kod> BakimTipList { get; set; }
        [DataMember] public List<Kod> BakimNedenList { get; set; }
        [DataMember] public List<Kod> DurusNedenList { get; set; }
        [DataMember] public List<Kod> ArizaNedenList { get; set; }
        [DataMember] public List<Kod> OZEL_ALAN_11_KOD_LIST { get; set; }
        [DataMember] public List<Kod> OZEL_ALAN_12_KOD_LIST { get; set; }
        [DataMember] public List<Kod> OZEL_ALAN_13_KOD_LIST { get; set; }
        [DataMember] public List<Kod> OZEL_ALAN_14_KOD_LIST { get; set; }
        [DataMember] public List<Kod> OZEL_ALAN_15_KOD_LIST { get; set; }
        [DataMember] public List<Atolye> AtolyeList { get; set; }
        [DataMember] public List<Kod> DurumList { get; set; }
        [DataMember] public OzelAlan OZEL_ALAN { get; set; }
        [DataMember] public bool MOBIL_BARKOD_AC_KAPA { get; set; }
    }
}