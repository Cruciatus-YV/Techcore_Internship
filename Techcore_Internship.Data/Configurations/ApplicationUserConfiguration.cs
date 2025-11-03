using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Techcore_Internship.Domain.Entities;

namespace Techcore_Internship.Data.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUserEntity>
    {
        public void Configure(EntityTypeBuilder<ApplicationUserEntity> builder)
        {
            builder.ToTable("AspNetUsers");

            builder.Property(u => u.DateOfBirth)
                .IsRequired(false)
                .HasColumnType("date");
        }
    }
}