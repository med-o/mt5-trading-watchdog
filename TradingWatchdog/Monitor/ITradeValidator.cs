namespace TradingWatchdog.Monitor
{
    public interface ITradeValidator
    {
        bool IsSuspicious(Trade x, Trade y);
    }
}