using FluentResults;
using FluentValidation;
using Kessler.Application.Abstractions;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Orders.Dtos;
using Kessler.Application.Orders.Mapping;
using Kessler.Domain.Orders;
using MediatR;

namespace Kessler.Application.Orders.Commands.CreateCommission;

public sealed record CommissionReferenceInput(string StorageKey, string Url);

public sealed record CreateCommissionCommand(
    string Description,
    WorkType Type = WorkType.Encomenda,
    string? Title = null,
    WorkPriority Priority = WorkPriority.Normal,
    CommissionStatus Status = CommissionStatus.Nova,
    // Cliente é opcional: encomendas trazem contato, trabalhos próprios da artista não.
    string? CustomerName = null,
    string? CustomerEmail = null,
    string? CustomerPhone = null,
    string? DesiredCategory = null,
    string? Colors = null,
    string? Size = null,
    DateTime? DesiredDeadline = null,
    decimal? QuotedPrice = null,
    string? ReferenceProductSlug = null,
    IReadOnlyList<CommissionReferenceInput>? ReferenceImages = null) : IRequest<Result<CommissionDto>>;

public sealed class CreateCommissionCommandValidator : AbstractValidator<CreateCommissionCommand>
{
    public CreateCommissionCommandValidator()
    {
        // Encomenda de cliente exige contato; trabalho próprio (sem cliente) não.
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(160)
            .When(x => x.Type == WorkType.Encomenda);
        RuleFor(x => x.CustomerName).MaximumLength(160);
        // E-mail opcional (a artista pode cadastrar por telefone) — valida o formato só se preenchido.
        RuleFor(x => x.CustomerEmail).EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.CustomerEmail));
        RuleFor(x => x.CustomerPhone).NotEmpty().MaximumLength(40)
            .When(x => x.Type == WorkType.Encomenda);
        RuleFor(x => x.CustomerPhone).MaximumLength(40);
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Conte o que será feito neste trabalho.")
            .MinimumLength(10).MaximumLength(2000);
        RuleFor(x => x.Title).MaximumLength(160);
        RuleFor(x => x.Colors).MaximumLength(300);
        RuleFor(x => x.Size).MaximumLength(300);
        RuleFor(x => x.DesiredCategory).MaximumLength(120);
    }
}

internal sealed class CreateCommissionCommandHandler(
    ICommissionRepository commissions,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateCommissionCommand, Result<CommissionDto>>
{
    public async Task<Result<CommissionDto>> Handle(CreateCommissionCommand request, CancellationToken cancellationToken)
    {
        // Sem nome de cliente = trabalho próprio da artista (cliente fica nulo).
        var customer = string.IsNullOrWhiteSpace(request.CustomerName)
            ? null
            : CustomerInfo.Create(request.CustomerName, request.CustomerEmail ?? "", request.CustomerPhone ?? "");

        var references = request.ReferenceImages?
            .Select(r => new CommissionReference(r.StorageKey, r.Url));

        var commission = CommissionRequest.Create(
            request.Description,
            customer,
            request.Type,
            request.Title,
            request.Priority,
            request.Status,
            request.DesiredCategory,
            request.Colors,
            request.Size,
            request.DesiredDeadline,
            request.QuotedPrice,
            request.ReferenceProductSlug,
            references);

        commissions.Add(commission);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(OrdersMapper.ToDto(commission));
    }
}
