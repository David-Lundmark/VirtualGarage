using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;

namespace VirtualGarage
{
    [Serializable]
    abstract class Vehicle
    {
        static List<string> _validtypes;
        static List<string> _rawprops;
        static List<string> _descprops;

        protected static string _type { get; } = "*FEL*";

        protected Guid ID { get; private set; }

        [PropertyDescription("Typ")]
        public string Type { get { return GetType().GetProperty("_type", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null).ToString(); } }

        [PropertyDescription("Registeringsnummer")]
        public string Registration { get; private set; }

        [PropertyDescription("Färg")]
        public string Color { get; private set; }

        [PropertyDescription("Antal passagerare")]
        public int NumPassengers { get; private set; }

        public Vehicle()
        {
            ID = Guid.NewGuid();
        }

        public Vehicle(string registration, string color, int numpassengers) : this()
        {
            Registration = registration;
            Color = color;
            NumPassengers = numpassengers;
        }

        /// <summary>
        /// Extracts the property descriptions of all public properties in this and any inherited class.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDescribedProperties()
        {
            if (_descprops == null)
            {
                _descprops = new List<string>();

                var subclasses = HelperMethods.GetDerivedConcreteClasses(typeof(Vehicle));

                foreach (var item in subclasses)
                {
                    var query = item.GetProperties()
                            .Where(p => p.GetCustomAttribute(typeof(PropertyDescriptionAttribute), false) != null)
                            .Select(s => ((PropertyDescriptionAttribute)s.GetCustomAttributes(typeof(PropertyDescriptionAttribute), false)[0]).Description);

                    _descprops.InsertRange(0, query.ToList());
                }

                _descprops = _descprops.Distinct().ToList();
            }

            return _descprops;
        }

        /// <summary>
        /// Extracts the names of all public properties in this and any inherited class.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRawProperties()
        {
            if (_rawprops == null)
            {
                _rawprops = new List<string>();

                var subclasses = HelperMethods.GetDerivedConcreteClasses(typeof(Vehicle));

                foreach (var item in subclasses)
                {
                    var query = item.GetProperties()
                            .Select(s => s.Name);

                    _rawprops.InsertRange(0, query.ToList());
                }

                _rawprops = _rawprops.Distinct().ToList();
            }

            return _rawprops;
        }

        /// <summary>
        /// Extracts the valid types of this and any inherited class as strings ("Bil", "Båt" etc).
        /// </summary>
        /// <returns></returns>
        public static List<string> GetValidTypes()
        {
            if (_validtypes == null)
            {
                _validtypes = new List<string>();

                var subclasses = HelperMethods.GetDerivedConcreteClasses(typeof(Vehicle));

                foreach (var item in subclasses)
                {
                    var val = item.GetProperty("_type", BindingFlags.Static | BindingFlags.NonPublic);

                    if (val != null)
                    {
                        var val2 = val.GetValue(null, null);
                        _validtypes.Add(val2.ToString());
                        //Console.WriteLine(val.ToString());
                    }
                }
            }

            return _validtypes;
        }

        public string GetTypeString()
        {
            return GetType().GetProperty("Type").GetValue(this, null).ToString();
        }
    }

    abstract class WheeledVehicle : Vehicle
    {
        [PropertyDescription("Antal hjul")]
        public int NumWheels { get; private set; }

        public WheeledVehicle(string registration, string color, int numpassengers, int numwheels) : base(registration, color, numpassengers)
        {
            NumWheels = numwheels;
        }
    }

    class Airplane : WheeledVehicle
    {
        protected static new string _type { get; } = "Flygplan";

        [PropertyDescription("Antal motorer")]
        public int NumEngines { get; private set; }

        public Airplane(string registration, string color, int numwheels, int numpassengers, int numengines) : base(registration, color, numpassengers, numwheels)
        {
            NumEngines = numengines;
        }
    }

    class Motorcycle : WheeledVehicle
    {
        protected static new string _type { get; } = "Motorcykel";

        public Motorcycle(string registration, string color, int numwheels, int numpassengers) : base(registration, color, numpassengers, numwheels)
        {
        }
    }

    class Car : WheeledVehicle
    {
        protected static new string _type { get; } = "Bil";

        public Car(string registration, string color, int numwheels, int numpassengers) : base(registration, color, numpassengers, numwheels)
        {
        }
    }

    class Bus : WheeledVehicle
    {
        protected static new string _type { get; } = "Buss";

        public Bus(string registration, string color, int numwheels, int numpassengers) : base(registration, color, numpassengers, numwheels)
        {
        }
    }

    class Boat : Vehicle
    {
        protected static new string _type { get; } = "Båt";

        public Boat(string registration, string color, int numpassengers) : base(registration, color, numpassengers)
        {
        }
    }
}
