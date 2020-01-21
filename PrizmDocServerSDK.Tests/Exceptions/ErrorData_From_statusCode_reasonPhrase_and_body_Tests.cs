using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Accusoft.PrizmDocServer.Exceptions.Tests
{
    [TestClass]
    public class ErrorData_From_statusCode_reasonPhrase_and_body_Tests
    {
        [TestMethod]
        public void When_HTTP_status_code_indicates_success_and_there_is_no_body_then_null_is_returned()
        {
            ErrorData err = ErrorData.From(HttpStatusCode.OK, "OK", null);

            Assert.IsNull(err);
        }

        [TestMethod]
        public void When_HTTP_status_code_indicates_success_and_there_is_a_JSON_body_but_no_errorCode_then_null_is_returned()
        {
            ErrorData err = ErrorData.From(HttpStatusCode.OK, "OK", @"{""firstName"":""Bob""}");

            Assert.IsNull(err);
        }

        [TestMethod]
        public void When_HTTP_status_code_indicates_success_but_the_body_contains_JSON_with_an_errorCode_then_an_ErrorData_instance_is_returned()
        {
            ErrorData err = ErrorData.From(HttpStatusCode.OK, "OK", @"{""errorCode"":""WatHappened""}");

            Assert.IsNotNull(err);
            Assert.AreEqual(HttpStatusCode.OK, err.StatusCode);
            Assert.AreEqual("OK", err.ReasonPhrase);
            Assert.AreEqual(@"{""errorCode"":""WatHappened""}", err.RawBody);
            Assert.AreEqual("WatHappened", err.ErrorCode);
            Assert.IsNull(err.RawErrorDetails);
            Assert.AreEqual(0, err.InnerErrors.Count);
        }

        [TestMethod]
        public void When_JSON_does_not_contain_an_errorCode_but_HTTP_status_code_indicates_an_error_then_an_ErrorData_instance_is_returned()
        {
            ErrorData err = ErrorData.From((HttpStatusCode)480, "wat", @"{""firstName"":""Bob""}");

            Assert.IsNotNull(err);
            Assert.AreEqual((HttpStatusCode)480, err.StatusCode);
            Assert.AreEqual("wat", err.ReasonPhrase);
            Assert.AreEqual(@"{""firstName"":""Bob""}", err.RawBody);
            Assert.IsNull(err.ErrorCode);
            Assert.IsNull(err.RawErrorDetails);
            Assert.AreEqual(0, err.InnerErrors.Count);
        }

        [TestMethod]
        public void When_JSON_only_contains_errorCode()
        {
            ErrorData err = ErrorData.From((HttpStatusCode)480, "wat", @"{""errorCode"":""ServerCaughtFire""}");

            Assert.AreEqual("ServerCaughtFire", err.ErrorCode);
            Assert.IsNull(err.RawErrorDetails);
            Assert.AreEqual(0, err.InnerErrors.Count);
        }

        [TestMethod]
        public void When_JSON_contains_errorCode_and_errorDetails()
        {
            ErrorData err = ErrorData.From((HttpStatusCode)480, "wat", @"{""errorCode"":""ServerCaughtFire"",""errorDetails"":{""in"":""body"",""at"":""some.prop.path""}}");

            Assert.AreEqual("ServerCaughtFire", err.ErrorCode);
            Assert.AreEqual(JObject.Parse(@"{""in"":""body"",""at"":""some.prop.path""}").ToString(), err.RawErrorDetails);
            Assert.AreEqual(0, err.InnerErrors.Count);
        }

        [TestMethod]
        public void When_JSON_contains_errorCode_and_one_result_with_an_inner_errorCode()
        {
            ErrorData err = ErrorData.From(HttpStatusCode.OK, "wat", @"{""errorCode"":""CouldNotDoTheThing"",""output"":{""results"":[{""errorCode"":""ReallyBadProblemHappened""}]}}");

            Assert.AreEqual("CouldNotDoTheThing", err.ErrorCode);
            Assert.IsNull(err.RawErrorDetails);
            Assert.AreEqual(1, err.InnerErrors.Count);
            Assert.AreEqual("ReallyBadProblemHappened", err.InnerErrors[0].ErrorCode);
            Assert.IsNull(err.InnerErrors[0].RawErrorDetails);
        }

        [TestMethod]
        public void When_JSON_contains_errorCode_and_one_result_with_an_inner_errorCode_and_inner_errorDetails()
        {
            ErrorData err = ErrorData.From(HttpStatusCode.OK, "wat", @"{""errorCode"":""CouldNotDoTheThing"",""output"":{""results"":[{""errorCode"":""ReallyBadProblemHappened"",""errorDetails"":{""causedBy"":""really-bad-server"",""misbehaving"":true}}]}}");

            Assert.AreEqual("CouldNotDoTheThing", err.ErrorCode);
            Assert.IsNull(err.RawErrorDetails);
            Assert.AreEqual(1, err.InnerErrors.Count);
            Assert.AreEqual("ReallyBadProblemHappened", err.InnerErrors[0].ErrorCode);
            Assert.AreEqual(JObject.Parse(@"{""causedBy"":""really-bad-server"",""misbehaving"":true}").ToString(), err.InnerErrors[0].RawErrorDetails);
        }

        [TestMethod]
        public void When_JSON_contains_errorCode_and_three_results_two_of_which_contain_inner_error_info()
        {
            ErrorData err = ErrorData.From(HttpStatusCode.OK, "wat", @"{""errorCode"":""CouldNotDoTheThing"",""output"":{""results"":[{""errorCode"":""ReallyBadProblemHappened"",""errorDetails"":{""causedBy"":""really-bad-server"",""misbehaving"":true}},{""successfulResult"":true},{""errorCode"":""YetAnotherInnerError""}]}}");

            Assert.AreEqual("CouldNotDoTheThing", err.ErrorCode);
            Assert.IsNull(err.RawErrorDetails);
            Assert.AreEqual(2, err.InnerErrors.Count);
            Assert.AreEqual("ReallyBadProblemHappened", err.InnerErrors[0].ErrorCode);
            Assert.AreEqual(JObject.Parse(@"{""causedBy"":""really-bad-server"",""misbehaving"":true}").ToString(), err.InnerErrors[0].RawErrorDetails);
            Assert.AreEqual("YetAnotherInnerError", err.InnerErrors[1].ErrorCode);
            Assert.IsNull(err.InnerErrors[1].RawErrorDetails);
        }

        [TestMethod]
        public void Results_without_a_parsable_inner_errorCode_are_not_considered_inner_errors()
        {
            ErrorData err = ErrorData.From(HttpStatusCode.OK, "wat", @"{""errorCode"":""CouldNotDoTheThing"",""output"":{""results"":[{""wat"":""foo""},{""errorCode"":""FailedToExtract""},{""errorCode"":[1,2,3]}]}}");

            Assert.AreEqual("CouldNotDoTheThing", err.ErrorCode);
            Assert.IsNull(err.RawErrorDetails);
            Assert.AreEqual(1, err.InnerErrors.Count);
            Assert.AreEqual("FailedToExtract", err.InnerErrors[0].ErrorCode);
            Assert.IsNull(err.InnerErrors[0].RawErrorDetails);
        }
    }
}
