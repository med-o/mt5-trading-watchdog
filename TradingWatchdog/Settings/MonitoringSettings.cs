namespace TradingWatchdog.Settings
{
    public class MonitoringSettings
    {
        /// <summary>
        /// Trades retention time in seconds in the monitor's processed trades queue
        /// </summary>
        public int RetentionPeriod { get; set; }
    }
}
