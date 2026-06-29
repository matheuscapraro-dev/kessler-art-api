using Kessler.Application.Abstractions.Storage;
using Kessler.Application.Catalog.Commands.AddProductImage;
using Kessler.Application.Catalog.Commands.CreateProduct;
using Kessler.Application.Catalog.Commands.DeleteProduct;
using Kessler.Application.Catalog.Commands.RemoveProductImage;
using Kessler.Application.Catalog.Commands.SetCoverImage;
using Kessler.Application.Catalog.Commands.UpdateProduct;
using Kessler.Application.Catalog.Queries.GetProductById;
using Kessler.Application.Catalog.Queries.GetProductBySlug;
using Kessler.Application.Catalog.Queries.ListProducts;
using Kessler.Domain.Catalog;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kessler.Web.Api.Controllers;

public sealed class ProductsController(ISender sender, IStorageService storage) : ApiControllerBase
{
    // ── Público ──────────────────────────────────────────────────────

    /// <summary>Lista peças (galeria/loja). Sem filtro = todas publicadas.</summary>
    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? categorySlug,
        [FromQuery] ProductAvailability? availability,
        [FromQuery] bool featuredOnly = false,
        [FromQuery] string? search = null,
        CancellationToken ct = default) =>
        HandleResult(await sender.Send(
            new ListProductsQuery(categorySlug, availability, featuredOnly, PublishedOnly: true, search), ct));

    /// <summary>Detalhe público de uma peça por slug.</summary>
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct) =>
        HandleResult(await sender.Send(new GetProductBySlugQuery(slug), ct));

    // ── Admin ────────────────────────────────────────────────────────

    /// <summary>Listagem para o admin — inclui peças não publicadas.</summary>
    [Authorize]
    [HttpGet("admin")]
    public async Task<IActionResult> ListAdmin(
        [FromQuery] string? categorySlug,
        [FromQuery] ProductAvailability? availability,
        [FromQuery] string? search,
        CancellationToken ct = default) =>
        HandleResult(await sender.Send(
            new ListProductsQuery(categorySlug, availability, FeaturedOnly: false, PublishedOnly: false, search), ct));

    [Authorize]
    [HttpGet("by-id/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        HandleResult(await sender.Send(new GetProductByIdQuery(id), ct));

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command, ct));

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command with { Id = id }, ct));

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        HandleResult(await sender.Send(new DeleteProductCommand(id), ct));

    // ── Imagens (admin) ──────────────────────────────────────────────

    [Authorize]
    [HttpPost("{id:guid}/images")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> AddImage(Guid id, IFormFile file, [FromForm] string? altText, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new ProblemDetails { Title = "Arquivo inválido", Status = 400, Detail = "Envie um arquivo de imagem." });

        await using var stream = file.OpenReadStream();
        var stored = await storage.SaveAsync(stream, file.FileName, file.ContentType, "products", ct);

        var result = await sender.Send(new AddProductImageCommand(id, stored.StorageKey, stored.Url, altText), ct);
        if (result.IsFailed)
            await storage.DeleteAsync(stored.StorageKey, ct); // não deixa arquivo órfão

        return HandleResult(result);
    }

    [Authorize]
    [HttpDelete("{id:guid}/images/{imageId:guid}")]
    public async Task<IActionResult> RemoveImage(Guid id, Guid imageId, CancellationToken ct) =>
        HandleResult(await sender.Send(new RemoveProductImageCommand(id, imageId), ct));

    [Authorize]
    [HttpPut("{id:guid}/images/{imageId:guid}/cover")]
    public async Task<IActionResult> SetCover(Guid id, Guid imageId, CancellationToken ct) =>
        HandleResult(await sender.Send(new SetCoverImageCommand(id, imageId), ct));
}
