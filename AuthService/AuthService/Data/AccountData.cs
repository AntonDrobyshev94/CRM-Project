using Microsoft.AspNetCore.Identity;
using AuthService.AuthModels;
using AuthService.ContextFolder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FinalAuthService.AuthModels;

namespace AuthService.Data
{
    public class AccountData
    {
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;
        public IConfiguration Configuration { get; }

        public AccountData(UserManager<User> userManager,
            IConfiguration configuration,
            DataContext context)
        {
            _userManager = userManager;
            Configuration = configuration;
            _context = context;
        }
        #region Authorization
        /// <summary>
        /// Асинхронный метод входа в учетную запись, который принимает
        /// UserLoginProp модель, описанную отдельным классом и возвращает
        /// TokenResponseModel, описанную отдельным классом. В данном методе
        /// происходит создание экземпляра пользователя User newUser, в 
        /// параметр UserName которого записывается UserName принимаемой модели.
        /// Далее создается экземпляр User "user" в который записывается 
        /// результат перебора таблицы Users базы данных на предмет совпадения
        /// имени пользователя с именем пользователя принимаемой модели.
        /// Если полученный экземпляр равен нулю, то возвращается null,
        /// приводящий к завершению метода.
        /// Если экземпляр не равен нулю, то создается экземпляр PasswordHasher
        /// параметаризированный строкой. Далее создается новый обобщенный
        /// экземпляр, который является результатом работы метода 
        /// VerifyHashedPassword на экземпляре passwordHasher. Данный метод
        /// принимает значение пароля из полученной модели и переводит сверяет
        /// его с паролем текущего экземпляра пользователя в виде Хэш пароля.
        /// Далее, с помощью конструкции switch происходит проверка полученного
        /// экземпляра passwordVerificationResult. При результате 
        /// PasswordVerificationResult.Failed происходит возвращение null с 
        /// помощью ключевого слова return и завершение метода.
        /// Если результат не ошибочный, то метод продолжается и происходит
        /// создание переменной строкового типа id, в которую записывается
        /// значение Id текущего экземпляра User. Далее происходит создание
        /// экземпляра коллекции List строкового типа и в цикле foreach
        /// происходит перебор таблицы UserRoles базы данных и с условием,
        /// что параметр UserId текущей таблицы равен Id пользователя происходит
        /// запись Id роли в коллекцию с помощью метода Add. Далее, создается
        /// строковая переменная roleId для записи в неё id роли. В следующем 
        /// цикле foreach происходит перебор полученной ранее коллекции List на
        /// предмет равенства id единице (т.е., означает что пользователь - 
        /// администратор). Если найден id, равный 1, то происходит завершение
        /// цикла с записью id в переменную roleId. В противном случае результат
        /// в любом случае записывается в roleId, но он уже не будет равен 1.
        /// Далее происходит создание переменной строкового типа jwt, в которую
        /// будет записан результат выполнения метода GenerateJwtToken,
        /// который принимает в себя имя текущего экземпляра пользователя и id
        /// полученной роли roleId, после чего создается модель TokenResponseModel,
        /// описанная отдельным классом, с параметрами токена и имени пользователя.
        /// В параметры созданной модели записываются полученные ранее параметры
        /// токена и имени пользователя и происходит возврат модели с помощью 
        /// метода return
        /// </summary>
        /// <param name="loginData"></param>
        /// <returns></returns>
        public async Task<TokenResponseModel> Login(UserLoginProp loginData)
        {
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(p => p.UserName == loginData.UserName);
                if (user is null)
                {
                    return null;
                }
                else
                {
                    var passwordHasher = new PasswordHasher<string>();
                    var passwordVerificationResult = passwordHasher.VerifyHashedPassword(null, user.PasswordHash, loginData.Password);
                    switch (passwordVerificationResult)
                    {
                        case PasswordVerificationResult.Failed:
                            return null;
                    }
                }
                string id = user.Id;
                List<string> idCol = new List<string>();
                foreach (var item in _context.UserRoles)
                {
                    if (item.UserId == id)
                    {
                        idCol.Add(item.RoleId);
                    }
                }
                string roleId = string.Empty;
                foreach (var item in idCol)
                {
                    if (item == "1")
                    {
                        roleId = item;
                        break;
                    }
                    else
                    {
                        roleId = item;
                    }
                }
                if (roleId != null)
                {
                    string jwt = GenerateJwtToken(user.UserName, roleId);

                    var response = new TokenResponseModel
                    {
                        Access_token = jwt,
                        Username = user.UserName
                    };
                    return (response);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Метод генерации JWT токена, принимающий строковые переменные
        /// имени пользователя и Id роли и возвращающий строковое значение
        /// токена. При условии, что Id роли равен 1 (т.е. это id 
        /// администратора) создаётся новая коллекция List класса Claim,
        /// в которую записываются даннные о имени пользователя и роли.
        /// Далее создается Jwt токен с конфигурационными параметрами,
        /// указанными в appsettings.json и хэш кодом 256. Время действия 
        /// токена задаётся равным 10 минутам. По окончанию происходит
        /// возвращение JWT токена в виде строки с помощью ключевого слова
        /// return. Если id роли не равен 1, то создается токен обычного
        /// пользователя.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        private string GenerateJwtToken(string userName, string roleId)
        {
            switch (roleId)
            {
                case "1":
                    var claims = new List<Claim> {
                            new Claim(ClaimTypes.Name, userName),
                            new Claim(ClaimTypes.Role, "Admin")
                        };
                    var jwt = new JwtSecurityToken(
                    issuer: Configuration["Jwt:Issuer"]!,
                    audience: Configuration["Jwt:Audience"]!,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"]!)
                        ),
                        SecurityAlgorithms.HmacSha256));
                    return new JwtSecurityTokenHandler().WriteToken(jwt); 
                default :
                    claims = new List<Claim> {
                            new Claim(ClaimTypes.Name, userName),
                            new Claim(ClaimTypes.Role, "User")
                        };
                    jwt = new JwtSecurityToken(
                    issuer: Configuration["Jwt:Issuer"]!,
                    audience: Configuration["Jwt:Audience"]!,
                    claims: claims,
                    expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(2)),
                    signingCredentials: new SigningCredentials(
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(Configuration["Jwt:Secret"]!)
                        ),
                        SecurityAlgorithms.HmacSha256));
                    return new JwtSecurityTokenHandler().WriteToken(jwt);
            }
        }

        /// <summary>
        /// Асинхронный метод регистрации нового пользователя, принимающий
        /// модель UserRegistration, описанную отдельным классом и 
        /// возвращающий TokenResponseModel, описанный отдельным классом.
        /// В методе происходит создание экземпляра нового пользователя с
        /// именем из принимаемой модели. С помощью метода CreateAsync 
        /// происходит создание пароля для пользовательского аккаунта   
        /// экземпляра User который проходит процедуру хэш кодирования. 
        /// Результат метода содержится в переменной createResult. Далее,
        /// с помощью метода AddToRoleAsync происходит добавление роли 
        /// User для вновь созданного пользовательского аккаунта. Результат
        /// выполнения метода сохраняется в переменной addRoleResult.
        /// С помощью метода GenerateJwtToken происходит создание нового 
        /// токена для пользовательского аккаунта. Далее происходит
        /// сохранение результатов, а именно токена и имени пользователя
        /// в модель TokenResponseModel, результат который возвращается
        /// с помощью ключевого слова return, при условии, что createResult 
        /// и addRoleResult возвращают значение Succeeded. В обратном 
        /// случае возвращается null.
        /// </summary>
        /// <param name="registrData"></param>
        /// <returns></returns>
        public async Task<TokenResponseModel> Register(UserRegistration registrData)
        {
            var user = new User { UserName = registrData.LoginProp };
            var createResult = await _userManager.CreateAsync(user, registrData.Password);
            var addRoleResult = await _userManager.AddToRoleAsync(user, "User");
            var jwt = GenerateJwtToken(user.UserName, "2");

            var response = new TokenResponseModel
            {
                Access_token = jwt,
                Username = user.UserName,
                RequestId = registrData.RequestId
            };

            if (createResult.Succeeded && addRoleResult.Succeeded)
            {
                return response;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}
