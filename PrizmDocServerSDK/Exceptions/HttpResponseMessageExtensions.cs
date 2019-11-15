#pragma warning disable SA1600 // Elements should be documented

using System.Net.Http;
using System.Threading.Tasks;

namespace Accusoft.PrizmDocServer.Exceptions
{
    internal static class HttpResponseMessageExtensions
    {
        internal static async Task ThrowIfRestApiError(this HttpResponseMessage response)
        {
            ErrorData err = await ErrorData.From(response);
            if (err != null)
            {
                throw new RestApiErrorException(err);
            }
        }
    }
}
