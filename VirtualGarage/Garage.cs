using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarage
{
    class Garage<T> : IEnumerable<T> where T : Vehicle
    {
        uint capacity;

        public Garage(uint limit = uint.MaxValue)
        {
            capacity = limit;
        }
    }
}
