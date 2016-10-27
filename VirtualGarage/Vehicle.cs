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
            ID = Guid.NewGuid();
        }

        public Vehicle(string registration, string color, int numwheels, int numpassengers) : this()
        {
            Registration = registration;
            Color = color;
            NumWheels = numwheels;
            NumPassengers = numpassengers;
        }

        public virtual string GetVehicleType()
        {
            throw new NotSupportedException();
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
    }

    class Airplane : Vehicle
    {
        [Description("Antal motorer")]
        public int NumEngines { get; private set; }

        public Airplane(string registration, string color, int numwheels, int numpassengers, int numengines) : base(registration, color, numwheels, numpassengers)
        {
            NumEngines = numengines;
        }

        public override string GetVehicleType()
        {
            return "Flygplan";
        }
    }

    class Motorcycle : Vehicle
    {
        public Motorcycle(string registration, string color, int numwheels, int numpassengers) : base(registration, color, numwheels, numpassengers)
        {
        }

        public override string GetVehicleType()
        {
            return "Motorcykel";
        }
    }

    class Car : Vehicle
    {
        public Car(string registration, string color, int numwheels, int numpassengers) : base(registration, color, numwheels, numpassengers)
        {
        }

        public override string GetVehicleType()
        {
            return "Bil";
        }
    }

    class Bus : Vehicle
    {
        public Bus(string registration, string color, int numwheels, int numpassengers) : base(registration, color, numwheels, numpassengers)
        {
        }

        public override string GetVehicleType()
        {
            return "Buss";
        }
    }

    class Boat : Vehicle
    {
        public Boat(string registration, string color, int numwheels, int numpassengers) : base(registration, color, numwheels, numpassengers)
        {
        }

        public override string GetVehicleType()
        {
            return "Båt";
        }
    }
}
