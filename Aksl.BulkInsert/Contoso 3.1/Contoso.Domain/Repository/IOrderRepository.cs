using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Aksl.Data;

using Contoso.Domain.Models;

namespace Contoso.Domain.Repository
{
    public interface IOrderRepository
    {
        //Task<IEnumerable<SaleOrder>> DataflowBulkInsertOrdersAsync(IEnumerable<SaleOrder> orders);

        ValueTask<IEnumerable<SaleOrder>> DataflowBulkInsertSaleOrdersAsync(IEnumerable<SaleOrder> saleOrders);

        // Task<IEnumerable<SaleOrder>> DataflowPipeBulkInsertOrdersAsync(IEnumerable<SaleOrder> orders);
        ValueTask<IEnumerable<SaleOrder>> DataflowPipeBulkInsertSaleOrdersAsync(IEnumerable<SaleOrder> saleOrders);

        //Task<IEnumerable<SaleOrder>> PipeBulkInsertOrdersAsync(IEnumerable<SaleOrder> orders);
        ValueTask<IEnumerable<SaleOrder>> PipeBulkInsertSaleOrdersAsync(IEnumerable<SaleOrder> saleOrders);

        ValueTask DeleteSaleOrdersAsync(IEnumerable<SaleOrder> saleOrders);

        ValueTask UpdateSaleOrdersAsync(IEnumerable<SaleOrder> saleOrders);

        // Task<IPagedList<SaleOrder>> GetPagedOrdersAsync(int pageIndex = 0, int pageSize = int.MaxValue);
        //ValueTask<IPagedList<SaleOrder>> GetPagedSaleOrdersAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        ValueTask<(IQueryable<SaleOrder> PagedQuery, int TotalCount)> GetPagedSaleOrdersAsync(int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
