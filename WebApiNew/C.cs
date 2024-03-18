

using System;
using WebApiNew.Filters;

namespace WebApiNew
{
    public class C
    {
        public static readonly string REF_GRUP_IS_TALEP = "CAGRI MERKEZI";
        public static readonly string KEY_USER = "user";
        public static readonly string DB_TIME_FORMAT = "HH:mm:ss";
        public static readonly string KGP_ARIZA_TIPLERI = "32401";
        public static readonly string KGP_BAKIM_TIPLERI = "32440";
        public static readonly string KGP_ISEMRI_DURUM = "32801";
        public static readonly string KGP_MALZEME_BIRIM = "13001";
        public static readonly string KGP_OLCU_BIRIM = "32001";
        public static readonly string KGP_MALZEME_TIP = "13005";
        public static readonly string KGP_VARDIYA_TANIMLARI = "32759";
        public static readonly string KGP_ARIZA_NEDENLERI = "32413";
        public static readonly string KGP_BAKIM_NEDENLERI = "32452";
        public static readonly string KGP_DURUS_NEDENLERI = "32300";
    }

    public class PersonelRolloeri
    {
        public static readonly int PERSONEL_TEKNESIYEN = 1;
        public static readonly int PERSONEL_SURUCU = 2;
        public static readonly int PERSONEL_OPERATOR = 3;
        public static readonly int PERSONEL_BAKIM = 4;
        public static readonly int PERSONEL_SANTIYE = 5;
    }

    public class UserInfo
    {
        public static readonly int USER_ID = Convert.ToInt32(JwtAuthenticationFilter.GetUserIdFromClaims());
    }

    public class PagesAuthCodes
    {
        public static readonly int MAKINE_TANIMLARI = 1001;
        public static readonly int BAKIM_TANIMLARI = 2001;
        public static readonly int ARIZA_TANIMLARI = 2002;
        public static readonly int ISEMIRLERI_TANIMLARI = 2003;
        public static readonly int ATOLYE_TANIMLARI = 9002;
        public static readonly int PERSONEL_TANIMLARI = 9001;
        public static readonly int IS_TALEPLERI_TANIMLARI = 10001;
        public static readonly int LOKASYON_TANIMLARI = 1003;
        public static readonly int VARDIYA_TANIMLARI = 30014;
	}

}