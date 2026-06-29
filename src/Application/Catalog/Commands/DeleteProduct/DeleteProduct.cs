using FluentResults;
using Kessler.Application.Abstractions;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Common.Errors;
using MediatR;

namespace Kessler.Application.Catalog.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : IRequest<Result>;

internal sealed class DeleteProductCommandHandler(
    IProductRepository products,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteProductCommand, Result>
{
    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return AppResults.NotFound("Peça não encontrada.");

        // Soft delete — preserva histórico de pedidos que referenciam a peça.
        product.MarkDeleted();
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
