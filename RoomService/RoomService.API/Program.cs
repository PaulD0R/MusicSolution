using RoomService.API.Controllers;
using RoomService.API.Extensions;
using Serilog;

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
app.MapHub<RoomHub>("/room-hub");

app.Run();