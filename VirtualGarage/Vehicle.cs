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

        [PropertyDescription("Antal passagerare")]
        public int NumPassengers { get; set; }

        [PropertyDescription("Färg")]
        public string Color { get; set; }

        [PropertyDescription("Registeringsnummer")]
        public string Registration { get; set; }

        [PropertyDescription("Typ")]
        public string Type { get { return GetType().GetProperty("_type", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null, null).ToString(); } }
        //public string Type { get { return _type; } }

        public Vehicle()
        {
            ID = Guid.NewGuid();
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

                //_descprops.InsertRange(0, query.ToList());
                _descprops.AddRange(query.ToList());
            }

            _descprops = _descprops.Distinct().Reverse().ToList();

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

                //_rawprops.InsertRange(0, query.ToList());
                _rawprops.AddRange(query.ToList());
            }

            _rawprops = _rawprops.Distinct().Reverse().ToList();

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

        public WheeledVehicle() : base()
        {
        }
    }

    abstract class PoweredVehicle : Vehicle
    {
        [PropertyDescription("Antal motorer")]
        public int NumEngines { get; set; }

        public PoweredVehicle() : base()
        {
        }
    }

    abstract class WheeledPoweredVehicle : WheeledVehicle
    {
        [PropertyDescription("Antal motorer")]
        public int NumEngines { get; set; }

        public WheeledPoweredVehicle() : base()
        {
        }
    }

    class Airplane : WheeledPoweredVehicle
    {
        protected static new string _type => "Flygplan";

        [PropertyDescription("Lastutrymme (m³)")]
        public int CargoSpace { get; set; }

        public Airplane() : base()
        {
        }

        public static Airplane Create()
        {
            return new Airplane();
        }
    }

    class Motorcycle : WheeledPoweredVehicle
    {
        protected static new string _type => "Motorcykel";

        public Motorcycle() : base()
        {
        }

        public static Motorcycle Create()
        {
            return new Motorcycle();
        }
    }

    class Car : WheeledPoweredVehicle
    {
        protected static new string _type => "Bil";

        [PropertyDescription("Lastutrymme (m³)")]
        public int CargoSpace { get; set; }

        public Car() : base()
        {
        }

        public static Car Create()
        {
            return new Car();
        }
    }

    class Bus : WheeledPoweredVehicle
    {
        protected static new string _type => "Buss";

        [PropertyDescription("Lastutrymme (m³)")]
        public int CargoSpace { get; set; }

        public Bus() : base()
        {
        }

        public static Bus Create()
        {
            return new Bus();
        }
    }

    class Motorboat : PoweredVehicle
    {
        protected static new string _type => "Motorbåt";

        [PropertyDescription("Lastutrymme (m³)")]
        public int CargoSpace { get; set; }

        public Motorboat() : base()
        {
        }

        public static Motorboat Create()
        {
            return new Motorboat();
        }
    }

    class Sailboat : Vehicle
    {
        protected static new string _type => "Segelbåt";

        [PropertyDescription("Lastutrymme (m³)")]
        public int CargoSpace { get; set; }

        public Sailboat() : base()
        {
        }

        public static Sailboat Create()
        {
            return new Sailboat();
        }
    }
}
