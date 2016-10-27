﻿using System;
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
            Garage<Vehicle> gar = new Garage<Vehicle>(5);

            TestInit(gar);

            UI.Init(gar);

            Console.ReadKey();
        }

        static void TestInit(Garage<Vehicle> gar)
        {
            Console.WriteLine(gar.Add(new Airplane("HGF564", "Vit", 3, 2, 2)));
            Console.WriteLine(gar.Add(new Car("KSY809", "Blå", 4, 5)));
            Console.WriteLine(gar.Add(new Boat("PPI136", "Röd", 0, 4)));
            Console.WriteLine(gar.Add(new Airplane("UGB741", "Grå", 3, 8, 2)));
            Console.WriteLine(gar.Add(new Boat("HAE323", "Gul", 0, 6)));
            Console.WriteLine(gar.Add(new Car("YUK615", "Svart", 4, 2)));

            Test.Test1(gar);
            Test.Test2(gar, typeof(Boat));
            Test.Test3(gar.First());
            Test.Test4(gar);
        }
    }
}
