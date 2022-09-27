using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper.Contrib.Extensions;

namespace WebApiNew.Models
{
    [Table("orjin.TB_MAKINE_BAKIM_TARIHCE")]
    public class MakineBakimTarihce
    {
        [Key]
        public int TB_MAKINE_BAKIM_TARIHCE_ID { get; set; }
        public int MBT_MAKINE_BAKIM_ID { get; set; }
        public DateTime? MBT_TARIH { get; set; }
        public string MBT_SAAT { get; set; }
        public string MBT_ACIKLAMA { get; set; }
        public int MBT_DEGISTIREN_ID { get; set; }
        public int MBT_OLUSTURAN_ID { get; set; }
        [Write(false)]
        [Computed]
        public Kullanici MBT_KULLANICI { get; set; }
        [Write(false)]
        [Computed]
        public Makine MBT_MAKINE { get; set; }
        [Write(false)]
        [Computed]
        public MakineBakim MBT_MAKINE_BAKIM { get; set; }
        public DateTime? MBT_OLUSTURMA_TARIH { get; set; }
        public DateTime? MBT_DEGISTIRME_TARIH { get; set; }
        [Write(false)]
        [Computed]
        public List<MakineBakimTarihceDetay> MBT_DETAY { get; set; }
        [Write(false)]
        [Computed]
        public List<IsTalep> MBT_IS_TALEPLERI { get; set; }
        [Write(false)]
        [Computed]
        public List<IsEmri> MBT_IS_EMIRLERI { get; set; }
    }
}