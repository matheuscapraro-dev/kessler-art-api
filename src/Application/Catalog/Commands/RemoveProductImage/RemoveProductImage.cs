using FluentResults;
using Kessler.Application.Abstractions;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Abstractions.Storage;
using Kessler.Application.Common.Errors;
using MediatR;

namespace Kessler.Application.Catalog.Commands.RemoveProductImage;

public sealed record RemoveProductImageCommand(Guid ProductId, Guid ImageId) : IRequest<Result>;

internal sealed class RemoveProductImageCommandHandler(
    IProductRepository products,
    IStorageService storage,
    IUnitOfWork unitOfWork) : IRequestHandler<RemoveProductImageCommand, Result>
{
    public async Task<Result> Handle(RemoveProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return AppResults.NotFound("Peça não encontrada.");

        var image = product.Images.FirstOrDefault(i => i.Id == request.ImageId);
        if (image is null)
            return AppResults.NotFound("Imagem não encontrada.");

        var storageKey = image.StorageKey;
        product.RemoveImage(request.ImageId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // Remove o arquivo só após confirmar no banco (orfão é tolerável; perda de dado não).
        await storage.DeleteAsync(storageKey, cancellationToken);

        return Result.Ok();
    }
}
