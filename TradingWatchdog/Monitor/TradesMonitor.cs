using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradingWatchdog.Logging;
using TradingWatchdog.Settings;

namespace TradingWatchdog.Monitor
{
    public class TradesMonitor : ITradesMonitor
    {
        public ConcurrentQueue<Trade> QueuedTrades { get; } = new ConcurrentQueue<Trade>();
        public ConcurrentQueue<Trade> ProcessedTrades { get; } = new ConcurrentQueue<Trade>();
        public List<FlaggedTrades> FlaggedTrades { get; } = new List<FlaggedTrades>();

        private readonly ITradeValidator _validator;
        private readonly ILogger _logger;
        private readonly IMonitoringSettingsRepository _settings;

        public TradesMonitor(
            ITradeValidator validator, 
            ILogger logger,
            IMonitoringSettingsRepository settings
        )
        {
            _validator = validator;
            _logger = logger;
            _settings = settings;
        }

        public async Task Monitor(CancellationToken token)
        {
            try
            {
                var detectionTask = Task.Run(() => DetectSuspiciousTrades(token));
                var retentionPolicyTask = Task.Run(() => RetentionPolicy(token));
                await Task.WhenAll(detectionTask, retentionPolicyTask);

                token.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Stopping monitoring");
                Console.WriteLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine();
            }
            finally
            {
                Console.WriteLine($"There were {QueuedTrades.Count} unprocessed trades left in the queue");
                Console.WriteLine($"Found {FlaggedTrades.Count} suspicious trade pairs");
                Console.WriteLine();
                FlaggedTrades.ForEach(t => Console.WriteLine(t));
                Console.WriteLine();
            }
        }

        private Task DetectSuspiciousTrades(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (QueuedTrades.TryDequeue(out var newTrade))
                {
                    var flaggedTrades = ProcessedTrades
                        .Where(processedTrade => _validator.IsSuspicious(processedTrade, newTrade))
                        .ToList();

                    flaggedTrades.ForEach(flaggedTrade =>
                    {
                        var flagged = new FlaggedTrades()
                        {
                            Match = flaggedTrade,
                            Trade = newTrade
                        };
                        FlaggedTrades.Add(flagged);
                        _logger.Log(flagged);
                    });

                    ProcessedTrades.Enqueue(newTrade);
                }
            }

            return Task.CompletedTask;
        }

        private Task RetentionPolicy(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                if (ProcessedTrades.TryPeek(out var trade))
                {
                    var latestTrade = ProcessedTrades.Last();
                    if (latestTrade.AsAt.Subtract(trade.AsAt).TotalSeconds > _settings.Get().RetentionPeriod)
                    {
                        Console.WriteLine($"Throwing away deal #{trade.Id}");
                        ProcessedTrades.TryDequeue(out _);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
