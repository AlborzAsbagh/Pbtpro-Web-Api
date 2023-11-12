using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    [Table("orjin.TB_IS_TANIM_KONTROLLIST")]
    public class IsTanimKontrol
    {
        [DataMember]
        [Key]
        public int TB_IS_TANIM_KONROLLIST_ID { get; set; }
        [DataMember]
        public int ISK_IS_TANIM_ID { get; set; }
        [DataMember]
        public string ISK_SIRANO { get; set; }
        [DataMember]
        public bool ISK_YAPILDI { get; set; }
        [DataMember]
        public string ISK_TANIM { get; set; }
		[DataMember]
		public string PERSONEL_ISIM { get; set; }
		[DataMember]
        public int ISK_OLUSTURAN_ID { get; set; }
        [DataMember]
        public DateTime? ISK_OLUSTURMA_TARIH { get; set; }
        [DataMember]
        public int ISK_DEGISTIREN_ID { get; set; }
        [DataMember]
        public DateTime? ISK_DEGISTIRME_TARIH { get; set; }
        [DataMember]
        public double ISK_MALIYET { get; set; }
        [DataMember]
        public string ISK_ACIKLAMA { get; set; }
        [DataMember]
        public int ISK_TALIMAT_ID { get; set; }
        [DataMember]
        [Write(false)]
        [Computed]
        public int ISK_IMAGE_ID { get; set; }
        [DataMember]
        [Write(false)]
        [Computed]
        public String ISK_IMAGE_IDS_STR { get; set; }

        [DataMember]
        [Write(false)]
        [Computed]
        public List<int> ISK_IMAGE_IDS
        {
            get
            {
                var list=new List<int>();
                var idsstr = ISK_IMAGE_IDS_STR?.Split(';')??new string[0];
                for (int i = 0; i < idsstr.Length; i++)
                {
                    list.Add(Convert.ToInt32(idsstr[i]));
                }
                return list;
            }
        }
    }

}