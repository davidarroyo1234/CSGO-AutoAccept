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


        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out Point lpPoint);


        private struct Point
        {
            public int X;
            public int Y;
        }

        static int _x, _y;

        private const uint LEFTMOUSE_CLICKDOWN = 0x0002;
        private const uint LEFTMOUSE_CLICKUP = 0x0004;

        private string[] argumentos;
        public static void Main(string[] args)
        {
            
            new Program().Init(args);
        }

        private bool beep;

        private void Init(string[] args)
        {
            argumentos = args;
            while (true)
            {
                Thread.Sleep(100);
                
                var csgOopenChecker = new CSGOopenChecker();

                if (csgOopenChecker.Minimizado() == -1)
                {
                    Console.WriteLine("Abre el csgo para empezar");
                }
                else if (csgOopenChecker.Minimizado() == 1)
                {
                    Console.WriteLine("Desminimiza el csgo");
                }

                while (csgOopenChecker.Minimizado() != 0)
                {
                    Thread.Sleep(300);
                }

                if (!beep)
                {
                    var player = new SoundPlayer
                    {
                        SoundLocation = Environment.CurrentDirectory + "/Media/beep.wav"
                    };
                    
                    player.PlaySync();
                    Thread.Sleep(200);
                    player.Stop();
                    beep = true;
                }

                BuscarPixel();
                Thread.Sleep(100);
            }
        }

        private int _xbck;
        private int _ybck;
        private int _contador2;

        private void BuscarPixel()
        {
            var salir = false;
            var contador = 0;
            Console.WriteLine("Buscando boton");
            var color = Color.FromArgb(76, 175, 80);
            var color2 = Color.FromArgb(90, 203, 94);

            var bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            var graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            for (var x = 0; x < bitmap.Width; x++)
            {
                for (var y = 0; y < bitmap.Height && !salir; y++)
                {
                    var searchPixel = bitmap.GetPixel(x, y);

                    if (searchPixel == color || searchPixel == color2)
                    {
                        if (contador >= 9000)
                        {
                            if (_xbck != x && _ybck != y)
                            {
                                SetCursorPos(x, y);
                            }

                            Thread.Sleep(200);


                            var bitmap2 = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                Screen.PrimaryScreen.Bounds.Height);
                            var graphics2 = Graphics.FromImage(bitmap2);
                            graphics2.CopyFromScreen(0, 0, 0, 0, bitmap2.Size);
                            var searchPixel2 = bitmap2.GetPixel(x, y);
                            if (searchPixel2 == color2)
                            {
                                SetCursorPos(x, y);
                                Console.WriteLine("Boton encontrado");
                                Console.WriteLine("Pulsando boton");
                                ClickF(x, y);
                                Thread.Sleep(1000);
                            }

                            if (_contador2 <= 1)
                            {
                                _xbck = x;
                                _ybck = y;
                            }

                            _contador2++;

                            salir = true;
                            break;
                        }

                        contador++;
                    }
                }
            }
        }

        private void ClickF(int x, int y)
        {
            Console.WriteLine("INTENTANDO CLICK");
            SetCursorPos(x, y);
            Thread.Sleep(50);
            SetCursorPos(x, y);
            mouse_event(LEFTMOUSE_CLICKDOWN, 0, 0, 0, 0);
            mouse_event(LEFTMOUSE_CLICKUP, 0, 0, 0, 0);
            var lol = new AfkCommands();
            Thread.Sleep(200);

            foreach (var argu in argumentos)
            {
                if (argu.ToLower().Equals("+left"))
                {
                    lol.PlusLeft();
                    Console.WriteLine("Activado +left");
                    Thread.Sleep(250);
                }

                else if (argu.ToLower().Equals("+forward"))
                {
                    lol.PlusForward();
                    Console.WriteLine("Activado +forward");
                    Thread.Sleep(250);
                }
            }
        }
    }
}