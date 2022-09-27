using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiNew.Models
{
    public class OlcumGirisData
    {
        public List<Lokasyon> LOKASYONLAR { get; set; }
        public List<OlcumParametre> OLCUM_TANIMLARI { get; set; }
    }
}