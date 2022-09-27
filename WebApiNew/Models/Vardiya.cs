using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiNew.Models
{
    public class Vardiya
    {
        public int TB_VARDIYA_ID { get; set; }
        public string VAR_TANIM { get; set; }
        public int VAR_LOKASYON_ID { get; set; }
        public int VAR_PROJE_ID { get; set; }
        public string VAR_ACIKLAMA { get; set; }
        public string VAR_BASLAMA_SAATI { get; set; }
        public string VAR_BITIS_SAATI { get; set; }
        public int VAR_MOLA_SURESI { get; set; }
        public int VAR_VARDIYA_TIPI_KOD_ID { get; set; }
        public short VAR_VARSAYILAN { get; set; }
        public int VAR_RENK { get; set; }
        public int VAR_OLUSTURAN_ID { get; set; }
        public DateTime? VAR_OLUSTURMA_TARIH { get; set; }
        public int VAR_DEGISTIREN_ID { get; set; }
        public DateTime? VAR_DEGISTIRME_TARIH { get; set; }
    }
}