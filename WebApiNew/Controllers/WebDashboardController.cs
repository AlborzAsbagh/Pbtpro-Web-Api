using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using Newtonsoft.Json.Linq;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
	[MyBasicAuthenticationFilter]
	public class WebDashboardController : ApiController
	{
		Util klas = new Util();
		string query = "";
		List<Prm> parametreler = new List<Prm>();
		Parametreler prms = new Parametreler();

		//Get Lokasyon List For Web App ( Lokasyon Sayfasi Icin )
		[Route("api/GetDashboardItems")]
		[HttpGet]
		public WebDashboard GetDashboardItems([FromUri] int ID)
		{
			prms.Clear();
			prms.Add("ISM_ID", ID);
			WebDashboard entity = new WebDashboard();
			query = @"
						  SELECT
						  (SELECT count(*) FROM orjin.TB_ISEMRI WHERE ISM_KAPATILDI = 0) AS ACIK_IS_EMIRLERI,
						  (SELECT count(*) FROM orjin.TB_IS_TALEBI WHERE IST_DURUM_ID != 4 AND IST_DURUM_ID != 5) AS DEVAM_EDEN_IS_TALEPLERI,
						  (SELECT count(*) FROM orjin.TB_STOK stk where stk.STK_MIKTAR < stk.STK_MIN_MIKTAR) AS DUSUK_STOKLU_MALZEMELER  ";

			DataTable dt = klas.GetDataTable(query,prms.PARAMS);
			entity.ACIK_IS_EMIRLERI = Convert.ToInt32(dt.Rows[0]["ACIK_IS_EMIRLERI"]);
			entity.DEVAM_EDEN_IS_TALEPLERI = Convert.ToInt32(dt.Rows[0]["DEVAM_EDEN_IS_TALEPLERI"]);
			entity.DUSUK_STOKLU_MALZEMELER = Convert.ToInt32(dt.Rows[0]["DUSUK_STOKLU_MALZEMELER"]);

			query = " SELECT * FROM orjin.UDF_TIPE_GORE_MAKINE_SAYISI(32501) ";
			dt = klas.GetDataTable(query, prms.PARAMS);

			for(int i = 0; i<dt.Rows.Count; i++)
			{
				entity.MAKINE_TIP_ENVANTER.Add(new MakineTipEnvanteri
					(

					Convert.ToInt32(dt.Rows[i]["TB_KOD_ID"]),
					Convert.ToString(dt.Rows[i]["MAKINE_TIPI"]),
					Convert.ToInt32(dt.Rows[i]["MAKINE_SAYISI"])
					
					));
			}

			return entity;
		}

	}
}
