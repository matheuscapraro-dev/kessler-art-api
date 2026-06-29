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

namespace Kessler.Application.Catalog.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    Guid CategoryId,
    ProductAvailability Availability,
    string? Description,
    decimal? Price,
    int? StockQuantity,
    int? LeadTimeDays,
    bool IsFeatured,
    bool IsPublished) : IRequest<Result<ProductDto>>;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
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

internal sealed class CreateProductCommandHandler(
    IProductRepository products,
    ICategoryRepository categories,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await categories.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
            return AppResults.NotFound("Categoria não encontrada.");

        var slug = SlugGenerator.Generate(request.Name);
        if (await products.ExistsBySlugAsync(slug, cancellationToken: cancellationToken))
            return AppResults.Conflict("Já existe uma peça com esse nome.");

        var product = Product.Create(
            request.Name,
            request.CategoryId,
            request.Availability,
            request.Description,
            request.Price,
            request.StockQuantity,
            request.LeadTimeDays,
            request.IsFeatured,
            request.IsPublished);

        products.Add(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(CatalogMapper.ToDto(product, category.Name));
    }
}
