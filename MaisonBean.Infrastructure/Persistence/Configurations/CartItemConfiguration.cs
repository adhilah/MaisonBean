using MaisonBean.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaisonBean.Infrastructure.Persistence.Configurations;

public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
{
    public void Configure(EntityTypeBuilder<CartItem> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Ignore(c => c.TotalPrice);

        builder.Property(c => c.UnitPrice).HasPrecision(18, 2);
        builder.Property(c => c.ProductName).HasMaxLength(200).IsRequired();
        builder.Property(c => c.ProductImage).HasMaxLength(500);
        builder.Property(c => c.ProductCategory).HasMaxLength(100);

        builder.HasOne(c => c.User)
               .WithMany()
               .HasForeignKey(c => c.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Product)
               .WithMany()
               .HasForeignKey(c => c.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Bean)
               .WithMany()
               .HasForeignKey(c => c.BeanId)
               .OnDelete(DeleteBehavior.SetNull)
               .IsRequired(false);

        builder.HasOne(c => c.Milk)
               .WithMany()
               .HasForeignKey(c => c.MilkId)
               .OnDelete(DeleteBehavior.SetNull)
               .IsRequired(false);

        builder.HasIndex(c => new { c.UserId, c.ProductId, c.BeanId, c.MilkId })
               .IsUnique();
    }
}