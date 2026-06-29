namespace Kessler.Domain.Common;

/// <summary>
/// Marcador de evento de domínio. Mantido livre de dependências de infra (ex. MediatR);
/// a camada de Application/Infrastructure faz a ponte para o despacho.
/// </summary>
public interface IDomainEvent;
