using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ashscan.Bot
{
    using System.Reactive.Linq;

    using ChatSharp;
    using ChatSharp.Events;

    using Extensibility;

    using Mono.Addins;

    [Extension(typeof(IIrcController))]
    public class IrcController : IIrcController
    {
        private readonly IObservable<Tuple<string, IUserInfo>> userJoins;

        private IObservable<Tuple<IUserInfo, string>> privateMessage;

        private IrcClient client;

        public IrcController()
        {
            this.client = Service.ircClient;
            userJoins =
                Observable.FromEventPattern<ChannelUserEventArgs>(
                    a => client.UserJoinedChannel += a,
                    a => client.UserJoinedChannel -= a)
                    .Select(e => new Tuple<string, IUserInfo>(e.EventArgs.Channel.Name, new UserInfo(e.EventArgs.User)));
        }
        public IObservable<Tuple<string, IUserInfo>> Joins
        {
            get
            {
                return userJoins;
            }
        }

        public IObservable<Tuple<string, IUserInfo>> Parts
        {
            get { throw new NotImplementedException(); }
        }

        public IObservable<Tuple<string, IUserInfo>> Quits
        {
            get { throw new NotImplementedException(); }
        }

        public IObservable<Tuple<string, IUserInfo, string>> ChannelMessage
        {
            get { throw new NotImplementedException(); }
        }

        public IObservable<Tuple<IUserInfo, string>> PrivateMessage
        {
            get { throw new NotImplementedException(); }
        }

        public void Kick(string channel, string nick, string reason)
        {
            var chan = client.Channels.FirstOrDefault(c => string.Compare(c.Name, channel, StringComparison.OrdinalIgnoreCase) == 0);

            if (chan != null)
            {
                if (this.AmIOp(channel))
                {
                    chan.Kick(nick, reason);
                }
            }
        }

        public void Ban(string channel, string mask)
        {
            if ("!@.".Any(mask.Contains))
            {
                Service.rawMessageQueue.Enqueue(string.Format("MODE {0} +v {1}", channel, mask));
            }
            else
            {
                client.WhoIs
                (
                    mask,
                    whois =>
                    {
                        if (whois != null && whois.User != null && !string.IsNullOrWhiteSpace(whois.User.Hostname))
                        {
                            Service.rawMessageQueue.Enqueue(string.Format("MODE {0} +b *!*@{1}", channel, whois.User.Hostname));
                        }
                    });
            }
        }

        public void Unban(string channel, string mask)
        {
            if ("!@.".Any(mask.Contains))
            {
                Service.rawMessageQueue.Enqueue(string.Format("MODE {0} +v {1}", channel, mask));
            }
            else
            {
                client.WhoIs
                (
                    mask,
                    whois =>
                    {
                        if (whois != null && whois.User != null && !string.IsNullOrWhiteSpace(whois.User.Hostname))
                        {
                            Service.rawMessageQueue.Enqueue(string.Format("MODE {0} -b *!*@{1}", channel, whois.User.Hostname));
                        }
                    });
            }
        }

        public void Voice(string channel, string nick)
        {
            Service.rawMessageQueue.Enqueue(string.Format("MODE {0} +v {1}", channel, nick));
        }

        public void Devoice(string channel, string nick)
        {
            Service.rawMessageQueue.Enqueue(string.Format("MODE {0} -v {1}", channel, nick));
        }

        public void Say(string target, string message)
        {
            Service.rawMessageQueue.Enqueue(string.Format("PRIVMSG {0} :{1}", target, message));
        }


        public string MyNick
        {
            get { return client.User.Nick; }
        }

        public bool AmIOp(string channel)
        {
            var chan =
                client.Channels.FirstOrDefault(
                    c => string.Compare(c.Name, channel, StringComparison.OrdinalIgnoreCase) == 0);
            return (chan != null && chan.UsersByMode.ContainsKey('o')
                    && chan.UsersByMode['o'].Any(
                        u => string.Compare(u.Nick, MyNick, StringComparison.OrdinalIgnoreCase) == 0));
        }


        public void Op(string channel, string nick)
        {
            Service.rawMessageQueue.Enqueue(string.Format("MODE {0} +o {1}", channel, nick));
        }

        public void Deop(string channel, string nick)
        {
            Service.rawMessageQueue.Enqueue(string.Format("MODE {0} -o {1}", channel, nick));
        }

        public void Topic(string channel, string topic)
        {
            Service.rawMessageQueue.Enqueue(string.Format("TOPIC {0} :{1}", channel, topic));
        }


        public void Join(string channel)
        {
            client.JoinChannel(channel);
        }

        public void Part(string channel)
        {
            client.PartChannel(channel);
        }
    }
}
