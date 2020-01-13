// This file defines the BurnMarkupAsync methods.
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Accusoft.PrizmDocServer.Exceptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accusoft.PrizmDocServer
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "Documented in PrizmDocServerClient.cs")]
    public partial class PrizmDocServerClient
    {
        /// <summary>
        /// Burns a markup JSON file into a document, producing a new PDF.
        /// </summary>
        /// <param name="localFilePathToSourceDocument">Path to a local file to
        /// use as the source document.</param>
        /// <param name="localFilePathToMarkupJson">Path to a local markup.json
        /// file containing the markup you want burned in to the source
        /// document.</param>
        /// <returns>RemoteWorkFile for the new PDF.</returns>
        public async Task<RemoteWorkFile> BurnMarkupAsync(string localFilePathToSourceDocument, string localFilePathToMarkupJson)
        {
            AffinitySession affinitySession = this.restClient.CreateAffinitySession();

            RemoteWorkFile sourceDocument = await affinitySession.UploadAsync(localFilePathToSourceDocument);
            RemoteWorkFile markupJson = await affinitySession.UploadAsync(localFilePathToMarkupJson);

            return await this.BurnMarkupAsync(sourceDocument, markupJson);
        }

        /// <summary>
        /// Burns a markup JSON file into a document, producing a new PDF.
        /// </summary>
        /// <param name="localFilePathToSourceDocument">Path to a local file to
        /// use as the source document.</param>
        /// <param name="markupJson">Existing <see cref="RemoteWorkFile" />
        /// containing the markup JSON you want burned into the source
        /// document.</param>
        /// <returns>RemoteWorkFile for the new PDF.</returns>
        public async Task<RemoteWorkFile> BurnMarkupAsync(string localFilePathToSourceDocument, RemoteWorkFile markupJson)
        {
            AffinitySession affinitySession = this.restClient.CreateAffinitySession();

            RemoteWorkFile sourceDocument = await affinitySession.UploadAsync(localFilePathToSourceDocument, markupJson.AffinityToken);

            return await this.BurnMarkupAsync(sourceDocument, markupJson);
        }

        /// <summary>
        /// Burns a markup JSON file into a document, producing a new PDF.
        /// </summary>
        /// <param name="sourceDocument">Existing <see cref="RemoteWorkFile" />
        /// to use as the source document.</param>
        /// <param name="localFilePathToMarkupJson">Path to a local markup.json
        /// file containing the markup you want burned in to the source
        /// document.</param>
        /// <returns>RemoteWorkFile for the new PDF.</returns>
        public async Task<RemoteWorkFile> BurnMarkupAsync(RemoteWorkFile sourceDocument, string localFilePathToMarkupJson)
        {
            AffinitySession affinitySession = this.restClient.CreateAffinitySession();

            RemoteWorkFile markupJson = await affinitySession.UploadAsync(localFilePathToMarkupJson, sourceDocument.AffinityToken);

            return await this.BurnMarkupAsync(sourceDocument, markupJson);
        }

        /// <summary>
        /// Burns a markup JSON file into a document, producing a new PDF.
        /// </summary>
        /// <param name="sourceDocument">Existing <see cref="RemoteWorkFile" />
        /// to use as the source document.</param>
        /// <param name="markupJson">Existing <see cref="RemoteWorkFile" />
        /// containing the markup JSON you want burned into the source
        /// document.</param>
        /// <returns>RemoteWorkFile for the new PDF.</returns>
        public async Task<RemoteWorkFile> BurnMarkupAsync(RemoteWorkFile sourceDocument, RemoteWorkFile markupJson)
        {
            // Make sure we use the existing affinity token, if defined.
            AffinitySession affinitySession = this.restClient.CreateAffinitySession(sourceDocument.AffinityToken);

            // Make sure markupJson has the same affinity as the sourceDocument
            markupJson = await markupJson.GetInstanceWithAffinity(affinitySession, sourceDocument.AffinityToken);

            string json = this.BuildPostMarkupBurnersRequestJson(sourceDocument, markupJson);

            // Start the redaction creation process
            using (HttpResponseMessage response = await affinitySession.PostAsync("/PCCIS/V1/MarkupBurner", new StringContent(json, Encoding.UTF8, "application/json")))
            {
                await this.ThrowIfPostMarkupBurnersError(sourceDocument, markupJson, response);
                json = await response.Content.ReadAsStringAsync();
            }

            JObject process = JObject.Parse(json);
            string processId = (string)process["processId"];

            // Wait for the process to complete
            using (HttpResponseMessage response = await affinitySession.GetFinalProcessStatusAsync($"/PCCIS/V1/MarkupBurner/{processId}"))
            {
                await this.ThrowIfGetMarkupBurnersError(sourceDocument, markupJson, response);
                json = await response.Content.ReadAsStringAsync();
            }

            process = JObject.Parse(json);

            return new RemoteWorkFile(affinitySession, (string)process["output"]["documentFileId"], affinitySession.AffinityToken, "pdf");
        }

        private string BuildPostMarkupBurnersRequestJson(RemoteWorkFile sourceDocument, RemoteWorkFile markupJson)
        {
            // Build the input JSON
            using (var stringWriter = new StringWriter())
            using (JsonTextWriter jsonWriter = new JsonTextWriter(stringWriter))
            {
                // start req JSON
                jsonWriter.WriteStartObject();

                // start input object
                jsonWriter.WritePropertyName("input");
                jsonWriter.WriteStartObject();

                jsonWriter.WritePropertyName("documentFileId");
                jsonWriter.WriteValue(sourceDocument.FileId);

                jsonWriter.WritePropertyName("markupFileId");
                jsonWriter.WriteValue(markupJson.FileId);

                // end input object
                jsonWriter.WriteEndObject();

                // end req JSON
                jsonWriter.WriteEndObject();

                string json = stringWriter.ToString();
                return json;
            }
        }

        private async Task ThrowIfPostMarkupBurnersError(RemoteWorkFile sourceDocument, RemoteWorkFile markupJson, HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                ErrorData err = await ErrorData.From(response);
                throw new RestApiErrorException(err);
            }
        }

        private async Task ThrowIfGetMarkupBurnersError(RemoteWorkFile sourceDocument, RemoteWorkFile markupJson, HttpResponseMessage response)
        {
            ErrorData err = await ErrorData.From(response);

            if (err != null)
            {
                string msg;

                if (err.ErrorCode == "RedactionError")
                {
                    msg = "The remote server was unable to burn the markup file into the document. It is possible there is a problem with the markup JSON or with the document itself.";
                    throw new RestApiErrorException(msg, err);
                }

                if (err.ErrorCode == "InvalidMarkup")
                {
                    msg = "The remote server rejected the given markup JSON because it contained content which did not conform to its allowed markup JSON schema. See the markup JSON schema documentation for your version of PrizmDoc Viewer (such as https://help.accusoft.com/PrizmDoc/latest/HTML/webframe.html#markup-json-specification.html).";
                    throw new RestApiErrorException(msg, err);
                }

                if (err.ErrorCode == "DocumentFileIdDoesNotExist")
                {
                    msg = "Could not use the given RemoteWorkFile as the source document: the work file resource could not be found on the remote server. It may have expired.";
                    throw new RestApiErrorException(msg, err);
                }

                if (err.ErrorCode == "MarkupFileIdDoesNotExist")
                {
                    msg = "Could not use the given RemoteWorkFile as the markup JSON file: the work file resource could not be found on the remote server. It may have expired.";
                    throw new RestApiErrorException(msg, err);
                }

                // Unknown error
                throw new RestApiErrorException(err);
            }
        }
    }
}
