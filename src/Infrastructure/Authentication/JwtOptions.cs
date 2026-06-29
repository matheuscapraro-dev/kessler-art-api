namespace Kessler.Infrastructure.Authentication;

/// <summary>Configuração do JWT, ligada à seção "Jwt" do appsettings.</summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "kessler-art";
    public string Audience { get; set; } = "kessler-art";
    public string Secret { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 480; // 8h
}
