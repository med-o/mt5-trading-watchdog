using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TradingWatchdog.Monitor
{
    public interface ITradesMonitor
    {
        ConcurrentQueue<Trade> QueuedTrades { get; }
        ConcurrentQueue<Trade> ProcessedTrades { get; }
        List<FlaggedTrades> FlaggedTrades { get; }

        Task Monitor(CancellationToken token);
    }
}
