using System;
using System.Net;

namespace Accusoft.PrizmDocServer.Exceptions
{
    /// <summary>
    /// The exception thrown when the PrizmDoc Server REST API responds with an error.
    /// </summary>
    public class RestApiErrorException : Exception
    {
#pragma warning disable SA1600 // Elements should be documented
        internal RestApiErrorException()
        {
        }

        internal RestApiErrorException(string message)
            : base(message)
        {
        }

        internal RestApiErrorException(string message, Exception inner)
            : base(message, inner)
        {
        }

        internal RestApiErrorException(string message, HttpStatusCode statusCode, string reasonPhrase, string errorCode, string rawErrorDetails)
            : this(message)
        {
            this.StatusCode = statusCode;
            this.ReasonPhrase = reasonPhrase;
            this.ErrorCode = errorCode;
            this.RawErrorDetails = rawErrorDetails;
        }

        internal RestApiErrorException(string message, ErrorData err)
            : this(message ?? err.DefaultExceptionMessage, err.StatusCode, err.ReasonPhrase, err.ErrorCode, err.RawErrorDetails)
        {
        }

        internal RestApiErrorException(ErrorData err)
            : this(err.DefaultExceptionMessage, err.StatusCode, err.ReasonPhrase, err.ErrorCode, err.RawErrorDetails)
        {
        }
#pragma warning restore SA1600 // Elements should be documented

        /// <summary>
        /// Gets the HTTP status code of the PrizmDoc Server REST API error response.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the HTTP reason phrase (status message) of the PrizmDoc Server REST API error response.
        /// </summary>
        public string ReasonPhrase { get; }

        /// <summary>
        /// Gets the JSON "errorCode" value of the PrizmDoc Server REST API error response.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Gets the JSON "errorDetails" value of the PrizmDoc Server REST API error response.
        /// </summary>
        public string RawErrorDetails { get; }
    }
}
