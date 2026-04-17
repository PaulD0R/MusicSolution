using CommentService.Application.Interfaces.Factories;
using CommentService.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CommentService.Infrastructure.Factories;

public class CommentFileServiceFactory(IServiceScopeFactory scopeFactory) : IFactory<CommentFileService>, IDisposable
{
    private readonly IServiceScope _scope = scopeFactory.CreateScope();
    
    public CommentFileService Create() =>  _scope.ServiceProvider.GetRequiredService<CommentFileService>();

    public void Dispose() => _scope.Dispose();
}