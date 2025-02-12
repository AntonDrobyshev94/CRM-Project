using FinalProject_Web.Services.Interfaces;
using System.Net;
using System.Net.Http.Headers;
namespace FinalProject_Web.Services
{
    public class AuthService : IAuthService
    {
        /// Метод добавления токена в заголовок запроса. Данный метод
        /// принимает в себя текущий HTTP-контекст (то есть текущий
        /// запрос), что в свою очередь позволяет обратиться к методу
        /// Request для вызова токена, сохраненного в куки.
        /// В методе происходит запись токена из куки и дальнейшее
        /// добавление токена в заголовок запроса с помощью 
        /// создания нового экземпляра заголовка аутентификации
        /// AuthenticationHeaderValue.
        public void AddTokenHeaderMethod(HttpContext httpContext, HttpClient httpClient)
        {
            string? tokenValue = httpContext.Request.Cookies["AuthToken"];
            if (!string.IsNullOrEmpty(tokenValue))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);
                Console.WriteLine($"{tokenValue}");
            }
        }

        public void CheckStatus(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException("User is not authorized.");
            }
        }
    }
}
