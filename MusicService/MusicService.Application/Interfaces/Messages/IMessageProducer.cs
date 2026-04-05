namespace MusicService.Application.Interfaces.Messages;

public interface IMessageProducer<in TMessage> : IDisposable
{
    Task ProduceAsync(TMessage message, CancellationToken ct = default);
}