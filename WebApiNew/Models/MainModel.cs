using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiNew.Models
{
    public class MainModel
    {
        public int KUL_ID { get; set; }
        public int RESIM_ID { get; set; }
        public string DB_NAME { get; set; }
        public string MASTER_DB_NAME { get; set; }
        public string AIS_TALEP { get; set; }
        public string KIS_TALEP { get; set; }
        public string AIS_EMRI { get; set; }
        public string KIS_EMRI { get; set; }
        public string AKTIF_MAKINE { get; set; }
        public string TOPLAM_MAKINE { get; set; }
        public int TOPLAM_ONAY { get; set; }
        public string BMALZEME_TALEBI { get; set; }
        public Personel PERSONEL { get; set; }
        public List<Yetki> YETKILER { get; set; }
        public List<int> YETKILI_LOKASYON_IDLER { get; set; }

    }
}