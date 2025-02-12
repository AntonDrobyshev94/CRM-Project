using ServicesLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FinalProject_Web.Filters
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CheckTokenAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => true;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var authService = serviceProvider.GetRequiredService<IAuthService>();
            return new CheckTokenFilter(httpClientFactory, authService);
        }
    }
}
