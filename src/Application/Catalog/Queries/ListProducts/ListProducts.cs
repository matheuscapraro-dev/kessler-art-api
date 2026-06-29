using FluentResults;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Catalog.Dtos;
using Kessler.Application.Catalog.Mapping;
using Kessler.Domain.Catalog;
using MediatR;

namespace Kessler.Application.Catalog.Queries.ListProducts;

/// <summary>
/// Lista peças para a galeria/loja/admin. Os flags de filtro mapeiam direto para
/// <see cref="ProductFilter"/>.
/// </summary>
public sealed record ListProductsQuery(
    string? CategorySlug = null,
    ProductAvailability? Availability = null,
    bool FeaturedOnly = false,
    bool PublishedOnly = true,
    string? Search = null) : IRequest<Result<IReadOnlyList<ProductSummaryDto>>>;

internal sealed class ListProductsQueryHandler(
    IProductRepository products,
    ICategoryRepository categories) : IRequestHandler<ListProductsQuery, Result<IReadOnlyList<ProductSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<ProductSummaryDto>>> Handle(
        ListProductsQuery request,
        CancellationToken cancellationToken)
    {
        var filter = new ProductFilter(
            request.CategorySlug,
            request.Availability,
            request.FeaturedOnly,
            request.PublishedOnly,
            request.Search);

        var list = await products.ListAsync(filter, cancellationToken);

        // Uma busca só de categorias evita N+1 ao resolver os nomes.
        var categoryNames = (await categories.ListAsync(publishedOnly: false, cancellationToken))
            .ToDictionary(c => c.Id, c => c.Name);

        IReadOnlyList<ProductSummaryDto> dtos = list
            .Select(p => CatalogMapper.ToSummary(p, categoryNames.GetValueOrDefault(p.CategoryId, string.Empty)))
            .ToList();

        return Result.Ok(dtos);
    }
}
