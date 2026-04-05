namespace MusicService.Application.Interfaces.Messages;

public interface IMessageHandler<in TMessage>
{
    Task HandleAsync(TMessage message, CancellationToken ct = default);
}