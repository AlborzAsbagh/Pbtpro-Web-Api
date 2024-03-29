using Dapper;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using WebApiNew.Filters;
using WebApiNew.Models;


namespace WebApiNew.Controllers
{
	[JwtAuthenticationFilter]
	public class FormRaporController : Controller
	{
		Util klas = new Util();
		YetkiController yetki = new YetkiController();
		string query = "";

		public ActionResult IsEmriFormByIdToPdf(long id)
		{
			try
			{
				var entity = GetIsEmriFormById(id);
				if (entity != null && entity is WebVersionIsEmriForm)
				{
					return new ViewAsPdf("IsEmriFormByIdToPdf", entity)
					{
						FileName = "IsEmriForm.pdf"
					};
				}
				else
				{
					return HttpNotFound();
				}
			}
			catch (Exception ex)
			{
				return HttpNotFound(ex.Message);
			}
		}


		public object GetIsEmriFormById([FromUri] long id)
		{
			List<WebVersionIsEmriForm> listem = new List<WebVersionIsEmriForm>();
			Bildirim bldr = new Bildirim();
			try
			{
				query = @" SELECT * FROM orjin.UDF_WEB_APP_ISEMRI_FORM_BY_ID (@ISEMRI_ID) ";
				using (var cnn = klas.baglan())
				{
					listem = cnn.Query<WebVersionIsEmriForm>(query, new { @ISEMRI_ID = id }).ToList();
				}
				return listem[0];
			}

			catch (Exception e)
			{
				bldr.Error = true;
				bldr.Aciklama = e.Message;
				return bldr;
			}
		}

	}
}
