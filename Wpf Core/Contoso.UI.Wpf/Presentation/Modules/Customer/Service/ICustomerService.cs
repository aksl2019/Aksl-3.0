using System.Collections.Generic;
using System.Threading.Tasks;

using Contoso.Modules.Customer.Models;

namespace Contoso.Modules.Customer.Service
{
    public interface ICustomerService
    {
        Task<List<CustomerDto>> GetCustomersAsync();

        Task AddCustomerAsync(CustomerDto customer);
    }
}
 