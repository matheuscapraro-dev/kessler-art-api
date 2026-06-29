using FluentResults;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Catalog.Dtos;
using Kessler.Application.Catalog.Mapping;
using Kessler.Application.Common.Errors;
using MediatR;

namespace Kessler.Application.Catalog.Queries.GetProductBySlug;

public sealed record GetProductBySlugQuery(string Slug) : IRequest<Result<ProductDto>>;

internal sealed class GetProductBySlugQueryHandler(
    IProductRepository products,
    ICategoryRepository categories) : IRequestHandler<GetProductBySlugQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(GetProductBySlugQuery request, CancellationToken cancellationToken)
    {
        var product = await products.GetBySlugAsync(request.Slug, cancellationToken);
        if (product is null)
            return AppResults.NotFound("Peça não encontrada.");

        var category = await categories.GetByIdAsync(product.CategoryId, cancellationToken);
        return Result.Ok(CatalogMapper.ToDto(product, category?.Name ?? string.Empty));
    }
}
