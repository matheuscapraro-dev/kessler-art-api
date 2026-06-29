using FluentResults;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Orders.Dtos;
using Kessler.Application.Orders.Mapping;
using Kessler.Domain.Orders;
using MediatR;

namespace Kessler.Application.Orders.Queries.ListCommissions;

public sealed record ListCommissionsQuery(CommissionStatus? Status = null)
    : IRequest<Result<IReadOnlyList<CommissionSummaryDto>>>;

internal sealed class ListCommissionsQueryHandler(ICommissionRepository commissions)
    : IRequestHandler<ListCommissionsQuery, Result<IReadOnlyList<CommissionSummaryDto>>>
{
    public async Task<Result<IReadOnlyList<CommissionSummaryDto>>> Handle(
        ListCommissionsQuery request,
        CancellationToken cancellationToken)
    {
        var list = await commissions.ListAsync(request.Status, cancellationToken);
        IReadOnlyList<CommissionSummaryDto> dtos = list.Select(OrdersMapper.ToSummary).ToList();
        return Result.Ok(dtos);
    }
}
