using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApiNew.Controllers;

namespace WebApiNew.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class BasicAuthenticationFilter : AuthorizationFilterAttribute
    {
        bool Active = true;

        public BasicAuthenticationFilter()
        { }

        /// <summary>
        /// Overriden constructor to allow explicit disabling of this
        /// filter's behavior. Pass false to disable (same as no filter
        /// but declarative)
        /// </summary>
        /// <param name="active"></param>
        public BasicAuthenticationFilter(bool active)
        {
            Active = active;
        }


        /// <summary>
        /// Override to Web API filter method to handle Basic Auth check
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (SkipAuthorization(actionContext)) return;
            if (Active)
            {
                var identity = ParseAuthorizationHeader(actionContext);
                if (identity == null)
                {
                    Challenge(actionContext);
                    return;
                }


                if (!OnAuthorizeUser(identity.Name, identity.Password, actionContext))
                {
                    Challenge(actionContext);
                    return;
                }

                var principal = new GenericPrincipal(identity, null);

                Thread.CurrentPrincipal = principal;

                // inside of ASP.NET this is required
                //if (HttpContext.Current != null)
                //    HttpContext.Current.User = principal;

                base.OnAuthorization(actionContext);
            }
        }

        /// <summary>
        /// Base implementation for user authentication - you probably will
        /// want to override this method for application specific logic.
        /// 
        /// The base implementation merely checks for username and password
        /// present and set the Thread principal.
        /// 
        /// Override this method if you want to customize Authentication
        /// and store user data as needed in a Thread Principle or other
        /// Request specific storage.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected virtual bool OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            return true;
        }

        /// <summary>
        /// Parses the Authorization header and creates user credentials
        /// </summary>
        /// <param name="actionContext"></param>
        protected virtual BasicAuthenticationIdentity ParseAuthorizationHeader(HttpActionContext actionContext)
        {
            string authHeader = null;
            var auth = actionContext.Request.Headers.Authorization;
            if (auth != null && auth.Scheme == "Basic")
                authHeader = auth.Parameter;

            if (string.IsNullOrEmpty(authHeader))
                return null;

            authHeader = Encoding.UTF8.GetString(Convert.FromBase64String(authHeader));

            var tokens = authHeader.Split(':');
            if (tokens.Length < 2)
                return null;

            return new BasicAuthenticationIdentity(tokens[0], tokens[1]);
        }


        /// <summary>
        /// Send the Authentication Challenge request
        /// </summary>
        /// <param name="message"></param>
        /// <param name="actionContext"></param>
        void Challenge(HttpActionContext actionContext)
        {
            var host = actionContext.Request.RequestUri.DnsSafeHost;
            actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", $"Basic realm=\"{host}\"");
        }
        private static bool SkipAuthorization(HttpActionContext actionContext)
        {
            Contract.Assert(actionContext != null);
            return actionContext.ActionDescriptor.GetCustomAttributes<System.Web.Http.AllowAnonymousAttribute>().Any()
                       || actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<System.Web.Http.AllowAnonymousAttribute>().Any();
        }

    }

    public class MyBasicAuthenticationFilter : BasicAuthenticationFilter
    {

        public MyBasicAuthenticationFilter()
        { }

        public MyBasicAuthenticationFilter(bool active) : base(active)
        { }


        protected override bool OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
        {
            var user= new LoginController().CheckUser(username,password);
            actionContext.ActionArguments.Add(C.KEY_USER,user);
            return user != null;
        }
    }
    public class BasicAuthenticationIdentity : GenericIdentity
    {
        public BasicAuthenticationIdentity(string name, string password)

:           base(name, "Basic")
        {
            this.Password = password;
        } /// 

        /// Basic Auth Password for custom authentication ///  
        public string Password { get; set; }
    }   

}