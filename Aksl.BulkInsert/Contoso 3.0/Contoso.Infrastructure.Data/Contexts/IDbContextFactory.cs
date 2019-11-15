using Microsoft.EntityFrameworkCore;

using Contoso.Infrastructure.Data.Configuration;

namespace Contoso.Infrastructure.Data.Context
{
    // public interface IRepositoryFactory<DBContext> where DBContext : DbContext
    public interface IDbContextFactory<TDbContext>  where TDbContext : DbContext
    {
        #region Properties
        DataProviderType DataProviderType { get; set; }
        #endregion

        TDbContext CreateDbContext();
    }
}
