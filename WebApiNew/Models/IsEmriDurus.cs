using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    [DataContract]
    public class IsEmriDurus
    {

         [DataMember] public int TB_MAKINE_DURUS_ID { get; set; }

         [DataMember] public int MKD_ISEMRI_ID { get; set; }

         [DataMember] public int MKD_MAKINE_ID { get; set; }

         [DataMember] public DateTime? MKD_BASLAMA_TARIH { get; set; }

         [DataMember] public string MKD_BASLAMA_SAAT { get; set; }

         [DataMember] public DateTime? MKD_BITIS_TARIH { get; set; }

         [DataMember] public string MKD_BITIS_SAAT { get; set; }

         [DataMember] public int MKD_SURE { get; set; }

         [DataMember] public double MKD_SAAT_MALIYET { get; set; }

         [DataMember] public double MKD_TOPLAM_MALIYET { get; set; }

         [DataMember] public int MKD_NEDEN_KOD_ID { get; set; }

         [DataMember] public string MKD_ACIKLAMA { get; set; }

         [DataMember] public bool MKD_PLANLI { get; set; }

         [DataMember] public int MKD_OLUSTURAN_ID { get; set; }

         [DataMember] public DateTime? MKD_OLUSTURMA_TARIH { get; set; }

         [DataMember] public int MKD_DEGISTIREN_ID { get; set; }

         [DataMember] public DateTime? MKD_DEGISTIRME_TARIH { get; set; }

         [DataMember] public int MKD_PROJE_ID { get; set; }

         [DataMember] public int MKD_PUANTAJ_CALISMA_KART_ID { get; set; }

         [DataMember] public int MKD_LOKASYON_ID { get; set; }

         [DataMember] public string MKD_NEDEN { get; set; }
         [DataMember] public Makine MKD_MAKINE { get; set; }
         [DataMember] public Lokasyon MKD_LOKASYON { get; set; }
         [DataMember] public Proje MKD_PROJE { get; set; }


    }
}