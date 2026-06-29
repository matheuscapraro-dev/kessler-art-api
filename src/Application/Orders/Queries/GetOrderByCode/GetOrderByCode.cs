using FluentResults;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Common.Errors;
using Kessler.Application.Orders.Dtos;
using Kessler.Application.Orders.Mapping;
using MediatR;

namespace Kessler.Application.Orders.Queries.GetOrderByCode;

/// <summary>Acompanhamento público do pedido pelo código (sem login).</summary>
public sealed record GetOrderByCodeQuery(string Code) : IRequest<Result<OrderDto>>;

internal sealed class GetOrderByCodeQueryHandler(IOrderRepository orders)
    : IRequestHandler<GetOrderByCodeQuery, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(GetOrderByCodeQuery request, CancellationToken cancellationToken)
    {
        var order = await orders.GetByCodeAsync(request.Code.Trim().ToUpperInvariant(), cancellationToken);
        return order is null
            ? AppResults.NotFound("Pedido não encontrado.")
            : Result.Ok(OrdersMapper.ToDto(order));
    }
}
