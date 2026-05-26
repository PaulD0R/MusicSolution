using CommentService.Application.Interfaces.Factories;
using CommentService.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CommentService.Infrastructure.Factories;

public class PersonServiceFactory(IServiceScopeFactory scopeFactory) : IFactory<IPersonService>, IDisposable
{
    private readonly IServiceScope _scope = scopeFactory.CreateScope();
    
    public IPersonService Create() => _scope.ServiceProvider.GetRequiredService<IPersonService>();

    public void Dispose() => _scope.Dispose();
}