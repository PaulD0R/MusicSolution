namespace MusicService.Infrastructure.Options;

public class KafkaProducerOptions
{
    public string Topic { get; set; } = null!;
    public string BootstrapServers { get; set; } = null!;
}