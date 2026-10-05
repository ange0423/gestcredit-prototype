using System.Diagnostics;

namespace GestCredit.Api.Middlewares;

// Mesure et journalise le temps de traitement de chaque requête.
// Ne journalise JAMAIS le corps de la requête/réponse : il pourrait contenir
// des données sensibles (mot de passe en clair avant hachage, etc.).
public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimingMiddleware> _logger;

    public RequestTimingMiddleware(RequestDelegate next, ILogger<RequestTimingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        _logger.LogInformation(
            "{Method} {Path} -> {StatusCode} en {ElapsedMilliseconds} ms",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode,
            stopwatch.ElapsedMilliseconds);
    }
}