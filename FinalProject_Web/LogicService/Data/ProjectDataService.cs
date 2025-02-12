using FinalProject_Web.Helpers;
using FinalProject_Web.Model;
using FinalProject_Web.Services.Interfaces;
using System.Text.Json;
using System.Text;
using FinalProject_Web.Interfaces;

namespace FinalProject_Web.Data
{
    public class ProjectDataService : IProjectData
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authHelpServices;
        public ProjectDataService(HttpClient httpClient,
            IAuthService authenticationHelpService)
        {
            _httpClient = httpClient;
            _authHelpServices = authenticationHelpService;
        }
        #region Projects
        /// <summary>
        /// Запрос на получение всех проектов, передающийся на API 
        /// сервер. Запрос возвращает результат в виде коллекции
        /// объектов типа ProjectModel.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ProjectModel>> GetProjects()
        {
            string url = @"https://localhost:7037/api/project";
            string json = await _httpClient.GetStringAsync(url);
            return JsonHelper.Deserialize<IEnumerable<ProjectModel>>(json);
        }

        /// <summary>
        /// Запрос на создание новго проекта, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр проекта и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="httpContext"></param>
        public async Task AddProject(CommonWithImageModel<ProjectModel> project, HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/project";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonSerializer.Serialize(project), Encoding.UTF8,
                mediaType: "application/json")
                );
            _authHelpServices.CheckStatus(r);
            Console.WriteLine("Статус ответа: " + (int)r.StatusCode);
            Console.WriteLine("Ответ: " + await r.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Запрос на изменение проекта, передающийся 
        /// на API сервер. Данный запрос принимает строковые переменные,
        /// которые используются для создания нового экземпляра ProjectModel
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра ProjectModel. Данный метод является невозвратным.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="httpContext"></param>
        public async Task ChangeProject(CommonWithImageModel<ProjectModel> project, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/project/ChangeProject";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonSerializer.Serialize(project), Encoding.UTF8,
                mediaType: "application/json")
                );
            _authHelpServices.CheckStatus(r);
            Console.WriteLine("Статус ответа: " + (int)r.StatusCode);
            Console.WriteLine("Ответ: " + await r.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Запрос на удаление проекта по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id проекта и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public async Task DeleteProject(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/project/{id}";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.DeleteAsync(
                requestUri: url);
            _authHelpServices.CheckStatus(r);
            Console.WriteLine("Статус ответа: " + (int)r.StatusCode);
            Console.WriteLine("Ответ: " + await r.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Запрос на поиск проекта по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id проекта и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос возвращает результат в 
        /// виде экземпляра ProjectModel.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public async Task<ProjectModel> FindProjectById(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/project/{id}";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            try
            {
                string json = await _httpClient.GetStringAsync(url);
                return JsonHelper.Deserialize<ProjectModel>(json);
            }
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
