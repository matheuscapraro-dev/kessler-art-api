using Kessler.Domain.Common;

namespace Kessler.Domain.Orders;

/// <summary>Dados de uma imagem de referência já enviada ao storage.</summary>
public sealed record CommissionReference(string StorageKey, string Url);

/// <summary>
/// Encomenda sob medida solicitada por um convidado. O atendimento e o orçamento
/// acontecem manualmente (WhatsApp); aqui guardamos o briefing e o ciclo de vida.
/// </summary>
public sealed class CommissionRequest : AuditableEntity
{
    private readonly List<CommissionReferenceImage> _referenceImages = [];

    public string Code { get; private set; } = null!;
    public CustomerInfo Customer { get; private set; } = null!;

    public string Description { get; private set; } = null!;
    public string? DesiredCategory { get; private set; }
    public string? Colors { get; private set; }
    public string? Size { get; private set; }
    public DateTime? DesiredDeadline { get; private set; }

    /// <summary>Slug da peça de referência, quando a encomenda partiu de uma peça do site.</summary>
    public string? ReferenceProductSlug { get; private set; }

    public decimal? QuotedPrice { get; private set; }
    public CommissionStatus Status { get; private set; }
    public string? AdminNotes { get; private set; }

    public IReadOnlyList<CommissionReferenceImage> ReferenceImages => _referenceImages.AsReadOnly();

    private CommissionRequest() { } // EF Core

    public static CommissionRequest Create(
        CustomerInfo customer,
        string description,
        string? desiredCategory = null,
        string? colors = null,
        string? size = null,
        DateTime? desiredDeadline = null,
        string? referenceProductSlug = null,
        IEnumerable<CommissionReference>? referenceImages = null)
    {
        var commission = new CommissionRequest
        {
            Id = Guid.NewGuid(),
            Code = ReferenceCode.New("ENC"),
            Customer = customer,
            Description = description.Trim(),
            DesiredCategory = Normalize(desiredCategory),
            Colors = Normalize(colors),
            Size = Normalize(size),
            DesiredDeadline = NormalizeToUtc(desiredDeadline),
            ReferenceProductSlug = Normalize(referenceProductSlug),
            Status = CommissionStatus.Nova
        };

        foreach (var img in referenceImages ?? [])
            commission._referenceImages.Add(CommissionReferenceImage.Create(img.StorageKey, img.Url));

        return commission;
    }

    public void SendQuote(decimal price)
    {
        QuotedPrice = price;
        Status = CommissionStatus.OrcamentoEnviado;
    }

    public void UpdateStatus(CommissionStatus status) => Status = status;

    public void SetAdminNotes(string? notes) => AdminNotes = Normalize(notes);

    private static string? Normalize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    /// <summary>
    /// Postgres (timestamptz) exige DateTime UTC. Datas vindas de &lt;input type="date"&gt;
    /// chegam com Kind=Unspecified — tratamos como UTC.
    /// </summary>
    private static DateTime? NormalizeToUtc(DateTime? value) => value?.Kind switch
    {
        null => null,
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.Value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
    };
}
