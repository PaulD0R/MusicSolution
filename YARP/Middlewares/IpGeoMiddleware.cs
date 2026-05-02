using MaxMind.GeoIP2;

namespace YARP.Middlewares;

public class IpGeoMiddleware(RequestDelegate next, IGeoIP2DatabaseReader reader)
{
    private readonly IEnumerable<string> _allowedCountries = ["RU", "BY"];
    
    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString();

        if (!string.IsNullOrEmpty(ip))
        {
            var country = reader.Country(ip).Country.IsoCode;

            if (!_allowedCountries.Contains(country))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync($"1984");
                return;
            }
        }
        
        await next(context);
    }
}