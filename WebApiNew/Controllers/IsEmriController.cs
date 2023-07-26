using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Dapper;
using WebApiNew.App_GlobalResources;
using WebApiNew.Filters;
using WebApiNew.Models;
using WebApiNew.Utility;
using WebApiNew.Utility.Abstract;

namespace WebApiNew.Controllers
{
    [MyBasicAuthenticationFilter]
    public class IsEmriController : ApiController
    {
        Util klas = new Util();
        List<Prm> parametreler = new List<Prm>();
        Parametreler prms = new Parametreler();
        private readonly System.Windows.Forms.RichTextBox RTB = new System.Windows.Forms.RichTextBox();
        private readonly ILogger _logger;

        public IsEmriController(ILogger logger)
        {
            _logger = logger;
        }

        private Dictionary<int, object> GetFilteredIsemriListWhere(Filtre filtre, int Id)
        {
            var prms = new DynamicParameters();

            prms.Add("KUL_ID", Id);
            string where =
                "where orjin.UDF_LOKASYON_YETKI_KONTROL(ISM_LOKASYON_ID, @KUL_ID ) = 1 AND orjin.UDF_ATOLYE_YETKI_KONTROL(ISM_ATOLYE_ID, @KUL_ID ) = 1";
            if (filtre != null)
            {
                if (filtre.PersonelID > 0)
                {
                    prms.Add("PRS_ID", filtre.PersonelID);
                    where += " AND (SELECT COUNT(*) FROM orjin.TB_ISEMRI_KAYNAK WHERE IDK_REF_ID = @PRS_ID AND IDK_ISEMRI_ID = TB_ISEMRI_ID)>0";
                }
                if (filtre.MakineID > 0)
                {
                    if (filtre.MasterMakineID > 0)
                    {
                        prms.Add("MAK_ID", filtre.MakineID);
                        where += " AND ( ISM_MAKINE_ID = @MAK_ID";
                    }
                    else
                    {
                        prms.Add("MAK_ID", filtre.MakineID);
                        where += " AND ISM_MAKINE_ID = @MAK_ID";
                    }
                }
                if (filtre.MasterMakineID > 0)
                {
                    if(filtre.MakineID > 0)
                    {
                        prms.Add("MAK_ID", filtre.MasterMakineID);
                        where += " OR ISM_MAKINE_ID = @MAK_ID ) ";
                    }
                    else
                    {
                        prms.Add("MAK_ID", filtre.MasterMakineID);
                        where += " AND ISM_MAKINE_ID = @MAK_ID";
                    }
                }

                if (filtre.LokasyonID > 0)
                {
                    prms.Add("LOK_ID", filtre.LokasyonID);
                    where += " AND ISM_LOKASYON_ID = @LOK_ID";
                }

                if (filtre.isEmriTipId > -1)
                {
                    prms.Add("IS_EMRI_TIP_ID", filtre.isEmriTipId);
                    where += " AND ISM_TIP_ID = @IS_EMRI_TIP_ID";
                }

                if (filtre.ProjeID > 0)
                {
                    prms.Add("ISM_PROJE_KOD_ID", filtre.ProjeID);
                    where += " AND ISM_PROJE_KOD_ID = @ISM_PROJE_KOD_ID";
                }

                if (filtre.DepoID > 0) //ATOLYE
                {
                    prms.Add("ISM_ATOLYE_ID", filtre.DepoID);
                    where += " AND ISM_ATOLYE_ID = @ISM_ATOLYE_ID";
                }

                if (!string.IsNullOrWhiteSpace(filtre.Kelime))
                {
                    prms.Add("KELIME", filtre.Kelime);
                    where += @" AND  (ISM_ISEMRI_NO like     '%'+@KELIME+'%' 
                                        OR ISM_KONU like            '%'+@KELIME+'%' 
                                        OR ISM_MAKINE_KOD like      '%'+@KELIME+'%' 
                                        OR ISM_MAKINE_PLAKA like    '%'+@KELIME+'%' 
                                        OR ISM_MAKINE_TANIMI like   '%'+@KELIME+'%'  
                                        OR ISM_ATOLYE like          '%'+@KELIME+'%'
                                        OR ISM_TIP like             '%'+@KELIME+'%' 
                                        OR ISM_LOKASYON like        '%'+@KELIME+'%' ) ";
                }

                //if (filtre.PersonelID > 0)
                //    query += " AND IST_TALEP_EDEN_ID = "  filtre.PersonelID;
                if (!string.IsNullOrWhiteSpace(filtre.BasTarih) && !string.IsNullOrWhiteSpace(filtre.BitTarih))
                {
                    DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                    DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                    prms.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
                    prms.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
                    where += " AND ISM_DUZENLEME_TARIH BETWEEN  @BAS_TARIH and @BIT_TARIH";
                }
                else if (!string.IsNullOrWhiteSpace(filtre.BasTarih))
                {
                    DateTime bas = Convert.ToDateTime(filtre.BasTarih);
                    prms.Add("BAS_TARIH", bas.ToString("yyyy-MM-dd"));
                    where += " AND ISM_DUZENLEME_TARIH >=  @BAS_TARIH ";
                }
                else if (!string.IsNullOrWhiteSpace(filtre.BitTarih))
                {
                    DateTime bit = Convert.ToDateTime(filtre.BitTarih);
                    prms.Add("BIT_TARIH", bit.ToString("yyyy-MM-dd"));
                    where += " AND ISM_DUZENLEME_TARIH <= @BIT_TARIH ";
                }
                //plan tarihler
                if (filtre.PlanBasTarih != null)
                {
                    prms.Add("PLAN_BAS_TARIH", filtre.PlanBasTarih);
                    where += " AND ISM_PLAN_BASLAMA_TARIH >=  @PLAN_BAS_TARIH ";
                }
                if (filtre.PlanBitTarih != null)
                {
                    prms.Add("PLAN_BIT_TARIH", filtre.PlanBitTarih);
                    where += " AND ISM_PLAN_BITIS_TARIH <= @PLAN_BIT_TARIH ";
                }
            }

            var mresult = new Dictionary<int, object>();
            mresult.Add(0, where);
            mresult.Add(1, prms);
            return mresult;
        }

