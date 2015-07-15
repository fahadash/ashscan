using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensibility
{

    using Extensibility.Contracts;
    using Mono.Addins;

    [TypeExtensionPoint]
    public interface IIrcController
    {
        IObservable<Tuple<string, IUserInfo>> Joins { get; }
        IObservable<Tuple<string, IUserInfo>> Parts { get; }

        IObservable<Tuple<string, IUserInfo>> Quits { get; }
        IObservable<IMessage> ChannelMessage { get; }

        IObservable<IMessage> PrivateMessage { get; }

        string MyNick { get; }

        void Kick(string channel, string nick, string reason);

        void Ban(string channel, string mask);
        void Unban(string channel, string mask);

        void Voice(string channel, string nick);

        void Devoice(string channel, string nick);
        void Op(string channel, string nick);

        void Deop(string channel, string nick);
        void Topic(string channel, string topic);

        void Join(string channel);

        void Part(string channel);

        void Say(string target, string message);

        void Notice(string target, string message);
        bool AmIOp(string channel);
    }
}
