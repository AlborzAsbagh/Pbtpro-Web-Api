using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Web;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;

namespace WebApiNew.Models
{
    [DataContract]
    [Table("orjin.TB_MAKINE_PUANTAJ")]
    public class MakineCalisma
    {
        [DataMember]
        [Key]
        public long TB_MAKINE_PUANTAJ_ID { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public int MPJ_DONEM_ID { get; set; }
		[DataMember]
        public int MPJ_SANTIYE_ID { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public int MPJ_TASERON_ID { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public int MPJ_PERSONEL_ID { get; set; }
		[DataMember] 
        public DateTime? MPJ_TARIH { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public double MPJ_SURE_GUN { get; set; }
		[DataMember] 
        public double MPJ_SURE_SAAT { get; set; }
		[DataMember] 
        public double MPJ_SURE_DAKIKA { get; set; }
		[DataMember] 
        public double MPJ_FIYAT { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public double MPJ_FIYAT_GUN { get; set; }
		[DataMember] 
        public double MPJ_TUTAR { get; set; }
		[DataMember]
        public string MPJ_ACIKLAMA { get; set; }
		[DataMember]
        public string MPJ_CALISMA_TIP { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public int MPJ_CALISMAMA_KOD_ID { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public int MPJ_IZIN_ID { get; set; }
		[DataMember]
        public int MPJ_ISTANIM_ID { get; set; }
		[DataMember]
        public int MPJ_PROJE_ID { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_PERSONEL_CALISMA_TIP { get; set; }
		[DataMember]
        public int MPJ_MAKINE_ID { get; set; }
		[DataMember]
        public int MPJ_PROJE_IS_ID { get; set; }
		[DataMember] 
        public double MPJ_IS_MIKTAR { get; set; }
		[DataMember] 
        public int MPJ_IS_BIRIM_KOD_ID { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public bool MPJ_CALISMAMA_VAR { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public int MPJ_CALISMAMA_NEDEN_KOD_ID { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public double MPJ_CALISILMAYAN_SURE { get; set; }
		[DataMember]
        public int MPJ_CALISMA_TIP_ID { get; set; }
		[DataMember] 
        public int MPJ_SAYAC_ID { get; set; }
		[DataMember] 
        public double MPJ_SAYAC_BASLANGIC_DEGER { get; set; }
		[DataMember] 
        public double MPJ_SAYAC_BITIS_DEGER { get; set; }
		[DataMember] 
        public double MPJ_SAYAC_FARK { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public int MPJ_MAKINE_UCRET_TIP_ID { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public double MPJ_CALISMA_TIP_SURE_SAAT { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public int MPJ_DIGER_ISLEM_TUR_KOD_ID { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public double MPJ_DIGER_MIKTAR { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public int MPJ_DIGER_BIRIM_KOD_ID { get; set; }
		[DataMember]
        public int MPJ_VARDIYA_KOD_ID { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public int MPJ_OPERATOR_ID { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public int MPJ_FINANS_GELIRGIDER_ID { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_SURE_BASLANGIC_SAAT { get; set; }
		[DataMember] 
        public string MPJ_SURE_BITIS_SAAT { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_SURE_BEKLEME_SURE { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_SURE_DETAY_ACIKLAMA { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public double MPJ_PUANTAJ_DEGER { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_OPERATOR { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public bool MPJ_PROJE_ETKILE { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_OZEL_ALAN_1 { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_OZEL_ALAN_2 { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_OZEL_ALAN_3 { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_OZEL_ALAN_4 { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_OZEL_ALAN_5 { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public double MPJ_OZEL_ALAN_6 { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public double MPJ_OZEL_ALAN_7 { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public double MPJ_OZEL_ALAN_8 { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public double MPJ_OZEL_ALAN_9 { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public double MPJ_OZEL_ALAN_10 { get; set; }
		[DataMember]
        public int MPJ_CALISMA_YERI_KOD_ID { get; set; }
		[DataMember] 
        public string MPJ_SAAT { get; set; }
		[DataMember]
        public DateTime? MPJ_BITIS_TARIH { get; set; }
		[DataMember]
        public DateTime? MPJ_BASLANGIC_TARIH { get; set; }
		[DataMember] 
        public int MPJ_OLUSTURAN_ID { get; set; }
		[DataMember] 
        public DateTime? MPJ_OLUSTURMA_TARIH { get; set; }
		[DataMember] 
        public int MPJ_DEGISTIREN_ID { get; set; }
		[DataMember] 
        public DateTime? MPJ_DEGISTIRME_TARIH { get; set; }
		[DataMember]
        public int MPJ_CALISMA_TIPI { get; set; }
		[DataMember]
        public string MPJ_KOD { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_BELGE_NO { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public int MPJ_DURUM_KOD_ID { get; set; }
		[DataMember]
        public int MPJ_IS_TANIMI_KOD_ID { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public int MPJ_HESAPLAMA_TURU { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public int MPJ_SEFER_SAYISI { get; set; }
		[DataMember]
        public int MPJ_FIRMA_ID { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public int MPJ_FINANS_HAREKET_ID { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MKN_ARAC_PLAKA { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public int MPJ_FINANS_DURUM { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_IS_TANIMI_KOD_YONETIMI { get; set; }
		[DataMember]
        [Write(false)]
        [Computed]
        public string MPJ_FIRMA { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_LOKASYON { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_CARI { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_MAKINE_KOD { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_MAKINE_TANIM { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_MAKINE_TIPI { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_MAKINE_MODEL { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_MAKINE_MARKA { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_PERSONEL { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_PROJE_KOD { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_PROJE_TANIM { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_OPERATOR_TANIM { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_CALISMA_TIP_ACIKLAMA { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_CALISMA_TIP_TANIM { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_PROJE_IS_TANIM { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_IS_BIRIM_TANIM { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_CALISMAMA_NEDEN { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_CALISMAMA_KOD { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_IZIN { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_ISTANIM { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_VARDIYA { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_VARDIYA_TANIM { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_FINANS_GELIRGIDER { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_MAKINE_UCRET_TIP { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_DURUM { get; set; }
		[DataMember]
		[Write(false)]
		[Computed]
        public string MPJ_CALISMA_YERI { get; set; }
        [DataMember]
        [Write(false)]
        [Computed]
        public List<int> ResimIDleri
        {
            get
            {
                if (RSM_IDS == null)
                    return new List<int>();
                var ids = RSM_IDS.Split(';').ToList();
                var idsint = new List<int>();
                foreach (var i in ids)
                {
                    idsint.Add(Int32.Parse(i));
                }

                return idsint;
            }
            set { }
        }

        [Write(false)]
        [Computed]
        public string RSM_IDS { get; set; }
        [DataMember]
        [Write(false)]
        [Computed]
        [JsonProperty("ResimVarsayilanID")]
        public int RSM_VAR_ID { get; set; }

        [DataMember]
        [Write(false)]
        [Computed]
        public List<MakineCalismaOperator> opsToInsert { get; set; }

        [DataMember]
        [Write(false)]
        [Computed]
        public List<Dosya> BELGELER
        {
            get
            {
                var list=new List<Dosya>();
                if (DOCUMENTS == null)
                    return list;
                var ids = Regex.Split(DOCUMENTS, "~;~").ToList();
                foreach (var str in ids)
                {
                    var dosya=new Dosya();
                    var items = Regex.Split(str, @"\*:\*");
                    dosya.TB_DOSYA_ID = Convert.ToInt32(items[0]);
                    dosya.DSY_DOSYA_UZANTI = items[1];
                    dosya.DSY_TANIM = items[2];
                    list.Add(dosya);
                }
                return list;
            }
        }

        [Write(false)]
        [Computed]
        public string DOCUMENTS { get; set; }

        [DataMember]
        [Write(false)]
        [Computed]
        public List<MakineCalismaOperator> OPERATORLER
        {
            get
            {
                var list=new List<MakineCalismaOperator>();
                if (OPERATORS == null)
                    return list;
                var opts = Regex.Split(OPERATORS, "~;~").ToList();
                foreach (var str in opts)
                {
                    var makineCalismaOperator=new MakineCalismaOperator();
                    var items = Regex.Split(str, @"\*:\*");
                    makineCalismaOperator.TB_MAKINE_PUANTAJ_PERSONEL_ID = Convert.ToInt32(items[0]);
                    makineCalismaOperator.MPP_PERSONEL_ISIM = items[1];
                    double sure = 0;
                    Double.TryParse(items[2], out sure);
                    makineCalismaOperator.MPP_SURE_SAAT = sure;
                    int resimId = -1;
                    Int32.TryParse(items[3],out resimId);
                        makineCalismaOperator.MPP_RESIM_ID = resimId;
                    list.Add(makineCalismaOperator);
                }
                return list;
            }
        }

        [Write(false)]
        [Computed]
        public string OPERATORS { get; set; }



    }

    [DataContract]
    [Table("orjin.TB_MAKINE_PUANTAJ_PERSONEL")]
    public class MakineCalismaOperator
    {
        [DataMember]
        [Key]
        public long TB_MAKINE_PUANTAJ_PERSONEL_ID { get; set; }
        [DataMember]
        public int MPP_MAKINE_PUANTAJ_ID { get; set; }
        [DataMember]
        public int MPP_PERSONEL_ID { get; set; }
        [DataMember]
        [Write(false)]
        [Computed]
        public string MPP_PERSONEL_ISIM { get; set; }
        [DataMember]
        public DateTime? MPP_BASLAMA_TARIHI { get; set; }
        [DataMember]
        public string MPP_BASLAMA_ZAMANI { get; set; }
        [DataMember]
        public DateTime? MPP_BITIS_TARIHI { get; set; }
        [DataMember]
        public string MPP_BITIS_ZAMANI { get; set; }
        [DataMember]
        public double MPP_SURE_SAAT { get; set; }
        [DataMember]
        public string MPP_ACIKLAMA { get; set; }
        [DataMember]
        public int MPP_OLUSTURAN_ID { get; set; }
        [DataMember]
        public DateTime? MPP_OLUSTURMA_TARIH { get; set; }
        [DataMember]
        [Write(false)]
        [Computed]
        public int MPP_RESIM_ID { get; set; }
        [DataMember]
        [Write(false)]
        [Computed]
        public Personel MPP_PERSONEL { get; set; }
    }
}