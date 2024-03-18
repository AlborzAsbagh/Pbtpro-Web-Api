using System;
using System.Data;
using System.Web.Http;
using WebApiNew.Models;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net.Http;
using System.Net;

namespace WebApiNew.Controllers
{
    public class LoginController : ApiController
    {
        Parametreler prms = new Parametreler();
        public HttpResponseMessage Post([FromBody]Kullanici gelenEntity)
        {
            var response = Request.CreateResponse(HttpStatusCode.Unauthorized, new { has_error = true, status_code = 401, status = "User Not Found !" });
			Kullanici entity = ValidateUser(gelenEntity);

			if (entity != null)
            {
				if(entity.TB_KULLANICI_ID > 0)
                {
					entity.AUTH_TOKEN = GenerateTokenForUser(entity.KLL_KOD, entity.TB_KULLANICI_ID);
					response = Request.CreateResponse(HttpStatusCode.OK, entity);
                    return response;
                }
                else
                {
					return response;
                }
            }
           else
            {
				return response;
			}
        }

		[Route("api/ValidateUser")]
		[HttpPost]
		public Kullanici ValidateUser(Kullanici gelenEntity)
        {
			Util klas = new Util();
			Kullanici entity = new Kullanici();
			try
			{
				prms.Clear();
				prms.Add("KLL_KOD", gelenEntity.KLL_KOD);
				klas.MasterBaglantisi = true;
				string query = "SELECT * FROM orjin.TB_KULLANICI WHERE KLL_AKTIF = 1 AND KLL_DURUM = 'K' AND KLL_KOD =@KLL_KOD";
				DataRow drKul = klas.GetDataRow(query, prms.PARAMS);

				if (drKul != null)
				{
					if ((drKul["KLL_SIFRE"] == DBNull.Value && gelenEntity.KLL_SIFRE == "") || drKul["KLL_SIFRE"].ToString() == gelenEntity.KLL_SIFRE)
					{
						entity.TB_KULLANICI_ID = Util.getFieldInt(drKul, "TB_KULLANICI_ID");
						entity.KLL_PERSONEL_ID = Util.getFieldInt(drKul, "KLL_PERSONEL_ID");
						entity.KLL_KOD = Util.getFieldString(drKul, "KLL_KOD");
						entity.KLL_AKTIF = Util.getFieldBool(drKul, "KLL_AKTIF");
						entity.KLL_TANIM = Util.getFieldString(drKul, "KLL_TANIM");
						entity.KLL_MAIL = Util.getFieldString(drKul, "KLL_MAIL");
						entity.apiVer = new indexController.AccessCheck().version;
						entity.dbName = klas.GetDbName();
						klas.MasterBaglantisi = false;
						prms.Clear();
						prms.Add("RSM_REF_ID", Util.getFieldInt(drKul, "KLL_PERSONEL_ID"));
						entity.resimId = DBNull.Value == drKul["KLL_PERSONEL_ID"] ? -1 : Convert.ToInt32(klas.GetDataCell("SELECT COALESCE(TB_RESIM_ID,-1) FROM orjin.TB_RESIM WHERE RSM_VARSAYILAN= 1 AND RSM_REF_GRUP = 'PERSONEL' AND RSM_REF_ID = @RSM_REF_ID", prms.PARAMS));
						string prsquery = @"SELECT * FROM orjin.VW_PERSONEL WHERE TB_PERSONEL_ID = @TB_PERSONEL_ID";
						var util = new Util();
						using (var cnn = util.baglan())
						{
							var personel = cnn.QueryFirstOrDefault<Personel>(prsquery, new { TB_PERSONEL_ID = entity.KLL_PERSONEL_ID });
							entity.KLL_PERSONEL = personel;
						}
					}
					else
						return entity;
				}
				else
					return entity;

				klas.MasterBaglantisi = false;
				return entity;
			}
			catch (Exception e)
			{
				klas.MasterBaglantisi = false;
				return entity;
			}
		}

		[Route("api/GenerateTokenForUser")]
		[HttpGet]
		private string GenerateTokenForUser(string username , int userId)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes("this is my custom Secret key for authentication"); 
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
			    new Claim("User Name", username),
			    new Claim("User Id", Convert.ToString(userId))
				}),
				Expires = DateTime.UtcNow.AddHours(24),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
				Issuer = "PbtProIssuer", 
				Audience = "PbtProAudience" 
			};
			SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
