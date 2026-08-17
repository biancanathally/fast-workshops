using FastWorkshops.Domain.Exceptions;

namespace FastWorkshops.Api.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IHostEnvironment env)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            await EscreverAsync(context, StatusCodes.Status404NotFound,
                "Recurso não encontrado", ex.Message);
        }
        catch (ConflictException ex)
        {
            await EscreverAsync(context, StatusCodes.Status409Conflict,
                "Conflito de estado", ex.Message);
        }
        catch (OperationCanceledException)
        {
            // Cliente desistiu (ex.: debounce do campo de busca). Não é erro do servidor.
            context.Response.StatusCode = 499;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro não tratado em {Path}", context.Request.Path);

            var detalhe = env.IsDevelopment()
                ? $"{ex.GetType().Name}: {ex.Message}"
                : "Ocorreu um erro inesperado.";

            await EscreverAsync(context, StatusCodes.Status500InternalServerError,
                "Erro interno", detalhe);
        }
    }

    private static Task EscreverAsync(HttpContext ctx, int status, string titulo, string detalhe)
    {
        ctx.Response.StatusCode = status;
        ctx.Response.ContentType = "application/problem+json";

        return ctx.Response.WriteAsJsonAsync(new
        {
            type = $"https://httpstatuses.io/{status}",
            title = titulo,
            status,
            detail = detalhe,
            traceId = ctx.TraceIdentifier
        });
    }
}
