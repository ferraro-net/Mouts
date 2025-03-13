using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(f => f.Product).IsRequired().HasMaxLength(50);
        builder.Property(f => f.Quantity).IsRequired();
        builder.Property(f => f.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(f => f.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(f => f.Discount).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(f => f.CreatedAt).IsRequired();

        builder.HasOne(si => si.Sale)
            .WithMany(s => s.Items)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
