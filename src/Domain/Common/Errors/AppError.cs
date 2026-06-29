using FluentResults;

namespace Kessler.Domain.Common.Errors;

/// <summary>
/// Erro tipado do domínio/aplicação carregado por um <see cref="Result"/> do FluentResults.
/// O <see cref="Code"/> é exposto no campo "code" dos metadados para o ApiControllerBase
/// traduzir em status HTTP + ProblemDetails.
/// </summary>
public sealed class AppError : Error
{
    public string Code { get; }
    public ErrorType Type { get; }

    public AppError(string code, string message, ErrorType type) : base(message)
    {
        Code = code;
        Type = type;
        Metadata.Add("code", code);
    }
}
