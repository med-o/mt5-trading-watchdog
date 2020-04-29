using TradingWatchdog.Monitor;

namespace TradingWatchdog.Logging
{
    public interface ILogger
    {
        void Log(string message);
        void Log(FlaggedTrades flagged);
    }
}
