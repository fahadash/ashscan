using Mono.Addins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensibility
{
    [TypeExtensionPoint]
    public interface IStorageProvider
    {
        bool Open(string storeName);
        void Store(string key, string data);

        string Retrieve(string key);

        bool Remove(string key);

        bool Exists(string key);
    }
}
