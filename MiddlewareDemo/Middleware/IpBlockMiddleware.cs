using System.Net;

namespace MiddlewareDemo.Middleware;

public class IpBlockMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IpBlockMiddleware> _logger;
    private readonly HashSet<IPAddress> _blockedIps;

    public IpBlockMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<IpBlockMiddleware> logger)
    {
        _next = next;
        _logger = logger;

        var configured = configuration.GetSection("BlockedIps").Get<string[]>() ?? Array.Empty<string>();

        _blockedIps = configured
            .Select(ip => IPAddress.TryParse(ip, out var parsed) ? Normalize(parsed) : null)
            .Where(ip => ip is not null)
            .Select(ip => ip!)
            .ToHashSet();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var remoteIp = context.Connection.RemoteIpAddress;

        if (remoteIp is not null && _blockedIps.Contains(Normalize(remoteIp)))
        {
            _logger.LogWarning("Заблокований IP {Ip} спробував отримати доступ до {Path}", remoteIp, context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("403 Forbidden: your IP is blocked.");
            return;
        }

        await _next(context);
    }

    private static IPAddress Normalize(IPAddress ip) => ip.IsIPv4MappedToIPv6 ? ip.MapToIPv4() : ip;
}

public static class IpBlockMiddlewareExtensions
{
    public static IApplicationBuilder UseIpBlocking(this IApplicationBuilder app)
        => app.UseMiddleware<IpBlockMiddleware>();
}
