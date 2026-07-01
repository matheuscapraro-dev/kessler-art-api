using Kessler.Domain.Common;

namespace Kessler.Domain.Content;

/// <summary>
/// Conteúdo editável do site (singleton) — deixa a artista gerenciar textos, contato
/// e as fotos da página "Sobre" sem precisar de deploy. Sempre existe uma única linha.
/// </summary>
public sealed class SiteContent : AuditableEntity
{
    private readonly List<AboutPhoto> _aboutPhotos = [];

    // ── Contato / redes ──────────────────────────────────────────────
    public string? WhatsApp { get; private set; }
    public string? InstagramUrl { get; private set; }

    // ── Página "Sobre" ───────────────────────────────────────────────
    public string? AboutTitle { get; private set; }
    public string? AboutIntro { get; private set; }
    public string? AboutStoryTitle { get; private set; }
    public string? AboutStory { get; private set; }

    public IReadOnlyList<AboutPhoto> AboutPhotos => _aboutPhotos.AsReadOnly();

    private SiteContent() { } // EF Core

    /// <summary>Cria o singleton com os textos padrão do site.</summary>
    public static SiteContent CreateDefault() => new()
    {
        Id = Guid.NewGuid(),
        AboutTitle = "Crochê com propósito e carinho",
        AboutIntro =
            "Num mundo acelerado, a Kessler Art Crochê é um refúgio de criação consciente. " +
            "Cada ponto é uma pequena meditação; cada peça, uma história contada com fios e mãos pacientes.",
        AboutStoryTitle = "Nossa história",
        AboutStory =
            "Tudo começou com uma agulha, um novelo e a vontade de criar com as próprias mãos. " +
            "A Kessler Art Crochê nasceu desse encontro entre afeto e ofício.\n\n" +
            "Não fazemos só peças — fazemos pequenas heranças táteis. Cada criação celebra a beleza do " +
            "feito à mão e a imperfeição perfeita que só o trabalho artesanal carrega.",
    };

    public void Update(
        string? whatsApp,
        string? instagramUrl,
        string? aboutTitle,
        string? aboutIntro,
        string? aboutStoryTitle,
        string? aboutStory)
    {
        WhatsApp = Digits(whatsApp);
        InstagramUrl = Trim(instagramUrl);
        AboutTitle = Trim(aboutTitle);
        AboutIntro = Trim(aboutIntro);
        AboutStoryTitle = Trim(aboutStoryTitle);
        AboutStory = Trim(aboutStory);
    }

    // ── Fotos da página "Sobre" ──────────────────────────────────────

    public AboutPhoto AddAboutPhoto(string storageKey, string url, string? caption = null)
    {
        var photo = AboutPhoto.Create(storageKey, url, caption, _aboutPhotos.Count);
        _aboutPhotos.Add(photo);
        return photo;
    }

    public void RemoveAboutPhoto(Guid photoId)
    {
        var photo = _aboutPhotos.FirstOrDefault(p => p.Id == photoId);
        if (photo is null)
            return;

        _aboutPhotos.Remove(photo);
        Reindex();
    }

    public void SetAboutPhotoCaption(Guid photoId, string? caption) =>
        _aboutPhotos.FirstOrDefault(p => p.Id == photoId)?.SetCaption(caption);

    private void Reindex()
    {
        for (var i = 0; i < _aboutPhotos.Count; i++)
            _aboutPhotos[i].SetDisplayOrder(i);
    }

    private static string? Trim(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    /// <summary>Mantém só dígitos no WhatsApp (formato wa.me).</summary>
    private static string? Digits(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        var digits = new string(value.Where(char.IsDigit).ToArray());
        return digits.Length == 0 ? null : digits;
    }
}
