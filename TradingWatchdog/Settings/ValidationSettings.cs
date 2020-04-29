namespace TradingWatchdog.Settings
{
    public class ValidationSettings
    {
        /// <summary>
        /// Trade opening time delta in seconds
        /// </summary>
        public int OpenTimeDelta { get; set; }

        /// <summary>
        /// Volume to balance ratio as decimal (not %)
        /// </summary>
        public decimal VolumeToBalanceRatio { get; set; }
    }
}
