// This file defines the OcrToPdfAsync methods which are convenience wrappers
// around ConvertAsync.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Conversion;

namespace Accusoft.PrizmDocServer
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "Documented in PrizmDocServerClient_Constructor.cs")]
    public partial class PrizmDocServerClient
    {
        /// <summary>
        /// Perform OCR on a local file, producing a searchable PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{ConversionSourceDocument}, DestinationOptions)" />, returning a single <see cref="ConversionResult" />.
        /// </para>
        /// </summary>
        /// <param name="localFilePath">Path to a local file to use as input.</param>
        /// <returns>ConversionResult for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(string, DestinationOptions)" />
        public Task<ConversionResult> OcrToPdfAsync(string localFilePath) =>
                            this.OcrToPdfAsync(localFilePath, new OcrOptions());

        /// <summary>
        /// Perform OCR on a local file, producing a searchable PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{ConversionSourceDocument}, DestinationOptions)" />, returning a single <see cref="ConversionResult" />.
        /// </para>
        /// </summary>
        /// <param name="localFilePath">Path to a local file to use as input.</param>
        /// <param name="options">OCR options.</param>
        /// <returns>ConversionResult for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(string, DestinationOptions)" />
        public Task<ConversionResult> OcrToPdfAsync(string localFilePath, OcrOptions options) =>
                            this.OcrToPdfAsync(new ConversionSourceDocument(localFilePath), options);

        /// <summary>
        /// Perform OCR on pages of a single source document, producing a PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{ConversionSourceDocument}, DestinationOptions)" />, returning a single <see cref="ConversionResult" />.
        /// </para>
        /// </summary>
        /// <param name="sourceDocument">Information about the source document to use as input.</param>
        /// <returns>ConversionResult for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(ConversionSourceDocument, DestinationOptions)" />
        public Task<ConversionResult> OcrToPdfAsync(ConversionSourceDocument sourceDocument) =>
                            this.OcrToPdfAsync(sourceDocument, new OcrOptions());

        /// <summary>
        /// Perform OCR on pages of a single source document, producing a PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{ConversionSourceDocument}, DestinationOptions)" />, returning a single <see cref="ConversionResult" />.
        /// </para>
        /// </summary>
        /// <param name="sourceDocument">Information about the source document to use as input.</param>
        /// <param name="options">OCR options.</param>
        /// <returns>ConversionResult for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(ConversionSourceDocument, DestinationOptions)" />
        public Task<ConversionResult> OcrToPdfAsync(ConversionSourceDocument sourceDocument, OcrOptions options) =>
                            this.OcrToPdfAsync(new ConversionSourceDocument[] { sourceDocument }, options);

        /// <summary>
        /// Perform OCR on pages from a collection of source documents, producing a PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{ConversionSourceDocument}, DestinationOptions)" />, returning a single <see cref="ConversionResult" />.
        /// </para>
        /// </summary>
        /// <param name="sourceDocuments">Collection of source documents whose pages should be combined, in order, to form the output.</param>
        /// <returns>ConversionResult for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(IEnumerable{ConversionSourceDocument}, DestinationOptions)" />
        public Task<ConversionResult> OcrToPdfAsync(IEnumerable<ConversionSourceDocument> sourceDocuments) =>
                            this.OcrToPdfAsync(sourceDocuments, new OcrOptions());

        /// <summary>
        /// Perform OCR on pages from a collection of source documents, producing a PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{ConversionSourceDocument}, DestinationOptions)" />, returning a single <see cref="ConversionResult" />.
        /// </para>
        /// </summary>
        /// <param name="sourceDocuments">Collection of source documents whose pages should be combined, in order, to form the output.</param>
        /// <param name="options">OCR options.</param>
        /// <returns>ConversionResult for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(IEnumerable{ConversionSourceDocument}, DestinationOptions)" />
        public async Task<ConversionResult> OcrToPdfAsync(IEnumerable<ConversionSourceDocument> sourceDocuments, OcrOptions options)
        {
            return (await this.ConvertAsync(sourceDocuments, new DestinationOptions(DestinationFileFormat.Pdf)
            {
                PdfOptions = new PdfDestinationOptions
                {
                    Ocr = options,
                },
            })).Where(x => x.IsSuccess).Single();
        }
    }
}
