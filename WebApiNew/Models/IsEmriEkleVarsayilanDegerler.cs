using System.Runtime.Serialization;

namespace WebApiNew.Models
{
	[DataContract]
	public class IsEmriEkleVarsayilanDegerler
	{
		[DataMember]
		public int ID { get; set; }

		[DataMember]
		public string TANIM { get; set; }

		[DataMember]
		public string TABLO_TANIMI { get; set; }
	}
}