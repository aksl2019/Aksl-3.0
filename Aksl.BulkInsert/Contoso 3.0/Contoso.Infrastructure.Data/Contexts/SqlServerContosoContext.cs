using Microsoft.EntityFrameworkCore;

//Add-Migration InitialCreate -Contex  SqlServerInsuranceContext 
//Remove-Migration   -Contex  SqlServerInsuranceContext 
//Update-Database InitialCreate  -Contex  SqlServerInsuranceContext 

//https://docs.microsoft.com/zh-cn/ef/core/get-started/netcore/new-db-sqlite
//http://www.cnblogs.com/linezero/p/EntityFrameworkCore.html

namespace Contoso.Infrastructure.Data.Context
{
    public class SqlServerContosoContext : ContosoContext
    {
        public SqlServerContosoContext(DbContextOptions<ContosoContext> options) : base(options)
        {
            DataProviderType = "SQLServer";
        }

        //public SqlServerInsuranceContext(string connectionString) : base(connectionString)
        //{
        //}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    //枚举得转化和缺省值
        //    //modelBuilder.Entity<Product>()
        //    //            .Property(p => p.Status)
        //    //            .HasConversion<string>()
        //    //            .HasDefaultValue("Processing");

        //    //缺省日期
        //    //modelBuilder.Entity<Order>()
        //    //            .Property(po => po.CreatedDate)
        //    //            .HasDefaultValueSql("getdate()");

        //    base.OnModelCreating(modelBuilder);
        //}

    }
}
