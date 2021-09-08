using Serilog;
using System;

namespace LeagueQuitter.services
{
    public class LoggingService
    {
        private ILogger _logger;

        public LoggingService() {
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("logs\\latest_log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public void Information(string content) =>
            _logger.Information(content);

        public void Warning(string content) =>
            _logger.Warning(content);

        public void Error(Exception e, string content) =>
            _logger.Error(e, content);

        public void Error(string content) =>
            _logger.Error(content);
    }
}
