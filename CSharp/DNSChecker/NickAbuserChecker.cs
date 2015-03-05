using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNSChecker
{
    using System.IO;

    public class NickAbuserChecker : IExploitChecker
    {
        private List<string> bannedWords; 
        private List<string> voicedNicks = new List<string>();

        private Func<IEnumerable<string>> nickFunc;
        public NickAbuserChecker(string filePath, Func<List<string>> usersFunc)
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
            nickFunc = usersFunc;
            bannedWords = lines.ToList();
        }

        public void SetNewBanlist(List<string> words)
        {
            bannedWords = words;
        }

        public IEnumerable<ExploitType> Check(string nick)
        {
            IEnumerable<string> voiced = Enumerable.Empty<string>();
            
            if (nickFunc != null)
            {
                voiced = this.nickFunc();
            }

            var combined = voiced
                            .Concat(bannedWords)
                            .Select(this.RemoveSpecialChars)
                            .Select(c => c.ToLower())
                            .Distinct()
                            .Where(l => !string.IsNullOrWhiteSpace(l));

            var targetNick = this.RemoveSpecialChars(nick).ToLower();
            if (combined.Any(targetNick.Contains))
            {
                return new[] { ExploitType.Nick_Abuser };
            }

            return Enumerable.Empty<ExploitType>();
        }


        private string RemoveSpecialChars(string str)
        {
            return str
                .Where(char.IsLetterOrDigit)
                .Aggregate(new StringBuilder(), (s, c) => s.Append(c))
                .ToString();
        }
    }
}
