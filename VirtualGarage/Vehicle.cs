using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarage
{
    class Vehicle
    {
        public Guid ID { get; private set; }
        public string Registration { get; set; }
        public string Color { get; set; }
        public int NumWheels { get; set; }
        public int NumPassengers { get; set; }

        public Vehicle()
        {
            ID = new Guid();
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
    }

    class Airplane : Vehicle
    {
        public int NumEngines { get; set; }

        public override string GetVehicleType()
        {
            return "Flygplan";
        }
    }

    class Motorcycle : Vehicle
    {
        public override string GetVehicleType()
        {
            return "Motorcykel";
        }
    }

    class Car : Vehicle
    {
        public override string GetVehicleType()
        {
            return "Bil";
        }
    }

    class Bus : Vehicle
    {
        public override string GetVehicleType()
        {
            return "Buss";
        }
    }

    class Boat : Vehicle
    {
        public override string GetVehicleType()
        {
            return "Båt";
        }
    }
}
