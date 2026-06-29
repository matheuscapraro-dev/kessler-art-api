namespace Kessler.Domain.Common;

/// <summary>
/// Base de todas as entidades do domínio. Identidade por <see cref="Id"/> (DDD) e
/// suporte a domain events que a infraestrutura despacha após o SaveChanges.
/// </summary>
public abstract class Entity : IEquatable<Entity>
{
    public Guid Id { get; protected set; }

    // ── Domain Events ────────────────────────────────────────────────

    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Retorna os eventos pendentes e limpa a lista interna.
    /// Chamado pela infraestrutura após a persistência para despachá-los.
    /// </summary>
    public IReadOnlyList<IDomainEvent> PopDomainEvents()
    {
        var events = _domainEvents.ToList();
        _domainEvents.Clear();
        return events;
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);

    // ── Identity Equality (DDD) ──────────────────────────────────────

    public bool Equals(Entity? other)
    {
        if (other is null || other.GetType() != GetType())
            return false;

        return Id == other.Id;
    }

    public override bool Equals(object? obj) => Equals(obj as Entity);

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity? left, Entity? right) => Equals(left, right);

    public static bool operator !=(Entity? left, Entity? right) => !Equals(left, right);
}
