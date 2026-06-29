using Kessler.Application.Abstractions;
using Kessler.Domain.Catalog;
using Kessler.Domain.Common;
using Kessler.Domain.Identity;
using Kessler.Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace Kessler.Infrastructure.Persistence;

public sealed class KesslerDbContext(DbContextOptions<KesslerDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<CommissionRequest> Commissions => Set<CommissionRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KesslerDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditTimestamps()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = now;
            else if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = now;
        }
    }
}
