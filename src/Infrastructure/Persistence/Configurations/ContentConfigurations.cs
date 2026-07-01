using Kessler.Domain.Content;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kessler.Infrastructure.Persistence.Configurations;

public sealed class SiteContentConfiguration : IEntityTypeConfiguration<SiteContent>
{
    public void Configure(EntityTypeBuilder<SiteContent> b)
    {
        b.ToTable("site_content");
        b.HasKey(c => c.Id);
        b.Property(c => c.WhatsApp).HasMaxLength(20);
        b.Property(c => c.InstagramUrl).HasMaxLength(300);
        b.Property(c => c.AboutTitle).HasMaxLength(160);
        b.Property(c => c.AboutIntro).HasMaxLength(1000);
        b.Property(c => c.AboutStoryTitle).HasMaxLength(160);
        b.Property(c => c.AboutStory).HasMaxLength(4000);

        b.HasMany(c => c.AboutPhotos)
            .WithOne()
            .HasForeignKey(p => p.SiteContentId)
            .OnDelete(DeleteBehavior.Cascade);
        b.Metadata.FindNavigation(nameof(SiteContent.AboutPhotos))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}

public sealed class AboutPhotoConfiguration : IEntityTypeConfiguration<AboutPhoto>
{
    public void Configure(EntityTypeBuilder<AboutPhoto> b)
    {
        b.ToTable("about_photos");
        b.HasKey(p => p.Id);
        b.Property(p => p.StorageKey).HasMaxLength(400).IsRequired();
        b.Property(p => p.Url).HasMaxLength(600).IsRequired();
        b.Property(p => p.Caption).HasMaxLength(200);
        b.HasIndex(p => p.SiteContentId);
    }
}
