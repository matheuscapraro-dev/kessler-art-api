using Kessler.Domain.Catalog;

namespace Kessler.Application.Abstractions.Repositories;

/// <summary>Filtros de listagem de peças (todos opcionais).</summary>
public sealed record ProductFilter(
    string? CategorySlug = null,
    ProductAvailability? Availability = null,
    bool FeaturedOnly = false,
    bool PublishedOnly = true,
    string? Search = null);

public interface IProductRepository
{
    void Add(Product product);
    void Remove(Product product);

    /// <summary>Carrega a peça com suas imagens (para edição/admin).</summary>
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Carrega a peça publicada por slug, com imagens e categoria (página pública).</summary>
    Task<Product?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>Carrega várias peças por Id (para montar itens de pedido).</summary>
    Task<IReadOnlyList<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Product>> ListAsync(ProductFilter filter, CancellationToken cancellationToken = default);

    Task<bool> ExistsBySlugAsync(string slug, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
