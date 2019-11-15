
using Contoso.DataSource.Configuration;

namespace Contoso.DataSource
{
    public interface IContosoDataSource
    {
        DataSourceType DataSourceType { get; }

        IOrderDataSource OrderDataSource { get; }
    }
}
