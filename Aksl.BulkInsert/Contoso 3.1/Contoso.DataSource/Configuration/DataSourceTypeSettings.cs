
namespace Contoso.DataSource.Configuration
{
    public class DataSourceTypeSettings
    {
        #region Constructors
        public DataSourceTypeSettings()
        {
            DataSourceType = DataSourceType.SqlServer;
        }
        #endregion

        #region Properties
        public DataSourceType DataSourceType { get; set; }
        #endregion
    }
}
