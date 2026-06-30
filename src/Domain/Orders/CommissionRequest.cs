using Kessler.Domain.Common;

namespace Kessler.Domain.Orders;

/// <summary>Dados de uma imagem de referência já enviada ao storage.</summary>
public sealed record CommissionReference(string StorageKey, string Url);

/// <summary>Item da checklist passado à fábrica (título + se já está concluído).</summary>
public sealed record CommissionTaskInput(string Title, bool IsDone = false);

/// <summary>
/// Item do quadro do ateliê. Pode ser uma <b>encomenda</b> de cliente (briefing + orçamento,
/// vindo do fluxo público ou cadastrada pela artista) ou um <b>trabalho próprio</b> dela
/// (projeto pessoal, peça de estoque, amostra…) — por isso o cliente é opcional.
/// O atendimento e o orçamento acontecem manualmente (WhatsApp).
/// </summary>
public sealed class CommissionRequest : AuditableEntity
{
    private readonly List<CommissionReferenceImage> _referenceImages = [];
    private readonly List<CommissionTask> _tasks = [];

    public string Code { get; private set; } = null!;

    /// <summary>Contato do cliente — nulo em trabalhos próprios da artista.</summary>
    public CustomerInfo? Customer { get; private set; }

    /// <summary>Natureza do trabalho (encomenda, projeto pessoal, estoque…).</summary>
    public WorkType Type { get; private set; }

    /// <summary>Nome curto exibido no cartão do Kanban (opcional).</summary>
    public string? Title { get; private set; }

    public string Description { get; private set; } = null!;
    public string? DesiredCategory { get; private set; }
    public string? Colors { get; private set; }
    public string? Size { get; private set; }
    public DateTime? DesiredDeadline { get; private set; }

    /// <summary>Slug da peça de referência, quando a encomenda partiu de uma peça do site.</summary>
    public string? ReferenceProductSlug { get; private set; }

    public decimal? QuotedPrice { get; private set; }
    public CommissionStatus Status { get; private set; }
    public WorkPriority Priority { get; private set; }

    /// <summary>Ordem do cartão dentro da coluna (double permite inserir no meio sem renumerar).</summary>
    public double Position { get; private set; }

    public string? AdminNotes { get; private set; }

    public IReadOnlyList<CommissionReferenceImage> ReferenceImages => _referenceImages.AsReadOnly();
    public IReadOnlyList<CommissionTask> Tasks => _tasks.AsReadOnly();

    private CommissionRequest() { } // EF Core

    public static CommissionRequest Create(
        string description,
        CustomerInfo? customer = null,
        WorkType type = WorkType.Encomenda,
        string? title = null,
        WorkPriority priority = WorkPriority.Normal,
        CommissionStatus status = CommissionStatus.Nova,
        string? desiredCategory = null,
        string? colors = null,
        string? size = null,
        DateTime? desiredDeadline = null,
        decimal? quotedPrice = null,
        string? referenceProductSlug = null,
        IEnumerable<CommissionReference>? referenceImages = null)
    {
        var commission = new CommissionRequest
        {
            Id = Guid.NewGuid(),
            Code = ReferenceCode.New("ENC"),
            Customer = customer,
            Type = type,
            Title = Normalize(title),
            Description = description.Trim(),
            DesiredCategory = Normalize(desiredCategory),
            Colors = Normalize(colors),
            Size = Normalize(size),
            DesiredDeadline = NormalizeToUtc(desiredDeadline),
            QuotedPrice = quotedPrice,
            ReferenceProductSlug = Normalize(referenceProductSlug),
            Status = status,
            Priority = priority,
            Position = DateTime.UtcNow.Ticks // novidades entram no fim da coluna por padrão
        };

        foreach (var img in referenceImages ?? [])
            commission._referenceImages.Add(CommissionReferenceImage.Create(img.StorageKey, img.Url));

        return commission;
    }

    /// <summary>Edição completa do briefing/identidade do trabalho (painel admin).</summary>
    public void UpdateDetails(
        string description,
        WorkType type,
        string? title,
        WorkPriority priority,
        string? desiredCategory,
        string? colors,
        string? size,
        DateTime? desiredDeadline)
    {
        Description = description.Trim();
        Type = type;
        Title = Normalize(title);
        Priority = priority;
        DesiredCategory = Normalize(desiredCategory);
        Colors = Normalize(colors);
        Size = Normalize(size);
        DesiredDeadline = NormalizeToUtc(desiredDeadline);
    }

    public void SendQuote(decimal price)
    {
        QuotedPrice = price;
        if (Status is CommissionStatus.Nova or CommissionStatus.EmAnalise)
            Status = CommissionStatus.OrcamentoEnviado;
    }

    /// <summary>Define orçamento sem alterar o status (edição livre no painel).</summary>
    public void SetQuote(decimal? price) => QuotedPrice = price;

    public void UpdateStatus(CommissionStatus status) => Status = status;

    /// <summary>Move o cartão de coluna e posição (drag-and-drop do Kanban).</summary>
    public void MoveTo(CommissionStatus status, double position)
    {
        Status = status;
        Position = position;
    }

    public void SetPriority(WorkPriority priority) => Priority = priority;

    public void SetAdminNotes(string? notes) => AdminNotes = Normalize(notes);

    /// <summary>Substitui a checklist inteira preservando a ordem informada.</summary>
    public void SetTasks(IEnumerable<CommissionTaskInput> tasks)
    {
        _tasks.Clear();
        var position = 0;
        foreach (var t in tasks)
        {
            if (string.IsNullOrWhiteSpace(t.Title))
                continue;
            _tasks.Add(CommissionTask.Create(t.Title, t.IsDone, position++));
        }
    }

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
