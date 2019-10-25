using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Json.Deserialization;
using Newtonsoft.Json;

namespace Accusoft.PrizmDocServer
{
  internal static class AffinitySessionExtensions
  {
    /// <summary>
    /// Upload a local file, creating a new <see cref="RemoteWorkFile"/> which
    /// can be used within this <see cref="ProcessingContext"/>.
    /// </summary>
    /// <param name="localFilePath">Path to a local file you want to upload.</param>
    /// <returns><see cref="RemoteWorkFile"/> instance.</returns>
    internal static async Task<RemoteWorkFile> UploadAsync(this AffinitySession affinitySession, string localFilePath, string affinityToken = null)
    {
      if (!File.Exists(localFilePath))
      {
        throw new ArgumentException($"File not found: {localFilePath}", "localFilePathToDocument");
      }

      var fileExtension = Path.GetExtension(localFilePath);

      using (var localFileReadStream = File.OpenRead(localFilePath))
      {
        return await affinitySession.UploadAsync(localFileReadStream, fileExtension, affinityToken);
      }
    }

    /// <summary>
    /// Upload a <see cref="System.IO.Stream"/>, creating a new
    /// <see cref="RemoteWorkFile"/> which can be used within this
    /// <see cref="ProcessingContext"/>.
    /// </summary>
    /// <param name="documentStream">Stream of bytes of the document (file) you want to upload.</param>
    /// <returns><see cref="RemoteWorkFile"/> instance.</returns>
    internal static async Task<RemoteWorkFile> UploadAsync(this AffinitySession affinitySession, Stream documentStream, string fileExtension = "txt", string affinityToken = null)
    {
      // Remove leading period on fileExtension if present.
      if (fileExtension != null && fileExtension.StartsWith("."))
      {
        fileExtension = fileExtension.Substring(1);
      }

      var req = new HttpRequestMessage(HttpMethod.Post, $"/PCCIS/V1/WorkFile?FileExtension={fileExtension}")
      {
        Content = new StreamContent(documentStream)
      };
      if (affinityToken != null)
      {
        req.Headers.Add("Accusoft-Affinity-Token", affinityToken);
      }

      string json;
      using (var response = await affinitySession.SendAsync(req))
      {
        await response.ThrowIfRestApiError();
        json = await response.Content.ReadAsStringAsync();
      }

      var info = JsonConvert.DeserializeObject<PostWorkFileResponse>(json);
      var remoteWorkFile = new RemoteWorkFile(affinitySession, info.fileId, info.affinityToken, info.fileExtension);

      return remoteWorkFile;
    }
  }
}
