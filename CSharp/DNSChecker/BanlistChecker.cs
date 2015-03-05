using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSChecker
{
    using System.IO;

    public class BanlistChecker : IExploitChecker
    {
        private List<string> banList;

        public BanlistChecker(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new InvalidOperationException("filePath cannot be null or empty");
            }

            if (!File.Exists(filePath))
            {
               throw new FileNotFoundException(filePath);
            }


            IEnumerable<string> lines = null;

            using (var reader = new StreamReader(filePath))
            {
                var data = reader.ReadToEnd();

                lines = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }

            banList = lines.ToList();
        }

        public void SetNewBanlist(List<string> banlist)
        {
            this.banList = banlist;
        }
        public IEnumerable<ExploitType> Check(string address)
        {
            return banList.Where(b => b.Equals(address)).Select(_ => ExploitType.Banlist_Match).Take(1);
        }

        
    }
}
