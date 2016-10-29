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
    class Vehicle
    {
        public Guid ID { get; private set; }

        static List<string> _validtypes;

        [Description("Typ")]
        public static string Type { get; protected set; } = "*FEL*";

        [Description("Registeringsnummer")]
        public string Registration { get; private set; }

        [Description("Färg")]
        public string Color { get; private set; }

        [Description("Antal hjul")]
        public int NumWheels { get; private set; }

        [Description("Antal passagerare")]
        public int NumPassengers { get; private set; }

        public Vehicle()
        {
            if (_validtypes == null)
            {
                PopulateValidTypeList();
            }

            ID = Guid.NewGuid();
        }

        public Vehicle(string registration, string color, int numwheels, int numpassengers) : this()
        {
            Registration = registration;
            Color = color;
            NumWheels = numwheels;
            NumPassengers = numpassengers;
        }

        /// <summary>
        /// Extracts the description from all public properties in this and any inherited class
        /// </summary>
        /// <returns></returns>
        public List<string> GetDescribedProperties()
        {
            var query = this.GetType().GetProperties()
                        .Where(p => p.GetCustomAttribute(typeof(DescriptionAttribute), true) != null)
                        .Select(s => ((DescriptionAttribute)s.GetCustomAttributes(typeof(DescriptionAttribute), true)[0]).Description);

            return query.ToList();
        }

        private void PopulateValidTypeList()
        {
            _validtypes = new List<string>();

            var subclasses = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.IsSubclassOf(typeof(Vehicle))
                             select type;

            foreach (var item in subclasses)
            {
                var val = item.GetProperty("Type").GetValue(this, null);
                _validtypes.Add(val.ToString());
            }
        }

        public static List<string> GetValidTypes()
        {
            return _validtypes;
        }
    }

    class Airplane : Vehicle
    {
        [Description("Typ")]
        public static new string Type { get; protected set; } = "Flygplan";

        [Description("Antal motorer")]
        public int NumEngines { get; private set; }

        public Airplane(string registration, string color, int numwheels, int numpassengers, int numengines) : base(registration, color, numwheels, numpassengers)
        {
            NumEngines = numengines;
        }
    }

    class Motorcycle : Vehicle
    {
        [Description("Typ")]
        public static new string Type { get; protected set; } = "Motorcykel";

        public Motorcycle(string registration, string color, int numwheels, int numpassengers) : base(registration, color, numwheels, numpassengers)
        {
        }
    }

    class Car : Vehicle
    {
        [Description("Typ")]
        public static new string Type { get; protected set; } = "Bil";

        public Car(string registration, string color, int numwheels, int numpassengers) : base(registration, color, numwheels, numpassengers)
        {
        }
    }

    class Bus : Vehicle
    {
        [Description("Typ")]
        public static new string Type { get; protected set; } = "Buss";

        public Bus(string registration, string color, int numwheels, int numpassengers) : base(registration, color, numwheels, numpassengers)
        {
        }
    }

    class Boat : Vehicle
    {
        [Description("Typ")]
        public static new string Type { get; protected set; } = "Båt";

        public Boat(string registration, string color, int numwheels, int numpassengers) : base(registration, color, numwheels, numpassengers)
        {
        }
    }
}
