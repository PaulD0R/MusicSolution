using Confluent.Kafka;
using Microsoft.Extensions.Options;
using MusicService.Application.Interfaces.Messages;
using MusicService.Infrastructure.Options;

namespace MusicService.Infrastructure.Kafka;

public class KafkaProducer<TMessage> : IMessageProducer<TMessage>
{
    private readonly IProducer<string, TMessage> _producer;
    private readonly string _topic;

    public KafkaProducer(IOptionsMonitor<KafkaProducerOptions> optionsSnapshot)
    {
        var options = optionsSnapshot.Get(typeof(TMessage).Name);
        var config = new ProducerConfig
        {
            BootstrapServers = options.BootstrapServers,
        };
        
        _producer = new ProducerBuilder<string, TMessage>(config).Build();
        _topic = options.Topic;
    }
    
    public async Task ProduceAsync(TMessage message, CancellationToken ct = default)
    {
        await _producer.ProduceAsync(_topic, new Message<string, TMessage>()
        {
            Value = message
        },  ct);
    }

    public void Dispose()
    {
        _producer.Dispose();
    }
}