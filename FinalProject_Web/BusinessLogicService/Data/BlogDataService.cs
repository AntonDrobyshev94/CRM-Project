using BusinessLogicService.Interfaces;
using ServicesLibrary.Services.Interfaces;
using ServicesLibrary.Helpers;
using Models.DataServiceModels;

namespace BusinessLogicService.Data
{
    public class BlogDataService : BaseDataService, IBlogData
    {
        public BlogDataService(HttpClient httpClient,
            IAuthService authService) : base(httpClient, authService)
        {}

        #region Blog
        /// <summary>
        /// Запрос на получение записей в блоге, передающийся на API 
        /// сервер. Запрос возвращает результат в виде коллекции
        /// объектов типа BlogModel.
        /// </summary>
        /// <param name="url"></param>
        public async Task<IEnumerable<BlogModel>> GetBlog(string url)
        {
            var blogModels = await GetAsynс<IEnumerable<BlogModel>>(url);
            return blogModels;
        }

        /// <summary>
        /// Запрос на создание записи в блоге, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр записи блога и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="blog"></param>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task AddBlog(CommonWithImageModel<BlogModel> blog, string token, 
            string url, string requsetId)
        {
            await PostAsync(url, blog, requsetId, token);
        }

        /// <summary>
        /// Запрос на изменение записи блога, передающийся 
        /// на API сервер. Данный запрос принимает строковые переменные,
        /// которые используются для создания нового экземпляра BlogModel
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра BlogModel. Данный метод является невозвратным.
        /// </summary>
        /// <param name="blog"></param>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task ChangeBlog(CommonWithImageModel<BlogModel> blog, string token, 
            string url, string requsetId)
        {
            await PostAsync(url, blog, requsetId, token);
        }

        /// <summary>
        /// Запрос на удаление записи в блоге по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id записи и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task DeleteBlog(string token, string url)
        {
            await DeleteAsync(url, token);
        }

        /// <summary>
        /// Запрос на поиск записи блога по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id записи и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос возвращает результат в 
        /// виде экземпляра BlogModel.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task<BlogModel> FindBlogById(string token, string url)
        {
            try
            {
                return await GetAsynс<BlogModel>(url, token);
            }
            catch { return null; }
        }
        #endregion
    }
}
