
namespace Contoso.Modules.Customer.Models
{
    public class CustomerDto
    {
        public CustomerDto()
        {
        }

        public int Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        /// </summary>
        public bool IsCompany { get; set; }

        /// </summary>
        public string LastName { get; set; }

        public double TotalSales { get;  set; }
    }
}
