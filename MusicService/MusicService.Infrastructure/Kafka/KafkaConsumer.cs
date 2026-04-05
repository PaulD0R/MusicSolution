using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MusicService.Application.Interfaces.Messages;
using MusicService.Infrastructure.Options;

namespace MusicService.Infrastructure.Kafka;

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
        _consumer = new ConsumerBuilder<string, TMessage>(config).Build();
        _consumer.Subscribe(options.Topic);
    }
    
    private async Task? ConsumeAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                var result = _consumer.Consume(token);
                await _handler.HandleAsync(result.Message.Value, token);
            }
        }
        catch
        {
            //
        }
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken);
    }
}