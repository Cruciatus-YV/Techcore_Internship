using System.Diagnostics;

namespace Techcore_Internship.WebApi.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly HashSet<string> _ignoredPaths;

    public RequestLoggingMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _ignoredPaths = new HashSet<string>{"/swagger/v1/swagger.json" };
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (_ignoredPaths.Contains(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var startTime = DateTime.UtcNow;
        var stopwatch = Stopwatch.StartNew();

        Console.WriteLine($"[{startTime:HH:mm:ss}] Started {context.Request.Method} {context.Request.Path}");

        await _next(context);

        stopwatch.Stop();
        var endTime = DateTime.UtcNow;

        Console.WriteLine($"[{endTime:HH:mm:ss}] Completed {context.Request.Method} {context.Request.Path} - " +
                         $"{context.Response.StatusCode} in {stopwatch.ElapsedMilliseconds}ms");
    }
}
