using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using LeagueQuitter.services;
using LeagueQuitter.config;

namespace LeagueQuitter
{
    class Program
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static LeagueService leagueDetect;
        private static LoggingService logger;
        private static Config cfg;

        public static void Main() {
            logger = SetupLogging();
            cfg = new Config(logger);

            _hookID = SetHook(_proc);

            logger.Information("Ready!");
            leagueDetect = new LeagueService(logger, cfg.LQCfg.ProcCheckDelay);

            Application.Run();

            UnhookWindowsHookEx(_hookID);
        }

        private static LoggingService SetupLogging() {
            return new LoggingService();
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc) {
            using(Process curProcess = Process.GetCurrentProcess())
            using(ProcessModule curModule = curProcess.MainModule) {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam) {
            if(nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN) {
                int vkCode = Marshal.ReadInt32(lParam);
                var keys = cfg.LQCfg.Hotkeys;
                for(int i = 0; i < keys.Length; i++) {
                    if(keys[i] == vkCode) {
                        leagueDetect.KillClient();
                    }
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
