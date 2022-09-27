using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    [MyBasicAuthenticationFilter]
    public class PeriyodikBakimController : ApiController
    {
        Util klas = new Util();
        Parametreler prms = new Parametreler();
        [Route("api/PeriyodikBakimGetir")]
        [HttpGet]
        public List<PeriyodikBakim> PeriyodikBakimGetirByMakine(int makineID)
        {
            prms.Clear();
            prms.Add("MAK_ID", makineID);
            string query = @"select *,(select COUNT(TB_ISEMRI_ID) from orjin.TB_ISEMRI where ISM_REF_GRUP='PERİYODİK BAKIM' and ISM_REF_ID = TB_PERIYODIK_BAKIM_ID and ISM_KAPATILDI=0 and ISM_MAKINE_ID= @MAK_ID) as PBK_ISEMRI from orjin.TB_PERIYODIK_BAKIM where TB_PERIYODIK_BAKIM_ID IN (SELECT PBM_PERIYODIK_BAKIM_ID FROM orjin.TB_PERIYODIK_BAKIM_MAKINE WHERE PBM_MAKINE_ID = @MAK_ID)";
            DataTable dt = klas.GetDataTable(query, prms.PARAMS);
            List<PeriyodikBakim> listem = new List<PeriyodikBakim>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                PeriyodikBakim entity = new PeriyodikBakim();
                entity.TB_PERIYODIK_BAKIM_ID = (int)dt.Rows[i]["TB_PERIYODIK_BAKIM_ID"];
                entity.PBK_TANIM = Util.getFieldString(dt.Rows[i], "PBK_TANIM");
                entity.PBK_AKTIF = Util.getFieldBool(dt.Rows[i], "PBK_AKTIF");
                entity.PBK_GRUP_KOD_ID = Util.getFieldInt(dt.Rows[i], "PBK_GRUP_KOD_ID");
                entity.PBK_KOD = Util.getFieldString(dt.Rows[i], "PBK_KOD");
                entity.PBK_TIP_KOD_ID = Util.getFieldInt(dt.Rows[i], "PBK_TIP_KOD_ID");
                entity.PBK_ISEMRI_VAR = Util.getFieldInt(dt.Rows[i], "PBK_ISEMRI") > 0 ? true : false;
                listem.Add(entity);
            }
            return listem;
        }

        [Route("api/PBakimKontrolList")]
        [HttpGet]
        public List<IsEmriKontrolList> PBakimKontrolList(int bakimID)
        {
            prms.Clear();
            prms.Add("PKN_PERIYODIK_BAKIM_ID", bakimID);
            string query = @"select * from orjin.TB_PERIYODIK_BAKIM_KONTROLLIST where PKN_PERIYODIK_BAKIM_ID = @PKN_PERIYODIK_BAKIM_ID";
            DataTable dt = klas.GetDataTable(query, prms.PARAMS);
            List<IsEmriKontrolList> listem = new List<IsEmriKontrolList>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //PBakimKontrolList entity = new PBakimKontrolList();
                //entity.TB_PERIYODIK_BAKIM_KONTROLLIST_ID = (int)dt.Rows[i]["TB_PERIYODIK_BAKIM_KONTROLLIST_ID"];
                //entity.PKN_TANIM = dt.Rows[i]["PKN_TANIM"].ToString();
                //entity.PKN_SIRANO = Convert.ToString(dt.Rows[i]["PKN_SIRANO"]);
                //listem.Add(entity);

                IsEmriKontrolList entity = new IsEmriKontrolList();
                entity.DKN_TANIM = Util.getFieldString(dt.Rows[i], "PKN_TANIM");
                entity.DKN_SIRANO = Util.getFieldString(dt.Rows[i], "PKN_SIRANO");
                entity.DKN_REF_ID = bakimID;
                entity.DKN_YAPILDI_SAAT = "0";
                entity.DKN_YAPILDI_PERSONEL_ID = -1;
                entity.DKN_YAPILDI_MESAI_KOD_ID = -1;
                entity.DKN_YAPILDI_ATOLYE_ID = -1;
                entity.DKN_YAPILDI_SURE = 0;
                listem.Add(entity);

            }
            return listem;
        }
        [Route("api/PBakimMazleme")]
        [HttpGet]
        public List<IsEmriMalzeme> PBakimMazleme(int bakimID)
        {
            prms.Clear();
            prms.Add("PBM_PERIYODIK_BAKIM_ID", bakimID);
            string query = @"select *,(select top 1 STK_KOD from orjin.TB_STOK where TB_STOK_ID = PBM_STOK_ID) as IDM_STOK_KOD, orjin.UDF_KOD_TANIM(PBM_BIRIM_KOD_ID) as PBM_BIRIM from orjin.TB_PERIYODIK_BAKIM_MLZ where PBM_PERIYODIK_BAKIM_ID = @PBM_PERIYODIK_BAKIM_ID";
            DataTable dt = klas.GetDataTable(query, prms.PARAMS);
            List<IsEmriMalzeme> listem = new List<IsEmriMalzeme>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IsEmriMalzeme entity = new IsEmriMalzeme();
                entity.IDM_BIRIM_KOD_ID = Util.getFieldInt(dt.Rows[i], "PBM_BIRIM_KOD_ID");
                entity.IDM_STOK_ID = Util.getFieldInt(dt.Rows[i], "PBM_STOK_ID");
                entity.IDM_BIRIM_FIYAT = Util.getFieldDouble(dt.Rows[i], "PBM_BIRIM_FIYAT");
                entity.IDM_MIKTAR = Util.getFieldDouble(dt.Rows[i], "PBM_MIKTAR");
                entity.IDM_TUTAR = Util.getFieldDouble(dt.Rows[i], "PBM_TUTAR");
                entity.IDM_BIRIM = Util.getFieldString(dt.Rows[i], "PBM_BIRIM");
                entity.IDM_STOK_KOD = Util.getFieldString(dt.Rows[i], "IDM_STOK_KOD");
                entity.IDM_STOK_TANIM = Util.getFieldString(dt.Rows[i], "PBM_STOK_TANIM");
                entity.IDM_ALTERNATIF_STOK_ID = -1;
                entity.IDM_DEPO_ID = -1;
                entity.IDM_REF_ID = bakimID;
                entity.IDM_STOK_DUS = entity.IDM_STOK_ID == -1 ? false : true;
                entity.IDM_MALZEME_STOKTAN = entity.IDM_STOK_ID == -1 ? "Düşmesin" : "Düşsün";
                listem.Add(entity);
            }
            return listem;
        }
    }
}
