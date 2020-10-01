using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AutoAccept_CSGO4.HideChecker
{
    public class CSGOopenChecker
    {
        [DllImport("User32.Dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.Dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("User32.Dll")]
        
        private static extern bool IsIconic(IntPtr hWnd);

        
        public int Minimizado()
        {
            Process[] processes = Process.GetProcessesByName("csgo");
            if (processes.Length == 1)
            {
                IntPtr hWnd = processes[0].MainWindowHandle;

                if (IsIconic(hWnd))
                {
                    return 1;
                }
                return 0;
            }
            return -1;
        }
    }
}