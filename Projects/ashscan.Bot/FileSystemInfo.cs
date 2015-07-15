using Extensibility;
using Mono.Addins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ashscan.Bot
{
    [Extension]
    public class FileSystemInfo : IFileSystemInfo
    {
        private string dataPath = string.Empty;

        public FileSystemInfo()
        {
            dataPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Data");
        }
        public string DataPath
        {
            get { return dataPath; }
        }
    }
}
