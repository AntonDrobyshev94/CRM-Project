using FinalProject_Web.Helpers;
using FinalProject_Web.Model;
using FinalProject_Web.Interfaces;
using System.Text.Json;
using System.Text;
using FinalProject_Web.Services.Interfaces;

namespace FinalProject_Web.Data
{
    public class ServiceDataService : IServiceData
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authHelpServices;
        public ServiceDataService(HttpClient httpClient,
            IAuthService authenticationHelpService)
        {
            _httpClient = httpClient;
            _authHelpServices = authenticationHelpService;
        }

        #region Services
        /// <summary>
        /// Запрос на получение всех услуг, передающийся на API 
        /// сервер. Запрос возвращает результат в виде коллекции
        /// объектов типа Service.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Service>> GetServices()
        {
            string url = @"https://localhost:7037/api/service";
            string json = await _httpClient.GetStringAsync(url);
            return JsonHelper.Deserialize<IEnumerable<Service>>(json);
        }

        /// <summary>
        /// Запрос на создание новой услуги, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр услуги и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="httpContext"></param>
        public async Task AddService(Service service, HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/service";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonSerializer.Serialize(service), Encoding.UTF8,
                mediaType: "application/json")
                );
            _authHelpServices.CheckStatus(r);
            Console.WriteLine("Статус ответа: " + (int)r.StatusCode);
            Console.WriteLine("Ответ: " + await r.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Запрос на изменение услуги, передающийся 
        /// на API сервер. Данный запрос принимает строковые переменные,
        /// которые используются для создания нового экземпляра Service
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра Service. Данный метод является невозвратным.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public async Task ChangeService(string name, string description,
            int id, HttpContext httpContext)
        {
            Service service = new Service()
            {
                Id = id,
                Description = description,
                Name = name
            };

            string url = $"https://localhost:7037/api/service/ChangeService";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonSerializer.Serialize(service), Encoding.UTF8,
                mediaType: "application/json")
                );
            _authHelpServices.CheckStatus(r);
            Console.WriteLine("Статус ответа: " + (int)r.StatusCode);
            Console.WriteLine("Ответ: " + r.Content.ReadAsStringAsync().Result);
        }

        /// <summary>
        /// Запрос на удаление услуги по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id услуги и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public async Task DeleteService(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/service/{id}";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.DeleteAsync(
                requestUri: url);
            _authHelpServices.CheckStatus(r);
        }

        /// <summary>
        /// Запрос на поиск услуги по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id услуги и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос возвращает результат в 
        /// виде экземпляра Service.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task<Service> FindServiceById(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/service/{id}";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            try
            {
                string json = await _httpClient.GetStringAsync(url);
                return JsonHelper.Deserialize<Service>(json);
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
