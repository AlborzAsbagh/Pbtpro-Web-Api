using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using WebApiNew.Filters;

namespace WebApiNew.Controllers
{
	[MyBasicAuthenticationFilter]
	public class YetkiController : ApiController
	{
		Util klas = new Util();
		Parametreler prms = new Parametreler();

		public object isAuthorizedToAdd(int id,int pageCode)
		{
			try
			{
				prms.Clear();
				prms.Add("KYT_KULLANICI_ID", id);
				prms.Add("KYT_YETKI_KOD", pageCode);
				DataTable dt = klas.GetDataTable(Queries.KLL_EKLE_YETKISI, prms.PARAMS);
				return dt.Rows[0]["KYT_EKLE"];
			}
			catch (Exception ex) 
			{
				return ex.Message;
			}
		}

		public object isAuthorizedToUpdate(int id, int pageCode)
		{
			try
			{
				prms.Clear();
				prms.Add("KYT_KULLANICI_ID", id);
				prms.Add("KYT_YETKI_KOD", pageCode);
				DataTable dt = klas.GetDataTable(Queries.KLL_GUNCELLE_YETKISI, prms.PARAMS);
				return dt.Rows[0]["KYT_DEGISTIR"];
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}

		public object isAuthorizedToDelete(int id, int pageCode)
		{
			try
			{
				prms.Clear();
				prms.Add("KYT_KULLANICI_ID", id);
				prms.Add("KYT_YETKI_KOD", pageCode);
				DataTable dt = klas.GetDataTable(Queries.KLL_SIL_YETKISI, prms.PARAMS);
				return dt.Rows[0]["KYT_SIL"];
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
		}
	}
}
