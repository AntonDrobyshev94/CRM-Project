using AuthService.AuthModels;
using AuthService.Data;
using System.Text.Json;
using System.Text;

namespace AuthService.Kafka
{
    public class RegistrationHandler : IMessageHandler
    {
        private readonly AccountData _account;
        private readonly KafkaProducerService _kafkaProducerService;

        public RegistrationHandler(AccountData accountData, KafkaProducerService kafkaProducerService)
        {
            _account = accountData;
            _kafkaProducerService = kafkaProducerService;
        }

        public async Task HandleMessageAsync(string key, byte[] message)
        {
            var commonModel = JsonSerializer.Deserialize<CommonModel>(Encoding.UTF8.GetString(message));

            if (commonModel.Type is nameof(UserRegistration))
            {
                var userRegistration = commonModel.BaseModel.Deserialize<UserRegistration>();
                var tokenResponse = await _account.Register(userRegistration);
                if (userRegistration != null)
                {
                    await _kafkaProducerService.SendMessageAsync("response-topic", userRegistration.RequestId, tokenResponse);
                }
                else
                {
                    await _kafkaProducerService.SendMessageAsync("response-topic", userRegistration.RequestId, null);
                }
                    
            }
        }
    }
}
