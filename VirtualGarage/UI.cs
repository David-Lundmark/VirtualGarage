using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using System.Runtime.Remoting;

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
            Console.SetWindowSize(Console.LargestWindowWidth / 2, Console.LargestWindowHeight / 2);
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
                    Console.WriteLine("Ö. Översikt av garaget");
                    Console.WriteLine("V. Visa fordon");
                    Console.WriteLine("S. Sök efter fordon");
                    Console.WriteLine("L. Lägg till fordon");
                    Console.WriteLine("T. Ta bort fordon");
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

        static string GetInput(bool allowNull = false)
        {
            string temp = Console.ReadLine();

            if (temp == null && allowNull == false)
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
                                GenericMessage("Varning: det existerande garaget kommer att ersättas!", ConsoleColor.Red);
                            }

                            if (GetConfirmation("Är du säker på att du vill skapa ett nytt garage? (J/N)", color: ConsoleColor.Yellow))
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

                                Console.ForegroundColor = ConsoleColor.White;
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
                                    var types = garage.Where(t => (string)t.GetType().GetProperty("Type").GetValue(t, null) == what);
                                    int count = types.Count();
                                    if (count > 0)
                                    {
                                        Console.ForegroundColor = ConsoleColor.Gray;
                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.DarkGray;
                                    }
                                    Console.WriteLine("{0}x {1}", count, what);
                                }
                                Console.WriteLine();
                                int taken = garage.Count();
                                Console.ForegroundColor = ConsoleColor.Gray;
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
                            if (garage != null)
                            {
                                DisplayVehicleInfo();
                            }
                            else
                            {
                                NullGarageError();
                            }
                            break;
                        
                        // Search for vehicles
                        case 's':
                            break;
                        
                        // Add vehicle
                        case 'l':
                            if (garage != null)
                            {
                                if (garage.Count() == garage.Limit())
                                {
                                    GenericMessage("Garaget är fullt!", ConsoleColor.Yellow);
                                }
                                else
                                {
                                    Console.WriteLine("\nVälj typ av fordon att lägga till:");
                                    int count = DisplayNumberedList(Vehicle.GetValidTypes());
                                    AbortMessage();
                                    int sel = GetNumber(0, count);
                                    if (sel > 0)
                                    {
                                        Type vehtype = HelperMethods.GetDerivedConcreteClasses(typeof(Vehicle)).ElementAt(sel - 1);
                                        //Console.WriteLine(vehtype);
                                        var obj = (Vehicle)vehtype.InvokeMember("CreateEmpty", BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public, null, null, null);
                                        //Console.WriteLine(obj);
                                        if (GetValidFields(obj))
                                        {
                                            garage.Add(obj);
                                            GenericMessage("\nFordonet lades till!\n", ConsoleColor.White);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                NullGarageError();
                            }
                            break;
                        
                        // Remove vehicle
                        case 't':
                            if (garage != null)
                            {
                                if (garage.Count() == 0)
                                {
                                    GenericMessage("Garaget är tomt!", ConsoleColor.Yellow);
                                }
                                else
                                {
                                    Console.WriteLine("\nVälj ett fordon att ta bort:");
                                    int count = DisplayNumberedList(MakeVehicleList());
                                    AbortMessage();
                                    int sel = GetNumber(0, count);
                                    if (sel > 0)
                                    {
                                        var veh = garage.ElementAt(sel - 1);
                                        if (GetConfirmation(string.Format("Vill du ta bort {0} med reg. nr. {1}? (J/N)", veh.Type, veh.Registration)))
                                        {
                                            GenericMessage("Fordonet togs bort!\n", ConsoleColor.White);
                                            garage.Remove(veh);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                NullGarageError();
                            }
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

        static void DisplayVehicleInfo()
        {
            Console.WriteLine("Fordon\n");
            Console.ForegroundColor = ConsoleColor.Cyan;

            foreach (var item in Vehicle.GetDescribedProperties())
            {
                Console.Write("{0, -20}", item);
            }

            Console.WriteLine();

            var group = garage.OrderBy(v => v.Type).Select(v => v);
            int ctr = 0;
            foreach (var veh in group)
            {
                ctr++;
                foreach (var item in Vehicle.GetRawProperties())
                {
                    var prop = veh.GetType().GetProperty(item);

                    if (prop != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write("{0, -20}", prop.GetValue(veh, null));
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("{0, -20}", "-");
                    }
                }
                Console.WriteLine();
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n{0} av totalt {1} fordon\n", ctr, garage.Count());
        }

        static int DisplayNumberedList(List<string> contents)
        {
            int ctr = 0;
            foreach (var item in contents)
            {
                ctr++;
                Console.WriteLine("{0}. {1}", ctr, item);
            }

            return ctr;
        }

        static List<string> MakeVehicleList()
        {
            List<string> list = new List<string>();

            foreach (var veh in garage)
            {
                list.Add(string.Format("{0,-20} (reg.nr. {1})", veh.GetTypeString(), veh.Registration));
            }

            return list;
        }

        static void GenericMessage(string message, ConsoleColor color = ConsoleColor.Gray)
        {
            var old = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = old;
        }

        static void AbortMessage(string message = "0. Avbryt\n", ConsoleColor color = ConsoleColor.DarkYellow)
        {
            GenericMessage(message, color);
        }

        static void InvalidSelectionError(string message = "Var god ange ett giltigt alternativ.\n", ConsoleColor color = ConsoleColor.White)
        {
            GenericMessage(message, color);
        }

        static void NullGarageError(string message = "Var god skapa ett garage först.\n", ConsoleColor color = ConsoleColor.Yellow)
        {
            GenericMessage(message, color);
        }

        static int GetNumber(int min, int max)
        {
            long sel = 0;
            bool done = false;

            do
            {
                bool error = false;
                string input = Console.ReadLine();

                try
                {
                    sel = long.Parse(input);
                }
                catch
                {
                    GenericMessage("Felaktig inmatning!\n", ConsoleColor.Yellow);
                    error = true;
                }

                if (!error)
                {
                    if (sel < min || sel > max)
                    {
                        InvalidSelectionError(string.Format("Var god ange ett tal mellan {0} och {1}.", min, max));
                    }
                    else
                    {
                        done = true;
                    }
                }
            } while (!done);

            return (int)sel;
        }

        static bool GetValidFields(Vehicle veh)
        {
            List<string> org = Vehicle.GetRawProperties(true);
            List<string> swe = Vehicle.GetDescribedProperties(true);

            for (int i = 0; i < org.Count; i++)
            {
                var prop = veh.GetType().GetProperty(org[i]);

                if (prop != null)
                {
                    bool done = false;
                    var type = prop.PropertyType;

                    do
                    {
                        Console.WriteLine("(Mata in Ctrl + Z om du vill avbryta operationen.)");
                        Console.Write("Ange värde för '{0}': ", swe[i]);
                        var input = GetInput(true);
                        if (input != null)
                        {
                            try
                            {
                                if (input != "" || (input == "" && GetConfirmation("Vill du verkligen ange ett tomt värde? (J/N)")))
                                {
                                    prop.SetValue(veh, Convert.ChangeType(input, type));
                                    done = true;
                                }
                            }
                            catch
                            {
                                GenericMessage("Felaktig inmatning; försök igen!", ConsoleColor.Yellow);
                            }
                        }
                        else
                        {
                            if (GetConfirmation("Vill du återvända till huvudmenyn utan att skapa ett fordon? (J/N)"))
                            {
                                return false;
                            }
                        }
                    } while (!done);
                }
            }
            return true;
        }

        static bool GetConfirmation(string message, char confirm = 'j', ConsoleColor color = ConsoleColor.Yellow)
        {
            if (message != null)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
            }

            Console.ResetColor();

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
