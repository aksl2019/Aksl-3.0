using Microsoft.EntityFrameworkCore;

//Add-Migration InitialCreate -Contex  SqliteInsuranceContext  -StartupProject Insurance.Management.Data.Migrations.Startup
//Remove-Migration   -Contex  SqliteInsuranceContext
//Update-Database InitialCreate  -Contex  SqliteInsuranceContext

//https://docs.microsoft.com/zh-cn/ef/core/get-started/netcore/new-db-sqlite
//http://www.cnblogs.com/linezero/p/EntityFrameworkCore.html

namespace Contoso.Infrastructure.Data.Context
{
    public class SqliteContosoContext : ContosoContext
    {
        public SqliteContosoContext(DbContextOptions<ContosoContext> options) : base(options)
        {
            DataProviderType = "SQLite";
        }

        //public SqliteInsuranceContext(string connectionString) : base(connectionString)
        //{
        //}

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(_connectionString);
        //}
    }
}
