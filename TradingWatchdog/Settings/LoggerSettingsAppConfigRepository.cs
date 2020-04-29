using System;
using System.Collections.Specialized;
using System.Configuration;

namespace TradingWatchdog.Settings
{
    public class LoggerSettingsAppConfigRepository  : ILoggerSettingsRepository
    {
        public LoggerSettings Get()
        {
            var connectionSettings = ConfigurationManager.GetSection("Logging") as NameValueCollection;
            var path = connectionSettings["OutputPath"];
            var filename = string.Format(
                connectionSettings["OutputFileName"],
                DateTime.Now.ToString(connectionSettings["OutputFileNameTimeStampFormat"])
            );

            var settings = new LoggerSettings()
            {
                OutputFileName = filename,
                OutputPath = path,
            };

            return settings;
        }
    }
}
