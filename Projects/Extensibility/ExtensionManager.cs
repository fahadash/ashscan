using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensibility
{
    using System.IO;
    using System.Net.Mime;
    using System.Reflection;

    using Mono.Addins;

    public class ExtensionManager
    {
        static ExtensionManager()
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);


            AddinManager.AddinLoadError += (s, e) =>
            {

            };
            AddinManager.Initialize(".", ".");
            AddinManager.Registry.Update();

            var bootstrappers = AddinManager.GetExtensionObjects<IAddinBootstrapper>();

            foreach (var bootstarpper in bootstrappers)
            {
                if (bootstarpper.Run() == false)
                {
                    // Report failure
                }
            }
        }

        public static ICommandHandler GetCommandHandler(string command)
        {
            var commands = AddinManager.GetExtensionObjects<ICommandHandler>();

            return commands.FirstOrDefault(c => c.Commands.Any(n => string.Compare(command, n, StringComparison.OrdinalIgnoreCase) == 0));
        }

        public static IIrcController GetController()
        {
            return AddinManager.GetExtensionObjects<IIrcController>(true).FirstOrDefault();
        }

        public static void Initialize()
        {

        }
    }
}
