using Kessler.Application.Abstractions.Storage;
using Kessler.Application.Orders.Commands.CreateCommission;
using Kessler.Application.Orders.Commands.ManageCommission;
using Kessler.Application.Orders.Queries.GetCommissionByCode;
using Kessler.Application.Orders.Queries.ListCommissions;
using Kessler.Domain.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kessler.Web.Api.Controllers;

public sealed class CommissionsController(ISender sender, IStorageService storage) : ApiControllerBase
{
    /// <summary>Cria uma encomenda sob medida (convidado).</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommissionCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command, ct));

    /// <summary>
    /// Upload de uma imagem de referência (antes de criar a encomenda). Anônimo —
    /// retorna os identificadores que vão no corpo do POST de criação.
    /// </summary>
    [HttpPost("reference-images")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> UploadReference(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0 || !file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return BadRequest(new ProblemDetails { Title = "Arquivo inválido", Status = 400, Detail = "Envie um arquivo de imagem." });

        await using var stream = file.OpenReadStream();
        var stored = await storage.SaveAsync(stream, file.FileName, file.ContentType, "commission-refs", ct);
        return Ok(new { storageKey = stored.StorageKey, url = stored.Url });
    }

    /// <summary>Acompanhamento público pelo código da encomenda.</summary>
    [HttpGet("track/{code}")]
    public async Task<IActionResult> Track(string code, CancellationToken ct) =>
        HandleResult(await sender.Send(new GetCommissionByCodeQuery(code), ct));

    // ── Admin ────────────────────────────────────────────────────────

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] CommissionStatus? status, CancellationToken ct) =>
        HandleResult(await sender.Send(new ListCommissionsQuery(status), ct));

    /// <summary>Cria um trabalho/encomenda pelo painel (cliente opcional).</summary>
    [Authorize]
    [HttpPost("admin")]
    public async Task<IActionResult> CreateAdmin([FromBody] CreateCommissionCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command, ct));

    [Authorize]
    [HttpPut("{id:guid}/quote")]
    public async Task<IActionResult> SendQuote(Guid id, [FromBody] SendCommissionQuoteCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command with { Id = id }, ct));

    /// <summary>Edição completa do trabalho (briefing, tipo, prioridade, status, orçamento, notas).</summary>
    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCommissionCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command with { Id = id }, ct));

    /// <summary>Move o cartão no quadro (coluna + posição) — drag-and-drop.</summary>
    [Authorize]
    [HttpPut("{id:guid}/move")]
    public async Task<IActionResult> Move(Guid id, [FromBody] MoveCommissionCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command with { Id = id }, ct));

    /// <summary>Sincroniza a checklist de tarefas do trabalho.</summary>
    [Authorize]
    [HttpPut("{id:guid}/tasks")]
    public async Task<IActionResult> SetTasks(Guid id, [FromBody] SetCommissionTasksCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command with { Id = id }, ct));

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct) =>
        HandleResult(await sender.Send(new DeleteCommissionCommand(id), ct));
}
