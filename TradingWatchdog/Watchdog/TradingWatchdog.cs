using MT5Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradingWatchdog.Monitor;
using TradingWatchdog.ServerListener;
using TradingWatchdog.Settings;

namespace TradingWatchdog.Watchdog
{
    public class TradingWatchdog : ITradingWatchdog
    {
        private readonly IConnectionSettingsRepository _connectionSettings;
        private readonly ITradingServerListenerFactory _serverListenerFactory;
        private readonly ITradesMonitor _tradesMonitor;

        public TradingWatchdog(
            IConnectionSettingsRepository connectionSettings,
            ITradingServerListenerFactory serverListenerFactory,
            ITradesMonitor tradesMonitor
        )
        {
            _connectionSettings = connectionSettings;
            _serverListenerFactory = serverListenerFactory;
            _tradesMonitor = tradesMonitor;
        }

        public IEnumerable<Task> Start(CancellationToken cancellationToken)
        {
            var tasks = new List<Task>();

            var monitoringTask = Task.Run(() => _tradesMonitor.Monitor(cancellationToken));
            tasks.Add(monitoringTask);

            var dealListeningTasks = _connectionSettings
                .Get()
                .GetConnections()
                .Select(connection => GetDealListeningTask(connection, cancellationToken))
                .ToList();
            tasks.AddRange(dealListeningTasks);

            return tasks;
        }

        private Task GetDealListeningTask(ConnectionParams connection, CancellationToken cancellationToken)
        {
            return Retry(() =>
            {
                _serverListenerFactory
                    .Create(connection)
                    .Listen(cancellationToken);

            }, connection, _connectionSettings.Get().RetryCount);
        }

        private static async Task Retry(Action action, ConnectionParams connection, int retryCount)
        {
            try
            {
                await Task.Run(action);
            }
            catch when (retryCount-- > 0)
            {
                Console.WriteLine($"Reconnecting to {connection.Name}");
                Console.WriteLine();
                await Retry(action, connection, retryCount);
            }
            catch
            {
                Console.WriteLine($"Givin up on connecting to {connection.Name}");
            }
        }
    }
}
