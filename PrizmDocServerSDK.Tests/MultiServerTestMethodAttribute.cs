using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Tests
{
  public class MultiServerTestMethodAttribute : TestMethodAttribute
  {
    public override TestResult[] Execute(ITestMethod testMethod)
    {
      if(System.Environment.GetEnvironmentVariable("MULTI_SERVER") == "true" ||
         System.Environment.GetEnvironmentVariable("BASE_URL") == "https://api.accusoft.com")
      {
        return base.Execute(testMethod);
      }
      else
      {
        return new TestResult[] { new TestResult { Outcome = UnitTestOutcome.Inconclusive } };
      }
    }
  }
}
