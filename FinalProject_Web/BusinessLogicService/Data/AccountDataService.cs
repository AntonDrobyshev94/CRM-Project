using BusinessLogicService.Interfaces;
using ServicesLibrary.Services.Interfaces;
using Models.AuthModels;

namespace BusinessLogicService.Data
{
    public class AccountDataService : BaseDataService, IAccountData
    {
        public AccountDataService(HttpClient httpClient,
            IAuthService authService) : base (httpClient, authService)
        {}

        #region Authentication
        /// <summary>
        /// Запрос на регистрацию, передающийся в API сервер.
        /// Данный запрос принимает модель регистрации и возвращает
        /// результат запроса в виде строки, содержащей токен.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> IsRegist(UserRegistration model, string url)
        {
            var tokenResponseModel = await PostAsync<TokenResponseModel>(url, model);
            return tokenResponseModel.Access_token;
        }

        /// <summary>
        /// Запрос на вход пользователя, передающийся в API
        /// Данный запрос принимает модель UserLoginProp
        /// и возвращает строковую переменную с ответом,
        /// который будет включать в себя токен при удачном
        /// входе или пустую строку при неудачном входе.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> IsLogin(UserLoginProp model, string url)
        {
            var tokenResponseModel = await PostAsync<TokenResponseModel>(url, model);
            return tokenResponseModel.Access_token;
        }
        #endregion
    }
}
