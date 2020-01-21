using System;
using Accusoft.PrizmDocServer.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Conversion.Tests
{
    [TestClass]
    public class DpiOptions_Tests
    {
        [TestMethod]
        public void Constructor_rejects_non_positive_horizontal_DPI_values()
        {
            UtilAssert.ThrowsExceptionWithMessageContaining<ArgumentException>(() => new DpiOptions(0, 100), new[] { "value must be greater than zero", "x" });
            UtilAssert.ThrowsExceptionWithMessageContaining<ArgumentException>(() => new DpiOptions(-1, 100), new[] { "value must be greater than zero", "x" });
        }

        [TestMethod]
        public void Constructor_rejects_non_positive_vertical_DPI_values()
        {
            UtilAssert.ThrowsExceptionWithMessageContaining<ArgumentException>(() => new DpiOptions(100, 0), new[] { "value must be greater than zero", "y" });
            UtilAssert.ThrowsExceptionWithMessageContaining<ArgumentException>(() => new DpiOptions(100, -1), new[] { "value must be greater than zero", "y" });
        }
    }
}
