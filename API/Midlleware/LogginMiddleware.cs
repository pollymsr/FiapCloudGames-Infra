namespace FiapCloudGames.API.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Requisição: {Method} {Path}", context.Request.Method, context.Request.Path);

        var start = DateTime.UtcNow;
        await _next(context);
        var duration = DateTime.UtcNow - start;

        _logger.LogInformation("Resposta: {StatusCode} em {Duration}ms", context.Response.StatusCode, duration.TotalMilliseconds);
    }
}