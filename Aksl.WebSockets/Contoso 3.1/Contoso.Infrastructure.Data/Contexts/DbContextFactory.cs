using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

using Contoso.Infrastructure.Data.Configuration;

namespace Contoso.Infrastructure.Data.Context
{
    //public class RepositoryFactory : IDbContextFactory<ContosoContext>
    public class DbContextFactory : IDbContextFactory<ContosoContext>
    {
        #region Members
        private readonly IEnumerable<ContosoContext> _dbContexts;
        private readonly DataProviderSettings _dataProviderSettings;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        #endregion

        #region Constructors
        public DbContextFactory(IEnumerable<ContosoContext> dbContexts, IOptions<DataProviderSettings> dataProviderOptions, ILoggerFactory loggerFactory)
        {
            _dbContexts = dbContexts ?? throw new ArgumentNullException(nameof(dbContexts));
            _dataProviderSettings = dataProviderOptions.Value ?? new DataProviderSettings();
            DataProviderType = _dataProviderSettings.DataProviderType;

            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
            _logger = _loggerFactory.CreateLogger(nameof(DbContextFactory));
            _logger.LogInformation($"{this.GetType().FullName}'s Constructor");
        }
        #endregion

        #region Properties
        public DataProviderType DataProviderType { get; set; }
        #endregion

        #region DbContext Method
        public ContosoContext CreateDbContext()
        {
            switch (DataProviderType)
            {
                case DataProviderType.SQLite:
                    var sqliteContext = _dbContexts.FirstOrDefault(d => d.DataProviderType == "SQLite");
                    return sqliteContext;

                case DataProviderType.SQLServer:
                    var sqlServerContext = _dbContexts.FirstOrDefault(d => d.DataProviderType == "SQLServer");
                    return sqlServerContext;
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion
    }
}

