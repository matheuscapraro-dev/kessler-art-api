using FluentResults;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Catalog.Dtos;
using Kessler.Application.Catalog.Mapping;
using Kessler.Application.Common.Errors;
using MediatR;

namespace Kessler.Application.Catalog.Queries.GetProductById;

/// <summary>Detalhe completo por Id — usado no admin (inclui não publicadas).</summary>
public sealed record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;

internal sealed class GetProductByIdQueryHandler(
    IProductRepository products,
    ICategoryRepository categories) : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return AppResults.NotFound("Peça não encontrada.");

        var category = await categories.GetByIdAsync(product.CategoryId, cancellationToken);
        return Result.Ok(CatalogMapper.ToDto(product, category?.Name ?? string.Empty));
    }
}
