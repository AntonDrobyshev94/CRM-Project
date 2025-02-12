using FinalProject_API.AuthFinalProjectApp;
using FinalProject_API.Data;
using FinalProject_API.Kafka;
using FinalProject_API.Temporary;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinalProject_API.Controllers
{
    /// <summary>
    /// Контроллер, осуществляющий взаимодействие с пользователями
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly AccountData _accountData;
        private readonly KafkaProducerService _kafkaProducerService;
        private TemporaryDatabase _temporaryDatabase;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AccountController"/> с инжекцией 
        /// зависимостей для классов <see cref="KafkaProducerService"/> и <see cref="AccountData"/>.
        /// </summary>
        /// <param name="kafkaProducerService">Экземпляр <see cref="KafkaProducerService"/>, 
        /// используемый для отправки сообщений в Kafka.</param>
        /// <param name="accountData">Экземпляр <see cref="AccountData"/>, 
        /// используемый для взаимодействия с данными учетных записей.</param>
        /// /// <param name="temporaryDatabase">Экземпляр <see cref="TemporaryDatabase"/>, 
        /// используемый для взаимодействия с кэшем ключей идемпотентности и 
        /// возвращения моделей, хранимых в кэше.</param>
        public AccountController(KafkaProducerService kafkaProducerService,
            AccountData accountData,
            TemporaryDatabase temporaryDatabase)
        {
            _accountData = accountData;
            _kafkaProducerService = kafkaProducerService;
            _temporaryDatabase = temporaryDatabase;
        }

        #region Authorization
        /// <summary>
        /// POST запрос для регистрации нового пользователя по
        /// указанным в модели данным.
        /// </summary>
        /// <param name="regData">Модель регистрации пользователя</param>
        [HttpPost]
        [Route("Registration")]
        public async Task<IActionResult> Registration([FromBody] UserRegistration regData)
        {
            if (_temporaryDatabase.Get(regData.RequestId) == null)
            {
                await _kafkaProducerService.SendMessageAsync("auth-top", "Reg", _accountData.CommonModelCreate(nameof(UserRegistration), regData));
                Console.WriteLine($"{regData.RequestId} отправлен на микросервис");
                try
                {
                    var response = await _accountData.WaitForResponse(regData.RequestId);
                    if (response == null)
                    {
                        return Unauthorized();
                    }
                    _temporaryDatabase.AddOrUpdate(regData.RequestId, response);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    return Unauthorized();
                }
            }
            else
            {
                Console.WriteLine($"{regData.RequestId} уже был отправлен на микросервис");
                try
                {
                    var cachedResponse = _temporaryDatabase.Get(regData.RequestId);
                    if (cachedResponse == null)
                    {
                        return Unauthorized();
                    }
                    return Ok(cachedResponse);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message.ToString());
                    return Unauthorized();
                }
            } 
        }

        /// <summary>
        /// POST запрос на аутентификацию пользователя по указанным
        /// аутентификационным данным
        /// </summary>
        /// <param name="loginData">Модель авторизации пользователя</param>
        [HttpPost]
        [Route("Authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserLoginProp loginData)
        {
            await _kafkaProducerService.SendMessageAsync("auth-top", "Auth", _accountData.CommonModelCreate(nameof(UserLoginProp), loginData));
            Console.WriteLine($"Id {loginData.RequestId} успешно попал в контроллер");
            try
            {
                var response = await _accountData.WaitForResponse(loginData.RequestId);
                if (response == null)
                {
                    return Unauthorized();
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
                return Unauthorized();
            }   
        }

        /// <summary>
        /// Запрос проверки токена
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult CheckToken()
        {
            return Ok();
        }
        #endregion
    }
}
