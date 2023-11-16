using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dapper;
using WebApiNew.Models;
using WebApiNew;
using WebApiNew.Filters;
using System.Collections;
using System.Data.SqlClient;

namespace WebApiNew.Controllers
{

    [MyBasicAuthenticationFilter]
    public class StokController : ApiController
    {
        Parametreler prms = new Parametreler();
        Util klas = new Util();
		SqlCommand cmd = null;
		public List<Stok> Get([FromUri] int ilkDeger, [FromUri] int sonDeger, [FromUri] int Tip, [FromUri] int Grup, [FromUri] string prm, [FromUri] Boolean b, [FromUri] int modulNo)
        {
            var prms = new DynamicParameters();
            prms.Add("MODUL_NO", modulNo);
            string query = @"select * 
                        ,(select COALESCE(TB_RESIM_ID,0) from orjin.TB_RESIM where RSM_VARSAYILAN= 1 AND RSM_REF_GRUP = 'STOK' and RSM_REF_ID = TB_STOK_ID) as RSM_VAR_ID
                        ,stuff((SELECT ';' + CONVERT(varchar(11), R.TB_RESIM_ID) FROM orjin.TB_RESIM R WHERE R.RSM_REF_GRUP = 'STOK' and R.RSM_REF_ID = TB_STOK_ID FOR XML PATH('')), 1, 1, '') [RSM_IDS]
                        from (select *,ROW_NUMBER() OVER(ORDER BY TB_STOK_ID) AS satir from orjin.VW_STOK where STK_AKTIF = 1 and STK_MODUL_NO= @MODUL_NO";

            if (Tip != -1)
            {
                prms.Add("STK_TIP_KOD_ID", Tip);
                query = query + " and STK_TIP_KOD_ID = @STK_TIP_KOD_ID";
            }
            if (Grup != -1)
            {
                prms.Add("STK_GRUP_KOD_ID", Grup);
                query = query + " and STK_GRUP_KOD_ID = @STK_GRUP_KOD_ID";
            }
            if (!String.IsNullOrEmpty(prm))
            {
                prms.Add("PRM", prm);
                if (b)
                    query = query + " and STK_KOD = @PRM";
                else
                    query = query + @" and (STK_KOD     LIKE '%'+@PRM+'%' OR 
                                            STK_TANIM   LIKE '%'+@PRM+'%' OR 
                                            STK_GRUP    LIKE '%'+@PRM+'%' OR 
                                            STK_MARKA   LIKE '%'+@PRM+'%' OR 
                                            STK_MODEL   LIKE '%'+@PRM+'%' OR 
                                            STK_TIP     LIKE '%'+@PRM+'%' OR 
                                            '*' = @PRM)";
            }
            prms.Add("ILK_DEGER", ilkDeger);
            prms.Add("SON_DEGER", sonDeger);
            query = query + ") as tablom  where satir > @ILK_DEGER and satir <= @SON_DEGER";
            var util = new Util();
            using (var cnn = util.baglan())
            {
                var listem = cnn.Query<Stok>(query, prms).ToList();
                listem.ForEach(delegate (Stok s)
                {
                    s.STK_ACIKLAMA = Util.RemoveRtfFormatting(s.STK_ACIKLAMA);
                });
                return listem;
            }

        }

