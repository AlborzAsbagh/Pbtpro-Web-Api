using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiNew.App_GlobalResources;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    
    [MyBasicAuthenticationFilter]
    public class SayimController : ApiController
    {
        Util _util = new Util();
        Parametreler prms = new Parametreler();
        //[Route("api/getSayimList")]
        [HttpPost]
        public List<Sayim> getSayimList([FromBody]Filtre filtre, [FromUri] int page, [FromUri] int pageSize, [FromUri] int ID, [FromUri] int durumID,[FromUri] bool sortAsc)
        {
            int ilkdeger = page * pageSize;
            int sondeger = ilkdeger + pageSize;
            string sort = sortAsc ? "ASC" : "DESC";
            try
            {
                prms.Clear();
                prms.Add("KUL_ID", ID);
                string query = @";WITH mTable AS
                                (SELECT *,ROW_NUMBER() OVER (ORDER BY SYM_TARIH "+sort+", SYM_SAAT "+sort+") AS RowNum FROM orjin.VW_STOK_SAYIM WHERE orjin.UDF_DEPO_YETKI_KONTROL(SYM_DEPO_ID, @KUL_ID) = 1";
                if (durumID > -1)
                {
                    prms.Add("SYM_KAPALI", durumID);
                    query += " AND SYM_KAPALI = @SYM_KAPALI";
                }
                if (filtre != null)
                {
                    if (filtre.PersonelID > 0)
                    {
                        prms.Add("SYM_PERSONEL_ID", filtre.PersonelID);
                        query += " AND SYM_PERSONEL_ID = @SYM_PERSONEL_ID";
                    }
                    if (filtre.DepoID > 0)
                    {
                        prms.Add("SYM_DEPO_ID", filtre.DepoID);
                        query += " AND SYM_DEPO_ID = @SYM_DEPO_ID";

                    }
                    if (filtre.Kelime != "")
                    {
                        prms.Add("KELIME", filtre.Kelime);
                        query += " AND (SYM_FIS_NO like '%'+@KELIME+'%' OR SYM_PERSONEL like '%'+@KELIME+'%' OR SYM_ACIKLAMA like '%'+@KELIME+'%' OR SYM_DEPO like '%'+@KELIME+'%')";
                    }
                    if (filtre.BasTarih != "" && filtre.BitTarih != "")
                    {
                        DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                        DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                        prms.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
                        prms.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
                        query = query + " AND SYM_TARIH BETWEEN  @BAS_TARIH and @BIT_TARIH";
                    }
                    else
                    if (filtre.BasTarih != "" )
                    {
                        DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                        prms.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
                        query = query + " AND SYM_TARIH >=  @BAS_TARIH ";
                    }
                    else
                    if (filtre.BitTarih != "")
                    {
                        DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                        prms.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
                        query = query + " AND SYM_TARIH <= @BIT_TARIH";
                    }
                }
                prms.Add("ILK_DEGER", ilkdeger);
                prms.Add("SON_DEGER", sondeger);
                query += " )SELECT * FROM mTable WHERE RowNum > @ILK_DEGER AND RowNum <= @SON_DEGER";
                List<Sayim> sayimList = new List<Sayim>();
                DataTable dt = _util.GetDataTable(query, prms.PARAMS);
                foreach (DataRow row in dt.Rows)
                {
                    Sayim sayim = new Sayim();
                    sayim.TB_STOK_SAYIM_ID = Util.getFieldInt(row, "TB_STOK_SAYIM_ID");
                    sayim.SYM_OLUSTURAN_ID = Util.getFieldInt(row, "SYM_OLUSTURAN_ID");
                    sayim.SYM_PERSONEL_ID = Util.getFieldInt(row, "SYM_PERSONEL_ID");
                    sayim.SYM_DEPO_ID = Util.getFieldInt(row, "SYM_DEPO_ID");
                    sayim.SYM_DEGISTIREN_ID = Util.getFieldInt(row, "SYM_DEGISTIREN_ID");
                    sayim.SYM_PERSONEL = Util.getFieldString(row, "SYM_PERSONEL");
                    sayim.SYM_ACIKLAMA = Util.getFieldString(row, "SYM_ACIKLAMA");
                    sayim.SYM_FIS_NO = Util.getFieldString(row, "SYM_FIS_NO");
                    sayim.SYM_DEPO = Util.getFieldString(row, "SYM_DEPO");
                    sayim.SYM_SAAT = Util.getFieldString(row, "SYM_SAAT");
                    sayim.SYM_TARIH = Util.getFieldDateTime(row, "SYM_TARIH");
                    sayim.SYM_OLUSTURMA_TARIH = Util.getFieldDateTime(row, "SYM_OLUSTURMA_TARIH");
                    sayim.SYM_DEGISTIRME_TARIH = Util.getFieldDateTime(row, "SYM_DEGISTIRME_TARIH");
                    sayim.SYM_KAPALI = Util.getFieldBool(row, "SYM_KAPALI");
                    sayimList.Add(sayim);
                }
                return sayimList;
            }
            catch (Exception)
            {
                _util.kapat();
            }
            return null;
        }

        [Route("api/getSayimDetay/{sayimID}")]
        [HttpPost]
        public List<SayimStok> getSayimDetay([FromUri] int sayimID, [FromUri] string stokKod)
        {
            try
            {
                prms.Clear();
                prms.Add("STK_KOD", stokKod);
                prms.Add("SSD_STOK_SAYIM_ID", sayimID);
                string query = @"SELECT		TOP (1)
                D.TB_STOK_SAYIM_DETAY_ID, 
				D.SSD_STOK_SAYIM_ID, 
				D.SSD_STOK_ID, 
				D.SSD_STOK_MIKTAR, 
                D.SSD_SAYIM_MIKTAR, 
				D.SSD_FARK_MIKTAR, 
				D.SSD_OLUSTURMA_TARIH, 
                D.SSD_DEGISTIREN_ID, 
				D.SSD_DEGISTIRME_TARIH, 
				S.STK_TANIM, 
				S.STK_KOD, 
				S.STK_URETICI_KOD,
				S.STK_GRUP_KOD_ID,
				S.STK_TIP_KOD_ID,
                             orjin.UDF_KOD_TANIM(S.STK_TIP_KOD_ID) AS STK_TIP,
                             orjin.UDF_KOD_TANIM(S.STK_GRUP_KOD_ID) AS STK_GRUP,
                             orjin.UDF_KOD_TANIM(S.STK_MARKA_KOD_ID) AS STK_MARKA,
                             orjin.UDF_KOD_TANIM(S.STK_MODEL_KOD_ID) AS STK_MODEL,
                             orjin.UDF_KOD_TANIM((SELECT STK_BIRIM_KOD_ID FROM orjin.TB_STOK WHERE TB_STOK_ID=SSD_STOK_ID)) AS STK_BIRIM
                    FROM orjin.TB_STOK_SAYIM_DETAY AS D LEFT OUTER JOIN orjin.TB_STOK AS S ON S.TB_STOK_ID = D.SSD_STOK_ID   WHERE D.SSD_STOK_SAYIM_ID = @SSD_STOK_SAYIM_ID AND  STK_KOD =@STK_KOD order by TB_STOK_SAYIM_DETAY_ID, SSD_OLUSTURMA_TARIH DESC";
                List<SayimStok> sayimList = new List<SayimStok>();
                DataTable dt = _util.GetDataTable(query, prms.PARAMS);
                foreach (DataRow row in dt.Rows)
                {
                    SayimStok stok = new SayimStok();
                    stok.TB_STOK_SAYIM_DETAY_ID = Util.getFieldInt(row, "TB_STOK_SAYIM_DETAY_ID");
                    stok.SSD_STOK_SAYIM_ID = Util.getFieldInt(row, "SSD_STOK_SAYIM_ID");
                    stok.SSD_STOK_ID = Util.getFieldInt(row, "SSD_STOK_ID");
                    stok.SSD_DEGISTIREN_ID = Util.getFieldInt(row, "SSD_DEGISTIREN_ID");
                    stok.SSD_SAYIM_MIKTAR = Util.getFieldDouble(row, "SSD_SAYIM_MIKTAR");
                    stok.SSD_DEGISTIRME_TARIH = Util.getFieldDateTime(row, "SSD_DEGISTIRME_TARIH");
                    stok.SSD_OLUSTURMA_TARIH = Util.getFieldDateTime(row, "SSD_OLUSTURMA_TARIH");
                    stok.STK_TANIM = Util.getFieldString(row, "STK_TANIM");
                    stok.STK_KOD = Util.getFieldString(row, "STK_KOD");
                    stok.STK_URETICI_KOD = Util.getFieldString(row, "STK_URETICI_KOD");
                    stok.STK_TIP = Util.getFieldString(row, "STK_TIP");
                    stok.STK_GRUP = Util.getFieldString(row, "STK_GRUP");
                    stok.STK_MARKA = Util.getFieldString(row, "STK_MARKA");
                    stok.STK_MODEL = Util.getFieldString(row, "STK_MODEL");
                    stok.STK_BIRIM = Util.getFieldString(row, "STK_BIRIM");
                    sayimList.Add(stok);
                }
                return sayimList;
            }
            catch (Exception)
            {
                _util.kapat();
            }
            return null;
        }

        [Route("api/getSayimDetay/{sayimID}")]
        [HttpPost]
        public List<SayimStok> getSayimDetay([FromBody]Filtre filtre, [FromUri] int sayimID, [FromUri] int page, [FromUri] int pageSize)
        {

            int ilkdeger = page * pageSize;
            int sondeger = ilkdeger + pageSize;
            try
            {
                prms.Clear();
                prms.Add("SSD_STOK_SAYIM_ID", sayimID);
                string query = @";WITH mTable AS
                (SELECT			D.TB_STOK_SAYIM_DETAY_ID, 
				D.SSD_STOK_SAYIM_ID, 
				D.SSD_STOK_ID, 
				D.SSD_STOK_MIKTAR, 
                D.SSD_SAYIM_MIKTAR, 
				D.SSD_FARK_MIKTAR, 
				D.SSD_OLUSTURMA_TARIH, 
                D.SSD_DEGISTIREN_ID, 
				D.SSD_DEGISTIRME_TARIH, 
				S.STK_TANIM, 
				S.STK_KOD, 
				S.STK_URETICI_KOD,
				S.STK_GRUP_KOD_ID,
				S.STK_TIP_KOD_ID,
                             orjin.UDF_KOD_TANIM(S.STK_TIP_KOD_ID) AS STK_TIP,
                             orjin.UDF_KOD_TANIM(S.STK_GRUP_KOD_ID) AS STK_GRUP,
                             orjin.UDF_KOD_TANIM(S.STK_MARKA_KOD_ID) AS STK_MARKA,
                             orjin.UDF_KOD_TANIM(S.STK_MODEL_KOD_ID) AS STK_MODEL,
                             orjin.UDF_KOD_TANIM((SELECT STK_BIRIM_KOD_ID FROM orjin.TB_STOK WHERE TB_STOK_ID=SSD_STOK_ID)) AS STK_BIRIM,
                             ROW_NUMBER() OVER (ORDER BY TB_STOK_SAYIM_DETAY_ID, SSD_OLUSTURMA_TARIH DESC) AS RowNum
                    FROM orjin.TB_STOK_SAYIM_DETAY AS D LEFT OUTER JOIN orjin.TB_STOK AS S ON S.TB_STOK_ID = D.SSD_STOK_ID   WHERE D.SSD_STOK_SAYIM_ID = @SSD_STOK_SAYIM_ID";
                if (filtre != null)
                {
                    if (filtre.Kelime != "")
                    {
                        prms.Add("KELIME", filtre.Kelime);
                        query += " AND (STK_KOD like '%'+@KELIME+'%' OR STK_TANIM like '%'+@KELIME+'%' OR STK_URETICI_KOD like '%'+@KELIME+'%')";
                    }
                    if (filtre.PersonelID > -1) //PersonelID  grup
                    {
                        prms.Add("STK_GRUP_KOD_ID", filtre.PersonelID);
                        query += " AND S.STK_GRUP_KOD_ID = @STK_GRUP_KOD_ID";
                    }
                    if (filtre.LokasyonID > -1)//LokasyonID tip
                    {
                        prms.Add("STK_TIP_KOD_ID", filtre.LokasyonID);
                        query += " AND S.STK_TIP_KOD_ID = @STK_TIP_KOD_ID";
                    }
                    if (filtre.durumID > 0)
                    {
                        switch (filtre.durumID)
                        {
                            case 2:
                                query += " AND (D.SSD_SAYIM_MIKTAR = 0 OR  D.SSD_SAYIM_MIKTAR IS NULL)";
                                break;
                            case 1:
                                query += " AND D.SSD_SAYIM_MIKTAR > 0";
                                break;
                        }
                    }
                }
                prms.Add("ILK_DEGER", ilkdeger);
                prms.Add("SON_DEGER", sondeger);
                query += " )SELECT * FROM mTable WHERE RowNum > @ILK_DEGER AND RowNum <= @SON_DEGER";
                List<SayimStok> sayimList = new List<SayimStok>();
                DataTable dt = _util.GetDataTable(query, prms.PARAMS);
                foreach (DataRow row in dt.Rows)
                {
                    SayimStok stok = new SayimStok();
                    stok.TB_STOK_SAYIM_DETAY_ID = Util.getFieldInt(row, "TB_STOK_SAYIM_DETAY_ID");
                    stok.SSD_STOK_SAYIM_ID = Util.getFieldInt(row, "SSD_STOK_SAYIM_ID");
                    stok.SSD_STOK_ID = Util.getFieldInt(row, "SSD_STOK_ID");
                    stok.SSD_DEGISTIREN_ID = Util.getFieldInt(row, "SSD_DEGISTIREN_ID");
                    stok.SSD_SAYIM_MIKTAR = Util.getFieldDouble(row, "SSD_SAYIM_MIKTAR");
                    stok.SSD_DEGISTIRME_TARIH = Util.getFieldDateTime(row, "SSD_DEGISTIRME_TARIH");
                    stok.SSD_OLUSTURMA_TARIH = Util.getFieldDateTime(row, "SSD_OLUSTURMA_TARIH");
                    stok.STK_TANIM = Util.getFieldString(row, "STK_TANIM");
                    stok.STK_KOD = Util.getFieldString(row, "STK_KOD");
                    stok.STK_URETICI_KOD = Util.getFieldString(row, "STK_URETICI_KOD");
                    stok.STK_TIP = Util.getFieldString(row, "STK_TIP");
                    stok.STK_GRUP = Util.getFieldString(row, "STK_GRUP");
                    stok.STK_MARKA = Util.getFieldString(row, "STK_MARKA");
                    stok.STK_MODEL = Util.getFieldString(row, "STK_MODEL");
                    stok.STK_BIRIM = Util.getFieldString(row, "STK_BIRIM");
                    sayimList.Add(stok);
                }
                return sayimList;
            }
            catch (Exception e)
            {
                _util.kapat();
            }
            return null;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        [Route("api/updateSayimDetay")]
        [HttpPost]
        public Bildirim update([FromBody]SayimStok entity)
        {
            Util klas = new Util();
            Bildirim bil = new Bildirim();
            bil.Aciklama = "Sayım miktarı bilgisi başarılı bir şekilde güncellendi!";
            bil.MsgId = Bildirim.MSG_SSD_GUNCELLE_OK;
            bil.Durum = true;
            try
            {

                string query = @"UPDATE orjin.TB_STOK_SAYIM_DETAY SET
                                     SSD_SAYIM_MIKTAR =@SSD_SAYIM_MIKTAR 
                                      ,SSD_FARK_MIKTAR = @SSD_SAYIM_MIKTAR - (SELECT SSD_STOK_MIKTAR FROM orjin.TB_STOK_SAYIM_DETAY WHERE TB_STOK_SAYIM_DETAY_ID = @TB_STOK_SAYIM_DETAY_ID)                               
                                      ,SSD_DEGISTIREN_ID = @SSD_DEGISTIREN_ID                                   
                                      ,SSD_DEGISTIRME_TARIH = @SSD_DEGISTIRME_TARIH
                                       WHERE TB_STOK_SAYIM_DETAY_ID = @TB_STOK_SAYIM_DETAY_ID";

                prms.Clear();
                prms.Add("@TB_STOK_SAYIM_DETAY_ID", entity.TB_STOK_SAYIM_DETAY_ID);
                prms.Add("@SSD_SAYIM_MIKTAR", entity.SSD_SAYIM_MIKTAR);
                prms.Add("@SSD_DEGISTIREN_ID", entity.SSD_DEGISTIREN_ID);
                prms.Add("@SSD_DEGISTIRME_TARIH", DateTime.Now);
                klas.cmd(query, prms.PARAMS);
            }
            catch (Exception e)
            {
                bil.Aciklama = String.Format(Localization.errorFormatted,e.Message);
                bil.MsgId = Bildirim.MSG_SSD_GUNCELLE_HATA;
                bil.HasExtra = true;
                bil.Durum = false;
            }
            return bil;
        }
    }
}