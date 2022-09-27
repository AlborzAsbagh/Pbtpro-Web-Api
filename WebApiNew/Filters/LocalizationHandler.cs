using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using WebApiNew.App_GlobalResources;

namespace WebApiNew.Filters
{
    public class LocalizationHandler:DelegatingHandler
    {

        private readonly List<string> _supportedLanguages = new List<string> { "tr", "en", "ru" };

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            SetCulture(request);

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }
        
        private void SetCulture(HttpRequestMessage request)
        {
            foreach (var loopLanguage in request.Headers.AcceptLanguage)
            {
                // Desteklediğimiz dillerden biri var ise culture bilgisini o dile göre güncelliyoruz.
                if (_supportedLanguages.Contains(loopLanguage.Value))
                {
                    Thread.CurrentThread.CurrentCulture = new CultureInfo(loopLanguage.Value);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(loopLanguage.Value);

                    break;
                }
            }
        }


    }
}