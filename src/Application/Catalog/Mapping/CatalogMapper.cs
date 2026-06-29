using Kessler.Application.Catalog.Dtos;
using Kessler.Domain.Catalog;

namespace Kessler.Application.Catalog.Mapping;

/// <summary>
/// Mapeamento explícito Domínio → DTO (sem AutoMapper, como no padrão dos demais projetos).
/// O nome da categoria é passado pelo handler, mantendo o agregado Product referenciando
/// Category apenas por Id.
/// </summary>
public static class CatalogMapper
{
    public static CategoryDto ToDto(Category category) => new(
        category.Id,
        category.Name,
        category.Slug,
        category.Description,
        category.DisplayOrder,
        category.IsPublished);

    public static ProductImageDto ToDto(ProductImage image) => new(
        image.Id,
        image.Url,
        image.AltText,
        image.DisplayOrder,
        image.IsCover);

    public static ProductDto ToDto(Product product, string categoryName) => new(
        product.Id,
        product.Name,
        product.Slug,
        product.Description,
        product.CategoryId,
        categoryName,
        product.Availability,
        product.Price,
        product.StockQuantity,
        product.LeadTimeDays,
        product.IsFeatured,
        product.IsPublished,
        product.Images.OrderBy(i => i.DisplayOrder).Select(ToDto).ToList());

    public static ProductSummaryDto ToSummary(Product product, string categoryName)
    {
        var cover = product.Images.OrderBy(i => i.DisplayOrder).FirstOrDefault(i => i.IsCover)
                    ?? product.Images.OrderBy(i => i.DisplayOrder).FirstOrDefault();

        return new ProductSummaryDto(
            product.Id,
            product.Name,
            product.Slug,
            product.Availability,
            product.Price,
            categoryName,
            cover?.Url,
            product.IsFeatured);
    }
}
