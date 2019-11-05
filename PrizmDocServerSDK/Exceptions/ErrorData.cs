#pragma warning disable SA1600 // Elements should be documented

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Accusoft.PrizmDocServer.Exceptions
{
    internal class ErrorData
    {
        internal ErrorData(string defaultExceptionMessage, HttpStatusCode statusCode, string reasonPhrase, string rawBody, string errorCode, string rawErrorDetails, List<InnerErrorData> innerErrors)
        {
            this.DefaultExceptionMessage = defaultExceptionMessage;
            this.StatusCode = statusCode;
            this.ReasonPhrase = reasonPhrase;
            this.RawBody = rawBody;

            this.ErrorCode = errorCode;
            this.RawErrorDetails = rawErrorDetails;

            this.InnerErrors = innerErrors;
        }

        internal string DefaultExceptionMessage { get; }

        internal HttpStatusCode StatusCode { get; }

        internal string ReasonPhrase { get; }

        internal string RawBody { get; }

        internal string ErrorCode { get; }

        internal string RawErrorDetails { get; }

        internal List<InnerErrorData> InnerErrors { get; }

        internal static async Task<ErrorData> From(HttpResponseMessage response)
        {
            if (response.Content.Headers.ContentType.MediaType != "application/json")
            {
                return null;
            }

            string body = await response.Content.ReadAsStringAsync();

            return From(response.StatusCode, response.ReasonPhrase, body);
        }

        internal static ErrorData From(HttpStatusCode statusCode, string reasonPhrase, string body)
        {
            string errorCode = null;
            try
            {
                errorCode = (string)JObject.Parse(body)["errorCode"];
                if (errorCode == null)
                {
                    return null;
                }
            }
            catch
            {
            }

            string rawErrorDetails = null;
            try
            {
                rawErrorDetails = JObject.Parse(body)["errorDetails"].ToString();
            }
            catch
            {
            }

            var innerErrors = new List<InnerErrorData>();

            JArray results = null;
            try
            {
                results = (JArray)JObject.Parse(body)["output"]["results"];
            }
            catch
            {
            }

            if (results != null)
            {
                foreach (JToken result in results)
                {
                    string innerErrorCode = null;
                    try
                    {
                        innerErrorCode = (string)result["errorCode"];
                    }
                    catch
                    {
                    }

                    if (innerErrorCode == null)
                    {
                        continue;
                    }

                    string rawInnerErrorDetails = null;
                    try
                    {
                        rawInnerErrorDetails = result["errorDetails"].ToString();
                    }
                    catch
                    {
                    }

                    innerErrors.Add(new InnerErrorData(innerErrorCode, rawInnerErrorDetails));
                }
            }

            // Ignore useless/misleading reason phrases
            if (reasonPhrase.ToUpperInvariant() == "UNKNOWN" || reasonPhrase.ToUpperInvariant() == "OK")
            {
                reasonPhrase = null;
            }

            string reasonPhraseAndOrErrorCode = string.Join(" ", new[] { reasonPhrase, errorCode }.Distinct()).Trim();

            return new ErrorData($"Remote server returned an error: {reasonPhraseAndOrErrorCode} {rawErrorDetails}".Trim(), statusCode, reasonPhrase, body, errorCode, rawErrorDetails, innerErrors);
        }
    }
}
