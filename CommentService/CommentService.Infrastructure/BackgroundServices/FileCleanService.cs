using CommentService.Application.Interfaces.Factories;
using CommentService.Application.Interfaces.Services;
using CommentService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CommentService.Infrastructure.BackgroundServices;

public class FileCleanupWorker(IFactory<ICommentService> factory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await factory.Create().CleanAsync();
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }
}