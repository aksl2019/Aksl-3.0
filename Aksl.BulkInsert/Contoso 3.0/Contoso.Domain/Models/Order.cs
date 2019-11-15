using System;
using System.Collections.Generic;
using System.Linq;

namespace Contoso.Domain.Models
{
    public enum OrderState
    {
        Pending,
        Processed,
        Shipped
    }

    // [Table("Order")]
    public class Order : Aksl.Data.BaseEntity
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        #region Properties
        //[Key]
        //[Column("Id")]
        //public int Id { get; set; }

        //[Required]
        //[Column("OrderNumber")]
        public int OrderNumber { get; set; }

        //[Required]
        //[Column("Status", TypeName = "nvarchar(12)")]
        public OrderState Status { get; set; }

        //[Required]
        //[DataType(DataType.DateTime)]
        //[Column("CreatedOnUtc")]
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// 不含税总金额
        /// </summary>
        public decimal TotalCostExcludeTax => OrderItems.Sum(oi => oi.TotalCostExcludeTax);

        /// <summary>
        /// 税的总金额
        /// </summary>
        public decimal TotalCostTax => OrderItems.Sum(oi => oi.TotalCostTax);

        //[DataType(DataType.Currency)]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //[Column("TotalCost", TypeName = "money", Order = 6)]
        /// <summary>
        /// 含税总金额
        /// </summary>
        public decimal TotalCostIncludeTax => OrderItems.Sum(oi => oi.TotalCostIncludeTax);
        #endregion

        #region Navigation properties
        public string CustomerName { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
        #endregion
    }
}
