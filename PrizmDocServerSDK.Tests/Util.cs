using Accusoft.PrizmDoc.Net.Http;

namespace Accusoft.PrizmDocServer.Tests
{
    public static class Util
    {
        private static string baseUrl = System.Environment.GetEnvironmentVariable("BASE_URL");
        private static string apiKey = System.Environment.GetEnvironmentVariable("API_KEY");

        static Util()
        {
            RestClient = new PrizmDocRestClient(baseUrl);

            if (apiKey != null)
            {
                RestClient.DefaultRequestHeaders.Add("Acs-Api-Key", apiKey);
            }
        }

        public static PrizmDocRestClient RestClient { get; private set; }

        public static PrizmDocServerClient CreatePrizmDocServerClient()
        {
            return new PrizmDocServerClient(baseUrl, apiKey);
        }
    }
}
