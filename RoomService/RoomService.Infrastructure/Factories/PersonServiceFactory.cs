using Microsoft.Extensions.DependencyInjection;
using RoomService.Application.Interfaces.Factories;
using RoomService.Application.Interfaces.Services;

namespace RoomService.Infrastructure.Factories;

public class PersonServiceFactory(IServiceScopeFactory serviceScopeFactory) : IFactory<IPersonService>, IDisposable
{
    private readonly IServiceScope _serviceScope =  serviceScopeFactory.CreateScope();
    
    public IPersonService Create() => _serviceScope.ServiceProvider.GetService<IPersonService>()!;

    public void Dispose() =>  _serviceScope.Dispose();
}