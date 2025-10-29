using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<BookEntity>
{
    public void Configure(EntityTypeBuilder<BookEntity> builder)
    {
        builder.ToTable("Books");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.Year)
            .IsRequired();

        builder.Property(b => b.IsDeleted)
            .IsRequired();

        builder.HasMany(a => a.Authors)
            .WithMany(b => b.Books);
    }
}