using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensibility.Attributes
{
    public class DependencyAttribute : Attribute
    {
        public Type Type { get; set; }

        public DependencyAttribute()
        {

        }

        public DependencyAttribute(Type type)
        {
            this.Type = type;
        }
    }
}
