#pragma warning disable SA1600 // Elements should be documented

using System;
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
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            string body = null;

            if (response.Content.Headers.ContentType != null &&
                response.Content.Headers.ContentType.MediaType == "application/json")
            {
                body = await response.Content.ReadAsStringAsync();
            }

            return From(response.StatusCode, response.ReasonPhrase, body);
        }

        internal static bool IsSuccessStatusCode(HttpStatusCode statusCode)
        {
            return (int)statusCode >= 200 && (int)statusCode < 300;
        }

        internal static ErrorData From(HttpStatusCode statusCode, string reasonPhrase, string body)
        {
            // There are two fundamental cases which we consider an error:
            //
            // 1. Anytime the response has a JSON body with an "errorCode"
            //    property, regardless of HTTP status code. For example, GET
            //    process status will return HTTP 200, but the process itself
            //    may have failed with an "errorCode".
            // 2. Anytime the HTTP status code itself indicates an error,
            //    regardless of whether there is a body or a JSON errorCode.
            //    For example, perhaps we receive an unexpected HTTP 500 error.

            // If HTTP success and no body, then there is no error.
            if (IsSuccessStatusCode(statusCode) && body == null)
            {
                return null;
            }

            string errorCode = null;
            string rawErrorDetails = null;
            JArray results = null;
            var innerErrors = new List<InnerErrorData>();

            if (body != null)
            {
                try
                {
                    errorCode = (string)JObject.Parse(body)["errorCode"];
                }
                catch
                {
                }

                // If HTTP success and no errorCode within the JSON body, then there is no error.
                if (IsSuccessStatusCode(statusCode) && errorCode == null)
                {
                    return null;
                }

                try
                {
                    rawErrorDetails = JObject.Parse(body)["errorDetails"].ToString();
                }
                catch
                {
                }

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
            }

            // Exclude the reason phrase from the error message if it does not add value
            string reasonPhraseForErrorMessage = reasonPhrase;
            if (reasonPhraseForErrorMessage != null && (reasonPhraseForErrorMessage.ToUpperInvariant() == "UNKNOWN" || reasonPhraseForErrorMessage.ToUpperInvariant() == "OK"))
            {
                reasonPhraseForErrorMessage = null;
            }

            string reasonPhraseAndOrErrorCode = string.Join(" ", new[] { reasonPhraseForErrorMessage, errorCode }.Distinct()).Trim();

            return new ErrorData($"Remote server returned an error: {reasonPhraseAndOrErrorCode} {rawErrorDetails}".Trim(), statusCode, reasonPhrase, body, errorCode, rawErrorDetails, innerErrors);
        }
    }
}
