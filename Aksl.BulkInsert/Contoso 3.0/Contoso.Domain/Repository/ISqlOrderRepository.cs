using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Aksl.Data;

using Contoso.Domain.Models;

namespace Contoso.Domain.Repository
{
    public interface ISqlOrderRepository : IRepository<Order>, IOrderRepository
    { }
}
