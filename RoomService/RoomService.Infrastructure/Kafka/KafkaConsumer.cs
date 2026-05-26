using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RoomService.Application.Interfaces.Messages;
using RoomService.Infrastructure.Options;

namespace RoomService.Infrastructure.Kafka;

public class KafkaConsumer<TMessage> : BackgroundService
{
    private readonly IConsumer<string, TMessage> _consumer;
    private readonly IMessageHandler<TMessage> _handler;

    public KafkaConsumer(
        IOptionsMonitor<KafkaConsumerOptions> optionsSnapshot,
        IMessageHandler<TMessage> handler)
    {
        var options = optionsSnapshot.Get(typeof(TMessage).Name);
        var config = new ConsumerConfig
        {
            BootstrapServers = options.BootstrapServers,
            GroupId = options.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };
        
        _handler = handler;
        _consumer = new ConsumerBuilder<string, TMessage>(config)
                .SetValueDeserializer(new KafkaJsonDeserializer<TMessage>()).Build();
        _consumer.Subscribe(options.Topic);
    }
    
    private async Task? ConsumeAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var result = _consumer.Consume(token);
                    await _handler.HandleAsync(result.Message.Value, token);
                }
                catch
                {
                    // ignored
                }
            }
        }
        catch
        {
            _consumer.Close();
        }
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken);
    }
}