using Kessler.Application.Orders.Commands.CreateOrder;
using Kessler.Application.Orders.Commands.ManageOrder;
using Kessler.Application.Orders.Queries.GetOrderByCode;
using Kessler.Application.Orders.Queries.ListOrders;
using Kessler.Domain.Orders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kessler.Web.Api.Controllers;

public sealed class OrdersController(ISender sender) : ApiControllerBase
{
    /// <summary>Cria um pedido de peças prontas (checkout convidado).</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command, ct));

    /// <summary>Acompanhamento público pelo código do pedido.</summary>
    [HttpGet("track/{code}")]
    public async Task<IActionResult> Track(string code, CancellationToken ct) =>
        HandleResult(await sender.Send(new GetOrderByCodeQuery(code), ct));

    // ── Admin ────────────────────────────────────────────────────────

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] OrderStatus? status, CancellationToken ct) =>
        HandleResult(await sender.Send(new ListOrdersQuery(status), ct));

    [Authorize]
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusCommand command, CancellationToken ct) =>
        HandleResult(await sender.Send(command with { Id = id }, ct));

    [Authorize]
    [HttpPut("{id:guid}/paid")]
    public async Task<IActionResult> MarkPaid(Guid id, CancellationToken ct) =>
        HandleResult(await sender.Send(new MarkOrderPaidCommand(id), ct));
}
