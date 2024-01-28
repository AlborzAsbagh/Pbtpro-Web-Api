using Dapper;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    
    [MyBasicAuthenticationFilter]
    public class PersonelController : ApiController
    {
        Util klas = new Util();
        string query = "";

        public List<Personel> Get([FromUri] int? lokasyonId = 0 , [FromUri] int? atolyeId = 0 , [FromUri] int? personelRol = 0)
        {
            List<Personel> listem = new List<Personel>();
            string query = @"SELECT *
                        ,ISNULL(PRS_BIRIM_UCRET,0) AS PRS_UCRET
                        ,ISNULL(PRS_SAAT_UCRET,0) AS PRS_SAATUCRET
                        ,ISNULL(PRS_UCRET_TIPI,250) PRS_UCRETTIPI
                        ,(SELECT COALESCE(TB_RESIM_ID,-1) FROM orjin.TB_RESIM WHERE RSM_VARSAYILAN = 1 AND RSM_REF_GRUP = 'PERSONEL' AND RSM_REF_ID = TB_PERSONEL_ID ) AS PRS_RESIM_ID
                        ,STUFF((SELECT ';' + CONVERT(VARCHAR(11), R.TB_RESIM_ID) FROM orjin.TB_RESIM R WHERE R.RSM_REF_GRUP = 'PERSONEL' AND R.RSM_REF_ID = TB_PERSONEL_ID FOR XML PATH('')), 1, 1, '') AS PRS_RESIM_IDLERI
                        FROM orjin.VW_PERSONEL WHERE PRS_AKTIF = 1 ";

			if (lokasyonId > 0 || atolyeId > 0 || personelRol > 0) query += getPersonelWhereQuery(personelRol, lokasyonId, atolyeId);


            using(var cnn = klas.baglan())
            {
                listem = cnn.Query<Personel>(query).ToList();

            }
            //for (int i = 0; i < listem.Count; i++)
            //{
            //    double birimUcret = Util.getFieldDouble(dt.Rows[i], "PRS_UCRET");
            //    double saatUcret = Util.getFieldDouble(dt.Rows[i], "PRS_SAATUCRET");
            //    if(dt.Rows[i]["PRS_UCRET_TIPI"] == DBNull.Value)
            //        saatUcret = birimUcret / 240;
            //    else if (Convert.ToInt32(dt.Rows[i]["PRS_UCRET_TIPI"]) == 250)
            //        saatUcret = birimUcret;
            //    else if (Convert.ToInt32(dt.Rows[i]["PRS_UCRET_TIPI"]) == 500)
            //        saatUcret = birimUcret / 8;
            //    else
            //        saatUcret = birimUcret / 240;
            //    entity.PRS_SAAT_UCRET = saatUcret;
            //    listem.Add(entity);
            //}
            return listem;
        }

        [HttpGet]
        [Route("api/GetPersonelWhereQuery")]
        public string getPersonelWhereQuery(int? personelRol = 0 , int? lokasyonId = 0 , int? atolyeId = 0 )
        {
            string where = "";

			if (lokasyonId != null && lokasyonId > 0) where += $" and PRS_LOKASYON_ID = {lokasyonId} ";
			if (atolyeId != null && atolyeId > 0) where += $" and PRS_ATOLYE_ID = {atolyeId} ";
            if (personelRol != null && personelRol > 0)
            {
                switch (personelRol)
                {
                    case 1:
						where += @" and PRS_TEKNISYEN = 1 or (
						( PRS_TEKNISYEN = 0 and PRS_SURUCU = 0 and PRS_OPERATOR = 0 and PRS_BAKIM = 0 and PRS_SANTIYE = 0 ) ) ";
						break;
                    case 2:
						where += @" and PRS_SURUCU = 1 or (
						( PRS_TEKNISYEN = 0 and PRS_SURUCU = 0 and PRS_OPERATOR = 0 and PRS_BAKIM = 0 and PRS_SANTIYE = 0 )  ) ";
						break;
                    case 3:
                        where += @" and PRS_OPERATOR = 1 or (
						( PRS_TEKNISYEN = 0 and PRS_SURUCU = 0 and PRS_OPERATOR = 0 and PRS_BAKIM = 0 and PRS_SANTIYE = 0 )  ) ";
						break;
                    case 4:
						where += @" and PRS_BAKIM = 1 or (
						( PRS_TEKNISYEN = 0 and PRS_SURUCU = 0 and PRS_OPERATOR = 0 and PRS_BAKIM = 0 and PRS_SANTIYE = 0 )  ) ";
						break;
                    case 5:
						where += @" and PRS_SANTIYE = 1 or (
						( PRS_TEKNISYEN = 0 and PRS_SURUCU = 0 and PRS_OPERATOR = 0 and PRS_BAKIM = 0 and PRS_SANTIYE = 0 )  ) ";
						break;
                    default:
                        where += " and PRS_DIGER = 1";
                        break;
                }
            }
            else where += " and 1=1 ";
            return where;
	
        }


        [HttpPost]
        [Route("api/AddPersonel")]
        public async Task<object> AddPersonel([FromBody] JObject entity) 
        {
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0)
					{
						query = " insert into orjin.TB_PERSONEL  ( PRS_OLUSTURMA_TARIH , ";
						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" {item.Key} , ";
							else query += $" {item.Key} ";
							count++;
						}

						query += $" ) values ( '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , ";
						count = 0;

						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" '{item.Value}' , ";
							else query += $" '{item.Value}' ";
							count++;
						}
						query += " ) ";
						await cnn.ExecuteAsync(query);

						return Json(new { has_error = false, status_code = 201, status = "Added Successfully" });
					}
					else return Json(new { has_error = false, status_code = 400, status = "Bad Request ( entity may be null or 0 lentgh)" });
				}
			}
			catch (Exception ex)
			{
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}

		[Route("api/UpdatePersonel")]
		[HttpPost]
		public async Task<object> UpdatePersonel([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_PERSONEL_ID")) >= 1)
					{
						query = " update orjin.TB_PERSONEL set ";
						foreach (var item in entity)
						{

							if (item.Key.Equals("TB_PERSONEL_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , PRS_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ";
						query += $" where TB_PERSONEL_ID = {Convert.ToInt32(entity.GetValue("TB_PERSONEL_ID"))}";

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

		[Route("api/PersonelSertifikaList")]
		[HttpGet]
		public List<PersonelSertifika> PersonelSertifikaList([FromUri] int? personelId)
		{
			var prm = new { @PSE_PERSONEL_ID =  personelId};
			string query = @" select * from orjin.VW_PERSONEL_SERTIFIKA where PSE_PERSONEL_ID = @PSE_PERSONEL_ID ";
			List<PersonelSertifika> listem = new List<PersonelSertifika>();
			using (var cnn = klas.baglan())
			{
				listem = cnn.Query<PersonelSertifika>(query,prm).ToList();
			}
			return listem;
		}

		[HttpPost]
		[Route("api/AddPersonelSertifika")]
		public async Task<object> AddPersonelSertifika([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0)
					{
						query = " insert into orjin.TB_PERSONEL_SERTIFIKA  ( PSE_OLUSTURMA_TARIH , ";
						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" {item.Key} , ";
							else query += $" {item.Key} ";
							count++;
						}

						query += $" ) values ( '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , ";
						count = 0;

						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" '{item.Value}' , ";
							else query += $" '{item.Value}' ";
							count++;
						}
						query += " ) ";
						await cnn.ExecuteAsync(query);

						return Json(new { has_error = false, status_code = 201, status = "Added Successfully" });
					}
					else return Json(new { has_error = false, status_code = 400, status = "Bad Request ( entity may be null or 0 lentgh)" });
				}
			}
			catch (Exception ex)
			{
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}

		[Route("api/UpdatePersonelSertifika")]
		[HttpPost]
		public async Task<object> UpdatePersonelSertifika([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_PERSONEL_SERTIFIKA_ID")) >= 1)
					{
						query = " update orjin.TB_PERSONEL_SERTIFIKA set ";
						foreach (var item in entity)
						{

							if (item.Key.Equals("TB_PERSONEL_SERTIFIKA_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , PSE_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ";
						query += $" where TB_PERSONEL_SERTIFIKA_ID = {Convert.ToInt32(entity.GetValue("TB_PERSONEL_SERTIFIKA_ID"))}";

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


		[Route("api/PersonelSantiyeList")]
		[HttpGet]
		public List<PersonelSantiye> PersonelSantiyeList([FromUri] int? personelId)
		{
			var prm = new { @PSS_PERSONEL_ID = personelId };
			string query = @" select * from orjin.VW_PERSONEL_SANTIYE where PSS_PERSONEL_ID = @PSS_PERSONEL_ID ";
			List<PersonelSantiye> listem = new List<PersonelSantiye>();
			using (var cnn = klas.baglan())
			{
				listem = cnn.Query<PersonelSantiye>(query, prm).ToList();
			}
			return listem;
		}

		[HttpPost]
		[Route("api/AddPersonelSantiye")]
		public async Task<object> AddPersonelSantiye([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0)
					{
						query = " insert into orjin.TB_PERSONEL_SANTIYE  ( PSS_OLUSTURMA_TARIH , ";
						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" {item.Key} , ";
							else query += $" {item.Key} ";
							count++;
						}

						query += $" ) values ( '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , ";
						count = 0;

						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" '{item.Value}' , ";
							else query += $" '{item.Value}' ";
							count++;
						}
						query += " ) ";
						await cnn.ExecuteAsync(query);

						return Json(new { has_error = false, status_code = 201, status = "Added Successfully" });
					}
					else return Json(new { has_error = false, status_code = 400, status = "Bad Request ( entity may be null or 0 lentgh)" });
				}
			}
			catch (Exception ex)
			{
				return Json(new { has_error = true, status_code = 500, status = ex.Message });
			}
		}

		[Route("api/UpdatePersonelSantiye")]
		[HttpPost]
		public async Task<object> UpdatePersonelSantiye([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_PERSONEL_SANTIYE_ID")) >= 1)
					{
						query = " update orjin.TB_PERSONEL_SANTIYE set ";
						foreach (var item in entity)
						{

							if (item.Key.Equals("TB_PERSONEL_SANTIYE_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , PSS_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ";
						query += $" where TB_PERSONEL_SANTIYE_ID = {Convert.ToInt32(entity.GetValue("TB_PERSONEL_SANTIYE_ID"))}";

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
	}
}
