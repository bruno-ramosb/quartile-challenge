using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quartile.Domain.Entities;

namespace Quartile.Infrastructure.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(s => s.Email)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(s => s.Phone)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(s => s.Address)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(s => s.City)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(s => s.State)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(s => s.ZipCode)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(s => s.CreatedAt)
            .IsRequired();
            
        builder.Property(s => s.UpdatedAt);
    }
} 