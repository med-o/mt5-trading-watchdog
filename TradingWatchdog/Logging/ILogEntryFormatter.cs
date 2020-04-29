using TradingWatchdog.Monitor;

namespace TradingWatchdog.Logging
{
    public interface ILogEntryFormatter
    {
        string Format(string message);
        string Format(FlaggedTrades flagged);
    }
}
