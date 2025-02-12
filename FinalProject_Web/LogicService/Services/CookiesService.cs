using FinalProject_Web.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace FinalProject_Web.Services
{
    public class CookiesService : ICookiesService
    {
        /// <summary>
        /// Метод установки куки и их сохранение
        /// </summary>
        /// <param name="token"></param>
        public void SetCookies(string token, HttpContext httpContext)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            httpContext.Response.Cookies.Append("AuthToken", token, cookieOptions);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
            if (role != null)
            {
                httpContext.Response.Cookies.Append("RoleCookie", role, cookieOptions);
            }

            var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            if (userName != null)
            {
                httpContext.Response.Cookies.Append("UserNameCookie", userName, cookieOptions);
            }
        }


        /// <summary>
        /// Метод установки куки
        /// </summary>
        /// <param name="token"></param>
        public void SetAuthenticationCookies(string token, HttpContext httpContext)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            httpContext.Response.Cookies.Append("AuthToken", token, cookieOptions);
            httpContext.Response.Cookies.Append("RoleCookie", "User", cookieOptions);

            var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
            if (!string.IsNullOrEmpty(userName))
            {
                httpContext.Response.Cookies.Append("UserNameCookie", userName, cookieOptions);
            }
        }

        /// <summary>
        /// Метод очистки куки
        /// </summary>
        public void ClearCookies(HttpContext httpContext)
        {
            httpContext.Response.Cookies.Delete("AuthToken");
            httpContext.Response.Cookies.Delete("RoleCookie");
            httpContext.Response.Cookies.Delete("UserNameCookie");
        }


        /// Метод выхода из учетной записи. В данном
        /// методе происходит удаление куки.
        public void LogoutMethod(HttpContext httpContext)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };

            httpContext.Response.Cookies.Delete("AuthToken");
            httpContext.Response.Cookies.Delete("RoleCookie");
            httpContext.Response.Cookies.Delete("UserNameCookie");
            httpContext.Response.Cookies.Delete("IsEditMode");
        }
    }
}
