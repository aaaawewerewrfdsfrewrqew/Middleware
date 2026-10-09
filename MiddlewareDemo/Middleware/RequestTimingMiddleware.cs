using System.Diagnostics;

namespace MiddlewareDemo.Middleware;

public class RequestTimingMiddleware
{
    private const string TimeFormat = "yyyy-MM-dd HH:mm:ss.fff";

    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var receivedAt = DateTime.Now;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation("Запит {Method} {Path} отримано о {ReceivedAt}",
            context.Request.Method, context.Request.Path, receivedAt.ToString(TimeFormat));

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var sentAt = DateTime.Now;

            _logger.LogInformation(
                "Відповідь на {Method} {Path} відправлено о {SentAt}. Тривалість: {ElapsedMs} мс",
                context.Request.Method, context.Request.Path, sentAt.ToString(TimeFormat), stopwatch.ElapsedMilliseconds);
        }
    }
}

public static class RequestTimingMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestTiming(this IApplicationBuilder app)
        => app.UseMiddleware<RequestTimingMiddleware>();
}
