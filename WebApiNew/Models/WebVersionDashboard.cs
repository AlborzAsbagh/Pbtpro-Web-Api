using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WebApiNew.Models;

[DataContract]
public class WebDashboardCards
{
	[DataMember]
	public int ACIK_IS_EMIRLERI { get; set; }

	[DataMember]
	public int DEVAM_EDEN_IS_TALEPLERI { get; set; }

	[DataMember]
	public int DUSUK_STOKLU_MALZEMELER { get; set; }

	[DataMember]
	public int MAKINE_SAYISI { get; set; }
}

public class MakineTipEnvanteri 
{
	[DataMember]
	public int TB_KOD_ID { get; set; }

	[DataMember]
	public string MAKINE_TIPI { get; set; }

	[DataMember]
	public int MAKINE_SAYISI { get; set; }

	public MakineTipEnvanteri(int TB_KOD_ID , string MAKINE_TIPI , int MAKINE_SAYISI)
	{
		this.TB_KOD_ID = TB_KOD_ID;
		this.MAKINE_TIPI = MAKINE_TIPI;
		this.MAKINE_SAYISI = MAKINE_SAYISI;
	}
}

public class IsTalebiTipi
{
	[DataMember]
	public int TB_KOD_ID { get; set; }

	[DataMember]
	public string TALEP_TIPI { get; set; }

	[DataMember]
	public int TALEP_SAYISI { get; set; }

	public IsTalebiTipi(int TB_KOD_ID, string TALEP_TIPI, int TALEP_SAYISI)
	{
		this.TB_KOD_ID = TB_KOD_ID;
		this.TALEP_TIPI = TALEP_TIPI;
		this.TALEP_SAYISI = TALEP_SAYISI;
	}
}

public class IsEmriTipi
{
	[DataMember]
	public int TB_ISEMRI_TIP_ID { get; set; }

	[DataMember]
	public string ISEMRI_TIPI { get; set; }

	[DataMember]
	public int ISEMRI_SAYISI { get; set; }

	public IsEmriTipi(int TB_ISEMRI_TIP_ID, string ISEMRI_TIPI, int ISEMRI_SAYISI)
	{
		this.TB_ISEMRI_TIP_ID = TB_ISEMRI_TIP_ID;
		this.ISEMRI_TIPI = ISEMRI_TIPI;
		this.ISEMRI_SAYISI = ISEMRI_SAYISI;
	}
}

public class IsEmriDurumu
{
	[DataMember]
	public int TB_KOD_ID { get; set; }

	[DataMember]
	public string ISEMRI_DURUMU { get; set; }

	[DataMember]
	public int ISEMRI_SAYISI { get; set; }

	public IsEmriDurumu(int TB_KOD_ID, string ISEMRI_DURUMU, int ISEMRI_SAYISI)
	{
		this.TB_KOD_ID = TB_KOD_ID;
		this.ISEMRI_DURUMU = ISEMRI_DURUMU;
		this.ISEMRI_SAYISI = ISEMRI_SAYISI;
	}
}

public class IsTalepDurumu
{
	[DataMember]
	public int IST_DURUM_ID { get; set; }

	[DataMember]
	public int IS_TALEP_SAYISI { get; set; }

	public IsTalepDurumu(int IST_DURUM_ID, int IS_TALEP_SAYISI)
	{
		this.IST_DURUM_ID = IST_DURUM_ID;
		this.IS_TALEP_SAYISI = IS_TALEP_SAYISI;
	}
}

public class AylikBakimIsEmrileri
{
	[DataMember]
	public int AY { get; set; }

	[DataMember]
	public int AYLIK_BAKIM_ISEMRI_MALIYET { get; set; }

	public AylikBakimIsEmrileri(int AY, int AYLIK_BAKIM_ISEMRI_MALIYET)
	{
		this.AY = AY;
		this.AYLIK_BAKIM_ISEMRI_MALIYET = AYLIK_BAKIM_ISEMRI_MALIYET;
	}
}

