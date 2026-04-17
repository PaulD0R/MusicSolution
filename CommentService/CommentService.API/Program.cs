using CommentService.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppInfrastructure(builder.Configuration)
    .AddAppTelemetry()
    .AddSecurityConfiguration(builder.Configuration.GetSection("Jwt"))
    .AddBusinessServices(builder.Configuration)
    .AddWebPresentation(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors("YarpPolice");
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapControllers();

app.Run();