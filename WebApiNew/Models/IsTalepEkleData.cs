using System.Collections.Generic;

namespace WebApiNew.Models
{
    public class IsTalepEkleData
    {
        public string IsTalepKod { get; set; }
        public IsTalepParametre Parametre { get; set; }
        public List<Lokasyon> Lokasyonlar { get; set; }
        public List<TalepKullanici> TalepKullanicilari { get; set; }
        public List<Kod> IsTalepTipleri { get; set; }
    }
}