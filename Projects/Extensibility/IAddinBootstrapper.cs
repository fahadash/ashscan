using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensibility
{
    [TypeExtensionPoint]
    public interface IAddinBootstrapper
    {
        bool Run();
    }
}
