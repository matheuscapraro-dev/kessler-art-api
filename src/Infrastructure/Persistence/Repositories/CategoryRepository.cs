using Kessler.Application.Abstractions.Repositories;
using Kessler.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Kessler.Infrastructure.Persistence.Repositories;

internal sealed class CategoryRepository(KesslerDbContext db) : ICategoryRepository
{
    public void Add(Category category) => db.Categories.Add(category);

    public void Remove(Category category) => db.Categories.Remove(category);

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Category>> ListAsync(bool publishedOnly, CancellationToken cancellationToken = default)
    {
        var query = db.Categories.AsNoTracking();
        if (publishedOnly)
            query = query.Where(c => c.IsPublished);

        return await query
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsBySlugAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default) =>
        db.Categories.AnyAsync(c => c.Slug == slug && (excludeId == null || c.Id != excludeId), cancellationToken);

    public Task<bool> HasProductsAsync(Guid categoryId, CancellationToken cancellationToken = default) =>
        db.Products.AnyAsync(p => p.CategoryId == categoryId, cancellationToken);
}
