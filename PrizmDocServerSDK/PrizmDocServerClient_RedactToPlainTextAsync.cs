// This file defines the RedactToPlainTextAsync methods.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
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
        /// <para>
        /// Applies redactions in a markup JSON file to a document, producing a
        /// new redacted plain text file.
        /// </para>
        /// <para>
        /// Note that redaction options (like redaction reason, border
        /// color, fill color, etc.) are not used by PrizmDoc Server when
        /// redacting to plain text. Instead, PrizmDoc Server will simply
        /// indicate when a portion of a line was redacted with the string
        /// <c>"&lt;Text Redacted&gt;"</c>.
        /// </para>
        /// </summary>
        /// <param name="localFilePathToSourceDocument">Path to a local file to
        /// use as the source document.</param>
        /// <param name="localFilePathToMarkupJson">Path to a local markup.json
        /// file containing the redactions you want burned in to the source
        /// document.</param>
        /// <param name="outputLineEndingFormat">Line ending to use in the
        /// output plain text file, such as <c>"\n"</c> or
        /// <c>"\r\n"</c>.</param>
        /// <returns>RemoteWorkFile for the new plain text file.</returns>
        public async Task<RemoteWorkFile> RedactToPlainTextAsync(string localFilePathToSourceDocument, string localFilePathToMarkupJson, string outputLineEndingFormat)
        {
            AffinitySession affinitySession = this.restClient.CreateAffinitySession();

            RemoteWorkFile sourceDocument = await affinitySession.UploadAsync(localFilePathToSourceDocument);
            RemoteWorkFile markupJson = await affinitySession.UploadAsync(localFilePathToMarkupJson);

            return await this.RedactToPlainTextAsync(sourceDocument, markupJson, outputLineEndingFormat);
        }

        /// <summary>
        /// <para>
        /// Applies redactions in a markup JSON file to a document, producing a
        /// new redacted plain text file.
        /// </para>
        /// <para>
        /// Note that redaction options (like redaction reason, border
        /// color, fill color, etc.) are not used by PrizmDoc Server when
        /// redacting to plain text. Instead, PrizmDoc Server will simply
        /// indicate when a portion of a line was redacted with the string
        /// <c>"&lt;Text Redacted&gt;"</c>.
        /// </para>
        /// </summary>
        /// <param name="localFilePathToSourceDocument">Path to a local file to
        /// use as the source document.</param>
        /// <param name="markupJson">Existing <see cref="RemoteWorkFile" />
        /// containing the markup JSON you want burned into the source
        /// document.</param>
        /// <param name="outputLineEndingFormat">Line ending to use in the
        /// output plain text file, such as <c>"\n"</c> or
        /// <c>"\r\n"</c>.</param>
        /// <returns>RemoteWorkFile for the new plain text file.</returns>
        public async Task<RemoteWorkFile> RedactToPlainTextAsync(string localFilePathToSourceDocument, RemoteWorkFile markupJson, string outputLineEndingFormat)
        {
            AffinitySession affinitySession = this.restClient.CreateAffinitySession();

            RemoteWorkFile sourceDocument = await affinitySession.UploadAsync(localFilePathToSourceDocument, markupJson.AffinityToken);

            return await this.RedactToPlainTextAsync(sourceDocument, markupJson, outputLineEndingFormat);
        }

        /// <summary>
        /// <para>
        /// Applies redactions in a markup JSON file to a document, producing a
        /// new redacted plain text file.
        /// </para>
        /// <para>
        /// Note that redaction options (like redaction reason, border
        /// color, fill color, etc.) are not used by PrizmDoc Server when
        /// redacting to plain text. Instead, PrizmDoc Server will simply
        /// indicate when a portion of a line was redacted with the string
        /// <c>"&lt;Text Redacted&gt;"</c>.
        /// </para>
        /// </summary>
        /// <param name="sourceDocument">Existing <see cref="RemoteWorkFile" />
        /// to use as the source document.</param>
        /// <param name="localFilePathToMarkupJson">Path to a local markup.json
        /// file containing the markup you want burned in to the source
        /// document.</param>
        /// <param name="outputLineEndingFormat">Line ending to use in the
        /// output plain text file, such as <c>"\n"</c> or
        /// <c>"\r\n"</c>.</param>
        /// <returns>RemoteWorkFile for the new plain text file.</returns>
        public async Task<RemoteWorkFile> RedactToPlainTextAsync(RemoteWorkFile sourceDocument, string localFilePathToMarkupJson, string outputLineEndingFormat)
        {
            AffinitySession affinitySession = this.restClient.CreateAffinitySession();

            RemoteWorkFile markupJson = await affinitySession.UploadAsync(localFilePathToMarkupJson, sourceDocument.AffinityToken);

            return await this.RedactToPlainTextAsync(sourceDocument, markupJson, outputLineEndingFormat);
        }

        /// <summary>
        /// <para>
        /// Applies redactions in a markup JSON file to a document, producing a
        /// new redacted plain text file.
        /// </para>
        /// <para>
        /// Note that redaction options (like redaction reason, border
        /// color, fill color, etc.) are not used by PrizmDoc Server when
        /// redacting to plain text. Instead, PrizmDoc Server will simply
        /// indicate when a portion of a line was redacted with the string
        /// <c>"&lt;Text Redacted&gt;"</c>.
        /// </para>
        /// </summary>
        /// <param name="sourceDocument">Existing <see cref="RemoteWorkFile" />
        /// to use as the source document.</param>
        /// <param name="markupJson">Existing <see cref="RemoteWorkFile" />
        /// containing the markup JSON you want burned into the source
        /// document.</param>
        /// <param name="outputLineEndingFormat">Line ending to use in the output plain
        /// text file, such as <c>"\n"</c> or <c>"\r\n"</c>.</param>
        /// <returns>RemoteWorkFile for the new plain text file.</returns>
        public async Task<RemoteWorkFile> RedactToPlainTextAsync(RemoteWorkFile sourceDocument, RemoteWorkFile markupJson, string outputLineEndingFormat)
        {
            // Make sure we use the existing affinity token, if defined.
            AffinitySession affinitySession = this.restClient.CreateAffinitySession(sourceDocument.AffinityToken);

            // Make sure markupJson has the same affinity as the sourceDocument
            markupJson = await markupJson.GetInstanceWithAffinity(affinitySession, sourceDocument.AffinityToken);

            string json = this.BuildPostPlainTextRedactorsRequestJson(sourceDocument, markupJson, outputLineEndingFormat);

            // Start the redaction creation process
            using (HttpResponseMessage response = await affinitySession.PostAsync("/v2/plainTextRedactors", new StringContent(json, Encoding.UTF8, "application/json")))
            {
                await this.ThrowIfPostPlainTextRedactorsError(sourceDocument, markupJson, response, outputLineEndingFormat);
                json = await response.Content.ReadAsStringAsync();
            }

            JObject process = JObject.Parse(json);
            string processId = (string)process["processId"];

            // Wait for the process to complete
            using (HttpResponseMessage response = await affinitySession.GetFinalProcessStatusAsync($"/v2/plainTextRedactors/{processId}"))
            {
                await this.ThrowIfGetPlainTextRedactorsError(sourceDocument, markupJson, response);
                json = await response.Content.ReadAsStringAsync();
            }

            process = JObject.Parse(json);

            return new RemoteWorkFile(affinitySession, (string)process["output"]["fileId"], affinitySession.AffinityToken, "pdf");
        }

        private string BuildPostPlainTextRedactorsRequestJson(RemoteWorkFile sourceDocument, RemoteWorkFile markupJson, string outputLineEndingFormat)
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

                jsonWriter.WritePropertyName("source");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("fileId");
                jsonWriter.WriteValue(sourceDocument.FileId);
                jsonWriter.WriteEndObject();

                jsonWriter.WritePropertyName("markup");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("fileId");
                jsonWriter.WriteValue(markupJson.FileId);
                jsonWriter.WriteEndObject();

                jsonWriter.WritePropertyName("dest");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("lineEndings");
                jsonWriter.WriteValue(outputLineEndingFormat);
                jsonWriter.WriteEndObject();

                // end input object
                jsonWriter.WriteEndObject();

                // end req JSON
                jsonWriter.WriteEndObject();

                string json = stringWriter.ToString();
                return json;
            }
        }

        private async Task ThrowIfPostPlainTextRedactorsError(RemoteWorkFile sourceDocument, RemoteWorkFile markupJson, HttpResponseMessage response, string outputLineEndingFormat)
        {
            if (!response.IsSuccessStatusCode)
            {
                ErrorData err = await ErrorData.From(response);

                if (err != null)
                {
                    string msg;
                    var at = this.GetAt(err.RawErrorDetails);

                    if (err.ErrorCode == "InvalidInput")
                    {
                        // Example response:
                        //
                        // {
                        //     "errorCode": "InvalidInput",
                        //     "errorDetails": {
                        //         "in": "body",
                        //         "at": "input.dest.lineEndings",
                        //         "expected": {
                        //             "enum": [
                        //                 "\n",
                        //                 "\r\n"]
                        //         }
                        //     }
                        // }
                        if (at == "input.dest.lineEndings")
                        {
                            JToken expected = JObject.Parse(err.RawErrorDetails)["expected"];
                            string requestedValue = outputLineEndingFormat;
                            IEnumerable<string> supportedValues = ((System.Collections.IEnumerable)expected["enum"]).Cast<dynamic>().Select(x => (string)x);

                            msg = $"Unsupported line ending \"{requestedValue}\". The remote server only supports the following values: {string.Join(", ", supportedValues.Select(x => "\"" + x.Replace("\n", "\\n").Replace("\r", "\\r") + "\"").ToArray())}.";
                            throw new RestApiErrorException(msg, err);
                        }
                    }

                    if (err.ErrorCode == "ResourceNotFound")
                    {
                        // Example response:
                        //
                        // {
                        //     "errorCode": "ResourceNotFound",
                        //     "errorDetails": {
                        //         "in": "body",
                        //         "at": "input.source.fileId"
                        //     }
                        // }
                        if (at == "input.source.fileId")
                        {
                            msg = "Could not use the given RemoteWorkFile as the source document: the work file resource could not be found on the remote server. It may have expired.";
                            throw new RestApiErrorException(msg, err);
                        }

                        // Example response:
                        //
                        // {
                        //     "errorCode": "ResourceNotFound",
                        //     "errorDetails": {
                        //         "in": "body",
                        //         "at": "input.markup.fileId"
                        //     }
                        // }
                        if (at == "input.markup.fileId")
                        {
                            msg = "Could not use the given RemoteWorkFile as the markup JSON file: the work file resource could not be found on the remote server. It may have expired.";
                            throw new RestApiErrorException(msg, err);
                        }
                    }
                }

                // Unknown error
                throw new RestApiErrorException(err);
            }
        }

        private async Task ThrowIfGetPlainTextRedactorsError(RemoteWorkFile sourceDocument, RemoteWorkFile markupJson, HttpResponseMessage response)
        {
            ErrorData err = await ErrorData.From(response);

            if (err != null)
            {
                string msg;

                // Example response:
                //
                // {
                //     "processId": "UXQ_D1QS0mD0LJD76Q5lSw",
                //     "expirationDateTime": "2019-12-24T20:55:26.086Z",
                //     "input": {
                //         "source": {
                //             "fileId": "VC5vD4x0xSxGF7_rpBfJRw"
                //         },
                //         "markup": {
                //             "fileId": "mhu6HgZOO1QHfxFR-8WEzg"
                //         },
                //         "dest": {
                //             "lineEndings": "\n"
                //         }
                //     },
                //     "state": "error",
                //     "percentComplete": 0,
                //     "errorCode": "InternalError"
                // }
                if (err.ErrorCode == "InternalError")
                {
                    msg = "The remote server was unable to burn the markup file into the document. It is possible there is a problem with the document itself.";
                    throw new RestApiErrorException(msg, err);
                }

                // Example response:
                //
                // {
                //     "processId": "Nef_m5ZCImL9tj6ZQVNh3w",
                //     "expirationDateTime": "2019-12-24T20:57:14.307Z",
                //     "input": {
                //         "source": {
                //             "fileId": "66iz9-y9HybllSPiBZAxkw"
                //         },
                //         "markup": {
                //             "fileId": "Ra04Ge5Hz5_Nanvkv8LdNA"
                //         },
                //         "dest": {
                //             "lineEndings": "\n"
                //         }
                //     },
                //     "state": "error",
                //     "percentComplete": 0,
                //     "errorCode": "InvalidJson"
                // }
                if (err.ErrorCode == "InvalidJson")
                {
                    msg = "The remove server was unable to burn the markup file into the document because the markup file was not valid JSON.";
                    throw new RestApiErrorException(msg, err);
                }

                // Example response:
                //
                // {
                //     "processId": "5K85RfuKzVaCPP7TRGTHtA",
                //     "expirationDateTime": "2019-12-24T21:01:40.293Z",
                //     "input": {
                //         "source": {
                //             "fileId": "66iz9-y9HybllSPiBZAxkw"
                //         },
                //         "markup": {
                //             "fileId": "smxJQ7hK0e_F5KtGWqPD_g"
                //         },
                //         "dest": {
                //             "lineEndings": "\n"
                //         }
                //     },
                //     "state": "error",
                //     "percentComplete": 0,
                //     "errorCode": "InvalidMarkup"
                // }
                if (err.ErrorCode == "InvalidMarkup")
                {
                    msg = "The remote server rejected the given markup JSON because it contained content which did not conform to its allowed markup JSON schema. See the markup JSON schema documentation for your version of PrizmDoc Viewer (such as https://help.accusoft.com/PrizmDoc/latest/HTML/webframe.html#markup-json-specification.html).";
                    throw new RestApiErrorException(msg, err);
                }

                // Unknown error
                throw new RestApiErrorException(err);
            }
        }
    }
}
