using BusinessLogicService.Interfaces;
using ServicesLibrary.Services.Interfaces;
using Models.DataServiceModels;
using ServicesLibrary.Enums;
using System.Net;

namespace BusinessLogicService.Data
{
    public class ContactDataService : BaseDataService, IContactData
    {
        public ContactDataService(HttpClient httpClient,
            IAuthService authService) : base(httpClient, authService)
        {}

        #region Contacts
        /// <summary>
        /// Запрос на получение контакта, передающийся на API 
        /// сервер. Запрос возвращает результат в виде экземпляра
        /// объекта типа Contacts.
        /// </summary>
        /// <param name="url"></param>
        public async Task<Contacts> GetContacts(string url)
        {
            var contacts = await GetAsynс<Contacts>(url);
            return contacts;
        }

        /// <summary>
        /// Запрос на изменение контактов, передающийся 
        /// на API сервер. Данный запрос принимает модель Contacts,
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра Contacts. Данный метод является невозвратным.
        /// </summary>
        /// <param name="contacts"></param>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task<ChangeContactsResult> ChangeContacts(Contacts contacts, string token,  string url)
        {
            try
            {
                var r = await PostAsync(url, contacts, token);
                switch (r.StatusCode)
                {
                    case HttpStatusCode.OK:
                        return ChangeContactsResult.Ok;
                    case HttpStatusCode.Unauthorized:
                        return ChangeContactsResult.Unauthorized;
                    case HttpStatusCode.Forbidden:
                        return ChangeContactsResult.Unauthorized;
                    default:
                        return ChangeContactsResult.BadRequest;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return ChangeContactsResult.BadRequest;
            }
        }
        #endregion
    }
}
