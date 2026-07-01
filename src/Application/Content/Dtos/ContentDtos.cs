using Kessler.Domain.Content;

namespace Kessler.Application.Content.Dtos;

public sealed record AboutPhotoDto(Guid Id, string Url, string? Caption, int DisplayOrder);

public sealed record SiteContentDto(
    string? WhatsApp,
    string? InstagramUrl,
    string? AboutTitle,
    string? AboutIntro,
    string? AboutStoryTitle,
    string? AboutStory,
    IReadOnlyList<AboutPhotoDto> AboutPhotos);

public static class ContentMapper
{
    public static AboutPhotoDto ToDto(AboutPhoto p) => new(p.Id, p.Url, p.Caption, p.DisplayOrder);

    public static SiteContentDto ToDto(SiteContent c) => new(
        c.WhatsApp,
        c.InstagramUrl,
        c.AboutTitle,
        c.AboutIntro,
        c.AboutStoryTitle,
        c.AboutStory,
        c.AboutPhotos.OrderBy(p => p.DisplayOrder).Select(ToDto).ToList());
}
