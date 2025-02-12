using Confluent.Kafka;

namespace FinalProject_API.Kafka
{
    public class KafkaConsumerService
    {
        private readonly IConsumer<string, byte[]> _consumer;
        private readonly TokenModelHandler _tokenModelHandler;

        public KafkaConsumerService(string bootstrapServers, string groupId,
            TokenModelHandler tokenModelHandler)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<string, byte[]>(config).Build();
            _tokenModelHandler = tokenModelHandler;
        }

        public Task StartConsuming(string topic, CancellationToken cancellationToken)
        {
            _consumer.Subscribe(topic);

            return Task.Run(() =>
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var consumeResult = _consumer.Consume(cancellationToken);
                        _tokenModelHandler.HandleMessage(consumeResult.Message.Key, consumeResult.Message.Value);
                    }
                }
                catch (OperationCanceledException)
                {
                    _consumer.Close();
                }
            }, cancellationToken);
        }
    }
}
