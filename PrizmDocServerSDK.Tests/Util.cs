using Accusoft.PrizmDoc.Net.Http;

namespace Accusoft.PrizmDocServer.Tests
{
    public static class Util
    {
        private static readonly string BaseUrl = System.Environment.GetEnvironmentVariable("BASE_URL");
        private static readonly string ApiKey = System.Environment.GetEnvironmentVariable("API_KEY");

        static Util()
        {
            RestClient = new PrizmDocRestClient(BaseUrl);

            if (ApiKey != null)
            {
                RestClient.DefaultRequestHeaders.Add("Acs-Api-Key", ApiKey);
            }
        }

        public static PrizmDocRestClient RestClient { get; private set; }

        public static PrizmDocServerClient CreatePrizmDocServerClient()
        {
            return new PrizmDocServerClient(BaseUrl, ApiKey);
        }
    }
}
