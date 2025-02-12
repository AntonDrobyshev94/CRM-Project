using Models.DataServiceModels;
using BusinessLogicService.Interfaces;
using ServicesLibrary.Services.Interfaces;
using ServicesLibrary.Vars;

namespace BusinessLogicService.Data
{
    public class NavigationDataService: BaseDataService, INavigationData
    {
        public NavigationDataService(HttpClient httpClient,
            IAuthService authService) : base(httpClient, authService)
        {}

        #region Title
        /// <summary>
        /// Запрос на получение экземпляра титульного листа, передающийся 
        /// на API сервер. Запрос возвращает результат в виде экземпляра
        /// объекта типа TitleModel.
        /// </summary>
        /// <param name="url"></param>
        public async Task<TitleModel> GetTitle(string url)
        {
            var titleModel = await GetAsynс<TitleModel>(url);
            return titleModel;
        }

        /// <summary>
        /// Запрос на изменение титульного листа, передающийся 
        /// на API сервер. Данный запрос принимает строковые переменные,
        /// которые используются для создания нового экземпляра TitleModel
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра TitleModel. Данный метод является невозвратным.
        /// </summary>
        /// <param name="titleModel"></param>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task ChangeTitle(TitleModel titleModel, string token, string url)
        { 
            await PostAsync(url, titleModel, token);
        }

        /// <summary>
        /// Метод изменения титульного листа, принимающий аргументы
        /// типа string, которые сохраняются в статическое свойство
        /// TitleModelVars класса Variables.
        /// Variables
        /// </summary>
        /// <param name="newTitle"></param>
        /// <param name="mainTitle"></param>
        /// <param name="servicesTitle"></param>
        /// <param name="projectsTitle"></param>
        /// <param name="blogTitle"></param>
        /// <param name="contactsTitle"></param>
        /// <returns></returns>
        public TitleModel EditTitle(string newTitle, string mainTitle,
            string servicesTitle, string projectsTitle,
            string blogTitle, string contactsTitle)
        {
            if (!string.IsNullOrEmpty(newTitle))
            {
                Variables.TitleModelVars.Title = newTitle;
            }
            if (!string.IsNullOrEmpty(mainTitle))
            {
                Variables.TitleModelVars.MainTitle = mainTitle;
            }
            if (!string.IsNullOrEmpty(servicesTitle))
            {
                Variables.TitleModelVars.ServicesTitle = servicesTitle;
            }
            if (!string.IsNullOrEmpty(projectsTitle))
            {
                Variables.TitleModelVars.ProjectsTitle = projectsTitle;
            }
            if (!string.IsNullOrEmpty(blogTitle))
            {
                Variables.TitleModelVars.BlogTitle = blogTitle;
            }
            if (!string.IsNullOrEmpty(contactsTitle))
            {
                Variables.TitleModelVars.ContactsTitle = contactsTitle;
            }
            return Variables.TitleModelVars;
        }
        #endregion
        #region Tag
        /// <summary>
        /// Запрос на получение тэгов, передающийся на API 
        /// сервер. Запрос возвращает результат в виде коллекции
        /// объектов типа TagModel.
        /// </summary>
        /// <param name="url"></param>
        public async Task<TagModel> GetRandomTag(string url)
        {
            try
            {
                var tagModel = await GetAsynс<TagModel>(url);
                return tagModel;
            }
            catch { return new TagModel();}
        }

        /// <summary>
        /// Запрос на получение тэгов, передающийся на API 
        /// сервер. Запрос возвращает результат в виде коллекции
        /// объектов типа TagModel.
        /// </summary>
        /// <param name="url"></param>
        public async Task<IEnumerable<TagModel>> GetTags(string url)
        {
            try
            {
                var tagModels = await GetAsynс<IEnumerable<TagModel>>(url);
                return tagModels;
            }
            catch { return new List<TagModel>();}
        }

        /// <summary>
        /// Запрос на создание нового тэга, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр тэга и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task AddTagMethod(TagModel tag, string token, string url)
        {
            await PostAsync(url, tag, token);
        }

        /// <summary>
        /// Запрос на удаление тэга по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id тэга и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="url"></param>
        public async Task DeleteTag(string token, string url)
        {
            await DeleteAsync(url, token);
        }
        #endregion
    }
}
