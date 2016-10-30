using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarage
{
    class Program
    {
        public const string version = "Garage 1.0";

        static void Main(string[] args)
        {
            Garage<Vehicle> gar = new Garage<Vehicle>(10);
            TestInit(gar);

            UI.Init(gar);
        }

        static void TestInit(Garage<Vehicle> gar)
        {
            gar.Add(new Airplane("HGF564", "Vit", 3, 2, 2));
            gar.Add(new Car("KSY809", "Blå", 4, 5));
            gar.Add(new Boat("PPI136", "Röd", 4));
            gar.Add(new Airplane("UGB741", "Grå", 3, 8, 2));
            gar.Add(new Boat("HAE323", "Gul", 6));
            gar.Add(new Car("YUK615", "Svart", 4, 2));

            //Test.TestProps();
            //Test.TestReg(gar);
        }
    }
}
