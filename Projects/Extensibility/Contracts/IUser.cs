using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensibility.Contracts
{
    public interface IUser
    {
        string Nick { get; }
        string Username { get; }

        string Host { get; }

        string Mask { get; }
    }
}
