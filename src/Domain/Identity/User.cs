using Kessler.Domain.Common;

namespace Kessler.Domain.Identity;

/// <summary>
/// Usuário administrativo (a artista). O site público é acessado como convidado;
/// só o painel exige login. Modelado para abrir caminho a contas de cliente no futuro.
/// </summary>
public sealed class User : AuditableEntity
{
    public const string AdminRole = "Admin";

    public string Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string Role { get; private set; } = AdminRole;

    private User() { } // EF Core

    public static User Create(string email, string passwordHash, string name, string role = AdminRole)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email.Trim().ToLowerInvariant(),
            PasswordHash = passwordHash,
            Name = name.Trim(),
            Role = role
        };
    }

    public void ChangePassword(string passwordHash) => PasswordHash = passwordHash;
}
