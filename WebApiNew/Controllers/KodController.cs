using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
		string query = "";
		SqlCommand cmd = null;

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
                entity.KOD_ISM_DURUM_VARSAYILAN = Util.getFieldBool(dt.Rows[i], "KOD_ISM_DURUM_VARSAYILAN");
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


		[Route("api/AddKodList")]
		[HttpPost]

		public Object AddKodList([FromUri] string entity, [FromUri] string grup)
		{
			try
			{
				query = " insert into orjin.TB_KOD (KOD_GRUP , KOD_TANIM , KOD_AKTIF , KOD_GOR , KOD_DEGISTIR , KOD_SIL ) ";
				query += $" values ( '{grup}' , '{entity}' , 1 , 1 , 1 ,1) ";

				using (var con = klas.baglan())
				{
					cmd = new SqlCommand(query, con);
					cmd.ExecuteNonQuery();
				}
				klas.kapat();
				return Json(new { status_code = 201, status = "Added Successfully" });

			}
			catch (Exception ex)
			{
				klas.kapat();
				return Json(new { status_code = 500, status = ex.Message });
			}
		}
	}
}
