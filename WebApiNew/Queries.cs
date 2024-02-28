using System;
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
    }
}