namespace Kessler.Application.Abstractions.Storage;

/// <summary>Resultado de um upload: chave interna (para apagar) + URL pública.</summary>
public sealed record StoredFile(string StorageKey, string Url);

/// <summary>
/// Abstrai onde as imagens ficam guardadas. Em dev usa disco local; em prod, S3.
/// Trocar de provedor = nova implementação na Infrastructure, sem tocar no Application.
/// </summary>
public interface IStorageService
{
    Task<StoredFile> SaveAsync(
        Stream content,
        string fileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default);
}
