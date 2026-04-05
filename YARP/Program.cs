var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// app.Use(async (context, next) =>
// {
//     var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
//     logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
//     await next(context);
//     logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
// });

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    var path = context.Request.Path.ToString().ToLower();

    var shouldLogBody = path.Contains("/user-service") && 
                        !(context.Request.ContentType?.StartsWith("multipart") ?? false);

    if (shouldLogBody)
    {
        context.Request.EnableBuffering();

        using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        logger.LogInformation("USER-SERVICE Request: {Method} {Path} | Body: {Body}", 
            context.Request.Method, context.Request.Path, body);
            
        context.Request.Body.Position = 0;
    }
    else
    {
        logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
    }

    await next(context);

    logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
});

app.MapReverseProxy();
app.Run();
