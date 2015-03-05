using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSChecker
{
    public class AggregateChecker
    {
        private IExploitChecker[] checkers;
        public AggregateChecker(params IExploitChecker[] checkers)
        {
            this.checkers = checkers;
        }

        public IEnumerable<ExploitType> Check(string address)
        {
            return checkers.Aggregate(Enumerable.Empty<ExploitType>(), (a, c) => a.Concat(c.Check(address)));
        }

    }
}
