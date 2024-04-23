using System;
using System.Runtime.Serialization;

namespace WebApiNew.Models
{
	[DataContract]
	public class IsEmriTabsCount
	{

		[DataMember]
		public int IsEmriKontrolListSayisi { get; set; }

		[DataMember]
		public int IsEmriPersonelListSayisi { get; set; }

		[DataMember]
		public int IsEmriDurusListSayisi { get; set; }

		[DataMember]
		public int IsEmriMalzemeListSayisi { get; set; }

		[DataMember]
		public int IsEmriOlcumListSayisi { get; set; }

		[DataMember]
		public int IsEmriAracGerevListSayisi { get; set; }

		[DataMember]
		public int IsEmriResimSayisi { get; set; }

		[DataMember]
		public int IsEmriDosyaSayisi { get; set; }

		[DataMember]
		public bool IsEmriNotVar { get; set; }

		[DataMember]
		public bool IsEmriAciklamaVar { get; set; }



	}

}