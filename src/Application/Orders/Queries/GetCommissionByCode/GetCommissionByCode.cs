using FluentResults;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Common.Errors;
using Kessler.Application.Orders.Dtos;
using Kessler.Application.Orders.Mapping;
using MediatR;

namespace Kessler.Application.Orders.Queries.GetCommissionByCode;

public sealed record GetCommissionByCodeQuery(string Code) : IRequest<Result<CommissionDto>>;

internal sealed class GetCommissionByCodeQueryHandler(ICommissionRepository commissions)
    : IRequestHandler<GetCommissionByCodeQuery, Result<CommissionDto>>
{
    public async Task<Result<CommissionDto>> Handle(GetCommissionByCodeQuery request, CancellationToken cancellationToken)
    {
        var commission = await commissions.GetByCodeAsync(request.Code.Trim().ToUpperInvariant(), cancellationToken);
        return commission is null
            ? AppResults.NotFound("Encomenda não encontrada.")
            : Result.Ok(OrdersMapper.ToDto(commission));
    }
}
