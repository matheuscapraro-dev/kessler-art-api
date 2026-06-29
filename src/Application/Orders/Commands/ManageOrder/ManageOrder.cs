using FluentResults;
using Kessler.Application.Abstractions;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Common.Errors;
using Kessler.Domain.Orders;
using MediatR;

namespace Kessler.Application.Orders.Commands.ManageOrder;

// ── Atualizar status ─────────────────────────────────────────────────
public sealed record UpdateOrderStatusCommand(Guid Id, OrderStatus Status) : IRequest<Result>;

internal sealed class UpdateOrderStatusCommandHandler(
    IOrderRepository orders,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateOrderStatusCommand, Result>
{
    public async Task<Result> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var order = await orders.GetByIdAsync(request.Id, cancellationToken);
        if (order is null)
            return AppResults.NotFound("Pedido não encontrado.");

        order.UpdateStatus(request.Status);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}

// ── Marcar como pago ─────────────────────────────────────────────────
public sealed record MarkOrderPaidCommand(Guid Id) : IRequest<Result>;

internal sealed class MarkOrderPaidCommandHandler(
    IOrderRepository orders,
    IUnitOfWork unitOfWork) : IRequestHandler<MarkOrderPaidCommand, Result>
{
    public async Task<Result> Handle(MarkOrderPaidCommand request, CancellationToken cancellationToken)
    {
        var order = await orders.GetByIdAsync(request.Id, cancellationToken);
        if (order is null)
            return AppResults.NotFound("Pedido não encontrado.");

        order.MarkPaid();
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
