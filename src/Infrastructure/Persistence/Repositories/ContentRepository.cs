using Kessler.Application.Abstractions.Repositories;
using Kessler.Domain.Content;
using Microsoft.EntityFrameworkCore;

namespace Kessler.Infrastructure.Persistence.Repositories;

internal sealed class ContentRepository(KesslerDbContext db) : IContentRepository
{
    public void Add(SiteContent content) => db.SiteContent.Add(content);

    public Task<SiteContent?> GetAsync(CancellationToken cancellationToken = default) =>
        db.SiteContent.Include(c => c.AboutPhotos).FirstOrDefaultAsync(cancellationToken);
}
