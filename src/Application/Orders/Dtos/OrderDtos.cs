using Kessler.Domain.Orders;

namespace Kessler.Application.Orders.Dtos;

// ── Pedidos (peças prontas) ──────────────────────────────────────────

public sealed record OrderItemDto(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal);

public sealed record OrderDto(
    Guid Id,
    string Code,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    OrderStatus Status,
    PaymentMethod PaymentMethod,
    PaymentStatus PaymentStatus,
    decimal TotalAmount,
    string? Notes,
    DateTime CreatedAt,
    IReadOnlyList<OrderItemDto> Items);

public sealed record OrderSummaryDto(
    Guid Id,
    string Code,
    string CustomerName,
    OrderStatus Status,
    PaymentStatus PaymentStatus,
    decimal TotalAmount,
    int ItemCount,
    DateTime CreatedAt);

// ── Encomendas / trabalhos do ateliê ─────────────────────────────────

public sealed record CommissionImageDto(Guid Id, string Url);

public sealed record CommissionTaskDto(Guid Id, string Title, bool IsDone);

public sealed record CommissionDto(
    Guid Id,
    string Code,
    WorkType Type,
    string? Title,
    string? CustomerName,
    string? CustomerEmail,
    string? CustomerPhone,
    string Description,
    string? DesiredCategory,
    string? Colors,
    string? Size,
    DateTime? DesiredDeadline,
    string? ReferenceProductSlug,
    decimal? QuotedPrice,
    CommissionStatus Status,
    WorkPriority Priority,
    double Position,
    string? AdminNotes,
    DateTime CreatedAt,
    IReadOnlyList<CommissionImageDto> ReferenceImages,
    IReadOnlyList<CommissionTaskDto> Tasks);

public sealed record CommissionSummaryDto(
    Guid Id,
    string Code,
    WorkType Type,
    string? Title,
    string? CustomerName,
    string Description,
    CommissionStatus Status,
    WorkPriority Priority,
    double Position,
    decimal? QuotedPrice,
    DateTime? DesiredDeadline,
    int TaskCount,
    int DoneTaskCount,
    int ReferenceImageCount,
    DateTime CreatedAt);
