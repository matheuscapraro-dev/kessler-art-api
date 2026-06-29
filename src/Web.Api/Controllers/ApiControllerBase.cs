using FluentResults;
using Kessler.Domain.Common.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Kessler.Web.Api.Controllers;

/// <summary>
/// Base dos controllers: traduz um <see cref="Result"/> do FluentResults em IActionResult,
/// mapeando o código do <see cref="AppError"/> para o status HTTP + ProblemDetails (PT-BR).
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult HandleResult<T>(Result<T> result) =>
        result.IsSuccess ? Ok(result.Value) : ToProblem(result.ToResult());

    protected IActionResult HandleResult(Result result) =>
        result.IsSuccess ? NoContent() : ToProblem(result);

    protected IActionResult HandleCreated<T>(Result<T> result, string actionName, Func<T, object> routeValues) =>
        result.IsSuccess
            ? CreatedAtAction(actionName, routeValues(result.Value), result.Value)
            : ToProblem(result.ToResult());

    private ObjectResult ToProblem(Result result)
    {
        var status = StatusCodes.Status400BadRequest;
        string? errorCode = null;

        foreach (var error in result.Errors)
        {
            errorCode = error switch
            {
                AppError appError => appError.Code,
                _ when error.Metadata.TryGetValue("code", out var code) => code?.ToString(),
                _ => errorCode
            };

            if (errorCode is null)
                continue;

            status = errorCode switch
            {
                "NotFound" => StatusCodes.Status404NotFound,
                "Conflict" => StatusCodes.Status409Conflict,
                "Forbidden" => StatusCodes.Status403Forbidden,
                "Unauthorized" => StatusCodes.Status401Unauthorized,
                "DomainRule" => StatusCodes.Status422UnprocessableEntity,
                _ => status
            };

            if (status != StatusCodes.Status400BadRequest)
                break;
        }

        var problem = new ProblemDetails
        {
            Title = status switch
            {
                StatusCodes.Status404NotFound => "Recurso não encontrado",
                StatusCodes.Status409Conflict => "Conflito detectado",
                StatusCodes.Status403Forbidden => "Acesso negado",
                StatusCodes.Status401Unauthorized => "Falha na autenticação",
                StatusCodes.Status422UnprocessableEntity => "Operação não processável",
                _ => "Erro de validação"
            },
            Status = status,
            Detail = string.Join("; ", result.Errors.Select(e => e.Message)),
            Instance = HttpContext?.Request.Path
        };

        if (errorCode is not null)
            problem.Extensions["errorCode"] = errorCode;

        return StatusCode(status, problem);
    }
}
