// This file defines the CreateRedactionsAsync methods.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Json.Serialization;
using Accusoft.PrizmDocServer.Redaction;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accusoft.PrizmDocServer
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "Documented in PrizmDocServerClient.cs")]
    public partial class PrizmDocServerClient
    {
        /// <summary>
        /// Automatically create redaction definitions for a document and a given set of text-matching rules,
        /// producing a new markup JSON file that can be used in a subsequent operation to actually apply the
        /// redaction definitions to the document.
        /// </summary>
        /// <param name="localFilePath">Path to a local file to use as the source document for which redactions should be created.</param>
        /// <param name="rules">Rules defining what content in the document should have a redaction region created for it.</param>
        /// <returns><see cref="RemoteWorkFile"/> for the created markup JSON file.</returns>
        public async Task<RemoteWorkFile> CreateRedactionsAsync(string localFilePath, IEnumerable<RedactionMatchRule> rules)
        {
            AffinitySession affinitySession = this.restClient.CreateAffinitySession();

            RemoteWorkFile sourceDocument = await affinitySession.UploadAsync(localFilePath);

            return await this.CreateRedactionsAsync(sourceDocument, rules);
        }

        /// <summary>
        /// Automatically create redaction definitions for a document and a given set of text-matching rules,
        /// producing a new markup JSON file that can be used in a subsequent operation to actually apply the
        /// redaction definitions to the document.
        /// </summary>
        /// <param name="sourceDocument">Source document the redactions should be created for.</param>
        /// <param name="rules">Rules defining what content in the document should have a redaction region created for it.</param>
        /// <returns><see cref="RemoteWorkFile"/> for the created markup JSON file.</returns>
        public async Task<RemoteWorkFile> CreateRedactionsAsync(RemoteWorkFile sourceDocument, IEnumerable<RedactionMatchRule> rules)
        {
            RedactionMatchRule[] rulesArray = rules.ToArray();

            // Make sure we use the existing affinity token, if defined.
            AffinitySession affinitySession = this.restClient.CreateAffinitySession(sourceDocument.AffinityToken);

            string json = this.BuildPostRedactionCreatorsRequestJson(sourceDocument, rules.ToArray());

            // Start the redaction creation process
            using (HttpResponseMessage response = await affinitySession.PostAsync("/v2/redactionCreators", new StringContent(json, Encoding.UTF8, "application/json")))
            {
                await this.ThrowIfPostRedactionCreatorsError(rulesArray, response);
                json = await response.Content.ReadAsStringAsync();
            }

            JObject process = JObject.Parse(json);
            string processId = (string)process["processId"];

            // Wait for the process to complete
            using (HttpResponseMessage response = await affinitySession.GetFinalProcessStatusAsync($"/v2/redactionCreators/{processId}"))
            {
                await this.ThrowIfGetRedactionCreatorsError(response);
                json = await response.Content.ReadAsStringAsync();
            }

            process = JObject.Parse(json);

            return new RemoteWorkFile(affinitySession, (string)process["output"]["markupFileId"], affinitySession.AffinityToken, "json");
        }

        private string BuildPostRedactionCreatorsRequestJson(RemoteWorkFile sourceDocument, RedactionMatchRule[] rules)
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

                // source
                jsonWriter.WritePropertyName("source");
                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("fileId");
                jsonWriter.WriteValue(sourceDocument.FileId);
                jsonWriter.WriteEndObject();

                // rules
                jsonWriter.WritePropertyName("rules");
                jsonWriter.WriteStartArray();
                foreach (RedactionMatchRule rule in rules)
                {
                    PrizmDocRestApiJsonSerializer.Instance.Serialize(jsonWriter, rule);
                }

                jsonWriter.WriteEndArray();

                // end input object
                jsonWriter.WriteEndObject();

                // end req JSON
                jsonWriter.WriteEndObject();

                string json = stringWriter.ToString();
                return json;
            }
        }

        private async Task ThrowIfPostRedactionCreatorsError(RedactionMatchRule[] rules, HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                ErrorData err = await ErrorData.From(response);
                string at = this.GetAt(err.RawErrorDetails);
                string msg;

                if (err.ErrorCode == "InvalidInput")
                {
                    int? ruleIndex = this.GetRuleIndexFromErrorDetailsAt(at);
                    string ruleDescription = this.GetRuleDescription(rules, ruleIndex);

                    // Example response:
                    //
                    // {
                    //     "in": "body"
                    //     "at": "input.rules[0].redactWith.fontColor",
                    //     "expected": {
                    //         "type": "string"
                    //     },
                    // }
                    if (Regex.Match(at, @"^input\.rules\[\d+\]\.redactWith\.fontColor$").Success)
                    {
                        msg = $"{ruleDescription} has invalid RedactWith.FontColor for remote server: \"{rules.ElementAt(ruleIndex.Value).RedactWith.FontColor}\"";
                        throw new RestApiErrorException(msg, err);
                    }

                    // Example response:
                    //
                    // {
                    //     "in": "body"
                    //     "at": "input.rules[0].redactWith.fillColor",
                    //     "expected": {
                    //         "type": "string"
                    //     },
                    // }
                    if (Regex.Match(at, @"^input\.rules\[\d+\]\.redactWith\.fillColor").Success)
                    {
                        msg = $"{ruleDescription} has invalid RedactWith.FillColor for remote server: \"{rules.ElementAt(ruleIndex.Value).RedactWith.FillColor}\"";
                        throw new RestApiErrorException(msg, err);
                    }

                    // Example response:
                    //
                    // {
                    //     "in": "body"
                    //     "at": "input.rules[0].redactWith.borderColor",
                    //     "expected": {
                    //         "type": "string"
                    //     },
                    // }
                    if (Regex.Match(at, @"^input\.rules\[\d+\]\.redactWith\.borderColor").Success)
                    {
                        msg = $"{ruleDescription} has invalid RedactWith.BorderColor for remote server: \"{rules.ElementAt(ruleIndex.Value).RedactWith.BorderColor}\"";
                        throw new RestApiErrorException(msg, err);
                    }

                    // Example response:
                    //
                    // {
                    //     "errorCode": "InvalidInput",
                    //     "errorDetails": {
                    //         "in": "body",
                    //         "at": "input.rules[0].redactWith.borderThickness",
                    //         "expected": {
                    //             "type": "integer",
                    //             "greaterThan": 0
                    //         }
                    //     }
                    // }
                    if (Regex.Match(at, @"^input\.rules\[\d+\]\.redactWith\.borderThickness").Success)
                    {
                        JToken expected = JObject.Parse(err.RawErrorDetails)["expected"];

                        if ((string)expected["type"] == "integer" && (int)expected["greaterThan"] == 0)
                        {
                            msg = $"{ruleDescription} has invalid RedactWith.BorderThickness for remote server: {rules.ElementAt(ruleIndex.Value).RedactWith.BorderThickness.Value}. Remote server requires a value greater than zero.";
                            throw new RestApiErrorException(msg, err);
                        }
                    }
                }

                // Example response:
                //
                // {
                //     "errorCode": "ResourceNotFound",
                //         "errorDetails": {
                //             "in": "body",
                //             "at": "input.source.fileId"
                //         }
                // }
                if (err.ErrorCode == "ResourceNotFound")
                {
                    if (at == "input.source.fileId")
                    {
                        msg = "Could not use the given RemoteWorkFile as the source document: the work file resource could not be found on the remote server. It may have expired.";
                        throw new RestApiErrorException(msg, err);
                    }
                }

                throw new RestApiErrorException(err);
            }
        }

        private async Task ThrowIfGetRedactionCreatorsError(HttpResponseMessage response)
        {
            ErrorData err = await ErrorData.From(response);

            if (err != null)
            {
                string msg;

                // Example response:
                //
                // {
                //     "processId": "BQa8BGwiY_Ee61ctACsm-w",
                //     "expirationDateTime": "2019-11-13T00:49:43.659Z",
                //     "input": {
                //         "source": {
                //             "fileId": "R-r-mEpIlhzaCf4Lxm1I3g"
                //         },
                //         "rules": [
                //             {
                //                 "find": {
                //                     "type": "regex",
                //                     "pattern": "wat"
                //                 },
                //                 "redactWith": {
                //                     "type": "RectangleRedaction"
                //                 }
                //             }
                //         ]
                //     },
                //     "state": "error",
                //     "percentComplete": 0,
                //     "errorCode": "MarkupCreationError",
                //     "affinityToken": "TD5wTFEBpgocZorewla2epxWbIMOo6GxWhK2oLJn3zo="
                // }
                if (err.ErrorCode == "MarkupCreationError")
                {
                    msg = "The remote server encountered an error when trying to create redactions for the given document. There may be a problem with the document itself.";
                    throw new RestApiErrorException(msg, err);
                }

                // Unknown error
                throw new RestApiErrorException(err);
            }
        }

        private int? GetRuleIndexFromErrorDetailsAt(string at)
        {
            Match match = Regex.Match(at, @"^input\.rules\[(\d+)\]\S*$");
            if (match.Success)
            {
                return int.TryParse(match.Groups[1].Value, out int parsedIndex) ? (int?)parsedIndex : null;
            }

            return null;
        }

        private string GetRuleDescription(RedactionMatchRule[] rules, int? ruleIndex)
        {
            string description = "RedactionMatchRule";

            if (ruleIndex.HasValue)
            {
                if (rules.Length > 1)
                {
                    description += $" at index {ruleIndex.Value}";
                }
            }

            return description;
        }
    }
}
