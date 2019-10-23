using System.Net.Http;
using System.Threading.Tasks;

namespace Accusoft.PrizmDocServer.Exceptions
{
  internal static class HttpResponseMessageExtensions
  {
    internal async static Task ThrowIfRestApiError(this HttpResponseMessage response)
    {
      var err = await ErrorData.From(response);
      if (err != null)
      {
        throw new RestApiErrorException(err);
      }
    }
  }
}
