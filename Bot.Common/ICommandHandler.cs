using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensibility
{
    using Mono.Addins;

    [TypeExtensionPoint]
    public interface ICommandHandler
    {
        IEnumerable<string> Commands { get; }

        void Handle(IUserInfo oper, IEnumerable<string> tokens);
    }
}
