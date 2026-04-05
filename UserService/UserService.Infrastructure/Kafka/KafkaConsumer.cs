using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserService.Application.Interfaces.Messages;
using UserService.Infrastructure.Options;

namespace UserService.Infrastructure.Kafka;

public class KafkaConsumer<TMessage> : BackgroundService
{
    private readonly IConsumer<string, TMessage> _consumer;
    private readonly IMessageHandler<TMessage> _handler;
    private readonly ILogger<KafkaConsumer<TMessage>> _logger;
    
    public KafkaConsumer(
        IOptionsMonitor<KafkaConsumerOptions> optionsMonitor, 
        IMessageHandler<TMessage> handler, 
        ILogger<KafkaConsumer<TMessage>> logger)
    {
        _logger = logger;
        
        var options = optionsMonitor.Get(typeof(TMessage).Name);
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
            _logger.LogError("Consume error");
        }
    }
    
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken);
    }
}