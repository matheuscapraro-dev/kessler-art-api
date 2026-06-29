using Kessler.Domain.Identity;

namespace Kessler.Application.Abstractions.Authentication;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string passwordHash);
}

public interface IJwtTokenGenerator
{
    /// <summary>Gera o JWT de acesso e devolve o token + sua expiração (UTC).</summary>
    (string Token, DateTime ExpiresAtUtc) Generate(User user);
}

public interface IUserRepository
{
    void Add(User user);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
}

/// <summary>Acesso ao usuário autenticado do request atual (preenchido pela Web a partir do JWT).</summary>
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}
