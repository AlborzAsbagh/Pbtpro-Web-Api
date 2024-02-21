using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using WebApiNew.Models;

[DataContract]
public class WebDashboard
{
	[DataMember]
	public int ACIK_IS_EMIRLERI { get; set; }

	[DataMember]
	public int DEVAM_EDEN_IS_TALEPLERI { get; set; }

	[DataMember]
	public int DUSUK_STOKLU_MALZEMELER { get; set; }

	[DataMember]
	public int BEKLEYEN_MALZEME_TALEPLERI { get; set; }

	[DataMember]
	public List<MakineTipEnvanteri> MAKINE_TIP_ENVANTER { get; set; }

	public WebDashboard() 
	{
		MAKINE_TIP_ENVANTER = new List<MakineTipEnvanteri>();
	}
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
