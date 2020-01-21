// This file defines the UploadAsync methods.
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;

namespace Accusoft.PrizmDocServer
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "Documented in PrizmDocServerClient.cs")]
    public partial class PrizmDocServerClient
    {
        /// <summary>
        /// Uploads a local file to PrizmDoc Server, creating a remote work file which can be used as input to document processing operations.
        /// </summary>
        /// <param name="localFilePath">Path to a local file to upload.</param>
        /// <param name="affinityToken">Optional affinity token defining which remote PrizmDoc Server this file should be uploaded to. This is an advanced option that you do not need to use.</param>
        /// <returns>RemoteWorkFile instance which can be used as input to other document processing methods.</returns>
        public async Task<RemoteWorkFile> UploadAsync(string localFilePath, string affinityToken = null)
        {
            AffinitySession affinitySession = this.restClient.CreateAffinitySession();
            return await affinitySession.UploadAsync(localFilePath, affinityToken);
        }

        /// <summary>
        /// Uploads a stream of bytes to PrizmDoc Server, creating a remote work file which can be used as input to document processing operations.
        /// </summary>
        /// <param name="stream">Stream of bytes for the file to upload.</param>
        /// <param name="fileExtension">File extension for the stream of bytes being uploaded. For plain text formats, this can influence how PrizmDoc Server will render the file.</param>
        /// <param name="affinityToken">Optional affinity token defining which remote PrizmDoc Server this file should be uploaded to. This is an advanced option that you do not need to use.</param>
        /// <returns>RemoteWorkFile instance which can be used as input to other document processing methods.</returns>
        public async Task<RemoteWorkFile> UploadAsync(Stream stream, string fileExtension = "txt", string affinityToken = null)
        {
            AffinitySession affinitySession = this.restClient.CreateAffinitySession();
            return await affinitySession.UploadAsync(stream, fileExtension, affinityToken);
        }
    }
}
