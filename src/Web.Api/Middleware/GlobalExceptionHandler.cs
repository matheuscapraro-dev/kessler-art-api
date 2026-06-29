using Kessler.Domain.Common.Errors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Kessler.Web.Api.Middleware;

/// <summary>
/// Converte exceções que escapam do pipeline em ProblemDetails. Invariantes de domínio
/// viram 422; o resto vira 500 (e é logado).
/// </summary>
internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, detail) = exception switch
        {
            DomainException domain => (
                StatusCodes.Status422UnprocessableEntity,
                "Operação não processável",
                domain.Message),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Erro interno",
                "Ocorreu um erro inesperado.")
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Exceção não tratada");

        var problem = new ProblemDetails
        {
            Title = title,
            Status = status,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        httpContext.Response.StatusCode = status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
