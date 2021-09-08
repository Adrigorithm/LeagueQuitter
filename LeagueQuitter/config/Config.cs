using LeagueQuitter.helpers.extensions;
using LeagueQuitter.services;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace LeagueQuitter.config
{
    public class Config
    {
        public LeagueQuitterCfg LQCfg { get; private set; }
        private LoggingService _logger;
        private const string ParsingRegex = "^>(.*?)((?:\\d+)(?:,\\d+)*)$";
        private const string AlphaRegex = "[a-zA-Z]+";

        public Config(LoggingService logger) =>
            LoadConfig(logger);

        private void LoadConfig(LoggingService logger) {
            _logger = logger;

            if(Directory.Exists(@".\config") && File.Exists(@".\config\settings.txt")) {
                try {
                    LQCfg = Parse();
                } catch(Exception e) {
                    _logger.Error(e, "Parsing failed!");
                }
            } else {
                _logger.Error($"Couldn't find the configuration file, creating a new one.");

                try {
                    LQCfg = WriteDefault();
                } catch(Exception e) {
                    _logger.Error(e, "Couldn't create file, please resolve the error manually.");
                }
            }
        }

        private static LeagueQuitterCfg WriteDefault() {
            Directory.CreateDirectory(@".\config");

            var stdConfig = "Numeric values of the accepted keys to quit the game\n" +
                "Valid numbers: https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.keys?view=net-5.0\n" +
                "> Hotkeys: 115,35\n" +
                "\n" +
                "Milliseconds to wait before checking for an active game\n" +
                "> ProcCheckDelay: 1000\n" +
                "";

            File.WriteAllText(@".\config\settings.txt", stdConfig);

            return Parse();
        }

        private static LeagueQuitterCfg Parse() {
            LeagueQuitterCfg cfg = new LeagueQuitterCfg();

            foreach(var line in File.ReadAllLines(@".\config\settings.txt")) {
                var lineModified = line.TrimEnd('\r', '\n');
                var match = Regex.Match(lineModified, ParsingRegex);
                if(!string.IsNullOrEmpty(match.Value)) {
                    var memberName = Regex.Match(match.Groups[1].Value, AlphaRegex).Value;
                    
                    switch(memberName) {
                        case "Hotkeys":
                            cfg.Hotkeys = Array.ConvertAll(match.Groups[2].Value.Split(','), int.Parse);
                            break;
                        case "ProcCheckDelay":
                            cfg.ProcCheckDelay = int.Parse(match.Groups[2].Value);
                            break;
                    }
                }
            }

            if(cfg.Hotkeys.Length == 0 || cfg.ProcCheckDelay is default(int)) {
                throw new FormatException("Configuration file is corrupt.\nEither manually fix or remove it.");
            }

            return cfg;
        }
    }

    public struct LeagueQuitterCfg
    {
        public int[] Hotkeys { get; set; }
        public int ProcCheckDelay { get; set; }
    }
}
