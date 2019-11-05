using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Defines how to connect to a remote PrizmDoc Server environment you want to use.
    /// </summary>
    public class PrizmDocServerClient
    {
        private readonly PrizmDocRestClient restClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="PrizmDocServerClient"/>
        /// class, allowing you to work with a PrizmDoc Server deployment at the
        /// specified <paramref name="baseAddress"/>.
        /// </summary>
        /// <param name="baseAddress">Base URL for connecting to your PrizmDoc
        /// Server deployment (e.g. "http://localhost:18681" or
        /// "https://my-load-balancer").</param>
        public PrizmDocServerClient(string baseAddress)
            : this(baseAddress, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrizmDocServerClient"/>
        /// class, allowing you to work with a PrizmDoc Server environment at
        /// the specified <paramref name="baseAddress"/>.
        /// </summary>
        /// <param name="baseAddress">Base URL for connecting to your PrizmDoc
        /// Server deployment (e.g. "http://localhost:18681" or
        /// "https://my-load-balancer").</param>
        public PrizmDocServerClient(Uri baseAddress)
            : this(baseAddress, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrizmDocServerClient"/>
        /// class, allowing you to work with <see
        /// href="https://cloud.accusoft.com">PrizmDoc Cloud</see>.
        /// </summary>
        /// <param name="baseAddress">Base URL to PrizmDoc Server (e.g.
        /// "https://api.accusoft.com").</param>
        /// <param name="apiKey">Your <see
        /// href="https://cloud.accusoft.com">PrizmDoc Cloud</see> API
        /// key.</param>
        public PrizmDocServerClient(string baseAddress, string apiKey)
            : this(new Uri(baseAddress), apiKey)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PrizmDocServerClient"/>
        /// class, allowing you to work with <see
        /// href="https://cloud.accusoft.com">PrizmDoc Cloud</see>.
        /// </summary>
        /// <param name="baseAddress">Base URL to PrizmDoc Server (e.g.
        /// "https://api.accusoft.com").</param>
        /// <param name="apiKey">Your <see
        /// href="https://cloud.accusoft.com">PrizmDoc Cloud</see> API
        /// key.</param>
        public PrizmDocServerClient(Uri baseAddress, string apiKey)
        {
            this.restClient = new PrizmDocRestClient(baseAddress);

            if (apiKey != null)
            {
                this.restClient.DefaultRequestHeaders.Add("Acs-Api-Key", apiKey);
            }
        }

        /// <summary>
        /// Perform OCR on a local file, producing a searchable PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{SourceDocument}, DestinationOptions)" />, returning a single <see cref="Result" />.
        /// </para>
        /// </summary>
        /// <param name="localFilePath">Path to a local file to use as input.</param>
        /// <returns>Result for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(string, DestinationOptions)" />
        public Task<Result> OcrToPdfAsync(string localFilePath) =>
                            this.OcrToPdfAsync(localFilePath, new OcrOptions());

        /// <summary>
        /// Perform OCR on a local file, producing a searchable PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{SourceDocument}, DestinationOptions)" />, returning a single <see cref="Result" />.
        /// </para>
        /// </summary>
        /// <param name="localFilePath">Path to a local file to use as input.</param>
        /// <param name="options">OCR options.</param>
        /// <returns>Result for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(string, DestinationOptions)" />
        public Task<Result> OcrToPdfAsync(string localFilePath, OcrOptions options) =>
                            this.OcrToPdfAsync(new SourceDocument(localFilePath), options);

        /// <summary>
        /// Perform OCR on pages of a single source document, producing a PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{SourceDocument}, DestinationOptions)" />, returning a single <see cref="Result" />.
        /// </para>
        /// </summary>
        /// <param name="sourceDocument">Information about the source document to use as input.</param>
        /// <returns>Result for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(SourceDocument, DestinationOptions)" />
        public Task<Result> OcrToPdfAsync(SourceDocument sourceDocument) =>
                            this.OcrToPdfAsync(sourceDocument, new OcrOptions());

        /// <summary>
        /// Perform OCR on pages of a single source document, producing a PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{SourceDocument}, DestinationOptions)" />, returning a single <see cref="Result" />.
        /// </para>
        /// </summary>
        /// <param name="sourceDocument">Information about the source document to use as input.</param>
        /// <param name="options">OCR options.</param>
        /// <returns>Result for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(SourceDocument, DestinationOptions)" />
        public Task<Result> OcrToPdfAsync(SourceDocument sourceDocument, OcrOptions options) =>
                            this.OcrToPdfAsync(new SourceDocument[] { sourceDocument }, options);

        /// <summary>
        /// Perform OCR on pages from a collection of source documents, producing a PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{SourceDocument}, DestinationOptions)" />, returning a single <see cref="Result" />.
        /// </para>
        /// </summary>
        /// <param name="sourceDocuments">Collection of source documents whose pages should be combined, in order, to form the output.</param>
        /// <returns>Result for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(IEnumerable{SourceDocument}, DestinationOptions)" />
        public Task<Result> OcrToPdfAsync(IEnumerable<SourceDocument> sourceDocuments) =>
                            this.OcrToPdfAsync(sourceDocuments, new OcrOptions());

        /// <summary>
        /// Perform OCR on pages from a collection of source documents, producing a PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{SourceDocument}, DestinationOptions)" />, returning a single <see cref="Result" />.
        /// </para>
        /// </summary>
        /// <param name="sourceDocuments">Collection of source documents whose pages should be combined, in order, to form the output.</param>
        /// <param name="options">OCR options.</param>
        /// <returns>Result for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(IEnumerable{SourceDocument}, DestinationOptions)" />
        public async Task<Result> OcrToPdfAsync(IEnumerable<SourceDocument> sourceDocuments, OcrOptions options)
        {
            return (await this.ConvertAsync(sourceDocuments, new DestinationOptions(DestinationFileFormat.Pdf)
            {
                PdfOptions = new PdfDestinationOptions
                {
                    Ocr = options,
                },
            })).Where(x => x.IsSuccess).Single();
        }

        /// <summary>
        /// Convert a local file to PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{SourceDocument}, DestinationOptions)" />, returning a single <see cref="Result" />.
        /// </para>
        /// </summary>
        /// <param name="localFilePath">Path to a local file to use as
        /// input.</param>
        /// <param name="header">Header to be appended to each page of the output
        /// document. The original page content will be left unaltered. The overall
        /// page dimensions will be expanded to accommodate the header
        /// content.</param>
        /// <param name="footer">Footer to be appended to each page of the output
        /// document. The original page content will be left unaltered. The overall
        /// page dimensions will be expanded to accommodate the footer
        /// content.</param>
        /// <returns>Result for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(string, DestinationOptions)" />
        public Task<Result> ConvertToPdfAsync(string localFilePath, HeaderFooterOptions header = null, HeaderFooterOptions footer = null) =>
                            this.ConvertToPdfAsync(new SourceDocument(localFilePath), header, footer);

        /// <summary>
        /// Convert pages of a single source document to PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{SourceDocument}, DestinationOptions)" />, returning a single <see cref="Result" />.
        /// </para>
        /// </summary>
        /// <param name="sourceDocument">Information about the source document to
        /// use as input.</param>
        /// <param name="header">Header to be appended to each page of the output
        /// document. The original page content will be left unaltered. The overall
        /// page dimensions will be expanded to accommodate the header
        /// content.</param>
        /// <param name="footer">Footer to be appended to each page of the output
        /// document. The original page content will be left unaltered. The overall
        /// page dimensions will be expanded to accommodate the footer
        /// content.</param>
        /// <returns>Result for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(SourceDocument, DestinationOptions)" />
        public Task<Result> ConvertToPdfAsync(SourceDocument sourceDocument, HeaderFooterOptions header = null, HeaderFooterOptions footer = null) =>
                            this.CombineToPdfAsync(new SourceDocument[] { sourceDocument }, header, footer);

        /// <summary>
        /// Combine pages from a collection of source documents into a PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{SourceDocument}, DestinationOptions)" />, returning a single <see cref="Result" />.
        /// </para>
        /// </summary>
        /// <param name="sourceDocuments">Collection of source documents whose pages
        /// should be combined, in order, to form the output.</param>
        /// <param name="header">Header to be appended to each page of the output
        /// document. The original page content will be left unaltered. The overall
        /// page dimensions will be expanded to accommodate the header
        /// content.</param>
        /// <param name="footer">Footer to be appended to each page of the output
        /// document. The original page content will be left unaltered. The overall
        /// page dimensions will be expanded to accommodate the footer
        /// content.</param>
        /// <returns>Result for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(string, DestinationOptions)" />
        public async Task<Result> CombineToPdfAsync(IEnumerable<SourceDocument> sourceDocuments, HeaderFooterOptions header = null, HeaderFooterOptions footer = null)
        {
            return (await this.ConvertAsync(sourceDocuments, new DestinationOptions(DestinationFileFormat.Pdf)
            {
                Header = header,
                Footer = footer,
            })).Where(x => x.IsSuccess).Single();
        }

        /// <summary>
        /// Convert a local file to a specified file format.
        /// </summary>
        /// <param name="localFilePath">Path to a local file to use as input.</param>
        /// <param name="destinationFormat"><see cref="DestinationFileFormat"/> to convert to.</param>
        /// <returns>One or more output results.</returns>
        public Task<IEnumerable<Result>> ConvertAsync(string localFilePath, DestinationFileFormat destinationFormat) =>
                                         this.ConvertAsync(new List<SourceDocument> { new SourceDocument(localFilePath) }, new DestinationOptions(destinationFormat));

        /// <summary>
        /// Convert a local file using the full available set of destination options.
        /// </summary>
        /// <param name="localFilePath">Path to a local file to use as input.</param>
        /// <param name="options">Destination options.</param>
        /// <returns>One or more output results.</returns>
        public Task<IEnumerable<Result>> ConvertAsync(string localFilePath, DestinationOptions options) =>
                                         this.ConvertAsync(new List<SourceDocument> { new SourceDocument(localFilePath) }, options);

        /// <summary>
        /// Convert pages of a single source document to a specified file format.
        /// </summary>
        /// <param name="sourceDocument">Information about the source document to use as input.</param>
        /// <param name="destinationFormat">File format to convert to.</param>
        /// <returns>One or more output results.</returns>
        public Task<IEnumerable<Result>> ConvertAsync(SourceDocument sourceDocument, DestinationFileFormat destinationFormat) =>
                                         this.ConvertAsync(new List<SourceDocument> { sourceDocument }, new DestinationOptions(destinationFormat));

        /// <summary>
        /// Convert pages of a single source document using the full available set of destination options.
        /// </summary>
        /// <param name="sourceDocument">Information about the source document to use as input.</param>
        /// <param name="options">Destination options.</param>
        /// <returns>One or more output results.</returns>
        public Task<IEnumerable<Result>> ConvertAsync(SourceDocument sourceDocument, DestinationOptions options) =>
                                         this.ConvertAsync(new List<SourceDocument> { sourceDocument }, options);

        /// <summary>
        /// Combine pages from a collection of source documents to a specified file format.
        /// </summary>
        /// <param name="sourceDocuments">Collection of source documents whose pages should be combined, in order, to form the output.</param>
        /// <param name="destinationFormat">File format to convert to.</param>
        /// <returns>One or more output results.</returns>
        public Task<IEnumerable<Result>> ConvertAsync(IEnumerable<SourceDocument> sourceDocuments, DestinationFileFormat destinationFormat) =>
                                         this.ConvertAsync(sourceDocuments, new DestinationOptions(destinationFormat));

        /// <summary>
        /// Combine pages from a collection of source documents using the full available set of destination options.
        /// This is the most flexible overload, exposing all of the potential options when performing a conversion.
        /// </summary>
        /// <param name="sourceDocuments">Collection of source documents whose pages should be combined, in order, to form the output.</param>
        /// <param name="options">Destination options.</param>
        /// <returns>One or more output results.</returns>
        public async Task<IEnumerable<Result>> ConvertAsync(IEnumerable<SourceDocument> sourceDocuments, DestinationOptions options)
        {
            AffinitySession affinitySession = this.restClient.CreateAffinitySession();

            // Convert the incoming enumerable into an array up front so that
            // we can safely go through each SourceDocument,
            // call EnsureRemoteWorkFileExists, and be sure that the
            // RemoteWorkFile.FileId will not be null.
            SourceDocument[] sourceDocumentsArray = sourceDocuments.ToArray();

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
                IEnumerable<SourceDocument> sources = result["sources"].Children().Select(source =>
                    new SourceDocument(
                        new RemoteWorkFile(
                            affinitySession,
                            (string)source["fileId"],
                            affinitySession.AffinityToken,
                            sourceDocumentsArray.Where(x => x.RemoteWorkFile.FileId == (string)source["fileId"]).Select(x => x.RemoteWorkFile.FileExtension).First()),
                        (string)source["pages"]));

                var errorCode = (string)result["errorCode"];

                if (errorCode != null)
                {
                    return new Result(errorCode, sources);
                }
                else
                {
                    var remoteWorkFile = new RemoteWorkFile(
                        affinitySession,
                        (string)result["fileId"],
                        affinitySession.AffinityToken,
                        Enum.GetName(typeof(DestinationFileFormat), options.Format).ToLowerInvariant());

                    var pageCount = (int)result["pageCount"];

                    return new Result(remoteWorkFile, pageCount, sources);
                }
            });
        }

        private async Task ThrowIfPostContentConvertersError(IEnumerable<SourceDocument> sourceDocuments, DestinationOptions options, HttpResponseMessage response)
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
                            dynamic expected = (dynamic)JObject.Parse(err.RawErrorDetails)["expected"];
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
                        msg = $"Remote server does not support applying headers or footers when producing {options.Format.ToString().ToUpperInvariant()} output.";
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
                        msg = $"Remote server does not support applying headers or footers when {options.Format.ToString()}Options.ForceOneFilePerPage is set to true.";
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
                        msg = $"When converting a CAD SourceDocument to {options.Format.ToString().ToUpperInvariant()}, you must specify {options.Format.ToString()}Options.MaxWidth or {options.Format.ToString()}Options.MaxHeight.";
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
                        msg = $"Remote server does not support combining multiple SourceDocument instances to {options.Format.ToString().ToUpperInvariant()}. When converting to {options.Format.ToString().ToUpperInvariant()}, use a single SourceDocument.";
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
                        msg = $"Remote server does not support applying headers or footers when \"pages\" is specified for a SourceDocument.";
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
                        msg = $"Remote server does not support applying headers or footers when using multiple SourceDocument instances. To apply headers or footers, use a single SourceDocument instance.";
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

                            int? lineIndex = null;
                            try
                            {
                                lineIndex = int.Parse(match.Groups[2].Value);
                            }
                            catch
                            {
                            }

                            int? positionIndex = null;
                            try
                            {
                                positionIndex = int.Parse(match.Groups[3].Value);
                            }
                            catch
                            {
                            }

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
                            unsupportedFileFormatMessage += $" for SourceDocument at index {sourceIndex.Value}";
                        }

                        string supportedFileFormatsString = null;
                        try
                        {
                            dynamic expected = (dynamic)JObject.Parse(err.RawErrorDetails)["expected"];
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

                            unsupportedFileFormatMessage += $" for SourceDocument at index {sourceIndex.Value}";
                        }

                        string supportedFileFormatsString = null;
                        try
                        {
                            dynamic expected = (dynamic)JObject.Parse(err.RawErrorDetails)["expected"];
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

        private void EnsureSourceDocumentPagesWereHonoredByTheRemoteServer(IEnumerable<SourceDocument> sourceDocuments, DestinationOptions options, string json)
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
                    SourceDocument sdkSourceDocument = sourceDocuments.Where(x => x.RemoteWorkFile != null && x.RemoteWorkFile.FileId == (string)responseSource["fileId"]).FirstOrDefault();
                    if (sdkSourceDocument != null && sdkSourceDocument.Pages != null && (string)responseSource["pages"] == null)
                    {
                        throw new RestApiErrorException($"Remote server does not support taking only specific pages of a source document when the destination type is {options.Format.ToString().ToUpperInvariant()}.");
                    }
                }
            }
        }

        private async Task ThrowIfGetContentConvertersError(IEnumerable<SourceDocument> sourceDocuments, HttpResponseMessage response)
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

                        SourceDocument source = sourceDocuments.Where(x => x.RemoteWorkFile.FileId == fileId && x.Password == password).SingleOrDefault();
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

        private string GetAt(string rawErrorDetails)
        {
            string at = string.Empty;
            try
            {
                at = (string)JObject.Parse(rawErrorDetails)["at"];
            }
            catch
            {
            }

            return at;
        }

        private int? GetSourceIndexFromErrorDetailsAt(string at)
        {
            int? i = null;
            Match match = Regex.Match(at, @"^input\.sources\[(\d+)\]\.\w+$"); // Only match on input.source[n].someProperty and no further
            if (match.Success)
            {
                try
                {
                    i = int.Parse(match.Groups[1].Value);
                }
                catch
                {
                }
            }

            return i;
        }

        private string GetSourceDescription(IEnumerable<SourceDocument> sourceDocuments, int? sourceIndex)
        {
            string description = "SourceDocument";

            if (sourceIndex.HasValue)
            {
                if (sourceDocuments.Count() > 1)
                {
                    description += $" at index {sourceIndex.Value}";
                }

                string localFilePath = sourceDocuments.ElementAt(sourceIndex.Value).LocalFilePath;
                if (localFilePath != null)
                {
                    description += $" (\"{localFilePath}\")";
                }
            }

            return description;
        }

        private async Task EnsureConversionSourceDocumentsHaveBeenUploadedWithSingleAffinity(AffinitySession session, IEnumerable<SourceDocument> sourceDocuments)
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
                SourceDocument first = sourceDocuments.FirstOrDefault();

                if (first != null)
                {
                    await first.EnsureUsableRemoteWorkFileAsync(session);
                }
            }

            // Upload source documents if necessary
            await Task.WhenAll(sourceDocuments.Select(s => s.EnsureUsableRemoteWorkFileAsync(session, mostCommonExistingAffinityToken ?? session.AffinityToken)));
        }

        private string BuildPostContentConvertersRequestJson(IEnumerable<SourceDocument> sourceDocuments, DestinationOptions options)
        {
            // Build the input JSON
            StringWriter stringWriter = new StringWriter();
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
                sourceDocuments.ToList().ForEach(s =>
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
                });
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
            }

            string json = stringWriter.ToString();
            return json;
        }
    }
}
