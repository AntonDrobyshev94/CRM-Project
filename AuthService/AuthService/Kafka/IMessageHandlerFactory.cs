namespace AuthService.Kafka
{
    public interface IMessageHandlerFactory
    {
        IMessageHandler CreateHandler(string key);
    }
}
