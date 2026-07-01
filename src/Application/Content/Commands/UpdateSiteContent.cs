using FluentResults;
using FluentValidation;
using Kessler.Application.Abstractions;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Content.Dtos;
using Kessler.Domain.Content;
using MediatR;

namespace Kessler.Application.Content.Commands;

public sealed record UpdateSiteContentCommand(
    string? WhatsApp,
    string? InstagramUrl,
    string? AboutTitle,
    string? AboutIntro,
    string? AboutStoryTitle,
    string? AboutStory) : IRequest<Result<SiteContentDto>>;

public sealed class UpdateSiteContentCommandValidator : AbstractValidator<UpdateSiteContentCommand>
{
    public UpdateSiteContentCommandValidator()
    {
        RuleFor(x => x.WhatsApp).MaximumLength(20);
        RuleFor(x => x.InstagramUrl).MaximumLength(300);
        RuleFor(x => x.AboutTitle).MaximumLength(160);
        RuleFor(x => x.AboutIntro).MaximumLength(1000);
        RuleFor(x => x.AboutStoryTitle).MaximumLength(160);
        RuleFor(x => x.AboutStory).MaximumLength(4000);
    }
}

internal sealed class UpdateSiteContentCommandHandler(
    IContentRepository content,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateSiteContentCommand, Result<SiteContentDto>>
{
    public async Task<Result<SiteContentDto>> Handle(UpdateSiteContentCommand request, CancellationToken cancellationToken)
    {
        var site = await content.GetAsync(cancellationToken);
        if (site is null)
        {
            site = SiteContent.CreateDefault();
            content.Add(site);
        }

        site.Update(
            request.WhatsApp,
            request.InstagramUrl,
            request.AboutTitle,
            request.AboutIntro,
            request.AboutStoryTitle,
            request.AboutStory);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(ContentMapper.ToDto(site));
    }
}
