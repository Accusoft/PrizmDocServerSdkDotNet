// This file defines the ConvertAsync methods which wrap the
// /v2/contentConverters portion of the PrizmDoc Server REST API.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accusoft.PrizmDoc.Net.Http;
using Accusoft.PrizmDocServer.Conversion;
using Accusoft.PrizmDocServer.Exceptions;
using Accusoft.PrizmDocServer.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accusoft.PrizmDocServer
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "Documented in PrizmDocServerClient.cs")]
    public partial class PrizmDocServerClient
    {
        /// <summary>
        /// Convert a local file to a specified file format.
        /// </summary>
        /// <param name="localFilePath">Path to a local file to use as input.</param>
        /// <param name="destinationFormat"><see cref="DestinationFileFormat"/> to convert to.</param>
        /// <returns>One or more output results.</returns>
        public Task<IEnumerable<ConversionResult>> ConvertAsync(string localFilePath, DestinationFileFormat destinationFormat) =>
                                         this.ConvertAsync(new List<ConversionSourceDocument> { new ConversionSourceDocument(localFilePath) }, new DestinationOptions(destinationFormat));

        /// <summary>
        /// Convert a local file using the full available set of destination options.
        /// </summary>
        /// <param name="localFilePath">Path to a local file to use as input.</param>
        /// <param name="options">Destination options.</param>
        /// <returns>One or more output results.</returns>
        public Task<IEnumerable<ConversionResult>> ConvertAsync(string localFilePath, DestinationOptions options) =>
                                         this.ConvertAsync(new List<ConversionSourceDocument> { new ConversionSourceDocument(localFilePath) }, options);

        /// <summary>
        /// Convert pages of a single source document to a specified file format.
        /// </summary>
        /// <param name="sourceDocument">Information about the source document to use as input.</param>
        /// <param name="destinationFormat">File format to convert to.</param>
        /// <returns>One or more output results.</returns>
        public Task<IEnumerable<ConversionResult>> ConvertAsync(ConversionSourceDocument sourceDocument, DestinationFileFormat destinationFormat) =>
                                         this.ConvertAsync(new List<ConversionSourceDocument> { sourceDocument }, new DestinationOptions(destinationFormat));

        /// <summary>
        /// Convert pages of a single source document using the full available set of destination options.
        /// </summary>
        /// <param name="sourceDocument">Information about the source document to use as input.</param>
        /// <param name="options">Destination options.</param>
        /// <returns>One or more output results.</returns>
        public Task<IEnumerable<ConversionResult>> ConvertAsync(ConversionSourceDocument sourceDocument, DestinationOptions options) =>
                                         this.ConvertAsync(new List<ConversionSourceDocument> { sourceDocument }, options);

        /// <summary>
        /// Combine pages from a collection of source documents to a specified file format.
        /// </summary>
        /// <param name="sourceDocuments">Collection of source documents whose pages should be combined, in order, to form the output.</param>
        /// <param name="destinationFormat">File format to convert to.</param>
        /// <returns>One or more output results.</returns>
        public Task<IEnumerable<ConversionResult>> ConvertAsync(IEnumerable<ConversionSourceDocument> sourceDocuments, DestinationFileFormat destinationFormat) =>
                                         this.ConvertAsync(sourceDocuments, new DestinationOptions(destinationFormat));

        /// <summary>
        /// Combine pages from a collection of source documents using the full available set of destination options.
        /// This is the most flexible overload, exposing all of the potential options when performing a conversion.
        /// </summary>
        /// <param name="sourceDocuments">Collection of source documents whose pages should be combined, in order, to form the output.</param>
        /// <param name="options">Destination options.</param>
        /// <returns>One or more output results.</returns>
        public async Task<IEnumerable<ConversionResult>> ConvertAsync(IEnumerable<ConversionSourceDocument> sourceDocuments, DestinationOptions options)
        {
            AffinitySession affinitySession = this.restClient.CreateAffinitySession();

            // Convert the incoming enumerable into an array up front so that
            // we can safely go through each ConversionSourceDocument,
            // call EnsureRemoteWorkFileExists, and be sure that the
            // RemoteWorkFile.FileId will not be null.
            ConversionSourceDocument[] sourceDocumentsArray = sourceDocuments.ToArray();

            // Prevent accidental use of the original argument.
            sourceDocuments = null;

            await this.EnsureConversionSourceDocumentsHaveBeenUploadedWithSingleAffinity(affinitySession, sourceDocumentsArray);
            string json = this.BuildPostContentConvertersRequestJson(sourceDocumentsArray, options);

            // Start the conversion
            using (HttpResponseMessage response = await affinitySession.PostAsync("/v2/contentConverters", new StringContent(json, Encoding.UTF8, "application/json")))
            {
                await this.ThrowIfPostContentConvertersError(sourceDocumentsArray, options, response);
                json = await response.Content.ReadAsStringAsync();
            }

            this.EnsureSourceDocumentPagesWereHonoredByTheRemoteServer(sourceDocumentsArray, options, json);

            JObject process = JObject.Parse(json);
            string processId = (string)process["processId"];

            // Wait for the conversion to complete
            using (HttpResponseMessage response = await affinitySession.GetFinalProcessStatusAsync($"/v2/contentConverters/{processId}"))
            {
                await this.ThrowIfGetContentConvertersError(sourceDocumentsArray, response);
                json = await response.Content.ReadAsStringAsync();
            }

            process = JObject.Parse(json);

            // Did the process error?
            if ((string)process["state"] != "complete")
            {
                throw new Exception("The conversion failed:\n" + json);
            }

            return process["output"]["results"].Children().Select(result =>
            {
                IEnumerable<ConversionSourceDocument> sources = result["sources"].Children().Select(source =>
                    new ConversionSourceDocument(
                        new RemoteWorkFile(
                            affinitySession,
                            (string)source["fileId"],
                            affinitySession.AffinityToken,
                            sourceDocumentsArray.Where(x => x.RemoteWorkFile.FileId == (string)source["fileId"]).Select(x => x.RemoteWorkFile.FileExtension).First()),
                        (string)source["pages"]));

                string errorCode = (string)result["errorCode"];

                if (errorCode != null)
                {
                    return new ConversionResult(errorCode, sources);
                }

                var remoteWorkFile = new RemoteWorkFile(
                    affinitySession,
                    (string)result["fileId"],
                    affinitySession.AffinityToken,
                    Enum.GetName(typeof(DestinationFileFormat), options.Format).ToLowerInvariant());

                int pageCount = (int)result["pageCount"];

                return new ConversionResult(remoteWorkFile, pageCount, sources);
            });
        }

        private string BuildPostContentConvertersRequestJson(ConversionSourceDocument[] sourceDocuments, DestinationOptions options)
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

                // sources
                jsonWriter.WritePropertyName("sources");
                jsonWriter.WriteStartArray();

                foreach (ConversionSourceDocument s in sourceDocuments)
                {
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName("fileId");
                    jsonWriter.WriteValue(s.RemoteWorkFile.FileId);
                    if (s.Pages != null)
                    {
                        jsonWriter.WritePropertyName("pages");
                        jsonWriter.WriteValue(s.Pages);
                    }

                    if (s.Password != null)
                    {
                        jsonWriter.WritePropertyName("password");
                        jsonWriter.WriteValue(s.Password);
                    }

                    jsonWriter.WriteEndObject();
                }

                jsonWriter.WriteEndArray();

                // _features.pdfToDocx.enabled
                if (options.Format == DestinationFileFormat.Docx)
                {
                    jsonWriter.WritePropertyName("_features");
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName("pdfToDocx");
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName("enabled");
                    jsonWriter.WriteValue(true);
                    jsonWriter.WriteEndObject();
                    jsonWriter.WriteEndObject();
                }

                // dest
                jsonWriter.WritePropertyName("dest");
                PrizmDocRestApiJsonSerializer.Instance.Serialize(jsonWriter, options);

                // end input object
                jsonWriter.WriteEndObject();

                // end req JSON
                jsonWriter.WriteEndObject();

                string json = stringWriter.ToString();
                return json;
            }
        }

        private async Task ThrowIfPostContentConvertersError(ConversionSourceDocument[] sourceDocuments, DestinationOptions options, HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                ErrorData err = await ErrorData.From(response);

                if (err != null)
                {
                    string msg = null;

                    string at = this.GetAt(err.RawErrorDetails);
                    int? sourceIndex = this.GetSourceIndexFromErrorDetailsAt(at);
                    string sourceDescription = this.GetSourceDescription(sourceDocuments, sourceIndex);
                    string format = options.Format.ToString();

                    // Example response:
                    //
                    // {
                    //   "input": {
                    //     "dest": {
                    //       "format": "pdf",
                    //       "pdfOptions": {
                    //         "forceOneFilePerPage": false
                    //       }
                    //     },
                    //     "sources": [{
                    //       "fileId": "a",
                    //       "pages": ""
                    //     }, {
                    //       "fileId": "b",
                    //       "pages": "2-"
                    //     }, {
                    //       "fileId": "c",
                    //       "pages": ""
                    //     }]
                    //   },
                    //   "minSecondsAvailable": 18000,
                    //   "errorCode": "WorkFileDoesNotExist",
                    //   "errorDetails": {
                    //     "in": "body",
                    //     "at": "input.sources[1].fileId"
                    //   }
                    // }
                    if (err.ErrorCode == "WorkFileDoesNotExist")
                    {
                        msg = $"{sourceDescription} refers to a remote work file which does not exist. It may have expired.";
                        throw new RestApiErrorException(msg, err);
                    }

                    // Example response:
                    //
                    // {
                    //   "errorCode": "InvalidPageSyntax",
                    //   "errorDetails": {
                    //     "in": "body",
                    //     "at": "input.sources[0].pages"
                    //   },
                    //   "input": {
                    //     "dest": {
                    //       "format": "pdf",
                    //       "pdfOptions": {
                    //         "forceOneFilePerPage": false
                    //       }
                    //     },
                    //     "sources": [{
                    //       "fileId": "a",
                    //       "pages": "wat"
                    //     }]
                    //   },
                    //   "minSecondsAvailable": 1200
                    // }
                    if (err.ErrorCode == "InvalidPageSyntax")
                    {
                        msg = $"{sourceDescription} has an invalid value for \"pages\". A valid pages value is a string like \"1\", \"1,3,5-10\", or \"2-\" (just like in a print dialog).";
                        throw new RestApiErrorException(msg, err);
                    }

                    if (err.ErrorCode == "InvalidInput")
                    {
                        // Example response:
                        //
                        // {
                        //   "errorCode": "InvalidInput",
                        //   "errorDetails": {
                        //     "in": "body",
                        //     "at": "input.dest.pdfOptions.ocr.language",
                        //     "expected": {
                        //       "enum": ["english"]
                        //     }
                        //   }
                        // }
                        if (at == "input.dest.pdfOptions.ocr.language")
                        {
                            JToken expected = JObject.Parse(err.RawErrorDetails)["expected"];
                            string requestedLanguage = options.PdfOptions.Ocr.Language;
                            IEnumerable<string> supportedLanguages = ((System.Collections.IEnumerable)expected["enum"]).Cast<dynamic>().Select(x => (string)x);

                            msg = $"Unsupported OCR language \"{requestedLanguage}\". The remote server only supports the following OCR languages: {string.Join(", ", supportedLanguages.Select(x => "\"" + x + "\"").ToArray())}.";
                            throw new RestApiErrorException(msg, err);
                        }

                        // Example response:
                        //
                        // {
                        //   "input": {
                        //     "dest": {
                        //       "format": "pdf",
                        //       "header": {
                        //         "lines": [
                        //           ["Test", "", ""]
                        //         ],
                        //         "fontSize": "wat"
                        //       },
                        //       "pdfOptions": {
                        //         "forceOneFilePerPage": false
                        //       }
                        //     },
                        //     "sources": [{
                        //       "fileId": "a",
                        //       "pages": ""
                        //     }]
                        //   },
                        //   "minSecondsAvailable": 1200,
                        //   "errorCode": "InvalidInput",
                        //   "errorDetails": {
                        //     "in": "body",
                        //     "at": "input.dest.header.fontSize"
                        //   }
                        // }
                        if (at == "input.dest.header.fontSize")
                        {
                            msg = $"Invalid Header.FontSize value for remote server: \"{options.Header.FontSize}\"";
                            throw new RestApiErrorException(msg, err);
                        }

                        // Example response:
                        //
                        //
                        // {
                        //   "input": {
                        //     "dest": {
                        //       "format": "pdf",
                        //       "footer": {
                        //         "lines": [
                        //           ["Test", "", ""]
                        //         ],
                        //         "fontSize": "wat"
                        //       },
                        //       "pdfOptions": {
                        //         "forceOneFilePerPage": false
                        //       }
                        //     },
                        //     "sources": [{
                        //       "fileId": "a",
                        //       "pages": ""
                        //     }]
                        //   },
                        //   "minSecondsAvailable": 1200,
                        //   "errorCode": "InvalidInput",
                        //   "errorDetails": {
                        //     "in": "body",
                        //     "at": "input.dest.header.fontSize"
                        //   }
                        // }
                        if (at == "input.dest.footer.fontSize")
                        {
                            msg = $"Invalid Footer.FontSize value for remote server: \"{options.Footer.FontSize}\"";
                            throw new RestApiErrorException(msg, err);
                        }

                        // Example response:
                        // {
                        //   "input": {
                        //     "dest": {
                        //       "format": "pdf",
                        //       "header": {
                        //         "lines": [
                        //           ["Test", "", ""]
                        //         ],
                        //         "color": "rrrrED!"
                        //       },
                        //       "pdfOptions": {
                        //         "forceOneFilePerPage": false
                        //       }
                        //     },
                        //     "sources": [{
                        //       "fileId": "a",
                        //       "pages": ""
                        //     }]
                        //   },
                        //   "minSecondsAvailable": 1200,
                        //   "errorCode": "InvalidInput",
                        //   "errorDetails": {
                        //     "in": "body",
                        //     "at": "input.dest.footer.color"
                        //   }
                        // }
                        if (at == "input.dest.header.color")
                        {
                            msg = $"Invalid Header.Color value for remote server: \"{options.Header.Color}\"";
                            throw new RestApiErrorException(msg, err);
                        }

                        // Example response:
                        // {
                        //   "input": {
                        //     "dest": {
                        //       "format": "pdf",
                        //       "footer": {
                        //         "lines": [
                        //           ["Test", "", ""]
                        //         ],
                        //         "color": "Bluuuuue"
                        //       },
                        //       "pdfOptions": {
                        //         "forceOneFilePerPage": false
                        //       }
                        //     },
                        //     "sources": [{
                        //       "fileId": "a",
                        //       "pages": ""
                        //     }]
                        //   },
                        //   "minSecondsAvailable": 1200,
                        //   "errorCode": "InvalidInput",
                        //   "errorDetails": {
                        //     "in": "body",
                        //     "at": "input.dest.footer.color"
                        //   }
                        // }
                        if (at == "input.dest.footer.color")
                        {
                            msg = $"Invalid Footer.Color value for remote server: \"{options.Footer.Color}\"";
                            throw new RestApiErrorException(msg, err);
                        }
                    }

                    // Example response:
                    //
                    // {
                    //   "input": {
                    //     "dest": {
                    //       "format": "jpeg",
                    //       "footer": {
                    //         "lines": [
                    //           ["Test", "", ""]
                    //         ]
                    //       }
                    //     },
                    //     "sources": [{
                    //       "fileId": "a"
                    //     }]
                    //   },
                    //   "minSecondsAvailable": 1200,
                    //   "errorCode": "UnsupportedDestinationFormatWhenUsingHeaderOrFooter",
                    //   "errorDetails": {
                    //     "in": "body",
                    //     "at": "input.dest.format",
                    //     "expected": {
                    //       "enum": ["pdf", "tiff"]
                    //     }
                    //   }
                    // }
                    if (err.ErrorCode == "UnsupportedDestinationFormatWhenUsingHeaderOrFooter")
                    {
                        msg = $"Remote server does not support applying headers or footers when producing {format.ToUpperInvariant()} output.";
                        throw new RestApiErrorException(msg, err);
                    }

                    // Example response:
                    //
                    // {
                    //   "input": {
                    //     "dest": {
                    //       "format": "tiff",
                    //       "tiffOptions": {
                    //         "forceOneFilePerPage": true
                    //       },
                    //       "footer": {
                    //         "lines": [
                    //           ["Test", "", ""]
                    //         ]
                    //       }
                    //     },
                    //     "sources": [{
                    //       "fileId": "a",
                    //       "pages": ""
                    //     }]
                    //   },
                    //   "minSecondsAvailable": 1200,
                    //   "errorCode": "ForceOneFilePerPageNotSupportedWhenUsingHeaderOrFooter",
                    //   "errorDetails": {
                    //     "in": "body",
                    //     "at": "input.dest.tiffOptions.forceOneFilePerPage",
                    //     "expected": {
                    //       "value": false
                    //     }
                    //   }
                    // }
                    if (err.ErrorCode == "ForceOneFilePerPageNotSupportedWhenUsingHeaderOrFooter")
                    {
                        msg = $"Remote server does not support applying headers or footers when {format}Options.ForceOneFilePerPage is set to true.";
                        throw new RestApiErrorException(msg, err);
                    }

                    // Example response:
                    //
                    // {
                    //   "errorCode": "InvalidDimensionValue",
                    //   "errorDetails": {
                    //     "in": "body",
                    //     "at": "input.dest.jpegOptions.maxWidth"
                    //   },
                    //   "input": {
                    //     "dest": {
                    //       "format": "jpeg",
                    //       "jpegOptions": {
                    //         "maxWidth": "wat"
                    //       }
                    //     },
                    //     "sources": [{
                    //       "fileId": "a"
                    //     }]
                    //   },
                    //   "minSecondsAvailable": 1200
                    // }
                    if (err.ErrorCode == "InvalidDimensionValue")
                    {
                        switch (at)
                        {
                            case "input.dest.jpegOptions.maxWidth":
                                msg = $"Invalid JpegOptions.MaxWidth for remote server: \"{options.JpegOptions.MaxWidth}\". Try using a CSS-style string, like \"800px\".";
                                break;
                            case "input.dest.jpegOptions.maxHeight":
                                msg = $"Invalid JpegOptions.MaxHeight for remote server: \"{options.JpegOptions.MaxHeight}\". Try using a CSS-style string, like \"600px\".";
                                break;
                            case "input.dest.pngOptions.maxWidth":
                                msg = $"Invalid PngOptions.MaxWidth for remote server: \"{options.PngOptions.MaxWidth}\". Try using a CSS-style string, like \"800px\".";
                                break;
                            case "input.dest.pngOptions.maxHeight":
                                msg = $"Invalid PngOptions.MaxHeight for remote server: \"{options.PngOptions.MaxHeight}\". Try using a CSS-style string, like \"600px\".";
                                break;
                            case "input.dest.tiffOptions.maxWidth":
                                msg = $"Invalid TiffOptions.MaxWidth for remote server: \"{options.TiffOptions.MaxWidth}\". Try using a CSS-style string, like \"800px\".";
                                break;
                            case "input.dest.tiffOptions.maxHeight":
                                msg = $"Invalid TiffOptions.MaxHeight for remote server: \"{options.TiffOptions.MaxHeight}\". Try using a CSS-style string, like \"600px\".";
                                break;
                            default:
                                break;
                        }

                        throw new RestApiErrorException(msg, err);
                    }

                    // Example response:
                    //
                    // {
                    //   "input": {
                    //     "dest": {
                    //       "format": "png",
                    //       "pngOptions": {
                    //         "forceOneFilePerPage": false
                    //       }
                    //     },
                    //     "sources": [{
                    //       "fileId": "a",
                    //       "pages": ""
                    //     }]
                    //   },
                    //   "minSecondsAvailable": 1200,
                    //   "errorCode": "MaxWidthOrMaxHeightMustBeSpecifiedWhenRasterizingCadInput",
                    //   "errorDetails": {
                    //     "in": "body",
                    //     "at": "input.dest.pngOptions"
                    //   }
                    // }
                    if (err.ErrorCode == "MaxWidthOrMaxHeightMustBeSpecifiedWhenRasterizingCadInput" &&
                        (options.Format == DestinationFileFormat.Jpeg ||
                         options.Format == DestinationFileFormat.Png ||
                         options.Format == DestinationFileFormat.Tiff))
                    {
                        msg = $"When converting a CAD ConversionSourceDocument to {format.ToUpperInvariant()}, you must specify {format}Options.MaxWidth or {format}Options.MaxHeight.";
                        throw new RestApiErrorException(msg, err);
                    }

                    // Example response:
                    //
                    // {
                    //   "errorCode": "MultipleSourcesAreNotSupportedForThisDestinationFormat",
                    //   "errorDetails": {
                    //     "in": "body",
                    //     "at": "input.sources"
                    //   }
                    // }
                    if (err.ErrorCode == "MultipleSourcesAreNotSupportedForThisDestinationFormat")
                    {
                        msg = $"Remote server does not support combining multiple ConversionSourceDocument instances to {format.ToUpperInvariant()}. When converting to {format.ToUpperInvariant()}, use a single ConversionSourceDocument.";
                        throw new RestApiErrorException(msg, err);
                    }

                    // Example response:
                    //
                    // {
                    //   "input": {
                    //     "dest": {
                    //       "format": "pdf",
                    //       "header": {
                    //         "lines": [
                    //           ["Acme", "", ""]
                    //         ]
                    //       },
                    //       "pdfOptions": {
                    //         "forceOneFilePerPage": false
                    //       }
                    //     },
                    //     "sources": [{
                    //       "fileId": "a",
                    //       "pages": "2-"
                    //     }]
                    //   },
                    //   "minSecondsAvailable": 1200,
                    //   "errorCode": "PagesPropertyNotSupportedWhenUsingHeaderOrFooter",
                    //   "errorDetails": {
                    //     "in": "body",
                    //     "at": "input.sources[0].pages"
                    //   }
                    // }
                    if (err.ErrorCode == "PagesPropertyNotSupportedWhenUsingHeaderOrFooter")
                    {
                        msg = $"Remote server does not support applying headers or footers when \"pages\" is specified for a ConversionSourceDocument.";
                        throw new RestApiErrorException(msg, err);
                    }

                    // Example response:
                    //
                    // {
                    //   "errorCode": "MultipleSourcesAreNotSupportedForThisDestinationFormat",
                    //   "errorDetails": {
                    //     "in": "body",
                    //     "at": "input.sources"
                    //   }
                    // }
                    if (err.ErrorCode == "MultipleSourceDocumentsNotSupportedWhenUsingHeaderOrFooter")
                    {
                        msg = $"Remote server does not support applying headers or footers when using multiple ConversionSourceDocument instances. To apply headers or footers, use a single ConversionSourceDocument instance.";
                        throw new RestApiErrorException(msg, err);
                    }

                    // Example response:
                    //
                    // {
                    //   "input": {
                    //     "dest": {
                    //       "format": "pdf",
                    //       "header": {
                    //         "lines": [
                    //           ["", "", "Page {{wat}}"]
                    //         ]
                    //       },
                    //       "pdfOptions": {
                    //         "forceOneFilePerPage": false
                    //       }
                    //     },
                    //     "sources": [{
                    //       "fileId": "53hLqJR9sW55cuQcnOpJhA",
                    //       "pages": ""
                    //     }]
                    //   },
                    //   "minSecondsAvailable": 1200,
                    //   "errorCode": "UnrecognizedExpression",
                    //   "errorDetails": {
                    //     "in": "body",
                    //     "at": "input.dest.header.lines[0][2]"
                    //   }
                    // }
                    if (err.ErrorCode == "UnrecognizedExpression")
                    {
                        msg = null;

                        Match match = Regex.Match(at, @"^input\.dest\.(header|footer)\.lines\[(\d+)\]\[(\d+)\]$"); // Only match on input.source[n].someProperty and no further

                        if (match.Success)
                        {
                            string headerFooter = match.Groups[1].Value;
                            string headerOrFooterPropertyName = headerFooter.Substring(0, 1).ToUpperInvariant() + headerFooter.Substring(1);

                            int? lineIndex = int.TryParse(match.Groups[2].Value, out int parsedLineIndex) ? (int?)parsedLineIndex : null;
                            int? positionIndex = int.TryParse(match.Groups[3].Value, out int parsedPositionIndex) ? (int?)parsedPositionIndex : null;

                            string position = null;
                            if (positionIndex.HasValue)
                            {
                                switch (positionIndex.Value)
                                {
                                    case 0:
                                        position = "Left";
                                        break;
                                    case 1:
                                        position = "Center";
                                        break;
                                    case 2:
                                        position = "Right";
                                        break;
                                    default:
                                        break;
                                }
                            }

                            if (lineIndex.HasValue && position != null)
                            {
                                msg = $"Remote server rejected {headerOrFooterPropertyName}.Lines[{lineIndex.Value}].{position} because it contains a dynamic expression (text in double curly braces) that it did not recognize.";
                            }
                        }

                        throw new RestApiErrorException(msg, err);
                    }

                    // Example response:
                    //
                    // {
                    //   "input": {
                    //     "dest": {
                    //       "format": "pdf",
                    //       "pdfOptions": {
                    //         "forceOneFilePerPage": false
                    //       }
                    //     },
                    //     "sources": [{
                    //       "fileId": "qjOgl2N45dasWTFTi4UjUw",
                    //       "pages": ""
                    //     }]
                    //   },
                    //   "minSecondsAvailable": 1200,
                    //   "errorCode": "UnsupportedSourceFileFormat",
                    //   "errorDetails": {
                    //     "in": "body",
                    //     "at": "input.sources[0].fileId",
                    //     "expected": {
                    //       "enum": ["bmp", "cal", "cals", "csv", "cur", "cut", "dcim", "dcm", "dcx", "dgn", "dib", "dicm", "dicom", "doc", "docm", "docx", "dot", "dotm", "dotx", "dwf", "dwg", "dxf", "eml", "emz", "fodg", "fodp", "fods", "fodt", "gif", "htm", "html", "ico", "img", "jp2", "jpc", "jpeg", "jpg", "jpx", "msg", "ncr", "odg", "odp", "ods", "odt", "otg", "otp", "ots", "ott", "pbm", "pcd", "pct", "pcx", "pdf", "pgm", "pic", "pict", "png", "pot", "potm", "potx", "ppm", "pps", "ppsm", "ppsx", "ppt", "pptm", "pptx", "psb", "psd", "ras", "rtf", "sct", "sgi", "tga", "tif", "tiff", "tpic", "txt", "vdx", "vsd", "vsdm", "vsdx", "wbmp", "wmf", "wmz", "wpg", "xbm", "xhtml", "xls", "xlsm", "xlsx", "xlt", "xltm", "xltx", "xwd"]
                    //     }
                    //   }
                    // }
                    if (err.ErrorCode == "UnsupportedSourceFileFormat")
                    {
                        string actualFileExtension = null;
                        try
                        {
                            actualFileExtension = sourceDocuments.ElementAt(sourceIndex.Value).RemoteWorkFile.FileExtension;
                        }
                        catch
                        {
                        }

                        string unsupportedFileFormatMessage = null;
                        if (actualFileExtension != null)
                        {
                            unsupportedFileFormatMessage = $"Unsupported file format \"{actualFileExtension}\"";
                        }
                        else
                        {
                            unsupportedFileFormatMessage = $"Unsupported file format";
                        }

                        if (sourceDocuments.Count() > 1)
                        {
                            unsupportedFileFormatMessage += $" for ConversionSourceDocument at index {sourceIndex.Value}";
                        }

                        string supportedFileFormatsString = null;
                        try
                        {
                            JToken expected = JObject.Parse(err.RawErrorDetails)["expected"];
                            IEnumerable<string> supportedFileFormats = ((System.Collections.IEnumerable)expected["enum"]).Cast<dynamic>().Select(x => (string)x);
                            supportedFileFormatsString = string.Join(", ", supportedFileFormats.Select(x => "\"" + x + "\"").ToArray());
                        }
                        catch
                        {
                        }

                        string supportedFileFormatsMessage = supportedFileFormatsString != null ? $"The remote server only supports the following input file formats: {supportedFileFormatsString}" : null;

                        msg = string.Join(". ", new List<string> { unsupportedFileFormatMessage, supportedFileFormatsMessage }.Where(x => x != null));

                        throw new RestApiErrorException(msg, err);
                    }

                    // Example response:
                    //
                    // {
                    //   "errorCode": "UnsupportedSourceFileFormatForOCR",
                    //   "errorDetails": {
                    //     "in": "body",
                    //     "at": "input.sources[0].fileId",
                    //     "expected": {
                    //       "enum": ["bmp", "cal", "cals", "cur", "cut", "dcim", "dcm", "dcx", "dib", "dicm", "dicom", "emz", "gif", "ico", "img", "jp2", "jpc", "jpeg", "jpg", "jpx", "ncr", "pbm", "pcd", "pct", "pcx", "pdf", "pgm", "pic", "pict", "png", "ppm", "psb", "psd", "ras", "sct", "sgi", "tga", "tif", "tiff", "tpic", "wbmp", "wmz", "wpg", "xbm", "xwd"]
                    //     }
                    //   }
                    // }
                    if (err.ErrorCode == "UnsupportedSourceFileFormatForOCR")
                    {
                        string actualFileExtension = null;
                        try
                        {
                            actualFileExtension = sourceDocuments.ElementAt(sourceIndex.Value).RemoteWorkFile.FileExtension;
                        }
                        catch
                        {
                        }

                        string unsupportedFileFormatMessage = null;
                        if (actualFileExtension != null)
                        {
                            unsupportedFileFormatMessage = $"Unsupported file format when performing OCR, \"{actualFileExtension}\"";
                        }
                        else
                        {
                            unsupportedFileFormatMessage = $"Unsupported file format when performing OCR";
                        }

                        if (sourceDocuments.Count() > 1)
                        {
                            if (actualFileExtension != null)
                            {
                                unsupportedFileFormatMessage += ",";
                            }

                            unsupportedFileFormatMessage += $" for ConversionSourceDocument at index {sourceIndex.Value}";
                        }

                        string supportedFileFormatsString = null;
                        try
                        {
                            JToken expected = JObject.Parse(err.RawErrorDetails)["expected"];
                            IEnumerable<string> supportedFileFormats = ((System.Collections.IEnumerable)expected["enum"]).Cast<dynamic>().Select(x => (string)x);
                            supportedFileFormatsString = string.Join(", ", supportedFileFormats.Select(x => "\"" + x + "\"").ToArray());
                        }
                        catch
                        {
                        }

                        string supportedFileFormatsMessage = supportedFileFormatsString != null ? $"The remote server only supports the following input file formats when performing OCR: {supportedFileFormatsString}" : null;

                        msg = string.Join(". ", new List<string> { unsupportedFileFormatMessage, supportedFileFormatsMessage }.Where(x => x != null));

                        throw new RestApiErrorException(msg, err);
                    }

                    if (err.ErrorCode == "FeatureNotLicensed")
                    {
                        // {
                        //   "errorCode": "FeatureNotLicensed",
                        //   "errorDetails": {
                        //     "in": "body",
                        //     "at": "input.dest.pdfOptions.ocr"
                        //   }
                        // }
                        if (at == "input.dest.pdfOptions.ocr")
                        {
                            msg = "Remote server is not licensed to perform OCR when producing PDF output.";

                            throw new RestApiErrorException(msg, err);
                        }
                    }

                    // {
                    //   "errorCode": "LicenseCouldNotBeVerified"
                    // }
                    if (err.ErrorCode == "LicenseCouldNotBeVerified")
                    {
                        msg = "Remote server does not have a valid license.";

                        throw new RestApiErrorException(msg, err);
                    }

                    throw new RestApiErrorException(err);
                }
            }
        }

        private void EnsureSourceDocumentPagesWereHonoredByTheRemoteServer(ConversionSourceDocument[] sourceDocuments, DestinationOptions options, string json)
        {
            if (options.Format != DestinationFileFormat.Pdf &&
                options.Format != DestinationFileFormat.Tiff)
            {
                JArray sources = null;
                try
                {
                    sources = (JArray)JObject.Parse(json)["input"]["sources"];
                }
                catch
                {
                }

                for (int i = 0; i < sources.Count; i++)
                {
                    JToken responseSource = sources[i];
                    ConversionSourceDocument sdkSourceDocument = sourceDocuments.Where(x => x.RemoteWorkFile != null && x.RemoteWorkFile.FileId == (string)responseSource["fileId"]).FirstOrDefault();
                    if (sdkSourceDocument != null && sdkSourceDocument.Pages != null && (string)responseSource["pages"] == null)
                    {
                        throw new RestApiErrorException($"Remote server does not support taking only specific pages of a source document when the destination type is {options.Format.ToString().ToUpperInvariant()}.");
                    }
                }
            }
        }

        private async Task ThrowIfGetContentConvertersError(ConversionSourceDocument[] sourceDocuments, HttpResponseMessage response)
        {
            ErrorData err = await ErrorData.From(response);

            if (err != null)
            {
                string at = this.GetAt(err.RawErrorDetails);

                JArray results = null;
                try
                {
                    results = (JArray)JObject.Parse(err.RawBody)["output"]["results"];
                }
                catch
                {
                }

                if (err.ErrorCode == "CouldNotConvert")
                {
                    // Example response:
                    //
                    // {
                    //   "input": {
                    //     "dest": {
                    //       "format": "pdf",
                    //       "footer": {
                    //         "lines": [
                    //           ["Test", "", ""]
                    //         ],
                    //         "fontFamily": "Lexicon"
                    //       },
                    //       "pdfOptions": {
                    //         "forceOneFilePerPage": false
                    //       }
                    //     },
                    //     "sources": [{
                    //       "fileId": "a",
                    //       "pages": ""
                    //     }]
                    //   },
                    //   "expirationDateTime": "2019-09-05T20:11:26.042Z",
                    //   "processId": "x",
                    //   "state": "error",
                    //   "percentComplete": 100,
                    //   "errorCode": "CouldNotConvert",
                    //   "output": {
                    //     "results": [{
                    //       "errorCode": "RequestedHeaderOrFooterFontIsNotAvailable",
                    //       "sources": [{
                    //         "fileId": "a"
                    //       }]
                    //     }]
                    //   }
                    // }
                    if (err.InnerErrors.Count == 1 && err.InnerErrors[0].ErrorCode == "RequestedHeaderOrFooterFontIsNotAvailable")
                    {
                        string msg = "Header.FontFamily or Footer.FontFamily specifies a font which is not available on the remote server.";
                        throw new RestApiErrorException(msg, err);
                    }

                    // Example response:
                    //
                    // {
                    //   "input": {
                    //     "dest": {
                    //       "format": "pdf",
                    //       "pdfOptions": {
                    //         "forceOneFilePerPage": false
                    //       }
                    //     },
                    //     "sources": [{
                    //       "fileId": "a",
                    //       "pages": ""
                    //     }, {
                    //       "fileId": "b",
                    //       "password": "wrong",
                    //       "pages": ""
                    //     }, {
                    //       "fileId": "a",
                    //       "pages": ""
                    //     }]
                    //   },
                    //   "expirationDateTime": "2019-09-05T20:13:40.037Z",
                    //   "processId": "x",
                    //   "state": "error",
                    //   "percentComplete": 100,
                    //   "errorCode": "CouldNotConvert",
                    //   "output": {
                    //     "results": [{
                    //       "errorCode": "InvalidPassword",
                    //       "sources": [{
                    //         "fileId": "a",
                    //         "password": "wrong"
                    //       }]
                    //     }]
                    //   }
                    // }
                    if (err.InnerErrors.Count == 1 && err.InnerErrors[0].ErrorCode == "InvalidPassword")
                    {
                        string fileId = null;
                        try
                        {
                            fileId = (string)results[0]["sources"][0]["fileId"];
                        }
                        catch
                        {
                        }

                        string password = null;
                        try
                        {
                            password = (string)results[0]["sources"][0]["password"];
                        }
                        catch
                        {
                        }

                        ConversionSourceDocument source = sourceDocuments.Where(x => x.RemoteWorkFile.FileId == fileId && x.Password == password).SingleOrDefault();
                        int? sourceIndex = source != null ? new int?(sourceDocuments.ToList().IndexOf(source)) : null;
                        string sourceDescription = this.GetSourceDescription(sourceDocuments, sourceIndex);

                        string messageOpening = (source != null && source.Password == null) ? "Password required" : "Invalid password";
                        string msg = $"{messageOpening} for {sourceDescription}.";

                        throw new RestApiErrorException(msg, err);
                    }
                }

                throw new RestApiErrorException(err);
            }
        }

        private async Task EnsureConversionSourceDocumentsHaveBeenUploadedWithSingleAffinity(AffinitySession session, ConversionSourceDocument[] sourceDocuments)
        {
            // Determine the most-common affinity token in the set.
            string mostCommonExistingAffinityToken = sourceDocuments
              .Where(x => x.RemoteWorkFile != null && x.RemoteWorkFile.AffinityToken != null)
              .GroupBy(x => x.RemoteWorkFile.AffinityToken)
              .OrderByDescending(groupings => groupings.Count())
              .Select(grouping => grouping.Key)
              .FirstOrDefault();

            // If an affinity token is not set,
            // wait for the first upload to finish on its own so that
            // the REST client knows what affinity token to use for the
            // remaining uploads.
            if (mostCommonExistingAffinityToken == null && session.AffinityToken == null)
            {
                ConversionSourceDocument first = sourceDocuments.FirstOrDefault();

                if (first != null)
                {
                    await first.EnsureUsableRemoteWorkFileAsync(session);
                }
            }

            // Upload source documents if necessary
            await Task.WhenAll(sourceDocuments.Select(s => s.EnsureUsableRemoteWorkFileAsync(session, mostCommonExistingAffinityToken ?? session.AffinityToken)));
        }

        private int? GetSourceIndexFromErrorDetailsAt(string at)
        {
            Match match = Regex.Match(at, @"^input\.sources\[(\d+)\]\.\w+$"); // Only match on input.source[n].someProperty and no further
            if (match.Success)
            {
                return int.TryParse(match.Groups[1].Value, out int parsedIndex) ? (int?)parsedIndex : null;
            }

            return null;
        }

        private string GetSourceDescription(ConversionSourceDocument[] sourceDocuments, int? sourceIndex)
        {
            string description = "ConversionSourceDocument";

            if (sourceIndex.HasValue)
            {
                if (sourceDocuments.Count() > 1)
                {
                    description += $" at index {sourceIndex.Value}";
                }

                string localFilePath = sourceDocuments[sourceIndex.Value].LocalFilePath;
                if (localFilePath != null)
                {
                    description += $" (\"{localFilePath}\")";
                }
            }

            return description;
        }
    }
}
