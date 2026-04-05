using UserService.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppInfrastructure(builder.Configuration)
    .AddAppTelemetry()
    .AddBusinessServices(builder.Configuration)
    .AddSecurityConfiguration(builder.Configuration.GetSection("Jwt"))
    .AddWebPresentation(builder.Configuration);


var app = builder.Build();

app.UseExceptionHandler();
app.UseCors("YarpPolice");
app.UseAuthentication();
app.UseAuthorization();
app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.MapControllers(); 

app.Run();