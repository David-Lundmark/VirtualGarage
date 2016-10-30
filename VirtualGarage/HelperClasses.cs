using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualGarage
{
    class HelperMethods
    {
        public static IEnumerable<Type> GetDerivedConcreteClasses(Type what)
        {
            var subclasses = from assembly in AppDomain.CurrentDomain.GetAssemblies()
                             from type in assembly.GetTypes()
                             where type.IsSubclassOf(what)
                             where !type.IsAbstract
                             select type;

            return subclasses;
        }
    }
}
