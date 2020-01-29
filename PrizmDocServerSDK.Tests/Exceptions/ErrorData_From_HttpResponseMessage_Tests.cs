using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Accusoft.PrizmDocServer.Exceptions.Tests
{
    [TestClass]
    public class ErrorData_From_HttpResponseMessage_Tests
    {
        [TestMethod]
        public async Task Validates_the_response_is_not_null()
        {
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await ErrorData.From(null));
        }

        [TestMethod]
        public async Task Extracts_body_when_Content_Type_is_JSON()
        {
            using (var response = new HttpResponseMessage((HttpStatusCode)418)
            {
                Content = new StringContent("\"Hello, world!\"", Encoding.UTF8, "application/json"),
            })
            {
                ErrorData err = await ErrorData.From(response);

                Assert.AreEqual(err.StatusCode, (HttpStatusCode)418);
                Assert.AreEqual(err.RawBody, "\"Hello, world!\"");
            }
        }

        [TestMethod]
        public async Task Does_not_extract_body_when_Content_Type_is_not_JSON()
        {
            using (var response = new HttpResponseMessage((HttpStatusCode)418)
            {
                Content = new StringContent("Hello, world!", Encoding.UTF8, "text/plain"),
            })
            {
                ErrorData err = await ErrorData.From(response);

                Assert.AreEqual(err.StatusCode, (HttpStatusCode)418);
                Assert.IsNull(err.RawBody);
            }
        }

        [TestMethod]
        public async Task Correctly_extracts_error_details_from_JSON()
        {
            using (var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                ReasonPhrase = "OK",
                Content = new StringContent(@"{""errorCode"":""CouldNotDoTheThing"",""output"":{""results"":[{""errorCode"":""ReallyBadProblemHappened"",""errorDetails"":{""causedBy"":""really-bad-server"",""misbehaving"":true}}]}}", Encoding.UTF8, "application/json"),
            })
            {
                ErrorData err = await ErrorData.From(response);

                Assert.AreEqual("CouldNotDoTheThing", err.ErrorCode);
                Assert.IsNull(err.RawErrorDetails);
                Assert.AreEqual(1, err.InnerErrors.Count);
                Assert.AreEqual("ReallyBadProblemHappened", err.InnerErrors[0].ErrorCode);
                Assert.AreEqual(JObject.Parse(@"{""causedBy"":""really-bad-server"",""misbehaving"":true}").ToString(), err.InnerErrors[0].RawErrorDetails);
            }
        }
    }
}
