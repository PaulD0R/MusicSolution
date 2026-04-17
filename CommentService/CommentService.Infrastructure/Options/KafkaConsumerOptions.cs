namespace CommentService.Infrastructure.Options;

public class KafkaConsumerOptions
{
    public string BootstrapServers { get; set; } = null!;
    public string Topic { get; set; } = null!;
    public string GroupId { get; set; } = null!;
}