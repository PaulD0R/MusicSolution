using MaxMind.GeoIP2;
using Microsoft.AspNetCore.Http.Features;
using YARP.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IGeoIP2DatabaseReader>(_ => 
    new DatabaseReader(builder.Configuration["GeoIP:Path"]!));

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 100 * 1024 * 1024);
builder.Services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = 100 * 1024 * 1024);

var app = builder.Build();

app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);
    await next(context);
    logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
});

app.UseForwardedHeaders();
app.UseMiddleware<IpGeoMiddleware>();
app.MapReverseProxy();
app.Run();
