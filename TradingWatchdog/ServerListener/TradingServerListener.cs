using MetaQuotes.MT5CommonAPI;
using MT5Wrapper;
using MT5Wrapper.Interface;
using System;
using System.Collections.Concurrent;
using System.Threading;
using TradingWatchdog.Monitor;

namespace TradingWatchdog.ServerListener
{
    public class TradingServerListener : ITradingServerListener
    {
        private readonly ConnectionParams _connection;
        private readonly IMT5Api _api;
        private readonly ITradesMonitor _monitor;
        private readonly ConcurrentQueue<RawTradeEvent> _rawTradeEvents;

        public TradingServerListener(
            ConnectionParams connection, 
            IMT5Api api, 
            ITradesMonitor monitor
        )
        {
            _connection = connection;
            _api = api;
            _monitor = monitor;
            _rawTradeEvents = new ConcurrentQueue<RawTradeEvent>();
        }

        public void Listen(CancellationToken token)
        {
            try
            {
                _api.ConnectionEvents.ConnectedEventHandler += ConnectionEvents_ConnectedEventHandler;
                _api.ConnectionEvents.DisconnectedEventHandler += ConnectionEvents_DisconnectedEventHandler;
                _api.DealEvents.DealAddEventHandler += DealEvents_DealAddEventHandler;

                _api.Connect(_connection);

                while (!token.IsCancellationRequested)
                {
                    if (_rawTradeEvents.TryDequeue(out RawTradeEvent rawTrade))
                    {
                        var balance = _api.GetUserBalance(rawTrade.Login);
                        var trade = new Trade(_connection.IP, rawTrade, balance);

                        _monitor.QueuedTrades.Enqueue(trade);
                    }
                }

                token.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Stopping {_connection.Name}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error while listening on {_connection.Name}");
                Console.WriteLine(e);
                Console.WriteLine();
                throw;
            }
            finally
            {
                _api.Disconnect();

                _api.ConnectionEvents.ConnectedEventHandler -= ConnectionEvents_ConnectedEventHandler;
                _api.ConnectionEvents.DisconnectedEventHandler -= ConnectionEvents_DisconnectedEventHandler;
                _api.DealEvents.DealAddEventHandler -= DealEvents_DealAddEventHandler;
            }
        }

        private void DealEvents_DealAddEventHandler(object control, CIMTDeal deal)
        {
            var info = new RawTradeEvent()
            {
                Id = deal.Deal(),
                Action = deal.Action(),
                Symbol = deal.Symbol(),
                Volume = deal.Volume(),
                TimeMsc = deal.TimeMsc(),
                Login = deal.Login(),
                Description = deal.Print()
            };

            _rawTradeEvents.Enqueue(info);

            Console.WriteLine($"Deal event received ({_connection.Name}): {deal.Print()}");
            Console.WriteLine();
        }

        private void ConnectionEvents_DisconnectedEventHandler(object sender, EventArgs e)
        {
            Console.WriteLine($"Disconnecting from {_connection.Name}");
            Console.WriteLine();
        }

        private void ConnectionEvents_ConnectedEventHandler(object sender, EventArgs e)
        {
            Console.WriteLine($"Connecting to {_connection.Name}");
            Console.WriteLine();
        }
    }
}
