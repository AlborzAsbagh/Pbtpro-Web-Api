using System;
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

}
