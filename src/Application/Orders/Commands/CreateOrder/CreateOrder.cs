using FluentResults;
using FluentValidation;
using Kessler.Application.Abstractions;
using Kessler.Application.Abstractions.Repositories;
using Kessler.Application.Common.Errors;
using Kessler.Application.Orders.Dtos;
using Kessler.Application.Orders.Mapping;
using Kessler.Domain.Catalog;
using Kessler.Domain.Orders;
using MediatR;

namespace Kessler.Application.Orders.Commands.CreateOrder;

public sealed record OrderLineInput(Guid ProductId, int Quantity);

public sealed record CreateOrderCommand(
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    IReadOnlyList<OrderLineInput> Items,
    string? Notes) : IRequest<Result<OrderDto>>;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(160);
        RuleFor(x => x.CustomerEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.CustomerPhone).NotEmpty().MaximumLength(40);
        RuleFor(x => x.Items).NotEmpty().WithMessage("Adicione ao menos uma peça ao pedido.");
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.ProductId).NotEmpty();
            item.RuleFor(i => i.Quantity).GreaterThan(0);
        });
    }
}

internal sealed class CreateOrderCommandHandler(
    IProductRepository products,
    IOrderRepository orders,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    public async Task<Result<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var ids = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var byId = (await products.GetByIdsAsync(ids, cancellationToken)).ToDictionary(p => p.Id);

        var lines = new List<OrderLine>();
        foreach (var item in request.Items)
        {
            if (!byId.TryGetValue(item.ProductId, out var product))
                return AppResults.NotFound("Uma das peças do pedido não existe mais.");

            if (product.Availability != ProductAvailability.ReadyToBuy || product.Price is null)
                return AppResults.DomainRule($"A peça \"{product.Name}\" não está disponível para compra direta.");

            if (!product.HasStock(item.Quantity))
                return AppResults.Conflict($"Estoque insuficiente para \"{product.Name}\".");

            product.DecreaseStock(item.Quantity);
            lines.Add(new OrderLine(product.Id, product.Name, product.Price.Value, item.Quantity));
        }

        var customer = CustomerInfo.Create(request.CustomerName, request.CustomerEmail, request.CustomerPhone);
        var order = Order.Create(customer, lines, request.Notes);

        orders.Add(order);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(OrdersMapper.ToDto(order));
    }
}
