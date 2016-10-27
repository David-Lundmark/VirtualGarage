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

        public static void Test2(Garage<Vehicle> g, Type t)
        {
            var subset = g.GetVehiclesByType(t);

            foreach (var item in subset)
            {
                if (item != null)
                {
                    Console.WriteLine(item.GetVehicleType());
                }
            }
        }

        public static void Test3(Vehicle v)
        {
            var subset = v.GetDescribedProperties();

            foreach (var item in subset)
            {
                if (item != null)
                {
                    Console.WriteLine(item);
                }
            }
        }

        public static void Test4(Garage<Vehicle> g)
        {
            var sort = g.SortByProperty("Registration", false);

            foreach (var item in sort)
            {
                if (item != null)
                {
                    Console.WriteLine("{0} ({1})", item, item.Registration);
                }
            }
        }
    }
}
