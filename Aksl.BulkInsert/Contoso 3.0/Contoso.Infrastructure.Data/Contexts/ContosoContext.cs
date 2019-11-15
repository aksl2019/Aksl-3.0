using Microsoft.EntityFrameworkCore;

using Contoso.Domain.Models;
using Contoso.Infrastructure.Data.Mappings;

//Add-Migration init -Contex  ContosoContext
//Remove-Migration   -Contex  ContosoContext
//Update-Database init  -Contex  ContosoContext

namespace Contoso.Infrastructure.Data.Context
{
    public class ContosoContext : DbContext
    {
        protected string _connectionString = null;

        public ContosoContext(DbContextOptions<ContosoContext> options) : base(options)
        {
        }

        public string DataProviderType { get; set; }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new OrderMap());
            modelBuilder.ApplyConfiguration(new OrderItemMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
