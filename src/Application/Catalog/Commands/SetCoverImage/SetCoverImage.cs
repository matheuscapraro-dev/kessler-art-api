using FluentResults;
using Kessler.Application.Abstractions;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Common.Errors;
using MediatR;

namespace Kessler.Application.Catalog.Commands.SetCoverImage;

public sealed record SetCoverImageCommand(Guid ProductId, Guid ImageId) : IRequest<Result>;

internal sealed class SetCoverImageCommandHandler(
    IProductRepository products,
    IUnitOfWork unitOfWork) : IRequestHandler<SetCoverImageCommand, Result>
{
    public async Task<Result> Handle(SetCoverImageCommand request, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
            return AppResults.NotFound("Peça não encontrada.");

        if (product.Images.All(i => i.Id != request.ImageId))
            return AppResults.NotFound("Imagem não encontrada.");

        product.SetCoverImage(request.ImageId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
