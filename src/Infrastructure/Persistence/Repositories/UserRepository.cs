using Kessler.Application.Abstractions.Authentication;
using Kessler.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kessler.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(KesslerDbContext db) : IUserRepository
{
    public void Add(User user) => db.Users.Add(user);

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        return db.Users.FirstOrDefaultAsync(u => u.Email == normalized, cancellationToken);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<bool> AnyAsync(CancellationToken cancellationToken = default) =>
        db.Users.AnyAsync(cancellationToken);
}
