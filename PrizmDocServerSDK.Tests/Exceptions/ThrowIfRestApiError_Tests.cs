using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Accusoft.PrizmDocServer.Exceptions.Tests
{
    [TestClass]
    public class ThrowIfRestApiError_Tests
    {
        [TestMethod]
        public async Task HttpResponseMessage_extension_ThrowIfRestApiError_does_not_throw_if_HTTP_status_code_indicates_success_and_there_is_no_JSON_errorCode_in_the_response_body()
        {
            var message = new HttpResponseMessage(HttpStatusCode.OK);
            message.ReasonPhrase = "wat";
            message.Content = new StringContent(@"{""firstName"":""Bob""}", Encoding.UTF8, "application/json");

            await message.ThrowIfRestApiError(); // should not throw
        }

        [TestMethod]
        public async Task HttpResponseMessage_extension_ThrowIfRestApiError_throws_if_the_HTTP_status_code_indicates_an_error_even_if_there_is_no_JSON_errorCode()
        {
            var message = new HttpResponseMessage((HttpStatusCode)480);
            message.ReasonPhrase = "wat";
            message.Content = new StringContent(@"{""firstName"":""Bob""}", Encoding.UTF8, "application/json");

            await Assert.ThrowsExceptionAsync<RestApiErrorException>(async () => await message.ThrowIfRestApiError());
        }

        [TestMethod]
        public async Task HttpResponseMessage_extension_ThrowIfRestApiError_throws_if_ErrorData_can_be_extracted_from_the_response()
        {
            var message = new HttpResponseMessage(HttpStatusCode.OK);
            message.ReasonPhrase = "OK";
            message.Content = new StringContent(@"{""errorCode"":""ServerCaughtFire"",""errorDetails"":{""in"":""body"",""at"":""foo""}}", Encoding.UTF8, "application/json");

            await Assert.ThrowsExceptionAsync<RestApiErrorException>(async () => await message.ThrowIfRestApiError());
        }
    }
}
