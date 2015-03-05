using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSChecker
{
    using System.Net;
    using System.Net.Sockets;

    public class BarracudaChecker : IExploitChecker
    {
        public IEnumerable<ExploitType> Check(string address)
        {
            var addresses = new List<IPAddress>();

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
                    "{0}.{1}.{2}.{3}.b.barracudacentral.org",
                    Convert.ToInt32(bytes[3]),
                    Convert.ToInt32(bytes[2]),
                    Convert.ToInt32(bytes[1]),
                    Convert.ToInt32(bytes[0]));

                try
                {
                    var dnsquery = Dns.GetHostAddresses(hostToTest);
                    var resultCodes =
                        dnsquery.Select(x => Convert.ToInt32(x.GetAddressBytes()[3]))
                            .Where(x => x == 2)
                            .Select(x => ExploitType.Barracuda_Blacklisted);

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
