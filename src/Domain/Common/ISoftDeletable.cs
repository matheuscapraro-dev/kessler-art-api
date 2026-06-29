namespace Kessler.Domain.Common;

/// <summary>
/// Entidades que suportam exclusão lógica (soft delete). O DbContext aplica um
/// query filter global para esconder registros marcados como excluídos.
/// </summary>
public interface ISoftDeletable
{
    bool IsDeleted { get; }
    DateTime? DeletedAt { get; }
}
