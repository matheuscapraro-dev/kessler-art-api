using Kessler.Domain.Common;

namespace Kessler.Domain.Orders;

/// <summary>
/// Foto de referência enviada pelo cliente numa encomenda (inspiração/exemplo).
/// Entidade-filha do agregado <see cref="CommissionRequest"/>.
/// </summary>
public sealed class CommissionReferenceImage : Entity
{
    public Guid CommissionRequestId { get; private set; }
    public string StorageKey { get; private set; } = null!;
    public string Url { get; private set; } = null!;

    private CommissionReferenceImage() { } // EF Core

    internal static CommissionReferenceImage Create(string storageKey, string url) =>
        new()
        {
            Id = Guid.NewGuid(),
            StorageKey = storageKey.Trim(),
            Url = url.Trim(),
        };
}
