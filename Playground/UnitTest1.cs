using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Playground
{
    using System.Collections.Generic;

    using ExploitChecker;

    using ExploitFinder;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GenericExploitFinderTest()
        {
            var finder = new DnsExploitFinder();
            finder.HostSuffix = "dnsbl.dronebl.org";
            finder.ProviderName = "DroneBl";
            finder.ExploitDictionary = new Dictionary<int, Exploit>();

            finder.ExploitDictionary.Add(3, new Exploit(ExploitType.IRCDrone));
            finder.ExploitDictionary.Add(5, new Exploit(ExploitType.Bottler));
            finder.ExploitDictionary.Add(6, new Exploit(ExploitType.UnknownSpambotOrDrone));
            finder.ExploitDictionary.Add(7, new Exploit(ExploitType.DDOSDrone));
            finder.ExploitDictionary.Add(8, new Exploit(ExploitType.SocksProxy));
            finder.ExploitDictionary.Add(9, new Exploit(ExploitType.HTTPProxy));
            finder.ExploitDictionary.Add(10, new Exploit(ExploitType.ProxyChain));
            finder.ExploitDictionary.Add(13, new Exploit(ExploitType.BruteForceAttacker));
            finder.ExploitDictionary.Add(14, new Exploit(ExploitType.OpenWingateProxy));
            finder.ExploitDictionary.Add(15, new Exploit(ExploitType.CompromisedRouterGateway));
            finder.ExploitDictionary.Add(17, new Exploit(ExploitType.AutomaticallyDeterminedBotnet));

            var results = finder.Run("93.93.193.150");

            Assert.IsTrue(results != null);
        }

        [TestMethod]
        public void MonoAddinsExploitFinderTest()
        {
            var finder = new AggregateHostExploitFinder();

            var results = finder.Run("us1x.mullvad.net");


            Assert.IsTrue(results != null);
        }
    }
}
