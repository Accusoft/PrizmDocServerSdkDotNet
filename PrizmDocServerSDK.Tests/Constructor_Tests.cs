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

      Assert.AreEqual("https://api.accusoft.com/", client.client.BaseAddress.ToString());
      Assert.AreEqual(true, client.client.DefaultRequestHeaders.Contains("Acs-Api-Key"));
      Assert.AreEqual("MY_API_KEY", client.client.DefaultRequestHeaders.GetValues("Acs-Api-Key").Single());
    }

    [TestMethod]
    public void Can_create_an_instance_for_self_hosted()
    {
      var client = new PrizmDocServerClient("http://mylocalinstance:18681");

      Assert.AreEqual("http://mylocalinstance:18681/", client.client.BaseAddress.ToString());
      Assert.AreEqual(false, client.client.DefaultRequestHeaders.Contains("Acs-Api-Key"));
    }
  }
}
