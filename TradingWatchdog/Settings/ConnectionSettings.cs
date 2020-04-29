using MT5Wrapper;
using System.Collections.Generic;
using System.Linq;

namespace TradingWatchdog.Settings
{
    public class ConnectionSettings
    {
        /// <summary>
        /// List of MT5 trading servers to connect to, including port numbers
        /// </summary>
        public List<string> Servers { get; set; }

        /// <summary>
        /// Login for MT5 trading server
        /// </summary>
        public ulong Login { get; set; }

        /// <summary>
        /// Password for MT5 trading server
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Number of retries when connection drops
        /// </summary>
        public int RetryCount { get; set; }
    }

    public static class ConnectionSettingsExtensions
    {
        public static IEnumerable<ConnectionParams> GetConnections(this ConnectionSettings settings)
        {
            return settings.Servers.Select(server => new ConnectionParams()
            {
                Login = settings.Login,
                Password = settings.Password,
                IP = server,
                Name = $"server: {server}"
            });
        }
    }
}
