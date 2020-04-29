using System.Collections.Specialized;
using System.Configuration;

namespace TradingWatchdog.Settings
{
    public class ValidationSettingsAppConfigRepository : IValidationSettingsRepository
    {
        public virtual ValidationSettings Get()
        {
            var connectionSettings = ConfigurationManager.GetSection("Validation") as NameValueCollection;
            int.TryParse(connectionSettings["OpenTimeDelta"], out var timeDelta);
            decimal.TryParse(connectionSettings["VolumeToBalanceRatio"], out var balanceRatio);

            var settings = new ValidationSettings()
            {
                OpenTimeDelta = timeDelta,
                VolumeToBalanceRatio = balanceRatio,
            };

            return settings;
        }
    }
}
