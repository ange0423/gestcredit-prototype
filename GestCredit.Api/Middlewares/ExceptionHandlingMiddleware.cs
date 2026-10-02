using System.Text.Json;

namespace GestCredit.Api.Middlewares;

// Capture toute exception non gérée, n'importe où dans le pipeline, et la
// transforme en réponse JSON structurée. C'est ÇA qui permet de retirer tous
// les try/catch des controllers : une seule gestion d'erreur centralisée.
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Identifiant unique de corrélation : permet de retrouver cette
            // exception précise dans les logs, à partir du message renvoyé au client.
            string correlationId = Guid.NewGuid().ToString();

            _logger.LogError(ex, "Erreur non gérée. CorrelationId: {CorrelationId}", correlationId);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var problem = new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                title = "Une erreur interne est survenue.",
                status = 500,
                correlationId
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problem));
        }
    }
}