using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Tests
{
    [TestClass]
    public class PrizmDocServerClient_Constructor_Tests
    {
        [TestMethod]
        public void Can_construct_with_string_baseAddress()
        {
            var client = new PrizmDocServerClient("http://localhost:18681");
        }

        [TestMethod]
        public void Can_construct_with_Uri_baseAddress()
        {
            var client = new PrizmDocServerClient(new Uri("http://localhost:18681"));
        }

        [TestMethod]
        public void Can_construct_with_string_baseAddress_and_cloud_API_key()
        {
            var client = new PrizmDocServerClient("https://api.accusoft.com", "MY_API_KEY");
        }

        [TestMethod]
        public void Can_construct_with_Uri_baseAddress_and_cloud_API_key()
        {
            var client = new PrizmDocServerClient(new Uri("https://api.accusoft.com"), "MY_API_KEY");
        }
    }
}
