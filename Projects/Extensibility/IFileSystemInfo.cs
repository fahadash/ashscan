using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensibility
{
    using Mono.Addins;

    [TypeExtensionPoint]
    public interface IFileSystemInfo
    {
        string DataPath { get; }
    }
}
