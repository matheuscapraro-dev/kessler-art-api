using Kessler.Domain.Common;

namespace Kessler.Domain.Catalog;

/// <summary>
/// Foto de uma peça. Entidade-filha do agregado <see cref="Product"/> — criada e
/// ordenada apenas através da raiz.
/// </summary>
public sealed class ProductImage : Entity
{
    public Guid ProductId { get; private set; }

    /// <summary>Chave no storage (disco local em dev, S3 em prod) usada para apagar o arquivo.</summary>
    public string StorageKey { get; private set; } = null!;

    /// <summary>URL pública servida ao navegador.</summary>
    public string Url { get; private set; } = null!;

    public string? AltText { get; private set; }
    public int DisplayOrder { get; private set; }
    public bool IsCover { get; private set; }

    private ProductImage() { } // EF Core

    internal static ProductImage Create(
        string storageKey,
        string url,
        string? altText,
        int displayOrder,
        bool isCover)
    {
        return new ProductImage
        {
            Id = Guid.NewGuid(),
            StorageKey = storageKey,
            Url = url,
            AltText = string.IsNullOrWhiteSpace(altText) ? null : altText.Trim(),
            DisplayOrder = displayOrder,
            IsCover = isCover
        };
    }

    internal void SetAsCover(bool isCover) => IsCover = isCover;

    internal void SetDisplayOrder(int order) => DisplayOrder = order;
}
