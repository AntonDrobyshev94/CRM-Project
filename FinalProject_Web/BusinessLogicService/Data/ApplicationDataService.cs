using BusinessLogicService.Interfaces;
using ServicesLibrary.Services.Interfaces;
using Models.DataServiceModels;

namespace BusinessLogicService.Data
{
    public class ApplicationDataService : BaseDataService, IApplicationData
    {
        public ApplicationDataService(HttpClient httpClient,
            IAuthService authService): base(httpClient, authService)
        {}

        #region Application
        /// <summary>
        /// Запрос на получение всех заявок, передающийся на API 
        /// сервер. Данный запрос принимает текущий Http-контекст, 
        /// который позволяет обратиться к куки, в которых хранится 
        /// токен. Запрос возвращает результат в виде коллекции
        /// объектов типа Application.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task<IEnumerable<Application>> GetApplications(string token, string url)
        {
            AddTokenHeader(token);
            var applications = await GetAsynс<IEnumerable<Application>>(url, token);
            return applications;
        }

        /// <summary>
        /// Запрос на создание новой заявки, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр заявки и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="application"></param>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task AddApplication(Application application, string token, 
            string url, string requestId)
        {
            await PostAsync(url, application, requestId);
        }

        /// <summary>
        /// Запрос на удаление заявки по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id заявки и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary> 
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task DeleteApplication(string token, string url)
        {
            await DeleteAsync(url, token);
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
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task ChangeApplicationStatus(string status, int id,
            string token, string url)
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
            await PostAsync(url, application, token);
            #endregion
        }
    }
}
