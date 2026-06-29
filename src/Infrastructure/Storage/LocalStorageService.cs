using Kessler.Application.Abstractions.Storage;
using Microsoft.Extensions.Options;

namespace Kessler.Infrastructure.Storage;

/// <summary>
/// Grava as imagens no disco local (dev). A chave de storage é o caminho relativo
/// (ex.: "products/abcd1234.jpg") e a URL pública é o prefixo configurado + chave.
/// Para produção, basta implementar <see cref="IStorageService"/> com S3 e trocar o registro no DI.
/// </summary>
internal sealed class LocalStorageService : IStorageService
{
    private readonly string _rootPath;
    private readonly string _publicBaseUrl;

    public LocalStorageService(IOptions<StorageOptions> options)
    {
        _rootPath = Path.IsPathRooted(options.Value.RootPath)
            ? options.Value.RootPath
            : Path.Combine(AppContext.BaseDirectory, options.Value.RootPath);
        _publicBaseUrl = options.Value.PublicBaseUrl.TrimEnd('/');
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<StoredFile> SaveAsync(
        Stream content,
        string fileName,
        string contentType,
        string folder,
        CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName);
        var key = $"{folder.Trim('/')}/{Guid.NewGuid():N}{extension}".Replace("\\", "/");

        var absolutePath = Path.Combine(_rootPath, key.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

        await using (var fileStream = File.Create(absolutePath))
            await content.CopyToAsync(fileStream, cancellationToken);

        return new StoredFile(key, $"{_publicBaseUrl}/{key}");
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken = default)
    {
        var absolutePath = Path.Combine(_rootPath, storageKey.Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(absolutePath))
            File.Delete(absolutePath);

        return Task.CompletedTask;
    }
}
