using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace VirtualGarage
{
    static class UI
    {
        public enum Page { Empty, Main };

        static public Page currentPage { get; private set; }

        static bool appRunning { get; set; }

        static Garage<Vehicle> garage;

        public static void Init(Garage<Vehicle> g = null)
        {
            appRunning = true;
            InitPages();
            SetGarage(g);
            DoInputLoop();
        }

        public static void SetGarage(Garage<Vehicle> g)
        {
            garage = g;
        }

        public static void DoInputLoop()
        {
            bool repeat = true;

            do
            {
                RenderTitle();
                if (repeat)
                {
                    RenderPage(currentPage);
                }
                string input = GetInput();
                repeat = HandleInput(input);
            } while (appRunning);
        }

        static void RenderTitle()
        {
            string temp = Program.version;

            if (garage != null)
            {
                temp += String.Format(" +++ {0} / {1} platser upptagna +++", garage.Count(), garage.Limit());
            }

            Console.Title = temp;
        }

        static void RenderPage(Page page)
        {
            Console.ResetColor();

            switch (page)
            {
                case Page.Empty:
                    break;

                case Page.Main:
                    Console.WriteLine("N. Skapa ett nytt garage");
                    if (garage == null)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    Console.WriteLine("L. Lägg till fordon");
                    Console.WriteLine("T. Ta bort fordon");
                    Console.WriteLine("Ö. Översikt av garaget");
                    Console.WriteLine("V. Visa fordon");
                    Console.WriteLine("S. Sök efter fordon");
                    Console.ResetColor();
                    Console.WriteLine("X. Avsluta");
                    break;

                default:
                    throw new NotImplementedException();
            }

            return;
        }

        static void InitPages()
        {
            currentPage = Page.Main;
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
            string temp = Console.ReadLine();

            if (temp == null)
            {
                temp = "";
            }

            return temp;
        }

        static bool HandleInput(string input)
        {
            if (input == "")
            {
                Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
                return false;
            }

            char c = input.ToLower()[0];

            switch(currentPage)
            {
                case Page.Empty:
                    break;

                case Page.Main:
                    switch(c)
                    {
                        // New garage
                        case 'n':
                            if (garage != null)
                            {
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Varning: det existerande garaget kommer att ersättas!");
                            }
                            if (GetConfirmation("Är du säker på att du vill skapa ett nytt garage? (J/N)", color: ConsoleColor.White))
                            {
                                Console.ResetColor();
                                Console.Write("Ange antal platser i garaget: ");

                                long limit = -1;
                                int maxlimit = int.MaxValue;

                                try
                                {
                                    limit = long.Parse(Console.ReadLine());
                                }
                                catch
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.WriteLine("Felaktig inmatning!\n");
                                    return true;
                                }

                                Console.ForegroundColor = ConsoleColor.Yellow;


                                if (limit < 1)
                                {
                                    Console.WriteLine("Ett garage måste ha minst en plats!\n");
                                    return true;
                                }
                                else if (limit > maxlimit)
                                {
                                    Console.WriteLine("Garaget kan maximalt ha {0} platser!\n", maxlimit);
                                    return true;
                                }

                                try
                                {
                                    garage = new Garage<Vehicle>((int)limit);
                                }
                                catch (OutOfMemoryException)
                                {
                                    Console.WriteLine("Garaget är för stort för att rymmas i minnet!");
                                    return true;
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    return true;
                                }

                                Console.ResetColor();

                                Console.WriteLine("\nEtt nytt garage har skapats.\n");
                            }
                            break;
                        
                        // Overview
                        case 'ö':
                            if (garage != null)
                            {
                                Console.WriteLine("\nÖversikt\n");
                                foreach (var what in Vehicle.GetValidTypes())
                                {
                                    var vehicles = garage.Where(t => (string)t.GetType().GetProperty("Type").GetValue(null, null) == what);
                                    int count = vehicles.Count();
                                    Console.WriteLine("{0}x {1}", count, what);
                                }
                                Console.WriteLine();
                                int taken = garage.Count();
                                Console.WriteLine("Lediga platser: {0}", garage.Limit() - taken);
                                Console.WriteLine("Upptagna platser: {0}", taken);
                                Console.WriteLine("Totalt antal platser: {0}", garage.Limit());
                                Console.WriteLine();
                            }
                            else
                            {
                                NullGarageError();
                            }
                            break;
                        
                        // Show vehicles
                        case 'v':
                            break;
                        
                        // Search for vehicles
                        case 's':
                            break;
                        
                        // Add vehicle
                        case 'l':
                            break;
                        
                        // Remove vehicle
                        case 't':
                            break;
                        
                        // Quit
                        case 'x':
                            if (GetConfirmation("Är du säker på att du vill avsluta? (J/N)"))
                            {
                                Environment.Exit(0);
                            }
                            break;
                        
                        default:
                            InvalidSelectionError();
                            break;
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }

            return true;
        }

        static void GenericError(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
        }

        static void InvalidSelectionError(string message = "Var god ange ett giltigt alternativ.\n", ConsoleColor color = ConsoleColor.White)
        {
            GenericError(message, color);
        }

        static void NullGarageError(string message = "Var god skapa ett garage först.\n", ConsoleColor color = ConsoleColor.Yellow)
        {
            GenericError(message, color);
        }

        static bool GetConfirmation(string message, char confirm = 'j', ConsoleColor color = ConsoleColor.Yellow)
        {
            if (message != null)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
            }

            string input = GetInput();

            if (input.Length > 0)
            {
                if (input.ToLower()[0] == confirm)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
