using Kessler.Application.Abstractions.Authentication;
using Kessler.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kessler.Infrastructure.Persistence;

/// <summary>
/// Aplica as migrations pendentes e garante que exista um usuário admin (a artista).
/// Chamado no startup da Web. As credenciais iniciais vêm da seção "Admin" do appsettings.
/// </summary>
public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var logger = sp.GetRequiredService<ILogger<KesslerDbContext>>();
        var db = sp.GetRequiredService<KesslerDbContext>();

        await db.Database.MigrateAsync(cancellationToken);

        var users = sp.GetRequiredService<IUserRepository>();
        if (await users.AnyAsync(cancellationToken))
            return;

        var config = sp.GetRequiredService<IConfiguration>();
        var email = config["Admin:Email"];
        var password = config["Admin:Password"];
        var name = config["Admin:Name"] ?? "Kessler";

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Nenhum admin configurado (seção Admin) — pulando seed do usuário inicial.");
            return;
        }

        var hasher = sp.GetRequiredService<IPasswordHasher>();
        users.Add(User.Create(email, hasher.Hash(password), name));
        await db.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Usuário admin inicial criado: {Email}", email);
    }
}
