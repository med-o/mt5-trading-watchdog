using MT5Wrapper;

namespace TradingWatchdog.ServerListener
{
    public interface ITradingServerListenerFactory
    {
        ITradingServerListener Create(ConnectionParams connection);
    }
}
