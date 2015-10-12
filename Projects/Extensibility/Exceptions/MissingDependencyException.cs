using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensibility.Exceptions
{
    public class MissingDependencyException : Exception
    {
        string[] names;
        public MissingDependencyException(string[] names)
        {
            this.names = names;
        }

        public override string Message
        {
            get
            {
                return "Missing dependencies: " + string.Join(", ", names);
            }
        }

        public override string ToString()
        {
            return "Missing dependencies: " + string.Join(", ", names);
        }
    }
}
