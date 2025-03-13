using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(f => f.SaleNumber).IsRequired().HasMaxLength(50);
        builder.Property(f => f.Customer).IsRequired().HasMaxLength(50);
        builder.Property(f => f.Branch).IsRequired().HasMaxLength(50);
        builder.Property(f => f.CreatedAt).IsRequired();
        builder.Property(f => f.UpdatedAt).IsRequired(false);

        builder.Property(f => f.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasMany(f => f.Items)
            .WithOne(si => si.Sale)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(f => new { f.SaleNumber, f.Branch }).IsUnique();
    }
}
