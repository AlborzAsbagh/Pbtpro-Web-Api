using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using WebApiNew.Filters;
using WebApiNew.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using WebApiNew.Utility.Abstract;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace WebApiNew.Controllers
{

    [MyBasicAuthenticationFilter]
    public class IsTanimController : ApiController
    {
        Util klas = new Util();
        Parametreler prms = new Parametreler();
        String query = "";

        private readonly ILogger _logger;
        private readonly System.Windows.Forms.RichTextBox RTB = new System.Windows.Forms.RichTextBox();

        public IsTanimController(ILogger logger)
        {
            _logger = logger;
        }

        [Route("api/BakimGetir")]
        [HttpGet]
        public List<IsTanim> BakimGetir()
        {
            prms.Clear();
            string query = @" select * from orjin.VW_BAKIM_PROCEDURE ";
            List<IsTanim> listem = new List<IsTanim>();
            using(var cnn = klas.baglan())
            {
                listem = cnn.Query<IsTanim>(query).ToList();
            }
            return listem;
        }

        [Route("api/ArizaGetir")]
        [HttpGet]
        public List<IsTanim> ArizaGetir()
        {
			prms.Clear();
			string query = @" select * from orjin.VW_ARIZA_PROCEDURE ";
			List<IsTanim> listem = new List<IsTanim>();
			using (var cnn = klas.baglan())
			{
				listem = cnn.Query<IsTanim>(query).ToList();
			}
			return listem;
		}
        [Route("api/IsTanimKontrolList")]
        [HttpGet]
        public List<IsTanimKontrol> IsTanimKontrolList(int isTanimID)
        {
            string query = @"SELECT *
                            ,(SELECT TB_RESIM_ID FROM orjin.TB_RESIM WHERE RSM_REF_GRUP = 'IS_TANIM_KONTROL' AND RSM_REF_ID = TB_IS_TANIM_KONROLLIST_ID AND RSM_VARSAYILAN=1) ISK_IMAGE_ID
                            ,(SELECT prs.PRS_ISIM FROM orjin.TB_IS_TANIM ist left join orjin.TB_PERSONEL prs on ist.IST_PERSONEL_ID = prs.TB_PERSONEL_ID
                            WHERE TB_IS_TANIM_ID = TB_IS_TANIM_KONROLLIST_ID AND PRS_AKTIF = 1) PERSONEL_ISIM
                            ,stuff((SELECT ';' + CONVERT(varchar(11), R.TB_RESIM_ID) FROM orjin.TB_RESIM R WHERE R.RSM_REF_GRUP = 'IS_TANIM_KONTROL' and R.RSM_REF_ID = TB_IS_TANIM_KONROLLIST_ID FOR XML PATH('')),1,1,'') ISK_IMAGE_IDS_STR
                             FROM orjin.TB_IS_TANIM_KONTROLLIST WHERE ISK_IS_TANIM_ID = @ISTNM_ID";
            var util = new Util();
            using (var cnn = util.baglan())
            {
                var list = cnn.Query<IsTanimKontrol>(query, new { ISTNM_ID = isTanimID }).ToList();
                list.ForEach(delegate (IsTanimKontrol item)
                {
                    try
                    {
                        RTB.Rtf = item.ISK_ACIKLAMA;
                        item.ISK_ACIKLAMA = RTB.Text;
                    }
                    catch (Exception e)
                    {

                    }
                }
                    );
                return list;
            }
        }

        [Route("api/IsTanimIsmKontrolList")]
        [HttpGet]
        public List<IsEmriKontrolList> IsTanimIsmKontrolList(int isTanimID)
        {
            prms.Clear();
            prms.Add("ISTNM_ID", isTanimID);
            string query = @"SELECT * FROM orjin.TB_IS_TANIM_KONTROLLIST WHERE ISK_IS_TANIM_ID = @ISTNM_ID";
            DataTable dt = klas.GetDataTable(query, prms.PARAMS);
            List<IsEmriKontrolList> listem = new List<IsEmriKontrolList>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //IsTanimKontrollist entity = new IsTanimKontrollist();
                //entity.TB_IS_TANIM_KONROLLIST_ID = (int)dt.Rows[i]["TB_IS_TANIM_KONROLLIST_ID"];
                //entity.ISK_TANIM = dt.Rows[i]["ISK_TANIM"].ToString();
                //entity.ISK_SIRANO = Convert.ToString(dt.Rows[i]["ISK_SIRANO"]);
                //entity.ISK_IS_TANIM_ID = Convert.ToInt32(dt.Rows[i]["ISK_IS_TANIM_ID"]);
                //listem.Add(entity);

                IsEmriKontrolList entity = new IsEmriKontrolList();
                entity.DKN_TANIM = Util.getFieldString(dt.Rows[i], "ISK_TANIM");
                entity.DKN_SIRANO = Util.getFieldString(dt.Rows[i], "ISK_SIRANO");
                entity.DKN_REF_ID = isTanimID;
                entity.DKN_YAPILDI_SAAT = "0";
                entity.DKN_YAPILDI_PERSONEL_ID = -1;
                entity.DKN_YAPILDI_MESAI_KOD_ID = -1;
                entity.DKN_YAPILDI_ATOLYE_ID = -1;
                entity.DKN_YAPILDI_SURE = 0;
                listem.Add(entity);
            }
            return listem;
        }


        [Route("api/IsTanimMazleme")]
        [HttpGet]
        public List<IsEmriMalzeme> IsTanimMazleme(int isTanimID)
        {
            prms.Clear();
            prms.Add("ISTNM_ID", isTanimID);
            string query = @"select mlz.* , stk.STK_KOD as ISM_STOK_KOD , orjin.UDF_KOD_TANIM(ISM_BIRIM_KOD_ID) as ISM_BIRIM  , dp.DEP_TANIM as ISM_DEPO , stk.STK_STOKSUZ_MALZEME as ISM_STOKSUZ from orjin.TB_IS_TANIM_MLZ mlz
                             left join orjin.VW_STOK stk on stk.TB_STOK_ID = mlz.ISM_STOK_ID
                             left join orjin.TB_DEPO dp on stk.STK_DEPO_ID = dp.TB_DEPO_ID where ISM_IS_TANIM_ID = @ISTNM_ID";

            DataTable dt = klas.GetDataTable(query, prms.PARAMS);
            List<IsEmriMalzeme> listem = new List<IsEmriMalzeme>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IsEmriMalzeme entity = new IsEmriMalzeme();
                entity.IDM_TUTAR = Util.getFieldDouble(dt.Rows[i], "ISM_TUTAR");
                entity.IDM_BIRIM_FIYAT = Util.getFieldDouble(dt.Rows[i], "ISM_BIRIM_FIYAT");
                entity.IDM_MIKTAR = Util.getFieldDouble(dt.Rows[i], "ISM_MIKTAR");
                entity.IDM_BIRIM_KOD_ID = Util.getFieldInt(dt.Rows[i], "ISM_BIRIM_KOD_ID");
                entity.IDM_STOK_ID = Util.getFieldInt(dt.Rows[i], "ISM_STOK_ID");
                entity.IDM_STOK_TANIM = Util.getFieldString(dt.Rows[i], "ISM_STOK_TANIM");
                entity.IDM_BIRIM = Util.getFieldString(dt.Rows[i], "ISM_BIRIM");
                entity.IDM_STOK_KOD = Util.getFieldString(dt.Rows[i], "ISM_STOK_KOD");
                entity.IDM_DEPO = Util.getFieldString(dt.Rows[i], "ISM_DEPO");
                entity.IDM_STOKSUZ = Util.getFieldBool(dt.Rows[i], "ISM_STOKSUZ");
                entity.IDM_ALTERNATIF_STOK_ID = -1;
                entity.IDM_DEPO_ID = -1;
                entity.IDM_REF_ID = -1;
                entity.IDM_STOK_DUS = entity.IDM_STOK_ID != -1;
                entity.IDM_MALZEME_STOKTAN = entity.IDM_STOK_ID == -1 ? "Düşmesin" : "Düşsün";
                listem.Add(entity);
            }
            return listem;
        }

        [Route("api/IsTanim/KontrolEkle")]
        [HttpPost]
        public Bildirim IsTanimKontrolEkle([FromBody]IsTanimKontrol entity)
        {
            var bildirim = new Bildirim();
            var util = new Util();
            var isUpdate = entity.TB_IS_TANIM_KONROLLIST_ID > 0;
            try
            {
                using (var cnn = util.baglan())
                {
                    if (isUpdate) {
                        bildirim.Durum = cnn.Update(entity);
                        bildirim.Id = entity.TB_IS_TANIM_KONROLLIST_ID;
                    }
                    else
                        bildirim.Id = cnn.Insert(entity);
                    if ((!isUpdate && bildirim.Id > 0) || bildirim.Durum )
                    {
                        bildirim.Durum = true;
                        bildirim.MsgId = Bildirim.MSG_ISLEM_BASARILI;
                    }
                    else
                    {
                        bildirim.Durum = false;
                        bildirim.MsgId = Bildirim.MSG_ISLEM_HATA;
                    }
                }
            }
            catch (Exception e)
            {
                bildirim.Durum = false;
                bildirim.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirim.Aciklama = e.Message;
            }
            return bildirim;
        }
    
        [Route("api/IsTanim/KontrolSil")]
        [HttpPost]
        public Bildirim IsTanimKontrolSil([FromUri]int id)
        {
            var bildirim = new Bildirim();
            var util = new Util();
            try
            {
                using (var cnn = util.baglan())
                {
                    var i = cnn.Execute("DELETE FROM orjin.TB_IS_TANIM_KONTROLLIST WHERE TB_IS_TANIM_KONROLLIST_ID = @ID", new { @ID=id});
                    if (i>0)
                    {
                        var ids = cnn.Query<int>("SELECT TB_RESIM_ID FROM orjin.TB_RESIM WHERE RSM_REF_ID = @ID AND RSM_REF_GRUP = 'IS_TANIM_KONTROL'", new { ID=id}).ToList();
                        if(ids.Count>0)
                            new ResimController(_logger).RemoveMultiple(ids.ToArray());
                        bildirim.Durum = true;
                        bildirim.MsgId = Bildirim.MSG_ISLEM_BASARILI;
                    }
                    else
                    {
                        bildirim.Durum = false;
                        bildirim.MsgId = Bildirim.MSG_ISLEM_HATA;
                    }
                }
            }
            catch (Exception e)
            {
                bildirim.Durum = false;
                bildirim.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirim.Aciklama = e.Message;
            }
            return bildirim;
        }


        //Get Prosedur Web App Version (Is Emri Ekleme Syafasi)
        [Route("api/GetProsedur")]
        [HttpGet]
        public Object GetProsedur([FromUri] string tip)
        {
            if (tip == "ARIZA") return Json(new { PROSEDUR_LISTE = ArizaGetir() });
            else if (tip == "BAKIM") return Json(new { PROSEDUR_LISTE = BakimGetir() });
            else return Json(new { error = "Tanimsiz !" });
		}

		// Add Bakim Wep App
		[Route("api/AddBakim")]
		[HttpPost]
		public async Task<object> AddBakim([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0)
					{
						query = " insert into orjin.TB_IS_TANIM  ( IST_OLUSTURMA_TARIH , IST_DURUM , ";
						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" {item.Key} , ";
							else query += $" {item.Key} ";
							count++;
						}

						query += $" ) values ( '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , 'BAKIM' , ";
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
				return Json(new { has_error = true, status_code = 500, status = ex.Message});
			}
		}

		// Bakim Guncelle Web App 
		[Route("api/UpdateBakim")]
		[HttpPost]
		public async Task<Object> UpdateBakim([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_IS_TANIM_ID")) >= 1)
					{
						query = " update orjin.TB_IS_TANIM set ";
						foreach (var item in entity)
						{

							if (item.Key.Equals("TB_IS_TANIM_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , IST_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ";
						query += $" where TB_IS_TANIM_ID = {Convert.ToInt32(entity.GetValue("TB_IS_TANIM_ID"))}";

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

        // Web App Malzeme List
		[Route("api/GetIsTanimMazleme")]
		[HttpGet]
		public List<IsTanimMalzemeWebAppModel> GetIsTanimMazleme(int isTanimID)
		{
			var prms = new {@ISTNM_ID =  isTanimID};
			string query = @"select mlz.* , stk.STK_KOD as ISM_STOK_KOD , orjin.UDF_KOD_TANIM(ISM_BIRIM_KOD_ID) as 
            ISM_BIRIM  , mlz.ISM_STOK_TIP_KOD_ID as ISM_STOK_TIP_KOD_ID , orjin.UDF_KOD_TANIM(mlz.ISM_STOK_TIP_KOD_ID) as ISM_STOK_TIP , dp.DEP_TANIM as ISM_DEPO , stk.STK_STOKSUZ_MALZEME as ISM_STOKSUZ from orjin.TB_IS_TANIM_MLZ mlz
            left join orjin.VW_STOK stk on stk.TB_STOK_ID = mlz.ISM_STOK_ID
            left join orjin.TB_DEPO dp on stk.STK_DEPO_ID = dp.TB_DEPO_ID where ISM_IS_TANIM_ID = @ISTNM_ID";
            List<IsTanimMalzemeWebAppModel> listem = new List<IsTanimMalzemeWebAppModel>();

            using(var cnn = klas.baglan())
            {
                listem = cnn.Query<IsTanimMalzemeWebAppModel>(query, prms).ToList();
            }
			
			return listem;
		}


		// Add Is Tanim Malzeme Wep App
		[Route("api/AddIsTanimMalzeme")]
		[HttpPost]
		public async Task<object> AddIsTanimMalzeme([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0)
					{
						query = " insert into orjin.TB_IS_TANIM_MLZ  ( ISM_OLUSTURMA_TARIH ,  ";
						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" {item.Key} , ";
							else query += $" {item.Key} ";
							count++;
						}

						query += $" ) values ( '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ,  ";
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

		// Is Tanim Malzeme Guncelle Web App 
		[Route("api/UpdateIsTanimMalzeme")]
		[HttpPost]
		public async Task<Object> UpdateIsTanimMalzeme([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_IS_TANIM_MLZ_ID")) >= 1)
					{
						query = " update orjin.TB_IS_TANIM_MLZ set ";
						foreach (var item in entity)
						{

							if (item.Key.Equals("TB_IS_TANIM_MLZ_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , ISM_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ";
						query += $" where TB_IS_TANIM_MLZ_ID = {Convert.ToInt32(entity.GetValue("TB_IS_TANIM_MLZ_ID"))}";

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


		// Web App Olcum List
		[Route("api/GetIsTanimOlcum")]
		[HttpGet]
		public List<OlcumParametreWebApp> GetIsTanimOlcum(int isTanimID)
		{
			var prms = new { @ISTNM_ID = isTanimID };
			string query = @" select * , orjin.UDF_KOD_TANIM(IOC_BIRIM_KOD_ID) as IOC_BIRIM from orjin.TB_IS_TANIM_OLCUM_PARAMETRE IOC where IOC_IS_TANIM_ID = @ISTNM_ID ";
			List<OlcumParametreWebApp> listem = new List<OlcumParametreWebApp>();

			using (var cnn = klas.baglan())
			{
				listem = cnn.Query<OlcumParametreWebApp>(query, prms).ToList();
			}

			return listem;
		}


		// Add Is Tanim Olcum Wep App
		[Route("api/AddIsTanimOlcum")]
		[HttpPost]
		public async Task<object> AddIsTanimOlcum([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0)
					{
						query = " insert into orjin.TB_IS_TANIM_OLCUM_PARAMETRE  ( IOC_OLUSTURMA_TARIH ,  ";
						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" {item.Key} , ";
							else query += $" {item.Key} ";
							count++;
						}

						query += $" ) values ( '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ,  ";
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

		// Is Tanim Olcum Guncelle Web App 
		[Route("api/UpdateIsTanimOlcum")]
		[HttpPost]
		public async Task<Object> UpdateIsTanimOlcum([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_IS_TANIM_OLCUM_PARAMETRE_ID")) >= 1)
					{
						query = " update orjin.TB_IS_TANIM_OLCUM_PARAMETRE set ";
						foreach (var item in entity)
						{

							if (item.Key.Equals("TB_IS_TANIM_OLCUM_PARAMETRE_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , IOC_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ";
						query += $" where TB_IS_TANIM_OLCUM_PARAMETRE_ID = {Convert.ToInt32(entity.GetValue("TB_IS_TANIM_OLCUM_PARAMETRE_ID"))}";

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

		// Add Is Tanim KontrolList Wep App
		[Route("api/AddIsTanimKontrolList")]
		[HttpPost]
		public async Task<object> AddIsTanimKontrolList([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0)
					{
						query = " insert into orjin.TB_IS_TANIM_KONTROLLIST  ( ISK_OLUSTURMA_TARIH ,  ";
						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" {item.Key} , ";
							else query += $" {item.Key} ";
							count++;
						}

						query += $" ) values ( '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ,  ";
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

		// Is Tanim KontrolList Guncelle Web App 
		[Route("api/UpdateIsTanimKontrolList")]
		[HttpPost]
		public async Task<Object> UpdateIsTanimKontrolList([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_IS_TANIM_KONROLLIST_ID")) >= 1)
					{
						query = " update orjin.TB_IS_TANIM_KONTROLLIST set ";
						foreach (var item in entity)
						{

							if (item.Key.Equals("TB_IS_TANIM_KONROLLIST_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , ISK_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ";
						query += $" where TB_IS_TANIM_KONROLLIST_ID = {Convert.ToInt32(entity.GetValue("TB_IS_TANIM_KONROLLIST_ID"))}";

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


		// Add Bakim Wep App
		[Route("api/AddAriza")]
		[HttpPost]
		public async Task<object> AddAriza([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0)
					{
						query = " insert into orjin.TB_IS_TANIM  ( IST_OLUSTURMA_TARIH , IST_DURUM , ";
						foreach (var item in entity)
						{
							if (count < entity.Count - 1) query += $" {item.Key} , ";
							else query += $" {item.Key} ";
							count++;
						}

						query += $" ) values ( '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' , 'ARIZA' , ";
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

		// Bakim Guncelle Web App 
		[Route("api/UpdateAriza")]
		[HttpPost]
		public async Task<Object> UpdateAriza([FromBody] JObject entity)
		{
			int count = 0;
			try
			{
				using (var cnn = klas.baglan())
				{
					if (entity != null && entity.Count > 0 && Convert.ToInt32(entity.GetValue("TB_IS_TANIM_ID")) >= 1)
					{
						query = " update orjin.TB_IS_TANIM set ";
						foreach (var item in entity)
						{

							if (item.Key.Equals("TB_IS_TANIM_ID")) continue;

							if (count < entity.Count - 2) query += $" {item.Key} = '{item.Value}', ";
							else query += $" {item.Key} = '{item.Value}' ";
							count++;
						}
						query += $" , IST_DEGISTIRME_TARIH = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}' ";
						query += $" where TB_IS_TANIM_ID = {Convert.ToInt32(entity.GetValue("TB_IS_TANIM_ID"))}";

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
