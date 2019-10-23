using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Accusoft.PrizmDocServer.Tests
{
  public static class UtilAssert
  {
    public static async Task ThrowsExceptionWithMessageAsync<TException>(Func<Task> function, string expectedMessage) where TException : Exception
    {
      try
      {
        await function();
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
  }
}