        [Route("api/isemriListe/{ID}")]
        [HttpPost]
        public object IsEmriListe([FromBody] Filtre filtre, [FromUri] int ilkDeger, [FromUri] int sonDeger,
            [FromUri] int ID, [FromUri] int durumID, [FromUri] bool sortAsc)
        {
            try
            {
                List<IsEmri> listem;
                string sortstr = sortAsc ? "ASC" : "DESC";
                string query = $@";WITH mTable AS
                    (select 	   TB_ISEMRI_ID
                                      ,coalesce(ISM_ISEMRI_NO,'') ISM_ISEMRI_NO
                                      ,ISM_MAKINE_ID
                                      ,ISM_REF_ID
                                      ,coalesce(ISM_REF_GRUP,'') ISM_REF_GRUP
                                      ,ISM_ONCELIK_ID
                                      ,ISM_BILDIRIM_TARIH
                                      ,ISM_BILDIRIM_SAAT
                                      ,ISM_DUZENLEME_TARIH
                                      ,ISM_DUZENLEME_SAAT
                                      ,ISM_BASLAMA_TARIH
                                      ,ISM_BASLAMA_SAAT
                                      ,ISM_BITIS_TARIH
                                      ,ISM_BITIS_SAAT
                                      ,ISM_KAPATILDI
                                      ,ISM_PLAN_BASLAMA_TARIH
                                      ,ISM_PLAN_BASLAMA_SAAT
                                      ,ISM_PLAN_BITIS_TARIH
                                      ,ISM_PLAN_BITIS_SAAT
                                      ,coalesce(ISM_ACIKLAMA,'') ISM_ACIKLAMA
                                      ,coalesce(ISM_BILDIREN,'') ISM_BILDIREN
                                      ,coalesce(ISM_PUAN,'') ISM_PUAN
                                      ,COALESCE(ISM_SONUC,'') ISM_SONUC
                                      ,ISM_LOKASYON_ID
                                      ,ISM_DURUM_KOD_ID
                                      ,COALESCE(ISM_KONU ,'')  ISM_KONU 
                                      ,COALESCE(ISM_NOT	 ,'')  ISM_NOT	 
                                      ,ISM_TIP_KOD_ID
                                      ,ISM_ATOLYE_ID
                                      ,ISM_EKIPMAN_ID
                                      ,ISM_OLUSTURAN_ID
                                      ,ISM_OLUSTURMA_TARIH
                                      ,ISM_DEGISTIREN_ID
                                      ,ISM_DEGISTIRME_TARIH
                                      ,ISM_MAKINE_DURUM_KOD_ID
                                      ,ISM_TIP_ID
                                      ,ISM_IS_TARIH
                                      ,ISM_IS_SAAT
                                      ,ISM_MASRAF_MERKEZ_ID
                                      ,COALESCE(ISM_IS_SONUC,'') ISM_IS_SONUC
                                      ,ISM_SURE_CALISMA
                                      ,ISM_SONUC_KOD_ID
                                      ,ISM_KAPAT_MAKINE_DURUM_KOD_ID
                                      ,ISM_SAYAC_DEGER
                                      ,COALESCE(ISM_MAKINE_GUVENLIK_NOTU,'') ISM_MAKINE_GUVENLIK_NOTU
                                      ,ISM_PROJE_KOD_ID AS ISM_PROJE_ID
                                      ,ISM_PROJE_KOD_ID
                                      ,COALESCE(ISM_LOKASYON,'') ISM_LOKASYON
                                      ,COALESCE(ISM_ATOLYE,'') ISM_ATOLYE
                                      ,COALESCE(ISM_EKIPMAN,'') ISM_EKIPMAN
                                      ,COALESCE(ISM_PROJE,'') ISM_PROJE
                                      ,COALESCE(ISM_IS_TALEP_KODU,'') ISM_IS_TALEP_KODU
                                      ,COALESCE(ISM_MAKINE_KOD,'') ISM_MAKINE_KOD
                                      ,COALESCE(ISM_MAKINE_TANIMI,'') ISM_MAKINE_TANIMI
                                      ,COALESCE(ISM_MAKINE_PLAKA,'') ISM_MAKINE_PLAKA
                                      ,COALESCE(ISM_TIP,'') ISM_TIP
                                      ,COALESCE(ISM_DURUM,'') ISM_DURUM
                                      ,ISM_OZEL_ALAN_1
                                      ,ISM_OZEL_ALAN_2
                                      ,ISM_OZEL_ALAN_3
                                      ,ISM_OZEL_ALAN_4
                                      ,ISM_OZEL_ALAN_5
                                      ,ISM_OZEL_ALAN_6
                                      ,ISM_OZEL_ALAN_7
                                      ,ISM_OZEL_ALAN_8
                                      ,ISM_OZEL_ALAN_9
                                      ,ISM_OZEL_ALAN_10
                                      ,ISM_OZEL_ALAN_11_KOD_ID
                                      ,ISM_OZEL_ALAN_12_KOD_ID
                                      ,ISM_OZEL_ALAN_13_KOD_ID
                                      ,ISM_OZEL_ALAN_14_KOD_ID
                                      ,ISM_OZEL_ALAN_15_KOD_ID
                                      ,ISM_OZEL_ALAN_16
                                      ,ISM_OZEL_ALAN_17
                                      ,ISM_OZEL_ALAN_18
                                      ,ISM_OZEL_ALAN_19
                                      ,ISM_OZEL_ALAN_20
                                      ,OA11.KOD_TANIM ISM_OZEL_ALAN_11_KOD_TANIM
                                      ,OA12.KOD_TANIM ISM_OZEL_ALAN_12_KOD_TANIM
                                      ,OA13.KOD_TANIM ISM_OZEL_ALAN_13_KOD_TANIM
                                      ,OA14.KOD_TANIM ISM_OZEL_ALAN_14_KOD_TANIM
                                      ,OA15.KOD_TANIM ISM_OZEL_ALAN_15_KOD_TANIM
                    ,(select COALESCE(TB_RESIM_ID,0) from orjin.TB_RESIM where RSM_VARSAYILAN= 1 AND RSM_REF_GRUP = 'ISEMRI' and RSM_REF_ID = TB_ISEMRI_ID) as ResimVarsayilanID
                    ,stuff((SELECT ';' + CONVERT(varchar(11), R.TB_RESIM_ID) FROM orjin.TB_RESIM R WHERE R.RSM_REF_GRUP = 'ISEMRI' and R.RSM_REF_ID = TB_ISEMRI_ID FOR XML PATH('')),1,1,'') ResimIDleri
                    ,ROW_NUMBER() OVER (ORDER BY ISM_DUZENLEME_TARIH {sortstr}, ISM_DUZENLEME_SAAT {sortstr}) AS RowNum
                    FROM  orjin.VW_ISEMRI
                            LEFT JOIN orjin.TB_KOD OA11 ON OA11.TB_KOD_ID=ISM_OZEL_ALAN_11_KOD_ID
                            LEFT JOIN orjin.TB_KOD OA12 ON OA12.TB_KOD_ID=ISM_OZEL_ALAN_12_KOD_ID
                            LEFT JOIN orjin.TB_KOD OA13 ON OA13.TB_KOD_ID=ISM_OZEL_ALAN_13_KOD_ID
                            LEFT JOIN orjin.TB_KOD OA14 ON OA14.TB_KOD_ID=ISM_OZEL_ALAN_14_KOD_ID
                            LEFT JOIN orjin.TB_KOD OA15 ON OA15.TB_KOD_ID=ISM_OZEL_ALAN_15_KOD_ID
";

                //var q2 = "SELECT COUNT(*) as cnt FROM orjin.VW_ISEMRI " + where + " AND ISM_KAPATILDI=0;";
                //var q3 = "SELECT COUNT(*) as cnt FROM orjin.VW_ISEMRI " + where + " AND ISM_KAPATILDI=1;";
                //var q4 = "SELECT COUNT(*) as cnt FROM orjin.VW_ISEMRI " + where + " AND ISM_KAPATILDI IN (1,0);";
                var wResult = GetFilteredIsemriListWhere(filtre, ID);
                string where = (string)wResult[0];
                if (durumID == 1)
                    where += " and ISM_KAPATILDI = 0";
                else if (durumID == 2)
                    where += " and ISM_KAPATILDI = 1";
                else
                    where += " and ISM_KAPATILDI IN (0,1)";

                query += where;
                query += ") ";
                var q1 = query + "SELECT * FROM mTable WHERE RowNum > @ILK_DEGER AND RowNum <= @SON_DEGER;";
                var q2 = "SELECT * FROM orjin.TB_OZEL_ALAN WHERE OZL_FORM ='ISEMRI';";
                query = q1 + q2; //+ q3 + q4;
                var prms = (DynamicParameters)wResult[1];

                prms.Add("ILK_DEGER", ilkDeger);
                prms.Add("SON_DEGER", sonDeger);
                using (var conn = klas.baglanCmd())
                {
                    var result = conn.QueryMultiple(query, prms);
                    listem = result.Read<IsEmri>().ToList();
                    listem.ForEach(delegate (IsEmri item)
                    {
                        try
                        {
                            RTB.Rtf = item.ISM_ACIKLAMA;
                            item.ISM_ACIKLAMA = RTB.Text;
                        }
                        catch (Exception e)
                        {
                        }
                    });
                    var ozelAlan = result.ReadFirst<OzelAlan>();
                    //var acikSayi = result.ReadFirst<int>();
                    //var kapaliSayi = result.ReadFirst<int>();
                    //var toplamSayi = result.ReadFirst<int>();
                    return new { AcikSayi = 0, KapaliSayi = 0, Toplam = 0, Isemirleri = listem, OzelAlan = ozelAlan };
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        [Route("api/isemriListePaging/{ID}")]
        [HttpPost]
        public async Task<object> IsEmriListePaging([FromBody] Filtre filtre, [FromUri] int ilkDeger, [FromUri] int sonDeger,
            [FromUri] int ID, [FromUri] int durumID, [FromUri] bool sortAsc)
        {
            try
            {
                List<IsEmri> listem;
                string sortstr = sortAsc ? "ASC" : "DESC";
                string query = $@";WITH mTable AS
                    (select 	   TB_ISEMRI_ID
                                      ,coalesce(ISM_ISEMRI_NO,'') ISM_ISEMRI_NO
                                      ,ISM_MAKINE_ID
                                      ,ISM_REF_ID
                                      ,coalesce(ISM_REF_GRUP,'') ISM_REF_GRUP
                                      ,ISM_ONCELIK_ID
									  ,ISM_IS_TALEP_TALEP_EDEN
                                      ,ISM_ONCELIK
                                      ,ISM_BILDIRIM_TARIH
                                      ,ISM_BILDIRIM_SAAT
                                      ,ISM_DUZENLEME_TARIH
                                      ,ISM_DUZENLEME_SAAT
                                      ,ISM_BASLAMA_TARIH
                                      ,ISM_BASLAMA_SAAT
                                      ,ISM_BITIS_TARIH
                                      ,ISM_BITIS_SAAT
                                      ,ISM_KAPATILDI
									  ,ISM_KAPANMA_YDK_TARIH
									  ,ISM_KAPANMA_YDK_SAAT
                                      ,ISM_PLAN_BASLAMA_TARIH
                                      ,ISM_PLAN_BASLAMA_SAAT
                                      ,ISM_PLAN_BITIS_TARIH
                                      ,ISM_PLAN_BITIS_SAAT
                                      ,coalesce(ISM_ACIKLAMA,'') ISM_ACIKLAMA
                                      ,coalesce(ISM_BILDIREN,'') ISM_BILDIREN
                                      ,coalesce(ISM_PUAN,'') ISM_PUAN
                                      ,COALESCE(ISM_SONUC,'') ISM_SONUC
                                      ,ISM_LOKASYON_ID
                                      ,ISM_DURUM_KOD_ID
                                      ,COALESCE(ISM_KONU ,'')  ISM_KONU 
                                      ,COALESCE(ISM_NOT	 ,'')  ISM_NOT	 
                                      ,ISM_TIP_KOD_ID
                                      ,ISM_ATOLYE_ID
                                      ,ISM_EKIPMAN_ID
                                      ,ISM_OLUSTURAN_ID
                                      ,ISM_OLUSTURMA_TARIH
                                      ,ISM_DEGISTIREN_ID
                                      ,ISM_DEGISTIRME_TARIH
                                      ,ISM_MAKINE_DURUM_KOD_ID
                                      ,ISM_GARANTI_KAPSAMINDA
                                      ,ISM_TAMAMLANMA_ORAN
                                      ,ISM_TIP_ID
                                      ,ISM_NEDEN_KOD_ID
                                      ,ISM_NEDEN
                                      ,ISM_TALIMAT_ID
                                      ,ISM_TALIMAT
                                      ,ISM_IS_TIP
                                      ,ISM_IS_TARIH
                                      ,ISM_IS_SAAT
                                      ,ISM_MASRAF_MERKEZ_ID
                                      ,COALESCE(ISM_IS_SONUC,'') ISM_IS_SONUC
                                      ,ISM_SURE_CALISMA
                                      ,ISM_SONUC_KOD_ID
                                      ,ISM_KAPAT_MAKINE_DURUM_KOD_ID
                                      ,ISM_SAYAC_DEGER
                                      ,COALESCE(ISM_MAKINE_GUVENLIK_NOTU,'') ISM_MAKINE_GUVENLIK_NOTU
                                      ,ISM_PROJE_KOD_ID AS ISM_PROJE_ID
                                      ,ISM_PROJE_KOD_ID
                                      ,COALESCE(ISM_LOKASYON,'') ISM_LOKASYON
                                      ,COALESCE(ISM_ATOLYE,'') ISM_ATOLYE
                                      ,COALESCE(ISM_EKIPMAN,'') ISM_EKIPMAN
                                      ,COALESCE(ISM_PROJE,'') ISM_PROJE
                                      ,COALESCE(ISM_IS_TALEP_KODU,'') ISM_IS_TALEP_KODU
                                      ,COALESCE(ISM_MAKINE_KOD,'') ISM_MAKINE_KOD
                                      ,COALESCE(ISM_MAKINE_TANIMI,'') ISM_MAKINE_TANIMI
                                      ,COALESCE(ISM_MAKINE_PLAKA,'') ISM_MAKINE_PLAKA
                                      ,ISM_MAKINE_DURUM
                                      ,COALESCE(ISM_TIP,'') ISM_TIP
                                      ,COALESCE(ISM_DURUM,'') ISM_DURUM
                                      ,ISM_OZEL_ALAN_1
                                      ,ISM_OZEL_ALAN_2
                                      ,ISM_OZEL_ALAN_3
                                      ,ISM_OZEL_ALAN_4
                                      ,ISM_OZEL_ALAN_5
                                      ,ISM_OZEL_ALAN_6
                                      ,ISM_OZEL_ALAN_7
                                      ,ISM_OZEL_ALAN_8
                                      ,ISM_OZEL_ALAN_9
                                      ,ISM_OZEL_ALAN_10
                                      ,ISM_OZEL_ALAN_11_KOD_ID
                                      ,ISM_OZEL_ALAN_12_KOD_ID
                                      ,ISM_OZEL_ALAN_13_KOD_ID
                                      ,ISM_OZEL_ALAN_14_KOD_ID
                                      ,ISM_OZEL_ALAN_15_KOD_ID
                                      ,ISM_OZEL_ALAN_16
                                      ,ISM_OZEL_ALAN_17
                                      ,ISM_OZEL_ALAN_18
                                      ,ISM_OZEL_ALAN_19
                                      ,ISM_OZEL_ALAN_20
                                      ,OA11.KOD_TANIM ISM_OZEL_ALAN_11_KOD_TANIM
                                      ,OA12.KOD_TANIM ISM_OZEL_ALAN_12_KOD_TANIM
                                      ,OA13.KOD_TANIM ISM_OZEL_ALAN_13_KOD_TANIM
                                      ,OA14.KOD_TANIM ISM_OZEL_ALAN_14_KOD_TANIM
                                      ,OA15.KOD_TANIM ISM_OZEL_ALAN_15_KOD_TANIM
                    ,(select COALESCE(TB_RESIM_ID,0) from orjin.TB_RESIM where RSM_VARSAYILAN= 1 AND RSM_REF_GRUP = 'ISEMRI' and RSM_REF_ID = TB_ISEMRI_ID) as ResimVarsayilanID
                    ,stuff((SELECT ';' + CONVERT(varchar(11), R.TB_RESIM_ID) FROM orjin.TB_RESIM R WHERE R.RSM_REF_GRUP = 'ISEMRI' and R.RSM_REF_ID = TB_ISEMRI_ID FOR XML PATH('')),1,1,'') ResimIDleri
                    ,ROW_NUMBER() OVER (ORDER BY ISM_DUZENLEME_TARIH {sortstr}, ISM_DUZENLEME_SAAT {sortstr}) AS RowNum
                    FROM  orjin.VW_ISEMRI
                            LEFT JOIN orjin.TB_KOD OA11 ON OA11.TB_KOD_ID=ISM_OZEL_ALAN_11_KOD_ID
                            LEFT JOIN orjin.TB_KOD OA12 ON OA12.TB_KOD_ID=ISM_OZEL_ALAN_12_KOD_ID
                            LEFT JOIN orjin.TB_KOD OA13 ON OA13.TB_KOD_ID=ISM_OZEL_ALAN_13_KOD_ID
                            LEFT JOIN orjin.TB_KOD OA14 ON OA14.TB_KOD_ID=ISM_OZEL_ALAN_14_KOD_ID
                            LEFT JOIN orjin.TB_KOD OA15 ON OA15.TB_KOD_ID=ISM_OZEL_ALAN_15_KOD_ID
";
                var wResult = GetFilteredIsemriListWhere(filtre, ID);
                string where = (string)wResult[0];

                if (durumID == 1)
                    where += " and ISM_KAPATILDI = 0";
                else if (durumID == 2)
                    where += " and ISM_KAPATILDI = 1";
                else
                    where += " and ISM_KAPATILDI IN (0,1)";

                query += where;
                query += ") ";
                var listQuery = query + "SELECT * FROM mTable WHERE RowNum > @ILK_DEGER AND RowNum <= @SON_DEGER;";
                var ozelAlanQuery = "SELECT * FROM orjin.TB_OZEL_ALAN WHERE OZL_FORM ='ISEMRI';";

                var totalCountQuery = $"SELECT COUNT(*) as cnt FROM orjin.VW_ISEMRI {where};";
                query = listQuery + ozelAlanQuery+totalCountQuery;
                var prms = (DynamicParameters)wResult[1];

                prms.Add("ILK_DEGER", ilkDeger);
                prms.Add("SON_DEGER", sonDeger);
                using (var conn = klas.baglanCmd())
                {
                    var result = await conn.QueryMultipleAsync(query, prms);
                    listem = result.Read<IsEmri>().ToList();
                    listem.ForEach(delegate (IsEmri item)
                    {
                        try
                        {
                            RTB.Rtf = item.ISM_ACIKLAMA;
                            item.ISM_ACIKLAMA = RTB.Text;
                        }
                        catch (Exception e)
                        {
                        }
                    });
                    var ozelAlan = result.ReadFirst<OzelAlan>();
                    var filteredCount = result.ReadFirst<int>();
                    //var kapaliSayi = result.ReadFirst<int>();
                    //var toplamSayi = result.ReadFirst<int>();
                    return new { Filtered = filteredCount, Items = listem, OzelAlan = ozelAlan };
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }



        [Route("api/isemrisayilar")]
        [HttpPost]
        public async Task<object> GetIsemriSayilar([FromBody] Filtre filtre, [FromUri] int id)
        {
            var wResult = GetFilteredIsemriListWhere(filtre, id);
            var where = (string)wResult[0];
            var prms = (DynamicParameters)wResult[1];
            var q2 = "SELECT COUNT(*) as cnt FROM orjin.VW_ISEMRI " + where + " AND ISM_KAPATILDI=0;";
            var q3 = "SELECT COUNT(*) as cnt FROM orjin.VW_ISEMRI " + where + " AND ISM_KAPATILDI=1;";
            var q4 = "SELECT COUNT(*) as cnt FROM orjin.VW_ISEMRI " + where + " AND ISM_KAPATILDI IN (1,0);";

            using (var conn = klas.baglan())
            {
                var result = await conn.QueryMultipleAsync(q2 + q3 + q4, prms);
                return new
                {
                    AcikSayi = result.ReadFirst<int>(),
                    KapaliSayi = result.ReadFirst<int>(),
                    Toplam = result.ReadFirst<int>()
                };
            }
        }

        // iş emri ekle
        public async Task<Bildirim> Post([FromBody] IsEmri entity, [FromUri] int ID)
        {
            Bildirim bildirimEntity = new Bildirim();
            var util = new Util();
            var prms = new DynamicParameters();
            using (var cnn = util.baglan())
            {
                try
                {
                    prms.Add("ISM_ISEMRI_NO", entity.ISM_ISEMRI_NO);
                    bool workOrderExists = await cnn.QueryFirstOrDefaultAsync<int>("SELECT COUNT(*) FROM orjin.TB_ISEMRI WHERE ISM_ISEMRI_NO=@ISM_ISEMRI_NO", prms) > 0;
                    if (entity.TB_ISEMRI_ID < 1 && !workOrderExists)
                    {

                        #region Kaydet

                        string sql = @"INSERT INTO orjin.TB_ISEMRI
                                    (  ISM_ISEMRI_NO
                                      ,ISM_MAKINE_ID   
                                      ,ISM_DUZENLEME_TARIH
                                      ,ISM_DUZENLEME_SAAT
                                      ,ISM_BASLAMA_TARIH
                                      ,ISM_BASLAMA_SAAT
                                      ,ISM_BITIS_TARIH
                                      ,ISM_BITIS_SAAT
                                      ,ISM_PLAN_BASLAMA_TARIH
                                      ,ISM_PLAN_BASLAMA_SAAT
                                      ,ISM_PLAN_BITIS_TARIH
                                      ,ISM_PLAN_BITIS_SAAT
                                      ,ISM_KONU
                                      ,ISM_LOKASYON_ID
                                      ,ISM_PROJE_ID
                                      ,ISM_TIP_KOD_ID
                                      ,ISM_ONCELIK_ID
                                      ,ISM_ATOLYE_ID     
                                      ,ISM_MASRAF_MERKEZ_ID
                                      ,ISM_REF_ID
                                      ,ISM_REF_GRUP
                                      ,ISM_TIP_ID
                                      ,ISM_OLUSTURAN_ID
                                      ,ISM_OLUSTURMA_TARIH
                                      ,ISM_SURE_CALISMA
                                      ,ISM_DURUM_KOD_ID
                                      ,ISM_SAYAC_DEGER
                                      ,ISM_BILDIREN
                                      ,ISM_MAKINE_DURUM_KOD_ID
                                      ,ISM_MAKINE_GUVENLIK_NOTU
                                      ,ISM_ACIKLAMA
                                      ,ISM_IS_TARIH
                                      ,ISM_IS_SAAT
                                      ,ISM_KAPATILDI
                                      ,ISM_OZEL_ALAN_1 
                                      ,ISM_OZEL_ALAN_2 
                                      ,ISM_OZEL_ALAN_3 
                                      ,ISM_OZEL_ALAN_4 
                                      ,ISM_OZEL_ALAN_5 
                                      ,ISM_OZEL_ALAN_6 
                                      ,ISM_OZEL_ALAN_7 
                                      ,ISM_OZEL_ALAN_8 
                                      ,ISM_OZEL_ALAN_9
                                      ,ISM_OZEL_ALAN_10
                                      ,ISM_OZEL_ALAN_11_KOD_ID
                                      ,ISM_OZEL_ALAN_12_KOD_ID
                                      ,ISM_OZEL_ALAN_13_KOD_ID
                                      ,ISM_OZEL_ALAN_14_KOD_ID
                                      ,ISM_OZEL_ALAN_15_KOD_ID
                                      ,ISM_OZEL_ALAN_16
                                      ,ISM_OZEL_ALAN_17
                                      ,ISM_OZEL_ALAN_18
                                      ,ISM_OZEL_ALAN_19
                                      ,ISM_OZEL_ALAN_20
)
                                    VALUES( @ISM_ISEMRI_NO
                                      ,@ISM_MAKINE_ID   
                                      ,@ISM_DUZENLEME_TARIH
                                      ,@ISM_DUZENLEME_SAAT
                                      ,@ISM_BASLAMA_TARIH
                                      ,@ISM_BASLAMA_SAAT
                                      ,@ISM_BITIS_TARIH
                                      ,@ISM_BITIS_SAAT
                                      ,@ISM_PLAN_BASLAMA_TARIH
                                      ,@ISM_PLAN_BASLAMA_SAAT
                                      ,@ISM_PLAN_BITIS_TARIH
                                      ,@ISM_PLAN_BITIS_SAAT
                                      ,@ISM_KONU
                                      ,@ISM_LOKASYON_ID
                                      ,@ISM_PROJE_ID
                                      ,@ISM_TIP_KOD_ID
                                      ,@ISM_ONCELIK_ID
                                      ,@ISM_ATOLYE_ID     
                                      ,@ISM_MASRAF_MERKEZ_ID
                                      ,@ISM_REF_ID
                                      ,@ISM_REF_GRUP
                                      ,@ISM_TIP_ID
                                      ,@ISM_OLUSTURAN_ID
                                      ,@ISM_OLUSTURMA_TARIH
                                      ,@ISM_SURE_CALISMA
                                      ,@ISM_DURUM_KOD_ID
                                      ,@ISM_SAYAC_DEGER
                                      ,@ISM_BILDIREN
                                      ,@ISM_MAKINE_DURUM_KOD_ID
                                      ,@ISM_MAKINE_GUVENLIK_NOTU
                                      ,@ISM_ACIKLAMA
                                      ,@ISM_IS_TARIH
                                      ,@ISM_IS_SAAT
                                      ,@ISM_KAPATILDI
                                      ,@ISM_OZEL_ALAN_1
                                      ,@ISM_OZEL_ALAN_2
                                      ,@ISM_OZEL_ALAN_3
                                      ,@ISM_OZEL_ALAN_4
                                      ,@ISM_OZEL_ALAN_5
                                      ,@ISM_OZEL_ALAN_6
                                      ,@ISM_OZEL_ALAN_7
                                      ,@ISM_OZEL_ALAN_8
                                      ,@ISM_OZEL_ALAN_9
                                      ,@ISM_OZEL_ALAN_10
                                      ,@ISM_OZEL_ALAN_11_KOD_ID
                                      ,@ISM_OZEL_ALAN_12_KOD_ID
                                      ,@ISM_OZEL_ALAN_13_KOD_ID
                                      ,@ISM_OZEL_ALAN_14_KOD_ID
                                      ,@ISM_OZEL_ALAN_15_KOD_ID
                                      ,@ISM_OZEL_ALAN_16
                                      ,@ISM_OZEL_ALAN_17
                                      ,@ISM_OZEL_ALAN_18
                                      ,@ISM_OZEL_ALAN_19
                                      ,@ISM_OZEL_ALAN_20
)";

                        prms.Add("@ISM_ISEMRI_NO", entity.ISM_ISEMRI_NO);
                        prms.Add("@ISM_DUZENLEME_TARIH", DateTime.Now.ToString("yyyy-MM-dd"));
                        prms.Add("@ISM_DUZENLEME_SAAT", DateTime.Now.ToString(C.DB_TIME_FORMAT));
                        prms.Add("@ISM_BASLAMA_TARIH", entity.ISM_BASLAMA_TARIH);
                        prms.Add("@ISM_BASLAMA_SAAT", string.IsNullOrWhiteSpace(entity.ISM_BASLAMA_SAAT) ? null : entity.ISM_BASLAMA_SAAT);
                        prms.Add("@ISM_BITIS_TARIH", entity.ISM_BITIS_TARIH);
                        prms.Add("@ISM_BITIS_SAAT", string.IsNullOrWhiteSpace(entity.ISM_BITIS_SAAT) ? null : entity.ISM_BITIS_SAAT);
                        prms.Add("@ISM_PLAN_BASLAMA_TARIH", entity.ISM_PLAN_BASLAMA_TARIH);
                        prms.Add("@ISM_PLAN_BASLAMA_SAAT", string.IsNullOrWhiteSpace(entity.ISM_PLAN_BASLAMA_SAAT) ? null : entity.ISM_PLAN_BASLAMA_SAAT);
                        prms.Add("@ISM_PLAN_BITIS_TARIH", entity.ISM_PLAN_BITIS_TARIH);
                        prms.Add("@ISM_PLAN_BITIS_SAAT", string.IsNullOrWhiteSpace(entity.ISM_PLAN_BITIS_SAAT) ? null : entity.ISM_PLAN_BITIS_SAAT);
                        prms.Add("@ISM_KONU", entity.ISM_KONU);
                        prms.Add("@ISM_MAKINE_ID", entity.ISM_MAKINE_ID);
                        prms.Add("@ISM_LOKASYON_ID", entity.ISM_LOKASYON_ID);
                        prms.Add("@ISM_PROJE_ID", entity.ISM_PROJE_ID);
                        prms.Add("@ISM_TIP_KOD_ID", entity.ISM_TIP_KOD_ID);
                        prms.Add("@ISM_ONCELIK_ID", entity.ISM_ONCELIK_ID);
                        prms.Add("@ISM_ATOLYE_ID", entity.ISM_ATOLYE_ID);
                        prms.Add("@ISM_MASRAF_MERKEZ_ID", entity.ISM_MASRAF_MERKEZ_ID);
                        prms.Add("@ISM_REF_ID", entity.ISM_REF_ID);
                        prms.Add("@ISM_REF_GRUP", entity.ISM_REF_GRUP);
                        prms.Add("@ISM_TIP_ID", entity.ISM_TIP_ID);
                        prms.Add("@ISM_OLUSTURAN_ID", entity.ISM_OLUSTURAN_ID);
                        prms.Add("@ISM_SURE_CALISMA", entity.ISM_SURE_CALISMA);
                        prms.Add("@ISM_OLUSTURMA_TARIH", DateTime.Now);
                        prms.Add("@ISM_DURUM_KOD_ID", entity.ISM_DURUM_KOD_ID);
                        prms.Add("@ISM_SAYAC_DEGER", entity.ISM_SAYAC_DEGER);
                        prms.Add("@ISM_BILDIREN", entity.ISM_BILDIREN != null ? entity.ISM_BILDIREN : "");
                        prms.Add("@ISM_MAKINE_DURUM_KOD_ID", entity.ISM_MAKINE_DURUM_KOD_ID != 0 ? entity.ISM_MAKINE_DURUM_KOD_ID : -1);
                        prms.Add("@ISM_MAKINE_GUVENLIK_NOTU", entity.ISM_MAKINE_GUVENLIK_NOTU ?? "");
                        prms.Add("@ISM_ACIKLAMA", entity.ISM_ACIKLAMA ?? "");
                        prms.Add("@ISM_IS_TARIH", entity.ISM_IS_TARIH);
                        prms.Add("@ISM_IS_SAAT", entity.ISM_IS_SAAT ?? "");
                        prms.Add("@ISM_KAPATILDI", false);
                        prms.Add("ISM_OZEL_ALAN_1", entity.ISM_OZEL_ALAN_1);
                        prms.Add("ISM_OZEL_ALAN_2", entity.ISM_OZEL_ALAN_2);
                        prms.Add("ISM_OZEL_ALAN_3", entity.ISM_OZEL_ALAN_3);
                        prms.Add("ISM_OZEL_ALAN_4", entity.ISM_OZEL_ALAN_4);
                        prms.Add("ISM_OZEL_ALAN_5", entity.ISM_OZEL_ALAN_5);
                        prms.Add("ISM_OZEL_ALAN_6", entity.ISM_OZEL_ALAN_6);
                        prms.Add("ISM_OZEL_ALAN_7", entity.ISM_OZEL_ALAN_7);
                        prms.Add("ISM_OZEL_ALAN_8", entity.ISM_OZEL_ALAN_8);
                        prms.Add("ISM_OZEL_ALAN_9", entity.ISM_OZEL_ALAN_9);
                        prms.Add("@ISM_OZEL_ALAN_11_KOD_ID", entity.ISM_OZEL_ALAN_11_KOD_ID);
                        prms.Add("@ISM_OZEL_ALAN_12_KOD_ID", entity.ISM_OZEL_ALAN_12_KOD_ID);
                        prms.Add("@ISM_OZEL_ALAN_13_KOD_ID", entity.ISM_OZEL_ALAN_13_KOD_ID);
                        prms.Add("@ISM_OZEL_ALAN_14_KOD_ID", entity.ISM_OZEL_ALAN_14_KOD_ID);
                        prms.Add("@ISM_OZEL_ALAN_15_KOD_ID", entity.ISM_OZEL_ALAN_15_KOD_ID);
                        prms.Add("ISM_OZEL_ALAN_10", entity.ISM_OZEL_ALAN_10);
                        prms.Add("ISM_OZEL_ALAN_16", entity.ISM_OZEL_ALAN_16);
                        prms.Add("ISM_OZEL_ALAN_17", entity.ISM_OZEL_ALAN_17);
                        prms.Add("ISM_OZEL_ALAN_18", entity.ISM_OZEL_ALAN_18);
                        prms.Add("ISM_OZEL_ALAN_19", entity.ISM_OZEL_ALAN_19);
                        prms.Add("ISM_OZEL_ALAN_20", entity.ISM_OZEL_ALAN_20);
                        await cnn.ExecuteAsync(sql, prms);

                        IsEmri drSonIsEmri = await cnn.QueryFirstAsync<IsEmri>("SELECT TOP 1 * FROM orjin.TB_ISEMRI ORDER BY TB_ISEMRI_ID DESC");
                        //ilk kayıtta dökümantasyon yapılmayacağı için kapatıldı.
                        // kontol listesi ekleme.
                        if (drSonIsEmri != null)
                        {
                            if (entity.ISM_REF_ID != -1)
                            {
                                if (entity.ISM_TIP == "PERİYODİK BAKIM")
                                {
                                    PeriyodikBakimController pBakim = new PeriyodikBakimController();
                                    entity.IsEmriKontrolList = pBakim.PBakimKontrolList(entity.ISM_REF_ID);
                                    entity.IsEmriMalzemeList = pBakim.PBakimMazleme(entity.ISM_REF_ID);
                                }
                                else
                                {
                                    IsTanimController tanim = new IsTanimController(_logger);
                                    entity.IsEmriKontrolList = tanim.IsTanimIsmKontrolList(entity.ISM_REF_ID);
                                    entity.IsEmriMalzemeList = tanim.IsTanimMazleme(entity.ISM_REF_ID);
                                }

                                foreach (var isEmriKontrolList in entity.IsEmriKontrolList)
                                {
                                    isEmriKontrolList.DKN_ISEMRI_ID = drSonIsEmri.TB_ISEMRI_ID;
                                    KontrolListKaydet(isEmriKontrolList);
                                }

                                foreach (var isEmriMalzeme in entity.IsEmriMalzemeList)
                                {
                                    isEmriMalzeme.IDM_ISEMRI_ID = drSonIsEmri.TB_ISEMRI_ID;
                                    isEmriMalzeme.IDM_TARIH = DateTime.Now;
                                    isEmriMalzeme.IDM_SAAT = DateTime.Now.ToString(C.DB_TIME_FORMAT);
                                    isEmriMalzeme.IDM_OLUSTURMA_TARIH = DateTime.Now;
                                    isEmriMalzeme.IDM_OLUSTURAN_ID = ID;
                                    MalzemeListKaydet(isEmriMalzeme);
                                }
                            }
                        }
                        //// Personel ekleme.
                        //for (int i = 0; i < entity.IsEmriPersonelList.Count; i++)
                        //{
                        //    PersonelListKaydet(entity.IsEmriPersonelList[i]);
                        //}

                        string query = @"INSERT INTO [orjin].[TB_ISEMRI_LOG]
                                           (ISL_ISEMRI_ID
                                           ,ISL_KULLANICI_ID
                                           ,ISL_TARIH
                                           ,ISL_SAAT
                                           ,ISL_ISLEM
                                           ,ISL_DURUM_ESKI_KOD_ID
                                           ,ISL_DURUM_YENI_KOD_ID
                                           ,ISL_OLUSTURAN_ID
                                           ,ISL_OLUSTURMA_TARIH)
                                     VALUES
                                            (@ISL_ISEMRI_ID
                                            ,@ISL_KULLANICI_ID
                                            ,@ISL_TARIH
                                            ,@ISL_SAAT
                                            ,@ISL_ISLEM
                                            ,@ISL_DURUM_ESKI_KOD_ID
                                            ,@ISL_DURUM_YENI_KOD_ID
                                            ,@ISL_OLUSTURAN_ID
                                            ,@ISL_OLUSTURMA_TARIH)";


                        if (drSonIsEmri != null)
                        {
                            prms = new DynamicParameters();
                            prms.Add("ISL_ISEMRI_ID", drSonIsEmri.TB_ISEMRI_ID);
                            prms.Add("ISL_KULLANICI_ID", entity.ISM_OLUSTURAN_ID);
                            prms.Add("ISL_TARIH", DateTime.Now);
                            prms.Add("ISL_SAAT", DateTime.Now.ToString(C.DB_TIME_FORMAT));
                            prms.Add("ISL_ISLEM", "İş emri açılışı");
                            prms.Add("ISL_DURUM_ESKI_KOD_ID", -1);
                            prms.Add("ISL_DURUM_YENI_KOD_ID", -1);
                            prms.Add("ISL_OLUSTURAN_ID", entity.ISM_OLUSTURAN_ID);
                            prms.Add("ISL_OLUSTURMA_TARIH", DateTime.Now);
                            await cnn.ExecuteAsync(query, prms);
                        }

                        #endregion

                    }
                    else
                    {
                        bildirimEntity.Id = entity.TB_ISEMRI_ID;
                        bildirimEntity.Durum = false;
                        bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                        bildirimEntity.Aciklama = Localization.IsemriMevcut;
                    }


                    bildirimEntity.Id = await cnn.QueryFirstAsync<int>("SELECT MAX(TB_ISEMRI_ID) FROM orjin.TB_ISEMRI");
                    bildirimEntity.Aciklama = "İşlem başarılı bir şekilde gerçekleşti.";
                    bildirimEntity.MsgId = Bildirim.MSG_ISLEM_BASARILI;
                    bildirimEntity.Durum = true;
                    // Bildirim Gönder
                    try
                    {
                        var parametreler=new DynamicParameters();
                        parametreler.Add("ISM_OLUSTURAN_ID", entity.ISM_OLUSTURAN_ID);
                        string queryBld =
                            $@"SELECT * FROM {util.GetMasterDbName()}.orjin.TB_KULLANICI WHERE KLL_ROLBILGISI ='TEKNIK' AND KLL_MOBIL_BILDIRI=1 AND KLL_MOBIL_KULLANICI = 1 AND TB_KULLANICI_ID <> @ISM_OLUSTURAN_ID";
                        if (entity.ISM_LOKASYON_ID > 0)
                        {
                            parametreler.Add("ISM_LOKASYON_ID", entity.ISM_LOKASYON_ID);
                            queryBld = queryBld +
                                       " AND orjin.UDF_LOKASYON_YETKI_KONTROL(@ISM_LOKASYON_ID ,TB_KULLANICI_ID) = 1";
                        }

                        if (entity.ISM_ATOLYE_ID > 0)
                        {
                            parametreler.Add("ISM_ATOLYE_ID", entity.ISM_ATOLYE_ID);
                            queryBld = queryBld + "AND orjin.UDF_ATOLYE_YETKI_KONTROL(@ISM_ATOLYE_ID ,TB_KULLANICI_ID) = 1";
                        }

                        var cihazIDler = (await cnn.QueryAsync<String>(queryBld, parametreler)).ToArray();
                        if (cihazIDler.Length > 0)
                        {
                            Util.SendNotificationToTopic(entity.ISM_ISEMRI_NO, Localization.YeniIsEmri,
                                string.Format(Localization.NoluIsemriOlusturuldu, entity.ISM_ISEMRI_NO), Util.isEmri,
                                cihazIDler);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e);
                    }
                }
                catch (Exception e)
                {
                    bildirimEntity.Aciklama = string.Format(Localization.errorFormatted, e.Message);
                    bildirimEntity.MsgId = Bildirim.MSG_IS_EMRI_KAYIT_HATA;
                    bildirimEntity.HasExtra = true;
                    bildirimEntity.Durum = false;
                    _logger.Error(e);
                }

                return bildirimEntity;
            }
        }

        [Route("api/IsEmriKodGetir")]
        public async Task<TanimDeger> GetIsEmriKodGetir()
        {
            var sql = "";
            TanimDeger entity = new TanimDeger();
            var util = new Util();
            using (var cnn=util.baglan())
            {
                await cnn.ExecuteAsync("UPDATE orjin.TB_NUMARATOR SET NMR_NUMARA = NMR_NUMARA+1 WHERE NMR_KOD = 'ISM_ISEMRI_NO'");
                string ss = "";
                sql = @"select 
                        NMR_ON_EK+right(replicate('0',NMR_HANE_SAYISI)+CAST(NMR_NUMARA AS VARCHAR(MAX)),NMR_HANE_SAYISI) as deger FROM ORJİN.TB_NUMARATOR
                        WHERE NMR_KOD = 'ISM_ISEMRI_NO'";
                entity.Tanim = await cnn.QueryFirstOrDefaultAsync<string>(sql);
            }
            
            return entity;
        }

        [HttpPost]
        [Route("api/IsEmriSil")]
        public Bildirim IsEmriSil([FromUri] int ID, [FromUri] int KulID)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                int _isemriID = Convert.ToInt32(ID);
                // Araç Gereç SİLİNİYOR..
                parametreler.Clear();
                parametreler.Add(new Prm("ISM_ID", _isemriID));
                klas.cmd("DELETE FROM orjin.TB_ISEMRI_ARAC_GEREC WHERE IAG_ISEMRI_ID = @ISM_ID", parametreler);
                // Personel siliniyor
                klas.cmd("DELETE FROM orjin.TB_ISEMRI_KAYNAK WHERE IDK_ISEMRI_ID = @ISM_ID", parametreler);
                // Kontrol Listesi siliniyor
                klas.cmd("DELETE FROM orjin.TB_ISEMRI_KONTROLLIST WHERE DKN_ISEMRI_ID =@ISM_ID", parametreler);
                // Kontrol Listesi siliniyor
                klas.cmd("DELETE FROM orjin.TB_ISEMRI_LOG WHERE ISL_ISEMRI_ID =@ISM_ID", parametreler);
                // Malzemeler siliniyor
                List<IsEmriMalzeme> mlzList = IsEmriMalzemeList(_isemriID);
                for (int i = 0; i < mlzList.Count; i++)
                {
                    if (mlzList[i].IDM_STOK_KULLANIM_SEKLI == 1)
                        IsEmriMalzemeSil(mlzList[i]);
                }

                klas.cmd("DELETE FROM orjin.TB_ISEMRI_MLZ WHERE IDM_ISEMRI_ID =@ISM_ID", parametreler);
                // olçüm listesi siliniyor
                klas.cmd("DELETE FROM orjin.TB_ISEMRI_OLCUM WHERE IDO_ISEMRI_ID =@ISM_ID", parametreler);
                // DOKUMAN SİLİNİYOR
                klas.cmd("DELETE FROM TB_DOSYA WHERE DSY_REF_ID = @ISM_ID AND DSY_REF_GRUP = 'ISEMRI'", parametreler);

                // iş talep durumu değiştirilip log yazılıyor
                DataRow drTalep = klas.GetDataRow("select * from orjin.TB_IS_TALEBI where IST_ISEMRI_ID=@ISM_ID",
                    parametreler);
                if (drTalep != null)
                {
                    klas.cmd("UPDATE orjin.TB_IS_TALEBI SET IST_DURUM_ID=1 WHERE IST_ISEMRI_ID = @ISM_ID",
                        parametreler);
                    IsTalepController.IsTalepTarihceYaz(Convert.ToInt32(drTalep["TB_IS_TALEP_ID"]), KulID, "Silme",
                        klas.GetDataCell("select ISM_ISEMRI_NO from orjin.TB_ISEMRI where TB_ISEMRI_ID= @ISM_ID",
                            parametreler) + " nolu iş emri silindi", "Silme");
                }

                //emrin kendisi siliniyor..
                klas.cmd("DELETE FROM orjin.TB_ISEMRI WHERE TB_ISEMRI_ID = @ISM_ID", parametreler);


                bildirimEntity.Aciklama = "İş emri başarılı bir şekilde silindi.";
                bildirimEntity.MsgId = Bildirim.MSG_ISEMRI_SIL_BASARILI;
                bildirimEntity.Durum = true;
            }
            catch (Exception e)
            {
                bildirimEntity.Aciklama = String.Format(Localization.errorFormatted, e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_ISEMRI_SIL_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }

            return bildirimEntity;
        }

        [HttpPost]
        [Route("api/IsEmriDurumDegisikligi")]
        public void IsEmriDurumDegisikligi(int eskiDID, int yeniDID, string aciklama, string eskiDTanim,
            string yeniDTanim, int isEmriID, int KulID)
        {
            try
            {
                string query = @"INSERT INTO orjin.TB_ISEMRI_LOG
                                           (ISL_ISEMRI_ID
                                           ,ISL_KULLANICI_ID
                                           ,ISL_TARIH
                                           ,ISL_SAAT
                                           ,ISL_ISLEM
                                           ,ISL_DURUM_ESKI_KOD_ID
                                           ,ISL_DURUM_YENI_KOD_ID
                                           ,ISL_OLUSTURAN_ID
                                           ,ISL_OLUSTURMA_TARIH,ISL_ACIKLAMA)
                                     VALUES
                                            (@ISL_ISEMRI_ID
                                            ,@ISL_KULLANICI_ID
                                            ,@ISL_TARIH
                                            ,@ISL_SAAT
                                            ,@ISL_ISLEM
                                            ,@ISL_DURUM_ESKI_KOD_ID
                                            ,@ISL_DURUM_YENI_KOD_ID
                                            ,@ISL_OLUSTURAN_ID
                                            ,@ISL_OLUSTURMA_TARIH,@ISL_ACIKLAMA)";
                prms.Clear();
                prms.Add("ISL_ISEMRI_ID", isEmriID);
                prms.Add("ISL_KULLANICI_ID", KulID);
                prms.Add("ISL_ACIKLAMA", aciklama);
                prms.Add("ISL_TARIH", DateTime.Now);
                prms.Add("ISL_SAAT", DateTime.Now.ToString("HH:mm:ss"));
                prms.Add("ISL_ISLEM",
                    String.Format("İş emri durum bilgisi değişti. {0} -> {1}", eskiDTanim, yeniDTanim));
                prms.Add("ISL_DURUM_ESKI_KOD_ID", eskiDID);
                prms.Add("ISL_DURUM_YENI_KOD_ID", yeniDID);
                prms.Add("ISL_OLUSTURAN_ID", KulID);
                prms.Add("ISL_OLUSTURMA_TARIH", DateTime.Now);
                klas.cmd(query, prms.PARAMS);
            }
            catch (Exception)
            {
                klas.kapat();
                throw;
            }
        }

        [HttpPost]
        [Route("api/IsEmriGuncelle")]
        public void IsEmriGuncelle([FromBody] IsEmri entity)
        {
            var util = new Util();
            using (var cnn=util.baglan())
            {

                try
                {
                    if (entity.TB_ISEMRI_ID > 0)
                    {
                        #region Guncelle

                        string query = @"UPDATE orjin.TB_ISEMRI SET
                                     ISM_ISEMRI_NO =@ISM_ISEMRI_NO 
                                      ,ISM_MAKINE_ID = @ISM_MAKINE_ID                                      
                                      ,ISM_BASLAMA_TARIH = @ISM_BASLAMA_TARIH
                                      ,ISM_BASLAMA_SAAT = @ISM_BASLAMA_SAAT
                                      ,ISM_BITIS_TARIH = @ISM_BITIS_TARIH
                                      ,ISM_BITIS_SAAT = @ISM_BITIS_SAAT                             
                                      ,ISM_PLAN_BASLAMA_TARIH   = @ISM_PLAN_BASLAMA_TARIH
                                      ,ISM_PLAN_BASLAMA_SAAT    = @ISM_PLAN_BASLAMA_SAAT
                                      ,ISM_PLAN_BITIS_TARIH     = @ISM_PLAN_BITIS_TARIH
                                      ,ISM_PLAN_BITIS_SAAT      = @ISM_PLAN_BITIS_SAAT
                                      ,ISM_KONU = @ISM_KONU
                                      ,ISM_ACIKLAMA = @ISM_ACIKLAMA
                                      ,ISM_LOKASYON_ID = @ISM_LOKASYON_ID
                                      ,ISM_PROJE_ID=@ISM_PROJE_ID   
                                      ,ISM_TIP_KOD_ID=@ISM_TIP_KOD_ID
                                      ,ISM_ONCELIK_ID=@ISM_ONCELIK_ID
                                      ,ISM_ATOLYE_ID=@ISM_ATOLYE_ID
                                      ,ISM_MASRAF_MERKEZ_ID=@ISM_MASRAF_MERKEZ_ID
                                      ,ISM_REF_ID=@ISM_REF_ID
                                      ,ISM_REF_GRUP=@ISM_REF_GRUP
                                      ,ISM_TIP_ID = @ISM_TIP_ID 
                                      ,ISM_DEGISTIREN_ID=@ISM_DEGISTIREN_ID
                                      ,ISM_DEGISTIRME_TARIH=@ISM_DEGISTIRME_TARIH
                                      ,ISM_SURE_CALISMA = @ISM_SURE_CALISMA 
                                      ,ISM_DURUM_KOD_ID=@ISM_DURUM_KOD_ID
                                      ,ISM_SAYAC_DEGER=@ISM_SAYAC_DEGER
                                      ,ISM_OZEL_ALAN_1  =@ISM_OZEL_ALAN_1
                                      ,ISM_OZEL_ALAN_2  =@ISM_OZEL_ALAN_2
                                      ,ISM_OZEL_ALAN_3  =@ISM_OZEL_ALAN_3
                                      ,ISM_OZEL_ALAN_4  =@ISM_OZEL_ALAN_4
                                      ,ISM_OZEL_ALAN_5  =@ISM_OZEL_ALAN_5
                                      ,ISM_OZEL_ALAN_6  =@ISM_OZEL_ALAN_6
                                      ,ISM_OZEL_ALAN_7  =@ISM_OZEL_ALAN_7
                                      ,ISM_OZEL_ALAN_8  =@ISM_OZEL_ALAN_8
                                      ,ISM_OZEL_ALAN_9  =@ISM_OZEL_ALAN_9
                                      ,ISM_OZEL_ALAN_10 =@ISM_OZEL_ALAN_10
                                      ,ISM_OZEL_ALAN_11_KOD_ID = @ISM_OZEL_ALAN_11_KOD_ID
                                      ,ISM_OZEL_ALAN_12_KOD_ID = @ISM_OZEL_ALAN_12_KOD_ID
                                      ,ISM_OZEL_ALAN_13_KOD_ID = @ISM_OZEL_ALAN_13_KOD_ID
                                      ,ISM_OZEL_ALAN_14_KOD_ID = @ISM_OZEL_ALAN_14_KOD_ID
                                      ,ISM_OZEL_ALAN_15_KOD_ID = @ISM_OZEL_ALAN_15_KOD_ID
                                      ,ISM_OZEL_ALAN_16 =@ISM_OZEL_ALAN_16
                                      ,ISM_OZEL_ALAN_17 =@ISM_OZEL_ALAN_17
                                      ,ISM_OZEL_ALAN_18 =@ISM_OZEL_ALAN_18
                                      ,ISM_OZEL_ALAN_19 =@ISM_OZEL_ALAN_19
                                      ,ISM_OZEL_ALAN_20 =@ISM_OZEL_ALAN_20
                                       WHERE TB_ISEMRI_ID = @TB_ISEMRI_ID";
                        prms.Clear();
                        prms.Add("@TB_ISEMRI_ID", entity.TB_ISEMRI_ID);
                        prms.Add("@ISM_ISEMRI_NO", entity.ISM_ISEMRI_NO);
                        prms.Add("@ISM_BASLAMA_TARIH", entity.ISM_BASLAMA_TARIH);
                        prms.Add("@ISM_BASLAMA_SAAT", entity.ISM_BASLAMA_SAAT);
                        prms.Add("@ISM_BITIS_TARIH", entity.ISM_BITIS_TARIH);
                        prms.Add("@ISM_BITIS_SAAT", entity.ISM_BITIS_SAAT);
                        prms.Add("@ISM_PLAN_BASLAMA_TARIH", entity.ISM_PLAN_BASLAMA_TARIH);
                        prms.Add("@ISM_PLAN_BASLAMA_SAAT", entity.ISM_PLAN_BASLAMA_SAAT);
                        prms.Add("@ISM_PLAN_BITIS_TARIH", entity.ISM_PLAN_BITIS_TARIH);
                        prms.Add("@ISM_PLAN_BITIS_SAAT", entity.ISM_PLAN_BITIS_SAAT);
                        prms.Add("@ISM_KONU", entity.ISM_KONU);
                        prms.Add("@ISM_ACIKLAMA", entity.ISM_ACIKLAMA);
                        prms.Add("@ISM_MAKINE_ID", entity.ISM_MAKINE_ID);
                        prms.Add("@ISM_LOKASYON_ID", entity.ISM_LOKASYON_ID);
                        prms.Add("@ISM_PROJE_ID", entity.ISM_PROJE_ID);
                        prms.Add("@ISM_TIP_KOD_ID", entity.ISM_TIP_KOD_ID);
                        prms.Add("@ISM_ONCELIK_ID", entity.ISM_ONCELIK_ID);
                        prms.Add("@ISM_ATOLYE_ID", entity.ISM_ATOLYE_ID);
                        prms.Add("@ISM_MASRAF_MERKEZ_ID", entity.ISM_MASRAF_MERKEZ_ID);
                        prms.Add("@ISM_REF_ID", entity.ISM_REF_ID);
                        prms.Add("@ISM_REF_GRUP", entity.ISM_REF_GRUP);
                        prms.Add("@ISM_TIP_ID", entity.ISM_TIP_ID);
                        prms.Add("@ISM_DEGISTIREN_ID", entity.ISM_DEGISTIREN_ID);
                        prms.Add("@ISM_SURE_CALISMA", entity.ISM_SURE_CALISMA);
                        prms.Add("@ISM_DEGISTIRME_TARIH", DateTime.Now);
                        prms.Add("@ISM_DURUM_KOD_ID", entity.ISM_DURUM_KOD_ID);
                        prms.Add("@ISM_SAYAC_DEGER", entity.ISM_SAYAC_DEGER);
                        prms.Add("@ISM_OZEL_ALAN_1", entity.ISM_OZEL_ALAN_1);
                        prms.Add("@ISM_OZEL_ALAN_2", entity.ISM_OZEL_ALAN_2);
                        prms.Add("@ISM_OZEL_ALAN_3", entity.ISM_OZEL_ALAN_3);
                        prms.Add("@ISM_OZEL_ALAN_4", entity.ISM_OZEL_ALAN_4);
                        prms.Add("@ISM_OZEL_ALAN_5", entity.ISM_OZEL_ALAN_5);
                        prms.Add("@ISM_OZEL_ALAN_6", entity.ISM_OZEL_ALAN_6);
                        prms.Add("@ISM_OZEL_ALAN_7", entity.ISM_OZEL_ALAN_7);
                        prms.Add("@ISM_OZEL_ALAN_8", entity.ISM_OZEL_ALAN_8);
                        prms.Add("@ISM_OZEL_ALAN_9", entity.ISM_OZEL_ALAN_9);
                        prms.Add("@ISM_OZEL_ALAN_10", entity.ISM_OZEL_ALAN_10);
                        prms.Add("@ISM_OZEL_ALAN_11_KOD_ID", entity.ISM_OZEL_ALAN_11_KOD_ID);
                        prms.Add("@ISM_OZEL_ALAN_12_KOD_ID", entity.ISM_OZEL_ALAN_12_KOD_ID);
                        prms.Add("@ISM_OZEL_ALAN_13_KOD_ID", entity.ISM_OZEL_ALAN_13_KOD_ID);
                        prms.Add("@ISM_OZEL_ALAN_14_KOD_ID", entity.ISM_OZEL_ALAN_14_KOD_ID);
                        prms.Add("@ISM_OZEL_ALAN_15_KOD_ID", entity.ISM_OZEL_ALAN_15_KOD_ID);
                        prms.Add("@ISM_OZEL_ALAN_16", entity.ISM_OZEL_ALAN_16);
                        prms.Add("@ISM_OZEL_ALAN_17", entity.ISM_OZEL_ALAN_17);
                        prms.Add("@ISM_OZEL_ALAN_18", entity.ISM_OZEL_ALAN_18);
                        prms.Add("@ISM_OZEL_ALAN_19", entity.ISM_OZEL_ALAN_19);
                        prms.Add("@ISM_OZEL_ALAN_20", entity.ISM_OZEL_ALAN_20);
                        klas.cmd(query,prms.PARAMS);


                        //if (logEntity != null)
                        //{
                        //    if (logEntity.ISL_ISEMRI_ID != -1)
                        //    {
                        //        IsEmriDurumDegisikligi(logEntity.ISL_DURUM_ESKI_KOD_ID, logEntity.ISL_DURUM_YENI_KOD_ID, logEntity.ISL_ACIKLAMA, logEntity.ISL_DURUM_ESKI_KOD_TANIM, logEntity.ISL_DURUM_YENI_KOD_TANIM, logEntity.ISL_ISEMRI_ID, logEntity.ISL_KULLANICI_ID);
                        //    }
                        //}

                        // tekli yapıya dönüldüğü için kapatıldı.
                        //// kontol listesi ekleme.
                        //for (int i = 0; i < entity.IsEmriKontrolList.Count; i++)
                        //{
                        //    IsEmriKontrolList kontEntity = entity.IsEmriKontrolList[i];
                        //    if (!kontEntity.DKN_SILINDI)
                        //        KontrolListKaydet(kontEntity);
                        //    else
                        //        klas.cmd("DELETE FROM orjin.TB_ISEMRI_KONTROLLIST WHERE TB_ISEMRI_KONTROLLIST_ID="  kontEntity.TB_ISEMRI_KONTROLLIST_ID);
                        //}
                        //// Personel ekleme.
                        //for (int i = 0; i < entity.IsEmriPersonelList.Count; i++)
                        //{
                        //    IsEmriPersonel perEntity = entity.IsEmriPersonelList[i];
                        //    if (!perEntity.IDK_SILINDI)
                        //    {
                        //        PersonelListKaydet(perEntity);
                        //    }
                        //    else
                        //    {
                        //        SqlCommand kmtMaliyet = new SqlCommand("UPDATE orjin.TB_ISEMRI SET ISM_MALIYET_PERSONEL = ISM_MALIYET_PERSONEL - Cast(@Maliyet AS FLOAT), ISM_MALIYET_TOPLAM =ISM_MALIYET_TOPLAM -  Cast(@Maliyet AS FLOAT) where TB_ISEMRI_ID = "  entity.TB_ISEMRI_ID, klas.baglan());
                        //        kmtMaliyet.Parameters.AddWithValue("Maliyet", perEntity.IDK_MALIYET);
                        //        kmtMaliyet.ExecuteNonQuery();

                        //        klas.cmd("DELETE FROM orjin.TB_ISEMRI_KAYNAK WHERE TB_ISEMRI_KAYNAK_ID="  perEntity.TB_ISEMRI_KAYNAK_ID);
                        //    }
                        //}

                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        [Route("api/IsEmriTarihce")]
        public List<IsEmriLog> GetIsEmriTarihceByID([FromUri] int ID)
        {
            string mdbn = klas.GetMasterDbName();
            parametreler.Clear();
            parametreler.Add(new Prm("@ID", ID));
            string sql =
                @"select L.* , K.KLL_TANIM AS ISL_KULLANICI from orjin.TB_ISEMRI_LOG L INNER JOIN {0}.orjin.TB_KULLANICI K ON L.ISL_KULLANICI_ID = K.TB_KULLANICI_ID where L.ISL_ISEMRI_ID =  @ID";
            sql = String.Format(sql, mdbn);
            List<IsEmriLog> listem = new List<IsEmriLog>();
            DataTable dt = klas.GetDataTable(sql, parametreler);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IsEmriLog entity = new IsEmriLog();
                entity.ISL_ACIKLAMA = Util.getFieldString(dt.Rows[i], "ISL_ACIKLAMA");
                entity.ISL_TARIH = Util.getFieldDateTime(dt.Rows[i], "ISL_TARIH");
                entity.ISL_ISLEM = Util.getFieldString(dt.Rows[i], "ISL_ISLEM");
                entity.ISL_SAAT = Util.getFieldString(dt.Rows[i], "ISL_SAAT");
                entity.ISL_KULLANICI = Util.getFieldString(dt.Rows[i], "ISL_KULLANICI");
                entity.TB_ISEMRI_LOG_ID = Util.getFieldInt(dt.Rows[i], "TB_ISEMRI_LOG_ID");
                entity.ISL_ISEMRI_ID = Util.getFieldInt(dt.Rows[i], "ISL_ISEMRI_ID");
                entity.ISL_KULLANICI_ID = Util.getFieldInt(dt.Rows[i], "ISL_KULLANICI_ID");

                listem.Add(entity);
            }

            return listem;
        }

        [Route("api/IsEmriKartiAcilis")]
        [HttpGet]
        public async Task<IsEmriKartAcilis> IsEmriKartData(int kulID, bool isNew)
        {
            var klas = new Util();
            var prms = new DynamicParameters();

            prms.Add("KUL_ID", kulID);
            prms.Add("KGRP1", klas.arizaTipleri);
            prms.Add("KGRP2", klas.bakimTipleri);
            prms.Add("KGRP3", klas.isEmriDurum);

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
                                SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP= @KGRP1 AND KOD_AKTIF = 1;
                                SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP= @KGRP2 AND KOD_AKTIF = 1;
                                SELECT * FROM orjin.TB_KOD WHERE KOD_GRUP= @KGRP3 AND KOD_AKTIF = 1;
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
                var result = await cnn.QueryMultipleAsync(query, prms);
                if (isNew)
                    entity.NEW_ISM_KOD = result.ReadFirstOrDefault<string>();
                entity.IsEmriTipList = result.Read<IsEmriTip>().ToList();
                entity.VarsayilanIsemriTipi = entity.IsEmriTipList.First(x => x.IMT_VARSAYILAN);
                entity.LokasyonList = result.Read<Lokasyon>().ToList();
                entity.AtolyeList = result.Read<Atolye>().ToList();
                entity.MasrafMerkeziList = result.Read<MasrafMerkezi>().ToList();
                entity.ArizaTipList = result.Read<Kod>().ToList();
                entity.BakimTipList = result.Read<Kod>().ToList();
                entity.DurumList = result.Read<Kod>().ToList();
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


        [Route("api/IsEmriKontrolListKaydet")]
        [HttpPost]
        public Bildirim KontrolListKaydet([FromBody] IsEmriKontrolList entity)
        {
            string plainText, rtfText;

            Bildirim bldr = new Bildirim();
            try
            {
                if(entity.DKN_ACIKLAMA.StartsWith(@"{\rtf"))
                {

                    System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox();
                    rtfText = entity.DKN_ACIKLAMA;
                    rtBox.Rtf = rtfText;
                    plainText = rtBox.Text;
                } 

                else { plainText = entity.DKN_ACIKLAMA; }

                if (entity.TB_ISEMRI_KONTROLLIST_ID < 1)
                {
                    // Yeni Ekle
                    string query = @"INSERT INTO orjin.TB_ISEMRI_KONTROLLIST
                               (DKN_ISEMRI_ID
                               ,DKN_SIRANO
                               ,DKN_YAPILDI
                               ,DKN_TANIM
                               ,DKN_OLUSTURAN_ID
                               ,DKN_OLUSTURMA_TARIH
                               ,DKN_MALIYET                              
                               ,DKN_YAPILDI_SAAT
                               ,DKN_YAPILDI_PERSONEL_ID
                               ,DKN_YAPILDI_MESAI_KOD_ID
                               ,DKN_YAPILDI_ATOLYE_ID
                               ,DKN_YAPILDI_SURE
                               ,DKN_ACIKLAMA
                               ,DKN_REF_ID)
                         VALUES (@DKN_ISEMRI_ID
                               ,@DKN_SIRANO
                               ,@DKN_YAPILDI
                               ,@DKN_TANIM
                               ,@DKN_OLUSTURAN_ID
                               ,@DKN_OLUSTURMA_TARIH
                               ,@DKN_MALIYET                              
                               ,@DKN_YAPILDI_SAAT
                               ,@DKN_YAPILDI_PERSONEL_ID
                               ,@DKN_YAPILDI_MESAI_KOD_ID
                               ,@DKN_YAPILDI_ATOLYE_ID
                               ,@DKN_YAPILDI_SURE
                               ,@DKN_ACIKLAMA
                               ,@DKN_REF_ID)";
                    prms.Clear();
                    prms.Add("DKN_ISEMRI_ID", entity.DKN_ISEMRI_ID);
                    prms.Add("DKN_SIRANO", entity.DKN_SIRANO);
                    prms.Add("DKN_YAPILDI", 0);
                    prms.Add("DKN_TANIM", entity.DKN_TANIM);
                    prms.Add("DKN_OLUSTURAN_ID", entity.DKN_OLUSTURAN_ID);
                    prms.Add("DKN_OLUSTURMA_TARIH", DateTime.Now);
                    prms.Add("DKN_MALIYET", 0);
                    prms.Add("DKN_YAPILDI_SAAT", 0);
                    prms.Add("DKN_YAPILDI_PERSONEL_ID", -1);
                    prms.Add("DKN_YAPILDI_MESAI_KOD_ID", -1);
                    prms.Add("DKN_YAPILDI_ATOLYE_ID", -1);
                    prms.Add("DKN_YAPILDI_SURE", 0);
                    prms.Add("DKN_ACIKLAMA", plainText);
                    prms.Add("DKN_REF_ID", entity.DKN_REF_ID);
                    klas.cmd(query, prms.PARAMS);

                    bldr.Id = Convert.ToInt32(klas.GetDataCell(
                        "select max(TB_ISEMRI_KONTROLLIST_ID) from orjin.TB_ISEMRI_KONTROLLIST", new List<Prm>()));
                    bldr.Aciklama = "İş emri kontrol listesi başarılı bir şekilde kaydedildi.";
                    bldr.MsgId = Bildirim.MSG_ISEMRI_KONTROL_LISTE_KAYIT_OK;
                    bldr.Durum = true;
                }
                else
                {
                    // Güncelle
                    string query = @"UPDATE orjin.TB_ISEMRI_KONTROLLIST SET
                                DKN_ISEMRI_ID = @DKN_ISEMRI_ID
                               ,DKN_SIRANO = @DKN_SIRANO
                               ,DKN_YAPILDI=@DKN_YAPILDI
                               ,DKN_TANIM = @DKN_TANIM
                               ,DKN_DEGISTIREN_ID = @DKN_DEGISTIREN_ID
                               ,DKN_DEGISTIRME_TARIH = @DKN_DEGISTIRME_TARIH 
                               ,DKN_ACIKLAMA = @DKN_ACIKLAMA
                               ,DKN_REF_ID = @DKN_REF_ID WHERE TB_ISEMRI_KONTROLLIST_ID = @TB_ISEMRI_KONTROLLIST_ID";
                    prms.Clear();
                    prms.Add("TB_ISEMRI_KONTROLLIST_ID", entity.TB_ISEMRI_KONTROLLIST_ID);
                    prms.Add("DKN_ISEMRI_ID", entity.DKN_ISEMRI_ID);
                    prms.Add("DKN_YAPILDI", entity.DKN_YAPILDI);
                    prms.Add("DKN_SIRANO", entity.DKN_SIRANO);
                    prms.Add("DKN_TANIM", entity.DKN_TANIM);
                    prms.Add("DKN_DEGISTIREN_ID", entity.DKN_OLUSTURAN_ID);
                    prms.Add("DKN_DEGISTIRME_TARIH", DateTime.Now);
                    prms.Add("DKN_ACIKLAMA", plainText);
                    prms.Add("DKN_REF_ID", entity.DKN_REF_ID);
                    klas.cmd(query, prms.PARAMS);
                    bldr.Aciklama = "İş emri kontrol listesi başarılı bir şekilde güncellendi.";
                    bldr.MsgId = Bildirim.MSG_ISEMRI_KONTROL_LISTE_GUNCELLE_OK;
                    bldr.Durum = true;
                }
            }
            catch (Exception e)
            {
                klas.kapat();
                bldr.Aciklama = String.Format(Localization.errorFormatted, e.Message);
                bldr.MsgId = Bildirim.MSG_ISLEM_HATA;
                bldr.HasExtra = true;
                bldr.Durum = false;
            }

            return bldr;
        }

        [Route("api/IsemriPersonelKaydet")]
        [HttpPost]
        public Bildirim PersonelListKaydet(IsEmriPersonel entity)
        {
            Bildirim bildirimEntity = new Bildirim();
            parametreler.Clear();
            parametreler.Add(new Prm("IDK_ISEMRI_ID", entity.IDK_ISEMRI_ID));
            parametreler.Add(new Prm("IDK_REF_ID", entity.IDK_REF_ID));
            try
            {
                klas.MasterBaglantisi = false;
                DataRow drPersonelVarmi =
                    klas.GetDataRow(
                        "SELECT * FROM orjin.TB_ISEMRI_KAYNAK WHERE IDK_REF_ID = @IDK_REF_ID  AND IDK_ISEMRI_ID=@IDK_ISEMRI_ID",
                        parametreler);
                if (entity.TB_ISEMRI_KAYNAK_ID < 1)
                {
                    #region Kaydet

                    string query = @"INSERT INTO orjin.TB_ISEMRI_KAYNAK
                                    (  IDK_ISEMRI_ID
                                      ,IDK_REF_ID   
                                      ,IDK_SURE
                                      ,IDK_SAAT_UCRETI
                                      ,IDK_MALIYET
                                      ,IDK_SOZLESME_ID
                                      ,IDK_FAZLA_MESAI_VAR
                                      ,IDK_FAZLA_MESAI_SURE
                                      ,IDK_FAZLA_MESAI_SAAT_UCRETI  ,IDK_VARDIYA                                   
                                      ,IDK_OLUSTURAN_ID
                                      ,IDK_OLUSTURMA_TARIH)
                                    VALUES (  @IDK_ISEMRI_ID
                                      ,@IDK_REF_ID   
                                      ,@IDK_SURE
                                      ,@IDK_SAAT_UCRETI
                                      ,@IDK_MALIYET
                                      ,-1
                                      ,0
                                      ,0
                                      ,0 ,@IDK_VARDIYA                                    
                                      ,@IDK_OLUSTURAN_ID
                                      ,@IDK_OLUSTURMA_TARIH)";
                    prms.Clear();
                    prms.Add("@IDK_ISEMRI_ID", entity.IDK_ISEMRI_ID);
                    prms.Add("@IDK_REF_ID", entity.IDK_REF_ID);
                    prms.Add("@IDK_SURE", entity.IDK_SURE);
                    prms.Add("@IDK_SAAT_UCRETI", entity.IDK_SAAT_UCRETI);
                    prms.Add("@IDK_MALIYET", entity.IDK_MALIYET);
                    prms.Add("@IDK_OLUSTURAN_ID", entity.IDK_OLUSTURAN_ID);
                    prms.Add("@IDK_VARDIYA", entity.IDK_VARDIYA);
                    prms.Add("@IDK_OLUSTURMA_TARIH", DateTime.Now);
                    klas.cmd(query, prms.PARAMS);

                    #endregion

                    string qu =
                        "UPDATE orjin.TB_ISEMRI SET ISM_MALIYET_PERSONEL = ISM_MALIYET_PERSONEL + Cast(@Maliyet AS FLOAT), ISM_MALIYET_TOPLAM =ISM_MALIYET_TOPLAM +  Cast(@Maliyet AS FLOAT) where TB_ISEMRI_ID = @IDK_ISEMRI_ID";
                    prms.Clear();
                    prms.Add("@Maliyet", entity.IDK_MALIYET);
                    prms.Add("@IDK_ISEMRI_ID", entity.IDK_ISEMRI_ID);
                    klas.cmd(qu, prms.PARAMS);
                    bildirimEntity.Id =
                        Convert.ToInt32(klas.GetDataCell("select max(TB_ISEMRI_KAYNAK_ID) from orjin.TB_ISEMRI_KAYNAK",
                            new List<Prm>()));
                    bildirimEntity.Aciklama = "İş emri personel kaydı başarılı bir şekilde gerçekleşti.";
                    bildirimEntity.MsgId = Bildirim.MSG_ISM_PERSONEL_KAYIT_OK;
                    bildirimEntity.Durum = true;
                    // Bildirim Gönder
                    parametreler.Clear();
                    parametreler.Add(new Prm("@IDK_ISEMRI_ID", entity.IDK_ISEMRI_ID));
                    string isEmriNo =
                        klas.GetDataCell(
                            "select TOP 1 ISM_ISEMRI_NO FROM orjin.TB_ISEMRI WHERE TB_ISEMRI_ID=@IDK_ISEMRI_ID",
                            parametreler);
                    klas.MasterBaglantisi = true;
                    parametreler.Clear();
                    parametreler.Add(new Prm("IDK_REF_ID", entity.IDK_REF_ID));
                    string deviceid =
                        klas.GetDataCell(
                            "select KLL_MOBILCIHAZ_ID from orjin.TB_KULLANICI WHERE KLL_MOBIL_BILDIRI = 1 and KLL_MOBIL_KULLANICI = 1 and KLL_PERSONEL_ID=@IDK_REF_ID",
                            parametreler);
                    if (deviceid != null && deviceid != "")
                    {
                        Util.SendNotificationToTopic(isEmriNo, Localization.YeniIsEmri,
                            String.Format(Localization.NoluIsemrineAtandiniz, isEmriNo), Util.isEmri,
                            new string[] { deviceid });
                    }

                    klas.MasterBaglantisi = false;
                }

                else
                {
                    #region Güncelle

                    parametreler.Clear();
                    parametreler.Add(new Prm("TB_ISEMRI_KAYNAK_ID", entity.TB_ISEMRI_KAYNAK_ID));
                    string query = @"UPDATE orjin.TB_ISEMRI_KAYNAK SET
                                      IDK_ISEMRI_ID = @IDK_ISEMRI_ID
                                      ,IDK_REF_ID   = @IDK_REF_ID   
                                      ,IDK_SURE     = @IDK_SURE
                                      ,IDK_SAAT_UCRETI = @IDK_SAAT_UCRETI
                                      ,IDK_MALIYET     = @IDK_MALIYET  
                                      ,IDK_VARDIYA     =@IDK_VARDIYA
                                      ,IDK_DEGISTIREN_ID = @IDK_DEGISTIREN_ID
                                      ,IDK_DEGISTIRME_TARIH = @IDK_DEGISTIRME_TARIH WHERE TB_ISEMRI_KAYNAK_ID=@TB_ISEMRI_KAYNAK_ID";
                    double eskiMaliyet = Convert.ToDouble(klas.GetDataCell(
                        "select coalesce(IDK_MALIYET,0) from orjin.TB_ISEMRI_KAYNAK WHERE TB_ISEMRI_KAYNAK_ID=@TB_ISEMRI_KAYNAK_ID",
                        parametreler));
                    prms.Clear();
                    prms.Add("@IDK_ISEMRI_ID", entity.IDK_ISEMRI_ID);
                    prms.Add("@IDK_REF_ID", entity.IDK_REF_ID);
                    prms.Add("@IDK_SURE", entity.IDK_SURE);
                    prms.Add("@IDK_SAAT_UCRETI", entity.IDK_SAAT_UCRETI);
                    prms.Add("@IDK_MALIYET", entity.IDK_MALIYET);
                    prms.Add("@IDK_DEGISTIREN_ID", entity.IDK_DEGISTIREN_ID);
                    prms.Add("@IDK_DEGISTIRME_TARIH", DateTime.Now);
                    prms.Add("@TB_ISEMRI_KAYNAK_ID", entity.TB_ISEMRI_KAYNAK_ID);
                    prms.Add("@IDK_VARDIYA", entity.IDK_VARDIYA);
                    klas.cmd(query, prms.PARAMS);

                    #endregion

                    string qu1 =
                        "UPDATE orjin.TB_ISEMRI SET ISM_MALIYET_PERSONEL = ISM_MALIYET_PERSONEL - Cast(@Maliyet AS FLOAT), ISM_MALIYET_TOPLAM =ISM_MALIYET_TOPLAM -  Cast(@Maliyet AS FLOAT) where TB_ISEMRI_ID = @IDK_ISEMRI_ID";
                    prms.Clear();
                    prms.Add("@Maliyet", eskiMaliyet);
                    prms.Add("@IDK_ISEMRI_ID", entity.IDK_ISEMRI_ID);
                    klas.cmd(qu1, prms.PARAMS);

                    string qu2 =
                        "UPDATE orjin.TB_ISEMRI SET ISM_MALIYET_PERSONEL = ISM_MALIYET_PERSONEL + Cast(@Maliyet AS FLOAT), ISM_MALIYET_TOPLAM =ISM_MALIYET_TOPLAM +  Cast(@Maliyet AS FLOAT) where TB_ISEMRI_ID = @IDK_ISEMRI_ID";
                    klas.cmd(qu2, prms.PARAMS);

                    bildirimEntity.Aciklama = "İş emri personel güncelleme başarılı bir şekilde gerçekleşti.";
                    bildirimEntity.MsgId = Bildirim.MSG_ISM_PERSONEL_GUNCELLE_OK;
                    bildirimEntity.Durum = true;
                    klas.kapat();
                }
            }
            catch (Exception e)
            {
                klas.kapat();
                bildirimEntity.Aciklama =
                    String.Format(Localization.errorFormatted, e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }

            return bildirimEntity;
        }

        [Route("api/IsemriMalzemeKaydet")]
        [HttpPost]
        public Bildirim MalzemeListKaydet(IsEmriMalzeme entity)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                if (entity.TB_ISEMRI_MLZ_ID < 1)
                {
                    #region Kaydet

                    string qu1 = @"INSERT INTO orjin.TB_ISEMRI_MLZ
                                                       (IDM_ISEMRI_ID
                                                       ,IDM_TARIH
                                                       ,IDM_SAAT
                                                       ,IDM_STOK_ID
                                                       ,IDM_DEPO_ID
                                                       ,IDM_BIRIM_KOD_ID
                                                       ,IDM_STOK_TIP_KOD_ID
                                                       ,IDM_STOK_DUS
                                                       ,IDM_STOK_TANIM
                                                       ,IDM_BIRIM_FIYAT
                                                       ,IDM_MIKTAR
                                                       ,IDM_TUTAR
                                                       ,IDM_OLUSTURAN_ID
                                                       ,IDM_OLUSTURMA_TARIH                                                      
                                                       ,IDM_REF_ID
                                                       ,IDM_STOK_KULLANIM_SEKLI
                                                       ,IDM_MALZEME_STOKTAN
                                                       ,IDM_ALTERNATIF_STOK_ID
                                                       ,IDM_MARKA_KOD_ID)
                                                 VALUES (@IDM_ISEMRI_ID
                                                       ,@IDM_TARIH
                                                       ,@IDM_SAAT
                                                       ,@IDM_STOK_ID
                                                       ,@IDM_DEPO_ID
                                                       ,@IDM_BIRIM_KOD_ID
                                                       ,@IDM_STOK_TIP_KOD_ID
                                                       ,@IDM_STOK_DUS
                                                       ,@IDM_STOK_TANIM
                                                       ,@IDM_BIRIM_FIYAT
                                                       ,@IDM_MIKTAR
                                                       ,@IDM_TUTAR
                                                       ,@IDM_OLUSTURAN_ID
                                                       ,@IDM_OLUSTURMA_TARIH                                                      
                                                       ,@IDM_REF_ID
                                                       ,@IDM_STOK_KULLANIM_SEKLI
                                                       ,@IDM_MALZEME_STOKTAN
                                                       ,-1
                                                       ,-1)";
                    prms.Clear();
                    prms.Add("@IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
                    prms.Add("@IDM_TARIH", entity.IDM_TARIH);
                    prms.Add("@IDM_SAAT", entity.IDM_SAAT);
                    prms.Add("@IDM_STOK_ID", entity.IDM_STOK_ID);
                    prms.Add("@IDM_DEPO_ID", entity.IDM_DEPO_ID);
                    prms.Add("@IDM_BIRIM_KOD_ID", entity.IDM_BIRIM_KOD_ID);
                    prms.Add("@IDM_STOK_TIP_KOD_ID", entity.IDM_STOK_TIP_KOD_ID);
                    prms.Add("@IDM_STOK_DUS", entity.IDM_STOK_DUS);
                    prms.Add("@IDM_STOK_TANIM", entity.IDM_STOK_TANIM);
                    prms.Add("@IDM_BIRIM_FIYAT", entity.IDM_BIRIM_FIYAT);
                    prms.Add("@IDM_MIKTAR", entity.IDM_MIKTAR);
                    prms.Add("@IDM_TUTAR", entity.IDM_TUTAR);
                    prms.Add("@IDM_OLUSTURAN_ID", entity.IDM_OLUSTURAN_ID);
                    prms.Add("@IDM_OLUSTURMA_TARIH", DateTime.Now);
                    prms.Add("@IDM_REF_ID", entity.IDM_REF_ID); // iş tanım veya periyodik bakım id
                    GenelListeController gn = new GenelListeController();
                    entity.IDM_STOK_KULLANIM_SEKLI = Convert.ToInt32(gn.ParametreDeger("320112").PRM_DEGER);
                    prms.Add("@IDM_STOK_KULLANIM_SEKLI",
                        entity.IDM_STOK_KULLANIM_SEKLI); // 1 ise iş emri açıkken düş 2 işe iş emri kapatılırken düş
                    prms.Add("@IDM_MALZEME_STOKTAN", entity.IDM_MALZEME_STOKTAN); // Düşsün
                    klas.cmd(qu1, prms.PARAMS);
                    string qu2 =
                        "UPDATE orjin.TB_ISEMRI SET ISM_MALIYET_MLZ = ISM_MALIYET_MLZ + Cast(@Maliyet AS FLOAT), ISM_MALIYET_TOPLAM =ISM_MALIYET_TOPLAM +  Cast(@Maliyet AS FLOAT) where TB_ISEMRI_ID = @IDM_ISEMRI_ID";
                    prms.Clear();
                    prms.Add("@Maliyet", entity.IDM_TUTAR);
                    prms.Add("@IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
                    klas.cmd(qu2, prms.PARAMS);
                    klas.kapat();
                    if (entity.IDM_STOK_DUS && entity.IDM_DEPO_ID > 0 && entity.IDM_STOK_KULLANIM_SEKLI == 1 &&
                        (entity.IDM_MALZEME_STOKTAN.Trim() == "Düşsün" ||
                         entity.IDM_MALZEME_STOKTAN.Trim() == "Sorsun"))
                    {
                        StokHareketIslemi(entity);
                    }

                    bildirimEntity.Aciklama = "İş emri malzeme kaydı başarılı bir şekilde gerçekleşti.";
                    bildirimEntity.MsgId = Bildirim.MSG_ISM_MALZEME_KAYIT_OK;
                    bildirimEntity.Durum = true;

                    #endregion
                }
                else
                {
                    #region Güncelle

                    parametreler.Clear();
                    parametreler.Add(new Prm("TB_ISEMRI_MLZ_ID", entity.TB_ISEMRI_MLZ_ID));
                    double eskiMaliyet = Convert.ToDouble(klas.GetDataCell(
                        "select coalesce(IDM_TUTAR,0) from orjin.TB_ISEMRI_MLZ WHERE TB_ISEMRI_MLZ_ID=@TB_ISEMRI_MLZ_ID",
                        parametreler));
                    string qu3 = @"UPDATE orjin.TB_ISEMRI_MLZ SET
                                                        IDM_ISEMRI_ID                     = @IDM_ISEMRI_ID
                                                       ,IDM_TARIH                         = @IDM_TARIH
                                                       ,IDM_SAAT                          = @IDM_SAAT
                                                       ,IDM_STOK_ID                       = @IDM_STOK_ID
                                                       ,IDM_DEPO_ID                       = @IDM_DEPO_ID
                                                       ,IDM_BIRIM_KOD_ID                  = @IDM_BIRIM_KOD_ID
                                                       ,IDM_STOK_TIP_KOD_ID               = @IDM_STOK_TIP_KOD_ID
                                                       ,IDM_STOK_DUS                      = @IDM_STOK_DUS
                                                       ,IDM_STOK_TANIM                    = @IDM_STOK_TANIM
                                                       ,IDM_BIRIM_FIYAT                   = @IDM_BIRIM_FIYAT
                                                       ,IDM_MIKTAR                        = @IDM_MIKTAR
                                                       ,IDM_TUTAR                         = @IDM_TUTAR
                                                       ,IDM_DEGISTIREN_ID                 = @IDM_DEGISTIREN_ID
                                                       ,IDM_DEGISTIRME_TARIH              = @IDM_DEGISTIRME_TARIH        
                                                       ,IDM_REF_ID                        = @IDM_REF_ID
                                                       ,IDM_STOK_KULLANIM_SEKLI           = @IDM_STOK_KULLANIM_SEKLI
                                                       ,IDM_MALZEME_STOKTAN               = @IDM_MALZEME_STOKTAN WHERE TB_ISEMRI_MLZ_ID = @TB_ISEMRI_MLZ_ID";
                    prms.Clear();

                    prms.Add("@IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
                    prms.Add("@IDM_TARIH", entity.IDM_TARIH);
                    prms.Add("@IDM_SAAT", entity.IDM_SAAT);
                    prms.Add("@IDM_STOK_ID", entity.IDM_STOK_ID);
                    prms.Add("@IDM_DEPO_ID", entity.IDM_DEPO_ID);
                    prms.Add("@IDM_BIRIM_KOD_ID", entity.IDM_BIRIM_KOD_ID);
                    prms.Add("@IDM_STOK_TIP_KOD_ID", entity.IDM_STOK_TIP_KOD_ID);
                    prms.Add("@IDM_STOK_DUS", entity.IDM_STOK_DUS);
                    prms.Add("@IDM_STOK_TANIM", entity.IDM_STOK_TANIM);
                    prms.Add("@IDM_BIRIM_FIYAT", entity.IDM_BIRIM_FIYAT);
                    prms.Add("@IDM_MIKTAR", entity.IDM_MIKTAR);
                    prms.Add("@IDM_TUTAR", entity.IDM_TUTAR);
                    prms.Add("@IDM_DEGISTIREN_ID", entity.IDM_DEGISTIREN_ID);
                    prms.Add("@IDM_DEGISTIRME_TARIH", DateTime.Now);
                    prms.Add("@IDM_REF_ID", entity.IDM_REF_ID);
                    prms.Add("@TB_ISEMRI_MLZ_ID", entity.TB_ISEMRI_MLZ_ID);

                    GenelListeController gn = new GenelListeController();
                    entity.IDM_STOK_KULLANIM_SEKLI = Convert.ToInt32(gn.ParametreDeger("320112").PRM_DEGER);

                    prms.Add("@IDM_STOK_KULLANIM_SEKLI", entity.IDM_STOK_KULLANIM_SEKLI);
                    prms.Add("@IDM_MALZEME_STOKTAN", entity.IDM_MALZEME_STOKTAN);
                    klas.cmd(qu3, prms.PARAMS);

                    #endregion

                    if (entity.IDM_STOK_DUS && entity.IDM_DEPO_ID > 0 && entity.IDM_STOK_KULLANIM_SEKLI == 1 &&
                        (entity.IDM_MALZEME_STOKTAN == "Düşşün"))
                    {
                        StokHareketIslemi(entity);
                    }

                    string qu4 =
                        "UPDATE orjin.TB_ISEMRI SET ISM_MALIYET_MLZ = ISM_MALIYET_MLZ - Cast(@Maliyet AS FLOAT), ISM_MALIYET_TOPLAM =ISM_MALIYET_TOPLAM -  Cast(@Maliyet AS FLOAT) where TB_ISEMRI_ID = @IDM_ISEMRI_ID";
                    prms.Clear();
                    prms.Add("@Maliyet", eskiMaliyet);
                    prms.Add("@IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
                    klas.cmd(qu4, prms.PARAMS);
                    string qu5 =
                        "UPDATE orjin.TB_ISEMRI SET ISM_MALIYET_MLZ = ISM_MALIYET_MLZ + Cast(@Maliyet AS FLOAT), ISM_MALIYET_TOPLAM =ISM_MALIYET_TOPLAM +  Cast(@Maliyet AS FLOAT) where TB_ISEMRI_ID = @IDM_ISEMRI_ID";
                    prms.Clear();
                    prms.Add("@Maliyet", entity.IDM_TUTAR);
                    prms.Add("@IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
                    klas.cmd(qu5, prms.PARAMS);
                    klas.kapat();

                    bildirimEntity.Aciklama = "İş emri malzeme güncelleme başarılı bir şekilde gerçekleşti.";
                    bildirimEntity.MsgId = Bildirim.MSG_ISM_MALZEME_GUNCELLE_OK;
                    bildirimEntity.Durum = true;
                }
            }
            catch (Exception e)
            {
                klas.kapat();
                bildirimEntity.Aciklama =
                    String.Format(Localization.errorFormatted, e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }

            return bildirimEntity;
        }

        [Route("api/IsEmriDurusKaydet")]
        [HttpPost]
        public Bildirim IsEmriDurusKaydet([FromBody] IsEmriDurus entity)
        {
            Bildirim bldr = new Bildirim();
            try
            {
                if (entity.TB_MAKINE_DURUS_ID < 1)
                {
                    #region Yeni Ekle

                    string query = @"INSERT INTO orjin.TB_MAKINE_DURUS
                                   (MKD_ISEMRI_ID
                                   ,MKD_MAKINE_ID
                                   ,MKD_BASLAMA_TARIH
                                   ,MKD_BASLAMA_SAAT
                                   ,MKD_BITIS_TARIH
                                   ,MKD_BITIS_SAAT
                                   ,MKD_SURE
                                   ,MKD_SAAT_MALIYET
                                   ,MKD_TOPLAM_MALIYET
                                   ,MKD_NEDEN_KOD_ID          
                                   ,MKD_PLANLI
                                   ,MKD_OLUSTURAN_ID
                                   ,MKD_OLUSTURMA_TARIH
                                   ,MKD_PROJE_ID       
                                   ,MKD_LOKASYON_ID    
                                   ,MKD_ACIKLAMA)
                             VALUES
		                           (@MKD_ISEMRI_ID
                                   ,@MKD_MAKINE_ID
                                   ,@MKD_BASLAMA_TARIH
                                   ,@MKD_BASLAMA_SAAT
                                   ,@MKD_BITIS_TARIH
                                   ,@MKD_BITIS_SAAT
                                   ,@MKD_SURE
                                   ,@MKD_SAAT_MALIYET
                                   ,@MKD_TOPLAM_MALIYET
                                   ,@MKD_NEDEN_KOD_ID          
                                   ,@MKD_PLANLI
                                   ,@MKD_OLUSTURAN_ID
                                   ,@MKD_OLUSTURMA_TARIH
                                   ,@MKD_PROJE_ID       
                                   ,@MKD_LOKASYON_ID    
                                   ,@MKD_ACIKLAMA)";
                    prms.Clear();

                    prms.Add("MKD_ISEMRI_ID", entity.MKD_ISEMRI_ID);
                    prms.Add("MKD_MAKINE_ID", entity.MKD_MAKINE_ID);
                    prms.Add("MKD_BASLAMA_TARIH", entity.MKD_BASLAMA_TARIH);
                    prms.Add("MKD_BASLAMA_SAAT", entity.MKD_BASLAMA_SAAT);
                    prms.Add("MKD_BITIS_TARIH", entity.MKD_BITIS_TARIH);
                    prms.Add("MKD_BITIS_SAAT", entity.MKD_BITIS_SAAT);
                    prms.Add("MKD_SURE", entity.MKD_SURE);
                    prms.Add("MKD_SAAT_MALIYET", entity.MKD_SAAT_MALIYET);
                    prms.Add("MKD_TOPLAM_MALIYET", entity.MKD_TOPLAM_MALIYET);
                    prms.Add("MKD_NEDEN_KOD_ID", entity.MKD_NEDEN_KOD_ID);
                    prms.Add("MKD_PLANLI", entity.MKD_PLANLI);
                    prms.Add("MKD_OLUSTURAN_ID", entity.MKD_OLUSTURAN_ID);
                    prms.Add("MKD_OLUSTURMA_TARIH", entity.MKD_OLUSTURMA_TARIH);
                    prms.Add("MKD_PROJE_ID", entity.MKD_PROJE_ID);
                    prms.Add("MKD_LOKASYON_ID", entity.MKD_LOKASYON_ID);
                    prms.Add("MKD_ACIKLAMA", entity.MKD_ACIKLAMA);
                    klas.cmd(query, prms.PARAMS);
                    klas.kapat();

                    #endregion

                    bldr.Aciklama = "İş emri duruş başarılı bir şekilde kaydedildi.";
                    bldr.MsgId = Bildirim.MSG_ISM_DURUS_KAYIT_OK;
                    bldr.Durum = true;
                }
                else
                {
                    #region Güncelle

                    string query = @"UPDATE orjin.TB_MAKINE_DURUS SET
                                    MKD_ISEMRI_ID                   =@MKD_ISEMRI_ID
                                   ,MKD_MAKINE_ID                   =@MKD_MAKINE_ID
                                   ,MKD_BASLAMA_TARIH               =@MKD_BASLAMA_TARIH
                                   ,MKD_BASLAMA_SAAT                =@MKD_BASLAMA_SAAT
                                   ,MKD_BITIS_TARIH                 =@MKD_BITIS_TARIH
                                   ,MKD_BITIS_SAAT                  =@MKD_BITIS_SAAT
                                   ,MKD_SURE                        =@MKD_SURE
                                   ,MKD_SAAT_MALIYET                =@MKD_SAAT_MALIYET
                                   ,MKD_TOPLAM_MALIYET              =@MKD_TOPLAM_MALIYET
                                   ,MKD_NEDEN_KOD_ID                =@MKD_NEDEN_KOD_ID      
                                   ,MKD_PLANLI                      =@MKD_PLANLI
                                   ,MKD_DEGISTIREN_ID               =@MKD_DEGISTIREN_ID
                                   ,MKD_DEGISTIRME_TARIH            =@MKD_DEGISTIRME_TARIH
                                   ,MKD_PROJE_ID                    =@MKD_PROJE_ID       
                                   ,MKD_LOKASYON_ID                 =@MKD_LOKASYON_ID 
                                   ,MKD_ACIKLAMA                    =@MKD_ACIKLAMA WHERE TB_MAKINE_DURUS_ID = @TB_MAKINE_DURUS_ID";
                    prms.Clear();
                    prms.Add("MKD_ISEMRI_ID", entity.MKD_ISEMRI_ID);
                    prms.Add("MKD_MAKINE_ID", entity.MKD_MAKINE_ID);
                    prms.Add("MKD_BASLAMA_TARIH", entity.MKD_BASLAMA_TARIH);
                    prms.Add("MKD_BASLAMA_SAAT", entity.MKD_BASLAMA_SAAT);
                    prms.Add("MKD_BITIS_TARIH", entity.MKD_BITIS_TARIH);
                    prms.Add("MKD_BITIS_SAAT", entity.MKD_BITIS_SAAT);
                    prms.Add("MKD_SURE", entity.MKD_SURE);
                    prms.Add("MKD_SAAT_MALIYET", entity.MKD_SAAT_MALIYET);
                    prms.Add("MKD_TOPLAM_MALIYET", entity.MKD_TOPLAM_MALIYET);
                    prms.Add("MKD_NEDEN_KOD_ID", entity.MKD_NEDEN_KOD_ID);
                    prms.Add("MKD_PLANLI", entity.MKD_PLANLI);
                    prms.Add("MKD_DEGISTIREN_ID", entity.MKD_DEGISTIREN_ID);
                    prms.Add("MKD_DEGISTIRME_TARIH", entity.MKD_DEGISTIRME_TARIH);
                    prms.Add("MKD_PROJE_ID", entity.MKD_PROJE_ID);
                    prms.Add("MKD_LOKASYON_ID", entity.MKD_LOKASYON_ID);
                    prms.Add("TB_MAKINE_DURUS_ID", entity.TB_MAKINE_DURUS_ID);
                    prms.Add("MKD_ACIKLAMA", entity.MKD_ACIKLAMA);
                    klas.cmd(query, prms.PARAMS);
                    klas.kapat();

                    bldr.Aciklama = "İş emri duruş başarılı bir şekilde güncellendi.";
                    bldr.MsgId = Bildirim.MSG_ISM_DURUS_GUNCELLE_OK;
                    bldr.Durum = true;
                }

                #endregion
            }
            catch (Exception e)
            {
                bldr.Aciklama = String.Format(Localization.errorFormatted, e.Message);
                bldr.MsgId = Bildirim.MSG_ISLEM_HATA;
                bldr.HasExtra = true;
                bldr.Durum = false;
            }

            return bldr;
        }

        private void StokHareketIslemi(IsEmriMalzeme entity)
        {
            try
            {
                prms.Clear();
                prms.Add("IDM_STOK_ID", entity.IDM_STOK_ID);
                prms.Add("IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
                int deger = Convert.ToInt32(klas.GetDataCell(
                    "select count(*) from orjin.TB_STOK_HRK WHERE SHR_REF_GRUP='ISEMRI_MLZ' AND SHR_STOK_ID=@IDM_STOK_ID  and SHR_REF_ID=@IDM_ISEMRI_ID",
                    prms.PARAMS));
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
                                       ,@SHR_BIRIM_KOD_ID
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
                    prms.Add("IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
                    string ISM_NO =
                        klas.GetDataCell(
                            "SELECT TOP 1 ISM_ISEMRI_NO FROM orjin.TB_ISEMRI WHERE TB_ISEMRI_ID = @IDM_ISEMRI_ID",
                            prms.PARAMS);
                    prms.Clear();

                    prms.Add("@SHR_STOK_ID", entity.IDM_STOK_ID);
                    prms.Add("@SHR_BIRIM_KOD_ID", entity.IDM_BIRIM_KOD_ID);
                    prms.Add("@SHR_DEPO_ID", entity.IDM_DEPO_ID);
                    prms.Add("@SHR_TARIH", entity.IDM_TARIH);
                    prms.Add("@SHR_SAAT", entity.IDM_SAAT);
                    prms.Add("@SHR_MIKTAR", entity.IDM_MIKTAR);
                    prms.Add("@SHR_ANA_BIRIM_MIKTAR", entity.IDM_MIKTAR);
                    prms.Add("@SHR_BIRIM_FIYAT", entity.IDM_BIRIM_FIYAT);
                    prms.Add("@SHR_ARA_TOPLAM", entity.IDM_TUTAR);
                    prms.Add("@SHR_TOPLAM", entity.IDM_TUTAR);
                    prms.Add("@SHR_HRK_ACIKLAMA", ISM_NO + " nolu iş emri - malzeme kullanımı");
                    prms.Add("@SHR_ACIKLAMA", ISM_NO + " nolu iş emri - malzeme kullanımı");
                    prms.Add("@SHR_GC", "C");
                    prms.Add("@SHR_REF_ID", entity.IDM_ISEMRI_ID);
                    prms.Add("@SHR_REF_GRUP", "ISEMRI_MLZ");
                    prms.Add("@SHR_OLUSTURAN_ID", entity.IDM_OLUSTURAN_ID);
                    prms.Add("@SHR_OLUSTURMA_TARIH", DateTime.Now);
                    klas.cmd(query, prms.PARAMS);

                    // depo stok güncelleniyor            
                    string qu1 =
                        "UPDATE orjin.TB_DEPO_STOK SET DPS_CIKAN_MIKTAR = DPS_CIKAN_MIKTAR + Cast(@Miktar AS FLOAT), DPS_KULLANILABILIR_MIKTAR = DPS_KULLANILABILIR_MIKTAR - Cast(@Miktar AS FLOAT), DPS_MIKTAR = DPS_MIKTAR - Cast(@Miktar AS FLOAT)  WHERE DPS_DEPO_ID = @IDM_DEPO_ID  AND DPS_STOK_ID = @IDM_STOK_ID";
                    prms.Clear();
                    prms.Add("@Miktar", entity.IDM_MIKTAR);
                    prms.Add("@IDM_DEPO_ID", entity.IDM_DEPO_ID);
                    prms.Add("@IDM_STOK_ID", entity.IDM_STOK_ID);
                    klas.cmd(qu1, prms.PARAMS);
                    // stok güncelleniyor.
                    string qu2 =
                        "UPDATE orjin.TB_STOK SET STK_CIKAN_MIKTAR = STK_CIKAN_MIKTAR + Cast(@Miktar AS FLOAT), STK_MIKTAR = STK_MIKTAR - Cast(@Miktar AS FLOAT)  WHERE TB_STOK_ID = @IDM_STOK_ID";
                    prms.Clear();
                    prms.Add("@Miktar", entity.IDM_MIKTAR);
                    prms.Add("@IDM_STOK_ID", entity.IDM_STOK_ID);
                    klas.cmd(qu2, prms.PARAMS);
                    klas.kapat();
                }
                else
                {
                    prms.Clear();
                    prms.Add("IDM_STOK_ID", entity.IDM_STOK_ID);
                    prms.Add("IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
                    double eskiMiktar = Convert.ToDouble(klas.GetDataCell(
                        "select coalesce(SHR_MIKTAR,0) from orjin.TB_STOK_HRK WHERE SHR_REF_GRUP='ISEMRI_MLZ' AND SHR_STOK_ID=@IDM_STOK_ID and SHR_REF_ID = @IDM_ISEMRI_ID",
                        prms.PARAMS));
                    double yeniMiktar = entity.IDM_MIKTAR - eskiMiktar;
                    //hareket kaydı
                    string query = @"UPDATE orjin.TB_STOK_HRK SET
                                SHR_STOK_ID                            = @SHR_STOK_ID
                               ,SHR_BIRIM_KOD_ID                       = @SHR_BIRIM_KOD_ID
                               ,SHR_DEPO_ID                            = @SHR_DEPO_ID
                               ,SHR_TARIH                              = @SHR_TARIH
                               ,SHR_SAAT                               = @SHR_SAAT
                               ,SHR_MIKTAR                             = @SHR_MIKTAR
                               ,SHR_ANA_BIRIM_MIKTAR                   = @SHR_ANA_BIRIM_MIKTAR
                               ,SHR_BIRIM_FIYAT                        = @SHR_BIRIM_FIYAT
                               ,SHR_ARA_TOPLAM                         = @SHR_ARA_TOPLAM
                               ,SHR_TOPLAM                             = @SHR_TOPLAM
                               ,SHR_HRK_ACIKLAMA                       = (SELECT TOP 1 ISM_ISEMRI_NO FROM orjin.TB_ISEMRI WHERE TB_ISEMRI_ID =@SHR_REF_ID) + ' nolu iş emri - malzeme kullanımı' 
                               ,SHR_ACIKLAMA                           = (SELECT TOP 1 ISM_ISEMRI_NO FROM orjin.TB_ISEMRI WHERE TB_ISEMRI_ID =@SHR_REF_ID) + ' nolu iş emri - malzeme kullanımı'
                               ,SHR_GC                                 = @SHR_GC
                               ,SHR_REF_ID                             = @SHR_REF_ID
                               ,SHR_REF_GRUP                           = @SHR_REF_GRUP
                               ,SHR_DEGISTIREN_ID                      = @SHR_DEGISTIREN_ID
                               ,SHR_DEGISTIRME_TARIH                   = @SHR_DEGISTIRME_TARIH where SHR_REF_GRUP='ISEMRI_MLZ' AND SHR_STOK_ID = @SHR_STOK_ID and SHR_REF_ID=@SHR_REF_ID";

                    prms.Clear();
                    prms.Add("@SHR_STOK_ID", entity.IDM_STOK_ID);
                    prms.Add("@SHR_BIRIM_KOD_ID", entity.IDM_BIRIM_KOD_ID);
                    prms.Add("@SHR_DEPO_ID", entity.IDM_DEPO_ID);
                    prms.Add("@SHR_TARIH", entity.IDM_TARIH);
                    prms.Add("@SHR_SAAT", entity.IDM_SAAT);
                    prms.Add("@SHR_MIKTAR", entity.IDM_MIKTAR);
                    prms.Add("@SHR_ANA_BIRIM_MIKTAR", entity.IDM_MIKTAR);
                    prms.Add("@SHR_BIRIM_FIYAT", entity.IDM_BIRIM_FIYAT);
                    prms.Add("@SHR_ARA_TOPLAM", entity.IDM_TUTAR);
                    prms.Add("@SHR_TOPLAM", entity.IDM_TUTAR);
                    prms.Add("@SHR_GC", "C");
                    prms.Add("@SHR_REF_ID", entity.IDM_ISEMRI_ID);
                    prms.Add("@SHR_REF_GRUP", "ISEMRI_MLZ");
                    prms.Add("@SHR_DEGISTIREN_ID", entity.IDM_DEGISTIREN_ID);
                    prms.Add("@SHR_DEGISTIRME_TARIH", DateTime.Now);
                    klas.cmd(query, prms.PARAMS);
                    // depo stok güncelleniyor                   
                    string qu1 =
                        "UPDATE orjin.TB_DEPO_STOK SET DPS_CIKAN_MIKTAR = DPS_CIKAN_MIKTAR + Cast(@Miktar AS FLOAT), DPS_KULLANILABILIR_MIKTAR = DPS_KULLANILABILIR_MIKTAR - Cast(@Miktar AS FLOAT), DPS_MIKTAR = DPS_MIKTAR - Cast(@Miktar AS FLOAT)  WHERE DPS_DEPO_ID = @IDM_DEPO_ID  AND DPS_STOK_ID = @IDM_STOK_ID";
                    prms.Clear();
                    prms.Add("@Miktar", yeniMiktar);
                    prms.Add("@IDM_DEPO_ID", entity.IDM_DEPO_ID);
                    prms.Add("@IDM_STOK_ID", entity.IDM_STOK_ID);
                    klas.cmd(qu1, prms.PARAMS);

                    // stok güncelleniyor.
                    string qu2 =
                        "UPDATE orjin.TB_STOK SET STK_CIKAN_MIKTAR = STK_CIKAN_MIKTAR + Cast(@Miktar AS FLOAT), STK_MIKTAR = STK_MIKTAR - Cast(@Miktar AS FLOAT)  WHERE TB_STOK_ID = @IDM_STOK_ID";
                    prms.Clear();
                    prms.Add("@Miktar", yeniMiktar);
                    prms.Add("@IDM_STOK_ID", entity.IDM_STOK_ID);
                    klas.cmd(qu2, prms.PARAMS);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [Route("api/IsEmriKontrolList")]
        public List<IsEmriKontrolList> GetIsEmriKontrolList([FromUri] int isemriID)
        {
            string rtfText, plainText;

            prms.Clear();
            prms.Add("ISM_ID", isemriID);
            string sql = "select * from orjin.TB_ISEMRI_KONTROLLIST where DKN_ISEMRI_ID = @ISM_ID";
            List<IsEmriKontrolList> listem = new List<IsEmriKontrolList>();
            DataTable dt = klas.GetDataTable(sql, prms.PARAMS);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IsEmriKontrolList entity = new IsEmriKontrolList();
                entity.DKN_SIRANO = dt.Rows[i]["DKN_SIRANO"] != DBNull.Value ? dt.Rows[i]["DKN_SIRANO"].ToString() : "";
                entity.DKN_TANIM = dt.Rows[i]["DKN_TANIM"] != DBNull.Value ? dt.Rows[i]["DKN_TANIM"].ToString() : "";

                if (dt.Rows[i]["DKN_ACIKLAMA"] != DBNull.Value && dt.Rows[i]["DKN_ACIKLAMA"].ToString().StartsWith(@"{\rtf"))
                {

                    System.Windows.Forms.RichTextBox rtBox = new System.Windows.Forms.RichTextBox();
                    rtfText = dt.Rows[i]["DKN_ACIKLAMA"].ToString();
                    rtBox.Rtf = rtfText;
                    plainText = rtBox.Text;
                    entity.DKN_ACIKLAMA = plainText;
                }
                else { entity.DKN_ACIKLAMA = dt.Rows[i]["DKN_ACIKLAMA"] != DBNull.Value ? dt.Rows[i]["DKN_ACIKLAMA"].ToString() : ""; }
                entity.TB_ISEMRI_KONTROLLIST_ID = Convert.ToInt32(dt.Rows[i]["TB_ISEMRI_KONTROLLIST_ID"]);
                entity.DKN_ISEMRI_ID = Convert.ToInt32(dt.Rows[i]["DKN_ISEMRI_ID"]);
                entity.DKN_YAPILDI = Convert.ToBoolean(dt.Rows[i]["DKN_YAPILDI"]);
                entity.DKN_RESIM_ID = Convert.ToInt32(dt.Rows[i]["DKN_RESIM_ID"] != DBNull.Value ? dt.Rows[i]["DKN_RESIM_ID"] : -1);

                listem.Add(entity);
            }

            return listem;
        }

        [Route("api/IsEmriPersonelList")]
        public List<IsEmriPersonel> GetIsEmriPersonelList([FromUri] int isemriID)
        {
            prms.Clear();
            prms.Add("ISM_ID", isemriID);
            string sql =
                "select * ,(select COALESCE(TB_RESIM_ID,(select top 1 COALESCE(TB_RESIM_ID,-1) from orjin.TB_RESIM where  RSM_REF_GRUP = 'PERSONEL' and RSM_REF_ID = IDK_REF_ID)) from orjin.TB_RESIM where RSM_VARSAYILAN= 1 AND RSM_REF_GRUP = 'PERSONEL' and RSM_REF_ID = IDK_REF_ID) IDK_RESIM_ID  from orjin.VW_ISEMRI_KAYNAK INNER JOIN orjin.TB_PERSONEL ON TB_PERSONEL_ID = IDK_REF_ID where IDK_ISEMRI_ID=@ISM_ID";
            List<IsEmriPersonel> listem = new List<IsEmriPersonel>();
            DataTable dt = klas.GetDataTable(sql, prms.PARAMS);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IsEmriPersonel entity = new IsEmriPersonel();
                entity.IDK_PERSONEL_KOD = dt.Rows[i]["PRS_PERSONEL_KOD"] != DBNull.Value
                    ? dt.Rows[i]["PRS_PERSONEL_KOD"].ToString()
                    : "";
                entity.IDK_ISIM = dt.Rows[i]["PRS_ISIM"] != DBNull.Value ? dt.Rows[i]["PRS_ISIM"].ToString() : "";
                entity.TB_ISEMRI_KAYNAK_ID = Util.getFieldInt(dt.Rows[i], "TB_ISEMRI_KAYNAK_ID");
                entity.IDK_ISEMRI_ID = Util.getFieldInt(dt.Rows[i], "IDK_ISEMRI_ID");
                entity.IDK_REF_ID = Util.getFieldInt(dt.Rows[i], "IDK_REF_ID");
                entity.IDK_RESIM_ID = Util.getFieldInt(dt.Rows[i], "IDK_RESIM_ID");
                entity.IDK_SURE = Util.getFieldDouble(dt.Rows[i], "IDK_SURE");
                entity.IDK_SAAT_UCRETI = Util.getFieldDouble(dt.Rows[i], "IDK_SAAT_UCRETI");
                entity.IDK_MALIYET = Util.getFieldDouble(dt.Rows[i], "IDK_MALIYET");
                entity.IDK_VARDIYA = Util.getFieldInt(dt.Rows[i], "IDK_VARDIYA");
                entity.IDK_VARDIYA_TANIM = Util.getFieldString(dt.Rows[i], "IDK_VARDIYA_TANIM");
                //entity.IDK_RESIM_ID =       Convert.ToInt32(klas.GetDataCell("select COALESCE(TB_RESIM_ID,-1) from orjin.TB_RESIM where RSM_VARSAYILAN= 1 AND RSM_REF_GRUP = 'PERSONEL' and RSM_REF_ID = "  entity.TB_ISEMRI_KAYNAK_ID));


                listem.Add(entity);
            }

            return listem;
        }

        [Route("api/IsEmriMalzemeList")]
        [HttpGet]
        public List<IsEmriMalzeme> IsEmriMalzemeList([FromUri] int isemriID)
        {
            prms.Clear();
            prms.Add("ISM_ID", isemriID);
            string sql =
                "select *,(select top 1 STK_KOD from orjin.TB_STOK where TB_STOK_ID = IDM_STOK_ID) as IDM_STOK_KOD from orjin.VW_ISEMRI_MLZ  where IDM_ISEMRI_ID=@ISM_ID";
            List<IsEmriMalzeme> listem = new List<IsEmriMalzeme>();
            DataTable dt = klas.GetDataTable(sql, prms.PARAMS);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                IsEmriMalzeme entity = new IsEmriMalzeme();
                entity.IDM_STOK_KOD = Util.getFieldString(dt.Rows[i], "IDM_STOK_KOD");
                entity.IDM_STOK_TANIM = Util.getFieldString(dt.Rows[i], "IDM_STOK_TANIM");
                entity.TB_ISEMRI_MLZ_ID = Util.getFieldInt(dt.Rows[i], "TB_ISEMRI_MLZ_ID");
                entity.IDM_ISEMRI_ID = Util.getFieldInt(dt.Rows[i], "IDM_ISEMRI_ID");
                entity.IDM_STOK_ID = Util.getFieldInt(dt.Rows[i], "IDM_STOK_ID");
                entity.IDM_DEPO_ID = Util.getFieldInt(dt.Rows[i], "IDM_DEPO_ID");
                entity.IDM_BIRIM_KOD_ID = Util.getFieldInt(dt.Rows[i], "IDM_BIRIM_KOD_ID");
                entity.IDM_STOK_TIP_KOD_ID = Util.getFieldInt(dt.Rows[i], "IDM_STOK_TIP_KOD_ID");
                entity.IDM_BIRIM_FIYAT = Util.getFieldDouble(dt.Rows[i], "IDM_BIRIM_FIYAT");
                entity.IDM_TUTAR = Util.getFieldDouble(dt.Rows[i], "IDM_TUTAR");
                entity.IDM_MIKTAR = Util.getFieldDouble(dt.Rows[i], "IDM_MIKTAR");
                entity.IDM_STOK_DUS = Util.getFieldBool(dt.Rows[i], "IDM_STOK_DUS");
                entity.IDM_DEPO = Util.getFieldString(dt.Rows[i], "IDM_DEPO");
                entity.IDM_TARIH = Util.getFieldDateTime(dt.Rows[i], "IDM_TARIH");
                entity.IDM_SAAT = Util.getFieldString(dt.Rows[i], "IDM_SAAT");
                entity.IDM_MALZEME_STOKTAN = Util.getFieldString(dt.Rows[i], "IDM_MALZEME_STOKTAN");


                entity.IDM_STOK_KULLANIM_SEKLI = Util.getFieldInt(dt.Rows[i], "IDM_STOK_KULLANIM_SEKLI");
                entity.IDM_BIRIM = Util.getFieldString(dt.Rows[i], "IDM_BIRIM");
                listem.Add(entity);
            }

            return listem;
        }

        [Route("api/IsEmriDurusList")]
        [HttpGet]
        public List<IsEmriDurus> IsEmriDurusList([FromUri] int page, [FromUri] int pageSize, [FromUri] int isemriID)
        {
            var util = new Util();
            var prms = new DynamicParameters();
            prms.Add("PAGE", page);
            prms.Add("PAGE_SIZE", pageSize);
            string sql = @"SELECT * FROM (SELECT 
                                                MKD.*
                                                ,L.TB_LOKASYON_ID
                                                ,L.LOK_TANIM
                                                ,M.TB_MAKINE_ID
                                                ,M.MKN_KOD
                                                ,M.MKN_TANIM
                                                ,M.MKN_LOKASYON_ID
                                                ,P.TB_PROJE_ID
                                                ,P.PRJ_KOD
                                                ,P.PRJ_TANIM
                                                ,ROW_NUMBER() OVER (ORDER BY MKD.MKD_BASLAMA_TARIH DESC, MKD_BASLAMA_SAAT DESC) ROW_NUM
                                                FROM orjin.VW_MAKINE_DURUS MKD
                                                LEFT JOIN orjin.TB_LOKASYON L ON L.TB_LOKASYON_ID=MKD.MKD_LOKASYON_ID
                                                LEFT JOIN orjin.TB_MAKINE M ON M.TB_MAKINE_ID=MKD.MKD_MAKINE_ID
                                                LEFT JOIN orjin.TB_PROJE P ON P.TB_PROJE_ID=MKD.MKD_PROJE_ID
                                                 WHERE 1=1 ";

            if (isemriID > 0)
            {
                sql += " AND MKD.MKD_ISEMRI_ID=@ISM_ID";
                prms.Add("ISM_ID", isemriID);
            }
            sql += @"
                                            ) MTABLE WHERE ROW_NUM BETWEEN @PAGE*@PAGE_SIZE+1 AND @PAGE*@PAGE_SIZE+@PAGE_SIZE";
            using (var cnn = util.baglan())
            {
                List<IsEmriDurus> listem = cnn.Query<IsEmriDurus, Lokasyon, Makine, Proje, IsEmriDurus>(sql, map: (i, l, m, p) =>
                     {
                         i.MKD_NEDEN = i.MKD_NEDEN ?? "";
                         i.MKD_ACIKLAMA = Util.RemoveRtfFormatting(i.MKD_ACIKLAMA);
                         i.MKD_MAKINE = m;
                         i.MKD_LOKASYON = l;
                         i.MKD_PROJE = p;
                         return i;
                     }, splitOn: "TB_LOKASYON_ID,TB_MAKINE_ID,TB_PROJE_ID", param: prms).ToList();
                return listem;
            }
        }


        [Route("api/IsEmriKapat")]
        [HttpPost]
        public Bildirim IsEmriKapat([FromBody] IsEmri entity)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                string qu1 = @"UPDATE orjin.TB_ISEMRI SET
                                     ISM_BILDIRIM_TARIH = @ISM_BILDIRIM_TARIH
                                      ,ISM_BILDIRIM_SAAT= @ISM_BILDIRIM_SAAT 
                                      ,ISM_KAPATILDI = @ISM_KAPATILDI                                      
                                      ,ISM_PUAN = @ISM_PUAN
                                      ,ISM_SONUC = @ISM_SONUC
                                      ,ISM_DURUM_KOD_ID = 3
                                      ,ISM_TAMAMLANMA_ORAN = 100
                                      ,ISM_KAPAT_MAKINE_DURUM_KOD_ID = @ISM_KAPAT_MAKINE_DURUM_KOD_ID
                                      ,ISM_SONUC_KOD_ID = @ISM_SONUC_KOD_ID                                    
                                      ,ISM_DEGISTIREN_ID=@ISM_DEGISTIREN_ID
                                      ,ISM_DEGISTIRME_TARIH=@ISM_DEGISTIRME_TARIH                                   
                                       WHERE TB_ISEMRI_ID = @TB_ISEMRI_ID";
                prms.Clear();
                prms.Add("@TB_ISEMRI_ID", entity.TB_ISEMRI_ID);
                if (entity.ISM_BILDIRIM_TARIH != null)
                    prms.Add("@ISM_BILDIRIM_TARIH", entity.ISM_BILDIRIM_TARIH);
                else
                    prms.Add("@ISM_BILDIRIM_TARIH", null);
                if (entity.ISM_BILDIRIM_SAAT != null)
                    prms.Add("@ISM_BILDIRIM_SAAT", entity.ISM_BILDIRIM_SAAT);
                else
                    prms.Add("@ISM_BILDIRIM_SAAT", null);
                prms.Add("@ISM_KAPATILDI", true);
                prms.Add("@ISM_PUAN", entity.ISM_PUAN);
                prms.Add("@ISM_SONUC", entity.ISM_SONUC);
                prms.Add("@ISM_KAPAT_MAKINE_DURUM_KOD_ID", entity.ISM_KAPAT_MAKINE_DURUM_KOD_ID);
                prms.Add("@ISM_SONUC_KOD_ID", entity.ISM_SONUC_KOD_ID);
                prms.Add("@ISM_DEGISTIREN_ID", entity.ISM_DEGISTIREN_ID);
                prms.Add("@ISM_DEGISTIRME_TARIH", DateTime.Now);
                klas.cmd(qu1, prms.PARAMS);
                string query = @"INSERT INTO orjin.TB_ISEMRI_LOG
                                           (ISL_ISEMRI_ID
                                           ,ISL_KULLANICI_ID
                                           ,ISL_TARIH
                                           ,ISL_SAAT
                                           ,ISL_ISLEM
                                           ,ISL_DURUM_ESKI_KOD_ID
                                           ,ISL_DURUM_YENI_KOD_ID
                                           ,ISL_OLUSTURAN_ID
                                           ,ISL_OLUSTURMA_TARIH)
                                     VALUES
                                            (@ISL_ISEMRI_ID
                                            ,@ISL_KULLANICI_ID
                                            ,@ISL_TARIH
                                            ,@ISL_SAAT
                                            ,@ISL_ISLEM
                                            ,@ISL_DURUM_ESKI_KOD_ID
                                            ,@ISL_DURUM_YENI_KOD_ID
                                            ,@ISL_OLUSTURAN_ID
                                            ,@ISL_OLUSTURMA_TARIH)";
                prms.Clear();
                prms.Add("ISL_ISEMRI_ID", entity.TB_ISEMRI_ID);
                prms.Add("ISL_KULLANICI_ID", entity.ISM_DEGISTIREN_ID);
                prms.Add("ISL_TARIH", entity.ISM_BILDIRIM_TARIH ?? DateTime.Now);
                prms.Add("ISL_SAAT", entity.ISM_BILDIRIM_SAAT ?? DateTime.Now.ToString(C.DB_TIME_FORMAT));
                prms.Add("ISL_ISLEM", "İş emri kapatıldı");
                prms.Add("ISL_DURUM_ESKI_KOD_ID", -1);
                prms.Add("ISL_DURUM_YENI_KOD_ID", -1);
                prms.Add("ISL_OLUSTURAN_ID", entity.ISM_DEGISTIREN_ID);
                prms.Add("ISL_OLUSTURMA_TARIH", DateTime.Now);
                klas.cmd(query, prms.PARAMS);

                prms.Clear();
                prms.Add("ISM_ID", entity.TB_ISEMRI_ID);
                // iş talep durumu değiştirilip log yazılıyor
                DataRow drTalep = klas.GetDataRow("select * from orjin.TB_IS_TALEBI where IST_ISEMRI_ID=@ISM_ID",
                    prms.PARAMS);
                if (drTalep != null)
                {
                    klas.cmd("UPDATE orjin.TB_IS_TALEBI SET IST_DURUM_ID=4 WHERE IST_ISEMRI_ID = @ISM_ID", prms.PARAMS);
                    prms.Clear();
                    prms.Add("ISM_SONUC_KOD_ID", entity.ISM_SONUC_KOD_ID);
                    string aciklama =
                        klas.GetDataCell("select KOD_TANIM from orjin.TB_KOD where TB_KOD_ID=@ISM_SONUC_KOD_ID",
                            prms.PARAMS);
                    if (aciklama == null) aciklama = "";
                    IsTalepController.IsTalepTarihceYaz(Convert.ToInt32(drTalep["TB_IS_TALEP_ID"]),
                        entity.ISM_DEGISTIREN_ID, "Kapandı", aciklama, "KAPANDI");
                }

                // kapatma esnasında malzeme hareketleri.
                List<IsEmriMalzeme> mlzListem = IsEmriMalzemeList(entity.TB_ISEMRI_ID)
                    .Where(a => a.IDM_STOK_KULLANIM_SEKLI == 2).ToList();

                for (int i = 0; i < mlzListem.Count; i++)
                {
                    StokHareketIslemi(mlzListem[i]);
                }

                bildirimEntity.Aciklama = "İş emri başarılı bir şekilde kapatıldı.";
                bildirimEntity.MsgId = Bildirim.MSG_ISM_KAPAT_OK;
                bildirimEntity.Durum = true;
            }
            catch (Exception ex)
            {
                klas.kapat();
                bildirimEntity.Aciklama = String.Format(Localization.errorFormatted, ex.Message);
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }

            return bildirimEntity;
        }

        [Route("api/IsEmriAc")]
        [HttpPost]
        public Bildirim IsEmriAc([FromUri] int isemriID, [FromUri] int KulID, [FromUri] int durumKodId)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                string qu1 = @"UPDATE orjin.TB_ISEMRI SET
                                     ISM_BILDIRIM_TARIH = @ISM_BILDIRIM_TARIH
                                      ,ISM_BILDIRIM_SAAT= @ISM_BILDIRIM_SAAT 
                                      ,ISM_KAPATILDI = @ISM_KAPATILDI                                      
                                      ,ISM_PUAN = ''
                                      ,ISM_SONUC = @ISM_SONUC
                                      ,ISM_DURUM_KOD_ID = (SELECT top 1 TB_KOD_ID FROM orjin.TB_KOD WHERE KOD_GRUP = 32801 AND KOD_TANIM = 'AÇIK')
                                      ,ISM_TAMAMLANMA_ORAN = 100
                                      ,ISM_KAPAT_MAKINE_DURUM_KOD_ID = @ISM_KAPAT_MAKINE_DURUM_KOD_ID
                                      ,ISM_SONUC_KOD_ID = @ISM_SONUC_KOD_ID                                    
                                      ,ISM_DEGISTIREN_ID=@ISM_DEGISTIREN_ID
                                      ,ISM_DEGISTIRME_TARIH=@ISM_DEGISTIRME_TARIH                                   
                                       WHERE TB_ISEMRI_ID = @TB_ISEMRI_ID";
                prms.Clear();
                prms.Add("@TB_ISEMRI_ID", isemriID);
                prms.Add("@ISM_BILDIRIM_TARIH", null);
                prms.Add("@ISM_BILDIRIM_SAAT", null);
                prms.Add("@ISM_KAPATILDI", false);
                prms.Add("@ISM_SONUC", "");
                prms.Add("@ISM_KAPAT_MAKINE_DURUM_KOD_ID", -1);
                prms.Add("@ISM_SONUC_KOD_ID", -1);
                prms.Add("@ISM_DEGISTIREN_ID", KulID);
                prms.Add("@ISM_DEGISTIRME_TARIH", DateTime.Now);
                klas.cmd(qu1, prms.PARAMS);
                string query = @"INSERT INTO [orjin].[TB_ISEMRI_LOG]
                                           (ISL_ISEMRI_ID
                                           ,ISL_KULLANICI_ID
                                           ,ISL_TARIH
                                           ,ISL_SAAT
                                           ,ISL_ACIKLAMA
                                           ,ISL_ISLEM
                                           ,ISL_DURUM_ESKI_KOD_ID
                                           ,ISL_DURUM_YENI_KOD_ID
                                           ,ISL_OLUSTURAN_ID
                                           ,ISL_OLUSTURMA_TARIH)
                                     VALUES
                                            (@ISL_ISEMRI_ID
                                            ,@ISL_KULLANICI_ID
                                            ,@ISL_TARIH
                                            ,@ISL_SAAT
                                            ,(SELECT TOP 1 KOD_TANIM FROM orjin.TB_KOD WHERE TB_KOD_ID=@durumKodId)
                                            ,@ISL_ISLEM
                                            ,@ISL_DURUM_ESKI_KOD_ID
                                            ,@ISL_DURUM_YENI_KOD_ID
                                            ,@ISL_OLUSTURAN_ID
                                            ,@ISL_OLUSTURMA_TARIH)";
                prms.Clear();
                prms.Add("ISL_ISEMRI_ID", isemriID);
                prms.Add("ISL_KULLANICI_ID", KulID);
                prms.Add("ISL_TARIH", DateTime.Now);
                prms.Add("ISL_SAAT", DateTime.Now.ToString("HH:mm:ss"));
                prms.Add("durumKodId", durumKodId);
                prms.Add("ISL_ISLEM", "İş emri yeniden açıldı");
                prms.Add("ISL_DURUM_ESKI_KOD_ID", -1);
                prms.Add("ISL_DURUM_YENI_KOD_ID", -1);
                prms.Add("ISL_OLUSTURAN_ID", KulID);
                prms.Add("ISL_OLUSTURMA_TARIH", DateTime.Now);
                klas.cmd(query, prms.PARAMS);

                // İş emri açılışında malzeme hareketleri geri alınıyor.
                List<IsEmriMalzeme> mlzListem =
                    IsEmriMalzemeList(isemriID).Where(a => a.IDM_STOK_KULLANIM_SEKLI == 2).ToList();

                for (int i = 0; i < mlzListem.Count; i++)
                {
                    if (mlzListem[i].IDM_STOK_DUS && mlzListem[i].IDM_STOK_KULLANIM_SEKLI == 1 &&
                        (mlzListem[i].IDM_MALZEME_STOKTAN == "Düşsün" || mlzListem[i].IDM_MALZEME_STOKTAN == "Sorsun"))
                    {
                        // depo stok güncelleniyor  
                        string qu2 =
                            "UPDATE orjin.TB_DEPO_STOK SET DPS_CIKAN_MIKTAR = DPS_CIKAN_MIKTAR - Cast(@Miktar AS FLOAT), DPS_KULLANILABILIR_MIKTAR = DPS_KULLANILABILIR_MIKTAR + Cast(@Miktar AS FLOAT), DPS_MIKTAR = DPS_MIKTAR + Cast(@Miktar AS FLOAT)  WHERE DPS_DEPO_ID = @IDM_DEPO_ID  AND DPS_STOK_ID = @IDM_STOK_ID";
                        prms.Clear();
                        prms.Add("Miktar", mlzListem[i].IDM_MIKTAR);
                        prms.Add("IDM_DEPO_ID", mlzListem[i].IDM_DEPO_ID);
                        prms.Add("IDM_STOK_ID", mlzListem[i].IDM_STOK_ID);
                        klas.cmd(qu2, prms.PARAMS);

                        // stok güncelleniyor.
                        string qu3 =
                            "UPDATE orjin.TB_STOK SET STK_CIKAN_MIKTAR = STK_CIKAN_MIKTAR - Cast(@Miktar AS FLOAT), STK_MIKTAR = STK_MIKTAR - Cast(@Miktar AS FLOAT)  WHERE TB_STOK_ID = @IDM_STOK_ID";
                        prms.Clear();
                        prms.Add("Miktar", mlzListem[i].IDM_MIKTAR);
                        prms.Add("IDM_STOK_ID", mlzListem[i].IDM_STOK_ID);
                        klas.cmd(qu3, prms.PARAMS);
                        //STOK HAREKET siliniyor
                        prms.Clear();
                        prms.Add("IDM_STOK_ID", mlzListem[i].IDM_STOK_ID);
                        prms.Add("IDM_ISEMRI_ID", mlzListem[i].IDM_ISEMRI_ID);
                        klas.cmd(
                            "DELETE FROM orjin.TB_STOK_HRK WHERE SHR_STOK_ID=@IDM_STOK_ID and SHR_REF_ID=@IDM_ISEMRI_ID",
                            prms.PARAMS);
                    }
                }

                // iş talep durumu değiştirilip log yazılıyor
                prms.Clear();
                prms.Add("ISM_ID", isemriID);
                DataRow drTalep = klas.GetDataRow("select * from orjin.TB_IS_TALEBI where IST_ISEMRI_ID=@ISM_ID",
                    prms.PARAMS);
                if (drTalep != null)
                {
                    klas.cmd("UPDATE orjin.TB_IS_TALEBI SET IST_DURUM_ID=3 WHERE IST_ISEMRI_ID = @ISM_ID", prms.PARAMS);
                    String str =
                        klas.GetDataCell("select ISM_ISEMRI_NO from orjin.TB_ISEMRI where TB_ISEMRI_ID=@ISM_ID",
                            prms.PARAMS) + " nolu iş emri yeniden açıldı";
                    IsTalepController.IsTalepTarihceYaz(Convert.ToInt32(drTalep["TB_IS_TALEP_ID"]), KulID,
                        "Yeniden Açıldı", str, "DEVAM EDIYOR");
                }

                bildirimEntity.Aciklama = "İş emri başarılı bir şekilde açıldı.";
                bildirimEntity.MsgId = Bildirim.MSG_ISM_AC_OK;
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

        [Route("api/IsemriMalzemeSil")]
        [HttpPost]
        public Bildirim IsEmriMalzemeSil([FromBody] IsEmriMalzeme entity)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                if (entity.IDM_STOK_DUS == true && (entity.IDM_MALZEME_STOKTAN.Trim() == "Düşsün" ||
                                                    entity.IDM_MALZEME_STOKTAN.Trim() == "Sorsun"))
                {
                    //depo stok güncelleniyor

                    string query =
                        "UPDATE orjin.TB_DEPO_STOK SET DPS_CIKAN_MIKTAR = DPS_CIKAN_MIKTAR - Cast(@Miktar AS FLOAT), DPS_KULLANILABILIR_MIKTAR = DPS_KULLANILABILIR_MIKTAR + Cast(@Miktar AS FLOAT), DPS_MIKTAR = DPS_MIKTAR + Cast(@Miktar AS FLOAT)  WHERE DPS_DEPO_ID = @IDM_DEPO_ID  AND DPS_STOK_ID = @IDM_STOK_ID";
                    prms.Clear();
                    prms.Add("Miktar", entity.IDM_MIKTAR);
                    prms.Add("IDM_DEPO_ID", entity.IDM_DEPO_ID);
                    prms.Add("IDM_STOK_ID", entity.IDM_STOK_ID);
                    klas.cmd(query, prms.PARAMS);
                    // stok güncelleniyor.
                    string qu1 =
                        "UPDATE orjin.TB_STOK SET STK_CIKAN_MIKTAR = STK_CIKAN_MIKTAR - Cast(@Miktar AS FLOAT), STK_MIKTAR = STK_MIKTAR + Cast(@Miktar AS FLOAT)  WHERE TB_STOK_ID = @IDM_STOK_ID";
                    prms.Clear();
                    prms.Add("Miktar", entity.IDM_MIKTAR);
                    prms.Add("IDM_STOK_ID", entity.IDM_STOK_ID);
                    klas.cmd(qu1, prms.PARAMS);

                    //STOK HAREKET siliniyor                  
                    prms.Clear();
                    prms.Add("IDM_STOK_ID", entity.IDM_STOK_ID);
                    prms.Add("IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
                    klas.cmd(
                        "DELETE FROM orjin.TB_STOK_HRK WHERE SHR_STOK_ID=@IDM_STOK_ID  and SHR_REF_ID=@IDM_ISEMRI_ID",
                        prms.PARAMS);
                }

                string qu2 =
                    "UPDATE orjin.TB_ISEMRI SET ISM_MALIYET_MLZ = ISM_MALIYET_MLZ - Cast(@Maliyet AS FLOAT), ISM_MALIYET_TOPLAM =ISM_MALIYET_TOPLAM -  Cast(@Maliyet AS FLOAT) where TB_ISEMRI_ID = @IDM_ISEMRI_ID";
                prms.Clear();
                prms.Add("Maliyet", entity.IDM_TUTAR);
                prms.Add("IDM_ISEMRI_ID", entity.IDM_ISEMRI_ID);
                klas.cmd(qu2, prms.PARAMS);

                prms.Clear();
                prms.Add("TB_ISEMRI_MLZ_ID", entity.TB_ISEMRI_MLZ_ID);
                klas.cmd("DELETE FROM orjin.TB_ISEMRI_MLZ WHERE TB_ISEMRI_MLZ_ID=@TB_ISEMRI_MLZ_ID", prms.PARAMS);

                bildirimEntity.Aciklama = "İşlem başarılı bir şekilde gerçekleşti.";
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_BASARILI;
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

        [Route("api/IsemriKontrolListSil")]
        [HttpPost]
        public Bildirim IsemriKontrolListSil([FromUri] int kontrolListId)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                prms.Clear();
                prms.Add("KONT_LIST_ID", kontrolListId);
                klas.cmd("DELETE FROM orjin.TB_ISEMRI_KONTROLLIST WHERE TB_ISEMRI_KONTROLLIST_ID=@KONT_LIST_ID",
                    prms.PARAMS);
                bildirimEntity.Aciklama = "İşlem başarılı bir şekilde gerçekleşti.";
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_BASARILI;
                bildirimEntity.Durum = true;
            }
            catch (Exception e)
            {
                klas.kapat();
                bildirimEntity.Aciklama =
                    String.Format(Localization.errorFormatted, e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }

            return bildirimEntity;
        }

        [Route("api/IsemriDurusSil")]
        [HttpPost]
        public Bildirim IsemriDurusSil([FromUri] int makineDurusId)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                prms.Clear();
                prms.Add("TB_MAKINE_DURUS_ID", makineDurusId);
                klas.cmd("DELETE FROM orjin.TB_MAKINE_DURUS WHERE TB_MAKINE_DURUS_ID=@TB_MAKINE_DURUS_ID", prms.PARAMS);
                bildirimEntity.Aciklama = "Duruş kaydı başarılı bir şekilde gerçekleşti.";
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_BASARILI;
                bildirimEntity.Durum = true;
            }
            catch (Exception e)
            {
                klas.kapat();
                bildirimEntity.Aciklama = string.Format(Localization.errorFormatted, e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }

            return bildirimEntity;
        }

        [Route("api/IsemriPersonelSil")]
        [HttpPost]
        public Bildirim IsemriPersonelSil([FromUri] int isemriPersonelId, [FromUri] double isemriPersonelMaliyet,
            [FromUri] int isemriId)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                string qu1 =
                    "UPDATE orjin.TB_ISEMRI SET ISM_MALIYET_PERSONEL = ISM_MALIYET_PERSONEL - Cast(@Maliyet AS FLOAT), ISM_MALIYET_TOPLAM =ISM_MALIYET_TOPLAM -  Cast(@Maliyet AS FLOAT) where TB_ISEMRI_ID = @ISM_ID";
                prms.Clear();
                prms.Add("Maliyet", isemriPersonelMaliyet);
                prms.Add("ISM_ID", isemriId);
                klas.cmd(qu1, prms.PARAMS);

                prms.Clear();
                prms.Add("TB_ISEMRI_KAYNAK_ID", isemriPersonelId);
                klas.cmd("DELETE FROM orjin.TB_ISEMRI_KAYNAK WHERE TB_ISEMRI_KAYNAK_ID=@TB_ISEMRI_KAYNAK_ID",
                    prms.PARAMS);
                bildirimEntity.Aciklama = "İşlem başarılı bir şekilde gerçekleşti.";
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_BASARILI;
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

        [Route("api/IsEmriNotEkle")]
        [HttpPost]
        public void IsEmriNotEkle([FromUri] int isemriID, [FromUri] string Not)
        {
            Bildirim bildirimEntity = new Bildirim();
            try
            {
                prms.Clear();
                prms.Add("NOT", Not);
                prms.Add("ISM_ID", isemriID);
                klas.cmd("UPDATE orjin.TB_ISEMRI SET ISM_NOT = @NOT WHERE TB_ISEMRI_ID = @ISM_ID", prms.PARAMS);
                bildirimEntity.Aciklama = "İşlem başarılı bir şekilde gerçekleşti.";
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_BASARILI;
                bildirimEntity.Durum = true;
            }
            catch (Exception e)
            {
                klas.kapat();
                bildirimEntity.Aciklama = string.Format(Localization.errorFormatted, e.Message);
                bildirimEntity.MsgId = Bildirim.MSG_ISLEM_HATA;
                bildirimEntity.HasExtra = true;
                bildirimEntity.Durum = false;
            }
        }

        [Route("api/IsEmriCozumKatalog")]
        [HttpGet]
        public List<CozumKatalogModel> GetCozumKatalogListByIsEmri([FromUri] int isEmriID)
        {
            prms.Clear();
            prms.Add("IS_EMRI_ID", isEmriID);
            string sql = @" select K1.KOD_TANIM AS TESHIS, K2.KOD_TANIM AS NEDEN,* from PBTPRO_1.orjin.TB_COZUM_KATALOG CK
                            LEFT JOIN PBTPRO_1.orjin.TB_ISEMRI I ON (CK.CMK_REF_ID=TB_ISEMRI_ID)
                            LEFT JOIN PBTPRO_1.orjin.TB_KOD K1 ON (CK.CMK_TESHIS_ID=K1.TB_KOD_ID)
                            LEFT JOIN PBTPRO_1.orjin.TB_KOD K2 ON (CK.CMK_NEDEN_ID=K2.TB_KOD_ID)
                            WHERE TB_ISEMRI_ID = @IS_EMRI_ID ";
            List<CozumKatalogModel> listem = new List<CozumKatalogModel>();
            DataTable dt = klas.GetDataTable(sql, prms.PARAMS);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                CozumKatalogModel entity = new CozumKatalogModel();
                entity.TB_COZUM_KATALOG_ID = Convert.ToInt32(dt.Rows[i]["TB_COZUM_KATALOG_ID"]);
                entity.CMK_KOD = (dt.Rows[i]["CMK_KOD"]).ToString();
                entity.CMK_KONU = Util.getFieldString(dt.Rows[i], "CMK_KONU");
                entity.CMK_TESHIS = dt.Rows[i]["TESHIS"] != DBNull.Value ? dt.Rows[i]["TESHIS"].ToString() : "";
                entity.CMK_NEDEN = dt.Rows[i]["NEDEN"] != DBNull.Value ? dt.Rows[i]["NEDEN"].ToString() : "";
                listem.Add(entity);
            }

            return listem;
        }

        [Route("api/IsEmriDetaySayilar")]
        [HttpGet]
        public Sayilar IsEmriDetaySayilar([FromUri] int isemriID)
        {
            try
            {
                prms.Clear();
                prms.Add("@ISM_ID", isemriID);
                Sayilar entity = new Sayilar();
                entity.MalzemeSayisi = Convert.ToInt32(klas.GetDataCell(
                    "select COUNT(TB_ISEMRI_MLZ_ID) from orjin.VW_ISEMRI_MLZ where IDM_ISEMRI_ID = @ISM_ID",
                    prms.PARAMS));
                entity.KontrolListYapildi = Convert.ToInt32(klas.GetDataCell(
                    "select COUNT(TB_ISEMRI_KONTROLLIST_ID) from orjin.TB_ISEMRI_KONTROLLIST where DKN_YAPILDI=1 and DKN_ISEMRI_ID = @ISM_ID",
                    prms.PARAMS));
                entity.KontrolListYapilmadi = Convert.ToInt32(klas.GetDataCell(
                    "select COUNT(TB_ISEMRI_KONTROLLIST_ID) from orjin.TB_ISEMRI_KONTROLLIST where DKN_YAPILDI=0 and DKN_ISEMRI_ID = @ISM_ID",
                    prms.PARAMS));
                entity.PersonelSayisi = Convert.ToInt32(klas.GetDataCell(
                    "select COUNT(TB_ISEMRI_KAYNAK_ID) from orjin.TB_ISEMRI_KAYNAK where IDK_ISEMRI_ID = @ISM_ID",
                    prms.PARAMS));
                entity.DurusSayisi = Convert.ToInt32(klas.GetDataCell(
                    "select COUNT(TB_MAKINE_DURUS_ID) from orjin.TB_MAKINE_DURUS where MKD_ISEMRI_ID = @ISM_ID",
                    prms.PARAMS));
                entity.Dosya = Convert.ToInt32(klas.GetDataCell(
                    "SELECT COUNT(TB_DOSYA_ID) FROM dbo.TB_DOSYA WHERE DSY_AKTIF = 1 AND DSY_REF_GRUP = 'ISEMRI' AND DSY_REF_ID = @ISM_ID",
                    prms.PARAMS));
                entity.CozumKataloglarSayisi = Convert.ToInt32(klas.GetDataCell(
                    @" select COUNT(*) from PBTPRO_1.orjin.TB_COZUM_KATALOG CK
                    LEFT JOIN PBTPRO_1.orjin.TB_ISEMRI I ON (CK.CMK_REF_ID=TB_ISEMRI_ID)
                    LEFT JOIN PBTPRO_1.orjin.TB_KOD K1 ON (CMK_TESHIS_ID=K1.TB_KOD_ID)
                    LEFT JOIN PBTPRO_1.orjin.TB_KOD K2 ON (CMK_NEDEN_ID=K2.TB_KOD_ID)
                    WHERE TB_ISEMRI_ID = @ISM_ID ",
                    prms.PARAMS));
                return entity;
            }
            catch (Exception)
            {
                klas.kapat();
                throw;
            }
        }

        [Route("api/getIsEmriOncelik")]
        [HttpGet]
		public String getIsEmriOncelik([FromUri] int isEmriId)
		{
			try
			{
				var util = new Util();
				using (var conn = util.baglan())
				{
					var sql = $"SELECT SOC_TANIM FROM orjin.TB_ISEMRI left join orjin.TB_SERVIS_ONCELIK on TB_SERVIS_ONCELIK_ID = ISM_ONCELIK_ID where TB_ISEMRI_ID = {isEmriId}";
					var oncelik = conn.Query<String>(sql).FirstOrDefault();
					return oncelik;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}