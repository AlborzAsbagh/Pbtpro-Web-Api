using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Http;
using Dapper;
using WebApiNew.Filters;
using WebApiNew.Models;

namespace WebApiNew.Controllers
{
    
    [MyBasicAuthenticationFilter]
    public class IsEmriTipController : ApiController
    {

        public List<IsEmriTip> Get()
        {
            string query = @"select * from orjin.TB_ISEMRI_TIP WHERE IMT_AKTIF = 1";
            var util = new Util();
            using (var cnn=util.baglan())
            {
                var liste = cnn.Query<IsEmriTip>(query).ToList();
                return liste;
            }
        }
        [Route("api/IsEmriTipVarsayilan")]
        [HttpGet]
        public IsEmriTip IsEmriTipVarsayilan()
        {
            return this.Get().FirstOrDefault(a => a.IMT_VARSAYILAN == true);
        }

    }
}
