using Kessler.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kessler.Infrastructure.Persistence.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users");
        b.HasKey(u => u.Id);
        b.Property(u => u.Email).HasMaxLength(256).IsRequired();
        b.HasIndex(u => u.Email).IsUnique();
        b.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();
        b.Property(u => u.Name).HasMaxLength(160).IsRequired();
        b.Property(u => u.Role).HasMaxLength(40).IsRequired();
    }
}
