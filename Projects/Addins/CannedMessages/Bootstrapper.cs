using Extensibility;
using Extensibility.Contracts;
using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CannedMessages
{
    [Extension]
    public class Bootstrapper : IAddinBootstrapper
    {
        internal static IIrcController controller = null;

        public Bootstrapper()
        {
            controller = ExtensionManager.GetController();
        }
        public bool Run()
        {

            controller
                .ChannelMessage
                .Select(m => new Tuple<IMessage, string[]>(m, m.Message.Split(' ')))
                .Where(s => s.Item2.Length > 0 && s.Item2.First().StartsWith("."))
                .Subscribe(tokens =>
                {
                    controller.Say(tokens.Item1.Channel, string.Format("Hey {0}, I received your commands. One day I will respond to that", tokens.Item1.User.Nick));
                });

            return true;
        }
    }
}
