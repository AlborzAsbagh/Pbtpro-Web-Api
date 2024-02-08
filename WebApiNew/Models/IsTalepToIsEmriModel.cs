
using System.Collections.Generic;
using System.Runtime.Serialization;

public class IsTalepToIsEmriModel
{

	[DataMember]
	public int TALEP_ID { get; set; }

	[DataMember]
	public int USER_ID { get; set; }

	[DataMember]
	public int ATOLYE_ID { get; set; }

	[DataMember]
	public List<int> TEKNISYEN_IDS { get; set; }

}





