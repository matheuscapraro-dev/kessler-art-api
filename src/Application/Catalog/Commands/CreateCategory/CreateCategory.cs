using FluentResults;
using FluentValidation;
using Kessler.Application.Abstractions;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Catalog.Dtos;
using Kessler.Application.Catalog.Mapping;
using Kessler.Application.Common.Errors;
using Kessler.Domain.Catalog;
using Kessler.Domain.Common;
using MediatR;

namespace Kessler.Application.Catalog.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string? Description,
    int DisplayOrder) : IRequest<Result<CategoryDto>>;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(120);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}

internal sealed class CreateCategoryCommandHandler(
    ICategoryRepository categories,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCategoryCommand, Result<CategoryDto>>
{
    public async Task<Result<CategoryDto>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var slug = SlugGenerator.Generate(request.Name);
        if (await categories.ExistsBySlugAsync(slug, cancellationToken: cancellationToken))
            return AppResults.Conflict("Já existe uma categoria com esse nome.");

        var category = Category.Create(request.Name, request.Description, request.DisplayOrder);
        categories.Add(category);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(CatalogMapper.ToDto(category));
    }
}
