using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarage
{
    class Test
    {
        public static void Test1(Garage<Vehicle> g)
        {
            foreach (var item in g)
            {
                if (item != null)
                {
                    Console.WriteLine(item.ToString());
                }
            }
        }

        public static void Test2(Garage<Vehicle> g)
        {
            var subset = g.GetVehiclesByType(typeof(Car));

            foreach (var item in subset)
            {
                if (item != null)
                {
                    Console.WriteLine(item.ToString());
                }
            }
        }
    }
}
