using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ashscan.Bot
{
    using ChatSharp;

    using Extensibility;

    internal class UserInfo : IUserInfo
    {
        public UserInfo()
        {
            
        }

        public UserInfo(IrcUser user)
        {
            this.Mask = string.Format("{0}!{1}@{2}", user.Nick, user.User, user.Hostname);

            this.Nick = user.Nick;
            this.Identd = user.User;
            this.Host = user.Hostname;
            this.IsRegistered = user.Mode!=null? user.Mode.Contains("r") : false;
        }
        public string Mask { get; set; }

        public string Host { get; set; }

        public string Nick { get; set; }

        public string Identd { get; set; }

        public bool IsRegistered { get; set; }
    }
}
