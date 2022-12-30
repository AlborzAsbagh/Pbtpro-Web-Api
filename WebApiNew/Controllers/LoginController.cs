using System;
using System.Data;
using System.Web.Http;
using WebApiNew.Models;
using Dapper;

namespace WebApiNew.Controllers
{
    public class LoginController : ApiController
    {
        Parametreler prms = new Parametreler();
        public Object Post([FromBody]Kullanici gelenEntity)
        {
            Util klas = new Util();
            Kullanici entity = new Kullanici();
            try
            {
                prms.Clear();
                prms.Add("KLL_KOD", gelenEntity.KLL_KOD);
                klas.MasterBaglantisi = true;
                string query = "SELECT * FROM orjin.TB_KULLANICI WHERE KLL_AKTIF = 1 AND KLL_DURUM = 'K' AND KLL_KOD =@KLL_KOD";
                DataRow drKul = klas.GetDataRow(query, prms.PARAMS);
                
                if (drKul != null)
                {
                    if ((drKul["KLL_SIFRE"] == DBNull.Value && gelenEntity.KLL_SIFRE == "") || drKul["KLL_SIFRE"].ToString() == gelenEntity.KLL_SIFRE)
                    {
                        entity.TB_KULLANICI_ID = Util.getFieldInt(drKul,"TB_KULLANICI_ID");
                        entity.KLL_PERSONEL_ID = Util.getFieldInt(drKul,"KLL_PERSONEL_ID");
                        entity.KLL_KOD = Util.getFieldString(drKul, "KLL_KOD");
                        entity.KLL_AKTIF = Util.getFieldBool(drKul,"KLL_AKTIF");
                        entity.KLL_TANIM = Util.getFieldString(drKul, "KLL_TANIM");
                        entity.KLL_MAIL = Util.getFieldString(drKul, "KLL_MAIL");
                        entity.apiVer = new indexController.AccessCheck().version;
                        entity.dbName = klas.GetDbName();
                        klas.MasterBaglantisi = false;
                        prms.Clear();
                        prms.Add("RSM_REF_ID", Util.getFieldInt(drKul,"KLL_PERSONEL_ID"));
                        entity.resimId = DBNull.Value == drKul["KLL_PERSONEL_ID"] ? -1 : Convert.ToInt32(klas.GetDataCell("SELECT COALESCE(TB_RESIM_ID,-1) FROM orjin.TB_RESIM WHERE RSM_VARSAYILAN= 1 AND RSM_REF_GRUP = 'PERSONEL' AND RSM_REF_ID = @RSM_REF_ID", prms.PARAMS));
                        string prsquery = @"SELECT * FROM orjin.VW_PERSONEL WHERE TB_PERSONEL_ID = @TB_PERSONEL_ID";
                        var util = new Util();
                        using (var cnn = util.baglan())
                        {
                            var personel = cnn.QueryFirstOrDefault<Personel>(prsquery, new { TB_PERSONEL_ID = entity.KLL_PERSONEL_ID });                            
                            entity.KLL_PERSONEL= personel;
                        }
                    }
                    else
                        return entity;
                }
                else
                    return entity;

                klas.MasterBaglantisi = false;
                return entity;
            }
            catch (Exception e)
            {
                klas.MasterBaglantisi = false;
                return e;
            }
        }



        public Kullanici CheckUser(string username, string password)
        {
            Util klas = new Util();
            using (var cnn=klas.baglan())
            {
                try
                {
                    var prms = new DynamicParameters();
                    string query;
                    if (string.IsNullOrEmpty(password))
                    {
                        prms.Add("KLL_KOD", username);
                        query = $"SELECT * FROM {klas.GetMasterDbName()}.orjin.TB_KULLANICI WHERE KLL_KOD =@KLL_KOD AND (KLL_SIFRE IS NULL OR KLL_SIFRE = '')";
                    }
                    else
                    {
                        prms.Add("KLL_KOD", username);
                        prms.Add("KLL_SIFRE", password);
                        query = $"SELECT * FROM {klas.GetMasterDbName()}.orjin.TB_KULLANICI WHERE KLL_KOD =@KLL_KOD AND KLL_SIFRE = @KLL_SIFRE";
                    }
                    var  drKul = cnn.QueryFirst<Kullanici>(query, prms);
                    return drKul;

                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        [Route("api/HDLogin")]
        [HttpPost]
        public Kullanici HDLogin([FromBody]Kullanici gelenEntity)
        {
            Util klas = new Util();
            Kullanici entity = new Kullanici();
            try
            {
                prms.Clear();
                prms.Add("ISK_KOD", gelenEntity.KLL_KOD);
                klas.MasterBaglantisi = false;
                string query = "select * from orjin.TB_IS_TALEBI_KULLANICI where ISK_KOD =@ISK_KOD";
                DataRow drKul = klas.GetDataRow(query, prms.PARAMS);

                if (drKul != null)
                {
                    if ((drKul["ISK_SIFRE"] == DBNull.Value && gelenEntity.KLL_SIFRE == "") || drKul["ISK_SIFRE"].ToString() == gelenEntity.KLL_SIFRE)
                    {
                        entity.TB_KULLANICI_ID = Convert.ToInt32(drKul["TB_IS_TALEBI_KULLANICI_ID"]);
                        entity.KLL_KOD = Util.getFieldString(drKul, "ISK_KOD");
                        entity.KLL_AKTIF = Util.getFieldBool(drKul, "ISK_AKTIF");
                        entity.KLL_TANIM = Util.getFieldString(drKul, "ISK_ISIM");
                        entity.KLL_MAIL = Util.getFieldString(drKul, "ISK_MAIL");
                        entity.lokasyonId = Util.getFieldInt(drKul, "ISK_LOKASYON_ID");
                    }
                    else
                        return entity;
                }
                else
                {
                    return entity;
                }
                klas.MasterBaglantisi = false;
                return entity;
            }
            catch (Exception)
            {
                klas.MasterBaglantisi = false;
                return entity;
            }
        }
    }
}
