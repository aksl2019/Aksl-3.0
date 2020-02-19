using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Aksl.Data;

using Contoso.Infrastructure.Data.Context;
using Contoso.Domain.Models;
using System.Linq;
using System.Collections.Generic;

namespace Contoso.ConsoleApp
{
    public partial class WebApiSender
    {
        #region BulkCopy Method
        public async Task BulkCopyAsync()
        {
            var logger = _loggerFactory.CreateLogger($"Bulk Copyer");

            try
            {
                var repositoryFactory = this.ServiceProvider.GetRequiredService<IDbContextFactory<ContosoContext>>();
                var dbContext = repositoryFactory.CreateDbContext();

               // var isFullText = await dbContext.IsFullTextSupportedAsync();

                //var orders = OrderJsonProvider.CreateOrders(0, 1000).ToList();

                //await dbContext.ExecuteSqlBulkCopyAync<SaleOrder>(new List<SaleOrder>());
            }
            catch (Exception ex)
            {
                logger.LogError($"Error while listening: {ex.Message}");
            }
        }
        #endregion
    }
}
