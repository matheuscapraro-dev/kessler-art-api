using Kessler.Domain.Catalog;

namespace Kessler.Application.Catalog.Dtos;

public sealed record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    int DisplayOrder,
    bool IsPublished);

public sealed record ProductImageDto(
    Guid Id,
    string Url,
    string? AltText,
    int DisplayOrder,
    bool IsCover);

/// <summary>Versão enxuta para grids da galeria/loja.</summary>
public sealed record ProductSummaryDto(
    Guid Id,
    string Name,
    string Slug,
    ProductAvailability Availability,
    decimal? Price,
    string CategoryName,
    string? CoverImageUrl,
    bool IsFeatured);

/// <summary>Versão completa para a página de detalhe e o admin.</summary>
public sealed record ProductDto(
    Guid Id,
    string Name,
    string Slug,
    string? Description,
    Guid CategoryId,
    string CategoryName,
    ProductAvailability Availability,
    decimal? Price,
    int? StockQuantity,
    int? LeadTimeDays,
    bool IsFeatured,
    bool IsPublished,
    IReadOnlyList<ProductImageDto> Images);
