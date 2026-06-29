using Kessler.Domain.Common;
using Kessler.Domain.Common.Errors;

namespace Kessler.Domain.Catalog;

/// <summary>
/// Peça de crochê — raiz do agregado de Catálogo. Cobre os três cenários do site
/// (vitrine, loja e sob encomenda) via <see cref="ProductAvailability"/>, com as
/// invariantes de preço/estoque/prazo garantidas aqui no domínio.
/// </summary>
public sealed class Product : AuditableEntity, ISoftDeletable
{
    private readonly List<ProductImage> _images = [];

    public string Name { get; private set; } = null!;
    public string Slug { get; private set; } = null!;
    public string? Description { get; private set; }
    public Guid CategoryId { get; private set; }
    public ProductAvailability Availability { get; private set; }

    /// <summary>Preço em BRL. Obrigatório para ReadyToBuy; "a partir de" opcional para MadeToOrder; nulo para Showcase.</summary>
    public decimal? Price { get; private set; }

    /// <summary>Estoque — só faz sentido para ReadyToBuy.</summary>
    public int? StockQuantity { get; private set; }

    /// <summary>Prazo de produção em dias — só faz sentido para MadeToOrder.</summary>
    public int? LeadTimeDays { get; private set; }

    public bool IsFeatured { get; private set; }
    public bool IsPublished { get; private set; }

    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

    public IReadOnlyList<ProductImage> Images => _images.AsReadOnly();

    private Product() { } // EF Core

    public static Product Create(
        string name,
        Guid categoryId,
        ProductAvailability availability,
        string? description = null,
        decimal? price = null,
        int? stockQuantity = null,
        int? leadTimeDays = null,
        bool isFeatured = false,
        bool isPublished = true)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Slug = SlugGenerator.Generate(name),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            CategoryId = categoryId,
            IsFeatured = isFeatured,
            IsPublished = isPublished
        };
        product.ApplyAvailability(availability, price, stockQuantity, leadTimeDays);
        return product;
    }

    public void UpdateDetails(string name, string? description, Guid categoryId, bool isFeatured, bool isPublished)
    {
        Name = name.Trim();
        Slug = SlugGenerator.Generate(name);
        Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        CategoryId = categoryId;
        IsFeatured = isFeatured;
        IsPublished = isPublished;
    }

    public void ChangeAvailability(ProductAvailability availability, decimal? price, int? stockQuantity, int? leadTimeDays)
        => ApplyAvailability(availability, price, stockQuantity, leadTimeDays);

    /// <summary>Garante as invariantes de preço/estoque/prazo conforme a disponibilidade.</summary>
    private void ApplyAvailability(ProductAvailability availability, decimal? price, int? stockQuantity, int? leadTimeDays)
    {
        switch (availability)
        {
            case ProductAvailability.Showcase:
                Price = null;
                StockQuantity = null;
                LeadTimeDays = null;
                break;

            case ProductAvailability.ReadyToBuy:
                if (price is null or < 0)
                    throw new DomainException("Peça pronta para venda exige um preço válido.");
                Price = price;
                StockQuantity = stockQuantity is null or < 0 ? 0 : stockQuantity;
                LeadTimeDays = null;
                break;

            case ProductAvailability.MadeToOrder:
                if (price is < 0)
                    throw new DomainException("O preço 'a partir de' não pode ser negativo.");
                Price = price;
                StockQuantity = null;
                LeadTimeDays = leadTimeDays is < 0 ? null : leadTimeDays;
                break;

            default:
                throw new DomainException($"Disponibilidade inválida: {availability}.");
        }

        Availability = availability;
    }

    // ── Imagens ──────────────────────────────────────────────────────

    public ProductImage AddImage(string storageKey, string url, string? altText = null)
    {
        var isFirst = _images.Count == 0;
        var image = ProductImage.Create(storageKey, url, altText, displayOrder: _images.Count, isCover: isFirst);
        _images.Add(image);
        return image;
    }

    public void RemoveImage(Guid imageId)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId);
        if (image is null)
            return;

        _images.Remove(image);

        // Se a capa foi removida, promove a primeira imagem restante.
        if (image.IsCover && _images.Count > 0)
            _images[0].SetAsCover(true);

        Reindex();
    }

    public void SetCoverImage(Guid imageId)
    {
        if (_images.All(i => i.Id != imageId))
            throw new DomainException("Imagem não pertence a esta peça.");

        foreach (var img in _images)
            img.SetAsCover(img.Id == imageId);
    }

    private void Reindex()
    {
        for (var i = 0; i < _images.Count; i++)
            _images[i].SetDisplayOrder(i);
    }

    // ── Estoque ──────────────────────────────────────────────────────

    public bool HasStock(int quantity) =>
        Availability != ProductAvailability.ReadyToBuy || (StockQuantity ?? 0) >= quantity;

    public void DecreaseStock(int quantity)
    {
        if (Availability != ProductAvailability.ReadyToBuy)
            return;

        if ((StockQuantity ?? 0) < quantity)
            throw new DomainException("Estoque insuficiente para a peça.");

        StockQuantity -= quantity;
    }

    // ── Soft delete ──────────────────────────────────────────────────

    public void MarkDeleted()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        IsPublished = false;
    }
}
