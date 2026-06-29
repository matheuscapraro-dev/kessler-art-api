using Kessler.Domain.Common.Errors;

namespace Kessler.Application.Common.Errors;

/// <summary>
/// Fábrica de erros de aplicação. Retorna <see cref="AppError"/> que o FluentResults
/// converte implicitamente em <c>Result</c> ou <c>Result&lt;T&gt;</c> falho:
/// <code>return AppResults.NotFound("Peça não encontrada.");</code>
/// </summary>
public static class AppResults
{
    public static AppError NotFound(string message) => new("NotFound", message, ErrorType.NotFound);
    public static AppError Conflict(string message) => new("Conflict", message, ErrorType.Conflict);
    public static AppError Validation(string message) => new("Validation", message, ErrorType.Validation);
    public static AppError Forbidden(string message) => new("Forbidden", message, ErrorType.Forbidden);
    public static AppError Unauthorized(string message) => new("Unauthorized", message, ErrorType.Unauthorized);
    public static AppError DomainRule(string message) => new("DomainRule", message, ErrorType.DomainRule);
}
