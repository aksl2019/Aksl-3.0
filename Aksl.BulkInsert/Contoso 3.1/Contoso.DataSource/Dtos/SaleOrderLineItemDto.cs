using System;
using System.Text;

namespace Contoso.DataSource.Dtos
{
    public class SaleOrderLineItemDto
    {
        //public OrderLineItemDto()
        //{
        //}

        public int Id { get; set; }

        public int Quantity { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal TaxRate { get; set; }

        /// <summary>
        /// 不含税单价
        /// </summary>
        public decimal UnitPriceExcludeTax { get; set; }

        /// <summary>
        /// 含税单价
        /// </summary>
        public decimal UnitPriceIncludeTax => (1 + TaxRate) * UnitPriceExcludeTax;

        /// <summary>
        /// 不含税金额
        /// </summary>
        public decimal TotalCostExcludeTax => Quantity * UnitPriceExcludeTax;

        /// <summary>
        /// 税的金额
        /// </summary>
        public decimal TotalCostTax => Quantity * TaxRate * UnitPriceExcludeTax;

        /// <summary>
        /// 含税金额
        /// </summary>
        public decimal TotalCostIncludeTax => Quantity * (1 + TaxRate) * UnitPriceExcludeTax;

        public string ProductId { get; set; }

        public override string ToString()
        {
            StringBuilder strbuf = new StringBuilder($"OrderItemId: {Id}\n");
            strbuf.Append($"\t\tProductId: {ProductId}\n");
            strbuf.Append($"\t\tQuantity: {Quantity}\n");
            strbuf.Append($"\t\tTaxRate: {TaxRate}\n");
            strbuf.Append($"\t\tUnitPriceExcludeTax: {UnitPriceExcludeTax}\n");
            strbuf.Append($"\t\tUnitPriceIncludeTax: {UnitPriceIncludeTax}\n");
            strbuf.Append($"\t\tTotalCostExcludeTax: {TotalCostExcludeTax}\n");
            strbuf.Append($"\t\tTotalCostTax: {TotalCostTax}\n");
            strbuf.Append($"\t\tTotalCostIncludeTax: {TotalCostIncludeTax}\n");

            return strbuf.ToString();
            //  return $"Order LineItem: {Quantity} of {ProductId} @unit price: ${TotalCostIncludeTax}\n";
        }
    }
}
