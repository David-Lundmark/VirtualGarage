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
            Console.SetWindowSize((int)(Console.LargestWindowWidth * 0.75), (int)(Console.LargestWindowHeight * 0.75));
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
                    if (garage == null)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine("N. Skapa ett nytt garage");
                    Console.ResetColor();
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

            switch (currentPage)
            {
                case Page.Empty:
                    break;

                case Page.Main:
                    switch (c)
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
                            else
                            {
                                AbortedMessage();
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
                                Console.WriteLine("Fordon:\n");
                                DisplayVehicleInfo(garage.ToList(), true);
                            }
                            else
                            {
                                NullGarageError();
                            }
                            break;

                        // Search for vehicles
                        case 's':
                            if (garage != null)
                            {
                                if (garage.Count() == 0)
                                {
                                    GenericMessage("Garaget är tomt!", ConsoleColor.Yellow);
                                }
                                else
                                {
                                    SearchVehicles();
                                }
                            }
                            else
                            {
                                NullGarageError();
                            }
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
                                    AbortOptionMessage();
                                    int sel = GetNumber(0, count);
                                    if (sel > 0)
                                    {
                                        Type vehtype = HelperMethods.GetDerivedConcreteClasses(typeof(Vehicle)).ElementAt(sel - 1);
                                        //Console.WriteLine(vehtype);
                                        var obj = (Vehicle)vehtype.InvokeMember("Create", BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public, null, null, null);
                                        //Console.WriteLine(obj);
                                        if (GetValidFields(obj))
                                        {
                                            GenericMessage("\nFordonet lades till!\n", ConsoleColor.White);
                                            garage.Add(obj);
                                        }
                                    }
                                    else
                                    {
                                        AbortedMessage();
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
                                    AbortOptionMessage();
                                    int sel = GetNumber(0, count);
                                    if (sel > 0)
                                    {
                                        var veh = garage.ElementAt(sel - 1);
                                        if (GetConfirmation(string.Format("Vill du ta bort {0} med reg. nr. {1}? (J/N)", veh.Type, veh.Registration)))
                                        {
                                            GenericMessage("Fordonet togs bort!\n", ConsoleColor.White);
                                            garage.Remove(veh);
                                        }
                                        else
                                        {
                                            AbortedMessage();
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

        static void DisplayVehicleInfo(List<Vehicle> group, bool showTotal, string searchProp = null)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            foreach (var item in Vehicle.GetDescribedProperties())
            {
                Console.Write("{0, -20}", item);
            }

            Console.WriteLine();

            var searchGroup = group.OrderBy(v => v.Type).Select(v => v);
            int ctr = 0;
            foreach (var veh in searchGroup)
            {
                ctr++;
                foreach (var item in Vehicle.GetRawProperties())
                {
                    var prop = veh.GetType().GetProperty(item);
                    Console.ResetColor();

                    if (prop != null)
                    {
                        if (searchProp == item)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Blue;
                        }
                        Console.Write("{0, -20}", prop.GetValue(veh, null));
                    }
                    else
                    {
                        if (searchProp == item)
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Blue;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.DarkGray;
                        }
                        Console.Write("{0, -20}", "-");
                    }
                }
                Console.WriteLine();
            }
            if (showTotal)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("\n{0} av totalt {1} fordon\n", ctr, searchGroup.Count());
            }
            else
            {
                Console.WriteLine();
            }
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

        static void AbortOptionMessage(string message = "0. Avbryt\n", ConsoleColor color = ConsoleColor.DarkYellow)
        {
            GenericMessage(message, color);
        }

        static void AbortedMessage(string message = "Åtgärden avbröts!\n", ConsoleColor color = ConsoleColor.Yellow)
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

        static bool GetValidFields(Vehicle veh)
        {
            const int maxInputLength = 18;

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
                            if (input.Length > maxInputLength)
                            {
                                input = input.Substring(0, Math.Min(input.Length, maxInputLength));
                                GenericMessage(string.Format("OBS: Värdets längd trunkerades till {0} tecken!", maxInputLength), ConsoleColor.White);
                            }
                            if (input != "" || (input == "" && GetConfirmation("Vill du verkligen ange ett tomt värde? (J/N)")))
                            {
                                try
                                {
                                    prop.SetValue(veh, Convert.ChangeType(input, type));
                                    done = true;
                                }
                                catch
                                {
                                    GenericMessage(string.Format("Felaktig inmatning; försök ange ett värde av typen '{0}'!", type), ConsoleColor.Yellow);
                                }
                            }
                        }
                        else
                        {
                            if (GetConfirmation("Vill du återvända till huvudmenyn utan att skapa ett fordon? (J/N)"))
                            {
                                AbortedMessage();
                                return false;
                            }
                        }
                    } while (!done);
                }
            }

            veh.Registration = veh.Registration.ToUpper();
            if (veh.Color.Length > 0)
            {
                veh.Color = veh.Color.Substring(0, 1).ToUpper() + veh.Color.Substring(1).ToLower();
            }

            return true;
        }

        static bool SearchVehicles()
        {
            List<string> org = Vehicle.GetRawProperties(true);
            List<string> swe = Vehicle.GetDescribedProperties(true);
            string input;
            string searchKey;
            string searchSweKey;

            Console.WriteLine("Välj söknyckel:");
            int max = DisplayNumberedList(swe);
            AbortOptionMessage();
            int sel = GetNumber(0, max);

            if (sel == 0)
            {
                GenericMessage("Sökningen avbröts.\n", ConsoleColor.Yellow);
                return false;
            }
            else
            {
                searchKey = org.ElementAt(sel - 1);
                searchSweKey = swe.ElementAt(sel - 1);
                Console.WriteLine();
            }

            do
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Söknyckel: {0} ", searchSweKey);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("('{0}')", searchKey);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Skriv in ett värde att söka efter eller '-' för att söka efter fordon som saknar värdet; alt. tryck Enter för att avbryta.");
                input = GetInput();
                if (input == "")
                {
                    if (GetConfirmation("Vill du avbryta sökningen? (J/N)"))
                    {
                        GenericMessage("Sökningen avbröts.\n", ConsoleColor.Yellow);
                        return false;
                    }
                }
            } while (input == "");

            string searchValue = input.ToUpper();

            Console.WriteLine();

            List<Vehicle> searchGroup = new List<Vehicle>();

            foreach (var veh in garage)
            {
                var specialCase = true;
                var props = veh.GetType().GetProperties();
                var query = props.Where(p => p.Name == searchKey)
                                 .Where(p => p.GetValue(veh, null) != null)
                                 .Select(p => p.GetValue(veh, null).ToString());

                if (query != null)
                {
                    var temp = query.Take(1);
                    if (temp.Count() > 0)
                    {
                        specialCase = false;

                        if (temp.First().ToUpper().Contains(searchValue))
                        {
                            searchGroup.Add(veh);
                        }
                    }
                }

                if (specialCase && searchValue == "-")
                {
                    searchGroup.Add(veh);
                }
            }

            if (searchGroup.Count() > 0)
            {
                Console.WriteLine("Sökresultat för '{0}':\n", searchValue);
                DisplayVehicleInfo(searchGroup, false, searchKey);
            }
            else
            {
                GenericMessage(string.Format("Inga resultat hittades för '{0}'!\n", searchValue), ConsoleColor.Yellow);
            }

            return true;
        }
    }
}
