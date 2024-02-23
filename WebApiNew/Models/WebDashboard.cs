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
