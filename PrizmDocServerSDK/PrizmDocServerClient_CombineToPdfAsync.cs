// This file defines the CombineToPdfAsync method which is a convenience wrapper
// around ConvertAsync.
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Accusoft.PrizmDocServer.Conversion;

namespace Accusoft.PrizmDocServer
{
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1601:Partial elements should be documented", Justification = "Documented in PrizmDocServerClient.cs")]
    public partial class PrizmDocServerClient
    {
        /// <summary>
        /// Combine pages from a collection of source documents into a PDF.
        /// <para>
        /// Convenience wrapper for <see cref="ConvertAsync(IEnumerable{ConversionSourceDocument}, DestinationOptions)" />, returning a single <see cref="ConversionResult" />.
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
        /// <returns>ConversionResult for the created PDF.</returns>
        /// <seealso cref="ConvertAsync(string, DestinationOptions)" />
        public async Task<ConversionResult> CombineToPdfAsync(IEnumerable<ConversionSourceDocument> sourceDocuments, HeaderFooterOptions header = null, HeaderFooterOptions footer = null)
        {
            return (await this.ConvertAsync(sourceDocuments, new DestinationOptions(DestinationFileFormat.Pdf)
            {
                Header = header,
                Footer = footer,
            })).Where(x => x.IsSuccess).Single();
        }
    }
}
