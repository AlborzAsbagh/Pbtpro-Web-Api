using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Dapper;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    [MyBasicAuthenticationFilter]
    public class KodController : ApiController
    {
        Util klas = new Util();
        Parametreler prms = new Parametreler();
        [Route("api/KodList")]
        [HttpGet]
        public List<Kod> KodList(string grup)
        {
            prms.Clear();
            prms.Add("KGRP",grup);
            string query = @"select * from orjin.TB_KOD where KOD_GRUP=@KGRP";
            DataTable dt = klas.GetDataTable(query,prms.PARAMS);
            List<Kod> listem = new List<Kod>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Kod entity = new Kod();
                entity.TB_KOD_ID = (int)dt.Rows[i]["TB_KOD_ID"];
                entity.KOD_GRUP = Util.getFieldString(dt.Rows[i],"KOD_GRUP");
                entity.KOD_TANIM = Util.getFieldString(dt.Rows[i],"KOD_TANIM");
                listem.Add(entity);
            }
            return listem;
        }
        [Route("api/KodLists")]
        [HttpGet]
        public Dictionary<string, IEnumerable<Kod>> KodList([FromUri]string[] grups)
        {
            string query = @"SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP=@KGRP";
            var klas = new Util();
            var listem = new Dictionary<string,IEnumerable<Kod>> ();
            using (var cnn=klas.baglan())
            {
                foreach (var grup in grups)
                {
                    listem.Add(grup,cnn.Query<Kod>(query,new {KGRP=grup}));
                }
            }
            
            return listem;
        }
    }
}
