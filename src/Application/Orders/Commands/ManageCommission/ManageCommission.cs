using FluentResults;
using FluentValidation;
using Kessler.Application.Abstractions;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Common.Errors;
using Kessler.Application.Orders.Dtos;
using Kessler.Application.Orders.Mapping;
using Kessler.Domain.Orders;
using MediatR;

namespace Kessler.Application.Orders.Commands.ManageCommission;

// ── Enviar orçamento ─────────────────────────────────────────────────
public sealed record SendCommissionQuoteCommand(Guid Id, decimal Price) : IRequest<Result<CommissionDto>>;

public sealed class SendCommissionQuoteCommandValidator : AbstractValidator<SendCommissionQuoteCommand>
{
    public SendCommissionQuoteCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Price).GreaterThan(0);
    }
}

internal sealed class SendCommissionQuoteCommandHandler(
    ICommissionRepository commissions,
    IUnitOfWork unitOfWork) : IRequestHandler<SendCommissionQuoteCommand, Result<CommissionDto>>
{
    public async Task<Result<CommissionDto>> Handle(SendCommissionQuoteCommand request, CancellationToken cancellationToken)
    {
        var commission = await commissions.GetByIdAsync(request.Id, cancellationToken);
        if (commission is null)
            return AppResults.NotFound("Encomenda não encontrada.");

        commission.SendQuote(request.Price);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(OrdersMapper.ToDto(commission));
    }
}

// ── Atualizar status / notas ─────────────────────────────────────────
public sealed record UpdateCommissionCommand(Guid Id, CommissionStatus Status, string? AdminNotes)
    : IRequest<Result<CommissionDto>>;

internal sealed class UpdateCommissionCommandHandler(
    ICommissionRepository commissions,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCommissionCommand, Result<CommissionDto>>
{
    public async Task<Result<CommissionDto>> Handle(UpdateCommissionCommand request, CancellationToken cancellationToken)
    {
        var commission = await commissions.GetByIdAsync(request.Id, cancellationToken);
        if (commission is null)
            return AppResults.NotFound("Encomenda não encontrada.");

        commission.UpdateStatus(request.Status);
        commission.SetAdminNotes(request.AdminNotes);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(OrdersMapper.ToDto(commission));
    }
}
