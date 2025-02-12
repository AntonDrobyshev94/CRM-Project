using FinalProject_Web.Helpers;
using FinalProject_Web.Model;
using System.Text.Json;
using System.Text;
using FinalProject_Web.Services.Interfaces;
using FinalProject_Web.Interfaces;

namespace FinalProject_Web.Data
{
    public class LinkDataService: ILinkData
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authHelpServices;
        public LinkDataService(HttpClient httpClient,
            IAuthService authHelpServices)
        {
            _httpClient = httpClient;
            _authHelpServices = authHelpServices;
        }
        #region Links
        /// <summary>
        /// Запрос на получение всех ссылок, передающийся на API 
        /// сервер. Запрос возвращает результат в виде коллекции
        /// объектов типа LinkModel.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<LinkModel>> GetLinks()
        {
            string url = @"https://localhost:7037/api/link";
            string json = await _httpClient.GetStringAsync(url);
            return JsonHelper.Deserialize<IEnumerable<LinkModel>>(json);
        }

        /// <summary>
        /// Запрос на создание новой ссылки, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр ссылки и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="link"></param>
        /// <param name="httpContext"></param>
        public async Task AddLink(CommonWithImageModel<LinkModel> link, HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/link";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonSerializer.Serialize(link), Encoding.UTF8,
                mediaType: "application/json")
                );
            _authHelpServices.CheckStatus(r);
            Console.WriteLine("Статус ответа: " + (int)r.StatusCode);
            Console.WriteLine("Ответ: " + await r.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Запрос на изменение ссылки, передающийся 
        /// на API сервер. Данный запрос принимает строковые переменные,
        /// которые используются для создания нового экземпляра LinkModel
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра LinkModel. Данный метод является невозвратным.
        /// </summary>
        /// <param name="link"></param>
        /// <param name="httpContext"></param>
        public async Task ChangeLink(CommonWithImageModel<LinkModel> link, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/link/ChangeLink";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonSerializer.Serialize(link), Encoding.UTF8,
                mediaType: "application/json")
                );
            _authHelpServices.CheckStatus(r);
            Console.WriteLine("Статус ответа: " + (int)r.StatusCode);
            Console.WriteLine("Ответ: " + await r.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Запрос на удаление ссылки по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id ссылки и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public async Task DeleteLink(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/link/{id}";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.DeleteAsync(
                requestUri: url);
            _authHelpServices.CheckStatus(r);
        }

        /// <summary>
        /// Запрос на поиск ссылки по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id ссылки и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос возвращает результат в 
        /// виде экземпляра LinkModel.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task<LinkModel> FindLinkById(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/link/{id}";
            try
            {
                string json = await _httpClient.GetStringAsync(url);
                return JsonHelper.Deserialize<LinkModel>(json);
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
