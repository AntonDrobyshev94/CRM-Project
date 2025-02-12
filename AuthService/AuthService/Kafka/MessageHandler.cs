//using AuthService.AuthModels;
//using AuthService.Data;
//using System.Text.Json;
//using System.Text;

//namespace AuthService.Kafka
//{
//    public class MessageHandler : IMessageHandler
//    {
//        private readonly AccountData _account;
//        private readonly KafkaProducerService _kafkaProducerService;

//        public MessageHandler(AccountData accountData, KafkaProducerService kafkaProducerService)
//        {
//            _account = accountData;
//            _kafkaProducerService = kafkaProducerService;
//        }

//        public async Task HandleMessageAsync(string key, byte[] message)
//        {
//            var commonModel = JsonSerializer.Deserialize<CommonModel>(Encoding.UTF8.GetString(message));

//            if (commonModel.Type == nameof(UserLoginProp))
//            {
//                var userLoginProp = commonModel.BaseModel.Deserialize<UserLoginProp>();
//                var tokenResponse = await _account.Login(userLoginProp);
//                await _kafkaProducerService.SendMessageAsync("response-topic", tokenResponse.username, tokenResponse);
//            }
//        }
//    }
//}
