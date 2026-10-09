using System.Security.Cryptography;
using System.Text;

namespace MiddlewareDemo.Middleware;

public class ApiKeyMiddleware
{
    private const string HeaderName = "X-API-KEY";

    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyMiddleware> _logger;
    private readonly byte[] _expectedKey;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        _logger = logger;

        var key = configuration["ApiKey"]
                  ?? throw new InvalidOperationException("ApiKey is not configured in appsettings.json");
        _expectedKey = Encoding.UTF8.GetBytes(key);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out var providedValue))
        {
            _logger.LogWarning("Запит {Path} без заголовка {Header}", context.Request.Path, HeaderName);
            await RejectAsync(context, "API key is missing.");
            return;
        }

        var providedKey = Encoding.UTF8.GetBytes(providedValue.ToString());

        if (!CryptographicOperations.FixedTimeEquals(providedKey, _expectedKey))
        {
            _logger.LogWarning("Запит {Path} з неправильним API ключем", context.Request.Path);
            await RejectAsync(context, "API key is invalid.");
            return;
        }

        await _next(context);
    }

    private static async Task RejectAsync(HttpContext context, string message)
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync($"401 Unauthorized: {message}");
    }
}

public static class ApiKeyMiddlewareExtensions
{
    public static IApplicationBuilder UseApiKeyValidation(this IApplicationBuilder app)
        => app.UseMiddleware<ApiKeyMiddleware>();
}
