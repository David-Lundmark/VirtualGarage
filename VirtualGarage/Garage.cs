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
        static List<Vehicle> storage;

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

        public int Limit()
        {
            return storage.Capacity;
        }

        /// <summary>
        /// Adds a vehicle to the garage if possible.
        /// </summary>
        /// <param name="v">Vehicle</param>
        /// <returns>True if there was space, False otherwise</returns>
        public bool Add(Vehicle v)
        {
            if (storage.Count == storage.Capacity)
            {
                return false;
            }

            storage.Add(v);
            return true;
        }

        /// <summary>
        /// Removes a vehicle from the garage if possible.
        /// </summary>
        /// <param name="v">Vehicle</param>
        /// <returns>True if the vehicle existed in the garage, False otherwise</returns>
        public bool Remove(Vehicle v)
        {
            if (!storage.Contains(v))
            {
                return false;
            }

            storage.Remove(v);
            return true;
        }

        /// <summary>
        /// Returns a list of all vehicles of a given type.
        /// </summary>
        /// <param name="type">Type (typeof(Car), typeof(Airplane) etc.)</param>
        /// <returns></returns>
        public List<Vehicle> GetVehiclesByType(Type type)
        {
            var query = storage.Where(v => v.GetType() == type)
                        .Select(v => v);

            return query.ToList();
        }

        /// <summary>
        /// Returns a list of vehicles sorted by the specified property.
        /// </summary>
        /// <param name="prop">Name of the property to sort by</param>
        /// <param name="descending"></param>
        /// <returns></returns>
        public List<Vehicle> SortByProperty(string prop, bool descending)
        {
            var query = storage.Select(o => o);

            if (descending)
            {
                query = query
                        .OrderByDescending(v => v.GetType()
                        .GetProperty(prop)
                        .GetValue(v, null));
            }
            else
            {
                query = query
                        .OrderBy(v => v.GetType()
                        .GetProperty(prop)
                        .GetValue(v, null));
            }

            return query.ToList();
        }
    }
}
