using System.Diagnostics;

namespace MiddlewareDemo.Middleware;

public class ConsoleLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public ConsoleLoggingMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var request = context.Request;
            var url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";

            Console.WriteLine($"{request.Method} {url} -> {context.Response.StatusCode} ({stopwatch.ElapsedMilliseconds} ms)");
        }
    }
}

public static class ConsoleLoggingMiddlewareExtensions
{
    public static IApplicationBuilder UseConsoleLogging(this IApplicationBuilder app)
        => app.UseMiddleware<ConsoleLoggingMiddleware>();
}
