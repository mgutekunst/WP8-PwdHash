using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using PwdHash.Utils;

namespace PwdHash.Universal.Tests.Utils
{
    [TestClass]
    public class DomainExtractorTests
    {
        [TestInitialize]
        public void Init()
        {

        }

        [TestCleanup]
        public void CleanUp()
        {
        }

        [TestMethod]
        public void Extract_Simplest_validResult()
        {
            var exp = "example.com";

            var act = DomainExtractor.Extract(exp);

            Assert.AreEqual(exp, act);
        }
        [TestMethod]
        public void Extract_withHttp_validResult()
        {
            var input = "http://example.com";
            var exp = "example.com";

            var act = DomainExtractor.Extract(input);

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void Extract_withPath_validResult()
        {
            var input = "http://example.com/aPath/test.html";
            var exp = "example.com";

            var act = DomainExtractor.Extract(input);

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void Extract_longHost_validResult()
        {
            var input = "http://login.test.example.com";
            var exp = "example.com";

            var act = DomainExtractor.Extract(input);

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void Extract_twoPartCountry_validResult()
        {
            var input = "http://example.co.uk";
            var exp = "example.co.uk";

            var act = DomainExtractor.Extract(input);

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void Extract_twoPartCountryLongHost_validResult()
        {
            var input = "http://login.example.co.uk";
            var exp = "example.co.uk";

            var act = DomainExtractor.Extract(input);

            Assert.AreEqual(exp, act);
        }

        [TestMethod]
        public void Extract_https_validResult()
        {
            var input = "https://login.example.co.uk/test.htm";
            var exp = "example.co.uk";

            var act = DomainExtractor.Extract(input);

            Assert.AreEqual(exp, act);
        }

        //		 [TestMethod]
        //		 public void Extract_NoUrl_Exception()
        //		 	{
        //		 		var input = "testing";
        //				
        //		 		var act = DomainExtractor.Extract(input);
        //		 	}

    }
}
