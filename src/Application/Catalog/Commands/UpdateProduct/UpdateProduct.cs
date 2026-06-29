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

namespace Kessler.Application.Catalog.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    Guid CategoryId,
    ProductAvailability Availability,
    string? Description,
    decimal? Price,
    int? StockQuantity,
    int? LeadTimeDays,
    bool IsFeatured,
    bool IsPublished) : IRequest<Result<ProductDto>>;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(160);
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Availability).IsInEnum();
        RuleFor(x => x.Description).MaximumLength(4000);

        When(x => x.Availability == ProductAvailability.ReadyToBuy, () =>
        {
            RuleFor(x => x.Price).NotNull().GreaterThanOrEqualTo(0)
                .WithMessage("Informe um preço para peça pronta para venda.");
            RuleFor(x => x.StockQuantity).GreaterThanOrEqualTo(0).When(x => x.StockQuantity.HasValue);
        });

        When(x => x.Availability == ProductAvailability.MadeToOrder, () =>
        {
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).When(x => x.Price.HasValue);
            RuleFor(x => x.LeadTimeDays).GreaterThanOrEqualTo(0).When(x => x.LeadTimeDays.HasValue);
        });
    }
}

internal sealed class UpdateProductCommandHandler(
    IProductRepository products,
    ICategoryRepository categories,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await products.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return AppResults.NotFound("Peça não encontrada.");

        var category = await categories.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            return AppResults.NotFound("Categoria não encontrada.");

        var slug = SlugGenerator.Generate(request.Name);
        if (await products.ExistsBySlugAsync(slug, request.Id, cancellationToken))
            return AppResults.Conflict("Já existe outra peça com esse nome.");

        product.UpdateDetails(request.Name, request.Description, request.CategoryId, request.IsFeatured, request.IsPublished);
        product.ChangeAvailability(request.Availability, request.Price, request.StockQuantity, request.LeadTimeDays);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(CatalogMapper.ToDto(product, category.Name));
    }
}
