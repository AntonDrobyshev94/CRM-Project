namespace AuthService.Kafka
{
    public class MessageHandlerFactory : IMessageHandlerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public MessageHandlerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IMessageHandler CreateHandler(string key)
        {
            if (key == "Auth")
            {
                return _serviceProvider.GetRequiredService<AuthHandler>();
            }
            else if (key == "Reg")
            {
                return _serviceProvider.GetRequiredService<RegistrationHandler>();
            }
            throw new ArgumentException($"Не найдено обработчиков для ключа: {key}");
        }
    }
}
