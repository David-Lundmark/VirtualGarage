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
            UI.Init();
        }

        /*
        static void TestInit(Garage<Vehicle> gar)
        {
            gar.Add(new Airplane("HGF564", "Vit", 2, 3, 2));
            gar.Add(new Car("KSY809", "Blå", 5, 4));
            gar.Add(new Boat("PPI136", "Röd", 5));
            gar.Add(new Airplane("UGB741", "Grå", 8, 3, 2));
            gar.Add(new Boat("HAE323", "Gul", 6));
            gar.Add(new Bus("YUK615", "Svart", 40, 4));

            //Test.TestProps();
            //Test.TestReg(gar);
        }
        */
    }
}
