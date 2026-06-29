namespace Kessler.Domain.Common.Errors;

/// <summary>
/// Categoria semântica de um <see cref="AppError"/>. A camada Web mapeia para o
/// status HTTP correspondente (ver ApiControllerBase).
/// </summary>
public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Forbidden,
    Unauthorized,
    DomainRule,
    Unexpected
}
