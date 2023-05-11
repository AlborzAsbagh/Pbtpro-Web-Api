using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    [MyBasicAuthenticationFilter]
    public class TalepKullaniciController : ApiController
    {
        Util klas = new Util();

        public List<TalepKullanici> Get()
        {
            List<TalepKullanici> listem = new List<TalepKullanici>();
            string query = @"select * from orjin.TB_IS_TALEBI_KULLANICI";
            DataTable dt = klas.GetDataTable(query, new List<Prm>());
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                TalepKullanici entity = new TalepKullanici();
                entity.ISK_ISIM = Util.getFieldString(dt.Rows[i],"ISK_ISIM");
                entity.ISK_KOD  = Util.getFieldString(dt.Rows[i],"ISK_KOD" );
                entity.TB_IS_TALEBI_KULLANICI_ID = Convert.ToInt32(dt.Rows[i]["TB_IS_TALEBI_KULLANICI_ID"]);
                entity.ISK_LOKASYON_ID = Util.getFieldInt(dt.Rows[i],"ISK_LOKASYON_ID");
                entity.ISK_PERSONEL_ID = Util.getFieldInt(dt.Rows[i], "ISK_PERSONEL_ID");
                entity.ISK_MAIL = Util.getFieldString(dt.Rows[i], "ISK_MAIL");
                entity.ISK_TELEFON_1 = Util.getFieldString(dt.Rows[i], "ISK_TELEFON_1");
                listem.Add(entity);
            }
            return listem;
        }
    }
}
