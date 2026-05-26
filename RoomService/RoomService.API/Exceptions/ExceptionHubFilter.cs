using Microsoft.AspNetCore.SignalR;

namespace RoomService.API.Exceptions;

public class ExceptionHubFilter : IHubFilter
{
    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext context, 
        Func<HubInvocationContext, ValueTask<object?>> next)
    {
        try
        {
            return await next(context);
        }
        catch (Exception ex)
        {
            await context.Hub.Clients.Caller.SendAsync("Error", new 
            { 
                message = "Failed",
                details = ex.Message
            });

            return null;
        }
    }
}