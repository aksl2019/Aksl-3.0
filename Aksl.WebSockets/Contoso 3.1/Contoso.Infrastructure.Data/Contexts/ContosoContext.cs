using Microsoft.EntityFrameworkCore;

using Contoso.Domain.Models;
using Contoso.Infrastructure.Data.Mappings;

//Drop-Database

//Add-Migration init -Contex  ContosoContext
//Remove-Migration   -Contex  ContosoContext
//Update-Database init  -Contex  ContosoContext

//Add-Migration Initial -Contex  ContosoContext
//Remove-Migration   -Contex  ContosoContext
//Update-Database Initial  -Contex  ContosoContext

//Add-Migration RowVersion -Contex  ContosoContext
//Remove-Migration -Contex  ContosoContext
//Update-Database RowVersion  -Contex  ContosoContext

namespace Contoso.Infrastructure.Data.Context
{
    public class ContosoContext : DbContext
    {
        protected string _connectionString = null;

        public ContosoContext(DbContextOptions<ContosoContext> options) : base(options)
        {
        }

        public string DataProviderType { get; set; }

        public DbSet<SaleOrder> SaleOrders { get; set; }

        public DbSet<SaleOrderItem> SaleOrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new SaleOrderMap());
            modelBuilder.ApplyConfiguration(new SaleOrderItemMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
