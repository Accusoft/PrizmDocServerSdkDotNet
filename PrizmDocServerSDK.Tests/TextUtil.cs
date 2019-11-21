using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Newtonsoft.Json.Linq;

namespace Accusoft.PrizmDocServer.Tests
{
    public static class TextUtil
    {
        /// <summary>
        /// Extracts text for each page, returning a string of plain text for each page in a RemoteWorkFile.
        /// </summary>
        public static async Task<string[]> ExtractPagesText(RemoteWorkFile remoteWorkFile)
        {
            AffinitySession session = Util.RestClient.CreateAffinitySession();

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "/v2/searchContexts");

            if (remoteWorkFile.AffinityToken != null)
            {
                req.Headers.Add("Accusoft-Affinity-Token", remoteWorkFile.AffinityToken);
            }

            req.Content = new StringContent(
                @"{
  ""input"": {
    ""documentIdentifier"": """ + remoteWorkFile.FileId + @""",
    ""source"": ""workFile"",
    ""fileId"": """ + remoteWorkFile.FileId + @"""
  }
}",
                Encoding.UTF8,
                "application/json");

            string json;
            using (HttpResponseMessage res = await session.SendAsync(req))
            {
                res.EnsureSuccessStatusCode();
                json = await res.Content.ReadAsStringAsync();
            }

            JObject process = JObject.Parse(json);
            string contextId = (string)process["contextId"];
            using (HttpResponseMessage res = await session.GetFinalProcessStatusAsync("/v2/searchContexts/" + contextId))
            {
                res.EnsureSuccessStatusCode();
            }

            using (HttpResponseMessage res = await session.GetAsync($"/v2/searchContexts/{contextId}/records?pages=0-"))
            {
                res.EnsureSuccessStatusCode();
                json = await res.Content.ReadAsStringAsync();
            }

            JObject data = JObject.Parse(json);
            JArray pages = (JArray)data["pages"];

            return pages.Select(x => (string)x["text"]).ToArray();
        }
    }
}
