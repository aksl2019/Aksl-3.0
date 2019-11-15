using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Contoso.Domain.Models;

namespace Contoso.Infrastructure.Data.Mappings
{
    public class OrderItemMap : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable(nameof(OrderItem));

            builder.HasKey(c => c.Id);

            builder.Property(oi => oi.Quantity)
                   .IsRequired();

            builder.Property(oi => oi.TaxRate)
                   .HasColumnType("decimal(18,4)")
                   .IsRequired();

            builder.Property(oi => oi.UnitPriceIncludeTax)
                   .HasColumnType("money")
                   .IsRequired();

            builder.Property(oi => oi.UnitPriceExcludeTax)
                   .HasColumnType("money")
                   .IsRequired();

            builder.Property(oi => oi.TotalCostExcludeTax)
                   .HasColumnType("money")
                   .ValueGeneratedOnAddOrUpdate();

            builder.Property(oi => oi.TotalCostIncludeTax)
                   .HasColumnType("money")
                   .ValueGeneratedOnAddOrUpdate();

            builder.Ignore(oi => oi.UnitPriceIncludeTax);
            builder.Ignore(oi => oi.TotalCostExcludeTax);
            builder.Ignore(oi => oi.TotalCostIncludeTax);

            builder.Property(oi => oi.ProductId)
                   .HasMaxLength(128)
                   .IsRequired();

            //builder.HasOne(oi => oi.Product)
            //       .WithMany()
            //       .HasForeignKey(oi => oi.ProductId)
            //       .IsRequired();
        }
    }
}
