using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper.Contrib.Extensions;

namespace WebApiNew.Models
{
    [Table("orjin.TB_MAKINE_BAKIM_TARIHCE_DETAY")]
    public class MakineBakimTarihceDetay
    {
        [Key]
        public int TB_MAKINE_BAKIM_TARIHCE_DETAY_ID { get; set; }
        public int MBD_MAKINE_BAKIM_TARIHCE_ID { get; set; }
        public DateTime? MBD_TARIH { get; set; }
        public string MBD_SAAT { get; set; }
        public string MBD_SIRANO { get; set; }
        public bool MBD_YAPILDI { get; set; }
        public string MBD_TANIM { get; set; }
        public string MBD_ACIKLAMA { get; set; }
        public int MBD_DEGISTIREN_ID { get; set; }
        public int MBD_OLUSTURAN_ID { get; set; }
        public DateTime? MBD_OLUSTURMA_TARIH { get; set; }
        public DateTime? MBD_DEGISTIRME_TARIH { get; set; }
    }

}