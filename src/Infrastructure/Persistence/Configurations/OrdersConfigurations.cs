using Kessler.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kessler.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("orders");
        b.HasKey(o => o.Id);
        b.Property(o => o.Code).HasMaxLength(20).IsRequired();
        b.HasIndex(o => o.Code).IsUnique();
        b.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
        b.Property(o => o.PaymentMethod).HasConversion<string>().HasMaxLength(20);
        b.Property(o => o.PaymentStatus).HasConversion<string>().HasMaxLength(20);
        b.Property(o => o.TotalAmount).HasPrecision(10, 2);
        b.Property(o => o.Notes).HasMaxLength(1000);

        b.OwnsOne(o => o.Customer, c =>
        {
            c.Property(p => p.Name).HasColumnName("customer_name").HasMaxLength(160).IsRequired();
            c.Property(p => p.Email).HasColumnName("customer_email").HasMaxLength(256).IsRequired();
            c.Property(p => p.Phone).HasColumnName("customer_phone").HasMaxLength(40).IsRequired();
        });

        b.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        b.Metadata.FindNavigation(nameof(Order.Items))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        b.HasIndex(o => o.Status);
    }
}

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> b)
    {
        b.ToTable("order_items");
        b.HasKey(i => i.Id);
        b.Property(i => i.ProductName).HasMaxLength(160).IsRequired();
        b.Property(i => i.UnitPrice).HasPrecision(10, 2);
        b.Ignore(i => i.LineTotal); // calculado
        b.HasIndex(i => i.OrderId);
    }
}

public sealed class CommissionRequestConfiguration : IEntityTypeConfiguration<CommissionRequest>
{
    public void Configure(EntityTypeBuilder<CommissionRequest> b)
    {
        b.ToTable("commission_requests");
        b.HasKey(c => c.Id);
        b.Property(c => c.Code).HasMaxLength(20).IsRequired();
        b.HasIndex(c => c.Code).IsUnique();
        b.Property(c => c.Description).HasMaxLength(2000).IsRequired();
        b.Property(c => c.DesiredCategory).HasMaxLength(120);
        b.Property(c => c.Colors).HasMaxLength(300);
        b.Property(c => c.Size).HasMaxLength(300);
        b.Property(c => c.ReferenceProductSlug).HasMaxLength(180);
        b.Property(c => c.QuotedPrice).HasPrecision(10, 2);
        b.Property(c => c.Status).HasConversion<string>().HasMaxLength(24);
        b.Property(c => c.AdminNotes).HasMaxLength(2000);

        b.OwnsOne(c => c.Customer, x =>
        {
            x.Property(p => p.Name).HasColumnName("customer_name").HasMaxLength(160).IsRequired();
            x.Property(p => p.Email).HasColumnName("customer_email").HasMaxLength(256).IsRequired();
            x.Property(p => p.Phone).HasColumnName("customer_phone").HasMaxLength(40).IsRequired();
        });

        b.HasIndex(c => c.Status);
    }
}
