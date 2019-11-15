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
        Task<IEnumerable<OrderDto>> DataflowBulkInsertOrdersAsync(IEnumerable<OrderDto> orderDtos);

        Task<IEnumerable<OrderDto>> DataflowPipeBulkInsertOrdersAsync(IEnumerable<OrderDto> orderDtos);

        Task<IEnumerable<OrderDto>> PipeBulkInsertOrdersAsync(IEnumerable<OrderDto> orderDtos);

        Task<IPagedList<OrderDto>> GetPagedOrdersAsync(int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
