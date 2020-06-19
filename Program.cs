using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using AutoAccept_CSGO3.HideChecker;
using Gma.System.MouseKeyHook;


namespace AutoAccept_CSGO4
{
    internal class Program
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwflags, uint dx, uint dy, uint dwData, uint dwExtraInf);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);


        [DllImport("user32.dll")]
        static extern bool GetCursorPos(out POINT lpPoint);


        public struct POINT
        {
            public int X;
            public int Y;
        }

        static int _x, _y;

        static void ShowMousePosition()
        {
            POINT point;
            if (GetCursorPos(out point) && point.X != _x && point.Y != _y)
            {
                Console.Clear();
                Console.WriteLine("({0},{1})", point.X, point.Y);
                _x = point.X;
                _y = point.Y;
            }
        }


        private const UInt32 LEFTMOUSE_CLICKDOWN = 0x0002;
        private const UInt32 LEFTMOUSE_CLICKUP = 0x0004;


        public static void Main(string[] args)
        {
            new Program().Init();
        }

        public void Init()
        {
            while (true)
            {
                Thread.Sleep(100);


                CSGOopenChecker csgOopenChecker = new CSGOopenChecker();

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


                BuscarPixel();
                Thread.Sleep(100);
            }
        }


        public void BuscarPixel()
        {
            bool salir = false;
            int contador = 0;
            Console.WriteLine("Buscando boton");
            Color color = Color.FromArgb(76, 175, 80);
            Color color2 = Color.FromArgb(76, 175, 80);

            Bitmap bitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height && !salir; y++)
                {
                    Color searchPixel = bitmap.GetPixel(x, y);

                    if (searchPixel == color || searchPixel == color2)
                    {
                        if (contador >= 100)
                        {
                            Console.WriteLine("Boton encontrado");
                            Console.WriteLine("Pulsando boton");
                            ClickF(x, y);
                            Thread.Sleep(5000);
                            salir = true;
                            break;
                        }
                        contador++;
                    }
                }
            }
        }

        public void ClickF(int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(LEFTMOUSE_CLICKDOWN, 0, 0, 0, 0);
            mouse_event(LEFTMOUSE_CLICKUP, 0, 0, 0, 0);
            mouse_event(LEFTMOUSE_CLICKDOWN, 0, 0, 0, 0);
            mouse_event(LEFTMOUSE_CLICKUP, 0, 0, 0, 0);
        }
    }
}