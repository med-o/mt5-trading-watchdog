using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TradingWatchdog.Watchdog
{
    public interface ITradingWatchdog
    {
        IEnumerable<Task> Start(CancellationToken cancellationToken);
    }
}