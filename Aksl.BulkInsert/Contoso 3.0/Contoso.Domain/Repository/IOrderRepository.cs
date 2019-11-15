using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Aksl.Data;

using Contoso.Domain.Models;

namespace Contoso.Domain.Repository
{
    public interface IOrderRepository 
    {
        Task<IEnumerable<Order>> DataflowBulkInsertOrdersAsync(IEnumerable<Order> orders);

        Task<IEnumerable<Order>> DataflowPipeBulkInsertOrdersAsync(IEnumerable<Order> orders);

        Task<IEnumerable<Order>> PipeBulkInsertOrdersAsync(IEnumerable<Order> orders);

        Task<IPagedList<Order>> GetPagedOrdersAsync(int pageIndex = 0, int pageSize = int.MaxValue);
    }
}
