namespace Kessler.Domain.Common.Errors;

/// <summary>
/// Lançada quando uma invariante do domínio é violada por um caminho de código
/// (erro de programação, não entrada do usuário — esta é validada no Application).
/// </summary>
public sealed class DomainException(string message) : Exception(message);
