using System.Threading;

namespace TradingWatchdog.ServerListener
{
    public interface ITradingServerListener
    {
        void Listen(CancellationToken token);
    }
}
