using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Contoso.Domain.Models;

namespace Contoso.Infrastructure.Data.Mappings
{
    public class SaleOrderItemMap : IEntityTypeConfiguration<SaleOrderItem>
    {
        public void Configure(EntityTypeBuilder<SaleOrderItem> builder)
        {
            builder.ToTable(nameof(SaleOrderItem));

            builder.HasKey(soi => soi.Id);

            builder.Property(soi => soi.Quantity)
                   .IsRequired();

            builder.Property(soi => soi.TaxRate)
                   .HasColumnType("decimal(18,4)")
                   .IsRequired();

            builder.Property(oi => oi.UnitPriceIncludeTax)
                   .HasColumnType("money")
                   .IsRequired();

            builder.Property(soi => soi.UnitPriceExcludeTax)
                   .HasColumnType("money")
                   .IsRequired();

            builder.Property(soi => soi.TotalCostExcludeTax)
                   .HasColumnType("money")
                   .ValueGeneratedOnAddOrUpdate();

            builder.Property(soi => soi.TotalCostIncludeTax)
                   .HasColumnType("money")
                   .ValueGeneratedOnAddOrUpdate();

            builder.Ignore(soi => soi.UnitPriceIncludeTax);
            builder.Ignore(soi => soi.TotalCostExcludeTax);
            builder.Ignore(soi => soi.TotalCostIncludeTax);

            builder.Property(soi => soi.ProductId)
                   .HasMaxLength(128)
                   .IsRequired();

            //builder.HasOne(oi => oi.Product)
            //       .WithMany()
            //       .HasForeignKey(oi => oi.ProductId)
            //       .IsRequired();
        }
    }
}
