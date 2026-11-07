using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Infrastructure.Persistence.Configurations;

public class UrlConfiguration : IEntityTypeConfiguration<Url>
{
    public void Configure(EntityTypeBuilder<Url> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.OriginalUrl)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(u => u.ShortenedCode)
            .IsRequired()
            .HasMaxLength(15);

        builder.HasIndex(u => u.ShortenedCode)
            .IsUnique();
    }
}
