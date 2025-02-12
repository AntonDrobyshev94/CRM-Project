namespace FinalProject_Web.Services.Interfaces
{
    public interface IAuthService
    {
        void AddTokenHeaderMethod(HttpContext httpContext, HttpClient httpClient);
        void CheckStatus(HttpResponseMessage response);
    }
}
