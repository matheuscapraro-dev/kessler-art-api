using FluentResults;
using FluentValidation;
using Kessler.Application.Abstractions;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Catalog.Dtos;
using Kessler.Application.Catalog.Mapping;
using Kessler.Application.Common.Errors;
using Kessler.Domain.Common;
using MediatR;

namespace Kessler.Application.Catalog.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    int DisplayOrder,
    bool IsPublished) : IRequest<Result<CategoryDto>>;

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

internal sealed class UpdateCategoryCommandHandler(
    ICategoryRepository categories,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCategoryCommand, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categories.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return AppResults.NotFound("Categoria não encontrada.");

        var slug = SlugGenerator.Generate(request.Name);
        if (await categories.ExistsBySlugAsync(slug, request.Id, cancellationToken))
            return AppResults.Conflict("Já existe outra categoria com esse nome.");

        category.Update(request.Name, request.Description, request.DisplayOrder, request.IsPublished);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(CatalogMapper.ToDto(category));
    }
}
