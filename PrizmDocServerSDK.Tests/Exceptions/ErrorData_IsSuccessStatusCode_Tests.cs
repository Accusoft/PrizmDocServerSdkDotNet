using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Accusoft.PrizmDocServer.Exceptions.Tests
{
    [TestClass]
    public class ErrorData_IsSuccessStatusCode_Tests
    {
        [TestMethod]
        public void Returns_true_for_200()
        {
            Assert.IsTrue(ErrorData.IsSuccessStatusCode((HttpStatusCode)201));
        }

        [TestMethod]
        public void Returns_true_for_201()
        {
            Assert.IsTrue(ErrorData.IsSuccessStatusCode((HttpStatusCode)201));
        }

        [TestMethod]
        public void Returns_true_for_299()
        {
            Assert.IsTrue(ErrorData.IsSuccessStatusCode((HttpStatusCode)299));
        }

        [TestMethod]
        public void Returns_false_for_199()
        {
            Assert.IsFalse(ErrorData.IsSuccessStatusCode((HttpStatusCode)199));
        }

        [TestMethod]
        public void Returns_false_for_300()
        {
            Assert.IsFalse(ErrorData.IsSuccessStatusCode((HttpStatusCode)300));
        }

        [TestMethod]
        public void Returns_false_for_400()
        {
            Assert.IsFalse(ErrorData.IsSuccessStatusCode((HttpStatusCode)400));
        }

        [TestMethod]
        public void Returns_false_for_404()
        {
            Assert.IsFalse(ErrorData.IsSuccessStatusCode((HttpStatusCode)404));
        }

        [TestMethod]
        public void Returns_false_for_480()
        {
            Assert.IsFalse(ErrorData.IsSuccessStatusCode((HttpStatusCode)480));
        }

        [TestMethod]
        public void Returns_false_for_500()
        {
            Assert.IsFalse(ErrorData.IsSuccessStatusCode((HttpStatusCode)500));
        }

        [TestMethod]
        public void Returns_false_for_580()
        {
            Assert.IsFalse(ErrorData.IsSuccessStatusCode((HttpStatusCode)580));
        }
    }
}
