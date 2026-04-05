using Microsoft.Extensions.DependencyInjection;
using MusicService.Application.Interfaces.Factories;
using MusicService.Application.Interfaces.Services;

namespace MusicService.Infrastructure.Factories;

public class MusicServiceFactory(IServiceScopeFactory serviceScopeFactory) : IFactory<IMusicService>, IDisposable
{
    private readonly IServiceScope _serviceScope =  serviceScopeFactory.CreateScope();
    
    public IMusicService Create() => _serviceScope.ServiceProvider.GetService<IMusicService>()!;

    public void Dispose() =>  _serviceScope.Dispose();
}