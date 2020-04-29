namespace TradingWatchdog.Settings
{
    public interface ISettingsRepository<T>
    {
        T Get();
    }
}
