using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensibility.Contracts
{
    public enum UserChannelMode
    {
        Op, Voice, Ban, Exempt, Unknown
    }
    public interface IChannelUser
    {
        string Channel { get; }

        IUser User { get; }

        UserChannelMode[] Modes { get; }

    }
}