public class TamamlananIsEmrileriIsTalepleri
{
	[DataMember]
	public int AY { get; set; }

	[DataMember]
	public int DEGER { get; set; }

	[DataMember]
	public string TIP { get; set; }

	public TamamlananIsEmrileriIsTalepleri(int aY, int dEGER, string tIP)
	{
		AY = aY;
		DEGER = dEGER;
		TIP = tIP;
	}
}

public class IsEmriByTarih
{
	[DataMember]
	public DateTime? TARIH { get; set; }

	[DataMember]
	public int DEGER { get; set; }

	public IsEmriByTarih(DateTime TARIH, int DEGER)
	{
		this.TARIH = TARIH;
		this.DEGER = DEGER;
	}
}

public class PersonelBazindaHarcananGuc
{
	[DataMember]
	public string ISIM { get; set; }

	[DataMember]
	public int DAKIKA { get; set; }

	public PersonelBazindaHarcananGuc(string ISIM, int DAKIKA)
	{
		this.ISIM = ISIM;
		this.DAKIKA = DAKIKA;
	}
}

public class ToplamHarcananIsGuc
{
	[DataMember]
	public string TANIM { get; set; }

	[DataMember]
	public int DAKIKA { get; set; }

	public ToplamHarcananIsGuc(string TANIM, int DAKIKA)
	{
		this.TANIM = TANIM;
		this.DAKIKA = DAKIKA;
	}
}

public class ArizaliMakineler
{
	[DataMember]
	public string MAKINE_KODU { get; set; }

	[DataMember]
	public string MAKINE_TANIMI { get; set; }

	[DataMember]
	public string MAKINE_TIPI { get; set; }

	[DataMember]
	public string LOKASYON { get; set; }

	[DataMember]
	public int IS_EMRI_SAYISI { get; set; }	

	public ArizaliMakineler(string MAKINE_KODU, string MAKINE_TANIMI, string MAKINE_TIPI , string LOKASYON , int IS_EMRI_SAYISI)
	{
		this.MAKINE_KODU = MAKINE_KODU;
		this.MAKINE_TANIMI = MAKINE_TANIMI;
		this.MAKINE_TIPI = MAKINE_TIPI;
		this.LOKASYON = LOKASYON;
		this.IS_EMRI_SAYISI = IS_EMRI_SAYISI;
	}
}

public class IsEmriOzetTable
{
	[DataMember]
	public string IS_EMRI_TIPI { get; set; }

	[DataMember]
	public int IS_EMRI_SAYISI { get; set; }

	[DataMember]
	public int TOPLAM_MALIYET { get; set; }

	[DataMember]
	public int TOPLAM_CALISMA_SURESI { get; set; }

	public IsEmriOzetTable(string IS_EMRI_TIPI, int IS_EMRI_SAYISI, int TOPLAM_MALIYET, int TOPLAM_CALISMA_SURESI)
	{
		this.IS_EMRI_TIPI = IS_EMRI_TIPI;
		this.IS_EMRI_SAYISI = IS_EMRI_SAYISI;
		this.TOPLAM_MALIYET = TOPLAM_MALIYET;
		this.TOPLAM_CALISMA_SURESI = TOPLAM_CALISMA_SURESI;
	}
}

public class LokasyonBazindaIsEmriTalebi
{
	[DataMember]
	public string LOKASYON { get; set; }

	[DataMember]
	public int ID { get; set; }

	[DataMember]
	public int TOPLAM_IS_EMRI { get; set; }

	[DataMember]
	public int TOPLAM_IS_TALEBI { get; set; }

	public LokasyonBazindaIsEmriTalebi(string LOKASYON, int ID, int TOPLAM_IS_EMRI, int TOPLAM_IS_TALEBI)
	{
		this.LOKASYON = LOKASYON;
		this.ID = ID;
		this.TOPLAM_IS_EMRI = TOPLAM_IS_EMRI;
		this.TOPLAM_IS_TALEBI = TOPLAM_IS_TALEBI;
	}
}