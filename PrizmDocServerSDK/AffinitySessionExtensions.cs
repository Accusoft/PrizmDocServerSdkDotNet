#pragma warning disable SA1611 // Element parameters should be documented
#pragma warning disable SA1612 // Element parameter documentation should match element parameters
#pragma warning disable SA1615 // Element return value should be documented

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
    /// <summary>
    /// Defines convenience extension methods for the PrizmDoc REST Client AffinitySession class.
    /// </summary>
    internal static class AffinitySessionExtensions
    {
        /// <summary>
        /// Upload a local file, creating a new <see cref="RemoteWorkFile"/>.
        /// </summary>
        internal static async Task<RemoteWorkFile> UploadAsync(this AffinitySession affinitySession, string localFilePath, string affinityToken = null)
        {
            if (affinitySession is null)
            {
                throw new ArgumentNullException(nameof(affinitySession));
            }

            if (!File.Exists(localFilePath))
            {
                throw new ArgumentException($"File not found: {localFilePath}", "localFilePathToDocument");
            }

            string fileExtension = Path.GetExtension(localFilePath);

            using (FileStream localFileReadStream = File.OpenRead(localFilePath))
            {
                return await affinitySession.UploadAsync(localFileReadStream, fileExtension, affinityToken);
            }
        }

        /// <summary>
        /// Upload a <see cref="Stream"/>, creating a new <see cref="RemoteWorkFile"/>.
        /// </summary>
        internal static async Task<RemoteWorkFile> UploadAsync(this AffinitySession affinitySession, Stream documentStream, string fileExtension = "txt", string affinityToken = null)
        {
            // Remove leading period on fileExtension if present.
            if (fileExtension != null && fileExtension.StartsWith("."))
            {
                fileExtension = fileExtension.Substring(1);
            }

            var req = new HttpRequestMessage(HttpMethod.Post, $"/PCCIS/V1/WorkFile?FileExtension={fileExtension}")
            {
                Content = new StreamContent(documentStream),
            };
            if (affinityToken != null)
            {
                req.Headers.Add("Accusoft-Affinity-Token", affinityToken);
            }

            string json;
            using (HttpResponseMessage response = await affinitySession.SendAsync(req))
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
