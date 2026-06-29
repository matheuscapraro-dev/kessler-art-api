using FluentResults;
using FluentValidation;
using Kessler.Domain.Common.Errors;
using MediatR;

namespace Kessler.Application.Behaviors;

/// <summary>
/// Roda os <see cref="IValidator{T}"/> do request antes do handler. Em vez de lançar
/// exceção, converte as falhas em um <c>Result</c> falho com erros "Validation" —
/// o ApiControllerBase devolve 400 com ProblemDetails.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : ResultBase, new()
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = results
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next(cancellationToken);

        var response = new TResponse();
        foreach (var failure in failures)
            response.Reasons.Add(new AppError("Validation", failure.ErrorMessage, ErrorType.Validation));

        return response;
    }
}
