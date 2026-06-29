using Kessler.Application.Abstractions.Repositories;
using Kessler.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Kessler.Infrastructure.Persistence.Repositories;

internal sealed class ProductRepository(KesslerDbContext db) : IProductRepository
{
    public void Add(Product product) => db.Products.Add(product);

    public void Remove(Product product) => db.Products.Remove(product);

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default) =>
        db.Products
            .AsNoTracking()
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsPublished, cancellationToken);

    public async Task<IReadOnlyList<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        return await db.Products.Where(p => idList.Contains(p.Id)).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> ListAsync(ProductFilter filter, CancellationToken cancellationToken = default)
    {
        var query = db.Products.AsNoTracking().Include(p => p.Images).AsQueryable();

        if (filter.PublishedOnly)
            query = query.Where(p => p.IsPublished);

        if (filter.FeaturedOnly)
            query = query.Where(p => p.IsFeatured);

        if (filter.Availability is { } availability)
            query = query.Where(p => p.Availability == availability);

        if (!string.IsNullOrWhiteSpace(filter.CategorySlug))
            query = query.Where(p => db.Categories.Any(c => c.Id == p.CategoryId && c.Slug == filter.CategorySlug));

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var term = $"%{filter.Search.Trim()}%";
            query = query.Where(p => EF.Functions.ILike(p.Name, term)
                || (p.Description != null && EF.Functions.ILike(p.Description, term)));
        }

        return await query
            .OrderByDescending(p => p.IsFeatured)
            .ThenByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsBySlugAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        db.Products.AnyAsync(p => p.Slug == slug && (excludeId == null || p.Id != excludeId), cancellationToken);
}
