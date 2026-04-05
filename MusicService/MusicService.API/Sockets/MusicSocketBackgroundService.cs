using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Options;
using MusicService.API.Options;
using MusicService.Application.Interfaces.Factories;
using MusicService.Application.Interfaces.Services;

namespace MusicService.API.Sockets;

public class MusicSocketBackgroundService(
    IFactory<IMusicService> musicServiceFactory,
    IOptions<SocketOptions> options,
    ILogger<MusicSocketBackgroundService> logger)
    : BackgroundService
{
    private const int BufferSize = 1400;
    private readonly ConcurrentDictionary<EndPoint, CancellationTokenSource> _activeSessions = new();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        var buffer = new byte[256];
        var localEndpoint = new IPEndPoint(IPAddress.Any, options.Value.Port);
        socket.Bind(localEndpoint);
        
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var result = await socket.ReceiveFromAsync(buffer, SocketFlags.None, localEndpoint, stoppingToken);
                
                if (_activeSessions.TryRemove(result.RemoteEndPoint, out var oldCt))
                {
                    await oldCt.CancelAsync();
                    oldCt.Dispose();
                }

                _activeSessions[result.RemoteEndPoint] = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                
                _ = StartTaskAsync(socket, buffer.AsMemory(0, result.ReceivedBytes), 
                    result.RemoteEndPoint, stoppingToken);
                _activeSessions.Remove(result.RemoteEndPoint, out _);
            }
        }
        catch(Exception ex)
        {
            logger.LogInformation(ex.Message);
        }
    }

    private async Task StartTaskAsync(Socket socket, ReadOnlyMemory<byte> data, EndPoint endPoint,
        CancellationToken ct)
    {
        try
        {
            if (data.Length < 24) return;   
            var id = new Guid(data.Span[..16]);
            logger.LogInformation("UDP Find music {id}", id);
            var music = await musicServiceFactory.Create().GetByIdAsync(id); 
            
            var offset = BitConverter.ToInt32(data.Span[16..24]);
            var packetBuffer = new byte[BufferSize];
    
            await using (music.Stream)
            {
                if (music.Stream.CanSeek) music.Stream.Seek(offset, SeekOrigin.Begin);

                var timeDelay = BufferSize * 1000 / (music.Bitrate * 128);
                int bytesRead;
                while ((bytesRead = await music.Stream.ReadAsync(packetBuffer, ct)) > 0)
                {
                    await socket.SendToAsync(packetBuffer.AsMemory(0, bytesRead), SocketFlags.None, 
                        endPoint, ct);
                    await Task.Delay(timeDelay, ct);
                }
                
                await socket.SendToAsync(Array.Empty<byte>(), endPoint, ct); 
                logger.LogInformation("UDP Streaming finished");
            }
        }
        catch(Exception ex)
        {
            logger.LogWarning(ex.Message);
        }
    }
}