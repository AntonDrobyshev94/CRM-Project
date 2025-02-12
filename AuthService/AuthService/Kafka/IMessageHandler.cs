using AuthService.ContextFolder;

namespace AuthService.Kafka
{
    public interface IMessageHandler
    {
        Task HandleMessageAsync(string key, byte[] message);
    }
}
