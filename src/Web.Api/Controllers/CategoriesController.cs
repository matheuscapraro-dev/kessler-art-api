using Kessler.Application.Catalog.Commands.CreateCategory;
using Kessler.Application.Catalog.Commands.DeleteCategory;
using Kessler.Application.Catalog.Commands.UpdateCategory;
using Kessler.Application.Catalog.Queries.ListCategories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kessler.Web.Api.Controllers;

public sealed class CategoriesController(ISender sender) : ApiControllerBase
{
    /// <summary>Lista categorias (público: só publicadas; admin: todas).</summary>
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] bool publishedOnly = true, CancellationToken ct = default) =>
        HandleResult(await sender.Send(new ListCategoriesQuery(publishedOnly), ct));

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command, ct));

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command with { Id = id }, ct));

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        HandleResult(await sender.Send(new DeleteCategoryCommand(id), ct));
}
