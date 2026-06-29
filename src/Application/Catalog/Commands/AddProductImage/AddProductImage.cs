using FluentResults;
using Kessler.Application.Abstractions;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Catalog.Dtos;
using Kessler.Application.Catalog.Mapping;
using Kessler.Application.Common.Errors;
using MediatR;

namespace Kessler.Application.Catalog.Commands.AddProductImage;

/// <summary>
/// Persiste os metadados de uma imagem já enviada ao storage pelo controller.
/// </summary>
public sealed record AddProductImageCommand(
    Guid ProductId,
    string StorageKey,
    string Url,
    string? AltText) : IRequest<Result<ProductImageDto>>;

internal sealed class AddProductImageCommandHandler(
    IProductRepository products,
    IUnitOfWork unitOfWork) : IRequestHandler<AddProductImageCommand, Result<ProductImageDto>>
{
    public async Task<Result<ProductImageDto>> Handle(AddProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return AppResults.NotFound("Peça não encontrada.");

        var image = product.AddImage(request.StorageKey, request.Url, request.AltText);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(CatalogMapper.ToDto(image));
    }
}
