using Kessler.Domain.Common;

namespace Kessler.Domain.Orders;

/// <summary>Item de um pedido. Guarda snapshot de nome/preço para preservar o histórico.</summary>
public sealed class OrderItem : Entity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = null!;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }

    public decimal LineTotal => UnitPrice * Quantity;

    private OrderItem() { } // EF Core

    internal static OrderItem Create(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        return new OrderItem
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            ProductName = productName.Trim(),
            UnitPrice = unitPrice,
            Quantity = quantity < 1 ? 1 : quantity
        };
    }
}
