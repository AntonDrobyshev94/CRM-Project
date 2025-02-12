using AuthService.AuthModels;
using System.Text.Json;
using System.Text;
using Confluent.Kafka;
using FinalAuthService.AuthModels;

namespace AuthService.Kafka
{
    public class KafkaProducerService
    {
        private readonly IProducer<string, byte[]> _producer;

        public KafkaProducerService(string bootstrapServers)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = bootstrapServers,
                Acks = Acks.All
            };

            _producer = new ProducerBuilder<string, byte[]>(config).Build();
        }

        public async Task SendMessageAsync(string topic, string key, TokenResponseModel message)
        {
            var jsonMessage = JsonSerializer.Serialize(message);
            var messageBytes = Encoding.UTF8.GetBytes(jsonMessage);

            await _producer.ProduceAsync(topic, new Message<string, byte[]>
            {
                Key = key,
                Value = messageBytes
            });
        }
    }
}
