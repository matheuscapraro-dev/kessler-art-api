using Kessler.Domain.Common;

namespace Kessler.Domain.Content;

/// <summary>
/// Foto da página "Sobre". Entidade-filha do agregado <see cref="SiteContent"/> —
/// criada/ordenada apenas pela raiz.
/// </summary>
public sealed class AboutPhoto : Entity
{
    public Guid SiteContentId { get; private set; }
    public string StorageKey { get; private set; } = null!;
    public string Url { get; private set; } = null!;
    public string? Caption { get; private set; }
    public int DisplayOrder { get; private set; }

    private AboutPhoto() { } // EF Core

    internal static AboutPhoto Create(string storageKey, string url, string? caption, int displayOrder) =>
        new()
        {
            Id = Guid.NewGuid(),
            StorageKey = storageKey,
            Url = url,
            Caption = string.IsNullOrWhiteSpace(caption) ? null : caption.Trim(),
            DisplayOrder = displayOrder,
        };

    internal void SetCaption(string? caption) =>
        Caption = string.IsNullOrWhiteSpace(caption) ? null : caption.Trim();

    internal void SetDisplayOrder(int order) => DisplayOrder = order;
}
