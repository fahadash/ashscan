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
    using Extensibility.Contracts;
    using Ashscan.Bot.Contracts;

    [Extension(typeof(IIrcController))]
    public class IrcController : IIrcController
    {
        private readonly IObservable<Tuple<string, IUserInfo>> userJoins;

        private IObservable<IMessage> privateMessage;
        private IObservable<IMessage> channelMessage;

        private IrcClient client;

        public IrcController()
        {
            this.client = Service.ircClient;
            userJoins =
                Observable.FromEventPattern<ChannelUserEventArgs>(
                    a => client.UserJoinedChannel += a,
                    a => client.UserJoinedChannel -= a)
                    .Select(e => new Tuple<string, IUserInfo>(e.EventArgs.Channel.Name, new UserInfo(e.EventArgs.User)));

            channelMessage =
        Observable.FromEventPattern<PrivateMessageEventArgs>(
            a => client.ChannelMessageRecieved += a,
            a => client.ChannelMessageRecieved -= a)
            .Where(m => m.EventArgs.PrivateMessage.IsChannelMessage == true)
            .Select(e => new UserMessage(e.EventArgs.PrivateMessage.Source,
                e.EventArgs.PrivateMessage.User.Nick,
                e.EventArgs.PrivateMessage.User.User,
                e.EventArgs.PrivateMessage.User.Hostname,
                e.EventArgs.PrivateMessage.Message));

            Observable.FromEventPattern<PrivateMessageEventArgs>(
            a => client.ChannelMessageRecieved += a,
            a => client.ChannelMessageRecieved -= a).Subscribe(x => System.Diagnostics.Debug.WriteLine("Message: "));

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

        
        public IObservable<IMessage> ChannelMessage
        {
            get { return channelMessage; }
        }

        public IObservable<IMessage> PrivateMessage
        {
            get { return channelMessage; }
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

        public void Notice(string target, string message)
        {
            Service.rawMessageQueue.Enqueue(string.Format("NOTICE {0} :{1}", target, message));
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


        public string ReportingChannel
        {
            get { return ConfigHelper.Config.ReportingChannel; }
        }

        public IEnumerable<IChannelUser> GetChannelUsers(string channelName)
        {
            return 
            client.Channels
                .Where(c => c.Name.Equals(channelName, StringComparison.OrdinalIgnoreCase))
                .SelectMany(c => c.Users,
                   (channel, user) => new ChannelUser()
                   {
                       Channel = channelName,
                       User = new User(user),
                       Modes = channel.UsersByMode!=null?
                                   channel.UsersByMode
                                   .Where(m => m.Value.Contains(user))
                                   .Select(m => m.Key)
                                   .Select(m =>
                                   {
                                       if (m == 'v')
                                       {
                                           return UserChannelMode.Voice;
                                       }
                                       else if (m == 'o')
                                       {
                                           return UserChannelMode.Op;
                                       }
                                       else if (m == 'e')
                                       {
                                           return UserChannelMode.Exempt;
                                       }
                                       else if (m == 'b')
                                       {
                                           return UserChannelMode.Ban;
                                       }
                                       else
                                       {
                                           return UserChannelMode.Unknown;
                                       }
                                   }).ToArray() : Enumerable.Empty<UserChannelMode>().ToArray()
                   }).ToArray();
        }
    }
}
