using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ashscan.Bot
{
    using Extensibility;

    using Ashscan.Bot.Properties;

    using Mono.Addins;

    [Extension]
    public class CommandsHandler : ICommandHandler
    {

        private IIrcController controller = null;
        public CommandsHandler()
        {
            this.Commands = new[] { "verbose" };
            this.controller = ExtensionManager.GetController();
        }
        public IEnumerable<string> Commands { get; private set; }

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

            var command = split[0];

            if (command == "verbose")
            {
                if (split.Length == 2)
                {
                    var val = split[1].ToLower();
                    if (val != "off" && val != "on")
                    {
                        controller.Say(oper.Nick, string.Format("Invalid value: It has to be on or off"));
                    }
                    else
                    {
                        ConfigHelper.Config.BeVerbose = val != "off";
                        //Settings.Default.Save();
                        controller.Say(
                            oper.Nick,
                            string.Format("Verbose is now set to: {0}", val.ToUpper()));
                    }
                }
                else
                {
                    controller.Say(
                        oper.Nick,
                        string.Format("Verbose is: {0}", ConfigHelper.Config.BeVerbose));
                }
            }
        }
    }
}
