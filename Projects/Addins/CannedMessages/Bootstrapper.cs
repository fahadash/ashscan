using Extensibility;
using Extensibility.Contracts;
using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CannedMessages
{
    [Extension]
    public class Bootstrapper : IAddinBootstrapper
    {
        internal static Random rnd = new Random();
        internal static IIrcController controller = null;
        internal static IStorageProvider storage = null;
        List<string> ignoreList = null;
        const string ignoreListKey="__/cannedmessages_ignorelist";
        public Bootstrapper()
        {
        }

        private void SaveIgnoreList()
        {
            var ignore = string.Join(",", ignoreList);

            storage.Store(ignoreListKey, ignore);
        }
        public bool Run()
        {
            controller = ExtensionManager.GetController();
            storage = ExtensionManager.Get<IStorageProvider>();
            ignoreList = (storage.Retrieve(ignoreListKey) ?? string.Empty)
                            .Split(new [] {','}, StringSplitOptions.RemoveEmptyEntries)
                            .ToList();
            controller
                .ChannelMessage
                // We need to find a way to not ignore the Bot Operators
                .Where(u => u.User.Nick.Equals("fahadash", StringComparison.OrdinalIgnoreCase) || !IsIgnored(u.User))
                .Select(m => new Tuple<IMessage, string[]>(m, m.Message.Split(new [] {' '}, StringSplitOptions.RemoveEmptyEntries)))
                .Where(s => s.Item2.Length > 1 && s.Item2.First().Equals(".can", StringComparison.OrdinalIgnoreCase))
                .Subscribe(tokens =>
                {
                    var all = tokens.Item2;
                    if (all[1].Equals("add", StringComparison.OrdinalIgnoreCase) && all.Count() > 3)
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
                    else if (all[1].Equals("modify", StringComparison.OrdinalIgnoreCase) && all.Count() > 3)
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
                    else if (all[1].Equals("delete", StringComparison.OrdinalIgnoreCase) && all.Count() > 2)
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
                    else if (all[1].Equals("ignore", StringComparison.OrdinalIgnoreCase) && all.Count() > 2)
                    {
                        var user = tokens.Item1.User.Nick;
                        var mask =  all[2];

                        // Refine this
                        if (!mask.Contains("!"))
                        {
                            mask = mask + "!*@*";
                        }
                        if (user.Equals("fahadash", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!ignoreList.Contains(mask))
                            {
                                ignoreList.Add(mask);
                                SaveIgnoreList();
                                controller.Notice(tokens.Item1.User.Nick, string.Format("Added {0} to canned ignore list", mask));
                            }
                            else
                            {
                                controller.Notice(tokens.Item1.User.Nick, string.Format("Mask {0} is already ignored", mask));
                            }
                        }                        
                    }
                    else if (all[1].Equals("unignore", StringComparison.OrdinalIgnoreCase) && all.Count() > 2)
                    {
                        var user = tokens.Item1.User.Nick;
                        var mask =  all[2];

                        // Refine this
                        if (!mask.Contains("!"))
                        {
                            mask = mask + "!*@*";
                        }
                        if (user.Equals("fahadash", StringComparison.OrdinalIgnoreCase))
                        {
                            if (ignoreList.Contains(mask))
                            {
                                ignoreList.Remove(mask);
                                SaveIgnoreList();
                                controller.Notice(tokens.Item1.User.Nick, string.Format("Removed {0} to canned ignore list", mask));
                            }
                            else
                            {
                                controller.Notice(tokens.Item1.User.Nick, string.Format("Mask {0} is not ignored", mask));
                            }
                        }                        
                    }
                    else if (all[1].Equals("wipeignore", StringComparison.OrdinalIgnoreCase))
                    {
                        var user = tokens.Item1.User.Nick;

                        if (user.Equals("fahadash", StringComparison.OrdinalIgnoreCase))
                        {
                            if (ignoreList.Any())
                            {
                                ignoreList.Clear();
                                SaveIgnoreList();
                                controller.Notice(tokens.Item1.User.Nick, "Ignore list wiped");
                            }
                            else
                            {
                                controller.Notice(tokens.Item1.User.Nick, "Ignore list already empty");
                            }
                        }                        
                    }
                    else if (all[1].Equals("rand", StringComparison.OrdinalIgnoreCase) && all.Length >= 3)
                    {
                        var search = tokens.Item1.User.Nick + "_" + all[2];
                        var nick = string.Empty;

                        if (all.Length > 3)
                        {
                            nick = all[3];
                        }

                        var regex = WildCardToRegular(search);

                        var results = storage.Search(p => Regex.IsMatch(p.Key, regex));
                        var total  = results.Count;

                        if (total == 0)
                        {
                            controller.Notice(tokens.Item1.User.Nick, "Nothing found in your search query. You may have to add your own canned messages.");                           
                        }
                        else
                        {
                            var values = results.Values.ToList();

                            var message = values[rnd.Next(total)];
                            
                            if (!string.IsNullOrEmpty(nick))
                            {
                                message = string.Format("{0}, {1}", nick, message);
                            }
                            
                            controller.Say(tokens.Item1.Channel, message);
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
                            controller.Notice(tokens.Item1.User.Nick, "Key not found, You may have to add your own canned messages.");
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
                            controller.Notice(tokens.Item1.User.Nick, "Key not found, You may have to add your own canned messages.");
                        }
                    }
                  });

            return storage.Open("cannedmessages_01");
        }

        private bool IsIgnored(IUser user)
        {
            return ignoreList.Any(m => Regex.IsMatch(user.Mask, WildCardToRegular(m)));
        }

          // If you want to implement both "*" and "?"
        private static String WildCardToRegular(String value) {
        return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$"; 
        }
    }
}
