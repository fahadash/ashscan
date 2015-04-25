using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperatorCommands
{
    using Extensibility;

    using Mono.Addins;

    [Extension]
    public class OperatorCommandHandler : ICommandHandler
    {

        private IIrcController controller = null;

        public OperatorCommandHandler()
        {
            this.controller = ExtensionManager.GetController();
        }

        public IEnumerable<string> Commands
        {
            get
            {
                return new[] { "op", "voice", "devoice", "deop", "kick", "ban", "kickban", "topic", "say" };
            }
        }

        public bool OperatorsOnly
        {
            get
            {
                return true;
            }
        }

        public void Handle(IUserInfo oper, IEnumerable<string> tokens)
        {
            var split = tokens.ToArray();

            if (split.Length >= 2)
            {
                var command = split[0];

                var channel = split[1];

                if (controller.AmIOp(channel))
                {
                    switch (command.ToLower())
                    {
                        case "voice":
                            if (split.Length == 3 && split[1].StartsWith("#"))
                            {
                                var nick = split[2];
                                controller.Voice(channel, nick);
                            }
                            break;
                        case "op":
                            if (split.Length == 3 && split[1].StartsWith("#"))
                            {
                                var nick = split[2];
                                controller.Op(channel, nick);
                            }
                            break;
                        case "deop":
                            if (split.Length == 3 && split[1].StartsWith("#"))
                            {
                                var nick = split[2];
                                controller.Deop(channel, nick);
                            }
                            break;
                        case "devoice":
                            if (split.Length == 3 && split[1].StartsWith("#"))
                            {
                                var nick = split[2];
                                controller.Devoice(channel, nick);
                            }
                            break;
                        case "topic":
                            if (split.Length >= 3 && split[1].StartsWith("#"))
                            {
                                var topic = string.Join(" ", split.Skip(2));
                                controller.Topic(channel, topic);
                            }
                            break;
                        case "kick":
                            if (split.Length >= 3 && split[1].StartsWith("#"))
                            {
                                var nick = split[2];
                                var reason = "Requested";

                                if (split.Length > 3)
                                {
                                    reason = string.Join(" ", split.Skip(3));
                                }
                                controller.Kick(channel, nick, reason);
                            }
                            break;
                        case "ban":
                            if (split.Length == 3 && split[1].StartsWith("#"))
                            {
                                var nick = split[2];
                                controller.Ban(channel, nick);
                            }
                            break;
                        case "kickban":
                            if (split.Length >= 3 && split[1].StartsWith("#"))
                            {
                                var nick = split[2];
                                var reason = "Requested";

                                if (split.Length > 3)
                                {
                                    reason = string.Join(" ", split.Skip(3));
                                }
                                controller.Ban(channel, nick);
                                controller.Kick(channel, nick, reason);
                            }
                            break;
                        case "say":
                            if (split.Length >= 3 && split[1].StartsWith("#"))
                            {
                                var message = string.Join(" ", split.Skip(2));
                                controller.Say(channel, message);
                            }
                            break;
                    }
                }
                else
                {
                    controller.Say(oper.Nick, "I am not Op there");
                }
            }
        }
    }

}
