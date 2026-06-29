using FluentResults;
using Kessler.Application.Abstractions;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Common.Errors;
using MediatR;

namespace Kessler.Application.Catalog.Commands.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid Id) : IRequest<Result>;

internal sealed class DeleteCategoryCommandHandler(
    ICategoryRepository categories,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categories.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return AppResults.NotFound("Categoria não encontrada.");

        if (await categories.HasProductsAsync(request.Id, cancellationToken))
            return AppResults.Conflict("Não é possível excluir uma categoria que possui peças.");

        categories.Remove(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}
