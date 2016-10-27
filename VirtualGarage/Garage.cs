using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarage
{
    class Garage<T> : IEnumerable<T> where T : Vehicle
    {
        List<Vehicle> storage;

        public Garage(int limit = int.MaxValue)
        {
            storage = new List<Vehicle>(limit);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < storage.Count; i++)
            {
                yield return storage[i] as T;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return storage.GetEnumerator();
        }

        public bool Add(Vehicle v)
        {
            if (storage.Count == storage.Capacity)
            {
                return false;
            }

            storage.Add(v);
            return true;
        }

        public bool Remove(Vehicle v)
        {
            if (!storage.Contains(v))
            {
                return false;
            }

            storage.Remove(v);
            return true;
        }

        public List<Vehicle> GetVehiclesByType(Type type)
        {
            var query = storage.Where(v => v.GetType() == type).Select(v => v);

            return query.ToList();
        }
    }
}
