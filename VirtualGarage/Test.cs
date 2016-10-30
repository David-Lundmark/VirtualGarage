using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarage
{
    class Test
    {
        public static void TestProps()
        {
            var subset = Vehicle.GetDescribedProperties();

            foreach (var item in subset)
            {
                if (item != null)
                {
                    Console.WriteLine(item);
                }
            }
        }

        public static void TestReg(Garage<Vehicle> g)
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
