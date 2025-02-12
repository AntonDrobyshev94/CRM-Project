using FinalProject_Web.Helpers;
using FinalProject_Web.Model;
using System.Text.Json;
using System.Text;
using FinalProject_Web.Services.Interfaces;
using FinalProject_Web.Interfaces;
using FinalProject_Web.Enums;
using System.Net;

namespace FinalProject_Web.Data
{
    public class ContactDataService : IContactData
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authHelpServices;
        public ContactDataService(IAuthService authHelpServices,
            HttpClient httpClient)
        {
            _httpClient = httpClient;
            _authHelpServices = authHelpServices;
        }

        #region Contacts
        /// <summary>
        /// Запрос на получение контакта, передающийся на API 
        /// сервер. Запрос возвращает результат в виде экземпляра
        /// объекта типа Contacts.
        /// </summary>
        /// <returns></returns>
        public async Task<Contacts> GetContacts()
        {
            string url = @"https://localhost:7037/api/contact";
            string json = await _httpClient.GetStringAsync(url);
            return JsonHelper.Deserialize<Contacts>(json);
        }

        /// <summary>
        /// Запрос на изменение контактов, передающийся 
        /// на API сервер. Данный запрос принимает модель Contacts,
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра Contacts. Данный метод является невозвратным.
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="httpContext"></param>
        public async Task<ChangeContactsResult> ChangeContacts(Contacts contacts, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/contact";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonSerializer.Serialize(contacts), Encoding.UTF8,
                mediaType: "application/json")
                );
            _authHelpServices.CheckStatus(r);
            Console.WriteLine("Статус ответа: " + (int)r.StatusCode);
            Console.WriteLine("Ответ: " + await r.Content.ReadAsStringAsync());
            switch (r.StatusCode)
            {
                case HttpStatusCode.OK:
                    return ChangeContactsResult.Ok;
                case HttpStatusCode.Unauthorized:
                    return ChangeContactsResult.Unauthorized;
                default:
                    return ChangeContactsResult.BadRequest;
            }
        }
        #endregion
    }
}
