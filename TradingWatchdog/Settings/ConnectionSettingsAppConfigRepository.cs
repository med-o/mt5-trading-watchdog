using System.Collections.Specialized;
using System.Configuration;
using System.Linq;

namespace TradingWatchdog.Settings
{
    public class ConnectionSettingsAppConfigRepository : IConnectionSettingsRepository
    {
        public ConnectionSettings Get()
        {
            var connectionSettings = ConfigurationManager.GetSection("Connection") as NameValueCollection;
            var serverList = connectionSettings["Servers"]?.Split(' ').ToList();
            ulong.TryParse(connectionSettings["Login"], out var login);
            var passwd = connectionSettings["Password"];
            int.TryParse(connectionSettings["RetryCount"], out var retryCount);

            var settings = new ConnectionSettings()
            {
                Servers = serverList,
                Login = login,
                Password = passwd,
                RetryCount = retryCount,
            };

            return settings;
        }
    }
}
