using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Http;
using WebApiNew.Models;
using Dapper;
using Dapper.Contrib.Extensions;
using WebApiNew.Filters;
using WebApiNew.Utility;
using WebApiNew.Utility.Abstract;

namespace WebApiNew.Controllers
{

    [MyBasicAuthenticationFilter]
    public class OtonomBakimController : ApiController
    {
        private readonly ILogger _logger;

        public OtonomBakimController(ILogger logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [Route("api/MakineBakim/GetByMakine")]
        public List<MakineBakim> GetMakineBakims([FromUri] int kllId, [FromUri] int makineId ,[FromUri] int prsId)
        {
            #region sql
            var sql = @"SELECT TB_MAKINE_BAKIM_ID
      ,MAB_MAKINE_ID
      ,MAB_BAKIM_ID
      ,MAB_DEGISTIREN_ID
      ,MAB_OLUSTURAN_ID
      ,MAB_OLUSTURMA_TARIH
      ,MAB_DEGISTIRME_TARIH
      ,CASE 
        WHEN IT.IST_UYAR = 0 THEN 0 
        WHEN IT.IST_UYARI_PERIYOT = 1 THEN 0
        WHEN IT.IST_UYARI_PERIYOT = 2 AND (SELECT COUNT(*) FROM orjin.TB_MAKINE_BAKIM_TARIHCE WHERE MBT_MAKINE_BAKIM_ID=MB.TB_MAKINE_BAKIM_ID AND MBT_TARIH BETWEEN DATEADD(DAY,-1,GETDATE()) AND GETDATE()) >= IT.IST_UYARI_SIKLIGI THEN 1
        WHEN IT.IST_UYARI_PERIYOT = 3 AND (SELECT COUNT(*) FROM orjin.TB_MAKINE_BAKIM_TARIHCE WHERE MBT_MAKINE_BAKIM_ID=MB.TB_MAKINE_BAKIM_ID AND MBT_TARIH BETWEEN DATEADD(WEEK,-1,GETDATE()) AND GETDATE()) >= IT.IST_UYARI_SIKLIGI THEN 1
        WHEN IT.IST_UYARI_PERIYOT = 4 AND (SELECT COUNT(*) FROM orjin.TB_MAKINE_BAKIM_TARIHCE WHERE MBT_MAKINE_BAKIM_ID=MB.TB_MAKINE_BAKIM_ID AND MBT_TARIH BETWEEN DATEADD(MONTH,-1,GETDATE()) AND GETDATE()) >= IT.IST_UYARI_SIKLIGI THEN 1
        WHEN IT.IST_UYARI_PERIYOT = 5 AND (SELECT COUNT(*) FROM orjin.TB_MAKINE_BAKIM_TARIHCE WHERE MBT_MAKINE_BAKIM_ID=MB.TB_MAKINE_BAKIM_ID AND MBT_TARIH BETWEEN DATEADD(YEAR,-1,GETDATE()) AND GETDATE()) >= IT.IST_UYARI_SIKLIGI THEN 1
        ELSE 0
        END AS MAB_UYAR
	  ,TB_IS_TANIM_ID
      ,IST_KOD
      ,IST_TANIM
      ,IST_DURUM
      ,IST_AKTIF
      ,IST_TIP_KOD_ID
      ,IST_GRUP_KOD_ID
      ,IST_ATOLYE_ID
      ,IST_CALISMA_SURE
      ,IST_DURUS_SURE
      ,IST_PERSONEL_SAYI
      ,IST_ONCELIK_ID
      ,IST_TALIMAT_ID
      ,IST_ACIKLAMA
      ,IST_OLUSTURAN_ID
      ,IST_OLUSTURMA_TARIH
      ,IST_DEGISTIREN_ID
      ,IST_DEGISTIRME_TARIH
      ,IST_MALZEME_MALIYET
      ,IST_ISCILIK_MALIYET
      ,IST_GENEL_GIDER_MALIYET
      ,IST_TOPLAM_MALIYET
      ,IST_NEDEN_KOD_ID
      ,IST_FIRMA_ID
      ,IST_IS_TALEPTE_GORUNSUN
      ,IST_MASRAF_MERKEZ_ID
      ,IST_SURE_LOJISTIK
      ,IST_SURE_SEYAHAT
      ,IST_SURE_ONAY
      ,IST_SURE_BEKLEME
      ,IST_SURE_DIGER
      ,IST_OZEL_ALAN_1
      ,IST_OZEL_ALAN_2
      ,IST_OZEL_ALAN_3
      ,IST_OZEL_ALAN_4
      ,IST_OZEL_ALAN_5
      ,IST_OZEL_ALAN_7_KOD_ID
      ,IST_OZEL_ALAN_9
      ,IST_OZEL_ALAN_10
      ,IST_OZEL_ALAN_6_KOD_ID
      ,IST_OZEL_ALAN_8_KOD_ID
      ,IST_UYAR
      ,IST_UYARI_SIKLIGI
      ,IST_LOKASYON_ID
      ,IST_OTONOM_BAKIM
      ,IST_UYARI_PERIYOT
      ,(SELECT COUNT(*) FROM orjin.TB_IS_TANIM_KONTROLLIST WHERE ISK_IS_TANIM_ID = TB_IS_TANIM_ID) AS IST_KONTROL_SAYI
       FROM orjin.TB_MAKINE_BAKIM MB
      INNER JOIN orjin.TB_MAKINE M ON M.TB_MAKINE_ID=MB.MAB_MAKINE_ID
      INNER JOIN orjin.TB_IS_TANIM IT ON IT.TB_IS_TANIM_ID=MB.MAB_BAKIM_ID " // WHERE CONDITION body starts
      + $" WHERE MB.MAB_MAKINE_ID=@MKN_ID AND orjin.UDF_LOKASYON_YETKI_KONTROL(M.MKN_LOKASYON_ID,@KLL_ID) = 1 /* AND IST_PERSONEL_ID = {prsId} */ ";
            #endregion
            var util = new Util();
            using (var cnn = util.baglan())
            {
                var result = cnn.Query<MakineBakim, IsTanim, MakineBakim>(sql, (mb, it) =>
                  {
                      mb.MAB_IS_TANIM = it;
                      return mb;
                  }, new { KLL_ID = kllId, MKN_ID = makineId }, splitOn: "TB_IS_TANIM_ID");
                var r = result.ToList();
                return r;
            }
        }
        [HttpGet]
        [Route("api/MakineBakim/GetHistoryByMakine")]
        public List<MakineBakimTarihce> GetMakineBakimHistoryByMakine([FromUri] int kllId, [FromUri] int makineId, [FromUri] int page, [FromUri] int pageSize)
        {
            var util = new Util();
            #region sql
            var sql = @"SELECT MBT.*
                              ,MBD.*  
                              ,M.*  
                              ,K.*
                              ,MB.*  
                              ,IT.*  
                          FROM orjin.TB_MAKINE_BAKIM_TARIHCE MBT
                          INNER JOIN orjin.TB_MAKINE_BAKIM MB ON MB.TB_MAKINE_BAKIM_ID=MBT.MBT_MAKINE_BAKIM_ID 
                          INNER JOIN orjin.TB_IS_TANIM IT ON MB.MAB_BAKIM_ID=IT.TB_IS_TANIM_ID 
                          INNER JOIN orjin.TB_MAKINE M ON M.TB_MAKINE_ID=MB.MAB_MAKINE_ID
                          INNER JOIN {0}.orjin.TB_KULLANICI K ON MBT.MBT_OLUSTURAN_ID=K.TB_KULLANICI_ID
                          INNER JOIN  orjin.TB_MAKINE_BAKIM_TARIHCE_DETAY MBD ON MBD.MBD_MAKINE_BAKIM_TARIHCE_ID=MBT.TB_MAKINE_BAKIM_TARIHCE_ID
                          WHERE MB.MAB_MAKINE_ID=@MKN_ID AND orjin.UDF_LOKASYON_YETKI_KONTROL(M.MKN_LOKASYON_ID,@KLL_ID)=1
                          ORDER BY MBT.MBT_TARIH DESC, MBT.MBT_SAAT DESC
";
            sql = String.Format(sql, util.GetMasterDbName());
            #endregion
            using (var cnn = util.baglan())
            {
                var mdict = new Dictionary<int, MakineBakimTarihce>();
                var list = cnn.Query<MakineBakimTarihce, MakineBakimTarihceDetay, Makine, Kullanici, MakineBakim, IsTanim, MakineBakimTarihce>(sql, (mbt, mbd, mkn, kll, mb, it) =>
                     {
                         MakineBakimTarihce mbtEntry;
                         if (!mdict.TryGetValue(mbt.TB_MAKINE_BAKIM_TARIHCE_ID, out mbtEntry))
                         {
                             mbtEntry = mbt;
                             mbtEntry.MBT_DETAY = new List<MakineBakimTarihceDetay>();
                             mbtEntry.MBT_KULLANICI = kll;
                             mbtEntry.MBT_MAKINE = mkn;
                             mb.MAB_IS_TANIM = it;
                             mbtEntry.MBT_MAKINE_BAKIM = mb;
                             mbtEntry.MBT_MAKINE = mkn;
                             mdict.Add(mbtEntry.TB_MAKINE_BAKIM_TARIHCE_ID, mbtEntry);
                         }
                         mbtEntry.MBT_DETAY.Add(mbd);
                         return mbtEntry;
                     }, new { MKN_ID = makineId, KLL_ID = kllId }, splitOn: "TB_MAKINE_BAKIM_TARIHCE_DETAY_ID,TB_MAKINE_ID,TB_KULLANICI_ID,TB_MAKINE_BAKIM_ID,TB_IS_TANIM_ID")
                    .Distinct()
                    .ToList();
                return list;
            }
        }
        [HttpPost]
        [Route("api/MakineBakim/AddMakineBakimTarihce")]
        public Bildirim AddMakineBakimTarihce([FromUri] int kllId, [FromBody] MakineBakimTarihce makineBakimTarihce)
        {
            var idList = new List<long>();
            var idList1 = new List<long>();
            var util = new Util();
            var istAciklama = "";
            var getIstUserQuery = String.Format(Queries.GET_KLL_ISTALEP_USER_QUERY, util.GetMasterDbName());
            using (var cnn = util.baglan())
            {
                _logger.Info($"Kullanıcı Id= {kllId}");
                try
                {

                    using (TransactionScope ts = new TransactionScope())
                    {
                        makineBakimTarihce.TB_MAKINE_BAKIM_TARIHCE_ID = (int)cnn.Insert(makineBakimTarihce);
                        foreach (var dty in makineBakimTarihce.MBT_DETAY)
                        {
                            dty.MBD_MAKINE_BAKIM_TARIHCE_ID = makineBakimTarihce.TB_MAKINE_BAKIM_TARIHCE_ID;
                            dty.TB_MAKINE_BAKIM_TARIHCE_DETAY_ID = (int)cnn.Insert(dty);
                        }

                        ts.Complete();
                    }
                    var isTalepController = new IsTalepController(_logger);
                    _logger.Info($"İş talep sayısı={makineBakimTarihce.MBT_IS_TALEPLERI.Count}");
                    foreach (var ist in makineBakimTarihce.MBT_IS_TALEPLERI)
                    {

                        string lq = @"select * from orjin.TB_LOKASYON where TB_LOKASYON_ID=@LOK_ID";
                        var lokasyon = cnn.QueryFirstOrDefault<Lokasyon>(lq, new { LOK_ID = ist.IST_BILDIREN_LOKASYON_ID });
                        if (lokasyon != null)
                        {
                            ist.IST_BILDIREN_LOKASYON = lokasyon.LOK_TANIM;
                            ist.IST_BILDIREN_LOKASYON_TUM = lokasyon.LOK_TUM_YOL;
                        }
                        var now = DateTime.Now;
                        now = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, 0);
                        ist.IST_KOD = cnn.QueryFirstOrDefault<string>(Queries.GENERATE_KOD, new { KOD = "IST_KOD" });
                        ist.IST_OLUSTURMA_TARIH = DateTime.Now;
                        ist.IST_ACILIS_TARIHI = now;
                        ist.IST_ACILIS_SAATI = DateTime.Now.ToString("HH:mm:ss");
                        var talepEden = cnn.QueryFirstOrDefault<TalepKullanici>(getIstUserQuery, new { KLL_ID = kllId });
                        if (talepEden != null)
                        {
                            ist.IST_TALEP_EDEN_ADI = talepEden.ISK_ISIM;
                            ist.IST_TALEP_EDEN_ID = talepEden.TB_IS_TALEBI_KULLANICI_ID;
                        }
                        var b = isTalepController.Post(ist, kllId);
                        if (b.Durum)
                        {
                            istAciklama += ist.IST_KOD + " nolu iş talebi oluşturuldu!\n";
                            idList1.Add(b.Id);
                            _logger.Info(ist.IST_KOD + " nolu iş talebi oluşturuldu!");
                        }
                        else
                        {
                            istAciklama += ist.IST_KOD + " nolu iş talebi kaydedilemedi!\n";
                            _logger.Info("api/MakineBakim/AddMakineBakimTarihce");
                            _logger.Error($"{ist.IST_KOD} no'lu iş talebi kaydedilemedi : {b.Aciklama}");
                        }
                    }
                }
                catch (Exception e)
                {
                    return new Bildirim
                    {
                        Aciklama = e.Message,
                        Durum = false,
                        MsgId = Bildirim.MSG_ISLEM_HATA,
                    };
                }
            }

            makineBakimTarihce.MBT_DETAY.ForEach(delegate (MakineBakimTarihceDetay item)
            {
                idList.Add(item.TB_MAKINE_BAKIM_TARIHCE_DETAY_ID);
            });

            var bildirim = new Bildirim
            {
                MsgId = Bildirim.MSG_ISLEM_BASARILI,
                Durum = true,
                Id = makineBakimTarihce.TB_MAKINE_BAKIM_TARIHCE_ID,
                Idlist = idList,
                Idlist1 = idList1
            };
            if (!String.IsNullOrWhiteSpace(istAciklama))
            {
                bildirim.Aciklama = istAciklama;
                bildirim.HasExtra = true;
            }
            return bildirim;
        }


    }
}