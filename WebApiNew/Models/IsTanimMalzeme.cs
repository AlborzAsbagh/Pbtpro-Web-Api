using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
    [DataContract]
    public class IsTanimMalzeme
    {
         [DataMember] public int TB_IS_TANIM_MLZ_ID { get; set; }

         [DataMember] public int ISM_IS_TANIM_ID { get; set; }

         [DataMember] public int ISM_STOK_ID { get; set; }

         [DataMember] public int ISM_BIRIM_KOD_ID { get; set; }

         [DataMember] public double ISM_BIRIM_FIYAT { get; set; }

         [DataMember] public double ISM_MIKTAR { get; set; }

         [DataMember] public double ISM_TUTAR { get; set; }

         [DataMember] public string ISM_ACIKLAMA { get; set; }      

         [DataMember] public string ISM_STOK_TANIM { get; set; }

         [DataMember] public int ISM_STOK_TIP_KOD_ID { get; set; }

         [DataMember] public int ISM_DEPO_ID { get; set; }

         [DataMember] public string ISM_STOK_KOD { get; set; }

        
    }

	public class IsTanimMalzemeWebAppModel
	{
        [DataMember] public int TB_IS_TANIM_MLZ_ID { get; set; }

        [DataMember] public int ISM_STOK_ID { get; set; }

        [DataMember] public int ISM_IS_TANIM_ID { get; set; }

		[DataMember] public int ISM_DEPO_ID { get; set; }

        [DataMember] public string ISM_STOK_KOD { get; set; }

        [DataMember] public string ISM_STOK_TANIM { get; set; }

        [DataMember] public int ISM_STOK_TIP_KOD_ID { get; set; }

        [DataMember] public string ISM_STOK_TIP { get; set; }

        [DataMember] public double ISM_MIKTAR { get; set; }

		[DataMember] public double ISM_TUTAR { get; set; }

		[DataMember] public double ISM_BIRIM_FIYAT { get; set; }

		[DataMember] public int ISM_BIRIM_KOD_ID { get; set; }

        [DataMember] public string ISM_BIRIM { get; set; }

        [DataMember] public DateTime? ISM_OLUSTURMA_TARIH { get; set; }

		[DataMember] public DateTime? ISM_DEGISTIRME_TARIH { get; set; }

        [DataMember] public int ISM_OLUSTURAN_ID { get; set; }

        [DataMember] public int ISM_DEGISTIREN_ID { get; set; }

		[DataMember] public string ISM_ACIKLAMA { get; set; }


	}
}