using Kessler.Application.Abstractions.Storage;
using Kessler.Application.Content.Commands;
using Kessler.Application.Content.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kessler.Web.Api.Controllers;

public sealed class ContentController(ISender sender, IStorageService storage) : ApiControllerBase
{
    /// <summary>Conteúdo do site (público) — textos, contato e fotos da página Sobre.</summary>
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) =>
        HandleResult(await sender.Send(new GetSiteContentQuery(), ct));

    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateSiteContentCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command, ct));

    // ── Fotos da página Sobre (admin) ────────────────────────────────

    [Authorize]
    [HttpPost("about-photos")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> AddAboutPhoto(IFormFile file, [FromForm] string? caption, CancellationToken ct)
    {
        if (file is null || file.Length == 0 || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new ProblemDetails { Title = "Arquivo inválido", Status = 400, Detail = "Envie um arquivo de imagem." });

        await using var stream = file.OpenReadStream();
        var stored = await storage.SaveAsync(stream, file.FileName, file.ContentType, "about", ct);

        var result = await sender.Send(new AddAboutPhotoCommand(stored.StorageKey, stored.Url, caption), ct);
        if (result.IsFailed)
            await storage.DeleteAsync(stored.StorageKey, ct);

        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("about-photos/{photoId:guid}")]
    public async Task<IActionResult> RemoveAboutPhoto(Guid photoId, CancellationToken ct) =>
        HandleResult(await sender.Send(new RemoveAboutPhotoCommand(photoId), ct));
}
