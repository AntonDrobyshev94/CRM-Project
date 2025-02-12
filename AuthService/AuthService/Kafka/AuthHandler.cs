using AuthService.AuthModels;
using AuthService.Data;
using System.Text.Json;
using System.Text;
using AuthService.ContextFolder;

namespace AuthService.Kafka
{
    public class AuthHandler : IMessageHandler
    {
        private readonly AccountData _account;
        private readonly KafkaProducerService _kafkaProducerService;

        public AuthHandler(AccountData accountData, KafkaProducerService kafkaProducerService)
        {
            _account = accountData;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task HandleMessageAsync(string key, byte[] message)
        {
            var commonModel = JsonSerializer.Deserialize<CommonModel>(Encoding.UTF8.GetString(message));

            if (commonModel.Type is nameof(UserLoginProp))
            {
                var userLoginProp = commonModel.BaseModel.Deserialize<UserLoginProp>();
                var tokenResponse = await _account.Login(userLoginProp);
                if (tokenResponse != null)
                {
                    await _kafkaProducerService.SendMessageAsync("response-topic", userLoginProp.RequestId, tokenResponse);
                }
                else
                {
                    await _kafkaProducerService.SendMessageAsync("response-topic", userLoginProp.RequestId, null);
                }
            }
        }
    }
}

