using BusinessLogicService.Interfaces;
using ServicesLibrary.Services.Interfaces;
using Models.DataServiceModels;

namespace BusinessLogicService.Data
{
    public class LinkDataService: BaseDataService, ILinkData
    {
        public LinkDataService(HttpClient httpClient,
            IAuthService authService) : base(httpClient, authService)
        {}
        #region Links
        /// <summary>
        /// Запрос на получение всех ссылок, передающийся на API 
        /// сервер. Запрос возвращает результат в виде коллекции
        /// объектов типа LinkModel.
        /// </summary>
        /// <param name="url"></param>
        public async Task<IEnumerable<LinkModel>> GetLinks(string url)
        {
            var linkModels = await GetAsynс<IEnumerable<LinkModel>>(url);
            return linkModels;
        }

        /// <summary>
        /// Запрос на создание новой ссылки, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр ссылки и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="link"></param>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task AddLink(CommonWithImageModel<LinkModel> link, string token, string url)
        {
            await PostAsync(url, link, token);
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
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task ChangeLink(CommonWithImageModel<LinkModel> link, string token, string url)
        {
            await PostAsync(url, link, token);
        }

        /// <summary>
        /// Запрос на удаление ссылки по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id ссылки и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task DeleteLink(string token, string url)
        {
            await DeleteAsync(url, token);
        }

        /// <summary>
        /// Запрос на поиск ссылки по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id ссылки и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос возвращает результат в 
        /// виде экземпляра LinkModel.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<LinkModel> FindLinkById(string token, string url)
        {
            try
            {
                return await GetAsynс<LinkModel>(url);
            }
            catch { return null; }
        }
        #endregion
    }
}
