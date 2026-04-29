using MaisonBean.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MaisonBean.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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
        builder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        // =========================
        builder.Entity<BeanType>()
            .Property(b => b.PriceAdd)
            .HasPrecision(18, 2);

        //builder.Entity<OrderItem>()
        //    .Property(o => o.BasePrice)
        //    .HasPrecision(18, 2);

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
        });
    }
}