using BusinessLogicService.Interfaces;
using ServicesLibrary.Services.Interfaces;
using Models.DataServiceModels;

namespace BusinessLogicService.Data
{
    public class ProjectDataService : BaseDataService, IProjectData
    {
        public ProjectDataService(HttpClient httpClient,
            IAuthService authService) : base(httpClient, authService)
        {}
        #region Projects
        /// <summary>
        /// Запрос на получение всех проектов, передающийся на API 
        /// сервер. Запрос возвращает результат в виде коллекции
        /// объектов типа ProjectModel.
        /// </summary>
        /// <param name="url"></param>
        public async Task<IEnumerable<ProjectModel>> GetProjects(string url)
        {
            var projectModels = await GetAsynс<IEnumerable<ProjectModel>>(url);
            return projectModels;
        }

        /// <summary>
        /// Запрос на создание новго проекта, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр проекта и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="project"></param>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task AddProject(CommonWithImageModel<ProjectModel> project, string token, string url)
        {
            await PostAsync(url, project, token);
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
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task ChangeProject(CommonWithImageModel<ProjectModel> project, string token, string url)
        {
            await PostAsync(url, project, token);
        }

        /// <summary>
        /// Запрос на удаление проекта по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id проекта и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task DeleteProject(string token, string url)
        {
            await DeleteAsync(url, token);
        }

        /// <summary>
        /// Запрос на поиск проекта по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id проекта и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос возвращает результат в 
        /// виде экземпляра ProjectModel.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task<ProjectModel> FindProjectById(string token, string url)
        {
            try
            {
                var projectModel = await GetAsynс<ProjectModel>(url, token);
                return projectModel;
            }
            catch { return null; }
        }
        #endregion
    }
}
