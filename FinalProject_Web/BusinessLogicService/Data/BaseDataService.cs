using System.Text.Json;
using System.Text;
using ServicesLibrary.Services.Interfaces;
using ServicesLibrary.Helpers;

namespace BusinessLogicService.Data
{
    public abstract class BaseDataService
    {
        protected readonly HttpClient _httpClient;
        private readonly IAuthService _authService;
        protected BaseDataService(HttpClient httpClient,
            IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        /// <summary>
        /// Базовый метод отправки Post запроса с возвращаемой
        /// моделью
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        protected async Task<T> PostAsync<T>(string url, object content)
        {
            var request = new HttpRequestMessage(method: HttpMethod.Post, requestUri: url)
            {
                Content = new StringContent(
               content: JsonSerializer.Serialize(content),
               encoding: Encoding.UTF8,
               mediaType: "application/json")
            };
            var response = await _httpClient.SendAsync(request);
            _authService.CheckStatus(response);
            string json = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json);
            return JsonHelper.Deserialize<T>(json);
        }

        /// <summary>
        /// Базовый метод для Post запроса возвращающий 
        /// статусное сообщение
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> PostAsync(string url, object content, 
            string idempotencyKey, string token)
        {
            AddTokenHeader(token);
            var request = new HttpRequestMessage(method:HttpMethod.Post, requestUri:url)
            {
                Content = new StringContent(
                content:JsonSerializer.Serialize(content),
                encoding:Encoding.UTF8,
                mediaType:"application/json")
            };
            _authService.AddIdempotencyKeyHeader(request, idempotencyKey);
            var response = await _httpClient.SendAsync(request);
            _authService.CheckStatus(response);
            string json = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json);
            return response;
        }

        /// <summary>
        /// Базовый метод для Post запроса возвращающий 
        /// статусное сообщение
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> PostAsync(string url, object content,
            string token)
        {
            AddTokenHeader(token);
            var request = new HttpRequestMessage(method: HttpMethod.Post, requestUri: url)
            {
                Content = new StringContent(
                content: JsonSerializer.Serialize(content),
                encoding: Encoding.UTF8,
                mediaType: "application/json")
            };
            var response = await _httpClient.SendAsync(request);
            _authService.CheckStatus(response);
            string json = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json);
            return response;
        }

        /// <summary>
        /// Базовый метод для Post запроса, возвращающий 
        /// статусное сообщение
        /// </summary>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        protected async Task<HttpResponseMessage> PostAsync(string url, MultipartFormDataContent content,
            string token)
        {
            AddTokenHeader(token);
            
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = content
            };

            var response = await _httpClient.SendAsync(request);
            _authService.CheckStatus(response);
            string json = await response.Content.ReadAsStringAsync();
            Console.WriteLine(json);
            return response;
        }

        /// <summary>
        /// Базовый метод Get запроса с возвращаемой моделью
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        protected async Task<T> GetAsynс<T>(string url)
        {
            var response = await _httpClient.GetStringAsync(
                requestUri: url);
            return JsonHelper.Deserialize<T>(response);
        }

        /// <summary>
        /// Базовый метод Get запроса с возвращаемой моделью
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        protected async Task<T> GetAsynс<T>(string url, string token)
        {
            AddTokenHeader(token);
            var response = await _httpClient.GetStringAsync(
                requestUri: url);
            return JsonHelper.Deserialize<T>(response);
        }

        /// <summary>
        /// Базовый метод Delete запроса
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        protected async Task DeleteAsync(string url, string token)
        {
            AddTokenHeader(token);
            var response = await _httpClient.DeleteAsync(
                requestUri: url);
            _authService.CheckStatus(response);
        }
        
        /// <summary>
        /// Добавление токена в заголовок запроса
        /// </summary>
        /// <param name="token"></param>
        protected void AddTokenHeader(string token)
        {
            _authService.AddTokenHeaderMethod(_httpClient, token);
        }
    }
}
