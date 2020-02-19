using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Aksl.Data;

using Contoso.DataSource.Dtos;

namespace Contoso.DataSource
{
    public interface IOrderDataSource
    {
        //Task<IEnumerable<SaleOrderDto>> DataflowBulkInsertOrdersAsync(IEnumerable<SaleOrderDto> orderDtos);

        //Task<IEnumerable<SaleOrderDto>> DataflowPipeBulkInsertOrdersAsync(IEnumerable<SaleOrderDto> orderDtos);

        //Task<IEnumerable<SaleOrderDto>> PipeBulkInsertOrdersAsync(IEnumerable<SaleOrderDto> orderDtos);

        //Task<IPagedList<SaleOrderDto>> GetPagedOrdersAsync(int pageIndex = 0, int pageSize = int.MaxValue);

        ValueTask<IEnumerable<SaleOrderDto>> DataflowBulkInsertSaleOrdersAsync(IEnumerable<SaleOrderDto> saleOrderDtos);

        ValueTask<IEnumerable<SaleOrderDto>> DataflowPipeBulkInsertSaleOrdersAsync(IEnumerable<SaleOrderDto> saleOrderDtos);

        ValueTask<IEnumerable<SaleOrderDto>> PipeBulkInsertSaleOrdersAsync(IEnumerable<SaleOrderDto> saleOrderDtos);

        ValueTask UpdateSaleOrdersAsync(IEnumerable<SaleOrderDto> saleOrderDtos);

        ValueTask DeleteSaleOrdersAsync(IEnumerable<SaleOrderDto> saleOrders);

        IAsyncEnumerable<SaleOrderDto> GetPagedSaleOrderListAsync(int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
