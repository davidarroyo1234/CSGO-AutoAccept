using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AutoAccept_CSGO4.HideChecker
{
    public class CsgoOpenChecker
    {
        [DllImport("User32.Dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.Dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("User32.Dll")]
        
        private static extern bool IsIconic(IntPtr hWnd);

        
        public int MinimizedCheck()
        {
            var processes = Process.GetProcessesByName("csgo");
            if (processes.Length != 1) return -1;
            
            var hWnd = processes[0].MainWindowHandle;

            return IsIconic(hWnd) ? 1 : 0;
        }
    }
}