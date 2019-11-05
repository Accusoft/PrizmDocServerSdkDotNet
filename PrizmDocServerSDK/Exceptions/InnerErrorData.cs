#pragma warning disable SA1600 // Elements should be documented

namespace Accusoft.PrizmDocServer.Exceptions
{
    internal class InnerErrorData
    {
        internal InnerErrorData(string errorCode, string rawErrorDetails = null)
        {
            this.ErrorCode = errorCode;
            this.RawErrorDetails = rawErrorDetails;
        }

        internal string ErrorCode { get; }

        internal string RawErrorDetails { get; }
    }
}
