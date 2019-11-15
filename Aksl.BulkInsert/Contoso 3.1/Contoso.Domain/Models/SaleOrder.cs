using System;
using System.Collections.Generic;
using System.Linq;

namespace Contoso.Domain.Models
{
    // [Table("Order")]
    public class SaleOrder : Aksl.Data.BaseEntity
    {
        public SaleOrder()
        {
            OrderItems = new HashSet<SaleOrderItem>();
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
        public OrderStatus Status { get; set; }

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

        public ICollection<SaleOrderItem> OrderItems { get; set; }
        #endregion
    }
}
