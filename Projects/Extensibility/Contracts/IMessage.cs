using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensibility.Contracts
{
    public interface IMessage
    {
        string Channel { get; }
        IUser User { get; }
        string Message { get; }
    }
}
