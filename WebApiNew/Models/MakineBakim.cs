using Dapper.Contrib.Extensions;
using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{

    [DataContract]
    public class MakineBakim
    {
        [DataMember]
        public int TB_MAKINE_BAKIM_ID { get; set; }
        [DataMember]
        public int MAB_MAKINE_ID { get; set; }
        [DataMember]
        public int MAB_BAKIM_ID { get; set; }
        [DataMember]
        public int MAB_DEGISTIREN_ID { get; set; }
        [DataMember]
        public int MAB_OLUSTURAN_ID { get; set; }
        [DataMember]
        public DateTime? MAB_OLUSTURMA_TARIH { get; set; }
        [DataMember]
        public DateTime? MAB_DEGISTIRME_TARIH { get; set; }
        [DataMember]
        [Write(false)]
        [Computed]
        public bool MAB_UYAR { get; set; }
        [DataMember]
        public IsTanim MAB_IS_TANIM { get; set; }
    }
}