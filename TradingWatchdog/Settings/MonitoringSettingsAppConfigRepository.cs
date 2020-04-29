using System.Collections.Specialized;
using System.Configuration;

namespace TradingWatchdog.Settings
{
    public class MonitoringSettingsAppConfigRepository : IMonitoringSettingsRepository
    {
        public virtual MonitoringSettings Get()
        {
            var connectionSettings = ConfigurationManager.GetSection("Monitoring") as NameValueCollection;
            int.TryParse(connectionSettings["RetentionPeriod"], out var retentionPeriod);

            var settings = new MonitoringSettings()
            {
                RetentionPeriod = retentionPeriod
            };

            return settings;
        }
    }
}
