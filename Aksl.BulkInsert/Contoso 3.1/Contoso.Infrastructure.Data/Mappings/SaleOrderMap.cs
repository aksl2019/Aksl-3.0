using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Contoso.Domain.Models;

namespace Contoso.Infrastructure.Data.Mappings
{
    public class SaleOrderMap : IEntityTypeConfiguration<SaleOrder>
    {
        public void Configure(EntityTypeBuilder<SaleOrder> builder)
        {
            builder.ToTable(nameof(SaleOrder));

            builder.HasKey(so =>so.Id);

            //在添加或更新时生成值
            //builder.Property(o => o.TotalExcludeCost)
            //       .HasColumnType("decimal(18,4)")
            //       .ValueGeneratedOnAddOrUpdate();

            builder.Ignore(so => so.TotalCostExcludeTax);
            builder.Ignore(so => so.TotalCostTax);
            builder.Ignore(so => so.TotalCostIncludeTax);

            //Unique约束
            // builder.HasAlternateKey(o => o.OrderNumber);

            //builder.Property(o => o.Status)
            //     .HasMaxLength(16);

            //枚举转化
            builder.Property(so => so.Status)
                   .HasConversion<string>()
                   .HasMaxLength(16);

            //缺省日期
            builder.Property(so => so.CreatedOnUtc)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("getdate()");

            builder.Property(so => so.CustomerName)
                   .HasMaxLength(128)
                   .IsRequired();

            builder.Property(so=> (so.RowVersion))
                   .IsRowVersion()
                   .HasColumnName("RowVersion"); 

            //https://docs.microsoft.com/zh-cn/ef/core/modeling/relationships
            //单个导航属性
            builder.HasMany(o => o.OrderItems)
                   .WithOne();

            //builder.HasOne(o => o.Customer)
            //       .WithMany()
            //       .HasForeignKey(o => o.CustomerId);
        }
    }
}
