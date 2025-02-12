namespace FinalProject_Web.Services.Interfaces
{
    public interface ICookiesService
    {
        void SetCookies(string token, HttpContext httpContext);
        void SetAuthenticationCookies(string token, HttpContext httpContext);
        void ClearCookies(HttpContext httpContext);
        void LogoutMethod(HttpContext httpContext);
    }
}
