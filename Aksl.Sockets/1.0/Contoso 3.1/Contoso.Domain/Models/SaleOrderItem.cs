
namespace Contoso.Domain.Models
{
    // Define the Order Line Item
    //[Table("OrderItem")]
    public class SaleOrderItem : Aksl.Data.BaseEntity
    {
        public SaleOrderItem()
        {
        }

        #region Properties
        //[Key]
        //[Column("Id")]
        //public int Id { get; set; }

        //[Required]
        //[Column("Quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal TaxRate { get; set; }

        /// <summary>
        /// 不含税单价
        /// </summary>
        //[Required]
        //[DataType(DataType.Currency)]
        //[Column("UnitPriceIncludeTax", TypeName = "money")]
        public decimal UnitPriceExcludeTax { get; set; }

        /// <summary>
        /// 含税单价
        /// </summary>
        //[Required]
        //[DataType(DataType.Currency)]
        //[Column("UnitPriceIncludeTax", TypeName = "money")]
        public decimal UnitPriceIncludeTax => (1 + TaxRate) * UnitPriceExcludeTax;

        /// <summary>
        /// 不含税金额
        /// </summary>
        //[DataType(DataType.Currency)]
        //[Column("PriceExcludelTax", TypeName = "money")]
        //public decimal PriceExcludelTax { get; set; }
        public decimal TotalCostExcludeTax => Quantity * UnitPriceExcludeTax;

        /// <summary>
        /// 税的金额
        /// </summary>
        public decimal TotalCostTax => Quantity * TaxRate * UnitPriceExcludeTax;

        /// <summary>
        /// 含税金额
        /// </summary>
        //[DataType(DataType.Currency)]
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        //[Column("TotalCost", TypeName = "money")]
        public decimal TotalCostIncludeTax => Quantity * (1 + TaxRate) * UnitPriceExcludeTax;
        #endregion

        #region Navigation properties
        //[Column("ProductId")]
        public string ProductId { get; set; }

        //public Product Product { get; set; }
        #endregion
    }
}

