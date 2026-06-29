using Kessler.Domain.Common;
using Kessler.Domain.Common.Errors;

namespace Kessler.Domain.Orders;

/// <summary>Dados de uma linha do pedido passados à fábrica (snapshot de nome/preço).</summary>
public sealed record OrderLine(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity);

/// <summary>
/// Pedido de peças prontas feito como convidado. Pagamento manual no v1
/// (Pix/WhatsApp); o snapshot de itens preserva valores no tempo.
/// </summary>
public sealed class Order : AuditableEntity
{
    private readonly List<OrderItem> _items = [];

    public string Code { get; private set; } = null!;
    public CustomerInfo Customer { get; private set; } = null!;
    public OrderStatus Status { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string? Notes { get; private set; }

    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

    private Order() { } // EF Core

    public static Order Create(CustomerInfo customer, IEnumerable<OrderLine> lines, string? notes = null)
    {
        var items = lines
            .Select(l => OrderItem.Create(l.ProductId, l.ProductName, l.UnitPrice, l.Quantity))
            .ToList();

        if (items.Count == 0)
            throw new DomainException("Um pedido precisa de ao menos um item.");

        var order = new Order
        {
            Id = Guid.NewGuid(),
            Code = ReferenceCode.New("KES"),
            Customer = customer,
            Status = OrderStatus.Pendente,
            PaymentMethod = PaymentMethod.Manual,
            PaymentStatus = PaymentStatus.Pendente,
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim()
        };
        order._items.AddRange(items);
        order.TotalAmount = items.Sum(i => i.LineTotal);
        return order;
    }

    public void UpdateStatus(OrderStatus status) => Status = status;

    public void MarkPaid()
    {
        PaymentStatus = PaymentStatus.Pago;
        if (Status == OrderStatus.Pendente)
            Status = OrderStatus.Confirmado;
    }
}
