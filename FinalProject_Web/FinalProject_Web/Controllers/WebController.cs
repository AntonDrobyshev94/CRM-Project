using Microsoft.AspNetCore.Mvc;
using Models.DataServiceModels;
using FinalProject_Web.Services.Interfaces;
using BusinessLogicService.Interfaces;

namespace FinalProject_Web.Controllers
{
    public class WebController : Controller
    {
        private readonly IApplicationData _applicationData;
        private readonly ILayoutViewModelServiceCreate<LayoutViewModel> _layoutViewModelServiceCreate;
        private readonly INavigationData _navigationData;

        public WebController(IApplicationData applicationData,
            ILayoutViewModelServiceCreate<LayoutViewModel> layoutViewModelServiceCreate,
            INavigationData navigationData)
        {
            _applicationData = applicationData;
            _layoutViewModelServiceCreate = layoutViewModelServiceCreate;
            _navigationData = navigationData;
        }

        /// <summary>
        /// Метод перехода во View главной страницы.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.TagModelList = await _navigationData.GetTags("https://localhost:7037/api/navigation/GetTags");
            LayoutViewModel layout = await _layoutViewModelServiceCreate.Create(isRedact: true);
            SimpleViewModel view = new SimpleViewModel
            {
                LayoutViewModel = layout
            };
            return View(view);
        }
        #region Application

        /// <summary>
        /// Метод добавления заявки, принимающей строковые переменные имени, 
        /// почты и сообщения. В методе используется блок try catch,
        /// в котором происходит запуск метода обращения к API для добавления
        /// заявки AddAplication. При успешном запросе происходит передача
        /// новой заявки в API. В TempData сохраняется результат успешной
        /// отправки, который считывается во View и отображается для 
        /// пользователя в виде сообщения об успешной отправке. В противном
        /// случае в TempData сохраняется результат ошибки и считывается
        /// во View.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="eMail"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddRequest(string name, string eMail, string message,
            string requestId)
        {
            bool isCorrect = false;
            string requestMessage = string.Empty;
            try
            {
                var application = new Application()
                {
                    Name = name,
                    EMail = eMail,
                    Message = message,
                    Date = DateTime.Now,
                    Status = "Получена"
                };
                string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
                await _applicationData.AddApplication(application, token, 
                    "https://localhost:7037/api/values", requestId);
                requestMessage = "Заявка успешно отправлена";
                isCorrect = true;
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex);
                
                requestMessage = "Произошла ошибка при отправке заявки";
                isCorrect = false;
            }
            TempData["RequestMessage"] = requestMessage;
            TempData["IsCorrectRequest"] = isCorrect;
            return Redirect("~/");
        }

        /// <summary>
        /// Метод настройки титульной страницы (заголовков)
        /// </summary>
        /// <param name="newTitle"></param>
        /// <param name="mainTitle"></param>
        /// <param name="servicesTitle"></param>
        /// <param name="projectsTitle"></param>
        /// <param name="blogTitle"></param>
        /// <param name="contactsTitle"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult EditTitleMethod(string newTitle, string mainTitle,
            string servicesTitle, string projectsTitle,
            string blogTitle, string contactsTitle)
        {
            TitleModel tittleModel = _navigationData.EditTitle(newTitle, mainTitle, servicesTitle,
                projectsTitle, blogTitle, contactsTitle);
            try
            {
                string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
                _navigationData.ChangeTitle(tittleModel, token, "https://localhost:7037/api/navigation");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("Index");
        }
        #endregion
    }
}