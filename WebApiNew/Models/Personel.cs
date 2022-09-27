using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class Personel
    {
         [DataMember] public int TB_PERSONEL_ID { get; set; }

         [DataMember] public int PRS_RESIM_ID { get; set; }

         [DataMember] public string PRS_PERSONEL_KOD { get; set; }

         [DataMember] public string PRS_ISIM { get; set; }

         [DataMember] public string PRS_UNVAN { get; set; }

         [DataMember] public bool PRS_AKTIF { get; set; }

         [DataMember] public double PRS_SAAT_UCRET { get; set; }

         [DataMember] public bool PRS_TEKNISYEN { get; set; }

         [DataMember] public bool PRS_SURUCU { get; set; }

         [DataMember] public bool PRS_OPERATOR { get; set; }

         [DataMember] public string PRS_LOKASYON { get; set; }

         [DataMember] public string PRS_DEPARTMAN { get; set; }

         [DataMember] public string PRS_TIP { get; set; }

         [DataMember] public string PRS_RESIM_IDLERI { get; set; }
    }
}