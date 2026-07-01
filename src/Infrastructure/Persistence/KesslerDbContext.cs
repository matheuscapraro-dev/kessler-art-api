using Kessler.Application.Abstractions;
using Kessler.Domain.Catalog;
using Kessler.Domain.Common;
using Kessler.Domain.Content;
using Kessler.Domain.Identity;
using Kessler.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Kessler.Infrastructure.Persistence;

public sealed class KesslerDbContext(DbContextOptions<KesslerDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<CommissionRequest> Commissions => Set<CommissionRequest>();
    public DbSet<SiteContent> SiteContent => Set<SiteContent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(KesslerDbContext).Assembly);

        // Os Ids (Guid) são gerados no domínio (factories). Sem isso, o EF assume
        // que ele gera o Guid e, ao descobrir uma entidade-filha do agregado com o
        // Id já preenchido, tenta UPDATE em vez de INSERT (DbUpdateConcurrencyException).
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var primaryKey = entityType.FindPrimaryKey();
            if (primaryKey is null)
                continue;

            foreach (var property in primaryKey.Properties)
            {
                if (property.ClrType == typeof(Guid))
                    property.ValueGenerated = ValueGenerated.Never;
            }
        }

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
