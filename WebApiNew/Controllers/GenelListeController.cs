﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using System.Web.WebPages;
using Dapper;
using WebApiNew.App_GlobalResources;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    
    [MyBasicAuthenticationFilter]
    public class GenelListeController : ApiController
    {
        List<Prm> parametreler = new List<Prm>();
        Parametreler prms = new Parametreler();
        Util klas = new Util();
        SqlCommand cmd = null;

        [Route("api/AtolyeList")]
        [HttpGet]
        public List<Atolye> AtolyeListesi(int kulID)
        {
            parametreler.Clear();
            parametreler.Add(new Prm("KUL_ID", kulID));
            string query =
                @"select * from orjin.TB_ATOLYE where orjin.UDF_ATOLYE_YETKI_KONTROL(TB_ATOLYE_ID, @KUL_ID) = 1";
            DataTable dt = klas.GetDataTable(query, parametreler);
            List<Atolye> listem = new List<Atolye>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Atolye entity = new Atolye();
                entity.TB_ATOLYE_ID = (int) dt.Rows[i]["TB_ATOLYE_ID"];
                entity.ATL_KOD = Util.getFieldString(dt.Rows[i], "ATL_KOD");
                entity.ATL_TANIM = Util.getFieldString(dt.Rows[i], "ATL_TANIM");
                listem.Add(entity);
            }
            return listem;
        }

        [Route("api/MasrafMerkeziList")]
        [HttpGet]
        public List<MasrafMerkezi> MasrafMerkeziList()
        {
            // Metodlar.SendNotificationToTopic("testkod", "testbaslik", "testbody", 5, "talep", "testtttt");

            string query = @"select * from orjin.TB_MASRAF_MERKEZ";
            DataTable dt = klas.GetDataTable(query, new List<Prm>());
            List<MasrafMerkezi> listem = new List<MasrafMerkezi>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                MasrafMerkezi entity = new MasrafMerkezi();
                entity.TB_MASRAF_MERKEZ_ID = (int) dt.Rows[i]["TB_MASRAF_MERKEZ_ID"];
                entity.MAM_KOD = Util.getFieldString(dt.Rows[i], "MAM_KOD");
                entity.MAM_TANIM = Util.getFieldString(dt.Rows[i], "MAM_TANIM");
                entity.MAM_USTGRUP_ID = Util.getFieldInt(dt.Rows[i], "MAM_USTGRUP_ID");
                listem.Add(entity);
            }
            return listem;
        }

        [Route("api/OncelikList")]
        [HttpGet]
        public List<Oncelik> OncelikList()
        {
            string query = @"select * from orjin.TB_SERVIS_ONCELIK";
            DataTable dt = klas.GetDataTable(query, new List<Prm>());
            List<Oncelik> listem = new List<Oncelik>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Oncelik entity = new Oncelik();
                entity.TB_SERVIS_ONCELIK_ID = (int) dt.Rows[i]["TB_SERVIS_ONCELIK_ID"];
                entity.SOC_KOD = Util.getFieldString(dt.Rows[i], "SOC_KOD");
                entity.SOC_TANIM = Util.getFieldString(dt.Rows[i], "SOC_TANIM");
                listem.Add(entity);
            }
            return listem;
        }

        [Route("api/ProjeList")]
        [HttpGet]
        public List<Proje> ProjeList()
        {
            string query = @"select * from orjin.TB_PROJE ";
            DataTable dt = klas.GetDataTable(query, new List<Prm>());
            List<Proje> listem = new List<Proje>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Proje entity = new Proje();
                entity.TB_PROJE_ID = (int) dt.Rows[i]["TB_PROJE_ID"];
                entity.PRJ_KOD = Util.getFieldString(dt.Rows[i], "PRJ_KOD");
                entity.PRJ_TANIM = Util.getFieldString(dt.Rows[i], "PRJ_TANIM");
                listem.Add(entity);
            }
            return listem;
        }

        [Route("api/ProjeIslerList")]
        [HttpGet]
        public IEnumerable<ProjeIs> ProjeIsList(int prjId)
        {
            string query = @"SELECT * FROM orjin.VW_PROJE_ISLER WHERE PIS_KAYNAK_ID > 0 AND PIS_PROJE_REF_ID = @ID";
            var prms = new {@ID = prjId};
            try
            {
                var util = new Util();
                using (var conn = util.baglan())
                {
                    var listem = conn.Query<ProjeIs>(query, prms);

                    return listem;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        [Route("api/OzelAlan")]
        [HttpGet]
        public OzelAlan Ozelalan(string form)
        {
            string query = @"SELECT * FROM orjin.TB_OZEL_ALAN WHERE OZL_FORM = @FORM";
            var prms = new {@FORM = form};
            try
            {
                var util = new Util();
                using (var conn = util.baglan())
                {
                    var ozelAlan = conn.QueryFirst<OzelAlan>(query, prms);
                    return ozelAlan;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [Route("api/IsTipiList")]
        [HttpGet]
        public List<Kod> KodList(string grupKod)
        {
            KodController kd = new KodController();
            return kd.KodList(grupKod);
        }

        [Route("api/MalzemeTipleri")]
        [HttpGet]
        public List<Kod> MalzemeTipleri()
        {
            KodController kd = new KodController();
            return kd.KodList(klas.malzemeTip);
        }

        [Route("api/olcuBirimleri")]
        [HttpGet]
        public List<Kod> olcuBirimleri()
        {
            KodController kd = new KodController();
            return kd.KodList(klas.olcuBirim);
        }

        [Route("api/VardiyaTanimlari")]
        [HttpGet]
        public List<Kod> VardiyaTanimlari()
        {
            KodController kd = new KodController();
            return kd.KodList(klas.vardiyaTanimlari);
        }
        [Route("api/Vardiyalar")]
        [HttpGet]
        public IEnumerable<Vardiya> Vardiyalar([FromUri]int userId)
        {
            var klas = new Util();
            using (var conn=klas.baglan())
            {
               return conn.Query<Vardiya>("SELECT * FROM orjin.TB_VARDIYA WHERE orjin.UDF_LOKASYON_YETKI_KONTROL(VAR_LOKASYON_ID, @KULLANICI_ID) = 1 ", new {KULLANICI_ID = userId});
            }
        }

        [Route("api/ParametreDeger")]
        [HttpGet]
        public Parametre ParametreDeger(string kod)
        {
            parametreler.Clear();
            parametreler.Add(new Prm("KOD", kod));
            string query = @"select * from orjin.TB_PARAMETRE where PRM_KOD=@KOD";
            DataTable dt = klas.GetDataTable(query, parametreler);
            Parametre entity = new Parametre();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                entity.TB_PARAMETRE_ID = (int) dt.Rows[i]["TB_PARAMETRE_ID"];
                entity.PRM_GRUP_ID = Util.getFieldInt(dt.Rows[i], "PRM_GRUP_ID");
                entity.PRM_TANIM = Util.getFieldString(dt.Rows[i], "PRM_TANIM");
                entity.PRM_KOD = Util.getFieldString(dt.Rows[i], "PRM_KOD");
                entity.PRM_DEGER = Util.getFieldString(dt.Rows[i], "PRM_DEGER");
            }
            return entity;
        }

        [Route("api/ParametreDegerList")]
        [HttpGet]
        public List<Parametre> ParametreDegerList(string kod)
        {
            List<Parametre> liste = new List<Parametre>();
            try
            {
                string[] _kodList = kod.Split(',');
                foreach (var item in _kodList)
                {
                    parametreler.Clear();
                    parametreler.Add(new Prm("KOD", item));
                    string query = @"select * from orjin.TB_PARAMETRE where PRM_KOD=@KOD";
                    DataTable dt = klas.GetDataTable(query, parametreler);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Parametre entity = new Parametre();
                        entity.TB_PARAMETRE_ID = (int) dt.Rows[i]["TB_PARAMETRE_ID"];
                        entity.PRM_GRUP_ID = Util.getFieldInt(dt.Rows[i], "PRM_GRUP_ID");
                        entity.PRM_TANIM = Util.getFieldString(dt.Rows[i], "PRM_TANIM");
                        entity.PRM_KOD = Util.getFieldString(dt.Rows[i], "PRM_KOD");
                        entity.PRM_DEGER = Util.getFieldString(dt.Rows[i], "PRM_DEGER");
                        liste.Add(entity);
                    }
                }
                return liste;
            }
            catch (Exception)
            {
                klas.kapat();
                return liste;
            }
        }

        [Route("api/CariListe/{userId}")]
        [HttpPost]
        public ResponseModel Carilist( [FromBody] Filtre filtre,[FromUri] int userId,[FromUri] int page,[FromUri] int pageSize, [FromUri] bool aktif=false,[FromUri] string search = "")
        {
            var util=new Util();
            var resp = new ResponseModel();
            resp.Status = true;
            resp.Error = false;
            var from = page * pageSize;
            var to = from + pageSize;
            #region sql

            var sql =
                @";WITH MTABLE AS (SELECT * , ROW_NUMBER() OVER(ORDER BY CAR_TANIM) AS RN FROM orjin.VW_CARI WHERE 
CAR_AKTIF = @AKTIF AND 
orjin.UDF_LOKASYON_YETKI_KONTROL(CAR_LOKASYON_ID, @KULL_ID) = 1 ";
if(!search.IsEmpty())
   sql+= @"  AND (CAR_TANIM LIKE '%'+@QU+'%' 
                                    OR CAR_ADRES LIKE '%'+@QU+'%' 
                                    OR CAR_SEHIR LIKE '%'+@QU+'%' 
                                    OR CAR_ILCE LIKE '%'+@QU+'%' 
                                    OR CAR_BOLGE LIKE '%'+@QU+'%' 
                                    OR CAR_LOKASYON LIKE '%'+@QU+'%' 
                                    OR CAR_POSTA_KOD LIKE '%'+@QU+'%' 
                                    OR CAR_KOD LIKE '%'+@QU+'%' 
                                    OR REPLACE(REPLACE(REPLACE(REPLACE(CAR_TEL1,' ',''),'(',''),')',''),'-','') LIKE '%'+REPLACE(@QU,' ','')+'%' 
                                    OR REPLACE(REPLACE(REPLACE(REPLACE(CAR_TEL2,' ',''),'(',''),')',''),'-','') LIKE '%'+REPLACE(@QU,' ','')+'%' 
                                    OR REPLACE(REPLACE(REPLACE(REPLACE(CAR_GSM,' ',''),'(',''),')',''),'-','') LIKE '%'+REPLACE(@QU,' ','')+'%' 
                                    OR REPLACE(REPLACE(REPLACE(REPLACE(CAR_FAKS1,' ',''),'(',''),')',''),'-','') LIKE '%'+REPLACE(@QU,' ','')+'%' 
                                    OR REPLACE(REPLACE(REPLACE(REPLACE(CAR_FAKS2,' ',''),'(',''),')',''),'-','') LIKE '%'+REPLACE(@QU,' ','')+'%' 
                                    OR CAR_EMAIL LIKE '%'+@QU+'%')";
       sql+=@"
)
SELECT * FROM MTABLE WHERE RN > @FROM AND RN <= @TO;
";

            #endregion
            try
            {
                using (var conn=util.baglan())
                {
                    resp.Data = conn.Query<Cari>(sql,
                        new {FROM = from, TO = to, KULL_ID = userId, QU = search, AKTIF = aktif});
                    resp.Page = page;
                    resp.PageSize = pageSize;
                    resp.Status = true;
                    resp.Error = false;
                }
            }
            catch (Exception e)
            {
                resp.Message = string.Format(Localization.FirmaListError, e.Message);
                resp.Status = false;
                resp.Error = true;
            }
                return resp;
        }


        [Route("api/VeritabaniBaglantiKontrol")]
        [AllowAnonymous]
        [HttpGet]
        public Boolean VeritabaniBaglantiKontrol()
        {
            try
            {
                klas.baglan();
                klas.kapat();
                return true;
            }
            catch (Exception)
            {
                klas.kapat();
                return false;
            }
        }


        [Route("api/ServisYoluGetir")]
        [AllowAnonymous]
        [HttpGet]
        public string ServisYoluGetir([FromUri] string firmaAd)
        {
            string servisYolu = "";
            parametreler.Clear();
            parametreler.Add(new Prm("FRM_AD", firmaAd));
            try
            {
                prms.Clear();
                string constr =
                    "RGF0YSBTb3VyY2U9bXNzcWwyLmJpcmhvc3QubmV0XG1zc3FsMjAxMjtJbml0aWFsIENhdGFsb2c9T3JqaW5fTXVzdGVyaV9XZWJTZXJ2aXM7IEludGVncmF0ZWQgU2VjdXJpdHk9ZmFsc2U7IFVzZXIgSUQ9b3JqaW5fc2VydmlzO1Bhc3N3b3JkPXNlcnZpc2RiMTMy";
                constr = Base64Decode(constr);
                SqlConnection baglan = new SqlConnection(constr);
                baglan.Open();
                string sql =
                    "SELECT ServisYolu,BitisTarihi,DISServisYolu,Demo FROM ServisYollari WHERE FirmaAdi = @FRM_AD";
                using (baglan)
                {
                    using (SqlCommand sorgu = new SqlCommand(sql, baglan))
                    {
                        prms.Clear();
                        prms.Add("@FRM_AD", firmaAd);
                        foreach (Prm parameter in prms.PARAMS)
                        {
                            if (parameter.ParametreDeger == null)
                            {
                                sorgu.Parameters.Add(parameter.ParametreAdi, SqlDbType.NVarChar).Value = DBNull.Value;
                            }
                            else
                            {
                                sorgu.Parameters.Add(parameter.ParametreAdi, SqlDbType.NVarChar).Value =
                                    parameter.ParametreDeger;
                            }

                            SqlDataAdapter adapter = new SqlDataAdapter(sorgu);
                            DataTable dt = new DataTable();
                            try
                            {
                                adapter.Fill(dt);
                                servisYolu =
                                    $"{dt.Rows[0][0].ToString()} {Base64Encode(Convert.ToDateTime(dt.Rows[0][1]).ToString("s"))} {dt.Rows[0][2].ToString()} {Util.getFieldBool(dt.Rows[0], "Demo").ToString()}";
                            }
                            catch (Exception)
                            {
                                klas.kapat();
                            }
                            adapter.Dispose();
                            baglan.Close();
                            baglan.Dispose();
                        }
                    }
                }
                return servisYolu;
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        [Route("api/keycheck")]
        [AllowAnonymous]
        [HttpGet]
        public bool KeyCheck([FromUri] string key)
        {
            parametreler.Clear();
            parametreler.Add(new Prm("KEY", key));
            try
            {
                prms.Clear();
                string constr =
                    "RGF0YSBTb3VyY2U9bXNzcWwyLmJpcmhvc3QubmV0XG1zc3FsMjAxMjtJbml0aWFsIENhdGFsb2c9T3JqaW5fTXVzdGVyaV9XZWJTZXJ2aXM7IEludGVncmF0ZWQgU2VjdXJpdHk9ZmFsc2U7IFVzZXIgSUQ9b3JqaW5fc2VydmlzO1Bhc3N3b3JkPXNlcnZpc2RiMTMy";
                constr = Base64Decode(constr);
                string sql = "SELECT COUNT(*) from dbo.ServisYollari where FirmaAdi = @KEY";
                using (var baglan = new SqlConnection(constr))
                {
                    return baglan.QueryFirst<int>(sql, new {@KEY = key}) > 0;
                }
            }
            catch (Exception)
            {
                klas.kapat();
                return false;
            }
        }


        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string plainText)
        {
            return System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(plainText));
        }




        [Route("api/IsEmriFiltreData")]
        [HttpGet]
        public IsEmriFiltreData IsEmriFiltreData(int kulID)
        {
            var klas = new Util();
            var prms = new DynamicParameters();

            prms.Add("KUL_ID", kulID);

       

            var query = @"SELECT * FROM orjin.TB_ISEMRI_TIP WHERE IMT_AKTIF = 1;
                          SELECT * FROM orjin.TB_LOKASYON WHERE orjin.UDF_LOKASYON_YETKI_KONTROL(TB_LOKASYON_ID,@KUL_ID) = 1 AND LOK_AKTIF = 1;
                          SELECT * FROM orjin.TB_ATOLYE WHERE orjin.UDF_ATOLYE_YETKI_KONTROL(TB_ATOLYE_ID, @KUL_ID) = 1 AND ATL_AKTIF = 1;
                          SELECT * FROM orjin.TB_PERSONEL WHERE PRS_AKTIF = 1;
                          SELECT * FROM orjin.TB_PROJE WHERE PRJ_AKTIF = 1;
";
            IsEmriFiltreData entity = new IsEmriFiltreData();
            using (var cnn = klas.baglan())
            {
                var result = cnn.QueryMultiple(query, prms);
                entity.IsEmriTipList = result.Read<IsEmriTip>().ToList();
                entity.LokasyonList = result.Read<Lokasyon>().ToList();
                entity.AtolyeList = result.Read<Atolye>().ToList();
                entity.PersonelList = result.Read<Personel>().ToList();
                entity.ProjeList = result.Read<Proje>().ToList();
                return entity;
            }
        }


        [Route("api/IsEmriKartiData")]
        [HttpGet]
        public IsEmriKartAcilis IsEmriKartData(int kulID, bool isNew)
        {
            var klas = new Util();
            var prms = new DynamicParameters();

            prms.Add("KUL_ID", kulID);
            prms.Add("KGPAZTIP", C.KGP_ARIZA_TIPLERI);
            prms.Add("KGPBKMTIP", C.KGP_BAKIM_TIPLERI);
            prms.Add("KGPISMDURUM", C.KGP_ISEMRI_DURUM);
            prms.Add("KGPAZNEDEN", C.KGP_ARIZA_NEDENLERI);
            prms.Add("KGPBKMNEDEN", C.KGP_BAKIM_NEDENLERI);
            prms.Add("KGPDRSNEDEN", C.KGP_DURUS_NEDENLERI);

            string query = "";
            if (isNew)
            {
                query += Queries.GENERATE_KOD;
                prms.Add("KOD", "ISM_ISEMRI_NO");
            }

            query += @"   SELECT * FROM orjin.TB_ISEMRI_TIP WHERE IMT_AKTIF = 1;
                                SELECT * FROM orjin.TB_LOKASYON WHERE orjin.UDF_LOKASYON_YETKI_KONTROL(TB_LOKASYON_ID,@KUL_ID) = 1 AND LOK_AKTIF = 1;
                                SELECT * FROM orjin.TB_ATOLYE WHERE orjin.UDF_ATOLYE_YETKI_KONTROL(TB_ATOLYE_ID, @KUL_ID) = 1 AND ATL_AKTIF = 1;
                                SELECT * FROM orjin.TB_MASRAF_MERKEZ WHERE  MAM_AKTIF = 1;
                                SELECT * FROM orjin.TB_IS_TANIM where IST_DURUM ='BAKIM';
                                SELECT * FROM orjin.TB_IS_TANIM where IST_DURUM ='ARIZA';
                                SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP= @KGPAZTIP AND KOD_AKTIF = 1;
                                SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP= @KGPBKMTIP AND KOD_AKTIF = 1;
                                SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP= @KGPISMDURUM AND KOD_AKTIF = 1;
                                SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP= @KGPAZNEDEN AND KOD_AKTIF = 1;
                                SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP= @KGPBKMNEDEN AND KOD_AKTIF = 1; 
                                SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP= @KGPDRSNEDEN AND KOD_AKTIF = 1; 
                                SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP='50010' AND KOD_AKTIF = 1;
                                SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP='50011' AND KOD_AKTIF = 1;
                                SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP='50012' AND KOD_AKTIF = 1;
                                SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP='50013' AND KOD_AKTIF = 1;
                                SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP='50014' AND KOD_AKTIF = 1;
                                SELECT * FROM orjin.TB_SERVIS_ONCELIK WHERE SOC_AKTIF = 1;
                                SELECT * FROM orjin.TB_PROJE WHERE PRJ_AKTIF = 1;
                                SELECT * FROM orjin.TB_OZEL_ALAN WHERE OZL_FORM = 'ISEMRI';
                                SELECT coalesce((SELECT 
                                                      CASE
                                                      WHEN PRM_DEGER = 'True' THEN 1
                                                      ELSE 0
                                                      END  
                                                      FROM orjin.TB_PARAMETRE WHERE PRM_KOD='320119'),0) ;
";
            IsEmriKartAcilis entity = new IsEmriKartAcilis();
            using (var cnn = klas.baglan())
            {
                var result = cnn.QueryMultiple(query, prms);
                if (isNew)
                    entity.NEW_ISM_KOD = result.ReadFirstOrDefault<string>();
                entity.IsEmriTipList = result.Read<IsEmriTip>().ToList();
                entity.VarsayilanIsemriTipi = entity.IsEmriTipList.First(x => x.IMT_VARSAYILAN);
                entity.LokasyonList = result.Read<Lokasyon>().ToList();
                entity.AtolyeList = result.Read<Atolye>().ToList();
                entity.MasrafMerkeziList = result.Read<MasrafMerkezi>().ToList();
                entity.BakimTanimlari = result.Read<IsTanim>().ToList();
                entity.ArizaTanimlari = result.Read<IsTanim>().ToList();
                entity.ArizaTipList = result.Read<Kod>().ToList();
                entity.BakimTipList = result.Read<Kod>().ToList();
                entity.DurumList = result.Read<Kod>().ToList();
                entity.ArizaNedenList = result.Read<Kod>().ToList();
                entity.BakimNedenList = result.Read<Kod>().ToList();
                entity.DurusNedenList = result.Read<Kod>().ToList();
                entity.OZEL_ALAN_11_KOD_LIST = result.Read<Kod>().ToList();
                entity.OZEL_ALAN_12_KOD_LIST = result.Read<Kod>().ToList();
                entity.OZEL_ALAN_13_KOD_LIST = result.Read<Kod>().ToList();
                entity.OZEL_ALAN_14_KOD_LIST = result.Read<Kod>().ToList();
                entity.OZEL_ALAN_15_KOD_LIST = result.Read<Kod>().ToList();
                entity.OncelikList = result.Read<Oncelik>().ToList();
                entity.ProjeList = result.Read<Proje>().ToList();
                entity.OZEL_ALAN = result.ReadFirst<OzelAlan>();
                entity.MOBIL_BARKOD_AC_KAPA = result.ReadFirst<bool>();
                return entity;
            }
        }


        //Takvim List For Web App Version

        [Route("api/GetTakvimList")]
        [HttpGet]
        public Object GetTakvimList()
        {
            string query = "select * from orjin.TB_TAKVIM";
            List<Takvim> listem = new List<Takvim>();
            try
            {
                using(var cnn = klas.baglan())
                {
                    listem = cnn.Query<Takvim>(query).ToList();
                }
                return Json(new { Takvim_Liste = listem });
            }
            catch(Exception ex) 
            {
                return Json(new { error = ex.Message });
            }
        }

		//Talimat List For Web App Version

		[Route("api/GetTalimatList")]
		[HttpGet]
		public Object GetTalimatList()
		{
			string query = "select * from orjin.VW_TALIMAT";
			List<Talimat> listem = new List<Talimat>();
			try
			{
				using (var cnn = klas.baglan())
				{
					listem = cnn.Query<Talimat>(query).ToList();
				}
				return Json(new { Talimat_Liste = listem });
			}
			catch (Exception ex)
			{
				return Json(new { error = ex.Message });
			}
		}
	}



}