using FluentResults;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Catalog.Dtos;
using Kessler.Application.Catalog.Mapping;
using MediatR;

namespace Kessler.Application.Catalog.Queries.ListCategories;

public sealed record ListCategoriesQuery(bool PublishedOnly = true)
    : IRequest<Result<IReadOnlyList<CategoryDto>>>;

internal sealed class ListCategoriesQueryHandler(ICategoryRepository categories)
    : IRequestHandler<ListCategoriesQuery, Result<IReadOnlyList<CategoryDto>>>
{
    public async Task<Result<IReadOnlyList<CategoryDto>>> Handle(
        ListCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var list = await categories.ListAsync(request.PublishedOnly, cancellationToken);
        IReadOnlyList<CategoryDto> dtos = list.Select(CatalogMapper.ToDto).ToList();
        return Result.Ok(dtos);
    }
}
