using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using Dapper;
using Newtonsoft.Json.Linq;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    
    [MyBasicAuthenticationFilter]
    public class IsEmriTipController : ApiController
    {
		Util klas = new Util();
		string query = "";
		SqlCommand cmd = null;

		public List<IsEmriTip> Get()
        {
            string query = @"select * from orjin.TB_ISEMRI_TIP WHERE IMT_AKTIF = 1";
            var util = new Util();
            using (var cnn=util.baglan())
            {
                var liste = cnn.Query<IsEmriTip>(query).ToList();
                return liste;
            }
        }
        [Route("api/IsEmriTipVarsayilan")]
        [HttpGet]
        public IsEmriTip IsEmriTipVarsayilan()
        {
            return this.Get().FirstOrDefault(a => a.IMT_VARSAYILAN == true);
        }


        //Update Is Emri Tipi For Web App Version
        [Route("api/UpdateIsEmriTipi")]
        [HttpPost]
        public Object UpdateIsEmriTipi([FromBody] JObject isEmripTipiBody) 
        {
            JObject isEmriTipleri = isEmripTipiBody ;
            int count = 0;
            query = "";
            try
            {
				if(isEmriTipleri != null && isEmriTipleri.Count > 0) 
                {
					foreach (var entity in isEmriTipleri)
					{
						JObject isEmriTipiKey = isEmriTipleri[entity.Key] as JObject;
                        count = 0;
						query += " update orjin.TB_ISEMRI_TIP set ";
						foreach (var item in isEmriTipiKey)
						{
							if (item.Key.Equals("TB_ISEMRI_TIP_ID")) continue;

							if(item.Value != null && !item.Value.ToString().Equals("")) query += $" {item.Key} = '{item.Value}' ";
                            else query += $" {item.Key} = null ";

							if (count < isEmriTipiKey.Count - 2) query += " , ";

							count++;
						}
						query += $" where TB_ISEMRI_TIP_ID = {isEmriTipiKey["TB_ISEMRI_TIP_ID"]} ";
					}
					using(var cnn = klas.baglan())
                    {
                        cmd = new SqlCommand(query,cnn);
                        cmd.ExecuteNonQuery();
                        klas.baglan().Close();
                    }
					return Json(new { success = " Güncelleme başarılı şekilde gerçekleşti !" });

				} 
                else
                {
					klas.baglan().Close();
					return Json(new { error = " İstemci hatası (Ekleme başarısız) !" });
				}
			}
            catch(Exception ex)
            {
		
				klas.baglan().Close();
				return Json(new { error = ex.ToString() } );
            }
        }


        //Add Is Emri Tip For Web App Version
        [Route("api/AddIsEmriTipi")]
        [HttpGet]
        public Object AddIsEmriTipi([FromUri] string isEmriTipiKey)
        {
            query = $"insert orjin.TB_ISEMRI_TIP (IMT_TANIM,IMT_OLUSTURMA_TARIH,IMT_AKTIF) values('{isEmriTipiKey}','{DateTime.Now}',1)";
            try
            {
				using (var cnn = klas.baglan())
				{

					cmd = new SqlCommand(query, cnn);
					cmd.ExecuteNonQuery();
					klas.baglan().Close();
				}
                return Json(new { success = "Ekleme Başarılı !" });
			} 
            catch(Exception e) 
            {
				return Json(new { error = "Ekleme Başarısız !" });
			}
        }

	}
}
