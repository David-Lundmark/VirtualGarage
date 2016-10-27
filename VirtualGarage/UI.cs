using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarage
{
    static class UI
    {
        static Garage<Vehicle> garage;

        public static void Init(Garage<Vehicle> g)
        {
            bool done = false;

            garage = g;

            do
            {
                Render();
                string input = GetInput();
            } while (!done);
        }

        static void Render()
        {
            Console.Title = String.Format("{0} +++ {1} / {2} platser upptagna +++", Program.version, garage.Count(), garage.Limit());
        }

        /*
        static void RenderHeader()
        {
            int ox, oy;

            ox = Console.CursorLeft;
            oy = Console.CursorTop;

            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Write("Garage 1.0 - {0} / {1} platser upptagna".PadRight(Console.BufferWidth, ' '), garage.Count(), garage.Limit());
            Console.ResetColor();

            Console.SetCursorPosition(ox, Math.Max(oy, 1));
            Console.CursorVisible = true;
        }
        */

        static string GetInput()
        {
            return Console.ReadLine();
        }
    }
}
