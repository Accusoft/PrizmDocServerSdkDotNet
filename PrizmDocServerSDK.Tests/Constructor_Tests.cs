using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Accusoft.PrizmDocServer.Tests
{
  [TestClass]
  public class Constructor_Tests
  {
    [TestMethod]
    public void Can_create_an_instance_for_PDC()
    {
      var client = new PrizmDocServerClient("https://api.accusoft.com", "MY_API_KEY");

      Assert.AreEqual("https://api.accusoft.com/", client.restClient.BaseAddress.ToString());
      Assert.AreEqual(true, client.restClient.DefaultRequestHeaders.Contains("Acs-Api-Key"));
      Assert.AreEqual("MY_API_KEY", client.restClient.DefaultRequestHeaders.GetValues("Acs-Api-Key").Single());
    }

    [TestMethod]
    public void Can_create_an_instance_for_self_hosted()
    {
      var client = new PrizmDocServerClient("http://mylocalinstance:18681");

      Assert.AreEqual("http://mylocalinstance:18681/", client.restClient.BaseAddress.ToString());
      Assert.AreEqual(false, client.restClient.DefaultRequestHeaders.Contains("Acs-Api-Key"));
    }
  }
}
