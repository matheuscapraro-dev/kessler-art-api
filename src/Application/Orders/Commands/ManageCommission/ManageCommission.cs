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
            return AppResults.NotFound("Trabalho não encontrado.");

        commission.SendQuote(request.Price);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(OrdersMapper.ToDto(commission));
    }
}

// ── Edição completa (briefing, tipo, prioridade, status, orçamento, notas) ──
public sealed record UpdateCommissionCommand(
    Guid Id,
    string Description,
    WorkType Type,
    string? Title,
    WorkPriority Priority,
    CommissionStatus Status,
    string? DesiredCategory,
    string? Colors,
    string? Size,
    DateTime? DesiredDeadline,
    decimal? QuotedPrice,
    string? AdminNotes) : IRequest<Result<CommissionDto>>;

public sealed class UpdateCommissionCommandValidator : AbstractValidator<UpdateCommissionCommand>
{
    public UpdateCommissionCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Descreva o trabalho.")
            .MinimumLength(10).MaximumLength(2000);
        RuleFor(x => x.Title).MaximumLength(160);
        RuleFor(x => x.Colors).MaximumLength(300);
        RuleFor(x => x.Size).MaximumLength(300);
        RuleFor(x => x.DesiredCategory).MaximumLength(120);
        RuleFor(x => x.QuotedPrice).GreaterThan(0).When(x => x.QuotedPrice.HasValue);
    }
}

internal sealed class UpdateCommissionCommandHandler(
    ICommissionRepository commissions,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCommissionCommand, Result<CommissionDto>>
{
    public async Task<Result<CommissionDto>> Handle(UpdateCommissionCommand request, CancellationToken cancellationToken)
    {
        var commission = await commissions.GetByIdAsync(request.Id, cancellationToken);
        if (commission is null)
            return AppResults.NotFound("Trabalho não encontrado.");

        commission.UpdateDetails(
            request.Description,
            request.Type,
            request.Title,
            request.Priority,
            request.DesiredCategory,
            request.Colors,
            request.Size,
            request.DesiredDeadline);
        commission.UpdateStatus(request.Status);
        commission.SetQuote(request.QuotedPrice);
        commission.SetAdminNotes(request.AdminNotes);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(OrdersMapper.ToDto(commission));
    }
}

// ── Mover no quadro (drag-and-drop: coluna + posição) ────────────────
public sealed record MoveCommissionCommand(Guid Id, CommissionStatus Status, double Position)
    : IRequest<Result>;

internal sealed class MoveCommissionCommandHandler(
    ICommissionRepository commissions,
    IUnitOfWork unitOfWork) : IRequestHandler<MoveCommissionCommand, Result>
{
    public async Task<Result> Handle(MoveCommissionCommand request, CancellationToken cancellationToken)
    {
        var commission = await commissions.GetByIdAsync(request.Id, cancellationToken);
        if (commission is null)
            return AppResults.NotFound("Trabalho não encontrado.");

        commission.MoveTo(request.Status, request.Position);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}

// ── Sincronizar checklist ────────────────────────────────────────────
public sealed record CommissionTaskItem(string Title, bool IsDone);

public sealed record SetCommissionTasksCommand(Guid Id, IReadOnlyList<CommissionTaskItem> Tasks)
    : IRequest<Result<CommissionDto>>;

internal sealed class SetCommissionTasksCommandHandler(
    ICommissionRepository commissions,
    IUnitOfWork unitOfWork) : IRequestHandler<SetCommissionTasksCommand, Result<CommissionDto>>
{
    public async Task<Result<CommissionDto>> Handle(SetCommissionTasksCommand request, CancellationToken cancellationToken)
    {
        var commission = await commissions.GetByIdAsync(request.Id, cancellationToken);
        if (commission is null)
            return AppResults.NotFound("Trabalho não encontrado.");

        commission.SetTasks(request.Tasks.Select(t => new CommissionTaskInput(t.Title, t.IsDone)));
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(OrdersMapper.ToDto(commission));
    }
}

// ── Excluir ──────────────────────────────────────────────────────────
public sealed record DeleteCommissionCommand(Guid Id) : IRequest<Result>;

internal sealed class DeleteCommissionCommandHandler(
    ICommissionRepository commissions,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCommissionCommand, Result>
{
    public async Task<Result> Handle(DeleteCommissionCommand request, CancellationToken cancellationToken)
    {
        var commission = await commissions.GetByIdAsync(request.Id, cancellationToken);
        if (commission is null)
            return AppResults.NotFound("Trabalho não encontrado.");

        commissions.Remove(commission);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok();
    }
}
