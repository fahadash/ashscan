using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSChecker
{
    using CommandLine;

    public class options
    {
        [Option('i')]
        public string InputFile { get; set; }
    }
}
