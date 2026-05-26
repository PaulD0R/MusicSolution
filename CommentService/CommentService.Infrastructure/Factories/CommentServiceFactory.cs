using CommentService.Application.Interfaces.Factories;
using CommentService.Application.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CommentService.Infrastructure.Factories;

public class CommentServiceFactory(IServiceScopeFactory scopeFactory) : IFactory<ICommentService>, IDisposable
{
    private readonly IServiceScope _scope = scopeFactory.CreateScope();
    
    public ICommentService Create() => _scope.ServiceProvider.GetRequiredService<ICommentService>();

    public void Dispose() => _scope.Dispose();
}