using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicService.Application.Interfaces.Messages;
using MusicService.Infrastructure.Options;

namespace MusicService.Infrastructure.Kafka;

public class KafkaProducer<TMessage> : IMessageProducer<TMessage>
{
    private readonly IProducer<string, TMessage> _producer;
    private readonly string _topic;
    private readonly ILogger<KafkaProducer<TMessage>> _logger;

    public KafkaProducer(IOptionsMonitor<KafkaProducerOptions> optionsSnapshot, ILogger<KafkaProducer<TMessage>> logger)
    {
        var options = optionsSnapshot.Get(typeof(TMessage).Name);
        var config = new ProducerConfig
        {
            BootstrapServers = options.BootstrapServers,
        };
        
        _producer = new ProducerBuilder<string, TMessage>(config)
            .SetValueSerializer(new KafkaJsonSerializer<TMessage>()).Build();
        _topic = options.Topic;
        _logger = logger;
    }
    
    public async Task ProduceAsync(TMessage message, CancellationToken ct = default)
    {
        try 
        {
            var result = await _producer.ProduceAsync(_topic, new Message<string, TMessage>
            {
                Value = message
            }, ct);

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