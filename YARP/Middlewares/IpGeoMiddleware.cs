using MaxMind.GeoIP2;

namespace YARP.Middlewares;

public class IpGeoMiddleware(RequestDelegate next, IGeoIP2DatabaseReader reader, ILogger<IpGeoMiddleware> logger)
{
    private readonly IEnumerable<string> _allowedCountries = ["RU", "BY"];
    
    public async Task InvokeAsync(HttpContext context)
    {
        var remoteIp = context.Connection.RemoteIpAddress;
    
        if (remoteIp != null)
        {
            if (reader.TryCountry(remoteIp, out var response))
            {
                var country = response.Country.IsoCode;
                logger.LogInformation("IP {ip} identified as {country}", remoteIp, country);

                if (!_allowedCountries.Contains(country))
                {
                    logger.LogWarning("Access denied for country: {country}", country);
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("1984");
                    return;
                }
            }
            else
            {
                logger.LogDebug("IP {ip} not found in GeoDB", remoteIp);
            }
        }

        await next(context);
    }
}