using MetaQuotes.MT5CommonAPI;
using System;
using TradingWatchdog.ServerListener;

namespace TradingWatchdog.Monitor
{
    public class Trade
    {
        public Trade() { }

        public Trade(string server, RawTradeEvent e, decimal balance)
        {
            Server = server;
            Id = e.Id;
            Action = (CIMTDeal.EnDealAction)e.Action;
            Symbol = e.Symbol;
            Volume = e.Volume;
            AsAt = DateTimeOffset.FromUnixTimeMilliseconds(e.TimeMsc).UtcDateTime;
            Login = e.Login;
            Description = e.Description;
            Balance = balance;
        }

        public string Server { get; set; }
        public ulong Id { get; set; }
        public CIMTDeal.EnDealAction Action { get; set; }
        public string Symbol { get; set; }
        public ulong Volume { get; set; }
        public DateTime AsAt { get; set; }
        public ulong Login { get; set; }
        public decimal Balance { get; set; }
        public string Description { get; set; }

        public decimal VolumeToBalanceRatio { get { return Volume / Balance; } }

        public override string ToString()
        {
            return $"Deal received: {Description}" +
                $"\n\tServer: {Server}" +
                $"\n\tDealId: {Id}" +
                $"\n\tAsAt: {AsAt}" +
                $"\n\tAction: {Action}" +
                $"\n\tLogin: {Login}" +
                $"\n\tBalance: {Balance}" +
                $"\n\tSymbol: {Symbol}" +
                $"\n\tVolume: {Volume}";
        }
    }
}
