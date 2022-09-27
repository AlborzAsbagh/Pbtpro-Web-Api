using System;
using System.Runtime.Serialization;
using Dapper.Contrib.Extensions;

namespace WebApiNew.Models
{
    [DataContract]
    [Table("orjin.TB_IS_TALEBI_LOG")]
    public class IsTalebiLog
    {
        [DataMember]
        [Key]
        public int TB_IS_TALEP_LOG_ID { get; set; }

        [DataMember]
        public int ITL_IS_TANIM_ID { get; set; }

        [DataMember]
        public int ITL_KULLANICI_ID { get; set; }

        [DataMember]
        public DateTime? ITL_TARIH { get; set; }

        [DataMember]
        public string ITL_SAAT { get; set; }

        [DataMember]
        public string ITL_ISLEM { get; set; }

        [DataMember]
        public string ITL_ACIKLAMA { get; set; }

        [DataMember]
        public string ITL_ISLEM_DURUM { get; set; }

        [DataMember]
        public string ITL_TALEP_ISLEM { get; set; }

        [DataMember]
        public int ITL_OLUSTURAN_ID { get; set; }

        [DataMember]
        public DateTime? ITL_OLUSTURMA_TARIH { get; set; }

    }
}