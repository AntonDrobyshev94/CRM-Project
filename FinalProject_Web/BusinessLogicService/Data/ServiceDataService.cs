using BusinessLogicService.Interfaces;
using ServicesLibrary.Services.Interfaces;
using Models.DataServiceModels;

namespace BusinessLogicService.Data
{
    public class ServiceDataService :BaseDataService, IServiceData
    {
        public ServiceDataService(HttpClient httpClient,
            IAuthService authService) : base(httpClient, authService)
        {}

        #region Services
        /// <summary>
        /// Запрос на получение всех услуг, передающийся на API 
        /// сервер. Запрос возвращает результат в виде коллекции
        /// объектов типа Service.
        /// </summary>
        /// <param name="url"></param>
        public async Task<IEnumerable<Service>> GetServices(string url)
        {
            return await GetAsynс<IEnumerable<Service>>(url);
        }

        /// <summary>
        /// Запрос на создание новой услуги, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр услуги и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task AddService(Service service, string token, string url)
        {
            await PostAsync(url, service, token);
        }

        /// <summary>
        /// Запрос на изменение услуги, передающийся 
        /// на API сервер. Данный запрос принимает строковые переменные,
        /// которые используются для создания нового экземпляра Service
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра Service. Данный метод является невозвратным.
        /// </summary>
        /// <param name="service"></param>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task ChangeService(Service service, string token, string url)
        {
            await PostAsync(url, service, token);
        }

        /// <summary>
        /// Запрос на удаление услуги по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id услуги и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task DeleteService(string token, string url)
        {
            await DeleteAsync(url, token);
        }

        /// <summary>
        /// Запрос на поиск услуги по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id услуги и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос возвращает результат в 
        /// виде экземпляра Service.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task<Service> FindServiceById(string token, string url)
        {
            try
            {
                return await GetAsynс<Service>(url, token);
            }
            catch { return null; }
        }
        #endregion
    }
}
