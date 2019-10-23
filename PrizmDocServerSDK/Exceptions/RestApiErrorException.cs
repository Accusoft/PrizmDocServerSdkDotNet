using System;
using System.Net;

namespace Accusoft.PrizmDocServer.Exceptions
{
  public class RestApiErrorException : Exception
  {
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
      StatusCode = statusCode;
      ReasonPhrase = reasonPhrase;
      ErrorCode = errorCode;
      RawErrorDetails = rawErrorDetails;
    }

    internal RestApiErrorException(string message, ErrorData err)
        : this(message ?? err.DefaultExceptionMessage, err.StatusCode, err.ReasonPhrase, err.ErrorCode, err.RawErrorDetails)
    {
    }

    internal RestApiErrorException(ErrorData err)
        : this(err.DefaultExceptionMessage, err.StatusCode, err.ReasonPhrase, err.ErrorCode, err.RawErrorDetails)
    {
    }

    public HttpStatusCode StatusCode { get; }
    public string ReasonPhrase { get; }
    public string ErrorCode { get; }
    public string RawErrorDetails { get; }
  }
}
