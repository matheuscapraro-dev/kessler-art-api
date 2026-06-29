using FluentResults;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Orders.Dtos;
using Kessler.Application.Orders.Mapping;
using Kessler.Domain.Orders;
using MediatR;

namespace Kessler.Application.Orders.Queries.ListOrders;

public sealed record ListOrdersQuery(OrderStatus? Status = null)
    : IRequest<Result<IReadOnlyList<OrderSummaryDto>>>;

internal sealed class ListOrdersQueryHandler(IOrderRepository orders)
    : IRequestHandler<ListOrdersQuery, Result<IReadOnlyList<OrderSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<OrderSummaryDto>>> Handle(
        ListOrdersQuery request,
        CancellationToken cancellationToken)
    {
        var list = await orders.ListAsync(request.Status, cancellationToken);
        IReadOnlyList<OrderSummaryDto> dtos = list.Select(OrdersMapper.ToSummary).ToList();
        return Result.Ok(dtos);
    }
}
