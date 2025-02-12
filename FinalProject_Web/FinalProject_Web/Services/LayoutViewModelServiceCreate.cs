using BusinessLogicService.Interfaces;
using Models.DataServiceModels;
using FinalProject_Web.Services.Interfaces;
using ServicesLibrary.Vars;

namespace FinalProject_Web.Services
{
    public class LayoutViewModelServiceCreate: ILayoutViewModelServiceCreate<LayoutViewModel>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly INavigationData _navigationData;

        public LayoutViewModelServiceCreate(IHttpContextAccessor httpContextAccessor,
            INavigationData navigationData)
        {
            _httpContextAccessor = httpContextAccessor;
            _navigationData = navigationData;
        }

        public async Task<LayoutViewModel> Create(bool isRedact)
        {
            var request = _httpContextAccessor.HttpContext.Request;

            bool isAdmin = request.Cookies["RoleCookie"] == "Admin";
            bool isAuth = !string.IsNullOrEmpty(request.Cookies["AuthToken"]);
            string userName = request.Cookies["UserNameCookie"] ?? string.Empty;

            return new LayoutViewModel
            {
                TitleModel = Variables.TitleModelVars,
                TagModel = await _navigationData.GetRandomTag("https://localhost:7037/api/navigation/GetRandomTag"),
                IsEditMode = SetEditMode(isRedact),
                IsAuth = isAuth,
                UserName = userName,
                IsAdmin = isAdmin,
                IsRedactWindow = isRedact
            }; 
        }

        private bool SetEditMode(bool isRedact)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var response = _httpContextAccessor.HttpContext.Response;
            if (isRedact)
            {
                if (!string.IsNullOrEmpty(request.Cookies["IsEditMode"]))
                {
                    if (request.Cookies["IsEditMode"] == "true")
                    {
                        return true;
                    }
                    else { return false; }
                }
                else
                {
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    };
                    response.Cookies.Append("IsEditMode", "false", cookieOptions);
                    return false;
                }
            }
            else
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };
                response.Cookies.Append("IsEditMode", "false", cookieOptions);
                return false;
            }
        }
    }
}
