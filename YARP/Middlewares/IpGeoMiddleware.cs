using MaxMind.GeoIP2;

namespace YARP.Middlewares;

public class IpGeoMiddleware(RequestDelegate next, IGeoIP2DatabaseReader reader)
{
    private readonly IEnumerable<string> _allowedCountries = ["RU", "BY"];
    
    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Пытаемся получить IP. Если работаем за прокси, 
        // RemoteIpAddress будет корректным только при настроенном UseForwardedHeaders
        var remoteIp = context.Connection.RemoteIpAddress;
    
        if (remoteIp != null)
        {
            // Пропускаем проверку для локальных IP (опционально)
            if (IPAddress.IsLoopback(remoteIp)) 
            {
                await next(context);
                return;
            }

            if (reader.TryCountry(remoteIp, out var response))
            {
                var country = response.Country.IsoCode;
                logger.LogInformation("IP {ip} identified as {country}", remoteIp, country);

                // 2. Логика блокировки
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
                // 3. Что делать, если IP не найден в базе? 
                // Сейчас код просто пропускает (country == null). 
                // Если нужна жесткая политика — блокируй и здесь.
                logger.LogDebug("IP {ip} not found in GeoDB", remoteIp);
            }
        }

        await next(context);
    }
}