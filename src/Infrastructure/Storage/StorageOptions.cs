namespace Kessler.Infrastructure.Storage;

/// <summary>Configuração do storage local (seção "Storage" do appsettings).</summary>
public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    /// <summary>Pasta física onde os arquivos são gravados (relativa ao content root).</summary>
    public string RootPath { get; set; } = "uploads";

    /// <summary>Prefixo público das URLs servidas (estático). Ex.: "/uploads".</summary>
    public string PublicBaseUrl { get; set; } = "/uploads";
}
