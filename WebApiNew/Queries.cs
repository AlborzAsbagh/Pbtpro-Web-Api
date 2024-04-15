﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiNew
{
    public class Queries
    {
        public static readonly string GENERATE_KOD = @"UPDATE orjin.TB_NUMARATOR SET NMR_NUMARA = NMR_NUMARA+1 WHERE NMR_KOD = @KOD;
                                SELECT NMR_ON_EK+right(replicate('0',NMR_HANE_SAYISI)+CAST(NMR_NUMARA AS VARCHAR(MAX)),NMR_HANE_SAYISI) AS deger FROM orjin.TB_NUMARATOR
                        WHERE NMR_KOD = @KOD;";

        public static readonly string GET_KLL_ISTALEP_USER_QUERY = @"SELECT * FROM PBTPRO_MASTER.orjin.TB_IS_TALEBI_KULLANICI  WHERE ISK_PERSONEL_ID = (SELECT KLL_PERSONEL_ID FROM {0}.orjin.TB_KULLANICI WHERE TB_KULLANICI_ID=@KLL_ID)";

        public static readonly string KLL_EKLE_YETKISI = @" select KYT_EKLE from PBTPRO_MASTER.orjin.TB_KULLANICI_YETKI where KYT_KULLANICI_ID = @KYT_KULLANICI_ID and KYT_YETKI_KOD = @KYT_YETKI_KOD ";

        public static readonly string KLL_GUNCELLE_YETKISI = @" select KYT_DEGISTIR from PBTPRO_MASTER.orjin.TB_KULLANICI_YETKI where KYT_KULLANICI_ID = @KYT_KULLANICI_ID and KYT_YETKI_KOD = @KYT_YETKI_KOD ";

        public static readonly string KLL_SIL_YETKISI = @" select KYT_SIL from PBTPRO_MASTER.orjin.TB_KULLANICI_YETKI where KYT_KULLANICI_ID = @KYT_KULLANICI_ID and KYT_YETKI_KOD = @KYT_YETKI_KOD ";

        public static readonly string MKN_FETCH_QUERY = @"WITH RowNumberedResults AS (
                                    select	mkn.TB_MAKINE_ID as TB_MAKINE_ID ,
                                            mkn.MKN_KOD as MKN_KOD ,
		                                    mkn.MKN_TANIM as MKN_TANIM , 
		                                    (
                                            SELECT COUNT(1) AS Expr1
                                            FROM dbo.TB_DOSYA
                                            WHERE DSY_REF_ID = mkn.TB_MAKINE_ID AND DSY_REF_GRUP = 'MAKINE'
                                        ) AS MKN_BELGE ,
	                                    CASE 
                                            WHEN (
                                                SELECT COUNT(1)
                                                FROM dbo.TB_DOSYA
                                                WHERE DSY_REF_ID = mkn.TB_MAKINE_ID AND DSY_REF_GRUP = 'MAKINE'
                                            ) = 0 THEN 0 
                                            ELSE 1 
                                        END AS MKN_BELGE_VAR ,

	                                    (
                                            SELECT COUNT(1) AS Expr1
                                            FROM orjin.TB_RESIM AS R
                                            WHERE R.RSM_REF_ID = mkn.TB_MAKINE_ID AND R.RSM_REF_GRUP = 'MAKINE'
                                        ) AS MKN_RESIM ,
	                                     CASE 
                                            WHEN (
                                                SELECT COUNT(1)
                                                FROM orjin.TB_RESIM R
                                                WHERE R.RSM_REF_ID = mkn.TB_MAKINE_ID AND R.RSM_REF_GRUP = 'MAKINE'
                                            ) = 0 THEN 0 
                                            ELSE 1 
                                        END AS MKN_RESIM_VAR ,
	                                     CASE 
                                            WHEN (
                                                SELECT COUNT(1)
                                                FROM orjin.TB_PERIYODIK_BAKIM_MAKINE PM
                                                WHERE PM.PBM_MAKINE_ID = mkn.TB_MAKINE_ID
                                            ) > 0 THEN 1 
                                            ELSE 0 
                                        END AS MKN_PERIYODIK_BAKIM ,

	                                    mkn.MKN_AKTIF as MKN_AKTIF ,
	                                    mkn.MKN_SERI_NO as MKN_SERI_NO ,
	                                    mkn.MKN_URETIM_YILI as MKN_URETIM_YILI ,
	                                    master_mkn.MKN_KOD as MKN_MASTER_MAKINE_KOD,
	                                    master_mkn.MKN_TANIM as MKN_MASTER_MAKINE_TANIM,
	                                    durum_kod.KOD_TANIM as MKN_DURUM ,
	                                    tip_kod.KOD_TANIM as MKN_TIP ,
	                                    mrk.MRK_MARKA as MKN_MARKA ,
	                                    mdl.MDL_MODEL as MKN_MODEL ,
	                                    lok.LOK_TANIM as MKN_LOKASYON ,
	                                    lok.LOK_TUM_YOL as MKN_LOKASYON_TUM_YOL ,
	                                    kategori_kod.KOD_TANIM as MKN_KATEGORI ,
	                                    orjin.UDF_KOD_TANIM(mkn.MKN_BAKIM_GRUP_ID) AS MKN_BAKIM_GRUP,
                                        orjin.UDF_KOD_TANIM(mkn.MKN_ARIZA_GRUP_ID) AS MKN_ARIZA_GRUP,
                                        orjin.UDF_KOD_TANIM(mkn.MKN_AKS_ON_EBAT_KOD_ID) AS MKN_AKS_ON_EBAT,
                                        orjin.UDF_KOD_TANIM(mkn.MKN_AKS_ARKA_EBAT_KOD_ID) AS MKN_AKS_ARKA_EBAT,
                                        (SELECT PRS_ISIM FROM orjin.TB_PERSONEL AS OPR WHERE TB_PERSONEL_ID = mkn.MKN_OPERATOR_PERSONEL_ID) AS MKN_OPERATOR,
                                        (SELECT TKV_TANIM FROM orjin.TB_TAKVIM AS T WHERE TB_TAKVIM_ID = mkn.MKN_TAKVIM_ID) AS MKN_TAKVIM,
                                        (SELECT MAM_TANIM FROM orjin.TB_MASRAF_MERKEZ AS MAM WHERE TB_MASRAF_MERKEZ_ID = mkn.MKN_MASRAF_MERKEZ_KOD_ID) AS MKN_MASRAF_MERKEZ,
                                        (SELECT ATL_TANIM FROM orjin.TB_ATOLYE AS ATL WHERE TB_ATOLYE_ID = mkn.MKN_ATOLYE_ID) AS MKN_ATOLYE,
                                        (SELECT TOP (1) SURE FROM orjin.UDF_MAKINE_ARIZA_SURE(mkn.TB_MAKINE_ID) WHERE ZAMAN = 'G') AS ARIZA_SIKLIGI,
                                        (SELECT ARIZA_SAYISI FROM orjin.UDF_MAKINE_ARIZA_SURE(mkn.TB_MAKINE_ID) WHERE ZAMAN = 'G') AS ARIZA_SAYISI,
                                        (SELECT ARC_EGZOS_TARIH FROM orjin.TB_ARAC AS A WHERE ARC_MAKINE_ID = mkn.TB_MAKINE_ID) AS MKN_ARC_EGZOS_TARIH,
	                                    (SELECT SOC_TANIM FROM orjin.TB_SERVIS_ONCELIK AS SOC WHERE SOC.TB_SERVIS_ONCELIK_ID = mkn.MKN_ONCELIK_ID) AS MKN_ONCELIK,
	                                    mkn.MKN_OZEL_ALAN_1 as MKN_OZEL_ALAN_1 ,
	                                    mkn.MKN_OZEL_ALAN_2 as MKN_OZEL_ALAN_2 ,
	                                    mkn.MKN_OZEL_ALAN_3 as MKN_OZEL_ALAN_3 ,
	                                    mkn.MKN_OZEL_ALAN_4 as MKN_OZEL_ALAN_4 ,
	                                    mkn.MKN_OZEL_ALAN_5 as MKN_OZEL_ALAN_5 ,
	                                    mkn.MKN_OZEL_ALAN_6 as MKN_OZEL_ALAN_6 ,
	                                    mkn.MKN_OZEL_ALAN_7 as MKN_OZEL_ALAN_7 ,
	                                    mkn.MKN_OZEL_ALAN_8 as MKN_OZEL_ALAN_8 ,
	                                    mkn.MKN_OZEL_ALAN_9 as MKN_OZEL_ALAN_9 ,
	                                    mkn.MKN_OZEL_ALAN_10 as MKN_OZEL_ALAN_10 ,
                                        orjin.UDF_KOD_TANIM(mkn.MKN_OZEL_ALAN_11_KOD_ID) AS MKN_OZEL_ALAN_11,
                                        orjin.UDF_KOD_TANIM(mkn.MKN_OZEL_ALAN_12_KOD_ID) AS MKN_OZEL_ALAN_12,
                                        orjin.UDF_KOD_TANIM(mkn.MKN_OZEL_ALAN_13_KOD_ID) AS MKN_OZEL_ALAN_13,
                                        orjin.UDF_KOD_TANIM(mkn.MKN_OZEL_ALAN_14_KOD_ID) AS MKN_OZEL_ALAN_14,
                                        orjin.UDF_KOD_TANIM(mkn.MKN_OZEL_ALAN_15_KOD_ID) AS MKN_OZEL_ALAN_15 ,
	                                    mkn.MKN_OZEL_ALAN_16 as MKN_OZEL_ALAN_16 ,
	                                    mkn.MKN_OZEL_ALAN_17 as MKN_OZEL_ALAN_17 ,
	                                    mkn.MKN_OZEL_ALAN_18 as MKN_OZEL_ALAN_18 ,
	                                    mkn.MKN_OZEL_ALAN_19 as MKN_OZEL_ALAN_19 ,
	                                    mkn.MKN_OZEL_ALAN_20 as MKN_OZEL_ALAN_20 ,

	                                     ROW_NUMBER() OVER (ORDER BY mkn.MKN_KOD) AS RowIndex
	                                    FROM orjin.TB_MAKINE mkn 
	                                    left join orjin.TB_MAKINE master_mkn on mkn.MKN_MASTER_ID = master_mkn.TB_MAKINE_ID

	                                    left join orjin.TB_KOD durum_kod on mkn.MKN_DURUM_KOD_ID = durum_kod.TB_KOD_ID

	                                    left join orjin.TB_KOD tip_kod on mkn.MKN_TIP_KOD_ID = tip_kod.TB_KOD_ID

	                                    left join orjin.TB_KOD kategori_kod on mkn.MKN_KATEGORI_KOD_ID = kategori_kod.TB_KOD_ID

	                                    left join orjin.TB_MARKA mrk on mkn.MKN_MARKA_KOD_ID = mrk.TB_MARKA_ID

	                                    left join orjin.TB_MODEL mdl on mkn.MKN_MODEL_KOD_ID = mdl.TB_MODEL_ID

	                                    left join orjin.TB_LOKASYON lok on mkn.MKN_LOKASYON_ID = lok.TB_LOKASYON_ID

                                    WHERE 1=1";

        public static readonly string MKN_FETCH_COUNT_QUERY = @"select count(*) from  (
                                            select	mkn.TB_MAKINE_ID as TB_MAKINE_ID ,
                                                    mkn.MKN_KOD as MKN_KOD ,
		                                            mkn.MKN_TANIM as MKN_TANIM , 
		                                            (
                                                    SELECT COUNT(1) AS Expr1
                                                    FROM dbo.TB_DOSYA
                                                    WHERE DSY_REF_ID = mkn.TB_MAKINE_ID AND DSY_REF_GRUP = 'MAKINE'
                                                ) AS MKN_BELGE ,
	                                            CASE 
                                                    WHEN (
                                                        SELECT COUNT(1)
                                                        FROM dbo.TB_DOSYA
                                                        WHERE DSY_REF_ID = mkn.TB_MAKINE_ID AND DSY_REF_GRUP = 'MAKINE'
                                                    ) = 0 THEN 0 
                                                    ELSE 1 
                                                END AS MKN_BELGE_VAR ,

	                                            (
                                                    SELECT COUNT(1) AS Expr1
                                                    FROM orjin.TB_RESIM AS R
                                                    WHERE R.RSM_REF_ID = mkn.TB_MAKINE_ID AND R.RSM_REF_GRUP = 'MAKINE'
                                                ) AS MKN_RESIM ,
	                                             CASE 
                                                    WHEN (
                                                        SELECT COUNT(1)
                                                        FROM orjin.TB_RESIM R
                                                        WHERE R.RSM_REF_ID = mkn.TB_MAKINE_ID AND R.RSM_REF_GRUP = 'MAKINE'
                                                    ) = 0 THEN 0 
                                                    ELSE 1 
                                                END AS MKN_RESIM_VAR ,
	                                             CASE 
                                                    WHEN (
                                                        SELECT COUNT(1)
                                                        FROM orjin.TB_PERIYODIK_BAKIM_MAKINE PM
                                                        WHERE PM.PBM_MAKINE_ID = mkn.TB_MAKINE_ID
                                                    ) > 0 THEN 1 
                                                    ELSE 0 
                                                END AS MKN_PERIYODIK_BAKIM ,

	                                            mkn.MKN_AKTIF as MKN_AKTIF ,
	                                            mkn.MKN_SERI_NO as MKN_SERI_NO ,
	                                            mkn.MKN_URETIM_YILI as MKN_URETIM_YILI ,
	                                            master_mkn.MKN_KOD as MKN_MASTER_MAKINE_KOD,
	                                            master_mkn.MKN_TANIM as MKN_MASTER_MAKINE_TANIM,
	                                            durum_kod.KOD_TANIM as MKN_DURUM ,
	                                            tip_kod.KOD_TANIM as MKN_TIP ,
	                                            mrk.MRK_MARKA as MKN_MARKA ,
	                                            mdl.MDL_MODEL as MKN_MODEL ,
	                                            lok.LOK_TANIM as MKN_LOKASYON ,
	                                            lok.LOK_TUM_YOL as MKN_LOKASYON_TUM_YOL ,
	                                            kategori_kod.KOD_TANIM as MKN_KATEGORI ,
	                                            orjin.UDF_KOD_TANIM(mkn.MKN_BAKIM_GRUP_ID) AS MKN_BAKIM_GRUP,
                                                orjin.UDF_KOD_TANIM(mkn.MKN_ARIZA_GRUP_ID) AS MKN_ARIZA_GRUP,
                                                orjin.UDF_KOD_TANIM(mkn.MKN_AKS_ON_EBAT_KOD_ID) AS MKN_AKS_ON_EBAT,
                                                orjin.UDF_KOD_TANIM(mkn.MKN_AKS_ARKA_EBAT_KOD_ID) AS MKN_AKS_ARKA_EBAT,
                                                (SELECT PRS_ISIM FROM orjin.TB_PERSONEL AS OPR WHERE TB_PERSONEL_ID = mkn.MKN_OPERATOR_PERSONEL_ID) AS MKN_OPERATOR,
                                                (SELECT TKV_TANIM FROM orjin.TB_TAKVIM AS T WHERE TB_TAKVIM_ID = mkn.MKN_TAKVIM_ID) AS MKN_TAKVIM,
                                                (SELECT MAM_TANIM FROM orjin.TB_MASRAF_MERKEZ AS MAM WHERE TB_MASRAF_MERKEZ_ID = mkn.MKN_MASRAF_MERKEZ_KOD_ID) AS MKN_MASRAF_MERKEZ,
                                                (SELECT ATL_TANIM FROM orjin.TB_ATOLYE AS ATL WHERE TB_ATOLYE_ID = mkn.MKN_ATOLYE_ID) AS MKN_ATOLYE,
                                                (SELECT TOP (1) SURE FROM orjin.UDF_MAKINE_ARIZA_SURE(mkn.TB_MAKINE_ID) WHERE ZAMAN = 'G') AS ARIZA_SIKLIGI,
                                                (SELECT ARIZA_SAYISI FROM orjin.UDF_MAKINE_ARIZA_SURE(mkn.TB_MAKINE_ID) WHERE ZAMAN = 'G') AS ARIZA_SAYISI,
                                                (SELECT ARC_EGZOS_TARIH FROM orjin.TB_ARAC AS A WHERE ARC_MAKINE_ID = mkn.TB_MAKINE_ID) AS MKN_ARC_EGZOS_TARIH,
	                                            (SELECT SOC_TANIM FROM orjin.TB_SERVIS_ONCELIK AS SOC WHERE SOC.TB_SERVIS_ONCELIK_ID = mkn.MKN_ONCELIK_ID) AS MKN_ONCELIK,
	                                            mkn.MKN_OZEL_ALAN_1 as MKN_OZEL_ALAN_1 ,
	                                            mkn.MKN_OZEL_ALAN_2 as MKN_OZEL_ALAN_2 ,
	                                            mkn.MKN_OZEL_ALAN_3 as MKN_OZEL_ALAN_3 ,
	                                            mkn.MKN_OZEL_ALAN_4 as MKN_OZEL_ALAN_4 ,
	                                            mkn.MKN_OZEL_ALAN_5 as MKN_OZEL_ALAN_5 ,
	                                            mkn.MKN_OZEL_ALAN_6 as MKN_OZEL_ALAN_6 ,
	                                            mkn.MKN_OZEL_ALAN_7 as MKN_OZEL_ALAN_7 ,
	                                            mkn.MKN_OZEL_ALAN_8 as MKN_OZEL_ALAN_8 ,
	                                            mkn.MKN_OZEL_ALAN_9 as MKN_OZEL_ALAN_9 ,
	                                            mkn.MKN_OZEL_ALAN_10 as MKN_OZEL_ALAN_10 ,
                                                orjin.UDF_KOD_TANIM(mkn.MKN_OZEL_ALAN_11_KOD_ID) AS MKN_OZEL_ALAN_11,
                                                orjin.UDF_KOD_TANIM(mkn.MKN_OZEL_ALAN_12_KOD_ID) AS MKN_OZEL_ALAN_12,
                                                orjin.UDF_KOD_TANIM(mkn.MKN_OZEL_ALAN_13_KOD_ID) AS MKN_OZEL_ALAN_13,
                                                orjin.UDF_KOD_TANIM(mkn.MKN_OZEL_ALAN_14_KOD_ID) AS MKN_OZEL_ALAN_14,
                                                orjin.UDF_KOD_TANIM(mkn.MKN_OZEL_ALAN_15_KOD_ID) AS MKN_OZEL_ALAN_15 ,
	                                            mkn.MKN_OZEL_ALAN_16 as MKN_OZEL_ALAN_16 ,
	                                            mkn.MKN_OZEL_ALAN_17 as MKN_OZEL_ALAN_17 ,
	                                            mkn.MKN_OZEL_ALAN_18 as MKN_OZEL_ALAN_18 ,
	                                            mkn.MKN_OZEL_ALAN_19 as MKN_OZEL_ALAN_19 ,
	                                            mkn.MKN_OZEL_ALAN_20 as MKN_OZEL_ALAN_20 ,

	                                             ROW_NUMBER() OVER (ORDER BY mkn.MKN_KOD) AS RowIndex
	                                            FROM orjin.TB_MAKINE mkn 
	                                            left join orjin.TB_MAKINE master_mkn on mkn.MKN_MASTER_ID = master_mkn.TB_MAKINE_ID

	                                            left join orjin.TB_KOD durum_kod on mkn.MKN_DURUM_KOD_ID = durum_kod.TB_KOD_ID

	                                            left join orjin.TB_KOD tip_kod on mkn.MKN_TIP_KOD_ID = tip_kod.TB_KOD_ID

	                                            left join orjin.TB_KOD kategori_kod on mkn.MKN_KATEGORI_KOD_ID = kategori_kod.TB_KOD_ID

	                                            left join orjin.TB_MARKA mrk on mkn.MKN_MARKA_KOD_ID = mrk.TB_MARKA_ID

	                                            left join orjin.TB_MODEL mdl on mkn.MKN_MODEL_KOD_ID = mdl.TB_MODEL_ID

	                                            left join orjin.TB_LOKASYON lok on mkn.MKN_LOKASYON_ID = lok.TB_LOKASYON_ID

                                             where 1=1  ";
    }
}