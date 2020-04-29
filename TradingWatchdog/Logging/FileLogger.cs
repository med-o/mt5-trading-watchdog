using System.IO;
using TradingWatchdog.Monitor;
using TradingWatchdog.Settings;

namespace TradingWatchdog.Logging
{
    public class FileLogger : ILogger
    {
        private readonly ILogEntryFormatter _logEntryFormatter;
        private readonly ILoggerSettingsRepository _loggerSettingsRepository;
        private readonly object lockObj = new object();

        public FileLogger(
            ILogEntryFormatter logEntryFormatter,
            ILoggerSettingsRepository loggerSettingsRepository
        )
        {
            _logEntryFormatter = logEntryFormatter;
            _loggerSettingsRepository = loggerSettingsRepository;
        }

        public void Log(string message)
        {
            var formattedMessage = _logEntryFormatter.Format(message);
            LogFormatted(formattedMessage);
        }

        public void Log(FlaggedTrades flagged)
        {
            var formattedMessage = _logEntryFormatter.Format(flagged);
            LogFormatted(formattedMessage);
        }

        private void LogFormatted(string message)
        {
            lock (lockObj)
            {
                using (var streamWriter = new StreamWriter(GetFilePath(), true))
                {
                    streamWriter.WriteLine(message);
                    streamWriter.Close();
                }
            }
        }

        private string GetFilePath()
        {
            var settings = _loggerSettingsRepository.Get();
            var path = Path.Combine(settings.OutputPath, settings.OutputFileName);
            var file = new FileInfo(path);
            file.Directory.Create();
            return file.FullName;
        }
    }
}
