namespace UserService.Infrastructure.Options;

public class KafkaProducerOptions
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
}