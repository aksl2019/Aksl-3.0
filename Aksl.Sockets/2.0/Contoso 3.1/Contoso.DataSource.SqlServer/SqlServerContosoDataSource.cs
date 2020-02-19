using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Contoso.DataSource.Configuration;

namespace Contoso.DataSource.SqlServer
{
    public class SqlServerContosoDataSource : IContosoDataSource
    {
        #region Members
       private readonly IServiceProvider _serviceProvider;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        #endregion

        #region Constructors
        public SqlServerContosoDataSource(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

            _logger = _loggerFactory.CreateLogger(nameof(SqlServerContosoDataSource));
            _logger.LogInformation($"{nameof(SqlServerContosoDataSource)}'s Constructor");
            //_logger.LogInformation($"{this.GetType().FullName}'s Constructor");
        }
        #endregion

        #region Properties
        public DataSourceType DataSourceType => DataSourceType.SqlServer;
        #endregion

        #region Order DataSource
        public IOrderDataSource OrderDataSource => 
                             _serviceProvider.GetRequiredService<ISqlServerOrderDataSource>();

     
        #endregion
    }
}

