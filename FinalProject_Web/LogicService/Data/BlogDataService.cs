using FinalProject_Web.Helpers;
using FinalProject_Web.Model;
using System.Text.Json;
using System.Text;
using FinalProject_Web.Services.Interfaces;
using FinalProject_Web.Interfaces;

namespace FinalProject_Web.Data
{
    public class BlogDataApi : IBlogData
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authHelpServices;
        public BlogDataApi(HttpClient httpClient,
            IAuthService authHelpService)
        {
            _httpClient = httpClient;
            _authHelpServices = authHelpService;
        }

        #region Blog
        /// <summary>
        /// Запрос на получение записей в блоге, передающийся на API 
        /// сервер. Запрос возвращает результат в виде коллекции
        /// объектов типа BlogModel.
        /// </summary>
        public async Task<IEnumerable<BlogModel>> GetBlog()
        {
            string url = @"https://localhost:7037/api/blog";
            string json = await _httpClient.GetStringAsync(url);
            return JsonHelper.Deserialize<IEnumerable<BlogModel>>(json);
        }

        /// <summary>
        /// Запрос на создание записи в блоге, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр записи блога и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="blog"></param>
        /// <param name="httpContext"></param>
        public async Task AddBlog(CommonWithImageModel<BlogModel> blog, HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/blog";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.PostAsync(
                    requestUri: url,
                    content: new StringContent(JsonSerializer.Serialize(blog), Encoding.UTF8,
                    mediaType: "application/json")
                    );
            _authHelpServices.CheckStatus(r);
            Console.WriteLine("Статус ответа: " + (int)r.StatusCode);
            Console.WriteLine("Ответ: " + await r.Content.ReadAsStringAsync());
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
        /// <param name="httpContext"></param>
        public async Task ChangeBlog(CommonWithImageModel<BlogModel> blog, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/blog/ChangeBlog";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonSerializer.Serialize(blog), Encoding.UTF8,
                mediaType: "application/json")
                );
            _authHelpServices.CheckStatus(r);
            Console.WriteLine("Статус ответа: " + (int)r.StatusCode);
            Console.WriteLine("Ответ: " + await r.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Запрос на удаление записи в блоге по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id записи и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public async Task DeleteBlog(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/blog/{id}";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.DeleteAsync(
                requestUri: url);
            _authHelpServices.CheckStatus(r);
        }

        /// <summary>
        /// Запрос на поиск записи блога по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id записи и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос возвращает результат в 
        /// виде экземпляра BlogModel.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public async Task<BlogModel> FindBlogById(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/blog/{id}";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            try
            {
                string json = await _httpClient.GetStringAsync(url);
                return JsonHelper.Deserialize<BlogModel>(json);
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
