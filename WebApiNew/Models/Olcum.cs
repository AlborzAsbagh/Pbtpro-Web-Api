using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper.Contrib.Extensions;

namespace WebApiNew.Models
{
    [Table("orjin.TB_ISEMRI_OLCUM")]
    public class Olcum
    {
        [Key]
        public int TB_ISEMRI_OLCUM_ID { get; set; }
        public int IDO_SIRANO { get; set; }
        public int IDO_ISEMRI_ID { get; set; }
        public int IDO_OLCUM_PARAMETRE_ID { get; set; }
        public int IDO_BIRIM_KOD_ID { get; set; }
        public string IDO_BIRIM { get; set; }
        public double IDO_HEDEF_DEGER { get; set; }
        public double IDO_MIN_DEGER { get; set; }
        public double IDO_MAX_DEGER { get; set; }
        public double IDO_OLCUM_DEGER { get; set; }
        public double IDO_FARK { get; set; }
        public DateTime? IDO_TARIH { get; set; }
        public string IDO_SAAT { get; set; }
        public int IDO_OLUSTURAN_ID { get; set; }
        public DateTime? IDO_OLUSTURMA_TARIH { get; set; }
        public int IDO_DEGISTIREN_ID { get; set; }
        public DateTime? IDO_DEGISTIRME_TARIH { get; set; }
        public string IDO_TANIM { get; set; }
        public short IDO_FORMAT { get; set; }
        public double IDO_MIN_MAX_DEGER { get; set; }
        public string IDO_DURUM { get; set; }
        public int IDO_REF_ID { get; set; }
        public int IDO_MAKINE_ID { get; set; }
        public int IDO_LOKASYON_ID { get; set; }
        [Write(false)]
        [Computed]
        public Makine IDO_MAKINE { get; set; }
        [Write(false)]
        [Computed]
        public Lokasyon IDO_LOKASYON { get; set; }
        [Write(false)]
        [Computed]
        public OlcumParametre IDO_PARAMETRE { get; set; }
        public string IDO_ACIKLAMA { get; set; }
    }

}