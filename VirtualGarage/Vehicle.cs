using System;
using System.Collections;
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

        protected static string _type => "*FEL*";

        protected Guid ID { get; private set; }

        [PropertyDescription("Typ")]
        public string Type { get { return GetType().GetProperty("_type", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null).ToString(); } }
        //public string Type { get { return _type; } }

        [PropertyDescription("Registeringsnummer")]
        public string Registration { get; set; }

        [PropertyDescription("Färg")]
        public string Color { get; set; }

        [PropertyDescription("Antal passagerare")]
        public int NumPassengers { get; set; }

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
        /// Extracts the property descriptions of all public properties in any non-abstract inherited class.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDescribedProperties(bool requirePublicSetter = false)
        {
            var _descprops = new List<string>();

            var subclasses = HelperMethods.GetDerivedConcreteClasses(typeof(Vehicle));

            foreach (var item in subclasses)
            {
                var query = item.GetProperties()
                        .Where(p => p.GetCustomAttribute(typeof(PropertyDescriptionAttribute), false) != null)
                        .Where(p => !(p.SetMethod == null && requirePublicSetter == true))
                        .Select(s => ((PropertyDescriptionAttribute)s.GetCustomAttributes(typeof(PropertyDescriptionAttribute), false)[0]).Description);

                _descprops.InsertRange(0, query.ToList());
            }

            _descprops = _descprops.Distinct().ToList();

            return _descprops;
        }

        /*
        /// <summary>
        /// Extracts the property descriptions of all public properties that are valid for the class of the specified vehicle.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetDescribedPropertiesFromVehicle(Vehicle veh, bool requirePublicSetter = false)
        {
            List<string> temp = new List<string>();

            var subclasses = HelperMethods.GetDerivedConcreteClasses(typeof(Vehicle));

            foreach (var item in subclasses)
            {
                var query = item.GetType().GetProperties()
                            .Where(p => p.Name == (veh.GetType().GetProperty(p.Name) != null ? veh.GetType().GetProperty(p.Name).Name : ""))
                            .Where(p => p.GetCustomAttribute(typeof(PropertyDescriptionAttribute), false) != null)
                            .Where(p => !(p.SetMethod == null && requirePublicSetter == true))
                            .Select(s => ((PropertyDescriptionAttribute)s.GetCustomAttributes(typeof(PropertyDescriptionAttribute), false)[0]).Description);

                temp.InsertRange(0, query.ToList());
            }

            return temp.Distinct().ToList();
        }
        */

        /// <summary>
        /// Extracts the names of all public properties in any non-abstract inherited class.
        /// </summary>
        /// <returns></returns>
        public static List<string> GetRawProperties(bool requirePublicSetter = false)
        {
            var _rawprops = new List<string>();

            var subclasses = HelperMethods.GetDerivedConcreteClasses(typeof(Vehicle));

            foreach (var item in subclasses)
            {
                var query = item.GetProperties()
                            .Where(p => p.GetCustomAttribute(typeof(PropertyDescriptionAttribute), false) != null)
                            .Where(p => !(p.SetMethod == null && requirePublicSetter == true))
                            .Select(p => p.Name);

                _rawprops.InsertRange(0, query.ToList());
            }

            _rawprops = _rawprops.Distinct().ToList();

            return _rawprops;
        }

        /// <summary>
        /// Extracts the valid types of any non-abstract inherited class as strings ("Bil", "Båt" etc).
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
        public int NumWheels { get; set; }

        public WheeledVehicle(string registration, string color, int numpassengers, int numwheels) : base(registration, color, numpassengers)
        {
            NumWheels = numwheels;
        }

    }

    class Airplane : WheeledVehicle
    {
        protected static new string _type => "Flygplan";

        [PropertyDescription("Antal motorer")]
        public int NumEngines { get; set; }

        public Airplane(string registration = "", string color = "", int numpassengers = 0, int numwheels = 0, int numengines = 0) : base(registration, color, numpassengers, numwheels)
        {
            NumEngines = numengines;
        }

        public static Airplane CreateEmpty()
        {
            return new Airplane();
        }
    }

    class Motorcycle : WheeledVehicle
    {
        protected static new string _type => "Motorcykel";

        public Motorcycle(string registration = "", string color = "", int numpassengers = 0, int numwheels = 0) : base(registration, color, numpassengers, numwheels)
        {
        }

        public static Motorcycle CreateEmpty()
        {
            return new Motorcycle();
        }
    }

    class Car : WheeledVehicle
    {
        protected static new string _type => "Bil";

        public Car(string registration = "", string color = "", int numpassengers = 0, int numwheels = 0) : base(registration, color, numpassengers, numwheels)
        {
        }

        public static Car CreateEmpty()
        {
            return new Car();
        }
    }

    class Bus : WheeledVehicle
    {
        protected static new string _type => "Buss";

        public Bus(string registration = "", string color = "", int numpassengers = 0, int numwheels = 0) : base(registration, color, numpassengers, numwheels)
        {
        }

        public static Bus CreateEmpty()
        {
            return new Bus();
        }
    }

    class Boat : Vehicle
    {
        protected static new string _type => "Båt";

        public Boat(string registration = "", string color = "", int numpassengers = 0) : base(registration, color, numpassengers)
        {
        }

        public static Boat CreateEmpty()
        {
            return new Boat();
        }
    }
}
