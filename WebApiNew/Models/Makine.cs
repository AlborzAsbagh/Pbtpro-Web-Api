using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;

namespace WebApiNew.Models
{
    [DataContract]
    [Table("TB_MAKINE")]
    public partial class Makine
    {

        public Makine()
        {
            MKN_KOD = "";
            MKN_TANIM = "";
            MKN_TIP = "";
            MKN_LOKASYON = "";
            MKN_OPERATOR = "";
            MKN_TAM_LOKASYON = "";
            MKN_SAYAC_BIRIM = "";
            MKN_KATEGORI = "";
            MKN_DURUM = "";
            MKN_MARKA = "";
            MKN_MODEL = "";
            MKN_URETIM_YILI = "";
            MKN_SERI_NO = "";
            MKN_ARAC_PLAKA = "";
            MKN_PROJE = "";
        }

        [DataMember]
        public int TB_MAKINE_ID { get; set; }

        [DataMember]
        public string MKN_KOD { get; set; }

        [DataMember]
        public string MKN_TANIM { get; set; }

        [DataMember]
        public string MKN_TIP { get; set; }

        [DataMember]
        public int MKN_TIP_KOD_ID { get; set; }

        [DataMember]
        public string MKN_LOKASYON { get; set; }

        [DataMember]
        public string MKN_OPERATOR { get; set; }

        [DataMember]
        public int MKN_OPERATOR_PERSONEL_ID { get; set; }

        [DataMember]
        public string MKN_TAM_LOKASYON { get; set; }

        [DataMember]
        public double MKN_SAYAC_GUNCEL_DEGER { get; set; }

        [DataMember]
        public string MKN_SAYAC_BIRIM { get; set; }

        [DataMember]
        public string MKN_KATEGORI { get; set; }

        [DataMember]
        public int MKN_ACIK_ISEMRI_SAYISI { get; set; }

        [DataMember]
        public int MKN_KAPALI_ISEMRI_SAYISI { get; set; }

        [DataMember]
        public int MKN_ACIK_ISTALEP_SAYISI { get; set; }

        [DataMember]
        public int MKN_KAPALI_ISTALEP_SAYISI { get; set; }

        [DataMember]
        public int MKN_DURUM_KOD_ID { get; set; }

        [DataMember]
        public string MKN_DURUM { get; set; }

        [DataMember]
        public string MKN_MARKA { get; set; }

        [DataMember]
        public string MKN_MODEL { get; set; }

        [DataMember]
        public int MKN_LOKASYON_ID { get; set; }

        [DataMember]
        public string MKN_URETIM_YILI { get; set; }

        [DataMember]
        public string MKN_SERI_NO { get; set; }

        [DataMember]
        public string MKN_ARAC_PLAKA { get; set; }

        [DataMember]
        public string MKN_PROJE { get; set; }

        [DataMember]
        public int MKN_PROJE_ID { get; set; }

        [DataMember]
        [Write(false)]
        [Computed]
        public List<int> ResimIDleri
        {
            get
            {
                if (RSM_IDS == null)
                    return new List<int>();
                var ids = RSM_IDS.Split(';').ToList();
                var idsint = new List<int>();
                foreach (var i in ids)
                {
                    idsint.Add(Int32.Parse(i));
                }

                return idsint;
            }
            set { }
        }

        [DataMember]
        [Write(false)]
        [Computed]
        public string RSM_IDS { get; set; }


        [DataMember]
        [Write(false)]
        [Computed]
        [JsonProperty("ResimVarsayilanID")]
        public int RSM_VAR_ID { get; set; }



    }


    [DataContract]
    [Table("orjin.TB_MAKINE")]
    public partial class MakineUpsert
    {

        [DataMember]
        [Key]
        public int TB_MAKINE_ID { get; set; }

        [DataMember]
        public string MKN_KOD { get; set; }

        [DataMember]
        public string MKN_TANIM { get; set; }

        [DataMember]
        public int MKN_TIP_KOD_ID { get; set; }

        [DataMember]
        public int MKN_DURUM_KOD_ID { get; set; }

        [DataMember]
        public int MKN_LOKASYON_ID { get; set; }
        
        [DataMember]
        public string MKN_SERI_NO { get; set; }

    }
}