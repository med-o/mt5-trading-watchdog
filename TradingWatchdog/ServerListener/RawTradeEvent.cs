namespace TradingWatchdog.ServerListener
{
    public class RawTradeEvent
    {
        public ulong Id { get; set; }
        public uint Action { get; set; }
        public string Symbol { get; set; }
        public ulong Volume { get; set; }
        public long TimeMsc { get; set; }
        public ulong Login { get; set; }
        public string Description { get; set; }
    }
}
