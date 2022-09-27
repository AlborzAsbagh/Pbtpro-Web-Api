using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Dapper.Contrib.Extensions;

namespace WebApiNew.Models
{
    [DataContract]
    [Table("orjin.TB_RESIM")]
    public class Resim
    {

        [DataMember] [Key] public int TB_RESIM_ID { get; set; }

        [DataMember] public int RSM_REF_ID { get; set; }

        [DataMember] public string RSM_REF_GRUP { get; set; }

        [DataMember] public bool RSM_VARSAYILAN { get; set; }

        [DataMember] public string RSM_UZANTI { get; set; }

        [DataMember] public string RSM_RESIM_AD { get; set; }

        [DataMember] public string RSM_YOL { get; set; }

        [DataMember] public string RSM_ARSIV_AD { get; set; }

        [DataMember] public string RSM_ARSIV_YOL { get; set; }

        [DataMember] public int RSM_BOYUT { get; set; }

        [DataMember] public string RSM_ETIKET { get; set; }

        [DataMember] public int RSM_OLUSTURAN_ID { get; set; }

        [DataMember] public DateTime? RSM_OLUSTURMA_TARIH { get; set; }

        [DataMember] public int RSM_DEGISTIREN_ID { get; set; }

        [DataMember] public DateTime? RSM_DEGISTIRME_TARIH { get; set; }
    }
}