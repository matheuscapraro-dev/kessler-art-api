using Kessler.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kessler.Infrastructure.Persistence.Configurations;

public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> b)
    {
        b.ToTable("categories");
        b.HasKey(c => c.Id);
        b.Property(c => c.Name).HasMaxLength(120).IsRequired();
        b.Property(c => c.Slug).HasMaxLength(140).IsRequired();
        b.HasIndex(c => c.Slug).IsUnique();
        b.Property(c => c.Description).HasMaxLength(500);
    }
}

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("products");
        b.HasKey(p => p.Id);
        b.Property(p => p.Name).HasMaxLength(160).IsRequired();
        b.Property(p => p.Slug).HasMaxLength(180).IsRequired();
        b.HasIndex(p => p.Slug).IsUnique();
        b.Property(p => p.Description).HasMaxLength(4000);
        b.Property(p => p.Price).HasPrecision(10, 2);
        b.Property(p => p.Availability).HasConversion<string>().HasMaxLength(20);

        b.HasOne<Category>()
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Coleção privada _images exposta via Images (somente leitura) → acesso por field.
        b.HasMany(p => p.Images)
            .WithOne()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
        b.Metadata.FindNavigation(nameof(Product.Images))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        // Soft delete: esconde peças excluídas de todas as queries por padrão.
        b.HasQueryFilter(p => !p.IsDeleted);

        b.HasIndex(p => p.CategoryId);
        b.HasIndex(p => p.IsFeatured);
    }
}

public sealed class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> b)
    {
        b.ToTable("product_images");
        b.HasKey(i => i.Id);
        b.Property(i => i.StorageKey).HasMaxLength(400).IsRequired();
        b.Property(i => i.Url).HasMaxLength(600).IsRequired();
        b.Property(i => i.AltText).HasMaxLength(200);
        b.HasIndex(i => i.ProductId);
    }
}
