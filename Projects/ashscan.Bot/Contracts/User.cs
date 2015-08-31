using ChatSharp;
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

        public User(IrcUser user)
        {
            this.Mask = string.Format("{0}!{1}@{2}", user.Nick, user.User, user.Hostname);

            this.Nick = user.Nick;
            this.Username = user.User;
            this.Host = user.Hostname;
        }

        public User()
        {

        }
    }
}
