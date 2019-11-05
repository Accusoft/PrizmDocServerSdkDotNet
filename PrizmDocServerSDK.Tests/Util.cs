using Accusoft.PrizmDoc.Net.Http;

namespace Accusoft.PrizmDocServer.Tests
{
    public static class Util
    {
        static Util()
        {
            RestClient = new PrizmDocRestClient(System.Environment.GetEnvironmentVariable("BASE_URL"));

            if (System.Environment.GetEnvironmentVariable("API_KEY") != null)
            {
                RestClient.DefaultRequestHeaders.Add("Acs-Api-Key", System.Environment.GetEnvironmentVariable("API_KEY"));
            }
        }

        public static PrizmDocRestClient RestClient { get; private set; }

        public static PrizmDocServerClient CreatePrizmDocServerClient()
        {
            return new PrizmDocServerClient(System.Environment.GetEnvironmentVariable("BASE_URL"), System.Environment.GetEnvironmentVariable("API_KEY"));
        }
    }
}
