using Extensibility.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ashscan.Bot.Contracts
{
    public class User : IUser
    {
        public string Nick
        {
            get;
            set;
        }

        public string Username
        {
            get;
            set;
        }

        public string Host
        {
            get;
            set;
        }

        public string Mask
        {
            get;
            set;
        }
    }
}
