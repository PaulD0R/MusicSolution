using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserService.Application.Interfaces.Messages;
using UserService.Infrastructure.Options;

namespace UserService.Infrastructure.Kafka;

public class KafkaProducer<TMessage> : IMessageProducer<TMessage>
{
    private readonly IProducer<string, TMessage> _producer;
    private readonly string _topic;
    private readonly ILogger<KafkaProducer<TMessage>> _logger;

    public KafkaProducer(
        IOptionsMonitor<KafkaProducerOptions> optionsMonitor, 
        ILogger<KafkaProducer<TMessage>> logger)
    {
        _logger = logger;
        
        var options = optionsMonitor.Get(typeof(TMessage).Name);
        var config = new ProducerConfig
        {
            BootstrapServers = options.BootstrapServers
        };
        
        _producer = new ProducerBuilder<string, TMessage>(config)
            .SetValueSerializer(new KafkaJsonSerializer<TMessage>()).Build();

        _topic = options.Topic;
    }
    
    public async Task ProduceAsync(TMessage message, CancellationToken cancellationToken)
    {
        try 
        {
            var result = await _producer.ProduceAsync(_topic, new Message<string, TMessage>
            {
                Value = message
            }, cancellationToken);

            _logger.LogInformation("Delivered to: {TopicPartitionOffset}", result.TopicPartitionOffset);
        }
        catch (ProduceException<string, TMessage> e)
        {
            _logger.LogError($"Delivery failed: {e.Error.Reason}");
        }
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}