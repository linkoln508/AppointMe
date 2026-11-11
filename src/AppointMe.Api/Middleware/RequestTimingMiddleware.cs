using System.Diagnostics;

namespace AppointMe.Api.Middleware;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;
    private readonly int _slowThresholdMs;
    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _slowThresholdMs = configuration.GetValue<int>("RequestTiming:SlowThresholdMs", 500);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-Elapsed-Ms"] = stopwatch.ElapsedMilliseconds.ToString();
            return Task.CompletedTask;
        });
        await _next(context);
        stopwatch.Stop();
        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        if (elapsedMilliseconds > _slowThresholdMs)
        {
            _logger.LogWarning("Slow request: {Method} {Path} took {ElapsedMilliseconds} ms", context.Request.Method, context.Request.Path, elapsedMilliseconds);
        }
    }
}
