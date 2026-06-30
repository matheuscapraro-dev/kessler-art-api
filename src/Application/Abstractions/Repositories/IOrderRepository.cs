using Kessler.Domain.Orders;

namespace Kessler.Application.Abstractions.Repositories;

public interface IOrderRepository
{
    void Add(Order order);
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Order?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> ListAsync(OrderStatus? status, CancellationToken cancellationToken = default);
}

public interface ICommissionRepository
{
    void Add(CommissionRequest commission);
    void Remove(CommissionRequest commission);
    Task<CommissionRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<CommissionRequest?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<CommissionRequest>> ListAsync(CommissionStatus? status, CancellationToken cancellationToken = default);
}
