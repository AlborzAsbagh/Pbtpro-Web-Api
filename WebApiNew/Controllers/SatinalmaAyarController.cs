using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    
    [MyBasicAuthenticationFilter]
    public class SatinalmaAyarController : ApiController
    {
        Util klas = new Util();
        [Route("api/SatinalmaAyarlari")]
        [HttpGet]
        public SatinAlmaAyar SatinalmaAyarlari()
        {
            SatinAlmaAyar entity = new SatinAlmaAyar();
            try
            {
                string query = @"select top 1 * from orjin.TB_SATINALMA_AYAR";
                DataRow drAyar = klas.GetDataRow(query,new List<Prm>());
                entity.TB_SATINALMA_AYAR_ID = (int)drAyar["TB_SATINALMA_AYAR_ID"];
                entity.STA_TALEP_ACIK_RENK                  = Util.getFieldString(drAyar,"STA_TALEP_ACIK_RENK"              );
                entity.STA_TALEP_IPTAL_RENK                 = Util.getFieldString(drAyar,"STA_TALEP_IPTAL_RENK"             );
                entity.STA_TALEP_KAPANDI_RENK               = Util.getFieldString(drAyar,"STA_TALEP_KAPANDI_RENK"           );
                entity.STA_TALEP_ONAYLANDI_RENK             = Util.getFieldString(drAyar,"STA_TALEP_ONAYLANDI_RENK"         );
                entity.STA_TALEP_ONAYLANMADI_RENK           = Util.getFieldString(drAyar,"STA_TALEP_ONAYLANMADI_RENK"       );
                entity.STA_TALEP_ONAY_BEKLIYOR_RENK         = Util.getFieldString(drAyar,"STA_TALEP_ONAY_BEKLIYOR_RENK"     );
                entity.STA_TALEP_SIPARIS_RENK               = Util.getFieldString(drAyar,"STA_TALEP_SIPARIS_RENK"           );
                entity.STA_TALEP_TEKLIF_RENK                = Util.getFieldString(drAyar,"STA_TALEP_TEKLIF_RENK"            );
                entity.STA_TALEP_KARSILANIYOR_RENK          = Util.getFieldString(drAyar,"STA_TALEP_KARSILANIYOR_RENK"      );
                entity.STA_TALEP_ACIK_YAZI_RENK             = Util.getFieldString(drAyar,"STA_TALEP_ACIK_YAZI_RENK"         );
                entity.STA_TALEP_IPTAL_YAZI_RENK            = Util.getFieldString(drAyar,"STA_TALEP_IPTAL_YAZI_RENK"        );
                entity.STA_TALEP_KAPANDI_YAZI_RENK          = Util.getFieldString(drAyar,"STA_TALEP_KAPANDI_YAZI_RENK"      );
                entity.STA_TALEP_ONAYLANDI_YAZI_RENK        = Util.getFieldString(drAyar,"STA_TALEP_ONAYLANDI_YAZI_RENK"    );
                entity.STA_TALEP_ONAYLANMADI_YAZI_RENK      = Util.getFieldString(drAyar,"STA_TALEP_ONAYLANMADI_YAZI_RENK"  );
                entity.STA_TALEP_ONAY_BEKLIYOR_YAZI_RENK    = Util.getFieldString(drAyar,"STA_TALEP_ONAY_BEKLIYOR_YAZI_RENK");
                entity.STA_TALEP_SIPARIS_YAZI_RENK          = Util.getFieldString(drAyar,"STA_TALEP_SIPARIS_YAZI_RENK"      );
                entity.STA_TALEP_TEKLIF_YAZI_RENK           = Util.getFieldString(drAyar,"STA_TALEP_TEKLIF_YAZI_RENK"       );
                entity.STA_TALEP_KARSILANIYOR_YAZI_RENK     = Util.getFieldString(drAyar,"STA_TALEP_KARSILANIYOR_YAZI_RENK" );
                return entity;
            }

            catch (Exception)
            {
                return entity;
            }
        }
    }
}
