using FluentResults;
using Kessler.Application.Abstractions;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Abstractions.Storage;
using Kessler.Application.Common.Errors;
using Kessler.Application.Content.Dtos;
using Kessler.Domain.Content;
using MediatR;

namespace Kessler.Application.Content.Commands;

// ── Adicionar foto (metadados; upload já feito no controller) ─────────
public sealed record AddAboutPhotoCommand(string StorageKey, string Url, string? Caption)
    : IRequest<Result<AboutPhotoDto>>;

internal sealed class AddAboutPhotoCommandHandler(
    IContentRepository content,
    IUnitOfWork unitOfWork) : IRequestHandler<AddAboutPhotoCommand, Result<AboutPhotoDto>>
{
    public async Task<Result<AboutPhotoDto>> Handle(AddAboutPhotoCommand request, CancellationToken cancellationToken)
    {
        var site = await content.GetAsync(cancellationToken);
        if (site is null)
        {
            site = SiteContent.CreateDefault();
            content.Add(site);
        }

        var photo = site.AddAboutPhoto(request.StorageKey, request.Url, request.Caption);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(ContentMapper.ToDto(photo));
    }
}

// ── Remover foto ─────────────────────────────────────────────────────
public sealed record RemoveAboutPhotoCommand(Guid PhotoId) : IRequest<Result>;

internal sealed class RemoveAboutPhotoCommandHandler(
    IContentRepository content,
    IStorageService storage,
    IUnitOfWork unitOfWork) : IRequestHandler<RemoveAboutPhotoCommand, Result>
{
    public async Task<Result> Handle(RemoveAboutPhotoCommand request, CancellationToken cancellationToken)
    {
        var site = await content.GetAsync(cancellationToken);
        var photo = site?.AboutPhotos.FirstOrDefault(p => p.Id == request.PhotoId);
        if (site is null || photo is null)
            return AppResults.NotFound("Foto não encontrada.");

        var storageKey = photo.StorageKey;
        site.RemoveAboutPhoto(request.PhotoId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        await storage.DeleteAsync(storageKey, cancellationToken);
        return Result.Ok();
    }
}
