using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Dapper;
using WebApiNew.Models;
using WebApiNew;
using WebApiNew.App_GlobalResources;
using WebApiNew.Filters;

namespace WebApiNew.Controllers
{
    
    [JwtAuthenticationFilter]
    public class YakitController : ApiController
    {
        Util klas = new Util();
        Parametreler prms = new Parametreler();

        #region yakit liste eski

        
        [Route("api/YakitHareketleriOld")]
        [HttpPost]
        public List<YakitHareket> YakitHareketleriOld([FromBody]Filtre filtre, [FromUri] int ilkDeger, [FromUri] int sonDeger)
        {
            List<YakitHareket> listem = new List<YakitHareket>();
            prms.Clear();
            string query = @"select * ,(select COALESCE(TB_RESIM_ID,0) from orjin.TB_RESIM where RSM_VARSAYILAN= 1 AND RSM_REF_GRUP = 'YAKITHRK' and RSM_REF_ID = TB_YAKIT_HRK_ID) as RSM_VAR_ID, stuff((SELECT ';' + CONVERT(varchar(11), R.TB_RESIM_ID)    FROM orjin.TB_RESIM R    WHERE R.RSM_REF_GRUP = 'YAKITHRK' and R.RSM_REF_ID = TB_YAKIT_HRK_ID    FOR XML PATH('')), 1, 1, '') [RSM_IDS]  from (select TOP 1000000 *,ROW_NUMBER() OVER(ORDER BY YKH_TARIH) AS satir,(select TOP 1 KOD_TANIM from orjin.TB_KOD where TB_KOD_ID =  (select TOP 1 STK_BIRIM_KOD_ID from orjin.TB_STOk WHERE TB_STOK_ID =YKH_STOK_ID)) AS YKH_BIRIM, ( select TOP 1 MKN_KOD + ' / ' + MKN_TANIM FROM orjin.TB_MAKINE WHERE TB_MAKINE_ID=YKH_MAKINE_ID) AS YKH_MAKINE from orjin.VW_YAKIT_HRK where 1=1 and YKH_MAKINE_ID IS NOT NULL ";

            if (filtre != null)
            {
                if (filtre.MakineID > 0)
                {
                    prms.Add("MAK_ID", filtre.MakineID);
                    query = query + " and YKH_MAKINE_ID=@MAK_ID";
                }
                if (filtre.LokasyonID > 0)
                {
                    prms.Add("LOK_ID", filtre.LokasyonID);
                    query = query + " and YKH_LOKASYON_ID=@LOK_ID";
                }
                //if (filtre.PersonelID > 0)
                //    query = query + " AND IST_TALEP_EDEN_ID = " + filtre.PersonelID;
                if (filtre.BasTarih != "" && filtre.BitTarih != "")
                {
                    prms.Add("BAS_TARIH", Convert.ToDateTime(filtre.BasTarih).ToString("yyyy-MM-dd"));
                    prms.Add("BIT_TARIH", Convert.ToDateTime(filtre.BitTarih).ToString("yyyy-MM-dd"));
                    query = query + " AND YKH_TARIH BETWEEN  @BAS_TARIH and @BIT_TARIH";
                }
                else
                if (filtre.BasTarih != "")
                {
                    prms.Add("BAS_TARIH", Convert.ToDateTime(filtre.BasTarih).ToString("yyyy-MM-dd"));
                    query = query + " AND YKH_TARIH >= @BAS_TARIH ";
                }
                else
                if (filtre.BitTarih != "")
                {
                    prms.Add("BIT_TARIH", Convert.ToDateTime(filtre.BitTarih).ToString("yyyy-MM-dd"));
                    query = query + " AND YKH_TARIH <= @BIT_TARIH";
                }

            }
            prms.Add("ILK_DEGER", ilkDeger);
            prms.Add("SON_DEGER", sonDeger);
            query = query + "  order by YKH_TARIH + YKH_SAAT desc) as tablom  where satir > @ILK_DEGER and satir <= @SON_DEGER";
            if (filtre != null)
            {
                if (filtre.Kelime != "")
                {
                    prms.Add("KELIME", filtre.Kelime);
                    query += @"  AND   (YKH_LOKASYON    like '%'+@KELIME+'%' OR 
                                        YKH_PROJE       like '%'+@KELIME+'%' OR 
                                        YKH_TANK        like '%'+@KELIME+'%' OR 
                                        YKH_MAKINE      like '%'+@KELIME+'%' OR 
                                        YKH_YAKIT       like '%'+@KELIME+'%')";
                }
            }
            DataTable dt = klas.GetDataTable(query, prms.PARAMS);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                YakitHareket entity = new YakitHareket();
                entity.TB_YAKIT_HRK_ID = Convert.ToInt32(dt.Rows[i]["TB_YAKIT_HRK_ID"]);
                entity.YKH_ACIKLAMA = dt.Rows[i]["YKH_ACIKLAMA"].ToString();
                entity.YKH_ALINAN_KM = Convert.ToInt32(dt.Rows[i]["YKH_ALINAN_KM"]);
                entity.YKH_DEPO_FULLENDI = dt.Rows[i]["YKH_DEPO_FULLENDI"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[i]["YKH_DEPO_FULLENDI"]) : false;
                entity.YKH_DEPO_ID = dt.Rows[i]["YKH_DEPO_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_DEPO_ID"]) : -1;
                entity.YKH_FARK_KM = dt.Rows[i]["YKH_FARK_KM"] != DBNull.Value ? Convert.ToDouble(dt.Rows[i]["YKH_FARK_KM"]) : 0;
                entity.YKH_FIYAT = dt.Rows[i]["YKH_FIYAT"] != DBNull.Value ? Convert.ToDouble(dt.Rows[i]["YKH_FIYAT"]) : 0;
                entity.YKH_OLUSTURAN_ID = dt.Rows[i]["YKH_OLUSTURAN_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_OLUSTURAN_ID"]) : -1;
                entity.YKH_OLUSTURMA_TARIH = Util.getFieldDateTime(dt.Rows[i],"YKH_OLUSTURMA_TARIH");
                entity.YKH_DEGISTIREN_ID = dt.Rows[i]["YKH_DEGISTIREN_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_DEGISTIREN_ID"]) : -1;
                entity.YKH_DEGISTIRME_TARIH = Util.getFieldDateTime(dt.Rows[i],"YKH_DEGISTIRME_TARIH");
                entity.YKH_LOKASYON_ID = dt.Rows[i]["YKH_LOKASYON_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_LOKASYON_ID"]) : -1;
                entity.YKH_MAKINE_ID = dt.Rows[i]["YKH_MAKINE_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_MAKINE_ID"]) : -1;
                entity.YKH_MIKTAR = dt.Rows[i]["YKH_MIKTAR"] != DBNull.Value ? Convert.ToDouble(dt.Rows[i]["YKH_MIKTAR"]) : 0;
                entity.YKH_SAAT = dt.Rows[i]["YKH_SAAT"].ToString();
                entity.YKH_SAYAC_BIRIM_ID = dt.Rows[i]["YKH_SAYAC_BIRIM_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_SAYAC_BIRIM_ID"]) : -1;
                entity.YKH_SAYAC_ID = dt.Rows[i]["YKH_SAYAC_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_SAYAC_ID"]) : -1;
                entity.YKH_SON_KM = dt.Rows[i]["YKH_SON_KM"] != DBNull.Value ? Convert.ToDouble(dt.Rows[i]["YKH_SON_KM"]) : 0;
                entity.YKH_STOK_ID = dt.Rows[i]["YKH_STOK_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_STOK_ID"]) : -1;
                entity.YKH_STOK_KULLANIM = dt.Rows[i]["YKH_STOK_KULLANIM"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[i]["YKH_STOK_KULLANIM"]) : false;
                entity.YKH_TARIH = Util.getFieldDateTime(dt.Rows[i],"YKH_TARIH");
                entity.YKH_TUTAR = dt.Rows[i]["YKH_TUTAR"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_TUTAR"]) : 0;
                entity.YKH_YAKIT_TIP_ID = dt.Rows[i]["YKH_YAKIT_TIP_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_YAKIT_TIP_ID"]) : -1;
                entity.YKH_SAYAC = dt.Rows[i]["YKH_SAYAC"].ToString();
                entity.YKH_SAYAC_BIRIM = dt.Rows[i]["YKH_SAYAC_BIRIM"].ToString();
                entity.YKH_YAKIT = dt.Rows[i]["YKH_YAKIT"].ToString();
                entity.YKH_TANK = dt.Rows[i]["YKH_TANK"].ToString();
                entity.YKH_MAKINE = dt.Rows[i]["YKH_MAKINE"].ToString();
                entity.YKH_BIRIM = dt.Rows[i]["YKH_BIRIM"].ToString();
                entity.YKH_LOKASYON = dt.Rows[i]["YKH_LOKASYON"].ToString();
                entity.ResimVarsayilanID = dt.Rows[i]["RSM_VAR_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["RSM_VAR_ID"]) : -1;
                List<int> resimIdler = new List<int>();
                if (dt.Rows[i]["RSM_IDS"] != DBNull.Value)
                {
                    string[] ids = dt.Rows[i]["RSM_IDS"].ToString().Split(';');
                    for (int j = 0; j < ids.Length; j++)
                    {
                        resimIdler.Add(Convert.ToInt32(ids[j]));
                    }
                }
                entity.ResimIDleri = resimIdler;
                listem.Add(entity);
            }
            return listem;
        }


            #endregion

        [Route("api/YakitHareketleri")]
        [HttpPost]
        public List<YakitHareket> YakitHareketleri([FromBody]Filtre filtre, [FromUri] int page, [FromUri] int pageSize)
        {
            int ilkdeger = page * pageSize;
            int sondeger = ilkdeger + pageSize;
            List<YakitHareket> listem = new List<YakitHareket>();
            prms.Clear();
            string query = @";WITH mTable AS
                (select vwy.* 
                ,(select COALESCE(TB_RESIM_ID,0) from orjin.TB_RESIM where RSM_VARSAYILAN= 1 AND RSM_REF_GRUP = 'YAKITHRK' and RSM_REF_ID = TB_YAKIT_HRK_ID) as RSM_VAR_ID,
                stuff((SELECT ';' + CONVERT(varchar(11), R.TB_RESIM_ID)    FROM orjin.TB_RESIM R    WHERE R.RSM_REF_GRUP = 'YAKITHRK' and R.RSM_REF_ID = TB_YAKIT_HRK_ID    FOR XML PATH('')), 1, 1, '') [RSM_IDS],  
                (select TOP 1 KOD_TANIM from orjin.TB_KOD where TB_KOD_ID = TBS.STK_BIRIM_KOD_ID) AS YKH_BIRIM, 
                +'['+tbm.MKN_KOD +'] '+ tbm.MKN_TANIM AS YKH_MAKINE,
                ROW_NUMBER() OVER (ORDER BY YKH_TARIH DESC, YKH_SAAT DESC) AS RowNum 
                from orjin.VW_YAKIT_HRK vwy INNER JOIN orjin.TB_STOK TBS ON TBS.TB_STOK_ID = vwy.YKH_STOK_ID INNER JOIN orjin.TB_MAKINE tbm ON tbm.TB_MAKINE_ID = vwy.YKH_MAKINE_ID where 1=1 ";

            if (filtre != null)
            {
                if (filtre.MakineID > 0)
                {
                    prms.Add("YKH_MAKINE_ID", filtre.MakineID);
                    query = query + " and YKH_MAKINE_ID = @YKH_MAKINE_ID";
                }
                if (filtre.LokasyonID > 0)
                {
                    prms.Add("YKH_LOKASYON_ID", filtre.LokasyonID);
                    query = query + " and YKH_LOKASYON_ID = @YKH_LOKASYON_ID";
                }
                if (filtre.DepoID > 0)
                {
                    prms.Add("YKH_DEPO_ID", filtre.DepoID);
                    query = query + " and YKH_DEPO_ID = @YKH_DEPO_ID";
                }
                if (filtre.isEmriTipId > 0)//YAKIT TİPİ
                {
                    prms.Add("STK_TIP_KOD_ID", filtre.isEmriTipId);
                    query = query + " and tbs.STK_TIP_KOD_ID = @STK_TIP_KOD_ID";
                }
                if (filtre.value1 > 0 && filtre.value2 > 0)
                {
                    prms.Add("VAL1", filtre.value1);
                    prms.Add("VAL2", filtre.value2);
                    query += " AND YKH_MIKTAR BETWEEN @VAL1 AND @VAL2";
                }
                else
                {
                    if (filtre.value1 > 0)
                    {
                        prms.Add("VAL1", filtre.value1);
                        query += " AND YKH_MIKTAR > @VAL1";
                    }
                    if (filtre.value2 > 0)
                    {
                        prms.Add("VAL2", filtre.value2);
                        query += " AND YKH_MIKTAR < @VAL2";
                    }
                }
                if (filtre.BasTarih != "" && filtre.BitTarih != "")
                {
                    prms.Add("BAS_TARIH", Convert.ToDateTime(filtre.BasTarih).ToString("yyyy-MM-dd"));
                    prms.Add("BIT_TARIH", Convert.ToDateTime(filtre.BitTarih).ToString("yyyy-MM-dd"));
                    query = query + " AND YKH_TARIH BETWEEN  @BAS_TARIH and @BIT_TARIH";
                }
                else
                if (filtre.BasTarih != "")
                {
                    prms.Add("BAS_TARIH", Convert.ToDateTime(filtre.BasTarih).ToString("yyyy-MM-dd"));
                    query = query + " AND YKH_TARIH >= @BAS_TARIH ";
                }
                else
                if (filtre.BitTarih != "")
                {
                    prms.Add("BIT_TARIH", Convert.ToDateTime(filtre.BitTarih).ToString("yyyy-MM-dd"));
                    query = query + " AND YKH_TARIH <= @BIT_TARIH";
                }
                if (filtre.Kelime != "")
                {
                    prms.Add("KELIME", filtre.Kelime);
                    query += @"  AND   (YKH_LOKASYON    like '%'+@KELIME+'%' OR 
                                        YKH_PROJE       like '%'+@KELIME+'%' OR 
                                        YKH_PERSONEL    like '%'+@KELIME+'%' OR 
                                        YKH_TANK        like '%'+@KELIME+'%' OR 
                                        tbm.MKN_TANIM   like '%'+@KELIME+'%' OR 
                                        tbm.MKN_KOD     like '%'+@KELIME+'%' OR 
                                        YKH_YAKIT       like '%'+@KELIME+'%')";
                }
            }

            prms.Add("ILK_DEGER", ilkdeger);
            prms.Add("SON_DEGER", sondeger);
            query = query + " ) SELECT * FROM mTable WHERE RowNum > @ILK_DEGER AND RowNum <= @SON_DEGER";

            DataTable dt = klas.GetDataTable(query, prms.PARAMS);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                YakitHareket entity = new YakitHareket();
                entity.TB_YAKIT_HRK_ID = Convert.ToInt32(dt.Rows[i]["TB_YAKIT_HRK_ID"]);
                entity.YKH_ACIKLAMA = Util.getFieldString(dt.Rows[i], "YKH_ACIKLAMA");
                entity.YKH_OLUSTURMA_TARIH = Util.getFieldDateTime(dt.Rows[i], "YKH_OLUSTURMA_TARIH");
                entity.YKH_SAYAC = Util.getFieldString(dt.Rows[i], "YKH_SAYAC");
                entity.YKH_SAYAC_BIRIM = Util.getFieldString(dt.Rows[i], "YKH_SAYAC_BIRIM");
                entity.YKH_YAKIT = Util.getFieldString(dt.Rows[i], "YKH_YAKIT");
                entity.YKH_TARIH = Util.getFieldDateTime(dt.Rows[i], "YKH_TARIH");
                entity.YKH_TANK = Util.getFieldString(dt.Rows[i], "YKH_TANK");
                entity.YKH_MAKINE = Util.getFieldString(dt.Rows[i], "YKH_MAKINE");
                entity.YKH_BIRIM = Util.getFieldString(dt.Rows[i], "YKH_BIRIM");
                entity.YKH_SAAT = Util.getFieldString(dt.Rows[i], "YKH_SAAT");
                entity.YKH_LOKASYON = Util.getFieldString(dt.Rows[i], "YKH_LOKASYON");
                entity.YKH_DEGISTIRME_TARIH = Util.getFieldDateTime(dt.Rows[i], "YKH_DEGISTIRME_TARIH");
                entity.YKH_PERSONEL = Util.getFieldString(dt.Rows[i], "YKH_PERSONEL");
                entity.YKH_PROJE = Util.getFieldString(dt.Rows[i], "YKH_PROJE");
                entity.YKH_MASRAF_MERKEZI = Util.getFieldString(dt.Rows[i], "YKH_MASRAF_MERKEZI");
                entity.YKH_ALINAN_KM = Util.getFieldInt(dt.Rows[i], "YKH_ALINAN_KM");
                entity.YKH_PROJE_ID = Util.getFieldInt(dt.Rows[i], "YKH_PROJE_ID");
                entity.YKH_MASRAF_MERKEZI_ID = Util.getFieldInt(dt.Rows[i], "YKH_MASRAF_MERKEZI_ID");
                entity.YKH_PERSONEL_ID = Util.getFieldInt(dt.Rows[i], "YKH_PERSONEL_ID");
                entity.YKH_DEPO_FULLENDI = dt.Rows[i]["YKH_DEPO_FULLENDI"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[i]["YKH_DEPO_FULLENDI"]) : false;
                entity.YKH_DEPO_ID = dt.Rows[i]["YKH_DEPO_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_DEPO_ID"]) : -1;
                entity.YKH_FARK_KM = dt.Rows[i]["YKH_FARK_KM"] != DBNull.Value ? Convert.ToDouble(dt.Rows[i]["YKH_FARK_KM"]) : 0;
                entity.YKH_FIYAT = dt.Rows[i]["YKH_FIYAT"] != DBNull.Value ? Convert.ToDouble(dt.Rows[i]["YKH_FIYAT"]) : 0;
                entity.YKH_OLUSTURAN_ID = dt.Rows[i]["YKH_OLUSTURAN_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_OLUSTURAN_ID"]) : -1;
                entity.YKH_DEGISTIREN_ID = dt.Rows[i]["YKH_DEGISTIREN_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_DEGISTIREN_ID"]) : -1;
                entity.YKH_LOKASYON_ID = dt.Rows[i]["YKH_LOKASYON_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_LOKASYON_ID"]) : -1;
                entity.YKH_MAKINE_ID = dt.Rows[i]["YKH_MAKINE_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_MAKINE_ID"]) : -1;
                entity.YKH_MIKTAR = dt.Rows[i]["YKH_MIKTAR"] != DBNull.Value ? Convert.ToDouble(dt.Rows[i]["YKH_MIKTAR"]) : 0;
                entity.YKH_SAYAC_BIRIM_ID = dt.Rows[i]["YKH_SAYAC_BIRIM_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_SAYAC_BIRIM_ID"]) : -1;
                entity.YKH_SAYAC_ID = dt.Rows[i]["YKH_SAYAC_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_SAYAC_ID"]) : -1;
                entity.YKH_SON_KM = dt.Rows[i]["YKH_SON_KM"] != DBNull.Value ? Convert.ToDouble(dt.Rows[i]["YKH_SON_KM"]) : 0;
                entity.YKH_STOK_ID = dt.Rows[i]["YKH_STOK_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_STOK_ID"]) : -1;
                entity.YKH_STOK_KULLANIM = dt.Rows[i]["YKH_STOK_KULLANIM"] != DBNull.Value ? Convert.ToBoolean(dt.Rows[i]["YKH_STOK_KULLANIM"]) : false;
                entity.YKH_TUTAR = dt.Rows[i]["YKH_TUTAR"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_TUTAR"]) : 0;
                entity.YKH_YAKIT_TIP_ID = dt.Rows[i]["YKH_YAKIT_TIP_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["YKH_YAKIT_TIP_ID"]) : -1;
                entity.ResimVarsayilanID = dt.Rows[i]["RSM_VAR_ID"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["RSM_VAR_ID"]) : -1;
                List<int> resimIdler = new List<int>();
                if (dt.Rows[i]["RSM_IDS"] != DBNull.Value)
                {
                    string[] ids = dt.Rows[i]["RSM_IDS"].ToString().Split(';');
                    for (int j = 0; j < ids.Length; j++)
                    {
                        resimIdler.Add(Convert.ToInt32(ids[j]));
                    }
                }
                entity.ResimIDleri = resimIdler;
                listem.Add(entity);
            }
            return listem;
        }

        [Route("api/YakitKayit")]
        [HttpPost]
        public Bildirim YakitKayit([FromBody] YakitHareket entity)
        {
            Bildirim bildirimEntity = new Bildirim();
            prms.Clear();
            prms.Add("YKH_MAKINE_ID", entity.YKH_MAKINE_ID);
            try
            {
                int depoHacim, yktID, lokid = 0; double yktFiyat = 0;
                DataRow drMakine = klas.GetDataRow("select *, coalesce(MKN_YAKIT_DEPO_HACMI,0) as MKN_YAKIT_DEPO_HACMI_COAL from orjin.TB_MAKINE where TB_MAKINE_ID = @YKH_MAKINE_ID", prms.PARAMS);
                depoHacim = Convert.ToInt32(drMakine["MKN_YAKIT_DEPO_HACMI_COAL"]);
                yktID = Convert.ToInt32(drMakine["MKN_YAKIT_TIP_ID"]);
                lokid = Convert.ToInt32(drMakine["MKN_LOKASYON_ID"]);
                prms.Clear();
                prms.Add("YKT_ID", yktID);
                yktFiyat = Convert.ToDouble(klas.GetDataCell("select COALESCE(STK_CIKIS_FIYAT_DEGERI,0) FROM orjin.TB_STOK WHERE TB_STOK_ID = @YKT_ID", prms.PARAMS));
                entity.YKH_FIYAT = yktFiyat;
                entity.YKH_TUTAR = yktFiyat * entity.YKH_MIKTAR;
                entity.YKH_STOK_ID = yktID;

                if (entity.TB_YAKIT_HRK_ID < 1)
                {// yakıt girişi
                    SayacController syc = new SayacController();
                    Sayac sycEntity = syc.Get(entity.YKH_MAKINE_ID).Where(a => a.MES_VARSAYILAN == true).FirstOrDefault();
                    string query = @"INSERT INTO orjin.TB_YAKIT_HRK
                                                   (YKH_MAKINE_ID
                                                   ,YKH_TARIH
                                                   ,YKH_SAAT
                                                   ,YKH_SON_KM
                                                   ,YKH_ALINAN_KM
                                                   ,YKH_FARK_KM
                                                   ,YKH_MIKTAR
                                                   ,YKH_FIYAT
                                                   ,YKH_KDV_TUTAR
                                                   ,YKH_TUTAR    
                                                   ,YKH_STOK_KULLANIM
                                                   ,YKH_STOK_ID
                                                   ,YKH_DEPO_ID
                                                   ,YKH_ISTASYON_KOD_ID
                                                   ,YKH_PERSONEL_ID
                                                   ,YKH_GUZERGAH_ID
                                                   ,YKH_YAKIT_TIP_ID
                                                   ,YKH_FIRMA_ID
                                                   ,YKH_DEPO_FULLENDI
                                                   ,YKH_DEPO_YAKIT_MIKTAR         
                                                   ,YKH_LOKASYON_ID         
                                                   ,YKH_OLUSTURAN_ID
                                                   ,YKH_OLUSTURMA_TARIH                                                  
                                                   ,YKH_SAYAC_ID
                                                   ,YKH_SAYAC_BIRIM_ID          
                                                   ,YKH_HAKEDIS_ID                                       
                                                   ,YKH_BEKLENEN_TUKETIM_ORAN                              
                                                   ,YKH_PROJE_ID  
                                                   ,YKH_MASRAF_MERKEZI_ID  
                                        ) values   (@YKH_MAKINE_ID
                                                   ,@YKH_TARIH
                                                   ,@YKH_SAAT
                                                   ,@YKH_SON_KM
                                                   ,@YKH_ALINAN_KM
                                                   ,@YKH_FARK_KM
                                                   ,@YKH_MIKTAR
                                                   ,@YKH_FIYAT
                                                   ,@YKH_KDV_TUTAR
                                                   ,@YKH_TUTAR    
                                                   ,@YKH_STOK_KULLANIM
                                                   ,@YKH_STOK_ID
                                                   ,@YKH_DEPO_ID
                                                   ,-1
                                                   ,@YKH_PERSONEL_ID
                                                   ,-1
                                                   ,-1
                                                   ,-1
                                                   ,@YKH_DEPO_FULLENDI
                                                   ,@YKH_DEPO_YAKIT_MIKTAR         
                                                   ,@YKH_LOKASYON_ID         
                                                   ,@YKH_OLUSTURAN_ID
                                                   ,@YKH_OLUSTURMA_TARIH                                                  
                                                   ,@YKH_SAYAC_ID
                                                   ,@YKH_SAYAC_BIRIM_ID          
                                                   ,0
                                                   ,(SELECT MKN_YAKIT_ONGORULEN_MAX FROM orjin.TB_MAKINE WHERE TB_MAKINE_ID=@YKH_MAKINE_ID)
                                                   ,@YKH_PROJE_ID
                                                   ,@YKH_MASRAF_MERKEZI_ID)
                                                   ";
                    prms.Clear();
                    prms.Add("@YKH_MAKINE_ID", entity.YKH_MAKINE_ID);
                    prms.Add("@YKH_PERSONEL_ID", entity.YKH_PERSONEL_ID);
                    prms.Add("@YKH_PROJE_ID", entity.YKH_PROJE_ID);
                    prms.Add("@YKH_MASRAF_MERKEZI_ID", entity.YKH_MASRAF_MERKEZI_ID);
                    prms.Add("@YKH_TARIH", entity.YKH_TARIH);
                    prms.Add("@YKH_SAAT", entity.YKH_SAAT);
                    prms.Add("@YKH_SON_KM", entity.YKH_SON_KM);
                    prms.Add("@YKH_ALINAN_KM", entity.YKH_ALINAN_KM);
                    prms.Add("@YKH_FARK_KM", entity.YKH_FARK_KM);
                    prms.Add("@YKH_MIKTAR", entity.YKH_MIKTAR);
                    prms.Add("@YKH_FIYAT", entity.YKH_FIYAT);
                    prms.Add("@YKH_KDV_TUTAR", (entity.YKH_MIKTAR * entity.YKH_FIYAT * 0.18));
                    prms.Add("@YKH_TUTAR", entity.YKH_TUTAR);
                    prms.Add("@YKH_STOK_KULLANIM", entity.YKH_STOK_KULLANIM);
                    prms.Add("@YKH_STOK_ID", entity.YKH_STOK_ID);
                    prms.Add("@YKH_DEPO_ID", entity.YKH_DEPO_ID);
                    prms.Add("@YKH_DEPO_FULLENDI", entity.YKH_DEPO_FULLENDI);
                    prms.Add("@YKH_DEPO_YAKIT_MIKTAR", depoHacim != 0 ? depoHacim - entity.YKH_MIKTAR : 0);
                    prms.Add("@YKH_LOKASYON_ID", lokid);
                    prms.Add("@YKH_OLUSTURAN_ID", entity.YKH_OLUSTURAN_ID);
                    prms.Add("@YKH_OLUSTURMA_TARIH", DateTime.Now);

                    if (sycEntity != null)
                    {
                        prms.Add("@YKH_SAYAC_ID", sycEntity.TB_SAYAC_ID);
                        prms.Add("@YKH_SAYAC_BIRIM_ID", sycEntity.MES_BIRIM_KOD_ID);
                        entity.YKH_SAYAC_ID = sycEntity.TB_SAYAC_ID;
                        entity.YKH_SAYAC_BIRIM_ID = sycEntity.MES_BIRIM_KOD_ID;
                    }
                    else
                    {
                        prms.Add("@YKH_SAYAC_ID", -1);
                        prms.Add("@YKH_SAYAC_BIRIM_ID", -1);
                        entity.YKH_SAYAC_ID = -1;
                        entity.YKH_SAYAC_BIRIM_ID = -1;
                    }
                    klas.cmd(query, prms.PARAMS);

                    entity.TB_YAKIT_HRK_ID = Convert.ToInt32(klas.GetDataCell("select max(TB_YAKIT_HRK_ID) from orjin.TB_YAKIT_HRK", new List<Prm>()));
                    YakitHareketSapmaHesapla(entity.TB_YAKIT_HRK_ID);
                    // Varsa sayac guncelemesi yapılıyor.
                    if (entity.YKH_SAYAC_ID > 0)
                    {
                        SayacOkuma sycKayitEntity = new SayacOkuma();
                        sycKayitEntity.SYO_SAYAC_ID = entity.YKH_SAYAC_ID;
                        sycKayitEntity.SYO_TARIH = entity.YKH_TARIH;
                        sycKayitEntity.SYO_SAAT = entity.YKH_SAAT;
                        sycKayitEntity.SYO_OKUNAN_SAYAC = entity.YKH_ALINAN_KM;
                        sycKayitEntity.SYO_FARK_SAYAC = entity.YKH_FARK_KM;
                        sycKayitEntity.SYO_ACIKLAMA = "YAKIT";
                        sycKayitEntity.SYO_ELLE_GIRIS = false;
                        sycKayitEntity.SYO_HAREKET_TIP = "GÜNCELLEME";
                        sycKayitEntity.SYO_REF_ID = entity.TB_YAKIT_HRK_ID;
                        sycKayitEntity.SYO_REF_GRUP = "YAKIT";
                        sycKayitEntity.SYO_MAKINE_PUANTAJ_ID = -1;
                        sycKayitEntity.SYO_PROJE_ID = entity.YKH_PROJE_ID;
                        sycKayitEntity.SYO_LOKASYON_ID = entity.YKH_LOKASYON_ID;
                        sycKayitEntity.SYO_OLUSTURAN_ID = entity.YKH_OLUSTURAN_ID;
                        SayacController sycController = new SayacController();
                        sycController.SayacKayit(sycKayitEntity);
                    }

                    bildirimEntity.Id = Convert.ToInt32(klas.GetDataCell("select max(TB_YAKIT_HRK_ID) from orjin.TB_YAKIT_HRK", new List<Prm>()));
                    bildirimEntity.Aciklama = "Yakıt kaydı başarılı bir şekilde gerçekleştirildi.";
                    bildirimEntity.MsgId = Bildirim.MSG_YKT_KAYIT_OK;
                    bildirimEntity.Durum = true;
                }
                else // güncelle 
                {

                    string query = @"UPDATE orjin.TB_YAKIT_HRK SET 
                                                    YKH_MAKINE_ID                =@YKH_MAKINE_ID
                                                   ,YKH_TARIH                    =@YKH_TARIH
                                                   ,YKH_SAAT                     =@YKH_SAAT
                                                   ,YKH_SON_KM                   =@YKH_SON_KM
                                                   ,YKH_ALINAN_KM                =@YKH_ALINAN_KM
                                                   ,YKH_FARK_KM                  =@YKH_FARK_KM
                                                   ,YKH_MIKTAR                   =@YKH_MIKTAR
                                                   ,YKH_FIYAT                    =@YKH_FIYAT
                                                   ,YKH_KDV_TUTAR                =@YKH_KDV_TUTAR
                                                   ,YKH_TUTAR                    =@YKH_TUTAR    
                                                   ,YKH_STOK_KULLANIM            =@YKH_STOK_KULLANIM
                                                   ,YKH_STOK_ID                  =@YKH_STOK_ID
                                                   ,YKH_DEPO_ID                  =@YKH_DEPO_ID                                                 
                                                   ,YKH_DEPO_FULLENDI            =@YKH_DEPO_FULLENDI
                                                   ,YKH_DEPO_YAKIT_MIKTAR        =@YKH_DEPO_YAKIT_MIKTAR 
                                                   ,YKH_LOKASYON_ID              =@YKH_LOKASYON_ID      
                                                   ,YKH_DEGISTIREN_ID            =@YKH_DEGISTIREN_ID
                                                   ,YKH_DEGISTIRME_TARIH         =@YKH_DEGISTIRME_TARIH                                          
                                                   ,YKH_SAYAC_ID                 =@YKH_SAYAC_ID 
                                                   ,YKH_SAYAC_BIRIM_ID           =@YKH_SAYAC_BIRIM_ID   
                                                   ,YKH_PERSONEL_ID              =@YKH_PERSONEL_ID   
                                                   ,YKH_MASRAF_MERKEZI_ID        =@YKH_MASRAF_MERKEZI_ID   
                                                   ,YKH_PROJE_ID                 =@YKH_PROJE_ID   
                                                   ,YKH_HAKEDIS_ID               =0 WHERE TB_YAKIT_HRK_ID=@TB_YAKIT_HRK_ID ";

                    prms.Clear();
                    prms.Add("@TB_YAKIT_HRK_ID", entity.TB_YAKIT_HRK_ID);
                    prms.Add("@YKH_MAKINE_ID", entity.YKH_MAKINE_ID);
                    prms.Add("@YKH_TARIH", entity.YKH_TARIH);
                    prms.Add("@YKH_SAAT", entity.YKH_SAAT);
                    prms.Add("@YKH_SON_KM", entity.YKH_SON_KM);
                    prms.Add("@YKH_ALINAN_KM", entity.YKH_ALINAN_KM);
                    prms.Add("@YKH_FARK_KM", entity.YKH_FARK_KM);
                    prms.Add("@YKH_MIKTAR", entity.YKH_MIKTAR);
                    prms.Add("@YKH_FIYAT", entity.YKH_FIYAT);
                    prms.Add("@YKH_KDV_TUTAR", (entity.YKH_MIKTAR * entity.YKH_FIYAT * 0.18));
                    prms.Add("@YKH_TUTAR", entity.YKH_TUTAR);
                    prms.Add("@YKH_STOK_KULLANIM", entity.YKH_STOK_KULLANIM);
                    prms.Add("@YKH_STOK_ID", entity.YKH_STOK_ID);
                    prms.Add("@YKH_DEPO_ID", entity.YKH_DEPO_ID);
                    prms.Add("@YKH_DEPO_FULLENDI", entity.YKH_DEPO_FULLENDI);
                    prms.Add("@YKH_DEPO_YAKIT_MIKTAR", depoHacim != 0 ? depoHacim - entity.YKH_MIKTAR : 0);
                    prms.Add("@YKH_LOKASYON_ID", lokid);
                    prms.Add("@YKH_DEGISTIREN_ID", entity.YKH_DEGISTIREN_ID);
                    prms.Add("@YKH_DEGISTIRME_TARIH", DateTime.Now);
                    prms.Add("@YKH_SAYAC_ID", entity.YKH_SAYAC_ID);
                    prms.Add("@YKH_PERSONEL_ID", entity.YKH_PERSONEL_ID);
                    prms.Add("@YKH_PROJE_ID", entity.YKH_PROJE_ID);
                    prms.Add("@YKH_MASRAF_MERKEZI_ID", entity.YKH_MASRAF_MERKEZI_ID);
                    prms.Add("@YKH_SAYAC_BIRIM_ID", entity.YKH_SAYAC_BIRIM_ID);
                    klas.cmd(query, prms.PARAMS);

                    // harcanan miktar ,sapma oranları vb.. hesaplamalar yapılıyor
                    YakitHareketSapmaHesapla(entity.TB_YAKIT_HRK_ID);

                    bildirimEntity.Aciklama = "Yakıt güncelleme başarılı bir şekilde gerçekleştirildi.";
                    bildirimEntity.MsgId = Bildirim.MSG_YKT_GUNCELLE_OK;
                    bildirimEntity.Durum = true;
                }

                if (entity.YKH_STOK_KULLANIM && entity.YKH_STOK_ID > 0 && entity.YKH_DEPO_ID > 0)
                {
                    StokHareketIslemi(entity);
                }

            }
            catch (Exception e)
            {
                klas.kapat();
                bildirimEntity.Aciklama = String.Format(Localization.errorFormatted, e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }
            return bildirimEntity;
        }

        private void YakitHareketSapmaHesapla(int hareketID)
        {
            try
            {
                string query = @" --Harcanan yakıt mıktarlarını hesaplama
                            UPDATE orjin.TB_YAKIT_HRK SET YKH_HARCANAN_YAKIT_MIKTAR  = YKH_MIKTAR ,YKH_ONGORULEN_LT  = YKH_FARK_KM * YKH_BEKLENEN_TUKETIM_ORAN WHERE YKH_DEPO_FULLENDI = 1  AND YKH_FARK_KM <> 0 AND YKH_BEKLENEN_TUKETIM_ORAN <> 0  and TB_YAKIT_HRK_ID = @HRK_ID

							UPDATE orjin.TB_YAKIT_HRK SET YKH_HARCANAN_YAKIT_MIKTAR  =  
                             (Select TOP(1) b.YKH_MIKTAR from orjin.TB_YAKIT_HRK as b where b.YKH_MAKINE_ID= orjin.TB_YAKIT_HRK.YKH_MAKINE_ID 
                            and (convert(datetime, b.YKH_TARIH + ' ' + b.YKH_SAAT, 120) < convert(datetime, orjin.TB_YAKIT_HRK.YKH_TARIH + ' ' + orjin.TB_YAKIT_HRK.YKH_SAAT, 120))	
						    order by (convert(datetime, b.YKH_TARIH + ' ' + b.YKH_SAAT, 120)) desc)WHERE YKH_DEPO_FULLENDI = 0  AND YKH_FARK_KM <> 0 and TB_YAKIT_HRK_ID =@HRK_ID

                            -- Yakıt Tüketim hesaplama
                            UPDATE orjin.TB_YAKIT_HRK SET YKH_TUKETIM  = 0 WHERE YKH_FARK_KM =0 and TB_YAKIT_HRK_ID =@HRK_ID

                            UPDATE orjin.TB_YAKIT_HRK SET YKH_TUKETIM  = YKH_HARCANAN_YAKIT_MIKTAR / YKH_FARK_KM  WHERE YKH_FARK_KM <> 0  AND TB_YAKIT_HRK_ID =@HRK_ID
                            -- Ön Görülen Km ve Sapma hesabı
                            UPDATE orjin.TB_YAKIT_HRK SET YKH_ONGORULEN_KM  = 0 , YKH_SAPMA_KM = 0 WHERE YKH_BEKLENEN_TUKETIM_ORAN =0 and  YKH_FARK_KM <> 0  AND TB_YAKIT_HRK_ID =@HRK_ID

                            UPDATE orjin.TB_YAKIT_HRK SET YKH_ONGORULEN_KM  = YKH_HARCANAN_YAKIT_MIKTAR / YKH_BEKLENEN_TUKETIM_ORAN
                              , YKH_SAPMA_KM = YKH_FARK_KM - YKH_ONGORULEN_KM WHERE YKH_BEKLENEN_TUKETIM_ORAN <> 0   AND TB_YAKIT_HRK_ID =@HRK_ID
                              -- SAPMA LT HESABI
                            UPDATE orjin.TB_YAKIT_HRK SET YKH_SAPMA_LT = YKH_HARCANAN_YAKIT_MIKTAR - YKH_ONGORULEN_LT where   YKH_FARK_KM <> 0 and
                               TB_YAKIT_HRK_ID =@HRK_ID

                            --Öngörülen ve gerçekleşen tutar hesabı
                            UPDATE orjin.TB_YAKIT_HRK SET YKH_ONGORULEN_TUTAR = YKH_FIYAT * YKH_ONGORULEN_LT ,YKH_GERCEKLESEN_TUTAR = YKH_FIYAT * YKH_HARCANAN_YAKIT_MIKTAR,
                            YKH_SAPMA_TUTAR = YKH_FIYAT * YKH_SAPMA_LT where   YKH_FARK_KM <> 0 AND TB_YAKIT_HRK_ID =@HRK_ID";

                Util klas = new Util();
                prms.Clear();
                prms.Add("HRK_ID", hareketID);
                klas.cmd(query, prms.PARAMS);

            }
            catch (Exception ex)
            {
                klas.kapat();
            }
        }

        private void StokHareketIslemi(YakitHareket entity)
        {
            try
            {
                prms.Clear();
                prms.Add("YKH_STOK_ID", entity.YKH_STOK_ID);
                prms.Add("TB_YAKIT_HRK_ID", entity.TB_YAKIT_HRK_ID);
                int deger = Convert.ToInt32(klas.GetDataCell("select count(*) from orjin.TB_STOK_HRK WHERE SHR_REF_GRUP='YAKIT' AND SHR_STOK_ID= @YKH_STOK_ID  and SHR_REF_ID= @TB_YAKIT_HRK_ID", prms.PARAMS));

                if (deger < 1)
                {
                    //hareket kaydı
                    string query = @"INSERT INTO orjin.TB_STOK_HRK
                               (SHR_STOK_FIS_DETAY_ID
                               ,SHR_STOK_ID
                               ,SHR_BIRIM_KOD_ID
                               ,SHR_DEPO_ID
                               ,SHR_TARIH
                               ,SHR_SAAT
                               ,SHR_MIKTAR
                               ,SHR_ANA_BIRIM_MIKTAR
                               ,SHR_BIRIM_FIYAT
                               ,SHR_KDV_ORAN
                               ,SHR_KDV_TUTAR
                               ,SHR_OTV_ORAN
                               ,SHR_OTV_TUTAR
                               ,SHR_INDIRIM_ORAN
                               ,SHR_INDIRIM_TUTAR
                               ,SHR_KDV_DH
                               ,SHR_ARA_TOPLAM
                               ,SHR_TOPLAM
                               ,SHR_HRK_ACIKLAMA
                               ,SHR_ACIKLAMA
                               ,SHR_GC
                               ,SHR_REF_ID
                               ,SHR_REF_GRUP
                               ,SHR_PARABIRIMI_ID
                               ,SHR_OLUSTURAN_ID
                               ,SHR_OLUSTURMA_TARIH)
                               VALUES (-1
                                       ,@SHR_STOK_ID
                                       ,(select STK_BIRIM_KOD_ID  from orjin.TB_STOK where TB_STOK_ID = @SHR_STOK_ID)
                                       ,@SHR_DEPO_ID
                                       ,@SHR_TARIH
                                       ,@SHR_SAAT
                                       ,@SHR_MIKTAR
                                       ,@SHR_ANA_BIRIM_MIKTAR
                                       ,@SHR_BIRIM_FIYAT
                                       ,0
                                       ,0
                                       ,0
                                       ,0
                                       ,0
                                       ,0
                                       ,0
                                       ,@SHR_ARA_TOPLAM
                                       ,@SHR_TOPLAM
                                       ,@SHR_HRK_ACIKLAMA
                                       ,@SHR_ACIKLAMA
                                       ,@SHR_GC
                                       ,@SHR_REF_ID
                                       ,@SHR_REF_GRUP
                                       ,-1
                                       ,@SHR_OLUSTURAN_ID
                                       ,@SHR_OLUSTURMA_TARIH)";

                    prms.Clear();
                    prms.Add("@SHR_STOK_ID", entity.YKH_STOK_ID);
                    prms.Add("@SHR_DEPO_ID", entity.YKH_DEPO_ID);
                    prms.Add("@SHR_TARIH", entity.YKH_TARIH);
                    prms.Add("@SHR_SAAT", entity.YKH_SAAT);
                    prms.Add("@SHR_MIKTAR", entity.YKH_MIKTAR);
                    prms.Add("@SHR_ANA_BIRIM_MIKTAR", entity.YKH_MIKTAR);
                    prms.Add("@SHR_BIRIM_FIYAT", entity.YKH_FIYAT);
                    prms.Add("@SHR_ARA_TOPLAM", entity.YKH_TUTAR);
                    prms.Add("@SHR_TOPLAM", entity.YKH_TUTAR);
                    prms.Add("@SHR_HRK_ACIKLAMA", "YAKIT");
                    prms.Add("@SHR_ACIKLAMA", "YAKIT");
                    prms.Add("@SHR_GC", "C");
                    prms.Add("@SHR_REF_ID", entity.TB_YAKIT_HRK_ID);
                    prms.Add("@SHR_REF_GRUP", "YAKIT");
                    prms.Add("@SHR_OLUSTURAN_ID", entity.YKH_OLUSTURAN_ID);
                    prms.Add("@SHR_OLUSTURMA_TARIH", DateTime.Now);
                    klas.cmd(query, prms.PARAMS);

                    // depo stok güncelleniyor   
                    query = "UPDATE orjin.TB_DEPO_STOK SET DPS_CIKAN_MIKTAR = DPS_CIKAN_MIKTAR + Cast(@Miktar AS FLOAT), DPS_KULLANILABILIR_MIKTAR = DPS_KULLANILABILIR_MIKTAR - Cast(@Miktar AS FLOAT), DPS_MIKTAR = DPS_MIKTAR - Cast(@Miktar AS FLOAT)  WHERE DPS_DEPO_ID = @YKH_DEPO_ID  AND DPS_STOK_ID = @YKH_STOK_ID";
                    prms.Clear();
                    prms.Add("Miktar", entity.YKH_MIKTAR);
                    prms.Add("YKH_DEPO_ID", entity.YKH_DEPO_ID);
                    prms.Add("YKH_STOK_ID", entity.YKH_STOK_ID);
                    klas.cmd(query, prms.PARAMS);


                    // stok güncelleniyor.
                    query = "UPDATE orjin.TB_STOK SET STK_CIKAN_MIKTAR = STK_CIKAN_MIKTAR + Cast(@Miktar AS FLOAT), STK_MIKTAR = STK_MIKTAR - Cast(@Miktar AS FLOAT)  WHERE TB_STOK_ID = @YKH_STOK_ID";
                    prms.Clear();
                    prms.Add("Miktar", entity.YKH_MIKTAR);
                    prms.Add("YKH_STOK_ID", entity.YKH_STOK_ID);
                    klas.cmd(query, prms.PARAMS);

                }
                else
                {
                    prms.Clear();
                    prms.Add("YKH_STOK_ID", entity.YKH_STOK_ID);
                    prms.Add("TB_YAKIT_HRK_ID", entity.TB_YAKIT_HRK_ID);
                    double eskiMiktar = Convert.ToDouble(klas.GetDataCell("select coalesce(SHR_MIKTAR,0) from orjin.TB_STOK_HRK WHERE SHR_REF_GRUP='YAKIT' AND SHR_STOK_ID= @YKH_STOK_ID and SHR_REF_ID= @TB_YAKIT_HRK_ID", prms.PARAMS));
                    double yeniMiktar = entity.YKH_MIKTAR - eskiMiktar;

                    //hareket kaydı
                    string query = @"UPDATE orjin.TB_STOK_HRK SET
                                SHR_STOK_ID                            = @SHR_STOK_ID
                               ,SHR_BIRIM_KOD_ID                       = (select STK_BIRIM_KOD_ID  from orjin.TB_STOK where TB_STOK_ID =@SHR_STOK_ID)
                               ,SHR_DEPO_ID                            = @SHR_DEPO_ID
                               ,SHR_TARIH                              = @SHR_TARIH
                               ,SHR_SAAT                               = @SHR_SAAT
                               ,SHR_MIKTAR                             = @SHR_MIKTAR
                               ,SHR_ANA_BIRIM_MIKTAR                   = @SHR_ANA_BIRIM_MIKTAR
                               ,SHR_BIRIM_FIYAT                        = @SHR_BIRIM_FIYAT
                               ,SHR_ARA_TOPLAM                         = @SHR_ARA_TOPLAM
                               ,SHR_TOPLAM                             = @SHR_TOPLAM
                               ,SHR_HRK_ACIKLAMA                       = @SHR_HRK_ACIKLAMA
                               ,SHR_ACIKLAMA                           = @SHR_ACIKLAMA
                               ,SHR_GC                                 = @SHR_GC
                               ,SHR_REF_ID                             = @SHR_REF_ID
                               ,SHR_REF_GRUP                           = @SHR_REF_GRUP
                               ,SHR_DEGISTIREN_ID                      = @SHR_DEGISTIREN_ID
                               ,SHR_DEGISTIRME_TARIH                   = @SHR_DEGISTIRME_TARIH WHERE SHR_REF_GRUP='YAKIT' AND SHR_STOK_ID = @SHR_STOK_ID and SHR_REF_ID=@SHR_REF_ID";
                    prms.Clear();
                    prms.Add("@SHR_STOK_ID", entity.YKH_STOK_ID);
                    prms.Add("@SHR_DEPO_ID", entity.YKH_DEPO_ID);
                    prms.Add("@SHR_TARIH", entity.YKH_TARIH);
                    prms.Add("@SHR_SAAT", entity.YKH_SAAT);
                    prms.Add("@SHR_MIKTAR", entity.YKH_MIKTAR);
                    prms.Add("@SHR_ANA_BIRIM_MIKTAR", entity.YKH_MIKTAR);
                    prms.Add("@SHR_BIRIM_FIYAT", entity.YKH_FIYAT);
                    prms.Add("@SHR_ARA_TOPLAM", entity.YKH_TUTAR);
                    prms.Add("@SHR_TOPLAM", entity.YKH_TUTAR);
                    prms.Add("@SHR_HRK_ACIKLAMA", "YAKIT");
                    prms.Add("@SHR_ACIKLAMA", "YAKIT");
                    prms.Add("@SHR_GC", "C");
                    prms.Add("@SHR_REF_ID", entity.TB_YAKIT_HRK_ID);
                    prms.Add("@SHR_REF_GRUP", "YAKIT");
                    prms.Add("@SHR_DEGISTIREN_ID", entity.YKH_DEGISTIREN_ID);
                    prms.Add("@SHR_DEGISTIRME_TARIH", DateTime.Now);
                    klas.cmd(query, prms.PARAMS);
                    // depo stok güncelleniyor     
                    query = "UPDATE orjin.TB_DEPO_STOK SET DPS_CIKAN_MIKTAR = DPS_CIKAN_MIKTAR + Cast(@Miktar AS FLOAT), DPS_KULLANILABILIR_MIKTAR = DPS_KULLANILABILIR_MIKTAR - Cast(@Miktar AS FLOAT), DPS_MIKTAR = DPS_MIKTAR - Cast(@Miktar AS FLOAT)  WHERE DPS_DEPO_ID = @YKH_DEPO_ID  AND DPS_STOK_ID = @YKH_STOK_ID";
                    prms.Clear();
                    prms.Add("Miktar", yeniMiktar);
                    prms.Add("YKH_DEPO_ID", entity.YKH_DEPO_ID);
                    prms.Add("YKH_STOK_ID", entity.YKH_STOK_ID);
                    klas.cmd(query, prms.PARAMS);
                    // stok güncelleniyor.
                    query = "UPDATE orjin.TB_STOK SET STK_CIKAN_MIKTAR = STK_CIKAN_MIKTAR + Cast(@Miktar AS FLOAT), STK_MIKTAR = STK_MIKTAR - Cast(@Miktar AS FLOAT)  WHERE TB_STOK_ID =  @YKH_STOK_ID";
                    prms.Clear();
                    prms.Add("Miktar", yeniMiktar);
                    prms.Add("YKH_STOK_ID", entity.YKH_STOK_ID);
                    klas.cmd(query, prms.PARAMS);
                }
            }
            catch (Exception)
            {
                klas.kapat();
                throw;
            }
        }

        [Route("api/MakineYakitBilgiGetir")]
        [HttpGet]
        public List<TanimDeger> MakineYakitBilgiGetir([FromUri] int yakithrkID, [FromUri] int makineID, [FromUri] string tarih, [FromUri] string saat)
        {
            List<TanimDeger> listem = new List<TanimDeger>();
            TanimDeger entity = new TanimDeger();
            prms.Clear();
            prms.Add("YKT_HAREKET_ID", yakithrkID);
            prms.Add("MAKINE_ID", makineID);
            prms.Add("TARIH", Convert.ToDateTime(tarih).ToString("yyyy-MM-dd"));
            prms.Add("SAAT", saat);
            double sonSayacDeger = Convert.ToDouble(klas.GetDataCell("select orjin.UDF_MAKINE_SON_YAKIT_GETIR (@YKT_HAREKET_ID,@MAKINE_ID,@TARIH,@SAAT)", prms.PARAMS));
            entity.Deger = sonSayacDeger;
            entity.Tanim = "Sayac";
            entity.TanimDegerID = 1;

            listem.Add(entity);

            TanimDeger entity1 = new TanimDeger();
            prms.Clear();
            prms.Add("MKN_ID", makineID);
            entity1.Deger = Convert.ToDouble(klas.GetDataCell("select top 1  coalesce(MKN_YAKIT_DEPO_HACMI,0) from orjin.TB_MAKINE where TB_MAKINE_ID = @MKN_ID", prms.PARAMS));
            entity1.Tanim = "DepoHacim";
            entity1.TanimDegerID = 2;

            listem.Add(entity1);

            prms.Clear();
            prms.Add("MKN_ID", makineID);
            TanimDeger entity2 = new TanimDeger();
            entity2.Deger = 0;
            entity2.Tanim = Convert.ToString(klas.GetDataCell("select top 1  coalesce(YKH_SAYAC_BIRIM,'') from orjin.VW_YAKIT_HRK where YKH_MAKINE_ID = @MKN_ID order by YKH_TARIH + YKH_SAAT  desc", prms.PARAMS));
            entity2.TanimDegerID = 3;
            listem.Add(entity2);
            return listem;
        }


        [Route("api/YakitHareketSil")]
        [HttpPost]
        public Bildirim YakitHareketSil([FromBody] YakitHareket entity)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                prms.Clear();
                prms.Add("TB_YAKIT_HRK_ID", entity.TB_YAKIT_HRK_ID);
                klas.cmd("DELETE FROM orjin.TB_SAYAC_OKUMA WHERE SYO_REF_GRUP='YAKIT' and SYO_REF_ID = @TB_YAKIT_HRK_ID", prms.PARAMS);

                if (entity.YKH_STOK_KULLANIM && entity.YKH_STOK_ID > 0 && entity.YKH_DEPO_ID > 0)
                {
                    prms.Clear();
                    prms.Add("TB_YAKIT_HRK_ID", entity.TB_YAKIT_HRK_ID);
                    klas.cmd("DELETE FROM orjin.TB_STOK_HRK WHERE SHR_REF_GRUP='YAKIT' and SHR_REF_ID = @TB_YAKIT_HRK_ID", prms.PARAMS);
                    // depo stok güncelleniyor              

                    string query = "UPDATE orjin.TB_DEPO_STOK SET DPS_CIKAN_MIKTAR = DPS_CIKAN_MIKTAR - Cast(@Miktar AS FLOAT), DPS_KULLANILABILIR_MIKTAR = DPS_KULLANILABILIR_MIKTAR + Cast(@Miktar AS FLOAT), DPS_MIKTAR = DPS_MIKTAR + Cast(@Miktar AS FLOAT)  WHERE DPS_DEPO_ID = @YKH_DEPO_ID  AND DPS_STOK_ID = @YKH_STOK_ID";
                    prms.Clear();
                    prms.Add("Miktar", entity.YKH_MIKTAR);
                    prms.Add("YKH_DEPO_ID", entity.YKH_DEPO_ID);
                    prms.Add("YKH_STOK_ID", entity.YKH_STOK_ID);
                    klas.cmd(query, prms.PARAMS);

                    // stok güncelleniyor.
                    query = "UPDATE orjin.TB_STOK SET STK_CIKAN_MIKTAR = STK_CIKAN_MIKTAR - Cast(@Miktar AS FLOAT), STK_MIKTAR = STK_MIKTAR + Cast(@Miktar AS FLOAT)  WHERE TB_STOK_ID = @YKH_STOK_ID";
                    prms.Clear();
                    prms.Add("Miktar", entity.YKH_MIKTAR);
                    prms.Add("YKH_STOK_ID", entity.YKH_STOK_ID);
                    klas.cmd(query, prms.PARAMS);

                }
                prms.Clear();
                prms.Add("TB_YAKIT_HRK_ID", entity.TB_YAKIT_HRK_ID);
                klas.cmd("DELETE FROM orjin.TB_YAKIT_HRK WHERE TB_YAKIT_HRK_ID = @TB_YAKIT_HRK_ID", prms.PARAMS);
                bildirimEntity.Aciklama = "Yakıt silme başarılı bir şekilde gerçekleştirildi.";
                bildirimEntity.MsgId = Bildirim.MSG_YKT_SIL_OK;
                bildirimEntity.Durum = true;
            }
            catch (Exception e)
            {
                klas.kapat();
                bildirimEntity.Aciklama = String.Format(Localization.errorFormatted, e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }
            return bildirimEntity;
        }

        [Route("api/YakitKartAcilis/{uid}")]
        public ResponseModel getYakitKartAcilis([FromUri] int uid)
        {
            var response=new ResponseModel();
            var yakitKartAcilis=new YakitHareketKartAcilis();
            var prms = new {@ID=uid};
            var sql = @"SELECT * FROM orjin.TB_MASRAF_MERKEZ WHERE MAM_AKTIF = 1;
                        SELECT * FROM orjin.TB_PROJE;
                        SELECT * FROM orjin.TB_LOKASYON WHERE orjin.UDF_LOKASYON_YETKI_KONTROL(TB_LOKASYON_ID,@ID) = 1;
                        SELECT * FROM orjin.TB_PARAMETRE WHERE PRM_KOD IN('330000','330002');";
            try
            {
                var util = new Util();
                using (var conn = util.baglan())
                {
                    var result=conn.QueryMultiple(sql, prms);
                    yakitKartAcilis.MasrafMerkeziList = result.Read<MasrafMerkezi>().ToList();
                    yakitKartAcilis.ProjeList      =result.Read<Proje>().ToList();
                    yakitKartAcilis.LokasyonList   =result.Read<Lokasyon>().ToList();
                    yakitKartAcilis.ParametreList = result.Read<Parametre>().ToList();
                    response.Data = yakitKartAcilis;
                    response.Status = true;
                    response.Error = false;
                    return response;
                }
            }
            catch (Exception e)
            {
                response.Status = false;
                response.Error = true;
                response.Message = string.Format( Localization.BilgilerAlinirkenSunucuHata,e.Message);
            }
            return response;
        }

    }
}
