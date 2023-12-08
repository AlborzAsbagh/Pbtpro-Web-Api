using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class IsEmriPersonel
    {
        [DataMember]
        public int TB_ISEMRI_KAYNAK_ID { get; set; }
        [DataMember]
        public int IDK_RESIM_ID { get; set; }

        [DataMember]
        public int IDK_ISEMRI_ID { get; set; }

        [DataMember]
        public int IDK_SOZLESME_ID { get; set; }

        [DataMember]
        public int IDK_REF_ID { get; set; }

        [DataMember]
        public string IDK_REF_GRUP { get; set; }

        [DataMember]
        public DateTime? IDK_BASLAMA_TARIH { get; set; }

        [DataMember]
        public string IDK_BASLAMA_SAAT { get; set; }

        [DataMember]
        public DateTime? IDK_BITIS_TARIH { get; set; }

        [DataMember]
        public string IDK_BITIS_SAAT { get; set; }

        [DataMember]
        public string IDK_EVRAK_NO { get; set; }

        [DataMember]
        public string IDK_ACIKLAMA { get; set; }

        [DataMember]
        public string IDK_VARDIYA_TANIM { get; set; }

        [DataMember]
        public double IDK_SURE { get; set; }

        [DataMember]
        public double IDK_SAAT_UCRETI { get; set; }

        [DataMember]
        public int IDK_VARDIYA { get; set; }

        [DataMember]
        public bool IDK_FAZLA_MESAI_VAR { get; set; }

        [DataMember]
        public int IDK_FAZLA_MESAI_SURE { get; set; }

        [DataMember]
        public double IDK_FAZLA_MESAI_SAAT_UCRETI { get; set; }

        [DataMember]
        public double IDK_MALIYET { get; set; }

        [DataMember]
        public bool IDK_OKUMA { get; set; }

        [DataMember]
        public bool IDK_MAIL_GONDERILDI { get; set; }

        [DataMember]
        public int IDK_OLUSTURAN_ID { get; set; }

        [DataMember]
        public string IDK_PERSONEL_KOD { get; set; }

        [DataMember]
        public string IDK_ISIM { get; set; }

        [DataMember]
        public string IDK_MASRAF_MERKEZI { get; set; }

        [DataMember]
        public Boolean IDK_SILINDI { get; set; }

        [DataMember]
        public int IDK_DEGISTIREN_ID { get; set; }

        [DataMember]
        public DateTime? IDK_DEGISTIRME_TARIH { get; set; }

        [DataMember]
        public int IDK_MASRAF_MERKEZI_ID { get; set; }
    }
}