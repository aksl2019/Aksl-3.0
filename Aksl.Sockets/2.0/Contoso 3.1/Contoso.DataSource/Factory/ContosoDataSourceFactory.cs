using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Options;

using Contoso.DataSource.Configuration;

namespace Contoso.DataSource                 
{
    public class ContosoDataSourceFactory : IDataSourceFactory<IContosoDataSource>
    {
        #region Members
        private readonly IServiceProvider _serviceProvider;
        private readonly DataSourceTypeSettings _dataSourceTypeSettings;
        private readonly IEnumerable<IContosoDataSource> _dataSources;
        #endregion

        #region Constructors
        public ContosoDataSourceFactory(IServiceProvider serviceProvider, IOptions<DataSourceTypeSettings> dataSourceTypeSettingsOptions, IEnumerable<IContosoDataSource> dataSources)
        {
            _serviceProvider = serviceProvider;
            _dataSourceTypeSettings = dataSourceTypeSettingsOptions.Value ?? throw new ArgumentNullException(nameof(dataSourceTypeSettingsOptions));
            _dataSources =dataSources ?? throw new ArgumentNullException(nameof(dataSources));

            DataSourceType = _dataSourceTypeSettings.DataSourceType;
            Current = _dataSources.FirstOrDefault(d => d.DataSourceType == _dataSourceType) ?? throw new ArgumentNullException(nameof(Current));
        }

        #endregion

        #region Properties
        public IContosoDataSource Current { get; private set; }

        private DataSourceType _dataSourceType= DataSourceType.SqlServer;
        public DataSourceType DataSourceType
        {
            get => _dataSourceType;
            set
            {
                if (_dataSourceType != value)
                {
                    _dataSourceType = value;

                    Current = _dataSources.FirstOrDefault(d => d.DataSourceType== _dataSourceType) ?? throw new ArgumentNullException(nameof(Current));
                }
            }
        }
        #endregion

        #region DataSource Methods
        public void CreateDataSource(DataSourceType dataSourceType)
        {
            Current = _dataSources.FirstOrDefault<IContosoDataSource>(d => d.DataSourceType == dataSourceType) ?? throw new ArgumentNullException(nameof(Current));
        }

        public void UseSql() =>
                           CreateDataSource(DataSourceType.SqlServer);

        public void UseRest() =>
                           CreateDataSource(DataSourceType.WebAPI);
        #endregion
    }
}

