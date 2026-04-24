using MaisonBean.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MaisonBean.Infrastructure.Persistence.Configurations;

public class MilkOptionConfiguration : IEntityTypeConfiguration<MilkOption>
{
    public void Configure(EntityTypeBuilder<MilkOption> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).IsRequired();
        builder.Property(m => m.PriceAdd).HasColumnType("decimal(18,2)");
        builder.Property(m => m.Calories);
    }
}