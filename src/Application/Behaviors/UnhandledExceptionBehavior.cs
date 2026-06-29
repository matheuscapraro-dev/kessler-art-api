using MediatR;
using Microsoft.Extensions.Logging;

namespace Kessler.Application.Behaviors;

/// <summary>
/// Rede de segurança do pipeline: loga qualquer exceção não tratada de um handler
/// e a repropaga para o middleware global da Web converter em 500.
/// </summary>
public sealed class UnhandledExceptionBehavior<TRequest, TResponse>(
    ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro não tratado processando {RequestName}", typeof(TRequest).Name);
            throw;
        }
    }
}
