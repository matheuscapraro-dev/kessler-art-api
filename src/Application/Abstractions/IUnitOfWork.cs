namespace Kessler.Application.Abstractions;

/// <summary>
/// Confirma as alterações pendentes do agregado em uma única transação.
/// Implementado pelo DbContext na Infrastructure.
/// </summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
