namespace MiddlewareDemo.Middleware;

public class RequestCounterMiddleware
{
    private static long _requestCount;

    private readonly RequestDelegate _next;

    public RequestCounterMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var number = Interlocked.Increment(ref _requestCount);

        Console.WriteLine($"Request #{number}: {context.Request.Method} {context.Request.Path}");

        await _next(context);
    }
}

public static class RequestCounterMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestCounter(this IApplicationBuilder app)
        => app.UseMiddleware<RequestCounterMiddleware>();
}
