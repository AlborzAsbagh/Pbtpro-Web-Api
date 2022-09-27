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

        public static readonly string GET_KLL_ISTALEP_USER_QUERY = @"SELECT * FROM orjin.TB_IS_TALEBI_KULLANICI  WHERE ISK_PERSONEL_ID = (SELECT KLL_PERSONEL_ID FROM {0}.orjin.TB_KULLANICI WHERE TB_KULLANICI_ID=@KLL_ID)";
    }
}