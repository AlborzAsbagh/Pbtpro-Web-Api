using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WebApiNew.Models
{
    [DataContract]
    public class Bildirim
    {
        public static readonly int MSG_ISLEM_BASARILI = 1
            ,MSG_IS_EMRI_KAYIT_HATA = 2
            ,MSG_ISEMRI_SIL_BASARILI = 3
            ,MSG_ISEMRI_SIL_HATA = 4
            ,MSG_ISEMRI_KONTROL_LISTE_KAYIT_OK = 5
            ,MSG_ISEMRI_KONTROL_LISTE_GUNCELLE_OK = 6
            ,MSG_ISLEM_HATA = 7
            ,MSG_ISM_PERSONEL_KAYIT_OK = 8
            ,MSG_ISM_PERSONEL_GUNCELLE_OK = 9
            ,MSG_ISM_MALZEME_KAYIT_OK = 10
            ,MSG_ISM_MALZEME_GUNCELLE_OK = 11
            ,MSG_ISM_DURUS_KAYIT_OK = 12
            ,MSG_ISM_DURUS_GUNCELLE_OK = 13
            ,MSG_ISM_KAPAT_OK = 14
            ,MSG_ISM_AC_OK = 15
            ,MSG_YKT_KAYIT_OK = 16
            ,MSG_YKT_GUNCELLE_OK = 17
            ,MSG_IST_KAYIT_OK = 18
            ,MSG_IST_GUNCELLE_OK = 19
            ,MSG_KLL_SIFRE_HATA = 20
            ,MSG_KLL_GUNCELLE_OK = 21
            ,MSG_MKN_KAYIT_OK = 22
            ,MSG_MKN_GUNCELLE_OK = 23
            ,MSG_LOK_DEG_KAYIT_OK = 24
            ,MSG_LOK_DEG_KAYIT_ERR = 25
            ,MSG_LOK_DEG_GUNCELLE_OK = 26
            ,MSG_LOK_DEG_GUNCELLE_ERR = 27
            ,MSG_LOK_DEG_SIL_OK = 28
            ,MSG_LOK_DEG_SIL_ERR = 29
            ,MSG_UYGUN_FORMATTA_RESIM_YUKLE_UYARI = 30
            ,MSG_SYO_SIL_BASARILI = 31
            ,MSG_SYO_SIL_HATA = 46
            ,MSG_SYO_KAYIT_OK = 32
            ,MSG_SYO_GUNCELLE_OK = 33
            ,MSG_SSD_GUNCELLE_OK = 34
            ,MSG_SSD_GUNCELLE_HATA = 35
            ,MSG_SFS_KAYIT_OK = 36//MALZEME TALEP
            ,MSG_SFS_GUNCELLE_OK = 37//MALZEME TALEP
            ,MSG_SFS_GUNCELLE_HATA = 38
            ,MSG_SFS_MLZ_KAYIT_OK = 39
            ,MSG_SFS_MLZ_GUNCELLE_OK = 40
            ,MSG_SFS_MLZ_SIL_OK = 41
            ,MSG_SFS_MLZ_SIL_HATA = 42
            ,MSG_SFS_SIL_OK = 43
            ,MSG_SFS_SIL_HATA = 44
            ,MSG_YKT_SIL_OK = 45
            ,MSG_NO_MSG = 46
            ,SHOW_MAIN_DESCRIPTION = 47
            ,MSG_KAYIT_YOK = 48
            ;

        public Bildirim()
        {
            this.MsgId = -1;
        }
        [DataMember]
        public bool Durum { get; set; }

        [DataMember]
        public bool Error { get; set; }

        [DataMember]
        public bool HasExtra { get; set; }

        [DataMember]
        public string Aciklama { get; set; }

        [DataMember]
        public long Id { get; set; }

        [DataMember]
        public int MsgId { get; set; }

        [DataMember]
        public List<long> Idlist { get; set; }

        [DataMember]
        public List<long> Idlist1 { get; set; }
    }
}