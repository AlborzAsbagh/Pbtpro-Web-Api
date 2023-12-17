using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class Filtre
    {
        [DataMember]
        public int MakineID { get; set; }
        [DataMember]
        public int MasterMakineID { get; set; }

        [DataMember]
        public int isEmriDurumId { get; set; }
		[DataMember]
        public int IlgiliKisiId { get; set; }
        [DataMember]
        public int LokasyonID { get; set; }
        [DataMember]
        public int PersonelID { get; set; }
        [DataMember]
        public string Kelime { get; set; }
        [DataMember]
        public string BasTarih { get; set; }
        [DataMember]
        public string BitTarih { get; set; }
        [DataMember]
        public DateTime? PlanBasTarih { get; set; }
        [DataMember]
        public DateTime? PlanBitTarih { get; set; }
        [DataMember]
        public int durumID { get; set; }
        [DataMember]
        public int ProjeID { get; set; }
        [DataMember]
        public string Tip { get; set; }
        [DataMember]
        public int DepoID { get; set; }
        [DataMember]
        public int nedenID { get; set; }
        [DataMember]
        public int isEmriTipId { get; set; }
        [DataMember]
        public float value1 { get; set; }
        [DataMember]
        public float value2 { get; set; }

    }
}