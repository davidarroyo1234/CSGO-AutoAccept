using System;
using System.Drawing;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using AutoAccept_CSGO4.HideChecker;
using AutoAccept_CSGO4.AntiAFK;


namespace AutoAccept_CSGO4
{
    internal class Program
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwflags, uint dx, uint dy, uint dwData, uint dwExtraInf);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);


        private const uint LEFTMOUSE_CLICKDOWN = 0x0002;
        private const uint LEFTMOUSE_CLICKUP = 0x0004;

        public static void Main(string[] args)
        {
            Console.WriteLine(@"
 _____  _____ _____ _____         ___        _         ___                     _                             
/  __ \/  ___|  __ \  _  |       / _ \      | |       / _ \                   | |                            
| /  \/\ `--.| |  \/ | | |______/ /_\ \_   _| |_ ___ / /_\ \ ___ ___ ___ _ __ | |_                           
| |     `--. \ | __| | | |______|  _  | | | | __/ _ \|  _  |/ __/ __/ _ \ '_ \| __|                          
| \__/\/\__/ / |_\ \ \_/ /      | | | | |_| | || (_) | | | | (_| (_|  __/ |_) | |_                           
 \____/\____/ \____/\___/       \_| |_/\__,_|\__\___/\_| |_/\___\___\___| .__/ \__|                          
                                                                        | |                                  
                                                                        |_|                                  
______                       _             _     _                                  __   _____  _____    ___ 
| ___ \            ____     | |           (_)   | |                                /  | / __  \|____ |  /   |
| |_/ /_   _      / __ \  __| | __ ___   ___  __| | __ _ _ __ _ __ ___  _   _  ___ `| | `' / /'    / / / /| |
| ___ \ | | |    / / _` |/ _` |/ _` \ \ / / |/ _` |/ _` | '__| '__/ _ \| | | |/ _ \ | |   / /      \ \/ /_| |
| |_/ / |_| |   | | (_| | (_| | (_| |\ V /| | (_| | (_| | |  | | | (_) | |_| | (_) || |_./ /___.___/ /\___  |
\____/ \__, |    \ \__,_|\__,_|\__,_| \_/ |_|\__,_|\__,_|_|  |_|  \___/ \__, |\___/\___/\_____/\____/     |_/
        __/ |     \____/                                                 __/ |                               
       |___/                                                            |___/                                
");
            new Program().Init(args);
        }

        private void Init(string[] args)
        {
            while (true)
            {
                Thread.Sleep(100);

                var csgOopenChecker = new CsgOopenChecker();

                if (csgOopenChecker.MinimizedCheck() == -1)
                {
                    Console.WriteLine("Open csgo to start");
                }
                else if (csgOopenChecker.MinimizedCheck() == 1)
                {
                    Console.WriteLine("Maximize CSGO");
                }

                while (csgOopenChecker.MinimizedCheck() != 0)
                {
                    Thread.Sleep(300);
                }

                SearchPixel();
                Thread.Sleep(100);
            }
        }


        //Starts searching for the accept button in the screen.
        private void SearchPixel()
        {
            var counter = 0;
            var exitLoop = false;
            Console.WriteLine("Searching button...");

            //Accept button color declaration.
            var color = Color.FromArgb(76, 175, 80);
            var color2 = Color.FromArgb(90, 203, 94);

            var bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);


            //Checks the pixels from x:0 and y:0 to your screen resolution.
            for (var x = 0; x < bitmap.Width && !exitLoop; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var searchPixel = bitmap.GetPixel(x, y);
                    //If the current checking color matches the declared colors it will add +1 to the counter.
                    //This counter is used to prevent to click any other matching color that is not the accept button.
                    //The csgo accept button contains more than 9000 green pixels.

                    if (searchPixel == color || searchPixel == color2)
                    {
                        if (counter >= 9000)
                        {
                            Thread.Sleep(200);
                            SetCursorPos(x, y);
                            Console.WriteLine("Button found! Pressing...");
                            ClickF(x, y);
                            exitLoop = true;
                            break;
                        }
                        counter++;
                    }
                }
            }
        }

        private void ClickF(int x, int y)
        {
            Console.WriteLine("Clicking...");
            SetCursorPos(x, y);
            mouse_event(LEFTMOUSE_CLICKDOWN, 0, 0, 0, 0);
            mouse_event(LEFTMOUSE_CLICKUP, 0, 0, 0, 0);
            Thread.Sleep(200);
        }
    }
}