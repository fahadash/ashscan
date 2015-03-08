using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test
{
    using System.Linq;

    using DNSChecker;

    [TestClass]
    public class Playground
    {
        [TestMethod]
        public void NickAbuserTest()
        {
            var checker = new NickAbuserChecker("c:\\temp\\bannedwords.txt", () => Enumerable.Empty<string>().ToList());

            var result = checker.Check("Khwab-Khwahish-Aur-Zindagi");

            Assert.IsFalse(result.Any());
        }
    }
}
