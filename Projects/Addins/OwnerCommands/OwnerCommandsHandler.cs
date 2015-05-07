using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwnerCommands
{
    using Extensibility;

    using Mono.Addins;

    [Extension]
    public class OwnerCommandsHandler : ICommandHandler
    {

        private IIrcController controller = null;

        public OwnerCommandsHandler()
        {
            this.controller = ExtensionManager.GetController();
        }

        public IEnumerable<string> Commands
        {
            get
            {
                return new[] { "join", "part" };
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

                    switch (command.ToLower())
                    {
                        case "join":
                            if (split.Length == 2 && split[1].StartsWith("#"))
                            {
                                controller.Join(channel);
                            }
                            break;
                        case "part":
                            if (split.Length == 2 && split[1].StartsWith("#"))
                            {
                                controller.Part(channel);
                            }
                            break;
                    }
            }
        }
    }
}
