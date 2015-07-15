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
        internal static IStorageProvider storage = null;

        public Bootstrapper()
        {
        }
        public bool Run()
        {
            controller = ExtensionManager.GetController();
            storage = ExtensionManager.Get<IStorageProvider>();

            controller
                .ChannelMessage
                .Select(m => new Tuple<IMessage, string[]>(m, m.Message.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries)))
                .Where(s => s.Item2.Length > 1 && s.Item2.First().Equals(".can", StringComparison.OrdinalIgnoreCase))
                .Subscribe(tokens =>
                {
                    var all = tokens.Item2;
                    if (all[1].Equals("add", StringComparison.OrdinalIgnoreCase))
                    {
                        var message = string.Join(" ", all.Skip(3));
                        var key = tokens.Item1.User.Nick + "_" + all[2];
                        if (!storage.Exists(key))
                        {
                            storage.Store(key, message);
                            controller.Notice(tokens.Item1.User.Nick, "Canned message added");
                        }
                        else 
                        {
                            controller.Notice(tokens.Item1.User.Nick, "Key already exists");
                        }
                    }
                    if (all[1].Equals("modify", StringComparison.OrdinalIgnoreCase))
                    {
                        var message = string.Join(" ", all.Skip(3));
                        var key = tokens.Item1.User.Nick + "_" + all[2];
                        if (storage.Exists(key))
                        {
                            storage.Remove(key);
                            storage.Store(key, message);
                            controller.Notice(tokens.Item1.User.Nick, "Canned message modified");
                        }
                        else
                        {
                            controller.Notice(tokens.Item1.User.Nick, "Key does not exist");
                        }
                    }
                    if (all[1].Equals("delete", StringComparison.OrdinalIgnoreCase))
                    {
                        var key = tokens.Item1.User.Nick + "_" + all[2];
                        if (storage.Exists(key))
                        {
                            storage.Remove(key);
                            controller.Notice(tokens.Item1.User.Nick, "Canned message deleted");
                        }
                        else
                        {
                            controller.Notice(tokens.Item1.User.Nick, "Key does not exist");
                        }
                    }
                    else if (all.Length == 2)
                    {
                        var key = tokens.Item1.User.Nick + "_" + all[1];

                        if (storage.Exists(key))
                        {
                            var message = storage.Retrieve(key);
                            controller.Say(tokens.Item1.Channel, message);
                        }
                        else
                        {
                            controller.Notice(tokens.Item1.User.Nick, "Key not found");
                        }
                    }
                    else if (all.Length == 3)
                    {
                        var key = tokens.Item1.User.Nick + "_" + all[1];

                        if (storage.Exists(key))
                        {
                            var message = storage.Retrieve(key);
                            controller.Say(tokens.Item1.Channel, string.Format("{0}, {1}", all[2], message));
                        }
                        else
                        {
                            controller.Notice(tokens.Item1.User.Nick, "Key not found");
                        }
                    }
                  });

            return storage.Open("cannedmessages_01");
        }
    }
}
