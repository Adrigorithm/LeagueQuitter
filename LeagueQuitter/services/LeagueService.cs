using System.Diagnostics;
using System.Timers;

namespace LeagueQuitter.services
{
    class LeagueService {
        private Timer _timer;
        private LoggingService _logger;
        private bool isDetected;

        public LeagueService(LoggingService logger, int procCheckDelay) {
            _logger = logger;
            SetupTimer(procCheckDelay);
        }

        ~LeagueService() {
            Shutdown();
        }

        private void SetupTimer(int procCheckDelay) {
            _timer = new Timer();
            _timer.Elapsed += TimerElapsed;
            _timer.Interval = 1000;
            if(procCheckDelay >= 500 && procCheckDelay <= 10000) {
                _timer.Interval = procCheckDelay;
            }
            _timer.Enabled = true;
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e) {
            switch((isDetected, Process.GetProcessesByName("League of Legends").Length > 0)){
                case (false, true):
                    _logger.Information("League in-game client detected! :)");
                    isDetected = true;
                    break;
                case (true, false):
                    _logger.Warning("Connection to in-game client lost. :(");
                    isDetected = false;
                    break;
            }
        }

        public void KillClient() {
            var clientProc = Process.GetProcessesByName("League of Legends");

            if(clientProc.Length == 0) {
                _logger.Error("Couldn't shutdown client, is the process zombie?");
                return;
            }
            clientProc[0].Kill();
            _logger.Warning("Bye bye league! ^.^");
        }

        public void Shutdown() {
            _timer.Stop();
            _timer.Dispose();
        }
    }
}
