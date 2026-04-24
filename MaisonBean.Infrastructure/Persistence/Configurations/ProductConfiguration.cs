using MaisonBean.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaisonBean.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(1000);
        builder.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.StockQuantity).IsRequired();
        builder.Property(p => p.IsActive).HasDefaultValue(true);
        builder.Property(p => p.Category).HasMaxLength(100);
        builder.Property(p => p.Image).HasMaxLength(500);
        builder.Property(p => p.BaseCalories).IsRequired();
        builder.Property(p => p.HealthBenefits).HasMaxLength(1000);
    }
}