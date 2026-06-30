using Kessler.Application.Orders.Dtos;
using Kessler.Domain.Orders;

namespace Kessler.Application.Orders.Mapping;

public static class OrdersMapper
{
    public static OrderItemDto ToDto(OrderItem item) => new(
        item.ProductId,
        item.ProductName,
        item.UnitPrice,
        item.Quantity,
        item.LineTotal);

    public static OrderDto ToDto(Order order) => new(
        order.Id,
        order.Code,
        order.Customer.Name,
        order.Customer.Email,
        order.Customer.Phone,
        order.Status,
        order.PaymentMethod,
        order.PaymentStatus,
        order.TotalAmount,
        order.Notes,
        order.CreatedAt,
        order.Items.Select(ToDto).ToList());

    public static OrderSummaryDto ToSummary(Order order) => new(
        order.Id,
        order.Code,
        order.Customer.Name,
        order.Status,
        order.PaymentStatus,
        order.TotalAmount,
        order.Items.Count,
        order.CreatedAt);

    public static CommissionDto ToDto(CommissionRequest c) => new(
        c.Id,
        c.Code,
        c.Type,
        c.Title,
        c.Customer?.Name,
        c.Customer?.Email,
        c.Customer?.Phone,
        c.Description,
        c.DesiredCategory,
        c.Colors,
        c.Size,
        c.DesiredDeadline,
        c.ReferenceProductSlug,
        c.QuotedPrice,
        c.Status,
        c.Priority,
        c.Position,
        c.AdminNotes,
        c.CreatedAt,
        c.ReferenceImages.Select(i => new CommissionImageDto(i.Id, i.Url)).ToList(),
        c.Tasks.OrderBy(t => t.Position).Select(t => new CommissionTaskDto(t.Id, t.Title, t.IsDone)).ToList());

    public static CommissionSummaryDto ToSummary(CommissionRequest c) => new(
        c.Id,
        c.Code,
        c.Type,
        c.Title,
        c.Customer?.Name,
        c.Description,
        c.Status,
        c.Priority,
        c.Position,
        c.QuotedPrice,
        c.DesiredDeadline,
        c.Tasks.Count,
        c.Tasks.Count(t => t.IsDone),
        c.ReferenceImages.Count,
        c.CreatedAt);
}
