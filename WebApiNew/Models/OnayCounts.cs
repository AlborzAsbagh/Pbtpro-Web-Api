using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiNew.Models
{
    public class OnayCounts
    {
        public int MKN_TRANSFER_ONAY {get;set;}
        public int MLZ_TRANSFER_ONAY {get;set;}
        public int YKT_TRANSFER_ONAY {get;set;}
        public int TOTAL => MKN_TRANSFER_ONAY + MLZ_TRANSFER_ONAY + YKT_TRANSFER_ONAY;
    }
}