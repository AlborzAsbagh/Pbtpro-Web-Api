﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    
    [JwtAuthenticationFilter]
    public class EkipmanController : ApiController
    {
        Util klas = new Util();
        List<Prm> prms = new List<Prm>();
        public List<Ekipman> Get([FromUri] int MakineID)
        {
            prms.Clear();
            prms.Add(new Prm("MAKINE_ID",MakineID));
            string query = @"select * from (select *,ROW_NUMBER() OVER(ORDER BY MKA_TABLO_UST_ID) AS satir from orjin.UDF_MAKINE_AGAC(@MAKINE_ID,'MAKINE') UMA WHERE UMA.MKA_TIP <> 'SAYAC' AND UMA.MKA_TIP <> 'MAKINE'  ) as tablom ";
            DataTable dt = klas.GetDataTable(query,prms);
            List<Ekipman> listem = new List<Ekipman>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Ekipman entity = new Ekipman();
                entity.EKP_KOD = Util.getFieldString(dt.Rows[i],"MKA_KOD");
                entity.EKP_TANIM = Util.getFieldString(dt.Rows[i],"MKA_TANIM");
                entity.EKP_REF_GRUP = Util.getFieldString(dt.Rows[i],"MKA_TIP");
                entity.EKP_REF_ID    = Util.getFieldInt(dt.Rows[i],"MKA_TABLO_UST_ID");
                entity.TB_EKIPMAN_ID = Util.getFieldInt(dt.Rows[i],"MKA_TABLO_ID");
                listem.Add(entity);
            }
            return listem;
        }

        [Route("api/GetEkipmanFullList")]
        [HttpGet]
		public Object GetEkipmanFullList([FromUri] int? MakineID)
		{
            string query = "";
			List<Ekipman> listem = new List<Ekipman>();
			try
            {
				if(MakineID > 0 && MakineID!=null) 
                {
					query = $"select * from orjin.TB_EKIPMAN where EKP_MAKINE_ID = {MakineID}";
					using (var cnn = klas.baglan())
					{
						listem = cnn.Query<Ekipman>(query).ToList();
						klas.kapat();
					}
					return Json(new { ekipmanListe = listem });
				} 
                else
                {
					query = $"select * from orjin.TB_EKIPMAN ";
					
					using (var cnn = klas.baglan())
					{
						listem = cnn.Query<Ekipman>(query).ToList();
						klas.kapat();
					}
					return Json(new { ekipmanListe = listem });
				}
			} 
            catch(Exception e)
            {
				return Json(new { error = e.Message });

			}

		}
	}
}
