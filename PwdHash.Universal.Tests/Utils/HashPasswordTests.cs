using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using PwdHash.Utils;

namespace PwdHash.Universal.Tests.Utils
{
    [TestClass]
    public class HashPasswordTests
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
        public void testToString()
        {
            var hashedPassword = HashPassword.create("my53cret#",
            "example.com");
            Assert.AreEqual("Bu6aSm+Zcsf", hashedPassword);
        }

        [TestMethod]
        public void testToStringWithNonAsciiChars()
        {
            var hashedPassword = HashPassword.create("mü53crét#",
            "example.com");
            Assert.AreEqual("r9qeSjv+lwJ", hashedPassword);
        }

        [TestMethod]
        public void testToStringWithNonLatin1Chars()
        {
            var hashedPassword = HashPassword.create("中文العربي",
            "example.com");
            Assert.AreEqual("AwMz3+BdMT", hashedPassword);
        }

        [TestMethod]
        public void testToStringWithoutNonAlphanumeric()
        {
            var hashedPassword = HashPassword.create("my53cret",
            "example.com");
            Assert.AreEqual("CIUD4SCSgh", hashedPassword);
        }

        [TestMethod]
        public void testToStringWithShortSecret()
        {
            var hashedPassword = HashPassword.create("ab",
            "example.com");
            Assert.AreEqual("0IKv", hashedPassword);
        }

        [TestMethod]
        public void testToStringWithShortestSecret()
        {
            var hashedPassword = HashPassword.create("a",
            "example.com");
            Assert.AreEqual("9FBo", hashedPassword);
        }

        [TestMethod]
        public void testToStringWithLongSecret()
        {
            var hashedPassword = HashPassword.create(
            "abcdefghijklmnopqrstuvwxyz0123456789=", "example.com");
            String result = hashedPassword;

            // The original algorithm appends NULL bytes at the end.
            // Those bytes should not be part of the output.
            // "XO3u58jVa1nd+8qd08SDIQ\0\0\0\0"
            Assert.AreEqual("XO3u58jVa1nd+8qd08SDIQ", result);
        }

        [TestMethod]
        public void testToStringWithEmptySecret()
        {
            try
            {
                var hashedPassword = HashPassword.create("",
                "example.com");
                
                Assert.Fail("App did not throw exception");
                
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void create_emptyPassword_throwsException()
        {
            try
            {
                var hashedPassword = HashPassword.create("blabla","");
                
                Assert.Fail("App did not throw exception");
                
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true);
            }
        }

    }
}