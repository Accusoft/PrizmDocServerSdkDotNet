using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Accusoft.PrizmDocServer.Exceptions.Tests
{
  [TestClass]
  public class ErrorData_can_be_correctly_extracted
  {
    [TestMethod]
    public void when_JSON_only_contains_errorCode()
    {
      var err = ErrorData.From((HttpStatusCode)480, "wat", @"{""errorCode"":""ServerCaughtFire""}");

      Assert.AreEqual("ServerCaughtFire", err.ErrorCode);
      Assert.IsNull(err.RawErrorDetails);
      Assert.AreEqual(0, err.InnerErrors.Count);
    }

    [TestMethod]
    public void when_JSON_contains_errorCode_and_errorDetails()
    {
      var err = ErrorData.From((HttpStatusCode)480, "wat", @"{""errorCode"":""ServerCaughtFire"",""errorDetails"":{""in"":""body"",""at"":""some.prop.path""}}");

      Assert.AreEqual("ServerCaughtFire", err.ErrorCode);
      Assert.AreEqual(JObject.Parse(@"{""in"":""body"",""at"":""some.prop.path""}").ToString(), err.RawErrorDetails);
      Assert.AreEqual(0, err.InnerErrors.Count);
    }

    [TestMethod]
    public void when_JSON_contains_errorCode_and_one_result_which_does_not_have_an_inner_errorCode()
    {
      var err = ErrorData.From(HttpStatusCode.OK, "wat", @"{""errorCode"":""CouldNotDoTheThing"",""output"":{""results"":[{""wat"":""foo""}]}}");

      Assert.AreEqual("CouldNotDoTheThing", err.ErrorCode);
      Assert.IsNull(err.RawErrorDetails);
      Assert.AreEqual(0, err.InnerErrors.Count);
    }

    [TestMethod]
    public void when_JSON_contains_errorCode_and_one_result_with_an_inner_errorCode()
    {
      var err = ErrorData.From(HttpStatusCode.OK, "wat", @"{""errorCode"":""CouldNotDoTheThing"",""output"":{""results"":[{""errorCode"":""ReallyBadProblemHappened""}]}}");

      Assert.AreEqual("CouldNotDoTheThing", err.ErrorCode);
      Assert.IsNull(err.RawErrorDetails);
      Assert.AreEqual(1, err.InnerErrors.Count);
      Assert.AreEqual("ReallyBadProblemHappened", err.InnerErrors[0].ErrorCode);
      Assert.IsNull(err.InnerErrors[0].RawErrorDetails);
    }

    [TestMethod]
    public void when_JSON_contains_errorCode_and_one_result_with_an_inner_errorCode_and_inner_errorDetails()
    {
      var err = ErrorData.From(HttpStatusCode.OK, "wat", @"{""errorCode"":""CouldNotDoTheThing"",""output"":{""results"":[{""errorCode"":""ReallyBadProblemHappened"",""errorDetails"":{""causedBy"":""really-bad-server"",""misbehaving"":true}}]}}");

      Assert.AreEqual("CouldNotDoTheThing", err.ErrorCode);
      Assert.IsNull(err.RawErrorDetails);
      Assert.AreEqual(1, err.InnerErrors.Count);
      Assert.AreEqual("ReallyBadProblemHappened", err.InnerErrors[0].ErrorCode);
      Assert.AreEqual(JObject.Parse(@"{""causedBy"":""really-bad-server"",""misbehaving"":true}").ToString(), err.InnerErrors[0].RawErrorDetails);
    }

    [TestMethod]
    public void when_JSON_contains_errorCode_and_three_results_two_of_which_contain_inner_error_info()
    {
      var err = ErrorData.From(HttpStatusCode.OK, "wat", @"{""errorCode"":""CouldNotDoTheThing"",""output"":{""results"":[{""errorCode"":""ReallyBadProblemHappened"",""errorDetails"":{""causedBy"":""really-bad-server"",""misbehaving"":true}},{""successfulResult"":true},{""errorCode"":""YetAnotherInnerError""}]}}");

      Assert.AreEqual("CouldNotDoTheThing", err.ErrorCode);
      Assert.IsNull(err.RawErrorDetails);
      Assert.AreEqual(2, err.InnerErrors.Count);
      Assert.AreEqual("ReallyBadProblemHappened", err.InnerErrors[0].ErrorCode);
      Assert.AreEqual(JObject.Parse(@"{""causedBy"":""really-bad-server"",""misbehaving"":true}").ToString(), err.InnerErrors[0].RawErrorDetails);
      Assert.AreEqual("YetAnotherInnerError", err.InnerErrors[1].ErrorCode);
      Assert.IsNull(err.InnerErrors[1].RawErrorDetails);
    }
  }
}
