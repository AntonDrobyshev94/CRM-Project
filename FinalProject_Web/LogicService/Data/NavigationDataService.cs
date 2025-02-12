using FinalProject_Web.Helpers;
using FinalProject_Web.Interfaces;
using FinalProject_Web.Services.Interfaces;
using FinalProject_Web.Model;
using System.Text.Json;
using System.Text;
using FinalProject_Web.Vars;

namespace FinalProject_Web.Data
{
    public class NavigationDataService: INavigationData
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authHelpServices;
        public NavigationDataService(IAuthService authHelpServices,
            HttpClient httpClient)
        {
            _httpClient = httpClient;
            _authHelpServices = authHelpServices;
        }

        public void EditMode(HttpContext context)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            if (!string.IsNullOrEmpty(context.Request.Cookies["IsEditMode"]))
            {
                string mode = context.Request.Cookies["IsEditMode"];
                if (mode == "true")
                {
                    mode = "false";
                    context.Response.Cookies.Append("IsEditMode", mode, cookieOptions);
                }
                else
                {
                    mode = "true";
                    context.Response.Cookies.Append("IsEditMode", mode, cookieOptions);
                }
            }
            else
            {
                context.Response.Cookies.Append("IsEditMode", "false", cookieOptions);
            }
        }

        #region Title
        /// <summary>
        /// Запрос на получение экземпляра титульного листа, передающийся 
        /// на API сервер. Запрос возвращает результат в виде экземпляра
        /// объекта типа TitleModel.
        /// </summary>
        /// <returns></returns>
        public async Task<TitleModel> GetTitle()
        {
            string url = @"https://localhost:7037/api/navigation";
            string json = await _httpClient.GetStringAsync(url);
            return JsonHelper.Deserialize<TitleModel>(json);
        }

        /// <summary>
        /// Запрос на изменение титульного листа, передающийся 
        /// на API сервер. Данный запрос принимает строковые переменные,
        /// которые используются для создания нового экземпляра TitleModel
        /// и текущий Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Далее происходит передача 
        /// экземпляра TitleModel. Данный метод является невозвратным.
        /// </summary>
        /// <param name="editModel"></param>
        /// <param name="httpContext"></param>
        public async Task ChangeTitle(TitleModel editModel, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/navigation";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonSerializer.Serialize(editModel), Encoding.UTF8,
                mediaType: "application/json")
                );
            _authHelpServices.CheckStatus(r);
            Console.WriteLine("Статус ответа: " + (int)r.StatusCode);
            Console.WriteLine("Ответ: " + await r.Content.ReadAsStringAsync());
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
        /// <returns></returns>
        public async Task<TagModel> GetRandomTag()
        {
            string url = @"https://localhost:7037/api/navigation/GetRandomTag";
            try
            {
                string json = await _httpClient.GetStringAsync(url);
                return JsonHelper.Deserialize<TagModel>(json);
            }
            catch
            {
                return new TagModel();
            }
        }

        /// <summary>
        /// Запрос на получение тэгов, передающийся на API 
        /// сервер. Запрос возвращает результат в виде коллекции
        /// объектов типа TagModel.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TagModel>> GetTags()
        {
            string url = @"https://localhost:7037/api/navigation/GetTags";
            try
            {
                string json = await _httpClient.GetStringAsync(url);
                return JsonHelper.Deserialize<IEnumerable<TagModel>>(json);
            }
            catch
            {
                return new List<TagModel>();
            }
        }

        /// <summary>
        /// Запрос на создание нового тэга, передающийся на API 
        /// сервер. Данный запрос принимает экземпляр тэга и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="httpContext"></param>
        public async Task AddTagMethod(TagModel tag, HttpContext httpContext)
        {
            string url = @"https://localhost:7037/api/navigation/AddTag";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonSerializer.Serialize(tag), Encoding.UTF8,
                mediaType: "application/json")
                );
            _authHelpServices.CheckStatus(r);
            Console.WriteLine("Статус ответа: " + (int)r.StatusCode);
            Console.WriteLine("Ответ: " + r.Content.ReadAsStringAsync().Result);
        }

        /// <summary>
        /// Запрос на удаление тэга по указанному id, передающийся 
        /// на API сервер. Данный запрос принимает id тэга и текущий 
        /// Http-контекст, который позволяет обратиться к куки,
        /// в которых хранится токен. Запрос является невозвратным.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="httpContext"></param>
        public async Task DeleteTag(int id, HttpContext httpContext)
        {
            string url = $"https://localhost:7037/api/navigation/DeleteTag/{id}";
            _authHelpServices.AddTokenHeaderMethod(httpContext, _httpClient);
            var r = await _httpClient.DeleteAsync(
                requestUri: url);
            _authHelpServices.CheckStatus(r);
        }
        #endregion
    }
}
