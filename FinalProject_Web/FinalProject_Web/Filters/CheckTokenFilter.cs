using ServicesLibrary.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FinalProject_Web.Filters
{
    public class CheckTokenFilter : IAsyncActionFilter
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAuthService _authService;

        public CheckTokenFilter(IHttpClientFactory httpClientFactory, IAuthService authService)
        {
            _httpClientFactory = httpClientFactory;
            _authService = authService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var attribute = context.ActionDescriptor.EndpointMetadata
                                .OfType<ApiEndpointAttribute>()
                                .FirstOrDefault();
            if (attribute != null)
            {
                string url = $"https://localhost:7037{attribute.Endpoint}";
                var httpClient = _httpClientFactory.CreateClient();
                string token = context.HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
                _authService.AddTokenHeaderMethod(httpClient, token);
                var response = await httpClient.GetAsync(url);
                _authService.CheckStatus(response);
            }

            await next(); 
        }
    }
}
