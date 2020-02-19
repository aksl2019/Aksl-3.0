
namespace Contoso.Infrastructure.Data.Configuration
{
    public class DataProviderSettings
    {
        #region Constructors
        public DataProviderSettings()
        {
            DataProviderType = DataProviderType.SQLServer;
        }
        #endregion

        #region Properties
        public DataProviderType DataProviderType { get; set; }
        #endregion
    }
}
