using Models.AuthModels;
using FinalProject_Web.Services.Interfaces;
using Models.DataServiceModels;
using Microsoft.AspNetCore.Mvc;
using BusinessLogicService.Interfaces;

namespace FinalProject_Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountData _accountData;
        private readonly ILayoutViewModelServiceCreate<LayoutViewModel> _layoutViewModelServiceCreate;
        private readonly ICookiesService _cookiesService;

        public AccountController(IAccountData accountData, 
            ILayoutViewModelServiceCreate<LayoutViewModel> layoutViewModelServiceCreate,
            ICookiesService cookiesService)
        {
            _accountData = accountData;
            _layoutViewModelServiceCreate = layoutViewModelServiceCreate;
            _cookiesService = cookiesService;
        }

        /// <summary>
        /// Get запрос, в результате которого происходит переход
        /// на страницу Login (входа в аккаунт). В результате
        /// запроса происходит запоминание адреса страницы 
        /// при помощи ключевого слова return для дальнейшего
        /// возврата на эту страницу.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            LayoutViewModel layoutModel = await _layoutViewModelServiceCreate.Create(isRedact:false);
            LoginViewModel viewModel = new LoginViewModel()
            {
                LayoutViewModel = layoutModel,
                UserLogin = new UserLogin()
            };
            return View(viewModel);
        }

        /// <summary>
        /// Асинхронный метод, принимающий модель UserLogin,
        /// реализованную отдельным классом и возвращающий результат
        /// выполнения данной модели. В методе происходит проверка
        /// принимаемой модели на валидность и если модель валидна,
        /// то создается с помощью метода IsLogin создается запрос
        /// в API для проверки модели и получения токена в случае
        /// успешной авторизации. Если полученный токен равен
        /// пустой строке, то произойдет ошибка авторизации. Если
        /// не равен, то происходит проверка содержимого токена 
        /// на наличие Claims Администратора и имени пользователя,
        /// которые записываются в файлы Куки для дальнейшего
        /// использования.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> LoginPost(LoginViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userLogin = new UserLoginProp
            {
                UserName = model.UserLogin.LoginProp,
                Password = model.UserLogin.Password,
                RequestId = Guid.NewGuid().ToString()
            };

            string token = await _accountData.IsLogin(userLogin, "https://localhost:7037/api/account/Authenticate/");

            if (string.IsNullOrEmpty(token))
            {
                _cookiesService.ClearCookies(HttpContext);
                ModelState.AddModelError("", "Неверный логин или пароль");
                return View(model);
            }

            _cookiesService.SetCookies(token, HttpContext);
            string redirectUrl = HttpContext.Request.Cookies["RedirectUrl"] ?? String.Empty;
            if (redirectUrl != string.Empty)
            {
                return Redirect($"~{redirectUrl}");
            }
            return RedirectToAction("Index", "Web");
        }

        /// <summary>
        /// Get запрос на открытие формы регистрации, который
        /// отправляет новый экземпляр UserRegistration в
        /// представление Registration в качестве модели.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Registration()
        {
            LayoutViewModel layoutModel = await _layoutViewModelServiceCreate.Create(isRedact: false);
            RegistrationViewModel viewModel = new RegistrationViewModel()
            {
                LayoutViewModel = layoutModel,
                UserRegistration = new UserRegistration()
            };
            return View(viewModel);
        }

        /// <summary>
        /// Асинхронный Post запрос, принимающий модель регистрации,
        /// проверяющий правильность этой модели и на ее основе
        /// с помощью метода IsRegiseter происходит запрос в API
        /// на регистрацию нового пользователя. Если полученный 
        /// токен равен пустой строке, то произойдет ошибка 
        /// авторизации. Если не равен, то происходит запись токена,
        /// имени пользователя и роли User  в куки для дальнейшего
        /// использования. В конце происходит редирект на главную
        /// страницу.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Registration(RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                string token = await _accountData.IsRegist(model.UserRegistration, "https://localhost:7037/api/account/Registration/");

                if (string.IsNullOrEmpty(token))
                {
                    ModelState.AddModelError("", "Ошибка регистрации. Сервер отклонил попытку регистрации.");
                    ModelState.AddModelError("", "Возможно пользователь уже зарегистрирован или ошибка формата");
                    ModelState.AddModelError("", "Пароль требует символы разного регистра и специальные символы (!,$,%...)");
                    return View(model);
                }
                _cookiesService.SetAuthenticationCookies(token, HttpContext);
                return RedirectToAction("Index", "Web");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                ModelState.AddModelError("", $"Непредвиденная ошибка на стороне сервера");
                ModelState.AddModelError("", $"Не удалось зарегистрировать пользователя.{ex}");
                return View(model);
            }
        }

        /// <summary>
        /// Метод, осуществляющий переход на страницу входа Account
        /// Login.
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout(string returnUrl = null)
        {
            _cookiesService.LogoutMethod(HttpContext);
            if (returnUrl != null)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(3)
                };
                HttpContext.Response.Cookies.Append("RedirectUrl", returnUrl, cookieOptions);
            }
            return RedirectToAction("Login", "Account");
        }
    }
}
