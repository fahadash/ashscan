using Extensibility.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ashscan.Bot.Contracts
{
    public class UserMessage : IMessage
    {

        public UserMessage()
        { 
        }

        public UserMessage(string channel, string nick, string ident, string host, string message)
        {
            this.Channel = channel;
            this.User = new User() 
            {
                Host = host,
                Username = ident,
                Nick = nick,
                Mask = string.Format("{0}!{1}@{2}", nick, ident, host)
            };
            this.Message = message;
        }
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

        public string Message
        {
            get;
            set;
        }
    }
}
