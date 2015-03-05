using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSChecker
{
    using System.Net;
    using System.Net.Sockets;

    public enum ExploitType
    {
        IRCDrone = 3,
        Bottler = 5,
        UnknownSpambotOrDrone = 6,
        DDOSDrone = 7,
        
        SocksProxy = 8,
        HTTPProxy = 9,
        ProxyChain = 10,
        BruteForceAttacker = 13,
        OpenWingateProxy = 14,
        CompromisedRouterGateway = 15,
        AutomaticallyDeterminedBotnet = 17,


        OpenProxy = 18,
        SpamTrap666 = 19,
        SpamTrap50=20,
        TOR = 21,
        Drones_Flooding=5,

        Barracuda_Blacklisted = 1303,
        Banlist_Match = 1304,
        TorSectoor_Blacklisted = 1305,
        Nick_Abuser = 1306,
        Long_Nick_Alert = 1307
    }
    public class DroneBlChcker : IExploitChecker
    {
        public IEnumerable<ExploitType> Check(string address)
        {
            var addresses  = new List<IPAddress>();

            IPAddress add;
            if (!IPAddress.TryParse(address, out add))
            {
                try
                {
                    var query = Dns.GetHostAddresses(address);

                    addresses.AddRange(query);
                }
                catch (Exception e)
                {
                    return Enumerable.Empty<ExploitType>();
                }
            }
            else
            {
                addresses.Add(add);
            }
            var exploits = new List<ExploitType>();

            foreach (var addr in addresses.Where(a => a.AddressFamily == AddressFamily.InterNetwork))
            {
                var bytes = addr.GetAddressBytes();

                var hostToTest = string.Format(
                    "{0}.{1}.{2}.{3}.dnsbl.dronebl.org",
                    Convert.ToInt32(bytes[3]),
                    Convert.ToInt32(bytes[2]),
                    Convert.ToInt32(bytes[1]),
                    Convert.ToInt32(bytes[0]));

                try
                {
                    var dnsquery = Dns.GetHostAddresses(hostToTest);
                    var resultCodes =
                        dnsquery.Select(x => Convert.ToInt32(x.GetAddressBytes()[3]))
                            .Where(x => Enum.IsDefined(typeof(ExploitType), x))
                            .Select(x => (ExploitType)x);

                    exploits.AddRange(resultCodes);

                }
                catch (Exception)
                {

                }
            }

            return exploits;
        }
    }
}
