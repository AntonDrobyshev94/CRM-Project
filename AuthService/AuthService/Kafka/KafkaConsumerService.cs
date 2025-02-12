using AuthService.ContextFolder;
using Confluent.Kafka;

namespace AuthService.Kafka
{
    public class KafkaConsumerService
    {
        private readonly IConsumer<string, byte[]> _consumer;
        private readonly IServiceScopeFactory _scopeFactory;

        public KafkaConsumerService(string bootstrapServers, string groupId, 
            IServiceScopeFactory scopeFactory)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<string, byte[]>(config).Build();
            _scopeFactory = scopeFactory;
        }

        public Task StartConsumingAsync(string topic, CancellationToken cancellationToken)
        {
            _consumer.Subscribe(topic);
            return Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var consumeResult = _consumer.Consume(cancellationToken);
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var handlerFactory = scope.ServiceProvider.GetRequiredService<IMessageHandlerFactory>();
                        var handler = handlerFactory.CreateHandler(consumeResult.Message.Key);
                        await handler.HandleMessageAsync(consumeResult.Message.Key, consumeResult.Message.Value);
                    }
                }
            }, cancellationToken);
        }
    }
}
