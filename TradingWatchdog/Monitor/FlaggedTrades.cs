namespace TradingWatchdog.Monitor
{
    public class FlaggedTrades
    {
        public Trade Match { get; set; }
        public Trade Trade { get; set; }

        public override string ToString()
        {
            return "Detected suspicious trade pair:" +
                $"\n\t{Match.Description}" +
                $"\n\t{Trade.Description}";
        }
    }
}
