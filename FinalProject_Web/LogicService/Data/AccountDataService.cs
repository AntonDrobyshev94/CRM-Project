using FinalProject_Web.AuthFinalProjectApp;
using FinalProject_Web.Helpers;
using FinalProject_Web.Interfaces;
using System.Text.Json;
using System.Text;

namespace FinalProject_Web.Data
{
    public class AccountDataApi : IAccountData
    {
        private readonly HttpClient _httpClient;
        public AccountDataApi(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region Authentication
        /// <summary>
        /// Запрос на регистрацию, передающийся в API сервер.
        /// Данный запрос принимает модель регистрации и возвращает
        /// результат запроса в виде строки, содержащей токен.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<string> IsRegist(UserRegistration model)
        {
            string url = $"https://localhost:7037/api/account/Registration/";

            var r = await _httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8,
                mediaType: "application/json")
                );
            string token = "";
            if (r.IsSuccessStatusCode)
            {
                string json = await r.Content.ReadAsStringAsync();
                var tokenResponse = JsonHelper.Deserialize<TokenResponseModel>(json);
                string tokenAuth = tokenResponse.Access_token;
                token = tokenAuth;
            }
            else
            {
                token = string.Empty;
            }
            return token;
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
        public async Task<string> IsLogin(UserLoginProp model)
        {
            string url = $"https://localhost:7037/api/account/Authenticate/";

            var r = await _httpClient.PostAsync(
                requestUri: url,
                content: new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8,
                mediaType: "application/json")
                );

            Console.WriteLine("Статус ответа: " + (int)r.StatusCode);
            Console.WriteLine("Ответ: " + await r.Content.ReadAsStringAsync());
            string token = "";
            if (r.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                string json = r.Content.ReadAsStringAsync().Result;
                var tokenResponse = JsonHelper.Deserialize<TokenResponseModel>(json);
                string tokenAuth = tokenResponse.Access_token;
                token = tokenAuth;
            }
            else
            {
                token = string.Empty;
            }
            return token;
        }
        #endregion
    }
}
