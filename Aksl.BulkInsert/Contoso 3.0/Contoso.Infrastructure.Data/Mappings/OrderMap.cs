using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Contoso.Domain.Models;

namespace Contoso.Infrastructure.Data.Mappings
{
    public class OrderMap : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable(nameof(Order));

            builder.HasKey(o =>o.Id);

            //在添加或更新时生成值
            //builder.Property(o => o.TotalExcludeCost)
            //       .HasColumnType("decimal(18,4)")
            //       .ValueGeneratedOnAddOrUpdate();

            builder.Ignore(o => o.TotalCostExcludeTax);
            builder.Ignore(o => o.TotalCostIncludeTax);

            //Unique约束
           // builder.HasAlternateKey(o => o.OrderNumber);

            //builder.Property(o => o.Status)
            //     .HasMaxLength(16);

            //枚举转化
            builder.Property(o => o.Status)
                   .HasConversion<string>()
                   .HasMaxLength(16);

            //缺省日期
            builder.Property(o => o.CreatedOnUtc)
                   .HasColumnType("datetime2")
                   .HasDefaultValueSql("getdate()");

            builder.Property(o => o.CustomerName)
                   .HasMaxLength(128)
                   .IsRequired();

            //一个导航属性
            builder.HasMany(o => o.OrderItems)
                   .WithOne();

            //builder.HasOne(o => o.Customer)
            //       .WithMany()
            //       .HasForeignKey(o => o.CustomerId);
        }
    }
}
