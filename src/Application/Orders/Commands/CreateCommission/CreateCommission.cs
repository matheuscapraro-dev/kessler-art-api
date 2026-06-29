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
    string CustomerName,
    string? CustomerEmail,
    string CustomerPhone,
    string Description,
    string? DesiredCategory,
    string? Colors,
    string? Size,
    DateTime? DesiredDeadline,
    string? ReferenceProductSlug,
    IReadOnlyList<CommissionReferenceInput>? ReferenceImages = null) : IRequest<Result<CommissionDto>>;

public sealed class CreateCommissionCommandValidator : AbstractValidator<CreateCommissionCommand>
{
    public CreateCommissionCommandValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(160);
        // E-mail opcional (admin pode cadastrar pedido por telefone) — valida o formato só se preenchido.
        RuleFor(x => x.CustomerEmail).EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.CustomerEmail));
        RuleFor(x => x.CustomerPhone).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Conte o que você gostaria de encomendar.")
            .MinimumLength(10).MaximumLength(2000);
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
        var customer = CustomerInfo.Create(request.CustomerName, request.CustomerEmail ?? "", request.CustomerPhone);
        var references = request.ReferenceImages?
            .Select(r => new CommissionReference(r.StorageKey, r.Url));

        var commission = CommissionRequest.Create(
            customer,
            request.Description,
            request.DesiredCategory,
            request.Colors,
            request.Size,
            request.DesiredDeadline,
            request.ReferenceProductSlug,
            references);

        commissions.Add(commission);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(OrdersMapper.ToDto(commission));
    }
}
