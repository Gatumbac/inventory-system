using Inventory.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public class ProductSupplierConfiguration : IEntityTypeConfiguration<ProductSupplier>
{
    public void Configure(EntityTypeBuilder<ProductSupplier> builder)
    {
        builder.Property(ps => ps.LotNumber).HasMaxLength(50);
        builder.Property(ps => ps.Price).HasPrecision(18, 2);

        builder.HasIndex(ps => new { ps.ProductId, ps.SupplierId, ps.LotNumber }).IsUnique().HasFilter("[DeletedAt] IS NULL");
        builder.HasQueryFilter(ps => ps.DeletedAt == null);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_ProductSuppliers_Price_NonNegative", "[Price] >= 0");
            t.HasCheckConstraint("CK_ProductSuppliers_Stock_NonNegative", "[Stock] >= 0");
        });

        builder.HasOne(ps => ps.Product)
            .WithMany(p => p.ProductSuppliers)
            .HasForeignKey(ps => ps.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ps => ps.Supplier)
            .WithMany(s => s.ProductSuppliers)
            .HasForeignKey(ps => ps.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
