    using CommentService.Application.Interfaces.Messages;
    using CommentService.Infrastructure.Options;
    using Confluent.Kafka;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    namespace CommentService.Infrastructure.Kafka;

    public class KafkaConsumer<TMessage> : BackgroundService
    {
        private readonly IConsumer<string, TMessage> _consumer;
        private readonly IMessageHandler<TMessage> _handler;
        private readonly ILogger<KafkaConsumer<TMessage>> _logger;

        public KafkaConsumer(
            IMessageHandler<TMessage> handler, 
            IOptionsMonitor<KafkaConsumerOptions> optionsMonitor, 
            ILogger<KafkaConsumer<TMessage>> logger)
        {
            var options = optionsMonitor.Get(typeof(TMessage).Name);
            _handler = handler;
            _logger = logger;

            var config = new ConsumerConfig
            {
                BootstrapServers = options.BootstrapServers,
                GroupId = options.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            
            _consumer = new ConsumerBuilder<string, TMessage>(config)
                .SetValueDeserializer(new KafkaDeserializer<TMessage>()).Build();
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
                    _logger.LogInformation($"Message: {result.Message.Value}");
                }
            }
            catch(Exception e)
            {
                _logger.LogError("Handler Error: {EMessage}", e.Message);
            }
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() => ConsumeAsync(stoppingToken), stoppingToken);
        }
    }