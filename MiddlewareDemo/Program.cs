using MiddlewareDemo.Middleware;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Warning)
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}"));

var app = builder.Build();

app.UseRequestCounter();    
app.UseRequestTiming();    
app.UseConsoleLogging();   
app.UseIpBlocking(); 
app.UseApiKeyValidation();  

app.MapGet("/", () => "Middleware demo is running");

app.MapGet("/api/products", () => new[]
{
    new { Id = 1, Name = "Laptop", Price = 1200m },
    new { Id = 2, Name = "Phone",  Price = 800m },
    new { Id = 3, Name = "Tablet", Price = 450m }
});

app.MapGet("/api/health", () => Results.Ok(new { status = "healthy" }));

try
{
    Log.Information("Application starting");
    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
