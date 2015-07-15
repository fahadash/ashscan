using Extensibility;
using Mono.Addins;
using STSdb4.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ashscan.StorageProvider.StsDb
{
    [Extension(typeof(IStorageProvider))]
    public class Provider : IStorageProvider, IDisposable
    {
        private string dataFilePath = string.Empty;
        bool initialized = false;
            IStorageEngine engine;
            ITable<string, string> table;
            bool open = false;
        public Provider()
        {
            var fileSystem = ExtensionManager.Get<IFileSystemInfo>();

            string fileName;

            if (fileSystem != null)
            {
                dataFilePath = fileSystem.DataPath;

                if (Directory.Exists(dataFilePath))
                {
                    fileName = Path.Combine(dataFilePath, "ashscan.stsdb.store");
                    engine = STSdb.FromFile(fileName + ".db");                    
                    initialized = true;
                }                
            }
        }

        public void Store(string key, string data)
        {
            if (open)
            {
                table.InsertOrIgnore(key, data);
            }
        }

        public string Retrieve(string key)
        {
            if (open)
            {
                return table.TryGetOrDefault(key, null);
            }

            return null;
        }

        public bool Open(string storeName)
        {
            table = engine.OpenXTablePortable<string, string>(storeName);

            if (table == null)
            {
                return false;
            }

            open = true;
            return true;
        }

        public void Dispose()
        {
            if (open)
            {
                engine.Close();
                open = false;
            }
        }


        public bool Remove(string key)
        {
            if (open)
            {
                if (table.Exists(key))
                {
                    table.Delete(key);
                    return true;
                }
            }

            return false;
        }

        public bool Exists(string key)
        {
            return open && table.Exists(key);
        }
    }
}
