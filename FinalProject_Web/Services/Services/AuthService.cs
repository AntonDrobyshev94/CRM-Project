using ServicesLibrary.Services.Interfaces;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ServicesLibrary.Services
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
        public void AddTokenHeaderMethod(HttpClient httpClient, string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine($"{token}");
            }
        }

        public void AddIdempotencyKeyHeader(HttpRequestMessage? request, string idempotencyKey)
        {
            if (request != null)
            {
                request.Headers.Add("Idempotency-Key", idempotencyKey);
            }
        }

        public void CheckStatus(HttpResponseMessage response)
        {
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnauthorizedAccessException();
                }
                else if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException();
                }
                else
                {
                    Console.WriteLine($"Произошла ошибка HTTP запроса: {ex.Message}");
                }
            }
        }
    }
}
