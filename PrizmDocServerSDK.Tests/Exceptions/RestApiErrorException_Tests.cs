using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accusoft.PrizmDocServer.Exceptions.Tests
{
    [TestClass]
    public class RestApiErrorException_Tests
    {
        [TestMethod]
        public void Can_construct_without_any_arguments()
        {
            _ = new RestApiErrorException();
        }

        [TestMethod]
        public void Can_construct_with_message_and_inner_exception()
        {
            _ = new RestApiErrorException("Some message", new InvalidOperationException());
        }

        [TestMethod]
        public void Can_construct_with_message_and_ErrorData()
        {
            var exception = new RestApiErrorException(
                "Some message",
                new ErrorData(
                    "Default exception message",
                    (HttpStatusCode)480,
                    "Reason Phrase",
                    @"{""errorCode"":""wat"",""errorDetails"":{""firstName"":""Bob""},""results"":[{""errorCode"":""innerProblem"",""errorDetails"":{""more"":""something bad""}}]}",
                    "wat",
                    @"{""firstName"":""Bob""}",
                    new List<InnerErrorData> { new InnerErrorData("someProblem", @"{""more"":""something bad""}") }));

            Assert.AreEqual(exception.Message, "Some message");
            Assert.AreEqual(exception.StatusCode, (HttpStatusCode)480);
            Assert.AreEqual(exception.ReasonPhrase, "Reason Phrase");
            Assert.AreEqual(exception.ErrorCode, "wat");
            Assert.AreEqual(exception.RawErrorDetails, @"{""firstName"":""Bob""}");
        }

        [TestMethod]
        public void When_constructing_with_null_message_and_ErrorData_the_ErrorData_DefaultExceptionMessage_is_used_as_the_message()
        {
            var exception = new RestApiErrorException(
                null,
                new ErrorData(
                    "Default exception message",
                    (HttpStatusCode)480,
                    "Reason Phrase",
                    @"{""errorCode"":""wat"",""errorDetails"":{""firstName"":""Bob""},""results"":[{""errorCode"":""innerProblem"",""errorDetails"":{""more"":""something bad""}}]}",
                    "wat",
                    @"{""firstName"":""Bob""}",
                    new List<InnerErrorData> { new InnerErrorData("someProblem", @"{""more"":""something bad""}") }));

            Assert.AreEqual(exception.Message, "Default exception message");
            Assert.AreEqual(exception.StatusCode, (HttpStatusCode)480);
            Assert.AreEqual(exception.ReasonPhrase, "Reason Phrase");
            Assert.AreEqual(exception.ErrorCode, "wat");
            Assert.AreEqual(exception.RawErrorDetails, @"{""firstName"":""Bob""}");
        }

        [TestMethod]
        public void Constructing_from_ErrorData_correctly_sets_properties()
        {
            ErrorData err = ErrorData.From((HttpStatusCode)480, "wat", @"{""errorCode"":""ServerCaughtFire"",""errorDetails"":{""in"":""body"",""at"":""foo""}}");

            var exception = new RestApiErrorException(err);

            Assert.AreEqual((HttpStatusCode)480, exception.StatusCode);
            Assert.AreEqual("wat", exception.ReasonPhrase);
            Assert.AreEqual("ServerCaughtFire", exception.ErrorCode);
            Assert.AreEqual(JValue.Parse(@"{""in"":""body"",""at"":""foo""}").ToString(Formatting.Indented), exception.RawErrorDetails);
        }

        [TestMethod]
        public void Constructing_from_ErrorData_correctly_sets_properties_when_there_are_no_errorDetails()
        {
            ErrorData err = ErrorData.From((HttpStatusCode)480, "wat", @"{""errorCode"":""ServerCaughtFire""}");

            var exception = new RestApiErrorException(err);

            Assert.AreEqual((HttpStatusCode)480, exception.StatusCode);
            Assert.AreEqual("wat", exception.ReasonPhrase);
            Assert.AreEqual("ServerCaughtFire", exception.ErrorCode);
            Assert.IsNull(exception.RawErrorDetails);
        }

        [TestMethod]
        public void Constructing_from_ErrorData_correctly_sets_properties_when_there_is_no_body()
        {
            ErrorData err = ErrorData.From((HttpStatusCode)480, "wat", null);

            var exception = new RestApiErrorException(err);

            Assert.AreEqual((HttpStatusCode)480, exception.StatusCode);
            Assert.AreEqual("wat", exception.ReasonPhrase);
            Assert.IsNull(exception.ErrorCode);
            Assert.IsNull(exception.RawErrorDetails);
        }
    }
}
