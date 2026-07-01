using FluentResults;
using Kessler.Application.Abstractions;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Content.Dtos;
using Kessler.Domain.Content;
using MediatR;

namespace Kessler.Application.Content.Queries;

/// <summary>Conteúdo do site (público). Cria o singleton com os padrões na 1ª vez.</summary>
public sealed record GetSiteContentQuery : IRequest<Result<SiteContentDto>>;

internal sealed class GetSiteContentQueryHandler(
    IContentRepository content,
    IUnitOfWork unitOfWork) : IRequestHandler<GetSiteContentQuery, Result<SiteContentDto>>
{
    public async Task<Result<SiteContentDto>> Handle(GetSiteContentQuery request, CancellationToken cancellationToken)
    {
        var site = await content.GetAsync(cancellationToken);
        if (site is null)
        {
            site = SiteContent.CreateDefault();
            content.Add(site);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Result.Ok(ContentMapper.ToDto(site));
    }
}
