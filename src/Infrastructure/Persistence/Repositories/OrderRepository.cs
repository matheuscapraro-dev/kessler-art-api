using Kessler.Application.Abstractions.Repositories;
using Kessler.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace Kessler.Infrastructure.Persistence.Repositories;

internal sealed class OrderRepository(KesslerDbContext db) : IOrderRepository
{
    public void Add(Order order) => db.Orders.Add(order);

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id, cancellationToken);

    public Task<Order?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        db.Orders.AsNoTracking().Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Code == code, cancellationToken);

    public async Task<IReadOnlyList<Order>> ListAsync(OrderStatus? status, CancellationToken cancellationToken = default)
    {
        var query = db.Orders.AsNoTracking().Include(o => o.Items).AsQueryable();
        if (status is { } s)
            query = query.Where(o => o.Status == s);

        return await query.OrderByDescending(o => o.CreatedAt).ToListAsync(cancellationToken);
    }
}

internal sealed class CommissionRepository(KesslerDbContext db) : ICommissionRepository
{
    public void Add(CommissionRequest commission) => db.Commissions.Add(commission);

    public void Remove(CommissionRequest commission) => db.Commissions.Remove(commission);

    public Task<CommissionRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Commissions.Include(c => c.ReferenceImages).Include(c => c.Tasks)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task<CommissionRequest?> GetByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        db.Commissions.AsNoTracking().Include(c => c.ReferenceImages).Include(c => c.Tasks)
            .FirstOrDefaultAsync(c => c.Code == code, cancellationToken);

    public async Task<IReadOnlyList<CommissionRequest>> ListAsync(CommissionStatus? status, CancellationToken cancellationToken = default)
    {
        var query = db.Commissions.AsNoTracking()
            .Include(c => c.ReferenceImages)
            .Include(c => c.Tasks)
            .AsQueryable();
        if (status is { } s)
            query = query.Where(c => c.Status == s);

        return await query
            .OrderBy(c => c.Position)
            .ThenByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