        [Route("api/StokTipleri")]
        [HttpGet]
        public List<TanimDeger> StokTipleri()
        {
            string query = @"SELECT * FROM (select TB_KOD_ID,KOD_TANIM, (SELECT COUNT(TB_STOK_ID) FROM orjin.TB_STOK WHERE STK_AKTIF = 1 AND STK_TIP_KOD_ID = TB_KOD_ID) as Sayi from orjin.TB_KOD where KOD_GRUP = 13005) AS TABLOM WHERE sayi>0";
            DataTable dt = klas.GetDataTable(query, new List<Prm>());
            List<TanimDeger> listem = new List<TanimDeger>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                TanimDeger entity = new TanimDeger();
                entity.Deger = Util.getFieldDouble(dt.Rows[i], "Sayi");
                entity.Tanim = Util.getFieldString(dt.Rows[i], "KOD_TANIM");
                entity.TanimDegerID = Util.getFieldInt(dt.Rows[i], "TB_KOD_ID");
                listem.Add(entity);
            }
            return listem;
        }
        [Route("api/StokGruplari")]
        [HttpGet]
        public List<TanimDeger> StokGruplari()
        {
            string query = @"SELECT * FROM (select TB_KOD_ID,KOD_TANIM, (SELECT COUNT(TB_STOK_ID) FROM orjin.TB_STOK WHERE STK_AKTIF = 1 AND STK_GRUP_KOD_ID = TB_KOD_ID) as Sayi from orjin.TB_KOD where KOD_GRUP = 13004) AS TABLOM WHERE sayi>0";
            prms.Clear();
            DataTable dt = klas.GetDataTable(query, prms.PARAMS);
            List<TanimDeger> listem = new List<TanimDeger>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                TanimDeger entity = new TanimDeger();
                entity.Deger = Util.getFieldDouble(dt.Rows[i], "Sayi");
                entity.Tanim = Util.getFieldString(dt.Rows[i], "KOD_TANIM");
                entity.TanimDegerID = Util.getFieldInt(dt.Rows[i], "TB_KOD_ID");
                listem.Add(entity);
            }
            return listem;
        }
        [Route("api/StokHareketleri")]
        [HttpPost]
        public List<StokHrk> StokHareketleri([FromBody] Filtre filtre, [FromUri] int ilkDeger, [FromUri] int sonDeger, [FromUri] int _stkID)
        {
            prms.Clear();
            prms.Add("@STK_ID", _stkID);
            string query = @"select * from (SELECT *,ROW_NUMBER() OVER(ORDER BY TB_STOK_HRK_ID) AS satir, 
                            CASE 
                            WHEN FIS_TIP = 'G' THEN 'Giriş'
                            WHEN FIS_TIP = 'C' THEN 'Çıkış'
                            WHEN FIS_TIP = 'T' THEN 'Transfer'
                            END TIP
                             FROM (
                            SELECT TOP(1000000) HRK.*,
                              ISM.ISM_ISEMRI_NO, 
                              LOK.LOK_TANIM,
                              LOK.TB_LOKASYON_ID,
                              CASE WHEN ISNULL(HRK.SHR_STOK_FIS_NO,'') = '' THEN HRK.SHR_GC ELSE FIS.SFS_GC END FIS_TIP  
                            FROM 
                              orjin.VW_STOK_HRK HRK   
                            LEFT OUTER JOIN orjin.TB_ISEMRI ISM ON HRK.SHR_REF_GRUP = 'ISEMRI_MLZ' AND HRK.SHR_REF_ID = ISM.TB_ISEMRI_ID
                            LEFT OUTER JOIN orjin.TB_LOKASYON LOK ON (ISM.ISM_LOKASYON_ID = LOK.TB_LOKASYON_ID)
                            LEFT OUTER JOIN orjin.TB_STOK_FIS FIS ON HRK.SHR_STOK_FIS_NO = FIS.SFS_FIS_NO where SHR_STOK_ID = @STK_ID order by SHR_TARIH desc) AS MALZEME_HAREKET where 1 = 1";

            if (filtre != null)
            {
                if (!String.IsNullOrEmpty(filtre.Tip))
                {
                    prms.Add("FIS_TIP", filtre.Tip);
                    query = query + " and FIS_TIP=@FIS_TIP";
                }
                if (filtre.DepoID > 0)
                {
                    prms.Add("SHR_DEPO_ID", filtre.DepoID);
                    query = query + " and SHR_DEPO_ID =@SHR_DEPO_ID";
                }
                if (filtre.LokasyonID > 0)
                {
                    prms.Add("TB_LOKASYON_ID", filtre.LokasyonID);
                    query = query + " and TB_LOKASYON_ID =@TB_LOKASYON_ID";
                }
                if (!String.IsNullOrEmpty(filtre.BasTarih) && !String.IsNullOrEmpty(filtre.BitTarih))
                {
                    prms.Add("BAS_TARIH", Convert.ToDateTime(filtre.BasTarih).ToString("yyyy-MM-dd"));
                    prms.Add("BIT_TARIH", Convert.ToDateTime(filtre.BitTarih).ToString("yyyy-MM-dd"));
                    query = query + " AND SHR_TARIH BETWEEN  @BAS_TARIH and @BIT_TARIH";
                }
                else
                if (!String.IsNullOrEmpty(filtre.BasTarih))
                {
                    prms.Add("BAS_TARIH", Convert.ToDateTime(filtre.BasTarih).ToString("yyyy-MM-dd"));
                    query = query + " AND SHR_TARIH >= @BAS_TARIH ";
                }
                else
                if (!String.IsNullOrEmpty(filtre.BitTarih))
                {
                    prms.Add("BIT_TARIH", Convert.ToDateTime(filtre.BitTarih).ToString("yyyy-MM-dd"));
                    query = query + " AND SHR_TARIH <= @BIT_TARIH";
                }
                if (!String.IsNullOrEmpty(filtre.Kelime))
                {
                    prms.Add("KELIME", filtre.Kelime);
                    query = query + @"  AND    (SHR_DEPO_KOD    like '%'+@KELIME+'%' OR 
                                                SHR_DEPO_TANIM  like '%'+@KELIME+'%' OR 
                                                SHR_STOK_FIS_NO like '%'+@KELIME+'%' OR 
                                                LOK_TANIM       like '%'+@KELIME+'%' OR 
                                                ISM_ISEMRI_NO   like '%'+@KELIME+'%')";
                }

            }
            prms.Add("ILK_DEGER", ilkDeger);
            prms.Add("SON_DEGER", sonDeger);
            query = query + ") as tablom  where satir > @ILK_DEGER and satir <= @SON_DEGER";
            DataTable dt = klas.GetDataTable(query, prms.PARAMS);
            List<StokHrk> listem = new List<StokHrk>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                StokHrk entity = new StokHrk();
                entity.TB_STOK_HRK_ID = Util.getFieldInt(dt.Rows[i], "TB_STOK_HRK_ID");
                entity.SHR_TARIH = Util.getFieldDateTime(dt.Rows[i], "SHR_TARIH");
                entity.FIS_TIP = Util.getFieldString(dt.Rows[i], "FIS_TIP");
                entity.TIP = Util.getFieldString(dt.Rows[i], "TIP");
                entity.ISM_ISEMRI_NO = Util.getFieldString(dt.Rows[i], "ISM_ISEMRI_NO");
                entity.LOK_TANIM = Util.getFieldString(dt.Rows[i], "LOK_TANIM");
                entity.SHR_BIRIM = Util.getFieldString(dt.Rows[i], "SHR_BIRIM");
                entity.SHR_DEPO_KOD = Util.getFieldString(dt.Rows[i], "SHR_DEPO_KOD");
                entity.SHR_DEPO_TANIM = Util.getFieldString(dt.Rows[i], "SHR_DEPO_TANIM");
                entity.SHR_SAAT = Util.getFieldString(dt.Rows[i], "SHR_SAAT");
                entity.SHR_STOK_KOD = Util.getFieldString(dt.Rows[i], "SHR_STOK_KOD");
                entity.SHR_STOK_TANIM = Util.getFieldString(dt.Rows[i], "SHR_STOK_TANIM");
                entity.SHR_STOK_FIS_NO = Util.getFieldString(dt.Rows[i], "SHR_STOK_FIS_NO");
                entity.SHR_MIKTAR = Util.getFieldDouble(dt.Rows[i], "SHR_MIKTAR");
                listem.Add(entity);
            }
            return listem;
        }
        [Route("api/StokDetaySayi")]
        [HttpGet]
        public List<TanimDeger> StokDetaySayi([FromUri] int stokid)
        {
            prms.Clear();
            prms.Add("STK_ID", stokid);
            try
            {
                List<TanimDeger> listem = new List<TanimDeger>();
                TanimDeger entity1 = new TanimDeger();
                entity1.Tanim = "StokHareket";
                entity1.Deger = Convert.ToInt32(klas.GetDataCell("select COUNT(TB_STOK_HRK_ID) from orjin.TB_STOK_HRK where SHR_STOK_ID = @STK_ID", prms.PARAMS));
                listem.Add(entity1);
                TanimDeger entity = new TanimDeger();
                entity.Tanim = "DepoDurum";
                entity.Deger = Convert.ToInt32(klas.GetDataCell("select COUNT(TB_DEPO_STOK_ID) from orjin.TB_DEPO_STOK where DPS_STOK_ID = @STK_ID", prms.PARAMS));
                listem.Add(entity);
                return listem;
            }
            catch (Exception)
            {
                throw;
            }

        }

        [Route("api/GetMalzemeModel")]
        [HttpGet]
        public Object GetMalzemeModel()
        {
			string query = @"SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP=13003";
            List<Kod> listem = new List<Kod>();
            try
            {
				using (var cnn = klas.baglan())
				{
                    listem = cnn.Query<Kod>(query).ToList();
				}
                return Json(new { malzeme_model_list = listem });
			}
            catch(Exception e)
            {
				return Json(new { error = e.Message });
			}

		}

		[Route("api/AddMalzemeModel")]
		[HttpGet]
		public Object AddMalzemeModel([FromUri] string malzemeModel)
		{
			try
			{
				string query = " insert into orjin.TB_KOD (KOD_GRUP , KOD_TANIM , KOD_AKTIF , KOD_GOR , KOD_DEGISTIR , KOD_SIL ) ";
				query += $" values ( 13003 , '{malzemeModel}' , 1 , 1 , 1 ,1 ) ";

				using (var con = klas.baglan())
				{
					cmd = new SqlCommand(query, con);
					cmd.ExecuteNonQuery();
				}
				klas.kapat();
				return Json(new { success = "Ekleme başarılı " });
			}
			catch (Exception e)
			{
				klas.kapat();
				return Json(new { error = " Ekleme başarısız " });
			}
		}

		[Route("api/GetMalzemeMarka")]
		[HttpGet]
		public Object GetMalzemeMarka()
		{
			string query = @"SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP=13002";
			List<Kod> listem = new List<Kod>();
			try
			{
				using (var cnn = klas.baglan())
				{
					listem = cnn.Query<Kod>(query).ToList();
				}
				return Json(new { malzeme_marka_list = listem });
			}
			catch (Exception e)
			{
				return Json(new { error = e.Message });
			}
		}

		[Route("api/AddMalzemeMarka")]
		[HttpGet]
		public Object AddMalzemeMarka([FromUri] string malzemeMarka)
		{
			try
			{
				string query = " insert into orjin.TB_KOD (KOD_GRUP , KOD_TANIM , KOD_AKTIF , KOD_GOR , KOD_DEGISTIR , KOD_SIL ) ";
				query += $" values ( 13002 , '{malzemeMarka}' , 1 , 1 , 1 ,1 ) ";

				using (var con = klas.baglan())
				{
					cmd = new SqlCommand(query, con);
					cmd.ExecuteNonQuery();
				}
				klas.kapat();
				return Json(new { success = "Ekleme başarılı " });
			}
			catch (Exception e)
			{
				klas.kapat();
				return Json(new { error = " Ekleme başarısız " });
			}
		}

		[Route("api/GetMalzemeTip")]
		[HttpGet]
		public Object GetMalzemeTip()
		{
			string query = @"SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP=13005";
			List<Kod> listem = new List<Kod>();
			try
			{
				using (var cnn = klas.baglan())
				{
					listem = cnn.Query<Kod>(query).ToList();
				}
				return Json(new { malzeme_tip_list = listem });
			}
			catch (Exception e)
			{
				return Json(new { error = e.Message });
			}
		}

		[Route("api/AddMalzemeTip")]
		[HttpGet]
		public Object AddMalzemeTip([FromUri] string malzemeTip)
		{
			try
			{
				string query = " insert into orjin.TB_KOD (KOD_GRUP , KOD_TANIM , KOD_AKTIF , KOD_GOR , KOD_DEGISTIR , KOD_SIL ) ";
				query += $" values ( 13005 , '{malzemeTip}' , 1 , 1 , 1 ,1 ) ";

				using (var con = klas.baglan())
				{
					cmd = new SqlCommand(query, con);
					cmd.ExecuteNonQuery();
				}
				klas.kapat();
				return Json(new { success = "Ekleme başarılı " });
			}
			catch (Exception e)
			{
				klas.kapat();
				return Json(new { error = " Ekleme başarısız " });
			}
		}

	}
}
