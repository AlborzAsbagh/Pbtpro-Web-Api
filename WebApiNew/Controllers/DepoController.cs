using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    
    [MyBasicAuthenticationFilter]
    public class DepoController : ApiController
    {
        Util klas = new Util();
        public static List<Prm> parametreler = new List<Prm>();
        [Route("api/Depo/{ID}")]
        [HttpGet]
        public List<Depo> Get([FromUri] int ID, [FromUri] int DEP_MODUL_NO)
        {
            List<Depo> listem = new List<Depo>();
            parametreler.Clear();
            parametreler.Add(new Prm("TB_KULLANICI_ID",ID));
            parametreler.Add(new Prm("DEP_MODUL_NO", DEP_MODUL_NO));
            string query = @"select d.*,STK_BIRIM as DEP_STOK_BIRIM from orjin.TB_DEPO d left join orjin.VW_STOK s on s.TB_STOK_ID = d.DEP_STOK_ID where orjin.UDF_DEPO_YETKI_KONTROL(TB_DEPO_ID,@TB_KULLANICI_ID)=1 AND DEP_MODUL_NO= @DEP_MODUL_NO";
            DataTable dt = klas.GetDataTable(query,parametreler);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Depo entity = new Depo();
                entity.DEP_KOD          = dt.Rows[i]["DEP_KOD"].ToString();
                entity.TB_DEPO_ID       = Convert.ToInt32(dt.Rows[i]["TB_DEPO_ID"]);
                entity.DEP_ATOLYE_ID    = Util.getFieldInt(dt.Rows[i],"DEP_ATOLYE_ID");
                entity.DEP_LOKASYON_ID  = Util.getFieldInt(dt.Rows[i],"DEP_LOKASYON_ID");
                //entity.DEP_KAPASITE   = dt.Rows[i]["DEP_KAPASITE"] != DBNull.Value ? Convert.ToDouble(dt.Rows[i]["DEP_KAPASITE"]) : 0;
                //entity.DEP_KRITIK_MIKTAR = Convert.ToDouble(dt.Rows[i]["DEP_KRITIK_MIKTAR"]);
                entity.DEP_TANIM        = Util.getFieldString(dt.Rows[i],"DEP_TANIM");
                entity.DEP_STOK_BIRIM   = Util.getFieldString(dt.Rows[i],"DEP_STOK_BIRIM");
                entity.DEP_AKTIF        = Util.getFieldBool(dt.Rows[i],"DEP_AKTIF");
                entity.DEP_MODUL_NO     = Util.getFieldInt(dt.Rows[i],"DEP_MODUL_NO");
                if (DEP_MODUL_NO == 2)
                {
                    parametreler.Clear();
                    parametreler.Add(new Prm("TB_DEPO_ID", entity.TB_DEPO_ID));
                    entity.DEP_MIKTAR   = Convert.ToDouble(klas.GetDataCell("select coalesce(SUM(DPS_MIKTAR),0) from orjin.TB_DEPO_STOK where DPS_DEPO_ID = @TB_DEPO_ID",parametreler));
                }

                listem.Add(entity);
            }
            return listem;
        }
        [Route("api/Depo/{ID}")]
        [HttpGet]
        public Depo GetDepoById([FromUri] int ID, [FromUri] int depoId, [FromUri] int modulNo)
        {
            parametreler.Clear();
            parametreler.Add(new Prm("TB_DEPO_ID", depoId));
            parametreler.Add(new Prm("KULLANICI_ID",ID));
            string query = @"select d.*,STK_BIRIM as DEP_STOK_BIRIM from orjin.TB_DEPO d left join orjin.VW_STOK s on s.TB_STOK_ID = d.DEP_STOK_ID where orjin.UDF_DEPO_YETKI_KONTROL(TB_DEPO_ID,@KULLANICI_ID)=1 AND d.TB_DEPO_ID= @TB_DEPO_ID";
            DataRow dtr = klas.GetDataRow(query,parametreler);
            if (dtr != null)
            { 
            Depo entity = new Depo();
            entity.DEP_KOD = dtr["DEP_KOD"].ToString();
            entity.TB_DEPO_ID = Convert.ToInt32(dtr["TB_DEPO_ID"]);
            entity.DEP_ATOLYE_ID = Util.getFieldInt(dtr, "DEP_ATOLYE_ID");
            entity.DEP_LOKASYON_ID = Util.getFieldInt(dtr, "DEP_LOKASYON_ID");
            //entity.DEP_KAPASITE   = dtr["DEP_KAPASITE"] != DBNull.Value ? Convert.ToDouble(dtr["DEP_KAPASITE"]) : 0;
            //entity.DEP_KRITIK_MIKTAR = Convert.ToDouble(dtr["DEP_KRITIK_MIKTAR"]);
            entity.DEP_TANIM = Util.getFieldString(dtr, "DEP_TANIM");
            entity.DEP_STOK_BIRIM = Util.getFieldString(dtr, "DEP_STOK_BIRIM");
            entity.DEP_AKTIF = Util.getFieldBool(dtr, "DEP_AKTIF");
            entity.DEP_MODUL_NO = Util.getFieldInt(dtr, "DEP_MODUL_NO");
            if (modulNo == 2)
            {
                    parametreler.Clear();
                    parametreler.Add(new Prm("DEPO_ID", entity.TB_DEPO_ID));
                    entity.DEP_MIKTAR = Convert.ToDouble(klas.GetDataCell("select coalesce(SUM(DPS_MIKTAR),0) from orjin.TB_DEPO_STOK where DPS_DEPO_ID = @DEPO_ID",parametreler));
            }

            return entity;
            }
            return null;
        }
        [Route("api/YakitTank/{ID}")]
        [HttpGet]
        public List<Depo> GetYakitTank([FromUri] int ID, [FromUri] int makineId)
        {
            List<Depo> listem = new List<Depo>();
            parametreler.Clear();
            parametreler.Add(new Prm("KULLANICI_ID",ID));
            parametreler.Add(new Prm("TB_MAKINE_ID", makineId));
            string query = @"select 
                d.*,
                STK_BIRIM as DEP_STOK_BIRIM,                                
                (select coalesce(SUM(DPS_MIKTAR),0) from orjin.TB_DEPO_STOK where DPS_DEPO_ID = d.TB_DEPO_ID) AS DEP_MIKTAR
                from orjin.TB_DEPO d left join orjin.VW_STOK s on s.TB_STOK_ID = d.DEP_STOK_ID where orjin.UDF_DEPO_YETKI_KONTROL(TB_DEPO_ID,@KULLANICI_ID)=1 AND DEP_MODUL_NO= 2 AND TB_DEPO_ID IN(SELECT DPS_DEPO_ID FROM orjin.TB_DEPO_STOK WHERE DPS_STOK_ID = (select MKN_YAKIT_TIP_ID from orjin.TB_MAKINE where TB_MAKINE_ID = @TB_MAKINE_ID))";
            DataTable dt = klas.GetDataTable(query,parametreler);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Depo entity = new Depo();
                entity.DEP_KOD = dt.Rows[i]["DEP_KOD"].ToString();
                entity.TB_DEPO_ID = Convert.ToInt32(dt.Rows[i]["TB_DEPO_ID"]);
                entity.DEP_ATOLYE_ID = Util.getFieldInt(dt.Rows[i], "DEP_ATOLYE_ID");
                entity.DEP_LOKASYON_ID = Util.getFieldInt(dt.Rows[i], "DEP_LOKASYON_ID");
                entity.DEP_TANIM = Util.getFieldString(dt.Rows[i], "DEP_TANIM");
                entity.DEP_STOK_BIRIM = Util.getFieldString(dt.Rows[i], "DEP_STOK_BIRIM");
                entity.DEP_AKTIF = Util.getFieldBool(dt.Rows[i], "DEP_AKTIF");
                entity.DEP_MODUL_NO = Util.getFieldInt(dt.Rows[i], "DEP_MODUL_NO");
                entity.DEP_MIKTAR = Util.getFieldDouble(dt.Rows[i], "DEP_MIKTAR");

                listem.Add(entity);
            }
            return listem;
        }

        [Route("api/DepoStok")]
        [HttpGet]
        public List<DepoStok> DepoStok([FromUri] int depoID, [FromUri] int stokID, [FromUri] Boolean stoklu)
        {
            List<DepoStok> listem = new List<DepoStok>();
            if (stoklu)
            {
                parametreler.Clear();
                string query = @"Select ds.*,s.STK_TIP_KOD_ID,s.STK_BIRIM_KOD_ID,s.STK_KOD,s.STK_GIRIS_FIYAT_DEGERI,s.STK_TIP,s.STK_SINIF,s.STK_BIRIM AS DPS_STOK_BIRIM from orjin.VW_DEPO_STOK ds left join orjin.VW_STOK s on s.TB_STOK_ID = DPS_STOK_ID where s.STK_AKTIF = 1 ";
                if (depoID != -1)
                {
                    query = query + " and DPS_DEPO_ID = @TB_DEPO_ID" ;
                    parametreler.Add(new Prm("TB_DEPO_ID", depoID));
                }
                if (stokID != -1)
                {
                    query = query + " and DPS_STOK_ID = @STOK_ID";
                    parametreler.Add(new Prm("STOK_ID", stokID));
                }
                DataTable dt = klas.GetDataTable(query,parametreler);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DepoStok entity = new DepoStok();
                    entity.DPS_STOK_KOD                 = dt.Rows[i]["STK_KOD"].ToString();
                    entity.DPS_CIKAN_MIKTAR             = Util.getFieldDouble(dt.Rows[i],"DPS_CIKAN_MIKTAR");
                    entity.DPS_GIREN_MIKTAR             = Util.getFieldDouble(dt.Rows[i],"DPS_GIREN_MIKTAR");
                    entity.DPS_KULLANILABILIR_MIKTAR    = Util.getFieldDouble(dt.Rows[i],"DPS_KULLANILABILIR_MIKTAR");
                    entity.DPS_MIKTAR                   = Util.getFieldDouble(dt.Rows[i],"DPS_MIKTAR");
                    entity.DPS_STOK                     = Util.getFieldString(dt.Rows[i],"DPS_STOK");
                    entity.DPS_STOK_ID                  = Util.getFieldInt(dt.Rows[i],"DPS_STOK_ID");
                    entity.DPS_DEPO                     = Util.getFieldString(dt.Rows[i],"DPS_DEPO");
                    entity.DPS_DEPO_ID                  = Util.getFieldInt(dt.Rows[i],"DPS_DEPO_ID");
                    entity.DPS_MALZEME_TIP_ID           = Util.getFieldInt(dt.Rows[i],"STK_TIP_KOD_ID");
                    entity.DPS_MALZEME_TIP              = Util.getFieldString(dt.Rows[i],"STK_TIP");
                    entity.DPS_STOK_BIRIM               = Util.getFieldString(dt.Rows[i],"DPS_STOK_BIRIM");                  
                    entity.DPS_BIRIM_ID                 = Util.getFieldInt(dt.Rows[i],"STK_BIRIM_KOD_ID");
                    entity.TB_DEPO_STOK_ID              = Util.getFieldInt(dt.Rows[i],"TB_DEPO_STOK_ID");
                    entity.DPS_STOK_SINIF               = Util.getFieldString(dt.Rows[i],"STK_SINIF");
                    entity.DPS_FIYAT                    = Util.getFieldDouble(dt.Rows[i],"STK_GIRIS_FIYAT_DEGERI");
                    listem.Add(entity);
                }
            }
            else
            {
                string query = @"Select * from orjin.VW_STOK where STK_AKTIF =1";
                parametreler.Clear();
                DataTable dt = klas.GetDataTable(query,parametreler);

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    DepoStok entity = new DepoStok();
                    entity.DPS_STOK_ID          = Convert.ToInt32(dt.Rows[i]["TB_STOK_ID"]);
                    entity.DPS_CIKAN_MIKTAR             = Util.getFieldDouble(dt.Rows[i], "STK_CIKAN_MIKTAR");
                    entity.DPS_GIREN_MIKTAR             = Util.getFieldDouble(dt.Rows[i], "STK_GIREN_MIKTAR");
                    entity.DPS_FIYAT                    = Util.getFieldDouble(dt.Rows[i], "STK_GIRIS_FIYAT_DEGERI");
                    entity.DPS_KULLANILABILIR_MIKTAR    = Util.getFieldDouble(dt.Rows[i], "STK_KULLANILABILIR_MIKTAR");
                    entity.DPS_MIKTAR                   = Util.getFieldDouble(dt.Rows[i], "STK_MIKTAR");
                    entity.DPS_MALZEME_TIP_ID           = Util.getFieldInt(dt.Rows[i],"STK_TIP_KOD_ID");
                    entity.DPS_BIRIM_ID                 = Util.getFieldInt(dt.Rows[i],"STK_BIRIM_KOD_ID");
                    entity.DPS_STOK                     = Util.getFieldString(dt.Rows[i],"STK_TANIM");
                    entity.DPS_STOK_KOD                 = Util.getFieldString(dt.Rows[i],"STK_KOD");
                    entity.DPS_MALZEME_TIP              = Util.getFieldString(dt.Rows[i],"STK_TIP");
                    entity.DPS_STOK_BIRIM               = Util.getFieldString(dt.Rows[i],"STK_BIRIM");
                    entity.DPS_STOK_SINIF               = Util.getFieldString(dt.Rows[i], "STK_SINIF");
                    entity.DPS_DEPO = "";
                    entity.DPS_DEPO_ID = -1;
                    entity.TB_DEPO_STOK_ID = -1;
                    listem.Add(entity);
                }
            }
            return listem;
        }
    }
}
