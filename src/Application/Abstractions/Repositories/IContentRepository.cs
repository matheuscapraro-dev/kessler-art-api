using Kessler.Domain.Content;

namespace Kessler.Application.Abstractions.Repositories;

public interface IContentRepository
{
    void Add(SiteContent content);

    /// <summary>Carrega o singleton (com as fotos). Pode não existir ainda.</summary>
    Task<SiteContent?> GetAsync(CancellationToken cancellationToken = default);
}
