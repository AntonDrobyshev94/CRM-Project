using FinalProject_Web.Interfaces;
using FinalProject_Web.Model;
using FinalProject_Web.Services.Interfaces;
using System.Text;
using System.Text.Json;
using FinalProject_Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_Web.Data
{
    public class ApplicationDataApi : IApplicationData
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authHelpServices;

        public ApplicationDataApi(IAuthService authHelpServices,
            HttpClient httpClient)
        {
            _httpClient = httpClient;
            _authHelpServices = authHelpServices;
        }

        #region Application
        /// <summary>
        /// Запрос на получение всех заявок, передающийся на API 
        /// сервер. Данный запрос принимает текущий Http-контекст, 
        /// который позволяет обратиться к куки, в которых хранится 
        /// токен. Запрос возвращает результат в виде коллекции
        /// объектов типа Application.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Application>> GetApplications(HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            string json = await _httpClient.GetStringAsync(url);
            return JsonHelper.Deserialize<IEnumerable<Application>>(json);
        }

        /// <summary>
        /// Запрос на создание новой заявки, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр заявки и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="application"></param>
        /// <param name="httpContext"></param>
        public async Task AddApplication(Application application, HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/values";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonSerializer.Serialize(application), Encoding.UTF8,
                mediaType: "application/json")
                );
            Console.WriteLine("Статус ответа: " + (int)r.StatusCode);
            Console.WriteLine("Ответ: " + await r.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Запрос на удаление заявки по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id заявки и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary> 
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public async Task DeleteApplication(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/values/{id}";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.DeleteAsync(
                requestUri: url);
            _authHelpServices.CheckStatus(r);
        }

        /// <summary>
        /// Запрос на изменение статуса заявки, передающийся 
        /// на API сервер. Данный запрос принимает строковую переменную,
        /// которая используется для создания нового экземпляра Application
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра Application. Данный метод является невозвратным.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        [HttpPost]
        public async Task ChangeApplicationStatus(string status, int id,
            HttpContext httpContext)
        {
            Application application = new Application()
            {
                Id = id,
                Status = status,
                Date = DateTime.Now,
                EMail = "@mail.ru",
                Message = "message",
                Name = "name"
            };

            string url = $"https://localhost:7037/api/values/ChangeStatus";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonSerializer.Serialize(application), Encoding.UTF8,
                mediaType: "application/json")
                );
            _authHelpServices.CheckStatus(r);
            Console.WriteLine("Статус ответа: " + (int)r.StatusCode);
            Console.WriteLine("Ответ: " + await r.Content.ReadAsStringAsync());
            #endregion
        }
    }
}
