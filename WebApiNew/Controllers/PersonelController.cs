using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    
    [MyBasicAuthenticationFilter]
    public class PersonelController : ApiController
    {
        Util klas = new Util();

        public List<Personel> Get()
        {
            List<Personel> listem = new List<Personel>();
            string query = @"SELECT *
,ISNULL(PRS_BIRIM_UCRET,0) AS PRS_UCRET
,ISNULL(PRS_SAAT_UCRET,0) AS PRS_SAATUCRET
,ISNULL(PRS_UCRET_TIPI,250) PRS_UCRETTIPI
,(SELECT COALESCE(TB_RESIM_ID,-1) FROM orjin.TB_RESIM WHERE RSM_VARSAYILAN = 1 AND RSM_REF_GRUP = 'PERSONEL' AND RSM_REF_ID = TB_PERSONEL_ID ) AS PRS_RESIM_ID
,STUFF((SELECT ';' + CONVERT(VARCHAR(11), R.TB_RESIM_ID) FROM orjin.TB_RESIM R WHERE R.RSM_REF_GRUP = 'PERSONEL' AND R.RSM_REF_ID = TB_PERSONEL_ID FOR XML PATH('')), 1, 1, '') AS PRS_RESIM_IDLERI
FROM orjin.VW_PERSONEL WHERE PRS_AKTIF = 1";
            DataTable dt = klas.GetDataTable(query, new List<WebApiNew.Prm>());
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                double birimUcret = Util.getFieldDouble(dt.Rows[i], "PRS_UCRET");
                double saatUcret = Util.getFieldDouble(dt.Rows[i], "PRS_SAATUCRET");
                Personel entity = new Personel();
                entity.PRS_UNVAN = Util.getFieldString(dt.Rows[i], "PRS_UNVAN");
                entity.PRS_PERSONEL_KOD = Util.getFieldString(dt.Rows[i], "PRS_PERSONEL_KOD");
                entity.PRS_ISIM = Util.getFieldString(dt.Rows[i], "PRS_ISIM");
                entity.PRS_LOKASYON = Util.getFieldString(dt.Rows[i], "PRS_LOKASYON");
                entity.PRS_DEPARTMAN = Util.getFieldString(dt.Rows[i], "PRS_DEPARTMAN");
                entity.PRS_TIP = Util.getFieldString(dt.Rows[i], "PRS_TIP");
                entity.PRS_RESIM_IDLERI = Util.getFieldString(dt.Rows[i], "PRS_RESIM_IDLERI");
                entity.TB_PERSONEL_ID = dt.Rows[i]["TB_PERSONEL_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["TB_PERSONEL_ID"]) : 0;
                entity.PRS_RESIM_ID = dt.Rows[i]["PRS_RESIM_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["PRS_RESIM_ID"]) : -1;
                entity.PRS_TEKNISYEN = dt.Rows[i]["PRS_TEKNISYEN"] != DBNull.Value && Convert.ToBoolean(dt.Rows[i]["PRS_TEKNISYEN"]);
                entity.PRS_OPERATOR = dt.Rows[i]["PRS_OPERATOR"] != DBNull.Value && Convert.ToBoolean(dt.Rows[i]["PRS_OPERATOR"]);
                entity.PRS_AKTIF = dt.Rows[i]["PRS_AKTIF"] != DBNull.Value && Convert.ToBoolean(dt.Rows[i]["PRS_AKTIF"]);
                entity.PRS_SURUCU = dt.Rows[i]["PRS_SURUCU"] != DBNull.Value && Convert.ToBoolean(dt.Rows[i]["PRS_SURUCU"]);

                if(dt.Rows[i]["PRS_UCRET_TIPI"] == DBNull.Value)
                    saatUcret = birimUcret / 240;
                else if (Convert.ToInt32(dt.Rows[i]["PRS_UCRET_TIPI"]) == 250)
                    saatUcret = birimUcret;
                else if (Convert.ToInt32(dt.Rows[i]["PRS_UCRET_TIPI"]) == 500)
                    saatUcret = birimUcret / 8;
                else
                    saatUcret = birimUcret / 240;
                entity.PRS_SAAT_UCRET = saatUcret;
                listem.Add(entity);
            }
            return listem;
        }
    }
}
