using Extensibility;
using Mono.Addins;
using STSdb4.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ashscan.StorageProvider.StsDb
{
    [Extension(typeof(IStorageProvider))]
    public class Provider : IStorageProvider, IDisposable
    {
        private static string dataFilePath = string.Empty;
        bool initialized = false;
        static IStorageEngine engine;
        ITable<string, string> table;
        bool open = false;
        int refCount = 0;

        static Provider()
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
                }
            }
        }

        public Provider()
        {
            Interlocked.Increment(ref refCount);
        }

        public void Store(string key, string data)
        {
            if (open)
            {
                table.InsertOrIgnore(key, data);
                engine.Commit();
            }
        }

        public IDictionary<string, string> Search(Func<KeyValuePair<string, string>, bool> query)
        {
            return table.Where(query).ToDictionary(q => q.Key, q => q.Value);
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
            Interlocked.Decrement(ref refCount);
            if (open && refCount <= 0)
            {
                open = false;
                engine.Close();
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
