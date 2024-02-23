using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
        public async Task<Object> UpdateIsEmriTipi([FromBody] JObject isEmripTipiBody) 
        {
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (isEmripTipiBody != null && isEmripTipiBody.Count > 0 && Convert.ToInt32(isEmripTipiBody.GetValue("TB_ISEMRI_TIP_ID")) >= 1)
					{
						query = " update orjin.TB_ISEMRI_TIP set ";
						foreach (var item in isEmripTipiBody)
						{

							if (item.Key.Equals("TB_ISEMRI_TIP_ID")) continue;

							if (count < isEmripTipiBody.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , IMT_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ";
						query += $" where TB_ISEMRI_TIP_ID = {Convert.ToInt32(isEmripTipiBody.GetValue("TB_ISEMRI_TIP_ID"))}";

						await cnn.ExecuteAsync(query);

					}
					else return Json(new { has_error = true, status_code = 400, status = "Missing coming data." });

				}
				return Json(new { has_error = false, status_code = 200, status = "Entity has updated successfully." });
			}
			catch (Exception e)
			{

				return Json(new { has_error = true, status_code = 500, status = e.Message });
			}
		}


        //Add Is Emri Tip For Web App Version
        [Route("api/AddIsEmriTipi")]
        [HttpGet]
        public Object AddIsEmriTipi([FromUri] string isEmriTipiKey)
        {
            //Default olarak bu alanlar 'false' olmalidir web'de hata verdigi icin. Sebep Belirsiz :( 
            query = @"insert into orjin.TB_ISEMRI_TIP (IMT_TANIM,
                    IMT_RENK,
                    IMT_MALZEME_FIYAT_TIP,
                    IMT_VARSAYILAN_MALZEME_MIKTAR,
                    IMT_ONCELIK,
                    IMT_FIRMA,
                    IMT_MAKINE_DURUM_DETAY,
                    IMT_SOZLESME,
                    IMT_SAYAC_DEGERI,
                    IMT_KONU,IMT_PLAN_BITIS,
                    IMT_PERSONEL_SURE,
                    IMT_REFERANS_NO,
                    IMT_EVRAK_NO,
                    IMT_EVRAK_TARIHI,
                    IMT_MALIYET,
                    IMT_ACIKLAMA_USTTAB,
                    IMT_OZEL_ALAN_1,
                    IMT_OZEL_ALAN_2,
                    IMT_OZEL_ALAN_3,
                    IMT_OZEL_ALAN_11,
                    IMT_OZEL_ALAN_12,
                    IMT_OZEL_ALAN_16,
                    IMT_OZEL_ALAN_17,
                    IMT_MAKINE_KAPAT,
                    IMT_EKIPMAN_KAPAT,
                    IMT_MAKINE_DURUM_KAPAT,
                    IMT_SAYAC_DEGER_KAPAT,
                    IMT_PROSEDUR_KAPAT,
                    IMT_IS_TIPI_KAPAT,
                    IMT_IS_NEDENI_KAPAT,
                    IMT_KONU_KAPAT,
                    IMT_ONCELIK_KAPAT,
                    IMT_ATOLYE_KAPAT,
                    IMT_PROJE_KAPAT,
                    IMT_REFNO_KAPAT,
                    IMT_FIRMA_KAPAT,
                    IMT_SOZLESME_KAPAT,
                    IMT_OZEL_ALAN_13,
                    IMT_AKTIF,
                    IMT_TOPLAM_MALIYET_ZORUNLU) values"; 
            query += $"('{isEmriTipiKey}',0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0)";

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
				return Json(new { error = e.Message });
			}
        }

	}
}
