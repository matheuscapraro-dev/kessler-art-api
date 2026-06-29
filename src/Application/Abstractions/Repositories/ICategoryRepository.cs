using Kessler.Domain.Catalog;

namespace Kessler.Application.Abstractions.Repositories;

public interface ICategoryRepository
{
    void Add(Category category);
    void Remove(Category category);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Category>> ListAsync(bool publishedOnly, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySlugAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> HasProductsAsync(Guid categoryId, CancellationToken cancellationToken = default);
}
