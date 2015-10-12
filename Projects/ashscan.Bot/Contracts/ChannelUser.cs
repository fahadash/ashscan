using Extensibility.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ashscan.Bot.Contracts
{
    public class ChannelUser : IChannelUser
    {
        public string Channel
        {
            get;
            set;
        }

        public IUser User
        {
            get;
            set;
        }

        public UserChannelMode[] Modes
        {
            get;
            set;
        }
    }
}
