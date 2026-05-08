using Serilog;
using UserService.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddLoggers(builder.Configuration.GetSection("Logs"))
    .AddAppInfrastructure(builder.Configuration)
    .AddAppTelemetry()
    .AddBusinessServices(builder.Configuration)
    .AddSecurityConfiguration(builder.Configuration.GetSection("Jwt"))
    .AddWebPresentation(builder.Configuration);

builder.Host.UseSerilog();

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors("YarpPolice");
app.UseAuthentication();
app.UseAuthorization();
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapControllers(); 

app.Run();