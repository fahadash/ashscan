using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSChecker
{
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

    using CommandLine;


    class AddressResult
    {
        public string Host { get; set; }
        public IEnumerable<ExploitType> Exploits { get; set; } 
    }
    class Program
    {
        static void Main(string[] args)
        {
            var opt = new options();
            var param = Parser.Default.ParseArguments(args, opt);

            if (string.IsNullOrWhiteSpace(opt.InputFile))
            {
                Console.WriteLine("usage: run -i <input file>");
                return;
            }

            if (!File.Exists(opt.InputFile))
            {
                Console.WriteLine("File does not exist: {0}", opt.InputFile);
                return;
            }


            IEnumerable<string> lines = null;

            using (var reader = new StreamReader(opt.InputFile))
            {
                var data = reader.ReadToEnd();

                lines = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }

            Console.WriteLine("Starting the BL queries");
            var drone = new DroneBlChcker();
            var barracuda = new BarracudaChecker();
            var torSectoor = new TorSectoorChecker();
            var efnetRbl = new EfnetRblChecker();
            var checker = new AggregateChecker(drone, barracuda, torSectoor, efnetRbl);

          //  var ipAddresses = ips.Concat(hostsConverted).Where(i => i.AddressFamily == AddressFamily.InterNetwork);
            var results = lines.Select(add => new AddressResult() { Host = add.ToString(), Exploits = checker.Check(add) }).ToList();

            var flat =
                results.SelectMany(c => c.Exploits, (e, x) => new { IP = e, Exploit = x })
                    .GroupBy(g => g.Exploit)
                    .Select(g => new { Category = g.Key.ToString(), Count = g.Count() });

            var summary = flat.Aggregate(
                new StringBuilder(string.Format("Total: \t {0}\r\nExploits: \t {1}\r\n", lines.Count(), results.Count(y => y.Exploits.Any()))),
                (s, x) => s.AppendFormat("{0} \t {1}\r\n", x.Category, x.Count));
            var detail = results.Aggregate(
                new StringBuilder(),
                (s, r) =>
                s.AppendFormat(
                    "({0}) - {1}\r\n",
                    r.Host,
                    r.Exploits.Aggregate(new StringBuilder(), (b, e) => b.AppendFormat("{0},", e.ToString()))));

            var output = Path.Combine(
                Path.GetDirectoryName(opt.InputFile),
                Path.GetFileNameWithoutExtension(opt.InputFile) +
                "_output.txt");

            using (var writer = new StreamWriter(output))
            {
                writer.Write(summary);
                writer.WriteLine();
                writer.Write(detail);
            }

            Console.WriteLine();
        }
    }
}
