using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class IsEmriKontrolList
    {
         [DataMember] public int TB_ISEMRI_KONTROLLIST_ID { get; set; }

         [DataMember] public int DKN_ISEMRI_ID { get; set; }

         [DataMember] public string DKN_SIRANO { get; set; }

         [DataMember] public bool DKN_YAPILDI { get; set; }

         [DataMember] public string DKN_TANIM { get; set; }

         [DataMember] public double DKN_MALIYET { get; set; }

         [DataMember] public string DKN_ACIKLAMA { get; set; }

         [DataMember] public DateTime? DKN_YAPILDI_TARIH { get; set; }

         [DataMember] public string DKN_YAPILDI_SAAT { get; set; }

         [DataMember] public int DKN_YAPILDI_PERSONEL_ID { get; set; }

         [DataMember] public int DKN_YAPILDI_MESAI_KOD_ID { get; set; }

         [DataMember] public int DKN_YAPILDI_ATOLYE_ID { get; set; }

         [DataMember] public int DKN_YAPILDI_SURE { get; set; }

         [DataMember] public int DKN_REF_ID { get; set; }

         [DataMember] public int DKN_TALIMAT_ID { get; set; }

         [DataMember] public DateTime? DKN_BITIS_TARIH { get; set; }

         [DataMember] public string DKN_BITIS_SAAT { get; set; }

         [DataMember] public int DKN_OLUSTURAN_ID { get; set; }

         [DataMember] public DateTime? DKN_OLUSTURMA_TARIH { get; set; }

         [DataMember] public int DKN_DEGISTIREN_ID { get; set; }

         [DataMember] public DateTime? DKN_DEGISTIRME_TARIH { get; set; }

         [DataMember] public Boolean DKN_SILINDI { get; set; }



    }
}