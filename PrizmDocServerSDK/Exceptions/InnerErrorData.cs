namespace Accusoft.PrizmDocServer.Exceptions
{
  internal class InnerErrorData
  {
    internal InnerErrorData(string errorCode, string rawErrorDetails = null)
    {
      ErrorCode = errorCode;
      RawErrorDetails = rawErrorDetails;
    }

    internal string ErrorCode { get; }
    internal string RawErrorDetails { get; }
  }
}
