using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using TradingWatchdog.Monitor;

namespace TradingWatchdog.Logging
{
    public class JsonLogEntryFormatter : ILogEntryFormatter
    {
        public string Format(string message)
        {
            var transformedEntry = new
            {
                _timestamp = DateTime.Now.ToUniversalTime(),//.ToString("yyyyMMddHHmmss"),
                message,
            };

            return Serialize(transformedEntry);
        }

        public string Format(FlaggedTrades flagged)
        {
            var transformedEntry = new
            {
                _timestamp = DateTime.Now,
                dealId = flagged.Trade.Id,
                dealAccount = flagged.Trade.Login,
                dealServer = flagged.Trade.Server,
                matchedDealId = flagged.Match.Id,
                matchedAccount = flagged.Match.Login,
                matchedServer = flagged.Match.Server,
            };

            return Serialize(transformedEntry);
        }

        private string Serialize(dynamic transformedEntry)
        {
            return JsonConvert.SerializeObject(transformedEntry, Formatting.None, new StringEnumConverter());
        }
    }
}
