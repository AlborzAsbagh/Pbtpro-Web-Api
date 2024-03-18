using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Microsoft.IdentityModel.Tokens;

namespace WebApiNew.Filters
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class JwtAuthenticationFilter : AuthorizationFilterAttribute
	{
		public override void OnAuthorization(HttpActionContext actionContext)
		{
			if (SkipAuthorization(actionContext)) return;

			var principal = ParseJwtToken(actionContext);
			if (principal == null)
			{
				actionContext.Response = actionContext.Request.CreateResponse(System.Net.HttpStatusCode.Unauthorized , new {status_code = 401 , 
					status = "Token has expired or is not valid ! "});
				return;
			}

			Thread.CurrentPrincipal = principal;
			if (HttpContext.Current != null)
			{
				HttpContext.Current.User = principal;
			}

			base.OnAuthorization(actionContext);
		}

		protected virtual ClaimsPrincipal ParseJwtToken(HttpActionContext actionContext)
		{
			var authHeader = actionContext.Request.Headers.Authorization;
			if (authHeader == null || authHeader.Scheme != "Bearer")
				return null;

			var token = authHeader.Parameter;
			if (string.IsNullOrEmpty(token))
				return null;

			try
			{
				var tokenHandler = new JwtSecurityTokenHandler();
				var validationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidIssuer = "PbtProIssuer", 
					ValidateAudience = true,
					ValidAudience = "PbtProAudience", 
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this is my custom Secret key for authentication")), 
					ClockSkew = TimeSpan.Zero
				};

				SecurityToken validatedToken;
				var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

				return principal;
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static bool SkipAuthorization(HttpActionContext actionContext)
		{
			return actionContext.ActionDescriptor.GetCustomAttributes<System.Web.Http.AllowAnonymousAttribute>().Any()
				   || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<System.Web.Http.AllowAnonymousAttribute>().Any();
		}

		public static string GetUserIdFromClaims()
		{
			var identity = HttpContext.Current.User.Identity as ClaimsIdentity;
			if (identity != null)
			{
				var userIdClaim = identity.FindFirst("User Id");
				if (userIdClaim != null)
				{
					return userIdClaim.Value;
				}
			}
			return null;
		}
	}
}
