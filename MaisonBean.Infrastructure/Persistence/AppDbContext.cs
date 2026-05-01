using MaisonBean.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MaisonBean.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // DB SETS
    public DbSet<Product> Products { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<BeanType> BeanTypes { get; set; }
    public DbSet<MilkOption> MilkOptions { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        builder.Entity<BeanType>()
            .Property(b => b.PriceAdd)
            .HasPrecision(18, 2);

        builder.Entity<MilkOption>()
            .Property(m => m.PriceAdd)
            .HasPrecision(18, 2);

        builder.Entity<Order>()
            .Property(o => o.Subtotal)
            .HasPrecision(18, 2);

        builder.Entity<Order>()
            .Property(o => o.Shipping)
            .HasPrecision(18, 2);

        builder.Entity<Order>()
            .Property(o => o.Total)
            .HasPrecision(18, 2);

        builder.Entity<OrderItem>()
            .Property(o => o.UnitPrice)
            .HasPrecision(18, 2);

        builder.Entity<OrderItem>()
            .Property(o => o.BeanPriceAdd)
            .HasPrecision(18, 2);

        builder.Entity<OrderItem>()
            .Property(o => o.MilkPriceAdd)
            .HasPrecision(18, 2);

        builder.Entity<WishlistItem>(entity =>
        {
            entity.HasKey(w => w.Id);

            entity.Property(w => w.UserId)
                  .IsRequired();

            entity.Property(w => w.ProductId)
                  .IsRequired();

            entity.HasOne(w => w.Product)
                  .WithMany()
                  .HasForeignKey(w => w.ProductId);

            // Prevent duplicate wishlist items
            entity.HasIndex(w => new { w.UserId, w.ProductId })
                  .IsUnique();
        });
    }
}