using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarage
{
    class Program
    {
        static void Main(string[] args)
        {
            Garage<Vehicle> gar = new Garage<Vehicle>(5);

            Console.WriteLine(gar.Add(new Car()));
            Console.WriteLine(gar.Add(new Boat()));
            Console.WriteLine(gar.Add(new Airplane()));
            Console.WriteLine(gar.Add(new Boat()));
            Console.WriteLine(gar.Add(new Car()));
            Console.WriteLine(gar.Add(new Airplane()));

            Test.Test1(gar);
            Test.Test2(gar);

            Console.ReadKey();
        }
    }
}
