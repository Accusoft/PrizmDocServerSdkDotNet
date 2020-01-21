using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Tests
{
    public static class UtilAssert
    {
        public static void ThrowsExceptionWithMessageContaining<TException>(Action action, string expectedSubstring)
            where TException : Exception
        {
            UtilAssert.ThrowsExceptionWithMessageContaining<TException>(action, new[] { expectedSubstring });
        }

        public static void ThrowsExceptionWithMessageContaining<TException>(Action action, string[] expectedSubstrings)
            where TException : Exception
        {
            try
            {
                action();
                Assert.Fail("No exception thrown!");
            }
            catch (TException exception)
            {
                foreach (var expectedSubstring in expectedSubstrings)
                {
                    StringAssert.Contains(exception.Message, expectedSubstring);
                }
            }
            catch (Exception exception)
            {
                Assert.Fail("Wrong exception thrown!\n" + exception.ToString());
            }
        }

        public static void ThrowsExceptionWithMessage<TException>(Action action, string expectedMessage)
            where TException : Exception
        {
            try
            {
                action();
                Assert.Fail("No exception thrown!");
            }
            catch (TException exception)
            {
                Assert.AreEqual(expectedMessage, exception.Message);
            }
            catch (Exception exception)
            {
                Assert.Fail("Wrong exception thrown!\n" + exception.ToString());
            }
        }

        public static async Task ThrowsExceptionWithMessageContainingAsync<TException>(Func<Task> function, string expectedSubstring)
            where TException : Exception
        {
            try
            {
                await function();
                Assert.Fail("No exception thrown!");
            }
            catch (TException exception)
            {
                StringAssert.Contains(exception.Message, expectedSubstring);
            }
            catch (Exception exception)
            {
                Assert.Fail("Wrong exception thrown!\n" + exception.ToString());
            }
        }

        public static async Task ThrowsExceptionWithMessageAsync<TException>(Func<Task> function, string expectedMessage, bool ignoreCase = false)
            where TException : Exception
        {
            try
            {
                await function();
                Assert.Fail("No exception thrown!");
            }
            catch (TException exception)
            {
                Assert.AreEqual(expectedMessage, exception.Message, ignoreCase);
            }
            catch (Exception exception)
            {
                Assert.Fail("Wrong exception thrown!\n" + exception.ToString());
            }
        }
    }
}
