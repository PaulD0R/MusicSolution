namespace MusicService.Infrastructure.Options;

public class KafkaConsumerOptions
{
    public string Topic { get; set; } = null!;
    public string BootstrapServers { get; set; } = null!;
    public string GroupId { get; set; } = null!;
}