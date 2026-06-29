using Kessler.Application.Orders.Commands.CreateCommission;
using Kessler.Application.Orders.Commands.ManageCommission;
using Kessler.Application.Orders.Queries.GetCommissionByCode;
using Kessler.Application.Orders.Queries.ListCommissions;
using Kessler.Domain.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kessler.Web.Api.Controllers;

public sealed class CommissionsController(ISender sender) : ApiControllerBase
{
    /// <summary>Cria uma encomenda sob medida (convidado).</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommissionCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command, ct));

    /// <summary>Acompanhamento público pelo código da encomenda.</summary>
    [HttpGet("track/{code}")]
    public async Task<IActionResult> Track(string code, CancellationToken ct) =>
        HandleResult(await sender.Send(new GetCommissionByCodeQuery(code), ct));

    // ── Admin ────────────────────────────────────────────────────────

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] CommissionStatus? status, CancellationToken ct) =>
        HandleResult(await sender.Send(new ListCommissionsQuery(status), ct));

    [Authorize]
    [HttpPut("{id:guid}/quote")]
    public async Task<IActionResult> SendQuote(Guid id, [FromBody] SendCommissionQuoteCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command with { Id = id }, ct));

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCommissionCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command with { Id = id }, ct));
}
