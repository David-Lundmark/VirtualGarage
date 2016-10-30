using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace VirtualGarage
{
    class PropertyDescriptionAttribute : DescriptionAttribute
    {
        public PropertyDescriptionAttribute(string desc) : base(desc)
        {
        }
    }
}
