using Microsoft.AspNetCore.Mvc;
using BusinessLogicService.Interfaces;
using Models.DataServiceModels;

namespace FinalProject_Web.Controllers
{
    public class NavigationController : Controller
    {
        private readonly INavigationData _navigationData;
        public NavigationController (INavigationData navigationData)
        {
            _navigationData = navigationData;
        }

        /// <summary>
        /// Метод, принимающий текущее название action view модели и 
        /// контроллера, из которого его вызывают. Далее происходит
        /// вызов метода EditMode, который меняет режим редактирования
        /// и редирект на view, из которого был вызван метод
        /// </summary>
        /// <param name="controllerName"></param>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public IActionResult EditModeMethod(string controllerName, string actionName)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            if (!string.IsNullOrEmpty(HttpContext.Request.Cookies["IsEditMode"]))
            {
                string mode = HttpContext.Request.Cookies["IsEditMode"];
                if (mode == "true")
                {
                    mode = "false";
                    HttpContext.Response.Cookies.Append("IsEditMode", mode, cookieOptions);
                }
                else
                {
                    mode = "true";
                    HttpContext.Response.Cookies.Append("IsEditMode", mode, cookieOptions);
                }
            }
            else
            {
                HttpContext.Response.Cookies.Append("IsEditMode", "false", cookieOptions);
            }
            return RedirectToAction(actionName, controllerName);
        }

        #region Tag
        /// <summary>
        /// Метод добавления тэга
        /// </summary>
        /// <param name="newTag"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddTag(string newTag)
        {
            var tag = new TagModel
            {
                Tag = newTag
            };
            string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
            await _navigationData.AddTagMethod(tag, token, "https://localhost:7037/api/navigation/AddTag");
            return Redirect("~/");
        }

        /// <summary>
        /// Метод удаления тэга
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DeleteTag(int id)
        {
            string token = HttpContext.Request.Cookies["AuthToken"] ?? string.Empty;
            await _navigationData.DeleteTag(token, $"https://localhost:7037/api/navigation/DeleteTag/{id}");
            return Redirect("~/");
        }
        #endregion
    }
}
