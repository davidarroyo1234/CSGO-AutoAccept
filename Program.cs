using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AutoAccept_CSGO4.HideChecker;
using AutoAccept_CSGO4.TelegramBot;

namespace AutoAccept_CSGO4
{
    internal class Program
    {
        private const uint LEFTMOUSE_CLICKDOWN = 0x0002;
        private const uint LEFTMOUSE_CLICKUP = 0x0004;

        private TelegramBotHandler telegramBotHandler;

        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwflags, uint dx, uint dy, uint dwData, uint dwExtraInf);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        public static async Task Main(string[] args)
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
            await new Program().Init(args);
        }

        private async Task Init(string[] args)
        {
            while (true)
            {
                try
                {
                    await StartDecision();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error has occurred Restarting..." + e.Message);
                }
            }
        }

        private async Task StartDecision()
        {
            Console.WriteLine(
                "1- Start bot with Telegram notifications\n" +
                "2- Start bot without Telegram notifications\n" +
                (TelegramBotHandler.HasTelegramToken() ? "3- Delete telegram token\n" : "")
            );

            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await HandleTelegramTokenBeforeInitialization();
                    break;
                case "2":
                    Console.WriteLine("Starting bot without Telegram notifications...");
                    await BeginSearch();
                    break;
                case "3" when TelegramBotHandler.HasTelegramToken():
                    TelegramBotHandler.DeleteTelegramToken();
                    Console.WriteLine("Token deleted");
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    break;
            }

            await StartDecision();
        }

        private async Task HandleTelegramTokenBeforeInitialization()
        {
            telegramBotHandler = new TelegramBotHandler();

            if (TelegramBotHandler.HasTelegramToken())
            {
                await telegramBotHandler.InitializeTelegramBot();
                Console.WriteLine("Starting bot with Telegram notifications...");
            }
            else
            {
                Console.WriteLine("Enter your telegram bot token:");
                var token = Console.ReadLine();
                if (!String.IsNullOrEmpty(token))
                {
                    TelegramBotHandler.SetTelegramToken(token);
                    await telegramBotHandler.InitializeTelegramBot();
                    Console.WriteLine("Starting bot with Telegram notifications...");
                }
                else
                {
                    Console.WriteLine("Invalid token");
                    return;
                }
            }

            await BeginSearch();
        }

        private async Task BeginSearch()
        {
            while (true)
            {
                Thread.Sleep(100);

                var csgoOpenChecker = new CsgoOpenChecker();

                if (csgoOpenChecker.MinimizedCheck() == -1)
                {
                    Console.WriteLine("Open csgo to start");
                }
                else if (csgoOpenChecker.MinimizedCheck() == 1)
                {
                    Console.WriteLine("Maximize CSGO");
                }

                while (csgoOpenChecker.MinimizedCheck() != 0)
                {
                    Thread.Sleep(300);
                }

                await SearchPixel();
                Thread.Sleep(100);
            }
        }

        private async Task SearchPixel()
        {
            var counter = 0;
            var exitLoop = false;
            Console.WriteLine("Searching button...");

            var color = Color.FromArgb(76, 175, 80);
            var color2 = Color.FromArgb(90, 203, 94);

            var bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);

            for (var x = 0; x < bitmap.Width && !exitLoop; x++)
            {
                for (var y = 0; y < bitmap.Height; y++)
                {
                    var searchPixel = bitmap.GetPixel(x, y);

                    if (searchPixel == color || searchPixel == color2)
                    {
                        if (counter >= 9000)
                        {
                            Thread.Sleep(200);
                            SetCursorPos(x, y);
                            await ClickFunc(x, y);
                            exitLoop = true;
                            break;
                        }

                        counter++;
                    }
                }
            }
        }

        private async Task ClickFunc(int x, int y)
        {
            Console.WriteLine("Clicking...");
            SetCursorPos(x, y);
            mouse_event(LEFTMOUSE_CLICKDOWN, 0, 0, 0, 0);
            mouse_event(LEFTMOUSE_CLICKUP, 0, 0, 0, 0);
            
            if (telegramBotHandler != null && telegramBotHandler.isInitialized)
            {
                try
                {
                    Console.WriteLine("Sending notification...");
                    await telegramBotHandler.SendMatchFoundToAllClients();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error sending notification: " + e.Message);
                }
            }
            
            Thread.Sleep(200);


        }
    }
}