using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Accusoft.PrizmDocServer.Tests
{
  public static class TextUtil
  {
    /// <summary>
    /// Extracts text for each page, returning a string of plain text for each page in a RemoteWorkFile.
    /// </summary>
    public static async Task<IEnumerable<string>> ExtractPagesText(RemoteWorkFile remoteWorkFile)
    {
      var session = Util.RestClient.CreateAffinitySession();

      var req = new HttpRequestMessage(HttpMethod.Post, "/v2/searchContexts");

      if (remoteWorkFile.AffinityToken != null)
      {
        req.Headers.Add("Accusoft-Affinity-Token", remoteWorkFile.AffinityToken);
      }

      req.Content = new StringContent(@"{
  ""input"": {
    ""documentIdentifier"": """ + remoteWorkFile.FileId + @""",
    ""source"": ""workFile"",
    ""fileId"": """ + remoteWorkFile.FileId + @"""
  }
}", Encoding.UTF8, "application/json");

      string json;
      using (var res = await session.SendAsync(req))
      {
        res.EnsureSuccessStatusCode();
        json = await res.Content.ReadAsStringAsync();
      }

      var process = JObject.Parse(json);
      var contextId = (string)process["contextId"];
      using (var res = await session.GetFinalProcessStatusAsync("/v2/searchContexts/" + contextId))
      {
        res.EnsureSuccessStatusCode();
      }

      using (var res = await session.GetAsync($"/v2/searchContexts/{contextId}/records?pages=0-"))
      {
        res.EnsureSuccessStatusCode();
        json = await res.Content.ReadAsStringAsync();
      }

      var data = JObject.Parse(json);
      var pages = (JArray)data["pages"];

      return pages.Select(x => (string)x["text"]).ToList();
    }
  }
}
