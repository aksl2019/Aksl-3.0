using System.Collections.Generic;
using System.Threading.Tasks;

using Contoso.Modules.Customer.Models;

namespace Contoso.Modules.Customer.Service
{
    public class CustomerService : ICustomerService
    {
        private List<CustomerDto> _customers;

        public CustomerService()
        {
            _customers = new List<CustomerDto>()
            {
                new CustomerDto{ Id=1, FirstName="Josh", LastName="Smith", IsCompany=false, Email="josh@contoso.com", TotalSales=32132.9d },
                new CustomerDto{ Id=2,FirstName="Greg", LastName="Bujak", IsCompany=false, Email="greg@contoso.com", TotalSales=9848.06d },

                new CustomerDto{Id=3, FirstName="Alfreds Futterkiste", LastName="", IsCompany=true, Email="maria@contoso.com", TotalSales=8832.16d },
                new CustomerDto{ Id=4,FirstName="Eastern Connection", LastName="", IsCompany=true, Email="ann@contoso.com", TotalSales=12831.73d },
            };
        }

        public Task AddCustomerAsync(CustomerDto customer)
        {
            return Task.CompletedTask;
        }

        public Task<List<CustomerDto>> GetCustomersAsync()
        {
            return Task.FromResult(_customers);
        }
    }
}
 