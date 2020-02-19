
using Contoso.DataSource.Configuration;

namespace Contoso.DataSource
{
    public interface IDataSourceFactory<TDataSource>
    {
        #region Properties
        DataSourceType DataSourceType { get; set; }

        TDataSource Current { get; }
        #endregion
    }
}
