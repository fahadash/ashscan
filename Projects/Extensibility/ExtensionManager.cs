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
    using Extensibility.Attributes;
    using Extensibility.Exceptions;

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
                var missing = FillDependencies(bootstarpper);

                if (missing.Count() == 0)
                {
                    if (bootstarpper.Run() == false)
                    {
                        // Report failure
                    }
                }
                else
                {
                    // report failure: dependencies could not be loaded
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

        public static TObj Get<TObj>() where TObj : class
        {

            var obj = AddinManager.GetExtensionObjects<TObj>(true).FirstOrDefault();

            var missing = FillDependencies(obj);

            if (missing.Count() != 0)
            {
                throw new MissingDependencyException(missing);
            }

            return obj;
        }
        public static void Initialize()
        {

        }

        private static string[] FillDependencies(object obj)
        {
            var type =  obj.GetType();
            var properties = type
                                .GetProperties()
                                .Select(p => new
                                {
                                    Property = p,
                                    Attribute = (DependencyAttribute)Attribute.GetCustomAttribute(p, typeof(DependencyAttribute))
                                })
                                .Where(p => p.Attribute != null)
                                .Select(p => new { Property = p.Property, Value = AddinManager.GetExtensionObjects(p.Attribute.Type, 
                                    // Poor man's context sharing query
                                    !type.Name.Equals("IStorageProvider")
                                    ).FirstOrDefault() })
                                .ToArray();

            var missing = properties
                            .Where(p => p.Value == null)
                            .Select(p => p.Property.Name)
                            .ToArray();

            foreach (var dependency in properties)
            {
                dependency.Property.SetValue(obj, dependency.Value);
            }

            return missing;
        }
    }
}
