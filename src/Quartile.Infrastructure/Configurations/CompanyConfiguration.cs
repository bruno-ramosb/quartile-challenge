using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quartile.Domain.Entities;

namespace Quartile.Infrastructure.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.DocumentNumber)
                .IsRequired()
                .HasMaxLength(14);

            builder.Property(x => x.DocumentType)
                .IsRequired();

            builder.HasIndex(x => x.DocumentNumber)
                .IsUnique();

            builder.HasIndex(x => x.Name);
        }
    }
} 