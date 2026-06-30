using Kessler.Domain.Common;

namespace Kessler.Domain.Orders;

/// <summary>
/// Item de checklist de um trabalho do ateliê (ex.: "comprar lã", "fazer a base").
/// Entidade-filha do agregado <see cref="CommissionRequest"/>.
/// </summary>
public sealed class CommissionTask : Entity
{
    public Guid CommissionRequestId { get; private set; }
    public string Title { get; private set; } = null!;
    public bool IsDone { get; private set; }
    public int Position { get; private set; }

    private CommissionTask() { } // EF Core

    internal static CommissionTask Create(string title, bool isDone, int position) =>
        new()
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            IsDone = isDone,
            Position = position,
        };
}
