using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Accusoft.PrizmDocServer.Exceptions;

namespace Accusoft.PrizmDocServer
{
    /// <summary>
    /// Contains information about a remote work file.
    /// </summary>
    public class RemoteWorkFile : IEquatable<RemoteWorkFile>
    {
        private readonly AffinitySession session;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:Elements should be documented", Justification = "Only used internally and by tests")]
        internal RemoteWorkFile(AffinitySession session, string fileId, string affinityToken, string fileExtension)
        {
            this.session = session;
            this.FileId = fileId;
            this.AffinityToken = affinityToken;
            this.FileExtension = fileExtension;
        }

        /// <summary>
        /// Gets unique id of the remote work file.
        /// </summary>
        public string FileId { get; }

        /// <summary>
        /// Gets affinity token of the remote work file, identifying the remote server the file is stored on.
        /// </summary>
        public string AffinityToken { get; }

        /// <summary>
        /// Gets file extension for the remote work file, indicating the file type.
        /// </summary>
        public string FileExtension { get; }

        /// <summary>
        /// Downloads the remote work file and saves it to a local file path.
        /// </summary>
        /// <param name="localFilePath">Path where the file should be written to.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task SaveAsync(string localFilePath)
        {
            using (FileStream localFileWriteStream = File.Open(localFilePath, FileMode.Create))
            {
                await this.CopyToAsync(localFileWriteStream);
            }
        }

        /// <summary>
        /// Downloads the remote work file and copies it to a local stream.
        /// </summary>
        /// <param name="stream">Stream where the file should be copied to.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        public async Task CopyToAsync(Stream stream)
        {
            using (HttpResponseMessage res = await this.session.GetAsync($"/PCCIS/V1/WorkFile/{this.FileId}"))
            {
                await res.ThrowIfRestApiError();
                await res.Content.CopyToAsync(stream);
            }
        }

        /// <inheritdoc/>
        public bool Equals(RemoteWorkFile other)
        {
            return this.FileId == other.FileId &&
                   this.AffinityToken == other.AffinityToken &&
                   this.FileExtension == other.FileExtension;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 3681487;
            hashCode = (hashCode * -1521134295) + EqualityComparer<AffinitySession>.Default.GetHashCode(this.session);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.FileId);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.AffinityToken);
            hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.FileExtension);
            return hashCode;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var other = obj as RemoteWorkFile;

            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("RemoteWorkFile { ");
            builder.Append($"FileId: {this.FileId}");
            if (this.AffinityToken != null)
            {
                builder.Append($", AffinityToken: {this.AffinityToken}");
            }

            if (this.FileExtension != null)
            {
                builder.Append($", FileExtension: {this.FileExtension}");
            }

            builder.Append(" }");

            return builder.ToString();
        }

        /// <summary>
        /// Start an HTTP GET to download the remote file bytes. You are responsible for disposing the returned HttpResponseMessage.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        internal async Task<HttpResponseMessage> HttpGetAsync()
        {
            return await this.session.GetAsync($"/PCCIS/V1/WorkFile/{this.FileId}");
        }

        /// <summary>
        /// Returns a RemoteWorkFile instance with the specified affinity.
        /// If this instance already has the specified affinity, it will be
        /// returned. If it does not, it will be downloaded and re-uploaded
        /// to create a new instance with the proper affinity, and that new
        /// instance will be returned.
        /// </summary>
        /// <param name="affinitySession">Existing affinity session to use in case of re-upload.</param>
        /// <param name="affinityToken">Existing affinity token which the RemoteWorkFile needs to match.</param>
        /// <returns>This existing RemoteWorkFile if the AffinityToken matched the provided affinityToken, or a new RemoteWorkFile with the provided affinityToken.</returns>
        internal async Task<RemoteWorkFile> GetInstanceWithAffinity(AffinitySession affinitySession, string affinityToken = null)
        {
            // If no affinity token was specified, there is nothing to do.
            if (this.AffinityToken == null)
            {
                return this;
            }

            // If this RemoteWorkFile already has the correct affinity, there is
            // nothing to do.
            if (this.AffinityToken == affinityToken)
            {
                return this;
            }

            // If this RemoteWorkFile has the wrong affinity (it is on the wrong
            // machine), then download and re-upload it to the correct machine,
            // and assign the new RemoteWorkFile to this
            // ConversionSourceDocument.
            using (HttpResponseMessage res = await this.HttpGetAsync())
            {
                await res.ThrowIfRestApiError();
                Stream downloadStream = await res.Content.ReadAsStreamAsync();
                return await affinitySession.UploadAsync(downloadStream, this.FileExtension, affinityToken: affinityToken);
            }
        }
    }
}
