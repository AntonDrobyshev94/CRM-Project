using Confluent.Kafka;
using FinalProject_API.AuthFinalProjectApp;
using System.Text.Json;
using System.Text;

namespace FinalProject_API.Kafka
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

        public async Task SendMessageAsync(string topic, string key, CommonModel message)
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
