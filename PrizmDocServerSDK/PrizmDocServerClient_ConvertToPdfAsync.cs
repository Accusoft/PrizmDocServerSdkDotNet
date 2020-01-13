// This file defines the ConvertToPdfAsync methods which are convenience
// wrappers around ConvertAsync.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Conversion;

namespace Accusoft.PrizmDocServer
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "Documented in PrizmDocServerClient.cs")]
    public partial class PrizmDocServerClient
    {
        /// <summary>
        /// Convert a local file to PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{ConversionSourceDocument}, DestinationOptions)" />, returning a single <see cref="ConversionResult" />.
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
        /// <returns>ConversionResult for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(string, DestinationOptions)" />
        public Task<ConversionResult> ConvertToPdfAsync(string localFilePath, HeaderFooterOptions header = null, HeaderFooterOptions footer = null) =>
                            this.ConvertToPdfAsync(new ConversionSourceDocument(localFilePath), header, footer);

        /// <summary>
        /// Convert pages of a single source document to PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{ConversionSourceDocument}, DestinationOptions)" />, returning a single <see cref="ConversionResult" />.
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
        /// <returns>ConversionResult for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(ConversionSourceDocument, DestinationOptions)" />
        public Task<ConversionResult> ConvertToPdfAsync(ConversionSourceDocument sourceDocument, HeaderFooterOptions header = null, HeaderFooterOptions footer = null) =>
                            this.CombineToPdfAsync(new ConversionSourceDocument[] { sourceDocument }, header, footer);
    }
}
