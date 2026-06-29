namespace Kessler.Domain.Common;

/// <summary>
/// Entidade com carimbos de auditoria. <see cref="CreatedAt"/>/<see cref="UpdatedAt"/>
/// são preenchidos automaticamente pelo DbContext em SaveChanges.
/// </summary>
public abstract class AuditableEntity : Entity
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
